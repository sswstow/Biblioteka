namespace BibliotekaPubliczna.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Reservations : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Reservations",
                c => new
                    {
                        ResID = c.Int(nullable: false, identity: true),
                        UserEmail = c.String(),
                        BookID = c.Int(nullable: false),
                        ResBookTitle = c.String(),
                        ReservationDate = c.DateTime(nullable: false),
                        EndOfReservation = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ResID);
            
            AlterColumn("dbo.BooksViewModels", "Quantity", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.BooksViewModels", "Quantity", c => c.String());
            DropTable("dbo.Reservations");
        }
    }
}
