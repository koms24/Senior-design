using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SeniorDesignFall2024.Server.Shared.Extensions;
using SeniorDesignFall2024.Server.Shared.Interfaces;
using SeniorDesignFall2024.TmcDriver.DI;

namespace SeniorDesignFall2024.TmcDriver.Extensions
{
    public static class TmcDriverExtension
    {
        public static void AddTmcDriver(this WebApplicationBuilder builder)
        {
            IStartupConfigOptions opts = builder.GetStartupConfigOptions();
            if (opts.EnableTmcDriver)
            {
                builder.Services.Configure<TmcDriverOptions>(builder.Configuration.GetSection(TmcDriverOptions.SectionName));
                builder.Services.AddSingleton<TmcUartProvider>();
                builder.Services.AddSingleton<SeniorDesignFall2024.TmcDriver.TmcDriver>();
            }
        }
    }
}
