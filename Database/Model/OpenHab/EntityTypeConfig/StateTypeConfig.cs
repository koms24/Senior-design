using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SeniorDesignFall2024.Database.Model.OpenHab.Entity;

namespace SeniorDesignFall2024.Database.Model.OpenHab.EntityTypeConfig
{
    public class StateTypeConfig : IEntityTypeConfiguration<StateType>
    {
        public void Configure(EntityTypeBuilder<StateType> builder)
        {
            builder
                .HasDiscriminator<string>("type")
                .HasValue<StateType>("object")
                .HasValue<DecimalType>("Decimal")
                .HasValue<QuantityType>("Quantity");
        }
    }
}
