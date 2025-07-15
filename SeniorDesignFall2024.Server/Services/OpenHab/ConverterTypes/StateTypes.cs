using System.Text.Json.Serialization;

namespace SeniorDesignFall2024.Server.Services.OpenHab.ConverterTypes
{
    //[JsonPolymorphic(TypeDiscriminatorPropertyName = "type", UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor)]
    //[JsonDerivedType(typeof(StateType), "object")]
    //[JsonDerivedType(typeof(DecimalType), "Decimal")]
    //[JsonDerivedType(typeof(QuantityType), "Quantity")]
    //public class StateType
    //{
    //    [JsonPropertyName("state")]
    //    public string State { get; set; }
    //    [JsonPropertyName("displayState")]
    //    public string DisplayState { get; set; }
    //}

    //public class DecimalType : StateType
    //{
    //    [JsonPropertyName("numericState")]
    //    public decimal NumericState { get; set; }
    //}

    //public class QuantityType : DecimalType
    //{
    //    [JsonPropertyName("unit")]
    //    public string Unit { get; set; }
    //}
}
