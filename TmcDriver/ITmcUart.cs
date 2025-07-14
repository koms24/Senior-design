using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeniorDesignFall2024.TmcDriver
{
    public interface ITmcUart
    {
        public Int32 UART_readWrite(byte[] data, int writeLength, int readLength);
    }
}
