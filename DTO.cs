namespace party_crab {
    // im too dumb so im doing it like this
    public class HostDTO
    {
        public string party_name   { get; set; }
        public int    party_max    { get; set; }
        public bool   party_public { get; set; }
    }
    
    public class MessageDTO
    {
        public string username { get; set; } 
        public string message  { get; set; }
    }
}