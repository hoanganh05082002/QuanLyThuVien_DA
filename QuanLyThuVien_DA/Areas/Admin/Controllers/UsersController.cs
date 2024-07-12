using QuanLyThuVien_DA.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace QuanLyThuVien_DA.Areas.Admin.Controllers
{
    public class UsersController : Controller
    {
        FITHOU_LIBEntities db = new FITHOU_LIBEntities();
        // GET: Admin/Users
        [HttpPost]
        public async Task<ActionResult> Disable(int id)
        {
            var user = await db.FITHOU_LIB_Users.FindAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            user.TrangThai = false; // Vô hiệu hóa người dùng
            await db.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<ActionResult> Enable(int id)
        {
            var user = await db.FITHOU_LIB_Users.FindAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            // Thực hiện thay đổi trạng thái bật lại người dùng
            user.TrangThai = true; // Cập nhật trường Disabled thành false
            db.Entry(user).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return Json(new { success = true });
        }


        // Cập nhật hàm Index để lấy danh sách người dùng theo vai trò và trạng thái
        public ActionResult Index(int? page)
        {
            int pageSize = 5;
            int pageNumber = (page ?? 1);

            var totalCount = db.FITHOU_LIB_Users
                                .Where(u => u.VaiTro.Equals("Sinh viên") || u.VaiTro.Equals("Giảng viên"))
                                .Count();

            var users = db.FITHOU_LIB_Users
                            .Where(u => u.VaiTro.Equals("Sinh viên") || u.VaiTro.Equals("Giảng viên"))
                            .OrderBy(u => u.ID)
                            .Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

            var pagination = new PaginationViewModel
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                PageTotal = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            ViewBag.Pagination = pagination;

            return View(users);
        }


        public ActionResult IndexAdmin(int? page)
        {
            int pageSize = 5; // Số lượng bản ghi trên mỗi trang
            int pageNumber = (page ?? 1); // Trang hiện tại, mặc định là trang 1 nếu không có

            var totalCount = db.FITHOU_LIB_Users.Where(u => u.VaiTro.Equals("Thủ thư") || u.VaiTro.Equals("Trưởng khoa") || u.VaiTro.Equals("Admin")).Count();

            var users = db.FITHOU_LIB_Users
                            .Where(u => u.VaiTro.Equals("Thủ thư") || u.VaiTro.Equals("Trưởng khoa") || u.VaiTro.Equals("Admin"))
                            .OrderBy(u => u.ID) // Sắp xếp theo ID hoặc một trường khác phù hợp
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

            return View(users);
        }
        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Delete()
        {
            return View();
        }

        public ActionResult Edit()
        {
            return View();
        }

        public ActionResult GetUser()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(FITHOU_LIB_Users user)
        {
            if (ModelState.IsValid)
            {
                // Hash the password before saving
                user.MatKhauHash = BCrypt.Net.BCrypt.HashPassword(user.MatKhauHash);

                db.FITHOU_LIB_Users.Add(user);
                db.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<ActionResult> Edit(FITHOU_LIB_Users user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await db.FITHOU_LIB_Users.FindAsync(user.ID);
                if (existingUser == null)
                {
                    return HttpNotFound();
                }

                // Update user details
                existingUser.HoTen = user.HoTen;
                existingUser.MaSinhVien = user.MaSinhVien;

                // Hash the password if it has changed
                if (!string.IsNullOrEmpty(user.MatKhauHash))
                {
                    existingUser.MatKhauHash = BCrypt.Net.BCrypt.HashPassword(user.MatKhauHash);
                }

                existingUser.SoDienThoai = user.SoDienThoai;
                existingUser.VaiTro = user.VaiTro;
                existingUser.Email = user.Email;

                db.Entry(existingUser).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }


        [HttpPost]
        public async Task<ActionResult> EditProfile(FITHOU_LIB_Users user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await db.FITHOU_LIB_Users.FindAsync(user.ID);
                if (existingUser == null)
                {
                    return HttpNotFound();
                }

                // Cập nhật thông tin người dùng
                existingUser.HoTen = user.HoTen;
                existingUser.MaSinhVien = user.MaSinhVien;
                existingUser.MatKhauHash = user.MatKhauHash;
                existingUser.SoDienThoai = user.SoDienThoai;
                existingUser.VaiTro = user.VaiTro;
                existingUser.Email =  user.Email;

                db.Entry(existingUser).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        public ActionResult EditProfile()
        {
            return Json(new { success = true });
        }


        [HttpGet]
        public async Task<ActionResult> GetUser(int ID)
        {
            var user = await db.FITHOU_LIB_Users.FindAsync(ID);
            if (user == null)
            {
                return HttpNotFound();
            }

            return Json(user, JsonRequestBehavior.AllowGet);
        }


        // POST: Admin/Users/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete(int ID)
        {
            var user = await db.FITHOU_LIB_Users.FindAsync(ID);
            if (user == null)
            {
                return HttpNotFound();
            }

            db.FITHOU_LIB_Users.Remove(user);
            await db.SaveChangesAsync();
            return Json(new { success = true });
        }

    }
}
