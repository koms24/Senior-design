using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using SeniorDesignFall2024.Database.Model.OpenHab.Context;

namespace DatabaseTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var dsb = new NpgsqlDataSourceBuilder("Host=localhost;Username=pi;Password=Fall2024");
            var ds = dsb.Build();
            var sp = new ServiceCollection()
                .AddLogging()
                .AddDbContextPool<SdDbContext>(opt => opt.UseNpgsql(ds, o =>
                {
                    o.SetPostgresVersion(15, 0);
                    o.MigrationsAssembly(typeof(SdDbContext).Assembly.FullName);
                }))
                .BuildServiceProvider();
            var db = sp.GetService<SdDbContext>();
        }
    }
}
