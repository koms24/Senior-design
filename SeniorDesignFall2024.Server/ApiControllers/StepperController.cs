using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SeniorDesignFall2024.Server.Services.OpenHab;
using SeniorDesignFall2024.Server.Shared.Options;
using SeniorDesignFall2024.TmcDriver;

namespace SeniorDesignFall2024.Server.ApiControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StepperController : ControllerBase
    {
        private bool _enabled = true;
        private readonly SeniorDesignFall2024.TmcDriver.TmcDriver? _tmcDriver;

        public StepperController(
            IOptions<StartupConfigOptions> opts,
            SeniorDesignFall2024.TmcDriver.TmcDriver? driver = null)
        {
            _enabled = opts.Value.EnableTmcDriver;
            _tmcDriver = driver;
        }

        private bool checkEnabled()
        {
            if (!_enabled)
                throw new Exception("TMC Driver Feature Disabled, Check appsettings file");
            return _enabled;
        }

        [HttpGet("{stepper_uid}/Init")]
        public async Task<IActionResult> Init(string stepper_uid)
        {
            Exception? ex = null;
            try
            {
                checkEnabled();
                var t = Task.Run(() => _tmcDriver?.Init(stepper_uid));
                await t.WaitAsync(CancellationToken.None);
            }
            catch (Exception _ex)
            {
                ex = _ex;
            }
            return ex == null ? Ok() : Problem(ex.ToString());
        }

        [HttpGet("{stepper_uid}/RunFor/{milliseconds}")]
        public async Task<IActionResult> RunFor(string stepper_uid, int milliseconds)
        {
            Exception? ex = null;
            try
            {
                checkEnabled();
                var t = Task.Run(() => _tmcDriver?.RunFor(stepper_uid, milliseconds));
                await t.WaitAsync(CancellationToken.None);
            }
            catch (Exception _ex)
            {
                ex = _ex;
            }
            return ex == null ? Ok() : Problem(ex.ToString());
        }

        [HttpGet("{stepper_uid}/RunFor/{milliseconds}/{speed}")]
        public async Task<IActionResult> RunFor(string stepper_uid, int milliseconds, double speed)
        {
            Exception? ex = null;
            try
            {
                checkEnabled();
                var t = Task.Run(() => _tmcDriver?.RunFor(stepper_uid, milliseconds, speed));
                await t.WaitAsync(CancellationToken.None);
            }
            catch (Exception _ex)
            {
                ex = _ex;
            }
            return ex == null ? Ok() : Problem(ex.ToString());
        }

        [HttpGet("{stepper_uid}/Move/{steps}")]
        public async Task<IActionResult> Move(string stepper_uid, int steps)
        {
            Exception? ex = null;
            try
            {
                checkEnabled();
                var t = Task.Run(() => _tmcDriver?.Move(stepper_uid, steps));
                await t.WaitAsync(CancellationToken.None);
            }
            catch (Exception _ex)
            {
                ex = _ex;
            }
            return ex == null ? Ok() : Problem(ex.ToString());
        }

        [HttpGet("{stepper_uid}/Move/{steps}/{speed}")]
        public async Task<IActionResult> Move(string stepper_uid, int steps, double speed)
        {
            Exception? ex = null;
            try
            {
                checkEnabled();
                var t = Task.Run(() => _tmcDriver?.Move(stepper_uid, steps, speed));
                await t.WaitAsync(CancellationToken.None);
            }
            catch (Exception _ex)
            {
                ex = _ex;
            }
            return ex == null ? Ok() : Problem(ex.ToString());
        }

        [HttpGet("{group_name}/GroupRunFor/{milliseconds}")]
        public async Task<IActionResult> GroupRunFor(string group_name, int milliseconds)
        {
            Exception? ex = null;
            try
            {
                checkEnabled();
                var t = Task.Run(() => _tmcDriver?.GroupRunFor(group_name, milliseconds));
                await t.WaitAsync(CancellationToken.None);
            }
            catch (Exception _ex)
            {
                ex = _ex;
            }
            return ex == null ? Ok() : Problem(ex.ToString());
        }

        [HttpGet("{group_name}/GroupRunFor/{milliseconds}/{speed}")]
        public async Task<IActionResult> GroupRunFor(string group_name, int milliseconds, double speed)
        {
            Exception? ex = null;
            try
            {
                checkEnabled();
                var t = Task.Run(() => _tmcDriver?.GroupRunFor(group_name, milliseconds, speed));
                await t.WaitAsync(CancellationToken.None);
            }
            catch (Exception _ex)
            {
                ex = _ex;
            }
            return ex == null ? Ok() : Problem(ex.ToString());
        }

        [HttpGet("{group_name}/GroupMove/{steps}")]
        public async Task<IActionResult> GroupMove(string group_name, int steps)
        {
            Exception? ex = null;
            try
            {
                checkEnabled();
                var t = Task.Run(() => _tmcDriver?.GroupMove(group_name, steps));
                await t.WaitAsync(CancellationToken.None);
            }
            catch (Exception _ex)
            {
                ex = _ex;
            }
            return ex == null ? Ok() : Problem(ex.ToString());
        }

        [HttpGet("{group_name}/GroupMove/{steps}/{speed}")]
        public async Task<IActionResult> GroupMove(string group_name, int steps, double speed)
        {
            Exception? ex = null;
            try
            {
                checkEnabled();
                var t = Task.Run(() => _tmcDriver?.GroupMove(group_name, steps, speed));
                await t.WaitAsync(CancellationToken.None);
            }
            catch (Exception _ex)
            {
                ex = _ex;
            }
            return ex == null ? Ok() : Problem(ex.ToString());
        }
    }
}
