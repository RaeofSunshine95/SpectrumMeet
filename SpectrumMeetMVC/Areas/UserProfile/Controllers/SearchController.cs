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
    public class SearchController : Controller
    {
        private SpectrumMeetEntities db = new SpectrumMeetEntities();

        // GET: UserProfile/Search
        public ActionResult Index()
        {
            var users = db.Users.Include(u => u.Account);
            return View(users.ToList());
        }

        // GET: UserProfile/Search/Details/5
        public ActionResult Details(int? id)
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

        // GET: UserProfile/Search/Create
        public ActionResult Create()
        {
            ViewBag.AccountID = new SelectList(db.Accounts, "AccountID", "Username");
            return View();
        }

        // POST: UserProfile/Search/Create
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

        // GET: UserProfile/Search/Edit/5
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

        // POST: UserProfile/Search/Edit/5
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

        // GET: UserProfile/Search/Delete/5
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

        // POST: UserProfile/Search/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult UserSearch()
        {
            ViewBag.Condition = new SelectList(db.Conditions.OrderBy(c => c.ConditionID), "ConditionID", "Name");
            var users = db.Users
                .Include(u => u.Account)
                .Include(u => u.Account.ParentChilds.Select(pc => pc.Child))
                .Include(u => u.Account.ParentChilds.Select(pc => pc.Child.ChildConditions.Select(cc => cc.Condition)));
            return View(users.ToList());
        }

        public ActionResult _SearchByLocation(string loc)
        {
            var user_Location = db.Users
                .Where(us => us.City.Contains(loc) || us.State.Contains(loc));
            return PartialView("_UserSearch", user_Location.ToList());
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
