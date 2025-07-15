using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using SeniorDesignFall2024.TmcDriver.HAL.Tmc2209.Registers;

namespace SeniorDesignFall2024.TmcDriver
{
    public class TmcDriverIc : IDisposable
    {
        private Dictionary<byte, int> _regCache = new Dictionary<byte, int>();

        public static readonly byte FullStepsPerRevolution = 200;

        private Stopwatch stopwatch = new Stopwatch();
        private TmcDriverIcOptions options;
        public TmcDriverIcOptions Options { get { return options; } }
        private TmcUart uart;
        private int icId;
        public int IcId { get { return icId; } }
        private GpioController gpioController;
        private GpioPin enablePin;
        private GpioPin? ad0Pin = null;
        private GpioPin? ad1Pin = null;
        private bool driverEnabled = false;
        public TmcDriverIc(TmcDriverIcOptions _opts, TmcUart _uart, int _icId, GpioController _gpioController)
        {
            options = _opts;
            uart = _uart;
            icId = _icId;
            gpioController = _gpioController;
            ConfigAddress();
            SetAddress();
        }

        private bool _isInit = false;
        private bool disposedValue;

        public bool IsInit { get { return _isInit; } }
        public bool IsEnabled { get { return driverEnabled; } }

        private void WriteReg(short id, General address, int value) => WriteReg(id, (byte)address, value);
        private void WriteReg(short id, Velocity address, int value) => WriteReg(id, (byte)address, value);
        private void WriteReg(short id, StallGuard address, int value) => WriteReg(id, (byte)address, value);
        private void WriteReg(short id, Sequencer address, int value) => WriteReg(id, (byte)address, value);
        private void WriteReg(short id, Chopper address, int value) => WriteReg(id, (byte)address, value);
        private void WriteReg(short id, byte address, int value) {
            TmcDriverWrapper.TmcWriteRegister(id, address, value);
            _regCache[address] = value;
        }
        private int ReadReg(short id, byte address) {
            int v = TmcDriverWrapper.TmcReadRegister(id, address);
            _regCache[address] = v;
            return v;
        }

        private void SetCurrent(double run_current_ma, double hold_ratio, byte delay_multiplier = 10)
        {
            byte irun = (byte)(run_current_ma * 18.3848 - 1);
            WriteReg((Int16)icId, Velocity.IHOLD_IRUN, (new IHOLD_IRUN(0)
            {
                Irun = irun,
                Ihold = (byte)(irun * hold_ratio),
                Iholddelay = delay_multiplier
            }).Val);
        }

        public void ConfigAddress()
        {
            if(options.GpioAddressPinNum_MS1_AD0 != null)
                ad0Pin = gpioController.OpenPin((int)options.GpioAddressPinNum_MS1_AD0, PinMode.Output);
            if (options.GpioAddressPinNum_MS2_AD1 != null)
                ad1Pin = gpioController.OpenPin((int)options.GpioAddressPinNum_MS2_AD1, PinMode.Output);
        }

        public void ReleaseAddressPins()
        {
            if (ad0Pin != null && options.GpioAddressPinNum_MS1_AD0 != null)
                gpioController.ClosePin((int)options.GpioAddressPinNum_MS1_AD0);
            if (ad1Pin != null && options.GpioAddressPinNum_MS2_AD1 != null)
                gpioController.ClosePin((int)options.GpioAddressPinNum_MS2_AD1);
        }

        public void SetAddress(bool useAlt = false)
        {
            if (useAlt && options.AltUartAddress == null) return;
            byte addr = (useAlt ? options.AltUartAddress : options.DefaultUartAddress) ?? options.DefaultUartAddress;
            if(ad0Pin != null)
            {
                ad0Pin.Write((addr & 0x01) == 0x01 ? PinValue.High : PinValue.Low);
            }
            if (ad1Pin != null)
            {
                ad1Pin.Write((addr & 0x02) == 0x02 ? PinValue.High : PinValue.Low);
            }
        }

        public void Init()
        {
            Int16 id = (Int16)icId;
            enablePin = gpioController.OpenPin(options.GpioEnablePinNum, PinMode.Output, PinValue.High);
            //WriteReg(id, General.GSTAT, 3);
            WriteReg(id, General.GSTAT, (new GSTAT(0)
            {
                Reset = 1
                //, DrvErr = 1
            }).Val);
            //WriteReg(id, General.GCONF, 12);
            WriteReg(id, General.GCONF, (new GCONF(0)
            {
                PdnDisable = 1,
                MstepRegSelect = 1,
                MultistepFilt = 1
            }).Val);
            //WriteReg(id, Velocity.IHOLD_IRUN, 659977);
            SetCurrent(1.06, 0.6);
            //WriteReg(id, Chopper.CHOPCONF, 335577732);
            WriteReg(id, Chopper.CHOPCONF, (new CHOPCONF(0)
            {
                Toff = 4,
                Hend = 5,
                Tbl = 1,
                Intpol = 1,
            }).Val);
            //WriteReg(id, Chopper.PWMCONF, -938668508);
            WriteReg(id, Chopper.PWMCONF, (new PWMCONF(0)
            {
                PwmOfs = 36,
                PwmGrad = 13,
                PwmFreq = 1,
                PwmAutoscale = 1,
                PwmAutograd = 1,
                PwmReg = 8,
                PwmLim = 12
            }).Val);
            //WriteReg(id, Velocity.VACTUAL, 8948);

            WriteReg(id, Velocity.VACTUAL, 0);
            EnableDriver();
            Thread.Sleep(150);
            DisableDriver();

            _isInit = true;
        }

        public VACTUAL Get(bool forceUpdate = false)
        {
            if (forceUpdate || !_regCache.ContainsKey((int)Velocity.VACTUAL))
                return new VACTUAL(ReadReg((Int16)icId, (byte)Velocity.VACTUAL));
            else
                return new VACTUAL(_regCache[(byte)Velocity.VACTUAL]);
        }

        private double getMicrostepsPerSecond(double rps)
        {
            return rps * FullStepsPerRevolution * 256;
        }

        public void SetSpeedRps(double rps) {
            int va_val = 0;
            if(true)    //TODO: check if internal oscilator
            {
                va_val = (int)(getMicrostepsPerSecond(rps) / 0.715);
            } else { 
            
            }
            WriteReg((Int16)icId, (byte)Velocity.VACTUAL, va_val);
        }

        public void RunFor(TimeSpan time, double speed = 1.0)
        {
            if (!_isInit)
                throw new Exception();
            SetSpeedRps(speed);
            bool enState = driverEnabled;
            if (time.TotalMilliseconds < 60) {
                var ticks = time.Ticks;
                if (!enState)
                    EnableDriver();
                stopwatch.Restart();
                while (stopwatch.ElapsedTicks < ticks);
                stopwatch.Stop();
            } else {
                if (!enState)
                    EnableDriver();
                Thread.Sleep((int)time.TotalMilliseconds);
            }
            if(!enState)
                DisableDriver();
        }

        public double CalculateTicksFromStepsForSpeed(int steps, double speed)
        {
            double avg_rps_speed = steps < 0 ? -1 : 1;
            double rotations = steps / 200;
            rotations += rotations / 200;
            return (rotations * 1000000 / avg_rps_speed) * TimeSpan.TicksPerMicrosecond;
        }

        public void Move(int steps, double speed = 1.0)
        {
            if (steps == 0) return;
            //double avg_rps_speed = steps < 0 ? -1 : 1;
            //double rotations = steps / 200;
            //rotations += rotations / 200;
            long max_time_ticks = (long)CalculateTicksFromStepsForSpeed(steps, speed);
            TimeSpan[] times = new TimeSpan[4];
            SetSpeedRps(0);
            bool enState = driverEnabled;
            if (!enState)
                EnableDriver();
            stopwatch.Restart();
            SetSpeedRps(speed);
            times[0] = stopwatch.Elapsed;
            while ((times[1] = stopwatch.Elapsed).Ticks < max_time_ticks);
            SetSpeedRps(0);
            times[2] = stopwatch.Elapsed;
            if (!enState)
                DisableDriver();
            times[3] = stopwatch.Elapsed;
            Console.WriteLine(times.Select(a => a.TotalMilliseconds).Aggregate("", (acc, v) => acc + $"{v}us, "));
        }

        public void EnableDriver()
        {
            enablePin.Write(PinValue.Low);
            driverEnabled = true;
        }

        public void DisableDriver()
        {
            enablePin.Write(PinValue.High);
            driverEnabled = false;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                ReleaseAddressPins();
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~TmcDriverIc()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
