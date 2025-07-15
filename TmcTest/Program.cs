using SeniorDesignFall2024.TmcDriver;

namespace TmcTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.ReadLine();
            var opts = new TmcUartOptions
            {
                BaudRate = 115200
            };
            var uart = new TmcUart(opts);
            uart.Open();
            TmcDriverWrapper.tmcUart = uart;
            TmcDriverWrapper.Init(1, new byte[] { 0 });

            int writeDelay = 2;
            TmcDriverWrapper.TmcWriteRegister(0, 1, 3);
            TmcDriverWrapper.TmcWriteRegister(0, 0, 12);
            TmcDriverWrapper.TmcWriteRegister(0, 16, 659977);
            TmcDriverWrapper.TmcWriteRegister(0, 108, 335577732);
            TmcDriverWrapper.TmcWriteRegister(0, 112, -938668508);
            TmcDriverWrapper.TmcWriteRegister(0, 34, 8948);
            TmcDriverWrapper.RunFor(5000);
            TmcDriverWrapper.TmcWriteRegister(0, 34, 0);


            //Thread.Sleep(writeDelay);
            //TmcDriverWrapper.TmcWriteRegister(0, 0, 448);
            //Thread.Sleep(writeDelay);
            //TmcDriverWrapper.TmcWriteRegister(0, 108, 335609939);
            //Thread.Sleep(writeDelay);
            //TmcDriverWrapper.TmcWriteRegister(0, 0, 448);
            //Thread.Sleep(writeDelay);
            //TmcDriverWrapper.TmcWriteRegister(0, 0, 448);
            //Thread.Sleep(writeDelay);
            //TmcDriverWrapper.TmcWriteRegister(0, 0, 448);
            //Thread.Sleep(writeDelay);
            //TmcDriverWrapper.TmcWriteRegister(0, 34, 8948);
            //Thread.Sleep(5000);
            //TmcDriverWrapper.TmcWriteRegister(0, 34, 0);
            //Thread.Sleep(2000);
        }
    }
}
