using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeniorDesignFall2024.TmcDriver
{
    public class TmcDriverIcOptions
    {
        private static int _cnt = 0;
        private string _uid = _cnt++.ToString();
        public string Uid { get { return _uid; } set { _uid = value; } }
        private string? _name = null;
        public string Name { get { return _name == null ? (_name = $"TMC Uart {Uid}") : _name; } set { _name = value; } }
        private string? _groupName = null;
        public string GroupName { get { return _groupName != null ? _groupName : Name; } set { _groupName = value; } }
        public byte DefaultUartAddress { get; set; }
        public int GpioEnablePinNum { get; set; }
        public string? UartUid { get; set; }
        public int? UartRxPinNum { get; set; }
        public int? UartTxPinNum { get; set; }
        public byte? AltUartAddress { get; set; }
        public int? GpioAddressPinNum_MS1_AD0 { get; set; }
        public int? GpioAddressPinNum_MS2_AD1 { get; set; }
        public int? GpioDiagPinNum{ get; set; }
        public int? GpioIndexPinNum { get; set; }
        public int? GpioStepPinNum { get; set; }

        public byte GetUartAddress()
        {
            return DefaultUartAddress;
        }
    }
}
