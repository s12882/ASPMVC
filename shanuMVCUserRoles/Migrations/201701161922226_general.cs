namespace shanuMVCUserRoles.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class general : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.ReplyLike", new[] { "Reply_ReplyId" });
            DropColumn("dbo.ReplyLike", "ReplyId");
            RenameColumn(table: "dbo.ReplyLike", name: "Reply_ReplyId", newName: "ReplyId");
            DropPrimaryKey("dbo.ReplyLike");
            AddColumn("dbo.ReplyLike", "RlikeId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.ReplyLike", "ReplyId", c => c.String(maxLength: 128));
            AddPrimaryKey("dbo.ReplyLike", "RlikeId");
            CreateIndex("dbo.ReplyLike", "ReplyId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.ReplyLike", new[] { "ReplyId" });
            DropPrimaryKey("dbo.ReplyLike");
            AlterColumn("dbo.ReplyLike", "ReplyId", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.ReplyLike", "RlikeId");
            AddPrimaryKey("dbo.ReplyLike", "ReplyId");
            RenameColumn(table: "dbo.ReplyLike", name: "ReplyId", newName: "Reply_ReplyId");
            AddColumn("dbo.ReplyLike", "ReplyId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.ReplyLike", "Reply_ReplyId");
        }
    }
}
