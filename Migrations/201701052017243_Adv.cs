namespace shanuMVCUserRoles.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Adv : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AdvCategory",
                c => new
                    {
                        AdvID = c.String(nullable: false, maxLength: 128),
                        CatID = c.String(nullable: false, maxLength: 128),
                        Checked = c.Boolean(nullable: false),
                        Category_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => new { t.AdvID, t.CatID })
                .ForeignKey("dbo.Advert", t => t.AdvID, cascadeDelete: true)
                .ForeignKey("dbo.Category", t => t.Category_Id)
                .Index(t => t.AdvID)
                .Index(t => t.Category_Id);
            
            CreateTable(
                "dbo.Advert",
                c => new
                    {
                        AdvID = c.String(nullable: false, maxLength: 128),
                        Title = c.String(nullable: false),
                        ShortDescription = c.String(nullable: false),
                        Body = c.String(nullable: false),
                        Meta = c.String(nullable: false),
                        UrlSeo = c.String(nullable: false),
                        Published = c.Boolean(nullable: false),
                        LikeCount = c.Int(nullable: false),
                        PostedOn = c.DateTime(nullable: false),
                        Modified = c.DateTime(),
                    })
                .PrimaryKey(t => t.AdvID);
            
            CreateTable(
                "dbo.AdvImage",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ImageUrl = c.String(nullable: false),
                        Imagehumbnail = c.String(),
                        AdvId = c.String(maxLength: 128),
                        SiteName = c.String(),
                        AdvViewModel_ID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Advert", t => t.AdvId)
                .ForeignKey("dbo.AdvViewModel", t => t.AdvViewModel_ID)
                .Index(t => t.AdvId)
                .Index(t => t.AdvViewModel_ID);
            
            CreateTable(
                "dbo.AdvLike",
                c => new
                    {
                        AdvID = c.String(nullable: false, maxLength: 128),
                        Username = c.String(),
                        Like = c.Boolean(nullable: false),
                        Dislike = c.Boolean(nullable: false),
                        Advert_AdvID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.AdvID)
                .ForeignKey("dbo.Advert", t => t.Advert_AdvID)
                .Index(t => t.Advert_AdvID);
            
            CreateTable(
                "dbo.AdvTag",
                c => new
                    {
                        AdvID = c.String(nullable: false, maxLength: 128),
                        TagId = c.String(nullable: false, maxLength: 128),
                        Checked = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.AdvID, t.TagId })
                .ForeignKey("dbo.Advert", t => t.AdvID, cascadeDelete: true)
                .ForeignKey("dbo.Tag", t => t.TagId, cascadeDelete: true)
                .Index(t => t.AdvID)
                .Index(t => t.TagId);
            
            CreateTable(
                "dbo.Tag",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                        UrlSeo = c.String(nullable: false),
                        Checked = c.Boolean(nullable: false),
                        AdvViewModel_ID = c.String(maxLength: 128),
                        AdvViewModel_ID1 = c.String(maxLength: 128),
                        ServiceViewModel_ID = c.String(maxLength: 128),
                        ServiceViewModel_ID1 = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AdvViewModel", t => t.AdvViewModel_ID)
                .ForeignKey("dbo.AdvViewModel", t => t.AdvViewModel_ID1)
                .ForeignKey("dbo.ServiceViewModel", t => t.ServiceViewModel_ID)
                .ForeignKey("dbo.ServiceViewModel", t => t.ServiceViewModel_ID1)
                .Index(t => t.AdvViewModel_ID)
                .Index(t => t.AdvViewModel_ID1)
                .Index(t => t.ServiceViewModel_ID)
                .Index(t => t.ServiceViewModel_ID1);
            
            CreateTable(
                "dbo.Comment",
                c => new
                    {
                        CommentId = c.String(nullable: false, maxLength: 128),
                        AdvId = c.String(maxLength: 128),
                        DateTime = c.DateTime(nullable: false),
                        UserName = c.String(),
                        Body = c.String(nullable: false),
                        NetLikeCount = c.Int(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        AdvViewModel_ID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.CommentId)
                .ForeignKey("dbo.Advert", t => t.AdvId)
                .ForeignKey("dbo.AdvViewModel", t => t.AdvViewModel_ID)
                .Index(t => t.AdvId)
                .Index(t => t.AdvViewModel_ID);
            
            CreateTable(
                "dbo.CommentLike",
                c => new
                    {
                        CommentId = c.String(nullable: false, maxLength: 128),
                        Username = c.String(),
                        Like = c.Boolean(nullable: false),
                        Dislike = c.Boolean(nullable: false),
                        Comment_CommentId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.CommentId)
                .ForeignKey("dbo.Comment", t => t.Comment_CommentId)
                .Index(t => t.Comment_CommentId);
            
            CreateTable(
                "dbo.Reply",
                c => new
                    {
                        ReplyId = c.String(nullable: false, maxLength: 128),
                        AdvID = c.String(maxLength: 128),
                        CommentId = c.String(maxLength: 128),
                        ParentReplyId = c.String(),
                        DateTime = c.DateTime(nullable: false),
                        UserName = c.String(),
                        Body = c.String(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ReplyId)
                .ForeignKey("dbo.Advert", t => t.AdvID)
                .ForeignKey("dbo.Comment", t => t.CommentId)
                .Index(t => t.AdvID)
                .Index(t => t.CommentId);
            
            CreateTable(
                "dbo.ReplyLike",
                c => new
                    {
                        ReplyId = c.String(nullable: false, maxLength: 128),
                        Username = c.String(),
                        Like = c.Boolean(nullable: false),
                        Dislike = c.Boolean(nullable: false),
                        Reply_ReplyId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ReplyId)
                .ForeignKey("dbo.Reply", t => t.Reply_ReplyId)
                .Index(t => t.Reply_ReplyId);
            
            CreateTable(
                "dbo.Category",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                        UrlSeo = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        Checked = c.Boolean(nullable: false),
                        AdvViewModel_ID = c.String(maxLength: 128),
                        AdvViewModel_ID1 = c.String(maxLength: 128),
                        ServiceViewModel_ID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AdvViewModel", t => t.AdvViewModel_ID)
                .ForeignKey("dbo.AdvViewModel", t => t.AdvViewModel_ID1)
                .ForeignKey("dbo.ServiceViewModel", t => t.ServiceViewModel_ID)
                .Index(t => t.AdvViewModel_ID)
                .Index(t => t.AdvViewModel_ID1)
                .Index(t => t.ServiceViewModel_ID);
            
            CreateTable(
                "dbo.AdvViewModel",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        Body = c.String(),
                        FirstAdvId = c.String(),
                        LastAdvId = c.String(),
                        NextAdvSlug = c.String(),
                        PreviousAdvSlug = c.String(),
                        AdvCount = c.Int(nullable: false),
                        AdvDislikes = c.Int(nullable: false),
                        AdvLikes = c.Int(nullable: false),
                        Title = c.String(),
                        Meta = c.String(),
                        UrlSeo = c.String(),
                        ShortDescription = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.ServiceViewModel",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        PostedOn = c.DateTime(nullable: false),
                        Modified = c.DateTime(),
                        AdvDislikes = c.Int(nullable: false),
                        AdvLikes = c.Int(nullable: false),
                        TotalAverts = c.Int(nullable: false),
                        ShortDescription = c.String(),
                        Title = c.String(),
                        UrlSlug = c.String(),
                        Advert_AdvID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Advert", t => t.Advert_AdvID)
                .Index(t => t.Advert_AdvID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tag", "ServiceViewModel_ID1", "dbo.ServiceViewModel");
            DropForeignKey("dbo.Tag", "ServiceViewModel_ID", "dbo.ServiceViewModel");
            DropForeignKey("dbo.ServiceViewModel", "Advert_AdvID", "dbo.Advert");
            DropForeignKey("dbo.Category", "ServiceViewModel_ID", "dbo.ServiceViewModel");
            DropForeignKey("dbo.Tag", "AdvViewModel_ID1", "dbo.AdvViewModel");
            DropForeignKey("dbo.Tag", "AdvViewModel_ID", "dbo.AdvViewModel");
            DropForeignKey("dbo.AdvImage", "AdvViewModel_ID", "dbo.AdvViewModel");
            DropForeignKey("dbo.Comment", "AdvViewModel_ID", "dbo.AdvViewModel");
            DropForeignKey("dbo.Category", "AdvViewModel_ID1", "dbo.AdvViewModel");
            DropForeignKey("dbo.Category", "AdvViewModel_ID", "dbo.AdvViewModel");
            DropForeignKey("dbo.AdvCategory", "Category_Id", "dbo.Category");
            DropForeignKey("dbo.ReplyLike", "Reply_ReplyId", "dbo.Reply");
            DropForeignKey("dbo.Reply", "CommentId", "dbo.Comment");
            DropForeignKey("dbo.Reply", "AdvID", "dbo.Advert");
            DropForeignKey("dbo.CommentLike", "Comment_CommentId", "dbo.Comment");
            DropForeignKey("dbo.Comment", "AdvId", "dbo.Advert");
            DropForeignKey("dbo.AdvTag", "TagId", "dbo.Tag");
            DropForeignKey("dbo.AdvTag", "AdvID", "dbo.Advert");
            DropForeignKey("dbo.AdvLike", "Advert_AdvID", "dbo.Advert");
            DropForeignKey("dbo.AdvImage", "AdvId", "dbo.Advert");
            DropForeignKey("dbo.AdvCategory", "AdvID", "dbo.Advert");
            DropIndex("dbo.ServiceViewModel", new[] { "Advert_AdvID" });
            DropIndex("dbo.Category", new[] { "ServiceViewModel_ID" });
            DropIndex("dbo.Category", new[] { "AdvViewModel_ID1" });
            DropIndex("dbo.Category", new[] { "AdvViewModel_ID" });
            DropIndex("dbo.ReplyLike", new[] { "Reply_ReplyId" });
            DropIndex("dbo.Reply", new[] { "CommentId" });
            DropIndex("dbo.Reply", new[] { "AdvID" });
            DropIndex("dbo.CommentLike", new[] { "Comment_CommentId" });
            DropIndex("dbo.Comment", new[] { "AdvViewModel_ID" });
            DropIndex("dbo.Comment", new[] { "AdvId" });
            DropIndex("dbo.Tag", new[] { "ServiceViewModel_ID1" });
            DropIndex("dbo.Tag", new[] { "ServiceViewModel_ID" });
            DropIndex("dbo.Tag", new[] { "AdvViewModel_ID1" });
            DropIndex("dbo.Tag", new[] { "AdvViewModel_ID" });
            DropIndex("dbo.AdvTag", new[] { "TagId" });
            DropIndex("dbo.AdvTag", new[] { "AdvID" });
            DropIndex("dbo.AdvLike", new[] { "Advert_AdvID" });
            DropIndex("dbo.AdvImage", new[] { "AdvViewModel_ID" });
            DropIndex("dbo.AdvImage", new[] { "AdvId" });
            DropIndex("dbo.AdvCategory", new[] { "Category_Id" });
            DropIndex("dbo.AdvCategory", new[] { "AdvID" });
            DropTable("dbo.ServiceViewModel");
            DropTable("dbo.AdvViewModel");
            DropTable("dbo.Category");
            DropTable("dbo.ReplyLike");
            DropTable("dbo.Reply");
            DropTable("dbo.CommentLike");
            DropTable("dbo.Comment");
            DropTable("dbo.Tag");
            DropTable("dbo.AdvTag");
            DropTable("dbo.AdvLike");
            DropTable("dbo.AdvImage");
            DropTable("dbo.Advert");
            DropTable("dbo.AdvCategory");
        }
    }
}
