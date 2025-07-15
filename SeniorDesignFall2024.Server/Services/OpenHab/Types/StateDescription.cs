namespace SeniorDesignFall2024.Server.Services.OpenHab.Types
{
    public class StateDescription
    {
        public float minimum {  get; set; }
        public float maximum { get; set; }
        public float step { get; set; }
        public string pattern { get; set; } = string.Empty;
        public bool readOnly { get; set; } = false;
        public List<StateOption> options { get; set; } = new List<StateOption>();
    }
}
