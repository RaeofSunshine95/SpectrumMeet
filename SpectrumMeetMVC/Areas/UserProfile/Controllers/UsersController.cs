using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SpectrumMeetEF;

namespace SpectrumMeetMVC.Areas.UserProfile.Controllers
{
    public class UsersController : Controller
    {
        private SpectrumMeetEntities db = new SpectrumMeetEntities();

        // GET: UserProfile/Users
        public ActionResult Index()
        {
            var users = db.Users.Include(u => u.Account);
            return View(users.ToList());
        }

        // GET: UserProfile/Users/Details/5

        // GET: UserProfile/Users/Create
        public ActionResult Create()
        {
            ViewBag.AccountID = new SelectList(db.Accounts, "AccountID", "Username");
            return View();
        }

        // POST: UserProfile/Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AccountID,FirstName,LastName,Email,City,State,Birthday")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AccountID = new SelectList(db.Accounts, "AccountID", "Username", user.AccountID);
            return View(user);
        }

        // GET: UserProfile/Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.AccountID = new SelectList(db.Accounts, "AccountID", "Username", user.AccountID);
            return View(user);
        }

        // POST: UserProfile/Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AccountID,FirstName,LastName,Email,City,State,Birthday")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AccountID = new SelectList(db.Accounts, "AccountID", "Username", user.AccountID);
            return View(user);
        }

        // GET: UserProfile/Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: UserProfile/Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        //Bespoke details action that uses the session variable to determine the correct
        //Account to go to instead of the usual integer id parameter
        //Kinda messy but it means a null session doesn't throw a bad request error and redirects
        //to login which makes more sense
        public ActionResult Details()
        {
            if (Session["AccountID"] != null)
            {
                var userProfile = db.Users.Find(Convert.ToInt32(Session["AccountID"]));
                return View(userProfile);
            }
            else
            {
                return RedirectToAction("Login", null, new {area="Administration", controller="Accounts"});
            }
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
