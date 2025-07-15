namespace SeniorDesignFall2024.Server.Services.OpenHab.Types
{
    public class EnrichedItemDto
    {
        public string type { get; set; } = String.Empty;
        public string name { get; set; } = String.Empty;
        public string label {  get; set; } = String.Empty;
        public string category { get; set; } = String.Empty;
        public HashSet<string> tags { get; set;} = new();
        public List<string> groupNames { get; set; } = new();
        public string link { get; set; } = string.Empty;
        public string state { get; set; } = String.Empty;
        public string? transformedState { get; set; } = null;
        public StateDescription stateDescription { get; set; } = new();
        public string unitSymbol {  get; set; } = String.Empty;
        public CommandDescription commandDescription { get; set; } = new();
        //public Dictionary<string, object> metadata { get; set; } = new();
        public bool editable { get; set; } = false;
    }
}
