using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BibliotekaPubliczna.Models
{
    public class BooksViewModels
    {   
        public int ID { get; set; }

        [Display(Name = "Tytuł")] 
        public string Title { get; set; }

        [Display(Name = "Autor")]
        public string Author { get; set; }

        [Display(Name = "Gatunek")]
        public string Type { get; set; }

        [Display(Name = "Wydawnictwo")]
        public string Print { get; set; }

        [Display(Name = "Ilość Egzemplarzy")]
        public int Quantity { get; set; }

    }

    public class Reservation
    {

        [Display(Name = "Użytkownik")]
        public string UserEmail { get; set; }

        [Key]
        public int ResID { get; set; }

        public int BookID { get; set; }

        [Display(Name = "Tytuł")]
        public string ResBookTitle { get; set; }

        [Display(Name = "Data rezerwacji")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime ReservationDate { get; set; }

        [Display(Name = "Data wygaśnięcia rezerwacji")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime EndOfReservation { get; set; }
    }

    public class BorrowedBooks
    {

        [Display(Name = "Użytkownik")]
        public string UserEmail { get; set; }

        [Key]
        public int BorrowID { get; set; }

        public int BookID { get; set; }

        [Display(Name = "Tytuł")]
        public string BookTitle { get; set; }

        [Display(Name = "Data Wypożyczenia")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime BorrowDate { get; set; }
    }

    public class BooksDBContext : DbContext
    {
        public DbSet<BooksViewModels> Books { get; set; }

        public DbSet<Reservation> Reservations { get; set; }

        public DbSet<BorrowedBooks> Borrowed { get; set; }
    }
}