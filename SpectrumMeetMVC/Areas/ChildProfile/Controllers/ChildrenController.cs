using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using SpectrumMeetEF;

namespace SpectrumMeetMVC.Areas.ChildProfile.Controllers
{
    public class ChildrenController : Controller
    {
        private SpectrumMeetEntities db = new SpectrumMeetEntities();
        public ActionResult _ChildConditionChange(string parm)
        {
            var parms = parm.Split('|');
            if(parm.Length >= 2)
            {
                if (int.TryParse(parms[0], out var childId))
                {
                    if (int.TryParse(parms[1], out var conditionId))
                    {
                        var childCondition = db.ChildConditions
                            .FirstOrDefault(cc=>cc.ChildID == childId && cc.ConditionID == conditionId);
                        if (childCondition == null)
                        {
                            childCondition = new ChildCondition()
                            {
                                ChildID = childId,
                                ConditionID = conditionId
                            };
                            db.ChildConditions.Add(childCondition);
                        }
                        else
                        {
                            db.ChildConditions.Remove(childCondition);
                        }
                        db.SaveChanges();
                        return PartialView("Succeeded");
                    }
                }
            }
            return PartialView("Failed");
        }

        // GET: ChildProfile/Children
        public ActionResult Index()
        {
            var children = db.Children.Include(c => c.SupportLevel);
            return View(children.ToList());
        }

        // GET: ChildProfile/Children/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                ViewData["RandomImagePath"] = GetRandomImagePath();
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Child child = db.Children.Find(id);
            if (child == null)
            {
                ViewData["RandomImagePath"] = GetRandomImagePath();
                return HttpNotFound();
            }
            ViewData["RandomImagePath"] = GetRandomImagePath();
            return View(child);
        }

        // GET: ChildProfile/Children/Create
        public ActionResult Create(int? accountId)
        {

            if (accountId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            ViewBag.AccountID = accountId;
            ViewBag.LevelID = new SelectList(db.SupportLevels, "LevelID", "Name");
            return View();
        }

        // POST: ChildProfile/Children/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,BirthDate,Verbal,Description,LevelID")] Child child, int? accountId)
        {
            if (ModelState.IsValid)
            {
                // Initialize the relationship with the account
                var parentChild = new ParentChild
                {
                    AccountID = accountId ?? 0,
                    Child = child
                };

                db.ParentChilds.Add(parentChild);
                db.Children.Add(child);
                db.SaveChanges();
                return RedirectToAction("Details", "Users", new { area = "UserProfile", id = accountId });
            }


            ViewBag.LevelID = new SelectList(db.SupportLevels, "LevelID", "Name", child.LevelID);
            return View(child);
        }

        // GET: ChildProfile/Children/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Child child = db.Children.Find(id);
            
            if (child == null)
            {
                return HttpNotFound();
            }
            ViewBag.LevelID = new SelectList(db.SupportLevels, "LevelID", "Name", child.LevelID);

            child.ChildConditions.ForEach(cc => cc.IsSelected = true);
            var conditionIds = child.ChildConditions.Select(cc => cc.ConditionID).ToList();

            var conditionList = new List<SpectrumMeetEF.Condition>();
            var conditions = db.Conditions
            .Where(c => conditionIds.Contains(c.ConditionID))
            .ToList();
            foreach (var condition in conditions)
            {
                var childCondition = new SpectrumMeetEF.Condition()
                {
                    ConditionID = condition.ConditionID,
                    Name = condition.Name,
                    IsSelected = true

                };
                conditionList.Add(childCondition);
            }
            conditions = db.Conditions
                .Where(c => !conditionIds.Contains(c.ConditionID))
                .ToList();
            foreach (var condition in conditions)
            {
                var childCondition = new SpectrumMeetEF.Condition()
                {
                    ConditionID = condition.ConditionID,
                    Name = condition.Name,
                    IsSelected = false

                };
                conditionList.Add(childCondition);
            }

            ViewBag.Conditions = conditionList.OrderBy(cc => cc.Name).ToList();
            return View(child);
        }

        // POST: ChildProfile/Children/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ChildID,Name,BirthDate,Verbal,Description,LevelID")] Child child)
        {
            if (ModelState.IsValid)
            {
                db.Entry(child).State = EntityState.Modified;
                db.SaveChanges();
               return RedirectToAction("Details", "Users", new { id = child.ChildID, area = "UserProfile"});//how do i get tis to work TODO
            }
            ViewBag.LevelID = new SelectList(db.SupportLevels, "LevelID", "Name", child.LevelID);
            return View(child);
        }

        // GET: ChildProfile/Children/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Child child = db.Children.Find(id);
            if (child == null)
            {
                return HttpNotFound();
            }
            return View(child);
        }

        // POST: ChildProfile/Children/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Child child = db.Children.Find(id);
            db.Children.Remove(child);
            db.SaveChanges(); //TODO FIX HELP PLEASE
            return RedirectToAction("Details", "UserProfile", new { id = child.ChildID }); //todo
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public string GetRandomImagePath()
        {
            var imagesDirectory = Server.MapPath("~/Content/Images/");
            var images = Directory.GetFiles(imagesDirectory);
            var randomImage = images[new Random().Next(images.Length)];
            return Url.Content("~/Content/Images/" + Path.GetFileName(randomImage));
        }
    }
}
