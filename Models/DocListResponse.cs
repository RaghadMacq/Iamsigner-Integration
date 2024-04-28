namespace Iamsigner_Integration.Models
{
    public class DocListResponse
    {
        public string message { get; set; }
        public bool successStatus { get; set; }
        public string statusCode { get; set; }
        public List<SupportingDocument> response { get; set; }
    }
}
