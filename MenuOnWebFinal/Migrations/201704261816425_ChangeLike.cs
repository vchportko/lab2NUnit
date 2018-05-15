namespace MenuOnWebFinal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeLike : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Likes", "UserId", "dbo.User");
            DropIndex("dbo.Likes", new[] { "UserId" });
            DropPrimaryKey("dbo.Likes");
            AddColumn("dbo.Likes", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.Likes", "UserId", c => c.String(maxLength: 128));
            AddPrimaryKey("dbo.Likes", "Id");
            CreateIndex("dbo.Likes", "UserId");
            AddForeignKey("dbo.Likes", "UserId", "dbo.User", "UserId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Likes", "UserId", "dbo.User");
            DropIndex("dbo.Likes", new[] { "UserId" });
            DropPrimaryKey("dbo.Likes");
            AlterColumn("dbo.Likes", "UserId", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.Likes", "Id");
            AddPrimaryKey("dbo.Likes", new[] { "UserId", "RecipeId" });
            CreateIndex("dbo.Likes", "UserId");
            AddForeignKey("dbo.Likes", "UserId", "dbo.User", "UserId", cascadeDelete: true);
        }
    }
}
