namespace UTM.Keto.Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class db22 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Reviews",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserId = c.Guid(nullable: false),
                        Title = c.String(nullable: false),
                        Content = c.String(nullable: false),
                        Rating = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        Status = c.Int(nullable: false),
                        ModerationComment = c.String(),
                        ProductId = c.Guid(),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.SupportTickets",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserId = c.Guid(nullable: false),
                        Subject = c.String(nullable: false, maxLength: 100),
                        InitialMessage = c.String(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        ClosedDate = c.DateTime(),
                        Status = c.Int(nullable: false),
                        TicketNumber = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.TicketMessages",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TicketId = c.Guid(nullable: false),
                        SenderId = c.Guid(nullable: false),
                        Message = c.String(nullable: false),
                        SentDate = c.DateTime(nullable: false),
                        IsFromAdmin = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.SenderId)
                .ForeignKey("dbo.SupportTickets", t => t.TicketId)
                .Index(t => t.TicketId)
                .Index(t => t.SenderId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SupportTickets", "UserId", "dbo.Users");
            DropForeignKey("dbo.TicketMessages", "TicketId", "dbo.SupportTickets");
            DropForeignKey("dbo.TicketMessages", "SenderId", "dbo.Users");
            DropForeignKey("dbo.Reviews", "UserId", "dbo.Users");
            DropForeignKey("dbo.Reviews", "ProductId", "dbo.Products");
            DropIndex("dbo.TicketMessages", new[] { "SenderId" });
            DropIndex("dbo.TicketMessages", new[] { "TicketId" });
            DropIndex("dbo.SupportTickets", new[] { "UserId" });
            DropIndex("dbo.Reviews", new[] { "ProductId" });
            DropIndex("dbo.Reviews", new[] { "UserId" });
            DropTable("dbo.TicketMessages");
            DropTable("dbo.SupportTickets");
            DropTable("dbo.Reviews");
        }
    }
}
