using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SeniorDesignFall2024.Server.Shared.Interfaces;
using SeniorDesignFall2024.Server.Shared.Options;

namespace SeniorDesignFall2024.Server.Shared.Extensions
{
    public static class StartupConfigExtension
    {
        private static IStartupConfigOptions? _optsCache = null;
        public static void AddStartupConfig(this WebApplicationBuilder builder) {
            builder.Services.Configure<StartupConfigOptions>(builder.Configuration.GetSection(StartupConfigOptions.SectionName));
        }

        public static IStartupConfigOptions GetStartupConfigOptions(this WebApplicationBuilder builder) {
            if (_optsCache != null)
                return _optsCache;
            IStartupConfigOptions? opts = builder.Configuration.GetSection(StartupConfigOptions.SectionName).Get<StartupConfigOptions>();
            return (_optsCache = (opts != null ? opts : new StartupConfigOptions()));
        }
    }
}
