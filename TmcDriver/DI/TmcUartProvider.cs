using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeniorDesignFall2024.TmcDriver.DI
{
    public class TmcUartProvider
    {
        private TmcUartOptions[] options;
        private TmcUart[] tmcUarts;
        private Dictionary<string, TmcUart> uartNameMap;
        private Dictionary<string, TmcUart> uartUidMap;
        public TmcUartProvider(IOptions<TmcDriverOptions> opts)
        {
            options = opts.Value.TmcUarts;
            tmcUarts = options.Select(opt => new TmcUart(opt)).ToArray();
            uartNameMap = tmcUarts.ToDictionary(o => o.Options.Name);
            uartUidMap = tmcUarts.ToDictionary(o => o.Options.Uid);
        }

        public TmcUart this[int index] { get { return tmcUarts[index]; } }
        public TmcUart this[string uid_name]
        {
            get
            {
                return TryToGet(uid_name) ?? throw new InvalidOperationException();
            }
        }
        public TmcUart? TryToGet(string uid_name)
        {
            if (uartUidMap.ContainsKey(uid_name))
                return uartUidMap[uid_name];
            else if (uartNameMap.ContainsKey(uid_name))
                return uartNameMap[uid_name];
            else 
                return null;
        }

        public int Length { get { return tmcUarts.Length; } }
    }
}
