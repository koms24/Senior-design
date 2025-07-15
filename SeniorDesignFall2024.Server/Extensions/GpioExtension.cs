using SeniorDesignFall2024.Server.Shared.Interfaces;
using SeniorDesignFall2024.Server.Shared.Extensions;
using System.Device.Gpio;
using SeniorDesignFall2024.Server.Services;

namespace SeniorDesignFall2024.Server.Extensions
{
    public static class GpioExtension
    {
        public static void AddGpioController(this WebApplicationBuilder builder)
        {
            IStartupConfigOptions opts = builder.GetStartupConfigOptions();
            if (opts?.EnableGpio ?? false)
            {
                builder.Services.AddSingleton<GpioController>();
                builder.Services.AddSingleton<GpioControllerService>();
            }
        }
    }
}
