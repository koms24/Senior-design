using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeniorDesignFall2024.Server.Shared.Interfaces
{
    public interface IStartupConfigOptions
    {
        public bool EnableZwaveOpenHab { get; }
        public bool EnableTmcDriver { get; }
        public bool EnableDb { get; }
        public bool EnableGpio { get; }
        public bool EnableImaging { get; }
        public bool EnableCamera { get; }
    }
}
