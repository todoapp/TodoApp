namespace TodoApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateTodoItem : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TodoItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        ApplicationUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .Index(t => t.ApplicationUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TodoItems", "ApplicationUserId", "dbo.AspNetUsers");
            DropIndex("dbo.TodoItems", new[] { "ApplicationUserId" });
            DropTable("dbo.TodoItems");
        }
    }
}
