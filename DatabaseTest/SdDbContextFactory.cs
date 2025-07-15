using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Design;
using SeniorDesignFall2024.Database.Model.OpenHab.Context;
using Npgsql;

namespace DatabaseTest
{
    public class SdDbContextFactory : IDesignTimeDbContextFactory<SdDbContext>
    {
        public SdDbContext CreateDbContext(string[] args)
        {
            var dsb = new NpgsqlDataSourceBuilder("Host=localhost;Database=senior_design;Username=pi;Password=Fall2024");
            var ds = dsb.Build();
            var ob = new DbContextOptionsBuilder<SdDbContext>();
            ob.UseNpgsql(ds, o =>
            {
                o.SetPostgresVersion(15, 0);
            });
            return new SdDbContext(ob.Options);
        }
    }
}
