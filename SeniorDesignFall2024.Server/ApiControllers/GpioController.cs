using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SeniorDesignFall2024.Server.Services;
using SeniorDesignFall2024.Server.Shared.Options;
using System.Device.Gpio;

namespace SeniorDesignFall2024.Server.ApiControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GpioController : ControllerBase
    {
        private bool _enabled = true;
        //private readonly ILogger<GpioController> _logger;
        private readonly GpioControllerService? _gpioController;

        public GpioController(IOptions<StartupConfigOptions> opts, GpioControllerService? gpioController = null)
        {
            _enabled = opts.Value.EnableGpio;
            //_logger = logger;
            _gpioController = gpioController;
        }

        private bool checkEnabled()
        {
            if (!_enabled)
                throw new Exception("Gpio Feature Disabled, Check appsettings file");
            return _enabled;
        }

        [HttpGet("{pinNum}")]
        public IActionResult GetGpioStatus(int pinNum) {
            GpioStatus? status = null;
            Exception? ex = null;
            try {
                checkEnabled();
                status = _gpioController.GetPinStatus(pinNum);
            } catch (Exception _ex) { 
                ex = _ex;
            }
            return status != null ? Ok(status) : Problem((ex ?? new Exception("Unknown Server Error")).ToString());
        }

        [HttpPost("{pinNum}/Open/{mode}")]
        public IActionResult OpenGpioPin(int pinNum, System.Device.Gpio.PinMode mode) {
            Exception? ex = null;
            try {
                checkEnabled();
                _gpioController.OpenPin(pinNum, mode);
            } catch (Exception _ex) { 
                ex= _ex;
            }
            return ex == null ? Ok() : Problem(ex.ToString());
        }

        [HttpPost("{pinNum}/Close")]
        public IActionResult CloseGpioPin(int pinNum)
        {
            Exception? ex = null;
            try
            {
                checkEnabled();
                _gpioController.ClosePin(pinNum);
            }
            catch (Exception _ex)
            {
                ex = _ex;
            }
            return ex == null ? Ok() : Problem(ex.ToString());
        }


        [HttpGet("{pinNum}/Read")]
        public IActionResult ReadGpioPin(int pinNum)
        {
            PinValue? pinValue = null;
            Exception? ex = null;
            try {
                checkEnabled();
                pinValue = _gpioController.Read(pinNum);
            } catch (Exception _ex) {
                ex = _ex;
            }
            return pinValue != null ? Ok((int)pinValue) : Problem((ex ?? new Exception("Unknown Server Error")).ToString());
        }


        [HttpGet("{pinNum}/Write/{value}")]
        public IActionResult WriteGpioPin(int pinNum, int value)
        {
            Exception? ex = null;
            try {
                checkEnabled();
                _gpioController.Write(pinNum, value);
            } catch (Exception _ex) {
                ex = _ex;
            }
            return ex == null ? Ok() : Problem(ex.ToString());
        }


        [HttpGet("{pinNum}/SetPinHigh")]
        public IActionResult SetPinHigh(int pinNum)
        {
            Exception? ex = null;
            try
            {
                checkEnabled();
                if(_gpioController.GetPinStatus(pinNum).Mode != AppPinMode.Output)
                    _gpioController.OpenPin(pinNum, PinMode.Output);
                _gpioController.SetPinHigh(pinNum);
            }
            catch (Exception _ex)
            {
                ex = _ex;
            }
            return ex == null ? Ok() : Problem(ex.ToString());
        }


        [HttpGet("{pinNum}/SetPinLow")]
        public IActionResult SetPinLow(int pinNum)
        {
            Exception? ex = null;
            try
            {
                checkEnabled();
                if (_gpioController.GetPinStatus(pinNum).Mode != AppPinMode.Output)
                    _gpioController.OpenPin(pinNum, PinMode.Output);
                _gpioController.SetPinLow(pinNum);
            }
            catch (Exception _ex)
            {
                ex = _ex;
            }
            return ex == null ? Ok() : Problem(ex.ToString());
        }

    }
}
