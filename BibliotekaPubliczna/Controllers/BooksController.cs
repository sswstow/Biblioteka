using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BibliotekaPubliczna.Models;
using Microsoft.AspNet.Identity;

namespace BibliotekaPubliczna.Controllers
{
    public class BooksController : Controller
    {
        private BooksDBContext db = new BooksDBContext();
        private ApplicationDbContext adb = new ApplicationDbContext();


        public void Refresh()
        {
            var query = from r in db.Reservations
                        where (r.EndOfReservation < DateTime.Now)
                        select r;
            List<int> listID = new List<int>();

            foreach (var item in query.ToList())
            {
                listID.Add(item.BookID);
                db.Reservations.Remove(item);
            }
            db.SaveChanges();

            var bookQry = from b in db.Books
                          select b;

            foreach (var itm in bookQry.ToList())
            {
                if (listID.Contains(itm.ID))
                {
                    itm.Quantity++;
                    db.Entry(itm).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

        }

        // GET: Books
        public ActionResult Index(string bookType, string searchString)
        {
            Refresh();

            var TypeLst = new List<string>();

            var TypeQry = from t in db.Books
                           orderby t.Type
                           select t.Type;

            TypeLst.AddRange(TypeQry.Distinct());
            ViewBag.bookType = new SelectList(TypeLst);

            var books = from b in db.Books
                         select b;

            if (!String.IsNullOrEmpty(searchString))
            {
                books = books.Where(s => s.Title.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(bookType))
            {
                books = books.Where(x => x.Type == bookType);
            }

            return View(books);
        }

        // GET: Books/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BooksViewModels booksViewModels = db.Books.Find(id);
            if (booksViewModels == null)
            {
                return HttpNotFound();
            }
            return View(booksViewModels);
        }

        [AuthLog(Roles = "Administrator")]
        // GET: Books/Create
        public ActionResult Create()
        {
            return View();
        }

        [AuthLog(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title,Author,Type,Print,Quantity")] BooksViewModels booksViewModels)
        {
            if (ModelState.IsValid)
            {
                db.Books.Add(booksViewModels);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(booksViewModels);
        }

        [AuthLog(Roles = "Administrator")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BooksViewModels booksViewModels = db.Books.Find(id);
            if (booksViewModels == null)
            {
                return HttpNotFound();
            }
            return View(booksViewModels);
        }

        [AuthLog(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title,Author,Type,Print,Quantity")] BooksViewModels booksViewModels)
        {
            if (ModelState.IsValid)
            {
                db.Entry(booksViewModels).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(booksViewModels);
        }

        [AuthLog(Roles = "Administrator")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BooksViewModels booksViewModels = db.Books.Find(id);
            if (booksViewModels == null)
            {
                return HttpNotFound();
            }
            return View(booksViewModels);
        }

        [AuthLog(Roles = "Administrator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BooksViewModels booksViewModels = db.Books.Find(id);
            db.Books.Remove(booksViewModels);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Reserve(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BooksViewModels booksViewModels = db.Books.Find(id);
            if (booksViewModels == null)
            {
                return HttpNotFound();
            }
            return View(booksViewModels);
        }

        [HttpPost, ActionName("Reserve")]
        [ValidateAntiForgeryToken]
        public ActionResult ReserveConfirmed(int id)
        {
            BooksViewModels bookViewModel = db.Books.Find(id);
            Reservation reservation;

            if (bookViewModel.Quantity == 0)
            {
                return Json(new { success = false, responseText = "Nie można zarezerwować wybranej książki!" }, JsonRequestBehavior.AllowGet);
            }
            else if (bookViewModel.Quantity < 2)
            {
                bookViewModel.Quantity = 0;
                db.Entry(bookViewModel).State = EntityState.Modified;
            }
            else
            {
                bookViewModel.Quantity--;
                db.Entry(bookViewModel).State = EntityState.Modified;
            }

            reservation = new Reservation();

            reservation.UserEmail = User.Identity.GetUserName();
            reservation.BookID = bookViewModel.ID;
            reservation.ResBookTitle = bookViewModel.Title;
            reservation.ReservationDate = DateTime.Now;
            reservation.EndOfReservation = DateTime.Now.AddHours(2);

            db.Reservations.Add(reservation);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult AllReservations()
        {
            Refresh();
            var query = from r in db.Reservations
                    select r;

            if (User.IsInRole("Administrator"))
            {

            }
            else
            {
                var user = User.Identity.GetUserName();
                query = query.Where(s => s.UserEmail == user);
            }

            return View(query);
        }

        public ActionResult Borrow(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BooksViewModels booksViewModels = db.Books.Find(id);
            if (booksViewModels == null)
            {
                return HttpNotFound();
            }
            return View(booksViewModels);
        }

        [HttpPost, ActionName("Borrow")]
        [ValidateAntiForgeryToken]
        public ActionResult BorrowConfirmed(int id)
        {
            BooksViewModels bookViewModel = db.Books.Find(id);
            BorrowedBooks borrowed;

            if (bookViewModel.Quantity == 0)
            {
                return Json(new { success = false, responseText = "Nie można wypożyczyć wybranej książki!" }, JsonRequestBehavior.AllowGet);
            }
            else if (bookViewModel.Quantity < 2)
            {
                bookViewModel.Quantity = 0;
                db.Entry(bookViewModel).State = EntityState.Modified;
            }
            else
            {
                bookViewModel.Quantity--;
                db.Entry(bookViewModel).State = EntityState.Modified;
            }

            borrowed = new BorrowedBooks();

            borrowed.UserEmail = User.Identity.GetUserName();
            borrowed.BookID = bookViewModel.ID;
            borrowed.BookTitle = bookViewModel.Title;
            borrowed.BorrowDate = DateTime.Now;

            db.Borrowed.Add(borrowed);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult AllBorrows()
        {           

            var query = from b in db.Borrowed
                        select b;

            if (User.IsInRole("Administrator"))
            {

            }
            else
            {
                var user = User.Identity.GetUserName();
                query = query.Where(s => s.UserEmail == user);
            }

            return View(query);
        }

        public ActionResult GiveBack(int id)
        {
            BorrowedBooks borrowed = db.Borrowed.Find(id);

            BooksViewModels book = db.Books.Find(borrowed.BookID);

            book.Quantity++;
            db.Entry(book).State = EntityState.Modified;
            db.Borrowed.Remove(borrowed);
            db.SaveChanges();

            return RedirectToAction("AllBorrows");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
