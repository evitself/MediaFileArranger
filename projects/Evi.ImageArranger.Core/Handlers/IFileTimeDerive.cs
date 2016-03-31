using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evi.ImageArranger.Core.Handlers
{
    public interface IFileTimeDerive
    {
        int Priority { get; }
        Tuple<DateTime, string, string> Derive(string name);
    }
}
