using SeniorDesignFall2024.Server.Services;
using SeniorDesignFall2024.Server.Shared.Interfaces;
using SeniorDesignFall2024.Server.Shared.Extensions;

namespace SeniorDesignFall2024.Server.Extensions
{
    public static class DataLoggerExtension
    {
        public static void AddBackgroundDataLogger(this WebApplicationBuilder builder)
        {
            IStartupConfigOptions opts = builder.GetStartupConfigOptions();
            if (opts.EnableDb)
                builder.Services.AddHostedService<DataLoggerBackgroundService>();
        }
    }
}
