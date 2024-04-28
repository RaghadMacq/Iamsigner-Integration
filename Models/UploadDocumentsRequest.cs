using System.Reflection.Metadata;

namespace Iamsigner_Integration.Models
{
    public class UploadDocumentsRequest
    {
        public string registerSignID { get; set; }
        public List<Document> PrimaryDocs { get; set; }
    }
}
