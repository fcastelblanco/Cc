namespace Cc.Upt.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Company",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TaxId = c.String(),
                        Name = c.String(),
                        Phone = c.String(),
                        Mobile = c.String(),
                        Email = c.String(),
                        Url = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        UpdatedOn = c.DateTime(),
                        UpdatedBy = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DownloadRequestRelease",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ReleaseId = c.Guid(nullable: false),
                        DownloadRequestReleaseStatusType = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        UpdatedOn = c.DateTime(),
                        UpdatedBy = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Release", t => t.ReleaseId, cascadeDelete: true)
                .Index(t => t.ReleaseId);
            
            CreateTable(
                "dbo.Release",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Description = c.String(),
                        Version = c.String(),
                        UserId = c.Guid(nullable: false),
                        Published = c.DateTime(nullable: false),
                        IsSafe = c.Boolean(nullable: false),
                        Notes = c.String(),
                        ReleaseContent = c.Binary(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        UpdatedOn = c.DateTime(),
                        UpdatedBy = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        CompanyId = c.Guid(nullable: false),
                        Name = c.String(),
                        LastName = c.String(),
                        Password = c.String(),
                        Profile = c.Int(nullable: false),
                        Email = c.String(),
                        Photo = c.Binary(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        UpdatedOn = c.DateTime(),
                        UpdatedBy = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Company", t => t.CompanyId, cascadeDelete: true)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.Parameter",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ParameterInternalIdentificator = c.Int(nullable: false),
                        Value = c.String(),
                        ParameterType = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        UpdatedOn = c.DateTime(),
                        UpdatedBy = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ServerRelease",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ReleaseId = c.Guid(nullable: false),
                        ServerId = c.Guid(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        UpdatedOn = c.DateTime(),
                        UpdatedBy = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Release", t => t.ReleaseId, cascadeDelete: true)
                .ForeignKey("dbo.Server", t => t.ServerId, cascadeDelete: true)
                .Index(t => t.ReleaseId)
                .Index(t => t.ServerId);
            
            CreateTable(
                "dbo.Server",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Code = c.String(),
                        Name = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        UpdatedOn = c.DateTime(),
                        UpdatedBy = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ServerUpdate",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Update = c.DateTime(nullable: false),
                        ReleaseId = c.Guid(nullable: false),
                        ServerId = c.Guid(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        UpdatedOn = c.DateTime(),
                        UpdatedBy = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Release", t => t.ReleaseId, cascadeDelete: true)
                .ForeignKey("dbo.Server", t => t.ServerId, cascadeDelete: true)
                .Index(t => t.ReleaseId)
                .Index(t => t.ServerId);
            
            CreateTable(
                "dbo.UnhandledException",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Thread = c.Long(nullable: false),
                        Level = c.String(),
                        Logger = c.String(),
                        Message = c.String(),
                        Exception = c.String(),
                        Stack = c.String(),
                        ExceptionType = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserToken",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserId = c.Guid(nullable: false),
                        Token = c.Guid(nullable: false),
                        Expiration = c.DateTime(nullable: false),
                        TokenType = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        UpdatedOn = c.DateTime(),
                        UpdatedBy = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserToken", "UserId", "dbo.User");
            DropForeignKey("dbo.ServerUpdate", "ServerId", "dbo.Server");
            DropForeignKey("dbo.ServerUpdate", "ReleaseId", "dbo.Release");
            DropForeignKey("dbo.ServerRelease", "ServerId", "dbo.Server");
            DropForeignKey("dbo.ServerRelease", "ReleaseId", "dbo.Release");
            DropForeignKey("dbo.DownloadRequestRelease", "ReleaseId", "dbo.Release");
            DropForeignKey("dbo.Release", "UserId", "dbo.User");
            DropForeignKey("dbo.User", "CompanyId", "dbo.Company");
            DropIndex("dbo.UserToken", new[] { "UserId" });
            DropIndex("dbo.ServerUpdate", new[] { "ServerId" });
            DropIndex("dbo.ServerUpdate", new[] { "ReleaseId" });
            DropIndex("dbo.ServerRelease", new[] { "ServerId" });
            DropIndex("dbo.ServerRelease", new[] { "ReleaseId" });
            DropIndex("dbo.User", new[] { "CompanyId" });
            DropIndex("dbo.Release", new[] { "UserId" });
            DropIndex("dbo.DownloadRequestRelease", new[] { "ReleaseId" });
            DropTable("dbo.UserToken");
            DropTable("dbo.UnhandledException");
            DropTable("dbo.ServerUpdate");
            DropTable("dbo.Server");
            DropTable("dbo.ServerRelease");
            DropTable("dbo.Parameter");
            DropTable("dbo.User");
            DropTable("dbo.Release");
            DropTable("dbo.DownloadRequestRelease");
            DropTable("dbo.Company");
        }
    }
}
