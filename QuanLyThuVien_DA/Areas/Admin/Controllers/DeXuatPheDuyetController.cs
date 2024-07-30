using DocumentFormat.OpenXml.Office2010.Excel;
using QuanLyThuVien_DA.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

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

            dexuat.TrangThai = 1; // Phê duyệt đề xuất

                // Xác định độ dài tối đa cho tiêu đề
                int maxTitleLength = 20; // hoặc bất kỳ độ dài bạn muốn

                // Cắt tiêu đề nếu dài hơn
                string truncatedTitle = dexuat.TenTheLoai.Length > maxTitleLength
                    ? dexuat.TenTheLoai.Substring(0, maxTitleLength) + "..."
                    : dexuat.TenTheLoai;

                // Tạo bản ghi thông báo
                var notification = new FITHOU_LIB_ThongBao
                {
                    UserID = dexuat.UserID,
                    NoiDung = $"Thể loại {truncatedTitle} của bạn đề xuất đã được phê duyệt.",
                    DeXuatID = dexuat.ID,
                    NgayTao = DateTime.Now,
                    TrangThai = false
                };


                // Lưu vào cơ sở dữ liệu
                db.FITHOU_LIB_ThongBao.Add(notification);
                await db.SaveChangesAsync();

            await db.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<ActionResult> Refuse(int id)
        {
            var dexuat = await db.FITHOU_LIB_DeXuatTheLoai.FindAsync(id);
            if (dexuat == null)
            {
                return HttpNotFound();
            }

            dexuat.TrangThai = 2; //Từ chối đề xuất

                // Xác định độ dài tối đa cho tiêu đề
                int maxTitleLength = 20; // hoặc bất kỳ độ dài bạn muốn

                // Cắt tiêu đề nếu dài hơn
                string truncatedTitle = dexuat.TenTheLoai.Length > maxTitleLength
                    ? dexuat.TenTheLoai.Substring(0, maxTitleLength) + "..."
                    : dexuat.TenTheLoai;

                // Tạo bản ghi thông báo
                var notification = new FITHOU_LIB_ThongBao
                {
                    UserID = dexuat.UserID,
                    NoiDung = $"Thể loại {truncatedTitle} của bạn đề xuất đã bị từ chối.",
                    DeXuatID = dexuat.ID,
                    NgayTao = DateTime.Now,
                    TrangThai = false
                };


                // Lưu vào cơ sở dữ liệu
                db.FITHOU_LIB_ThongBao.Add(notification);
                await db.SaveChangesAsync();

            await db.SaveChangesAsync();
            return Json(new { success = true });
        }

        public ActionResult IndexPheDuyet(int? page)
        {
            int pageSize = 5; // Số lượng mục trên mỗi trang
            int pageNumber = (page ?? 1); // Trang hiện tại

            var dexuattheloai = (from user in db.FITHOU_LIB_Users
                                 join dexuat in db.FITHOU_LIB_DeXuatTheLoai on user.ID equals dexuat.UserID
                                 orderby dexuat.TrangThai , dexuat.TrangThai descending
                                 select new FITHOU_LIB_DeXuatTheLoaiView
                                 {
                                     ID = dexuat.ID,
                                     HoTen = user.HoTen,
                                     Email = user.Email,
                                     TenTheLoai = dexuat.TenTheLoai,
                                     LyDoDeXuat = dexuat.LyDoDeXuat,
                                     NgayDeXuat = dexuat.NgayDeXuat ?? DateTime.MinValue,
                                     TrangThai = (int)dexuat.TrangThai
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
            // Xóa các thông báo liên quan đến bài đăng
            var notifications = db.FITHOU_LIB_ThongBao.Where(n => n.DeXuatID == ID);
            db.FITHOU_LIB_ThongBao.RemoveRange(notifications);

            db.FITHOU_LIB_DeXuatTheLoai.Remove(dexuat);
            await db.SaveChangesAsync();
            return Json(new { success = true });
        }

    }

}