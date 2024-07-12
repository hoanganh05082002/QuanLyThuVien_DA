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
    public class DeXuatPheDuyetController : Controller
    {
        // GET: Admin/DeXuatTheLoai
        FITHOU_LIBEntities db = new FITHOU_LIBEntities();


        [HttpPost]
        public async Task<ActionResult> Confirm(int id)
        {
            var dexuat = await db.FITHOU_LIB_DeXuatTheLoai.FindAsync(id);
            if (dexuat == null)
            {
                return HttpNotFound();
            }

            dexuat.TrangThai = true; // Phê duyệt đề xuất
            await db.SaveChangesAsync();
            return Json(new { success = true });
        }

        public ActionResult IndexPheDuyet(int? page)
        {
            int pageSize = 5; // Số lượng mục trên mỗi trang
            int pageNumber = (page ?? 1); // Trang hiện tại

            var dexuattheloai = (from user in db.FITHOU_LIB_Users
                                 join dexuat in db.FITHOU_LIB_DeXuatTheLoai on user.ID equals dexuat.UserID
                                 orderby dexuat.ID
                                 select new FITHOU_LIB_DeXuatTheLoaiView
                                 {
                                     ID = dexuat.ID,
                                     HoTen = user.HoTen,
                                     Email = user.Email,
                                     TenTheLoai = dexuat.TenTheLoai,
                                     LyDoDeXuat = dexuat.LyDoDeXuat,
                                     NgayDeXuat = dexuat.NgayDeXuat ?? DateTime.MinValue,
                                     TrangThai = dexuat.TrangThai ?? false
                                 })
                                 .Skip((pageNumber - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToList();

            var totalCount = db.FITHOU_LIB_DeXuatTheLoai.Count();

            // Tạo đối tượng phân trang
            var pagination = new PaginationViewModel
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                PageTotal = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            ViewBag.Pagination = pagination;

            return View(dexuattheloai);
        }

        // POST: Admin/DeXuatPheDuyet/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete(int ID)
        {
            var dexuat = await db.FITHOU_LIB_DeXuatTheLoai.FindAsync(ID);
            if (dexuat == null)
            {
                return HttpNotFound();
            }

            db.FITHOU_LIB_DeXuatTheLoai.Remove(dexuat);
            await db.SaveChangesAsync();
            return Json(new { success = true });
        }

    }

}