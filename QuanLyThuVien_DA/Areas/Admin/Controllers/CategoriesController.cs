using QuanLyThuVien_DA.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace QuanLyThuVien_DA.Areas.Admin.Controllers
{
    public class CategoriesController : Controller
    {
        FITHOU_LIBEntities db = new FITHOU_LIBEntities();

        public ActionResult Index(int? page)
        {
            int pageSize = 5; 
            int pageNumber = (page ?? 1);
            var totalCount = db.FITHOU_LIB_TheLoaiTaiLieu.Count();
            var categories = (from user in db.FITHOU_LIB_Users
                              join cate in db.FITHOU_LIB_TheLoaiTaiLieu on user.ID equals cate.UserID
                              orderby cate.ID
                              select new FITHOU_LIB_CategoriesView
                              {
                                  ID = cate.ID,
                                  TenTheLoai = cate.TenTheLoai,
                                  MoTa = cate.MoTa,
                                  HoTen = user.HoTen
                              })
                              .Skip((pageNumber - 1) * pageSize)
                              .Take(pageSize)
                              .ToList();

            // Tạo đối tượng phân trang
            var pagination = new PaginationViewModel
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                PageTotal = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            ViewBag.Pagination = pagination;

            return View(categories);
        }
        [HttpPost]
        public ActionResult Create(FITHOU_LIB_TheLoaiTaiLieu cate)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.FITHOU_LIB_TheLoaiTaiLieu.Add(cate);
                    db.SaveChanges();
                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi và ghi log (nếu cần)
                    return Json(new { success = false, error = ex.Message });
                }
            }

            return Json(new { success = false });
        }

        public async Task<ActionResult> GetTheLoai(int ID)
        {
            var theloai = await db.FITHOU_LIB_TheLoaiTaiLieu.FindAsync(ID);
            if (theloai == null)
            {
                return HttpNotFound();
            }

            return Json(new FITHOU_LIB_CategoriesView
            {
                ID = theloai.ID,
                TenTheLoai = theloai.TenTheLoai,
                MoTa = theloai.MoTa,
                HoTen = db.FITHOU_LIB_Users.Find(theloai.UserID).HoTen,
                UserID = (int)theloai.UserID
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(FITHOU_LIB_TheLoaiTaiLieu cate)
        {
            if (ModelState.IsValid)
            {
                var existingCate = await db.FITHOU_LIB_TheLoaiTaiLieu.FindAsync(cate.ID);
                if (existingCate != null)
                {
                    existingCate.TenTheLoai = cate.TenTheLoai;
                    existingCate.MoTa = cate.MoTa;
                    existingCate.UserID = cate.UserID;
                    await db.SaveChangesAsync();
                    return Json(new { success = true });
                }
            }

            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int ID)
        {
            var cate = await db.FITHOU_LIB_TheLoaiTaiLieu.FindAsync(ID);
            if (cate == null)
            {
                return HttpNotFound();
            }

            //// Lấy danh sách tài liệu thuộc thể loại này
            //var documents = db.FITHOU_LIB_TaiLieu.Where(d => d.TheLoaiTaiLieuID == ID).ToList();

            //// Xóa các tài liệu thuộc thể loại này
            //foreach (var document in documents)
            //{
            //    db.FITHOU_LIB_TaiLieu.Remove(document);
            //}
            var documentsCount = await db.FITHOU_LIB_TaiLieu.CountAsync(d => d.TheLoaiTaiLieuID == ID);
            if (documentsCount > 0)
            {
                return Json(new { success = false, message = "Không thể xóa vì đã có tài liệu liên quan." });
            }

            // Xóa thể loại
            db.FITHOU_LIB_TheLoaiTaiLieu.Remove(cate);

            await db.SaveChangesAsync();
            return Json(new { success = true });
        }




    }
}
