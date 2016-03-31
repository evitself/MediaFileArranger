using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evi.ImageArranger.Core
{
    public class Utils
    {
        public static IEnumerable<string> SearchFiles(string root)
        {
            var queue = new Queue<string>();
            queue.Enqueue(root);
            while (queue.Count > 0)
            {
                var folder = queue.Dequeue();
                foreach (var file in Directory.GetFiles(folder))
                {
                    yield return file;
                }
                foreach (var dir in Directory.GetDirectories(folder))
                {
                    queue.Enqueue(dir);
                }
            }
        }
    }
}
