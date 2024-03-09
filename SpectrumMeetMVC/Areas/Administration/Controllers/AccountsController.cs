using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using SpectrumMeetEF;

namespace SpectrumMeetMVC.Areas.Administration.Controllers
{
    public class AccountsController : Controller
    {
        
        private SpectrumMeetEntities db = new SpectrumMeetEntities();

        // GET: Administration/Accounts
        public ActionResult Index()
        {
            var accounts = db.Accounts.Include(a => a.Role).Include(a => a.User);
            return View(accounts.ToList());
        }

        // GET: Administration/Accounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // GET: Administration/Accounts/Create
        public ActionResult Create()
        {
            ViewBag.RoleID = new SelectList(db.Roles, "RoleID", "Name");
            ViewBag.AccountID = new SelectList(db.Users, "AccountID", "FirstName");
            return View();
        }

        // POST: Administration/Accounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AccountID,RoleID,Username,Password,Email,CreationDate")] Account account)
        {
            if (ModelState.IsValid)
            {
                db.Accounts.Add(account);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RoleID = new SelectList(db.Roles, "RoleID", "Name", account.RoleID);
            ViewBag.AccountID = new SelectList(db.Users, "AccountID", "FirstName", account.AccountID);
            return View(account);
        }

        // GET: Administration/Accounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            ViewBag.RoleID = new SelectList(db.Roles, "RoleID", "Name", account.RoleID);
            ViewBag.AccountID = new SelectList(db.Users, "AccountID", "FirstName", account.AccountID);
            return View(account);
        }

        // POST: Administration/Accounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AccountID,RoleID,Username,Password,Email,CreationDate")] Account account)
        {
            if (ModelState.IsValid)
            {
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RoleID = new SelectList(db.Roles, "RoleID", "Name", account.RoleID);
            ViewBag.AccountID = new SelectList(db.Users, "AccountID", "FirstName", account.AccountID);
            return View(account);
        }

        // GET: Administration/Accounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // POST: Administration/Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Account account = db.Accounts.Find(id);
            db.Accounts.Remove(account);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Login()
        {
            return View("Login");
        }
        [HttpPost, ActionName("Login")]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Account objAccount)
        {
            if (ModelState.IsValid)
            {
                var obj = db.Accounts.Where(a => a.Username.Equals(objAccount.Username) && a.Password.Equals(objAccount.Password)).FirstOrDefault();
                if (obj != null)
                {
                    Session["AccountID"] = obj.AccountID.ToString();
                    Session["Username"] = obj.Username.ToString();
                    return RedirectToAction("Details",null, new {area="UserProfile", controller="Users", id = obj.AccountID});
                }
            }
            return View(objAccount);
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
