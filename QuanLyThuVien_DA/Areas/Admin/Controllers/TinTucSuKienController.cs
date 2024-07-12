using QuanLyThuVien_DA.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace QuanLyThuVien_DA.Areas.Admin.Controllers
{
    public class TinTucSuKienController : Controller
    {
        private FITHOU_LIBEntities db = new FITHOU_LIBEntities();

        // GET: Admin/TinTucSuKien
        public ActionResult Edit()
        {
            return View();
        }

        public ActionResult Delete()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }
        public ActionResult Index(int? page)
        {
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            var totalCount = db.FITHOU_LIB_TinTucSuKien.Count();
            var news = (from user in db.FITHOU_LIB_Users
                        join nw in db.FITHOU_LIB_TinTucSuKien on user.ID equals nw.UserID
                        orderby nw.ID
                        select new FITHOU_LIB_TinTucSuKienView
                        {
                            ID = nw.ID,
                            HoTen = user.HoTen,
                            NoiDung = nw.NoiDung,
                            AnhNen = nw.AnhNen,
                            TieuDe = nw.TieuDe,
                            NgayDang = nw.NgayDang ?? DateTime.MinValue
                        })
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

            // Create pagination object
            var pagination = new PaginationViewModel
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                PageTotal = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            ViewBag.Pagination = pagination;

            return View(news);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(FITHOU_LIB_TinTucSuKien news, HttpPostedFileBase AnhNen)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (AnhNen != null && AnhNen.ContentLength > 0)
                    {
                        if (AnhNen.ContentLength > 4 * 1024 * 1024) // 4MB limit
                        {
                            return Json(new { success = false, message = "File ảnh nền không được vượt quá 4MB." });
                        }

                        var anhNenFileName = Path.GetFileName(AnhNen.FileName);
                        var anhNenPath = Path.Combine(Server.MapPath("~/Image"), anhNenFileName);
                        AnhNen.SaveAs(anhNenPath);
                        news.AnhNen = anhNenFileName;
                    }
                    news.NgayDang = DateTime.Now;
                    db.FITHOU_LIB_TinTucSuKien.Add(news);
                    db.SaveChanges();
                    return Json(new { success = true });
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            ModelState.AddModelError(validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                    return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
                }
                catch (Exception ex)
                {
                    // Handle error and log if necessary
                    return Json(new { success = false, error = ex.Message });
                }
            }

            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Edit(FITHOU_LIB_TinTucSuKien news, HttpPostedFileBase AnhNen)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingNew = await db.FITHOU_LIB_TinTucSuKien.FindAsync(news.ID);
                    if (existingNew != null)
                    {
                        existingNew.NoiDung = news.NoiDung;
                        existingNew.TieuDe = news.TieuDe;
                        existingNew.UserID = news.UserID;

                        if (AnhNen != null && AnhNen.ContentLength > 0)
                        {
                            if (AnhNen.ContentLength > 4 * 1024 * 1024) // 4MB limit
                            {
                                return Json(new { success = false, message = "File ảnh nền không được vượt quá 4MB." });
                            }

                            var anhNenFileName = Path.GetFileName(AnhNen.FileName);
                            var anhNenPath = Path.Combine(Server.MapPath("~/Image"), anhNenFileName);
                            AnhNen.SaveAs(anhNenPath);
                            existingNew.AnhNen = anhNenFileName;
                        }
                        await db.SaveChangesAsync();
                        return Json(new { success = true });
                    }
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            ModelState.AddModelError(validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                    return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, error = ex.Message });
                }
            }

            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }


        [HttpGet]
        public ActionResult GetTinTuc(int id)
        {
            var news = db.FITHOU_LIB_TinTucSuKien.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }

            var result = new FITHOU_LIB_TinTucSuKienView
            {
                ID = news.ID,
                TieuDe = news.TieuDe,
                NoiDung = news.NoiDung, // Ensure the content is correctly retrieved
                AnhNen = Url.Content("~/Image/" + news.AnhNen),
                UserID = (int)news.UserID,
                HoTen = db.FITHOU_LIB_Users.Find(news.UserID)?.HoTen
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public async Task<ActionResult> Delete(int ID)
        {
            var news = await db.FITHOU_LIB_TinTucSuKien.FindAsync(ID);
            if (news == null)
            {
                return HttpNotFound();
            }

            db.FITHOU_LIB_TinTucSuKien.Remove(news);
            await db.SaveChangesAsync();
            return Json(new { success = true });
        }
    }
}
