using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Iamsigner_Integration.Models
{
    public class Signatory
    {
        public Guid signatoriesID { get; set; } = Guid.NewGuid();
        public string name { get; set; }
        public string email { get; set; }
        public string countryCode { get; set; }
        public string phone { get; set; }
        public string company { get; set; }
        public string? signStatus { get; set; }
        public DateTime? signDate { get; set; }
        public int signOrder { get; set; }
        public string notificationType { get; set; }
        public string signatureType { get; set; }
        public string signatureColor { get; set; }
        public string signPage { get; set; }
        public string signPosition { get; set; }
        public bool isMobile { get; set; }
        public DateTime? expiryDate { get; set; }
        public string? expiryDateFormatted { get; set; }
        public bool isExpiring { get; set; }
        public bool isPasswordProtected { get; set; }
        public string? password { get; set; }
        public Guid masterDocID { get; set; }
        public string registerSignID { get; set; } = "8a123c7f-37e3-4570-9531-0dc98a014a1b";
        public DateTime? createdOn { get; set; }
        public string? createdBy { get; set; }
        public string? createdByName { get; set; }
        public DateTime? modifiedOn { get; set; }
        public string? modifiedBy { get; set; }
    }

}
