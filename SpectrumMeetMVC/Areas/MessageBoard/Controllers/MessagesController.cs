using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SpectrumMeetEF;

namespace SpectrumMeetMVC.Areas.MessageBoard.Controllers
{
    public class MessagesController : Controller
    {
        private SpectrumMeetEntities db = new SpectrumMeetEntities();

        // GET: MessageBoard/Messages
        public ActionResult Index()
        {
            var messages = db.Messages.Include(m => m.Group).Include(m => m.Message2);
            return View(messages.ToList());
        }

        // GET: MessageBoard/Messages/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Message message = db.Messages.Find(id);
            if (message == null)
            {
                return HttpNotFound();
            }
            return View(message);
        }

        // GET: MessageBoard/Messages/Create
        public ActionResult Create()
        {
            ViewBag.GroupID = new SelectList(db.Groups, "GroupID", "Name");
            ViewBag.ParentMessageID = new SelectList(db.Messages, "MessageID", "Title");
            return View();
        }

        // POST: MessageBoard/Messages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MessageID,GroupID,Title,Content,PostedDate,AccountID,ParentMessageID")] Message message)
        {
            if (ModelState.IsValid)
            {
                db.Messages.Add(message);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.GroupID = new SelectList(db.Groups, "GroupID", "Name", message.GroupID);
            ViewBag.ParentMessageID = new SelectList(db.Messages, "MessageID", "Title", message.ParentMessageID);
            return View(message);
        }

        // GET: MessageBoard/Messages/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Message message = db.Messages.Find(id);
            if (message == null)
            {
                return HttpNotFound();
            }
            ViewBag.GroupID = new SelectList(db.Groups, "GroupID", "Name", message.GroupID);
            ViewBag.ParentMessageID = new SelectList(db.Messages, "MessageID", "Title", message.ParentMessageID);
            return View(message);
        }

        // POST: MessageBoard/Messages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MessageID,GroupID,Title,Content,PostedDate,AccountID,ParentMessageID")] Message message)
        {
            if (ModelState.IsValid)
            {
                db.Entry(message).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.GroupID = new SelectList(db.Groups, "GroupID", "Name", message.GroupID);
            ViewBag.ParentMessageID = new SelectList(db.Messages, "MessageID", "Title", message.ParentMessageID);
            return View(message);
        }

        // GET: MessageBoard/Messages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Message message = db.Messages.Find(id);
            if (message == null)
            {
                return HttpNotFound();
            }
            return View(message);
        }

        // POST: MessageBoard/Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Message message = db.Messages.Find(id);
            db.Messages.Remove(message);
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
