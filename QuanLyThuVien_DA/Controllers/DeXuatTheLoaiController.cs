using QuanLyThuVien_DA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace QuanLyThuVien_DA.Controllers
{
    public class DeXuatTheLoaiController : Controller
    {
        // GET: DeXuatTheLoai
        private FITHOU_LIBEntities db = new FITHOU_LIBEntities();
        
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(FITHOU_LIB_DeXuatTheLoaiView dexuat)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra xem tên Thể Loại đã tồn tại trong bảng Thể Loại hay chưa
                    bool isExist = db.FITHOU_LIB_TheLoaiTaiLieu.Any(tl => tl.TenTheLoai == dexuat.TenTheLoai);
                    if (isExist)
                    {
                        return Json(new { success = false, error = "Tên Thể Loại đã tồn tại trong hệ thống." });
                    }

                    var newDexuat = new FITHOU_LIB_DeXuatTheLoai
                    {
                        UserID = dexuat.UserID,
                        TenTheLoai = dexuat.TenTheLoai,
                        LyDoDeXuat = dexuat.LyDoDeXuat,
                        NgayDeXuat = DateTime.Now,
                        TrangThai = 0
                    };

                    db.FITHOU_LIB_DeXuatTheLoai.Add(newDexuat);
                    db.SaveChanges();
                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, error = ex.Message });
                }
            }

            return Json(new { success = false, error = "Model state is invalid" });
        }


        [HttpGet]
        public ActionResult GetNotifications()
        {
            // Lấy danh sách thông báo cho người dùng hiện tại (giả sử bạn có UserID trong session)
            int userId = (int)Session["UserID"];
            var notifications = db.FITHOU_LIB_ThongBao
                                 .Where(n => n.UserID == userId)
                                 .OrderByDescending(n => n.NgayTao)
                                 .ToList();

            return PartialView("_Notification", notifications);
        }

    }
}

