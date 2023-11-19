using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuna.EventSourcing.Core
{
    public interface IHaveVersion
    {
        public int Version { get; set; }
    }
}
