using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SeniorDesignFall2024.TmcDriver
{
    public class TmcUart : ITmcUart, IDisposable
    {
        private int ShortWriteDelayTicks;
        private int LongWriteDelayTicks;
        private Stopwatch sw = new();

        private TmcUartOptions tmcUartOptions;

        private System.IO.Ports.SerialPort? _port = null;

        protected System.IO.Ports.SerialPort SerialPort { 
            get { 
                if(_port != null) return _port;
                return (_port = new(tmcUartOptions.PortName, tmcUartOptions.BaudRate, tmcUartOptions.Parity, tmcUartOptions.DataBits, tmcUartOptions.StopBits));
            }
        }

        public TmcUartOptions Options {  get { return tmcUartOptions; } }

        public TmcUart(TmcUartOptions opts) {
            tmcUartOptions = opts;
            int us = (32 + 1 + 1 + (15 * (2 * 8))) * 1000000 / opts.BaudRate;     //short frame data bits + start bit + stop bit + 2*(DELAYTIME * (8 bit times)) * convert to us / baudrate
            ShortWriteDelayTicks = (int)(new TimeSpan(0,0,0,0,0,us)).Ticks;
            us = (64 + 1 + 1 + (15 * (2 * 8))) * 1000000 / opts.BaudRate;
            LongWriteDelayTicks = (int)(new TimeSpan(0, 0, 0, 0, 0, us)).Ticks;
        }

        public Int32 UART_readWrite(byte[] data, int writeLength, int readLength) {
            var s = SerialPort;
            if (!s.IsOpen)
                s.Open();

            int writeDelay = writeLength <= 4 ? ShortWriteDelayTicks : LongWriteDelayTicks;

            s.DiscardOutBuffer();
            s.DiscardInBuffer();
            try {
                //sw.Restart();
                s.Write(data, 0, writeLength);
                //sw.Stop();
                //Console.WriteLine($"Write Op: ${sw.ElapsedTicks} ticks");
            } catch (Exception ex) {
                var e = ex;
                Console.WriteLine(e);
            }

            sw.Restart();
            while (sw.ElapsedTicks < writeDelay);
            sw.Stop();
            //Thread.Sleep(20000/tmcUartOptions.BaudRate);

            if(readLength <= 0) { 
                return 0;
            }

            s.Read(data, 0, readLength);

            return 0;
        }

        public void Open()
        {
            if (_port != null && !_port.IsOpen)
                _port.Open();
        }

        public void Close()
        {
            if(_port != null && _port.IsOpen)
                _port.Close();
        }

        public void Dispose()
        {
            if(_port != null) {
                Close();
                _port.Dispose();
            }
        }
    }
}
