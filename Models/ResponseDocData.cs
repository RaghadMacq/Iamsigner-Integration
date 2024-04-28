namespace Iamsigner_Integration.Models
{
    public class ResponseDocData
    {
        public string message { get; set; }
        public bool successStatus { get; set; }
        public string statusCode { get; set; }
        public Document response { get; set; } // Change this to Document type
    }
}
