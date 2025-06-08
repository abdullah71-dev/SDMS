using System.ComponentModel.DataAnnotations.Schema;

namespace SecureDocumentManagementSystem.Models
{
    public class SharedDocument
    {
        public int Id { get; set; }

        public int DocumentId { get; set; }
        [ForeignKey("DocumentId")]
        public virtual Document Document { get; set; }

        public int SharedWithUserId { get; set; }
        [ForeignKey("SharedWithUserId")]
        public virtual User SharedWith { get; set; }

        public string Permission { get; set; } 
    }
}
