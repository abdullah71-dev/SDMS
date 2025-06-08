using System;
using System.Collections.Generic;

namespace SecureDocumentManagementSystem.Models
{
    public class Document
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string FilePath { get; set; }
        public string EncryptedKey { get; set; }
        public DateTime UploadedAt { get; set; }

        public int OwnerId { get; set; }
        public virtual User Owner { get; set; }

        public virtual ICollection<DocumentVersion> Versions { get; set; }
    }
}
