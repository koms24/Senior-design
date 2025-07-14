using System.Collections.Concurrent;
using System.Device.Gpio;
using System.Runtime.InteropServices;

namespace SeniorDesignFall2024.TmcDriver
{
    public static class TmcDriverWrapper
    {
        public static ITmcUart? tmcUart = null;

        public static ConcurrentDictionary<Int16, ITmcUart> tmcUartMap = new();

        const string LIB_PATH = "./libtmcapi.so";

        [DllImport(LIB_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern void InitTmcDriver(Int16 numIc, byte[] icAddresses);

        [DllImport(LIB_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DeinitTmcDriver();



        //[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate Int32 UartInterfaceDelegate(Int16 icId, IntPtr data, Int32 writeLength, Int32 readLength);

        [DllImport(LIB_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetUartInterfaceCallback([MarshalAs(UnmanagedType.FunctionPtr)]UartInterfaceDelegate cb);

        public static Int32 UartInterface(Int16 icId, IntPtr data, Int32 writeLength, Int32 readLength) 
        {
            ITmcUart? uart = null;
            tmcUartMap.TryGetValue(icId, out uart);
            if (uart == null)
                uart = tmcUart ?? throw new ArgumentNullException(nameof(uart));
            Int32 size = Int32.Max(writeLength, readLength);
            byte[] _data = new byte[size];
            Marshal.Copy(data, _data, 0, writeLength);
            if(uart == null) {
                //throw new NullReferenceException(nameof(tmcUart));
                return -1;
            }
            var result = uart.UART_readWrite(_data, writeLength, readLength);
            if (readLength > 0)
                Marshal.Copy(_data, 0, data, readLength);
            return result;
        }
        public static UartInterfaceDelegate uartInterfaceHandler = UartInterface;



        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void ErrorDelegate(int errorCode);

        [DllImport(LIB_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetErrorCallback(ErrorDelegate cb);
        public static void Error(int errorCode) {

        }
        public static ErrorDelegate errorHandler = Error;



        [DllImport(LIB_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern void TmcWriteRegister(Int16 icId, byte address, Int32 value);

        [DllImport(LIB_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 TmcReadRegister(Int16 icId, byte address);



        public static void Init(Int16 numIc, byte[] icAddresses) {
            SetUartInterfaceCallback(uartInterfaceHandler);
            SetErrorCallback(errorHandler);
            InitTmcDriver(numIc, icAddresses);
        }
        public static void Deinit() {
            DeinitTmcDriver();
        }

        public static void RunFor(int time) {
            var c = new GpioController();
            var p = c.OpenPin(21, PinMode.Output, PinValue.High);
            var sw = new System.Diagnostics.Stopwatch();
            Thread.Sleep(100);
            p.Write(PinValue.High);
            sw.Restart();
            p.Write(PinValue.Low);
            sw.Stop();
            Console.WriteLine($"Pin Time: {sw.ElapsedTicks} ticks");
            Thread.Sleep((int)time);
            sw.Restart();
            p.Write(PinValue.High);
            sw.Stop();
            Console.WriteLine($"Pin Time: {sw.ElapsedTicks} ticks");
            c.ClosePin(21);
        }
    }
}
