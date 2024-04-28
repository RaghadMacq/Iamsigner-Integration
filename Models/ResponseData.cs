using Iamsigner_Integration.Models;

namespace Iamsigner_Integration.Models
{
    public class ResponseData
    {
        public string message { get; set; }
        public bool successStatus { get; set; }
        public string statusCode { get; set; }
        public List<Signatory> response { get; set; }
    }
}
