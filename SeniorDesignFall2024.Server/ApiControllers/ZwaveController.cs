using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SeniorDesignFall2024.Server.Services;
using SeniorDesignFall2024.Server.Services.OpenHab;
using SeniorDesignFall2024.Server.Services.OpenHab.Types;
using SeniorDesignFall2024.Server.Shared.Options;

namespace SeniorDesignFall2024.Server.ApiControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ZwaveController : ControllerBase
    {
        private bool _enabled = true;
        private readonly OpenHabService? _ohsController;

        public ZwaveController(
            IOptions<StartupConfigOptions> opts,
            OpenHabService? ohsController = null)
        {
            _enabled = opts.Value.EnableZwaveOpenHab;
            _ohsController = ohsController;
        }

        private bool checkEnabled()
        {
            if (!_enabled)
                throw new Exception("Zwave OpenHab Feature Disabled, Check appsettings file");
            return _enabled;
        }

        [HttpGet("items/{item_name}")]
        public async Task<IActionResult> GetItem(string item_name)
        {
            EnrichedItemDto? item = null;
            Exception? ex = null;
            try {
                checkEnabled();
                item = await _ohsController.GetItem(item_name);
            } catch(Exception _ex) { 
                ex = _ex;
            }
            return item!=null ? Ok(item) : Problem((ex ?? new Exception("Unknown Server Error")).ToString());
        }

        [HttpPost("items/{item_name}")]
        [Consumes("text/plain")]
        public async Task<IActionResult> SendItemCommand(string item_name, [FromBody]string command) {
            Exception? ex = null;
            try
            {
                checkEnabled();
                await _ohsController.SendItemCommand(item_name, command);
            } catch (Exception _ex) {
                ex = _ex;
            }
            return ex == null ? Ok() : Problem(ex.ToString());
        }
    }
}
