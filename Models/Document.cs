namespace Iamsigner_Integration.Models
{
    public class Document
    {
        public string docName { get; set; }
        public string docExtension { get; set; }
        public string docFile { get; set; }
        public bool isPasswordProtected { get; set; }
        public string masterDocID { get; set; }
        public bool IsDownloadAuditTrail { get; set; } = true;
        public string docCompanyPrefixNo { get; set; }
        public string docStatus { get; set; }
    }
}
