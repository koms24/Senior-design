using Microsoft.EntityFrameworkCore;

namespace SeniorDesignFall2024.Database.Model.OpenHab.Context
{
    public class SdDbContext(DbContextOptions<SdDbContext> options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SdDbContext).Assembly);
        }
    }
}
