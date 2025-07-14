using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SeniorDesignFall2024.Database.Model.OpenHab.Entity
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "type", UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor)]
    [JsonDerivedType(typeof(StateType), "object")]
    [JsonDerivedType(typeof(DecimalType), "Decimal")]
    [JsonDerivedType(typeof(QuantityType), "Quantity")]
    public class StateType
    {
        [Key]
        [JsonIgnore]
        public int Id { get; set; }
        [ForeignKey(nameof(ParentItem))]
        [JsonIgnore]
        public string ParentItemId { get; set; }
        [JsonIgnore]
        public virtual Item ParentItem { get; set; }
        [JsonIgnore]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        [JsonPropertyName("state")]
        public string State { get; set; }
        [JsonPropertyName("displayState")]
        public string DisplayState { get; set; }
    }

    public class DecimalType : StateType
    {
        [JsonPropertyName("numericState")]
        public decimal NumericState { get; set; }
    }

    public class QuantityType : DecimalType
    {
        [JsonPropertyName("unit")]
        public string Unit { get; set; }
    }
}
