using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SecureDocumentManagementSystem.Models;
using SecureDocumentManagementSystem.Services;

namespace SecureDocumentManagementSystem.Controllers
{
    public class DocumentsController : Controller
    {
        private AppDbContext db = new AppDbContext();
        private AesEncryptionService _encryption = new AesEncryptionService();

        // GET: رفع مستند
        public ActionResult Upload()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");
            return View();
        }

        // POST: رفع مستند
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file, string title)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            if (file != null && file.ContentLength > 0)
            {
                var userId = (int)Session["UserId"];
                var fileName = Path.GetFileName(file.FileName);
                var path = Server.MapPath("~/UploadedFiles/" + Guid.NewGuid() + Path.GetExtension(fileName));

                using (var ms = new MemoryStream())
                {
                    file.InputStream.CopyTo(ms);
                    var encrypted = _encryption.Encrypt(ms.ToArray());
                    System.IO.File.WriteAllBytes(path, encrypted);
                }

                var document = new Document
                {
                    Title = title,
                    FilePath = path,
                    UploadedAt = DateTime.Now,
                    OwnerId = userId
                };

                db.Documents.Add(document);
                db.SaveChanges();

                return RedirectToAction("MyDocuments");
            }

            ViewBag.Error = "يرجى اختيار ملف.";
            return View();
        }

        // GET: مستنداتي
        public ActionResult MyDocuments()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            var userId = (int)Session["UserId"];
            var docs = db.Documents.Where(d => d.OwnerId == userId).ToList();
            return View(docs);
        }

        // GET: مشاركة معي
        public ActionResult SharedWithMe()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            var userId = (int)Session["UserId"];
            var sharedDocs = db.SharedDocuments
                .Where(s => s.SharedWithUserId == userId)
                .Select(s => s.Document)
                .ToList();

            return View(sharedDocs);
        }

        // GET: مشاركة مستند
        public ActionResult Share(int id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            // ❌ لا تستخدم Session مباشرة في LINQ
            int currentUserId = (int)Session["UserId"];

            // ✅ خزن القيمة أولاً في متغير
            var users = db.Users
                          .Where(u => u.Id != currentUserId)
                          .ToList();

            ViewBag.Users = new SelectList(users, "Id", "Username");
            return View(id);
        }



        [HttpPost]
        public ActionResult Share(int documentId, int sharedWithUserId, string permission)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            var shared = new SharedDocument
            {
                DocumentId = documentId,
                SharedWithUserId = sharedWithUserId,
                Permission = permission
            };

            db.SharedDocuments.Add(shared);
            db.SaveChanges();

            return RedirectToAction("MyDocuments");
        }



        public FileResult Download(int id)
        {
            if (Session["UserId"] == null)
                RedirectToAction("Login", "Account");

            var document = db.Documents.FirstOrDefault(d => d.Id == id);
            if (document == null)
                return null;

            byte[] encryptedBytes = System.IO.File.ReadAllBytes(document.FilePath);
            byte[] decryptedBytes = _encryption.Decrypt(encryptedBytes);

            string fileName = Path.GetFileName(document.FilePath);
            return File(decryptedBytes, "application/octet-stream", fileName);
        }


        public FileResult ViewDocument(int id)
        {
            var document = db.Documents.FirstOrDefault(d => d.Id == id);
            if (document == null || !System.IO.File.Exists(document.FilePath))
                return null;

            byte[] encryptedBytes = System.IO.File.ReadAllBytes(document.FilePath);
            byte[] decryptedBytes = _encryption.Decrypt(encryptedBytes);

            string mimeType = MimeMapping.GetMimeMapping(document.FilePath);
            return File(decryptedBytes, mimeType);
        }


        public ActionResult Delete(int id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            var document = db.Documents.Find(id);
            if (document == null)
                return HttpNotFound();

            // تأكد أن المستخدم هو صاحب الملف
            int currentUserId = (int)Session["UserId"];
            if (document.OwnerId != currentUserId)
                return new HttpUnauthorizedResult();

            // حذف الملف من السيرفر
            if (System.IO.File.Exists(document.FilePath))
                System.IO.File.Delete(document.FilePath);

            // حذف الملف من قاعدة البيانات
            db.Documents.Remove(document);
            db.SaveChanges();

            return RedirectToAction("MyDocuments");
        }


    }
}
