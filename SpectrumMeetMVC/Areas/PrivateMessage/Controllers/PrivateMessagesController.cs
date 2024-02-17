using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SpectrumMeetEF;

namespace SpectrumMeetMVC.Areas.PrivateMessage.Controllers
{
    public class PrivateMessagesController : Controller
    {
        private SpectrumMeetEntities db = new SpectrumMeetEntities();

        // GET: PrivateMessage/PrivateMessages
        public ActionResult Index()
        {
            var privateMessages = db.PrivateMessages.Include(p => p.Account).Include(p => p.Account1).Include(p => p.PrivateMessage2);
            return View(privateMessages.ToList());
        }

        // GET: PrivateMessage/PrivateMessages/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SpectrumMeetEF.PrivateMessage privateMessage = db.PrivateMessages.Find(id);
            if (privateMessage == null)
            {
                return HttpNotFound();
            }
            return View(privateMessage);
        }

        // GET: PrivateMessage/PrivateMessages/Create
        public ActionResult Create()
        {
            ViewBag.SenderID = new SelectList(db.Accounts, "AccountID", "Username");
            ViewBag.ReceiverID = new SelectList(db.Accounts, "AccountID", "Username");
            ViewBag.ParentPrivateMessageID = new SelectList(db.PrivateMessages, "PrivateMessageID", "Subject");
            return View();
        }

        // POST: PrivateMessage/PrivateMessages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PrivateMessageID,Subject,Content,PostedDate,SenderID,ReceiverID,ParentPrivateMessageID")] SpectrumMeetEF.PrivateMessage privateMessage)
        {
            if (ModelState.IsValid)
            {
                db.PrivateMessages.Add(privateMessage);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SenderID = new SelectList(db.Accounts, "AccountID", "Username", privateMessage.SenderID);
            ViewBag.ReceiverID = new SelectList(db.Accounts, "AccountID", "Username", privateMessage.ReceiverID);
            ViewBag.ParentPrivateMessageID = new SelectList(db.PrivateMessages, "PrivateMessageID", "Subject", privateMessage.ParentPrivateMessageID);
            return View(privateMessage);
        }

        // GET: PrivateMessage/PrivateMessages/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SpectrumMeetEF.PrivateMessage privateMessage = db.PrivateMessages.Find(id);
            if (privateMessage == null)
            {
                return HttpNotFound();
            }
            ViewBag.SenderID = new SelectList(db.Accounts, "AccountID", "Username", privateMessage.SenderID);
            ViewBag.ReceiverID = new SelectList(db.Accounts, "AccountID", "Username", privateMessage.ReceiverID);
            ViewBag.ParentPrivateMessageID = new SelectList(db.PrivateMessages, "PrivateMessageID", "Subject", privateMessage.ParentPrivateMessageID);
            return View(privateMessage);
        }

        // POST: PrivateMessage/PrivateMessages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PrivateMessageID,Subject,Content,PostedDate,SenderID,ReceiverID,ParentPrivateMessageID")] SpectrumMeetEF.PrivateMessage privateMessage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(privateMessage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SenderID = new SelectList(db.Accounts, "AccountID", "Username", privateMessage.SenderID);
            ViewBag.ReceiverID = new SelectList(db.Accounts, "AccountID", "Username", privateMessage.ReceiverID);
            ViewBag.ParentPrivateMessageID = new SelectList(db.PrivateMessages, "PrivateMessageID", "Subject", privateMessage.ParentPrivateMessageID);
            return View(privateMessage);
        }

        // GET: PrivateMessage/PrivateMessages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SpectrumMeetEF.PrivateMessage privateMessage = db.PrivateMessages.Find(id);
            if (privateMessage == null)
            {
                return HttpNotFound();
            }
            return View(privateMessage);
        }

        // POST: PrivateMessage/PrivateMessages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SpectrumMeetEF.PrivateMessage privateMessage = db.PrivateMessages.Find(id);
            db.PrivateMessages.Remove(privateMessage);
            db.SaveChanges();
            return RedirectToAction("Index");
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
