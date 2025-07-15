using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SeniorDesignFall2024.Database.Model.OpenHab.Context;
using SeniorDesignFall2024.Database.Options;
using SeniorDesignFall2024.Server.Shared.Interfaces;
using SeniorDesignFall2024.Server.Shared.Extensions;

namespace SeniorDesignFall2024.Database.Extension
{
    public static class SdDbContextSetupExtension
    {
        public static void AddSdDbContextOptions(this WebApplicationBuilder builder)
        {
            IStartupConfigOptions opts = builder.GetStartupConfigOptions();
            if (opts?.EnableDb ?? false)
                builder.Services.Configure<DbConfig>(builder.Configuration.GetSection(DbConfig.SectionName));
        }
        public static void AddSdDbContext(this WebApplicationBuilder builder)
        {
            IStartupConfigOptions opts = builder.GetStartupConfigOptions();
            if (opts?.EnableDb ?? false)
            {
                builder.Services.AddDbContext<SdDbContext>((services, builder) =>
                {
                    DbConfig opts = services.GetRequiredService<IOptions<DbConfig>>().Value;
                    builder.UseNpgsql(opts.ConnectionString, opts =>
                    {
                        opts.SetPostgresVersion(15, 0);
                    });
                });
            }
        }
    }
}
