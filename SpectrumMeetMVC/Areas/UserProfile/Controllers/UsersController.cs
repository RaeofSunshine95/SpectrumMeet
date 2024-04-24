using SpectrumMeetEF;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;

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

            ViewBag.SupportLevels = new SelectList(db.SupportLevels, "LevelID", "Name");

            ViewBag.AccountID = new SelectList(db.Accounts, "AccountID", "Username", user.AccountID);


            var conditions = db.Conditions.ToList();

            // Create a list of SelectListItem objects
            var conditionOptions = conditions.Select(c => new SelectListItem
            {
                Value = c.ConditionID.ToString(),
                Text = c.Name
            }).ToList();

            // Pass the condition options to the view
            ViewBag.ConditionOptions = conditionOptions;


            return View(user);
        }

        // POST: UserProfile/Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AccountID,FirstName,LastName,Email,City,State,Birthday")] User user, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                if (user != null)
                {
                   
                    db.Entry(user).State = EntityState.Modified;


                    foreach (var parentChild in user.Account.ParentChilds)
                    {
                        // Update the child entity
                        //db.Entry(parentChild.Child).State = EntityState.Modified;

                        //if (profilePicture != null && profilePicture.ContentLength > 0)
                        //{
                        //    var fileName = Path.GetFileName(profilePicture.FileName);
                        //    var path = Path.Combine(Server.MapPath("~/ProfilePictures"), fileName);
                        //    profilePicture.SaveAs(path);
                        //    user.ProfilePicturePath = "~/ProfilePictures/" + fileName;
                        //} TODO make pics upload 


                        // Update the child conditions
                       if (parentChild.Child.ChildConditions != null)
                        {
                            parentChild.Child.ChildConditions.Clear(); // Clear existing conditions
                            foreach (var conditionId in parentChild.Child.ChildConditions)
                            {
                                var condition = db.Conditions.Find(conditionId);
                                if (condition != null)
                                {
                                    parentChild.Child.ChildConditions.Add(conditionId);
                                }
                            }
                        }
                    }

                    // Save changes to the database
                    db.SaveChanges();
                }
                else
                {
                    // Handle null objects gracefully
                    // You can add logging or other error handling here if needed
                }

                // Redirect to the details page with the updated user's ID
                return RedirectToAction("Details", new { id = user.AccountID });
            }
            else
            {
                // If the ModelState is not valid, return to the edit view with the current user object
                ViewBag.AccountID = new SelectList(db.Accounts, "AccountID", "Username", user.AccountID);
                return View(user);
            }
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
        public ActionResult Details(int? id)
        {
            if (id != null || Session["AccountID"] != null)
            {
                var accountId = 0;
                if (id == null)
                {
                    accountId = (int)Session["AccountID"];
                }
                else
                {
                    accountId = (int)id;
                }
                var userProfile = db.Users
                    .Include(u => u.Account.ParentChilds.Select(pc => pc.Child))
                    .Include(u => u.Account.ParentChilds.Select(pc => pc.Child.ChildConditions.Select(cc => cc.Condition)))

                    //add another include to have the descrptions and conditions and stuff for the child TODO
                    .FirstOrDefault(u => u.AccountID == accountId);

                return View(userProfile);
            }
            else
            {
                return RedirectToAction("Login", null, new { area = "Administration", controller = "Accounts" });
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
