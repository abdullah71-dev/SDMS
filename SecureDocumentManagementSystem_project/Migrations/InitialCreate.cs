using System;
using System.Data.Entity.Migrations;

namespace SecureDocumentManagementSystem.Migrations
{
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Users",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Username = c.String(nullable: false),
                    Email = c.String(nullable: false),
                    PasswordHash = c.String(nullable: false),
                    IsAdmin = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.Documents",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Title = c.String(),
                    FilePath = c.String(),
                    EncryptedKey = c.String(),
                    UploadedAt = c.DateTime(nullable: false),
                    OwnerId = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.OwnerId, cascadeDelete: true)
                .Index(t => t.OwnerId);

            CreateTable(
                "dbo.DocumentVersions",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    DocumentId = c.Int(nullable: false),
                    FilePath = c.String(),
                    VersionDate = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Documents", t => t.DocumentId, cascadeDelete: true)
                .Index(t => t.DocumentId);

            CreateTable(
                "dbo.SharedDocuments",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    DocumentId = c.Int(nullable: false),
                    SharedWithUserId = c.Int(nullable: false),
                    Permission = c.String(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Documents", t => t.DocumentId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.SharedWithUserId)
                .Index(t => t.DocumentId)
                .Index(t => t.SharedWithUserId);
        }

        public override void Down()
        {
            DropForeignKey("dbo.SharedDocuments", "SharedWithUserId", "dbo.Users");
            DropForeignKey("dbo.SharedDocuments", "DocumentId", "dbo.Documents");
            DropForeignKey("dbo.DocumentVersions", "DocumentId", "dbo.Documents");
            DropForeignKey("dbo.Documents", "OwnerId", "dbo.Users");

            DropIndex("dbo.SharedDocuments", new[] { "SharedWithUserId" });
            DropIndex("dbo.SharedDocuments", new[] { "DocumentId" });
            DropIndex("dbo.DocumentVersions", new[] { "DocumentId" });
            DropIndex("dbo.Documents", new[] { "OwnerId" });

            DropTable("dbo.SharedDocuments");
            DropTable("dbo.DocumentVersions");
            DropTable("dbo.Documents");
            DropTable("dbo.Users");
        }
    }
}
