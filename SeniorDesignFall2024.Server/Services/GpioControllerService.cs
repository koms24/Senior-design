using System.Collections.Concurrent;
using System.Device.Gpio;

namespace SeniorDesignFall2024.Server.Services
{
    public enum AppPinMode
    {
        Input = PinMode.Input,
        Output = PinMode.Output,
        InputPullDown = PinMode.InputPullDown,
        InputPullUp = PinMode.InputPullUp,
        Unkown = Int32.MaxValue
    }
    public readonly struct PinState : IEquatable<PinState>
    {
        private readonly byte _value;
        private PinState(byte value) => _value = value;

        public static PinState Opened => new PinState(1);
        public static PinState Closed => new PinState(2);
        public static PinState Unknown => new PinState(4);

        public static implicit operator PinState(int value) => value > 1 ? Unknown : ( value>=1 ? Opened : Closed );

        public static implicit operator PinState(bool value) => value ? Opened : Closed;
        public static implicit operator PinState(string value) => value.Equals("Opened", StringComparison.OrdinalIgnoreCase) ? Opened : (value.Equals("Closed", StringComparison.OrdinalIgnoreCase) ? Closed : Unknown);

        public static explicit operator byte(PinState value) => (byte)(value._value <= 2 ? (value._value==1 ? 1 : 0) : 4);
        public static explicit operator int(PinState value) => (int)(value._value <= 2 ? (value._value == 1 ? 1 : 0) : 4);
        public static explicit operator bool(PinState value) => value._value == 1 ? true : false;

        public bool Equals(PinState other) => other._value == _value;
        public override bool Equals(object? obj)
        {
            if(obj is PinState)
            {
                return Equals((PinState)obj);
            }
            return false;
        }
        public static bool operator ==(PinState left, PinState right) => left.Equals(right);
        public static bool operator !=(PinState left, PinState right) => !left.Equals(right);

        public static PinState operator !(PinState v) => v._value > 2 ? Unknown : ( v._value == 1 ? Closed : Opened );

        public override int GetHashCode() => _value.GetHashCode();

        public override string ToString() => _value > 2 ? "Unknown" : ( _value == 1 ? "Opened" : "Closed" );
    }
    public class GpioStatus
    {
        public static TimeSpan DefaultLockWait = new TimeSpan(0, 0, 5);
        public int PinNum { get; set; }
        public PinState State { get; set; } = PinState.Unknown;
        public AppPinMode Mode { get; set; } = AppPinMode.Unkown;
        public PinValue? Value { get; set; } = null;
        public DateTime? Timestamp { get; set; } = null;
        public DateTime? ValueTimestamp { get; set; } = null;
        public object Lock { get; set; } = new();

        public GpioStatus(int pinNum, GpioController controller) {
            PinNum = pinNum;
            State = controller.IsPinOpen(pinNum);
            if (State == PinState.Opened) {
                Mode = (AppPinMode)controller.GetPinMode(pinNum);
            }
            DateTime? dt = null;
            if (State == PinState.Opened) {
                Value = controller.Read(PinNum);
                dt = DateTime.Now;
                ValueTimestamp = dt;
            }
            Timestamp = dt ?? DateTime.Now;
        }
    }
    public class GpioControllerService
    {
        private ILogger<GpioControllerService> _logger; 
        protected GpioController gpioController { get; set; } = new();
        protected ConcurrentDictionary<int, GpioStatus> gpioStatusAry { get; set; } = new();

        public GpioControllerService(ILogger<GpioControllerService> logger)
        {
            _logger = logger;
            Enumerable.Range(0, gpioController.PinCount - 1).AsParallel().ForAll(pinNum => gpioStatusAry[pinNum] = new GpioStatus(pinNum, gpioController));
        }

        public void Write(int pinNum, PinValue pinValue)
        {
            var pin = gpioStatusAry[pinNum];
            if (pin.Mode != AppPinMode.Output)
                throw new Exception($"Pin Number: ({pinNum}), Must be set to output to set pin value");
            if (pin.State != PinState.Opened)
                throw new Exception($"Pin Number: ({pinNum}), Must be opened first!");
            bool locked = false;
            DateTime? dt = null;
            try
            {
                Monitor.TryEnter(pin.Lock, ref locked);
                if (locked) {
                    gpioController.Write(pinNum, pinValue);
                    pin.Value = pinValue;
                    dt = DateTime.Now;
                    pin.ValueTimestamp = dt;
                } else { 
                }
            } finally {
                pin.Timestamp = dt ?? DateTime.Now;
                if (locked)
                    Monitor.Exit(pin.Lock);
                else {
                    throw new Exception($"Pin Number: ({pinNum}), is in use, locked timed out");
                }
            }
        }

        public PinValue Read(int pinNum)
        {
            var pin = gpioStatusAry[pinNum];
            if (pin.State != PinState.Opened)
                throw new Exception($"Pin Number: ({pinNum}), Must be opened first!");
            bool locked = false;
            PinValue value = PinValue.Low;
            DateTime? dt = null;
            try
            {
                Monitor.TryEnter(pin.Lock, ref locked);
                if (locked)
                {
                    value = gpioController.Read(pinNum);
                    pin.Value = value;
                    dt = DateTime.Now;
                    pin.ValueTimestamp = dt;
                } else {
                }
            } finally {
                pin.Timestamp = dt ?? DateTime.Now;
                if (locked)
                    Monitor.Exit(pin.Lock);
                else {
                    throw new Exception($"Pin Number: ({pinNum}), is in use, locked timed out");
                }
            }
            return value;
        }

        public void SetPinLow(int pinNum) => Write(pinNum, PinValue.Low);
        public void SetPinHigh(int pinNum) => Write(pinNum, PinValue.High);

        public void OpenPin(int pinNum, PinMode mode) { 
            var pin = gpioStatusAry[pinNum];
            bool alreadyOpen = false;
            if(alreadyOpen = (pin.State == PinState.Opened)) {
                throw new Exception($"Pin Number: ({pinNum}), Is already open");
            } else { 
                bool locked = false;
                DateTime? dt = null;
                try {
                    Monitor.TryEnter(pin.Lock, ref locked);
                    if (locked) {
                        if(!(alreadyOpen = gpioController.IsPinOpen(pinNum))) {
                            gpioController.OpenPin(pinNum, mode);
                            pin.State = PinState.Opened;
                            pin.Mode = (AppPinMode)mode;
                            dt = DateTime.Now;
                            pin.Timestamp = dt;
                        }
                    } else { 
                    }
                } finally {
                    pin.Timestamp = dt ?? DateTime.Now;
                    if (locked && alreadyOpen) {
                        Monitor.Exit(pin.Lock);
                        throw new Exception($"Pin Number: ({pinNum}), Is already open");
                    } else if (locked) {
                        Monitor.Exit(pin.Lock);
                    } else if(alreadyOpen) {
                        throw new Exception($"Pin Number: ({pinNum}), Is already open");
                    } else {
                        throw new Exception($"Pin Number: ({pinNum}), is in use, locked timed out");
                    }
                }
            }
        }
        public void ClosePin(int pinNum)
        {
            var pin = gpioStatusAry[pinNum];
            bool alreadyOpen = true;
            bool locked = false;
            DateTime? dt = null;
            try
            {
                Monitor.TryEnter(pin.Lock, ref locked);
                if (locked)
                {
                    if (alreadyOpen = gpioController.IsPinOpen(pinNum)) {
                        gpioController.ClosePin(pinNum);
                    } else {
                        //Warning pin already closed
                    }
                    pin.State = PinState.Closed;
                    dt = DateTime.Now;
                    pin.Timestamp = dt;
                } else {
                }
            } finally {
                pin.Timestamp = dt ?? DateTime.Now;
                if (locked) {
                    Monitor.Exit(pin.Lock);
                } else {
                    throw new Exception($"Pin Number: ({pinNum}), is in use, locked timed out");
                }
            }
        }

        public GpioStatus GetPinStatus(int pinNum) => gpioStatusAry[pinNum];















        public void Test()
        {

        }
    }
}
