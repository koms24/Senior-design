using SeniorDesignFall2024.Database.Model.OpenHab.EntityAbstract;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeniorDesignFall2024.Database.Model.OpenHab.Entity
{
    public class ItemKey : ItemAbstract
    {
        [InverseProperty("ParentItem")]
        public virtual ICollection<StateType> AllStateData { get; set; }
    }

    public class Item : ItemKey
    {
        public required string Name { get; set; }
        public string? Label { get; set; }
        public string? Category { get; set; }
        public virtual StateType? State { get; set; }
    }
}
