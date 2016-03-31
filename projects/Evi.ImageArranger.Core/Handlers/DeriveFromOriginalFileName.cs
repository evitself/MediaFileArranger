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
    public class DeriveFromOriginalFileName : FileTimeDerive
    {
        protected Regex DateRegex = new Regex(@"20[01][0-9]((0[1-9])|(1[012]))(([012][0-9])|(3[01]))", RegexOptions.Compiled);
        protected Regex DateTimeRegex = new Regex(@"20[01][0-9]((0[1-9])|(1[012]))(([012][0-9])|(3[01]))(([0-1][0-9])|2[0-3])([0-5][0-9])([0-5][0-9])", RegexOptions.Compiled);
        public override Tuple<DateTime, string, string> Derive(string name)
        {
            var file = FilterFileName(name);
            if (file == null)
            {
                return null;
            }
            var ext = Path.GetExtension(file);
            if (!PrefixMap.ContainsKey(ext))
            {
                return null;
            }
            var fileName = Path.GetFileName(file);
            DateTime fileTime;
            var dateTimeMatch = DateTimeRegex.Match(fileName);
            if (dateTimeMatch.Success &&
                DateTime.TryParseExact(dateTimeMatch.Value, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out fileTime))
            {
                return new Tuple<DateTime, string, string>(fileTime, PrefixMap[ext], Path.GetFileName(file));
            }

            var dateMatch = DateRegex.Match(fileName);
            if (!dateMatch.Success ||
                !DateTime.TryParseExact(dateMatch.Value, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out fileTime))
            {
                return null;
            }

            return new Tuple<DateTime, string, string>(fileTime, PrefixMap[ext], Path.GetFileName(file));
        }
    }
}
