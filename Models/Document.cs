namespace Iamsigner_Integration.Models
{
    public class Document
    {
        public string docName { get; set; }
        public string docExtension { get; set; }
        public string docFile { get; set; }
        public bool isPasswordProtected { get; set; }
        public Guid masterDocID { get; set; } = Guid.NewGuid();
        
    }
}
