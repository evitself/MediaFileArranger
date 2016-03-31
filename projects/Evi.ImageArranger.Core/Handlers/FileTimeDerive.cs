using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evi.ImageArranger.Core.Handlers
{
    public abstract class FileTimeDerive : IFileTimeDerive
    {
        protected IDictionary<string, string> PrefixMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            [".jpg"] = "IMG",
            [".jpeg"] = "IMG",
            [".png"] = "IMG",
            [".mp4"] = "VID",
            [".3gp"] = "VID",
        };

        public int Priority { get; set; }
        public abstract Tuple<DateTime, string, string> Derive(string name);
        public virtual string FilterFileName(string name)
        {
            var ext = Path.GetExtension(name);
            return PrefixMap.ContainsKey(ext) ? ext : null;
        }

        public static Tuple<DateTime, string, string> Run(string file, params IFileTimeDerive[] handlers)
        {
            return handlers
                .OrderBy(h => h.Priority)
                .Select(handler => handler.Derive(file))
                .FirstOrDefault(ret => ret != null);
        }
    }
}
