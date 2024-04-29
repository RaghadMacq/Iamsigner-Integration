namespace Iamsigner_Integration.Models
{
    public class DocumentStatus
    {
        public string masterDocID { get; set; }

        public bool IsDownloadAuditTrail { get; set; } = true;
        public string docCompanyPrefixNo { get; set; }
        public string docStatus { get; set; }
    }
}
