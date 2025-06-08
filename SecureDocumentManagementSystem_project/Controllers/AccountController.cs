using System.Linq;
using System.Web.Mvc;
using SecureDocumentManagementSystem.Models;

namespace SecureDocumentManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            var user = db.Users.FirstOrDefault(u => u.Username == username);

            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                Session["UserId"] = user.Id;
                Session["Username"] = user.Username;
                Session["IsAdmin"] = user.IsAdmin;

                return RedirectToAction("Upload", "Documents");
            }

            ViewBag.Error = "اسم المستخدم أو كلمة المرور غير صحيحة";
            return View();
        }

        // GET: Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: Register
        [HttpPost]
        public ActionResult Register(string username, string email, string password)
        {
            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                IsAdmin = false
            };

            db.Users.Add(user);
            db.SaveChanges();

            return RedirectToAction("Login");
        }

        // GET: Logout
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
