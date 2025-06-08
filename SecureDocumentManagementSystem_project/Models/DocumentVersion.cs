using System;

namespace SecureDocumentManagementSystem.Models
{
    public class DocumentVersion
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public string FilePath { get; set; }
        public DateTime VersionDate { get; set; }

        public virtual Document Document { get; set; }
    }
}
