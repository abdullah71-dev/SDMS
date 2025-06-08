using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using SecureDocumentManagementSystem.Models;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<DocumentVersion> DocumentVersions { get; set; }
    public DbSet<SharedDocument> SharedDocuments { get; set; }

    public AppDbContext() : base("DefaultConnection") { }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        // تعطيل الحذف التلقائي للعلاقة مع SharedWithUser
        modelBuilder.Entity<SharedDocument>()
            .HasRequired(sd => sd.SharedWith)
            .WithMany()
            .HasForeignKey(sd => sd.SharedWithUserId)
            .WillCascadeOnDelete(false);

        base.OnModelCreating(modelBuilder);
    }
}
