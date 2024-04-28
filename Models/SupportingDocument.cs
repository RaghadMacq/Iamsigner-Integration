using System;

namespace Iamsigner_Integration.Models
{
    public class SupportingDocument
    {
        public Guid supportingDocID { get; set; } = Guid.NewGuid();
        public string supportingDocName { get; set; }
        public string supportingDocPath { get; set; } = "https://api-dev.iamsigner.com/CDN/8a123c7f-37e3-4570-9531-0dc98a014a1b/SupportingDoc/";
        public string supportingDocExtension { get; set; }
        public string SupportingDocFile { get; set; } // Adjust the type based on your requirements
        //public IFormFile SupportingDocFile { get; set; } 

        public DateTime? createdOn { get; set; }
        public bool? isStoredinDB { get; set; }
        public string createdBy { get; set; } = "raghad@macquires.com";
        public DateTime? modifiedOn { get; set; }
        public string? modifiedBy { get; set; }
        public string? folderName { get; set; }
        public string masterDocID { get; set; } = "f1155e41-8b6c-473b-b440-007ffc8f5510";
        //public string registerSignID { get; set; } = string.Empty;
        public string? createdByName { get; set; }

    }
}
