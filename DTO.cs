namespace party_crab {
    // im too dumb so im doing it like this
    public class HostDTO
    {
        public string party_name   { get; set; }
        public int    party_max    { get; set; }
        public bool   party_public { get; set; }
    }
    
    public class HostResponseDTO
    {
        public bool successful { get; set; }
        public HostResponse data { get; set; }
    }
    public class HostResponse
    {
        public string party_id { get; set; }
        public string error { get; set; }
    }

    public class DisbandResponseDTO
    {
        public bool successful { get; set; }
        public DisbandResponse data { get; set; }
    }
    public class DisbandResponse
    {
        public string error { get; set; }
    }

    public class JoinResponseDTO
    {
        public bool successful { get; set; }
        public JoinResponse data { get; set; }
    }
    public class JoinResponse
    {
        public string party_name   { get; set; }
        public int    party_max    { get; set; }
        public int    party_count  { get; set; }
        public bool   party_public { get; set; }
        public string party_host   { get; set; }
        public string party_id     { get; set; }
        public string error        { get; set; }
    }

    public class ShortDTO
    {
        public string message { get; set; }
    }

    public class MessageDTO
    {
        public string username { get; set; } 
        public string message  { get; set; }
    }

    public class JoinedDTO
    {
        public string username { get; set;}
    }

    public class PartyIDDTO
    {
        public string party_id { get; set; }
    }

    public class Party
    {
        public string party_name   { get; set; }
        public int    party_max    { get; set; }
        public int    party_count  { get; set; }
        public bool   party_public { get; set; }
        public string party_host   { get; set; }
        public string party_id     { get; set; }
    }

    public class DisbandedDTO
    {
        public string username { get; set; }
        public string party_id { get; set; }
    }
}