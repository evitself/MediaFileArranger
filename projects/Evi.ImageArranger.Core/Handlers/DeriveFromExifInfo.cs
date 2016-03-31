using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExifLib;

namespace Evi.ImageArranger.Core.Handlers
{
    public class DeriveFromExifInfo : FileTimeDerive
    {
        public override Tuple<DateTime, string, string> Derive(string name)
        {
            var ext = FilterFileName(name);
            if (ext == null)
            {
                return null;
            }
            ext = ext.ToLower();
            if (ext != ".jpg" || ext != ".jpeg")
            {
                return null;
            }
            var prefix = PrefixMap[ext];
            using (var reader = new ExifReader(name))
            {
                DateTime datePictureTaken;
                if (reader.GetTagValue(ExifTags.DateTimeDigitized, out datePictureTaken))
                {
                    return new Tuple<DateTime, string, string>(datePictureTaken, prefix, Path.GetFileName(ext));
                }
            }
            return null;
        }
    }
}
