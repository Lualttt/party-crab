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

    public class PartyListDTO
    {
        public int page { get; set; }
    }
    public class PartyListResponseDTO
    {
        public bool successful { get; set; }
        public PartyListResponse data { get; set; }

    }
    public class PartyListResponse
    {
        public int page { get; set; }
        public int max_page { get; set; }
        public Party[] parties { get; set; }
        public string error { get; set; }
    }

    public class UserListResponseDTO
    {
        public bool successful { get; set; }
        public UserListResponse data { get; set; }
    }
    public class UserListResponse
    {
        public int page { get; set; }
        public int max_page { get; set; }
        public User[] users { get; set; }
        public string error { get; set; }
    }
    public class User
    {
        public string name { get; set; }
        public string id { get; set; }
    }

    public class PromoteDTO
    {
        public string party_id { get; set; }
        public string new_host { get; set; }
    }
    public class PromoteResponse
    {
        public bool successful { get; set; }
        public Promote data { get; set; }
    }
    public class Promote
    {
        public string error { get; set; }
        public string new_host { get; set; }
    }

    public class PromotedDTO
    {
        public string old_host { get; set; }
        public string new_host { get; set; }
    }

    public class WarpDTO
    {
        public string lobby_id { get; set; }
    }
    public class WarpResponseDTO
    {
        public bool successful { get; set; }
        public WarpResponse data { get; set; }

    }
    public class WarpResponse
    {
        public string lobby_id { get; set; }
        public string error { get; set; }
    }
}