namespace Iamsigner_Integration.Models
{
    public class AuthResponse
    {
        public string message { get; set; }
        public bool successStatus { get; set; }
        public string statusCode { get; set; }
        public TokenResponse response { get; set; }
    }
}
