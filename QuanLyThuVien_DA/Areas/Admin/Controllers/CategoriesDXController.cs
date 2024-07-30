using QuanLyThuVien_DA.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyThuVien_DA.Areas.Admin.Controllers
{
    public class CategoriesDXController : Controller
    {
        FITHOU_LIBEntities db = new FITHOU_LIBEntities();

        // GET: Admin/CategoriesDX
        public ActionResult IndexPheDuyet(int? page)
        {
            int pageSize = 5;
            int pageNumber = (page ?? 1);

            // Lấy danh sách các DeXuatTheLoaiID đã được thêm vào FITHOU_LIB_TheLoaiTaiLieu
            var addedDeXuatIds = db.FITHOU_LIB_TheLoaiTaiLieu.Select(c => c.DeXuatTheLoaiID).ToList();

            var totalCount = db.FITHOU_LIB_TheLoaiTaiLieu.Count();
            var dexuattheloai = (from user in db.FITHOU_LIB_Users
                                 join dexuat in db.FITHOU_LIB_DeXuatTheLoai on user.ID equals dexuat.UserID
                                 orderby dexuat.TrangThai , dexuat.NgayDeXuat descending
                                 select new FITHOU_LIB_CategoriesView
                                 {
                                     ID = dexuat.ID,
                                     TenTheLoai = dexuat.TenTheLoai,
                                     TrangThai = (int)dexuat.TrangThai
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

            // Loại bỏ các DeXuatTheLoaiID đã được thêm khỏi SelectList
            var selectListItems = db.FITHOU_LIB_DeXuatTheLoai
                                    .Where(d => !addedDeXuatIds.Contains(d.ID) && d.TrangThai == 1)
                                    .ToList();

            ViewBag.TheLoaiList = new SelectList(selectListItems, "ID", "TenTheLoai");
            ViewBag.Pagination = pagination;

            return View(dexuattheloai);
        }


        [HttpPost]
        public ActionResult Create(FITHOU_LIB_TheLoaiTaiLieu cate)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Lấy thông tin đề xuất từ bảng FITHOU_LIB_DeXuatTheLoai
                    var deXuat = db.FITHOU_LIB_DeXuatTheLoai.Find(cate.DeXuatTheLoaiID);

                    // Kiểm tra xem đề xuất có tồn tại không
                    if (deXuat == null)
                    {
                        return Json(new { success = false, error = "Đề xuất thể loại không tồn tại." });
                    }

                    // Tạo mới đối tượng FITHOU_LIB_TheLoaiTaiLieu và gán các giá trị từ đề xuất
                    var newCate = new FITHOU_LIB_TheLoaiTaiLieu
                    {
                        UserID = cate.UserID, // Gán từ form hoặc từ dữ liệu khác
                        TenTheLoai = deXuat.TenTheLoai, // Lấy từ đề xuất
                        MoTa = cate.MoTa, // Gán từ form hoặc từ dữ liệu khác
                        DeXuatTheLoaiID = cate.DeXuatTheLoaiID // Gán từ form hoặc từ dữ liệu khác
                    };

                    // Thêm mới vào db
                    db.FITHOU_LIB_TheLoaiTaiLieu.Add(newCate);
                    db.SaveChanges();

                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi và ghi log (nếu cần)
                    return Json(new { success = false, error = ex.Message });
                }
            }

            // Trường hợp ModelState.IsValid == false
            return Json(new { success = false, error = "Dữ liệu không hợp lệ." });
        }

    }
}
