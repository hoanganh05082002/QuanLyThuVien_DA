using QuanLyThuVien_DA.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace QuanLyThuVien_DA.Areas.Admin.Controllers
{
    public class DocumentsController : Controller
    {
        FITHOU_LIBEntities db = new FITHOU_LIBEntities();

        // GET: Admin/Documents
        public ActionResult Index(int? page)
        {
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            var totalCount = db.FITHOU_LIB_TaiLieu.Count();
            var documents = (from user in db.FITHOU_LIB_Users
                             join cate in db.FITHOU_LIB_TheLoaiTaiLieu on user.ID equals cate.UserID
                             join docu in db.FITHOU_LIB_TaiLieu on cate.ID equals docu.TheLoaiTaiLieuID
                             orderby docu.ID
                             select new FITHOU_LIB_DocumentsView
                             {
                                 ID = docu.ID,
                                 TenTheLoai = cate.TenTheLoai,
                                 TenTaiLieu = docu.TenTaiLieu,
                                 AnhNen = docu.AnhNen,
                                 TieuDe = docu.TieuDe,
                                 MoTa = docu.MoTa,
                                 TacGia = docu.TacGia,
                                 FileTaiLieu = docu.FileTaiLieu,
                                 NgayTai = (DateTime)docu.NgayTai,
                                 HoTen = user.HoTen
                             })
                              .Skip((pageNumber - 1) * pageSize)
                              .Take(pageSize).ToList();

            var pagination = new PaginationViewModel
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                PageTotal = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            ViewBag.Pagination = pagination;

            ViewBag.TheLoaiList = new SelectList(db.FITHOU_LIB_TheLoaiTaiLieu.ToList(), "ID", "TenTheLoai");
            return View(documents);
        }

        [HttpPost]
        public ActionResult Create(FITHOU_LIB_TaiLieu document, HttpPostedFileBase AnhNen, HttpPostedFileBase FileTaiLieu)
        {
            if (ModelState.IsValid)
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
                    document.AnhNen = anhNenFileName;
                }

                // Tương tự cho FileTaiLieu
                if (FileTaiLieu != null && FileTaiLieu.ContentLength > 0)
                {
                    if (FileTaiLieu.ContentLength > 4 * 1024 * 1024) // 4MB limit
                    {
                        return Json(new { success = false, message = "File tài liệu không được vượt quá 4MB." });
                    }

                    var fileTaiLieuFileName = Path.GetFileName(FileTaiLieu.FileName);
                    var fileTaiLieuPath = Path.Combine(Server.MapPath("~/Documents"), fileTaiLieuFileName);
                    FileTaiLieu.SaveAs(fileTaiLieuPath);
                    document.FileTaiLieu = fileTaiLieuFileName;
                }

                document.NgayTai = DateTime.Now; // Set current date as upload date
                document.SoLuotTruyCap = 0;
                db.FITHOU_LIB_TaiLieu.Add(document);
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Invalid data" });
        }

        [HttpPost]
        public async Task<ActionResult> Edit(FITHOU_LIB_TaiLieu docu, HttpPostedFileBase AnhNen, HttpPostedFileBase FileTaiLieu)
        {
            if (ModelState.IsValid)
            {
                var existingDocu = await db.FITHOU_LIB_TaiLieu.FindAsync(docu.ID);
                if (existingDocu != null)
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
                        existingDocu.AnhNen = anhNenFileName;
                    }

                    // Tương tự cho FileTaiLieu
                    if (FileTaiLieu != null && FileTaiLieu.ContentLength > 0)
                    {
                        if (FileTaiLieu.ContentLength > 4 * 1024 * 1024) // 4MB limit
                        {
                            return Json(new { success = false, message = "File tài liệu không được vượt quá 4MB." });
                        }

                        var fileTaiLieuFileName = Path.GetFileName(FileTaiLieu.FileName);
                        var fileTaiLieuPath = Path.Combine(Server.MapPath("~/Documents"), fileTaiLieuFileName);
                        FileTaiLieu.SaveAs(fileTaiLieuPath);
                        existingDocu.FileTaiLieu = fileTaiLieuFileName;
                    }

                    existingDocu.TieuDe = docu.TieuDe;
                    existingDocu.TenTaiLieu = docu.TenTaiLieu;
                    existingDocu.TacGia = docu.TacGia;
                    existingDocu.MoTa = docu.MoTa;
                    existingDocu.TheLoaiTaiLieuID = docu.TheLoaiTaiLieuID;
                    existingDocu.UserID = docu.UserID;

                    await db.SaveChangesAsync();
                    return Json(new { success = true });
                }
            }

            return Json(new { success = false });
        }

        public ActionResult Delete()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int ID)
        {
            var docu  = await db.FITHOU_LIB_TaiLieu.FindAsync(ID);
            if (docu == null)
            {
                return HttpNotFound();
            }

            db.FITHOU_LIB_TaiLieu.Remove(docu);
            await db.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpGet]
        public ActionResult GetDocumentById(int id)
        {
            var document = db.FITHOU_LIB_TaiLieu.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }

            return Json(new FITHOU_LIB_DocumentsView
            {
                ID = document.ID,
                TieuDe = document.TieuDe,
                TenTaiLieu = document.TenTaiLieu,
                MoTa = document.MoTa,
                TacGia = document.TacGia,
                AnhNen = Url.Content("~/Image/" + document.AnhNen), // Điều chỉnh đường dẫn
                FileTaiLieu = Url.Content("~/Documents/" + document.FileTaiLieu), // Điều chỉnh đường dẫn
                TheLoaiTaiLieuID = db.FITHOU_LIB_TheLoaiTaiLieu.Find(document.TheLoaiTaiLieuID).ID
                // Thêm các trường cần thiết khác tại đây
            }, JsonRequestBehavior.AllowGet);
        }


      
    }
}
