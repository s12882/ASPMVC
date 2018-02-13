namespace ASPMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class advmig : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CommentViewModel",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        UrlSeo = c.String(),
                        DateTime = c.DateTime(nullable: false),
                        Body = c.String(),
                        ParentReplyId = c.String(),
                        UserName = c.String(),
                        CommentViewModel_Id = c.String(maxLength: 128),
                        Comment_CommentId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CommentViewModel", t => t.CommentViewModel_Id)
                .ForeignKey("dbo.Comment", t => t.Comment_CommentId)
                .Index(t => t.CommentViewModel_Id)
                .Index(t => t.Comment_CommentId);
            
            AddColumn("dbo.Comment", "CommentViewModel_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.AdvViewModel", "CommentViewModel_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Comment", "CommentViewModel_Id");
            CreateIndex("dbo.AdvViewModel", "CommentViewModel_Id");
            AddForeignKey("dbo.Comment", "CommentViewModel_Id", "dbo.CommentViewModel", "Id");
            AddForeignKey("dbo.AdvViewModel", "CommentViewModel_Id", "dbo.CommentViewModel", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AdvViewModel", "CommentViewModel_Id", "dbo.CommentViewModel");
            DropForeignKey("dbo.Comment", "CommentViewModel_Id", "dbo.CommentViewModel");
            DropForeignKey("dbo.CommentViewModel", "Comment_CommentId", "dbo.Comment");
            DropForeignKey("dbo.CommentViewModel", "CommentViewModel_Id", "dbo.CommentViewModel");
            DropIndex("dbo.CommentViewModel", new[] { "Comment_CommentId" });
            DropIndex("dbo.CommentViewModel", new[] { "CommentViewModel_Id" });
            DropIndex("dbo.AdvViewModel", new[] { "CommentViewModel_Id" });
            DropIndex("dbo.Comment", new[] { "CommentViewModel_Id" });
            DropColumn("dbo.AdvViewModel", "CommentViewModel_Id");
            DropColumn("dbo.Comment", "CommentViewModel_Id");
            DropTable("dbo.CommentViewModel");
        }
    }
}
