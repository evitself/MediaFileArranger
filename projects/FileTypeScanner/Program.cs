using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Evi.ImageArranger.Core;
using Evi.ImageArranger.Core.Handlers;
using Newtonsoft.Json;

namespace FileTypeScanner
{
    class Program
    {
        static void Main(string[] args)
        {
            CopyFileAndOrganizeByTime();
        }

        static void SeachFiles()
        {
            const string root = @"C:\Users\evits\OneDrive";
            var fileTypeMapping = new ConcurrentDictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            int count = 0;

            Parallel.ForEach(Utils.SearchFiles(root), new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, file =>
            {
                var ext = Path.GetExtension(file);
                if (string.IsNullOrWhiteSpace(ext))
                {
                    return;
                }
                fileTypeMapping.AddOrUpdate(ext, 1, (k, v) => v + 1);
                Interlocked.Increment(ref count);
                if (count % 100 == 0)
                {
                    Console.Write($"\rProcessed: {count}");
                }
            });
            Console.WriteLine($"\rProcessed: {count}");
            var content = JsonConvert.SerializeObject(fileTypeMapping.OrderByDescending(k => k.Value).ToArray());
            File.WriteAllText("result.json", content);
        }

        static void DeriveFileTime()
        {
            const string root = @"C:\Users\evits\OneDrive";
            var exifHandler = new DeriveFromExifInfo() { Priority = 10 };
            var fileNameHandler = new DeriveFromOriginalFileName() { Priority = 20 };
            var fileTimeHandler = new DeriveFromFileTime() { Priority = 30 };

            int count = 0;
            int processed = 0;
            var resultPool = new List<Tuple<DateTime, string, string>>();

            Parallel.ForEach(Utils.SearchFiles(root), new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, file =>
            {
                var tup = FileTimeDerive.Run(file, exifHandler, fileNameHandler, fileTimeHandler);
                Interlocked.Increment(ref count);
                if (tup == null)
                {
                    return;
                }

                resultPool.Add(tup);
                Interlocked.Increment(ref processed);
                if (count % 100 == 0)
                {
                    Console.Write($"\rProcessed: {processed} Total:{count}");
                }
            });

            Console.WriteLine($"\rProcessed: {processed} Total:{count}");

            var content = JsonConvert.SerializeObject(resultPool.OrderBy(k => k.Item1).ToArray(), Formatting.Indented);
            File.WriteAllText("result_dt.json", content);
        }
        static void CopyFileAndOrganizeByTime()
        {
            const string root = @"C:\Users\evits\OneDrive";
            const string export = @"d:\export_photos\";

            var exifHandler = new DeriveFromExifInfo() { Priority = 10 };
            var fileNameHandler = new DeriveFromOriginalFileName() { Priority = 20 };
            var fileTimeHandler = new DeriveFromFileTime() { Priority = 30 };

            int count = 0;
            int processed = 0;

            foreach (var file in Utils.SearchFiles(root))
            {
                var tup = FileTimeDerive.Run(file, exifHandler, fileNameHandler, fileTimeHandler);
                count++;
                if (tup == null)
                {
                    continue;
                }

                var ext = Path.GetExtension(file);
                var exportFolder = Path.Combine(export, $"{tup.Item2}", $"{tup.Item1.Year}", $"{tup.Item1.Month:00}");
                var prefix = $"{tup.Item2}_{tup.Item1:yyyyMMddHHmmss}";
                var fullPath = Path.Combine(exportFolder, $"{prefix}{ext}");
                if (File.Exists(fullPath))
                {
                    fullPath = Path.Combine(exportFolder, $"{prefix}_{Guid.NewGuid().ToString("n")}{ext}");
                }

                var folder = Path.GetDirectoryName(fullPath);
                if (string.IsNullOrWhiteSpace(folder))
                {
                    continue;
                }
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                File.Copy(file, fullPath);

                processed++;
                if (count % 100 == 0)
                {
                    Console.Write($"\rProcessed: {processed} Total:{count}");
                }
            }

            Console.WriteLine($"\rProcessed: {processed} Total:{count}");
        }
    }
}
