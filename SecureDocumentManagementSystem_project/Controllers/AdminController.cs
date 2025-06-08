using System.Linq;
using System.Web.Mvc;
using SecureDocumentManagementSystem.Models;

namespace SecureDocumentManagementSystem.Controllers
{
    public class AdminController : Controller
    {
        private AppDbContext db = new AppDbContext();

        public ActionResult Users()
        {
            if (Session["IsAdmin"] == null || !(bool)Session["IsAdmin"])
                return RedirectToAction("Login", "Account");

            var users = db.Users.ToList();
            return View(users);
        }

        public ActionResult MakeAdmin(int id)
        {
            var user = db.Users.Find(id);
            if (user != null)
            {
                user.IsAdmin = true;
                db.SaveChanges();
            }
            return RedirectToAction("Users");
        }

        public ActionResult RemoveAdmin(int id)
        {
            var user = db.Users.Find(id);
            if (user != null)
            {
                user.IsAdmin = false;
                db.SaveChanges();
            }
            return RedirectToAction("Users");
        }

        // عرض كل المستندات
        public ActionResult Documents()
        {
            if (Session["IsAdmin"] == null || !(bool)Session["IsAdmin"])
                return RedirectToAction("Login", "Account");

            var docs = db.Documents.Include("Owner").ToList();
            return View(docs);
        }

        // حذف مستند
        public ActionResult DeleteDocument(int id)
        {
            if (Session["IsAdmin"] == null || !(bool)Session["IsAdmin"])
                return RedirectToAction("Login", "Account");

            var doc = db.Documents.Find(id);
            if (doc != null)
            {
                if (System.IO.File.Exists(doc.FilePath))
                    System.IO.File.Delete(doc.FilePath);

                db.Documents.Remove(doc);
                db.SaveChanges();
            }

            return RedirectToAction("Documents");
        }

        public ActionResult Index()
        {
            if (Session["IsAdmin"] == null || !(bool)Session["IsAdmin"])
                return RedirectToAction("Login", "Account");

            return View();
        }


        public ActionResult DeleteUser(int id)
        {
            if (Session["IsAdmin"] == null || !(bool)Session["IsAdmin"])
                return RedirectToAction("Login", "Account");

            var currentUserId = (int)Session["UserId"];
            if (id == currentUserId)
                return RedirectToAction("Users");

            var user = db.Users.Find(id);
            if (user != null)
            {
                var docs = db.Documents.Where(d => d.OwnerId == id).ToList();
                foreach (var doc in docs)
                {
                    if (System.IO.File.Exists(doc.FilePath))
                        System.IO.File.Delete(doc.FilePath);

                    db.Documents.Remove(doc);
                }

                db.Users.Remove(user);
                db.SaveChanges();
            }

            return RedirectToAction("Users");
        }

    }
}
