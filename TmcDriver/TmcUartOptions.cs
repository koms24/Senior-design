using Iot.Device.Mcp23xxx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeniorDesignFall2024.TmcDriver
{
    public class TmcUartOptions
    {
        public static string CollectionSectionName = "TmcUarts";
        private static int _uartCnt = 0;
        private string _uid = _uartCnt++.ToString();
        public string Uid { get { return _uid; } set { _uid = value; } }
        private string? _name = null;
        public string Name { get { return _name == null ? (_name = $"TMC Uart {Uid}") : _name; } set { _name = value; } }
        public string PortName { get; set; } = "/dev/ttyAMA3";
        public int BaudRate { get; set; } = 115200;
        public System.IO.Ports.Parity Parity { get; set; } = System.IO.Ports.Parity.None;
        public int DataBits { get; set; } = 8;
        public System.IO.Ports.StopBits StopBits { get; set; } = System.IO.Ports.StopBits.One;
    }
}
