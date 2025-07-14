using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SeniorDesignFall2024.Database.Model.OpenHab.Context;
using SeniorDesignFall2024.Database.Model.OpenHab.Entity;
using SeniorDesignFall2024.Server.Options;
using SeniorDesignFall2024.Server.Services.OpenHab;
using SeniorDesignFall2024.Server.Shared.Interfaces;
using SeniorDesignFall2024.Server.Shared.Options;
using SeniorDesignFall2024.Server.Shared.Extensions;

namespace SeniorDesignFall2024.Server.Extensions
{
    public static class OpenHabServiceExtension
    {
        public const string OpenHabHttpClient = "OpenHabHttpClient";
        public static void AddOpenHabHttpClient(this WebApplicationBuilder builder) {
            var opts = builder.GetOpenHabConfigOptions();
            builder.Services.AddHttpClient(OpenHabHttpClient, client =>
            {
                client.BaseAddress = new Uri($"{opts.ApiEndpoint}/rest/");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("Accept", "text/event-stream");
            });
        }
        public static void AddOHStateMessageProcessorComService(this IServiceCollection services) {
            services.AddSingleton<OHStateMessageProcessorComService>();
        }
        public static void AddOpenHabComService(this IServiceCollection services) {
            services.AddSingleton<OpenHabComService>();
        }
        public static void AddOpenHabService(this IServiceCollection services) {
            services.AddSingleton<OpenHabService>();
        }
        public static void AddOpenHabEventService(this IServiceCollection services) {
            services.AddSingleton<OpenHabEventService>();
        }
        public static void AddOpenHabBackgroundService(this IServiceCollection services) {
            services.AddHostedService<OpenHabBackgroundService>();
        }
        
        public static void AddOpenHabBase(this WebApplicationBuilder builder)
        {
            IStartupConfigOptions opts = builder.GetStartupConfigOptions();
            builder.AddOpenHabHttpClient();
            var services = builder.Services;
            services.Configure<OpenHabConfigOptions>(builder.Configuration.GetSection(OpenHabConfigOptions.SectionName));
            services.AddOpenHabComService();
            if (opts.EnableZwaveOpenHab)
            {
                services.AddOHStateMessageProcessorComService();
                services.AddOpenHabService();
                services.AddOpenHabEventService();
            }
        }
        public static void AddOpenHabProcesses(this WebApplicationBuilder builder) {
            IStartupConfigOptions opts = builder.GetStartupConfigOptions();
            if(opts.EnableZwaveOpenHab)
            {
                var services = builder.Services;
                services.AddOpenHabBackgroundService();
            }
        }

        public static OpenHabConfigOptions GetOpenHabConfigOptions(this WebApplicationBuilder builder)
        {
            OpenHabConfigOptions? opts = builder.Configuration.GetSection(OpenHabConfigOptions.SectionName).Get<OpenHabConfigOptions>();
            return opts != null ? opts : new OpenHabConfigOptions();
        }

        public static void SyncOpenHabToDb(this WebApplication app)
        {
            StartupConfigOptions statupOpts = app.Services.GetRequiredService<IOptions<StartupConfigOptions>>().Value;
            if (statupOpts.EnableDb && statupOpts.EnableZwaveOpenHab)
            {
                Console.WriteLine("Startup DB Sync");
                var opts = app.Services.GetRequiredService<IOptions<OpenHabConfigOptions>>().Value;
                HashSet<string> subs = opts.ItemSubscriptions.ToHashSet();
                if (subs.Count > 0)
                {
                    using IServiceScope scope = app.Services.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<SdDbContext>();
                    var itemSet = db.Set<Item>();
                    var found = itemSet.Select(r => r.Uid).Where(r => subs.Contains(r)).ToHashSet();
                    var notFound = subs.Where(r => !found.Contains(r));
                    OpenHabService ohs = app.Services.GetRequiredService<OpenHabService>();
                    var t = Task.WhenAll(notFound.Select(uid => _retrieveOpenHabItem(ohs, itemSet, uid)).ToArray());
                    t.Wait();
                    db.SaveChanges();
                }
            }
        }

        private static async Task<Item> _retrieveOpenHabItem(OpenHabService ohs, DbSet<Item> itemSet,  string uid)
        {
            var itemDto = await ohs.GetItem(uid);
            Item item = new Item
            {
                Uid = uid,
                Name = itemDto.name,
                Label = itemDto.label,
                Category = itemDto.category
            };
            itemSet.Add(item);
            return item;
        }
    }
}
