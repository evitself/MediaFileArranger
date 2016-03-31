using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Evi.ImageArranger.Core.Handlers
{
    public class DeriveFromFileTime : FileTimeDerive
    {
        public static DateTime Threshold = new DateTime(2000, 1, 1);
        public override Tuple<DateTime, string, string> Derive(string name)
        {
            var ext = FilterFileName(name);
            if (ext == null)
            {
                return null;
            }
            DateTime[] dts = { File.GetCreationTime(name), File.GetLastAccessTime(name), File.GetLastWriteTime(name) };
            var minDate = dts.Min();
            var ajusted = dts.OrderBy(dt => dt).FirstOrDefault(dt => dt > Threshold);

            return new Tuple<DateTime, string, string>(dts.Any(dt => dt > Threshold) ? ajusted : minDate, PrefixMap[ext], Path.GetFileName(name));
        }
    }
}
