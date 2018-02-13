namespace ASPMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class advmigration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServiceViewModel", "CommentViewModel_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.ServiceViewModel", "CommentViewModel_Id");
            AddForeignKey("dbo.ServiceViewModel", "CommentViewModel_Id", "dbo.CommentViewModel", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ServiceViewModel", "CommentViewModel_Id", "dbo.CommentViewModel");
            DropIndex("dbo.ServiceViewModel", new[] { "CommentViewModel_Id" });
            DropColumn("dbo.ServiceViewModel", "CommentViewModel_Id");
        }
    }
}
