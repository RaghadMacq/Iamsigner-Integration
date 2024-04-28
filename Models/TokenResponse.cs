namespace Iamsigner_Integration.Models
{
    public class TokenResponse
    {
        public string token { get; set; }
        public int tokenValidityDays { get; set; }
        public string validityDate { get; set; }
    }
}