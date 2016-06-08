namespace BibliotekaPubliczna.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BorrowedBooks : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BorrowedBooks",
                c => new
                    {
                        BorrowID = c.Int(nullable: false, identity: true),
                        UserEmail = c.String(),
                        BookID = c.Int(nullable: false),
                        BookTitle = c.String(),
                        BorrowDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.BorrowID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.BorrowedBooks");
        }
    }
}
