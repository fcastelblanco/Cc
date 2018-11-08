namespace Isn.Upt.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FromZero : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Company",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false),
                        DateEndSupport = c.DateTime(nullable: false),
                        Prefix = c.String(),
                        ApplicationServerId = c.Guid(),
                        DataBaseServerId = c.Guid(),
                        Url = c.String(),
                        UserRequestDeployId = c.Guid(),
                        DeployDate = c.DateTime(),
                        ContactName = c.String(),
                        ContactPhone = c.String(),
                        State = c.Int(),
                        ContactEmail = c.String(),
                        Size = c.Int(),
                        SizeOnTestEnvironment = c.Int(),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Server", t => t.ApplicationServerId)
                .ForeignKey("dbo.Server", t => t.DataBaseServerId)
                .ForeignKey("dbo.User", t => t.UserRequestDeployId)
                .Index(t => t.ApplicationServerId)
                .Index(t => t.DataBaseServerId)
                .Index(t => t.UserRequestDeployId);
            
            CreateTable(
                "dbo.Server",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Capacity = c.Int(),
                        Installed = c.Int(),
                        IsDataBaseServer = c.Boolean(),
                        UserName = c.String(),
                        Password = c.String(),
                        Instance = c.String(),
                        ServerIpOrName = c.String(),
                        Dns = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CompanyModule",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        CompanyId = c.Guid(),
                        ModuleId = c.Guid(),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Company", t => t.CompanyId)
                .ForeignKey("dbo.Module", t => t.ModuleId)
                .Index(t => t.CompanyId)
                .Index(t => t.ModuleId);
            
            CreateTable(
                "dbo.Module",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        IsolucionId = c.Int(),
                        ProcedureParameterName = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        LastName = c.String(),
                        UserName = c.String(),
                        Password = c.String(),
                        Profile = c.Int(nullable: false),
                        Email = c.String(),
                        CompanyId = c.Guid(nullable: false),
                        Photo = c.Binary(),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CompanyRelease",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ReleaseId = c.Guid(nullable: false),
                        CompanyId = c.Guid(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CompanyUpdate",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ReleaseId = c.Guid(nullable: false),
                        Update = c.DateTime(nullable: false),
                        CompanyId = c.Guid(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        UpdatedDate = c.DateTime(),
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
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Parameter",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ParameterInternalIdentificator = c.Int(nullable: false),
                        Value = c.String(),
                        ParameterType = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.Id);
            
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
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.Id);
            
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
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Company", "UserRequestDeployId", "dbo.User");
            DropForeignKey("dbo.CompanyModule", "ModuleId", "dbo.Module");
            DropForeignKey("dbo.CompanyModule", "CompanyId", "dbo.Company");
            DropForeignKey("dbo.Company", "DataBaseServerId", "dbo.Server");
            DropForeignKey("dbo.Company", "ApplicationServerId", "dbo.Server");
            DropIndex("dbo.CompanyModule", new[] { "ModuleId" });
            DropIndex("dbo.CompanyModule", new[] { "CompanyId" });
            DropIndex("dbo.Company", new[] { "UserRequestDeployId" });
            DropIndex("dbo.Company", new[] { "DataBaseServerId" });
            DropIndex("dbo.Company", new[] { "ApplicationServerId" });
            DropTable("dbo.UserToken");
            DropTable("dbo.UnhandledException");
            DropTable("dbo.Release");
            DropTable("dbo.Parameter");
            DropTable("dbo.DownloadRequestRelease");
            DropTable("dbo.CompanyUpdate");
            DropTable("dbo.CompanyRelease");
            DropTable("dbo.User");
            DropTable("dbo.Module");
            DropTable("dbo.CompanyModule");
            DropTable("dbo.Server");
            DropTable("dbo.Company");
        }
    }
}
