using QuanLyThuVien_DA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace QuanLyThuVien_DA.Areas.Admin.Controllers
{
    public class ForumPheDuyetController : Controller
    {
        FITHOU_LIBEntities db = new FITHOU_LIBEntities();

        [HttpPost]
        public async Task<ActionResult> Confirm(int id)
        {
            var post = await db.FITHOU_LIB_BaiDang.FindAsync(id);
            if (post == null)
            {
                return HttpNotFound();
            }

            post.TrangThai = 1; // Phê duyệt bài đăng

           var notification = new FITHOU_LIB_ThongBao
           {
               BaiDangID = post.ID,
               NoiDung = "Bài đăng của bạn đã được phê duyệt.",
               NgayTao = DateTime.Now,
               UserID = post.UserID,
               TrangThai = false
           };
            db.FITHOU_LIB_ThongBao.Add(notification);

            await db.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<ActionResult> Refuse(int id)
        {
            var post = await db.FITHOU_LIB_BaiDang.FindAsync(id);
            if (post == null)
            {
                return HttpNotFound();
            }

            post.TrangThai = 2; // Từ chối bài đăng

                // Tạo bản ghi thông báo
                var notification = new FITHOU_LIB_ThongBao
                {
                    UserID = post.UserID,
                    NoiDung = "Bài đăng của bạn đã bị từ chối",
                    BaiDangID = post.ID,
                    NgayTao = DateTime.Now,
                    TrangThai = false
                };


                // Lưu vào cơ sở dữ liệu
                db.FITHOU_LIB_ThongBao.Add(notification);
                await db.SaveChangesAsync();

            await db.SaveChangesAsync();
            return Json(new { success = true });
        }

        public ActionResult Index(int? page)
        {
            int pageSize = 5;
            int pageNumber = (page ?? 1);

            // Filter and order the posts by TrangThai and NgayTao
            var postsQuery = from user in db.FITHOU_LIB_Users
                             join post in db.FITHOU_LIB_BaiDang on user.ID equals post.UserID
                             orderby post.TrangThai, post.NgayTao descending
                             select new FITHOU_LIB_PostView
                             {
                                 ID = post.ID,
                                 HoTen = user.HoTen,
                                 NoiDung = post.NoiDung,
                                 TieuDe = post.TieuDe,
                                 NgayTao = post.NgayTao ?? DateTime.MinValue,
                                 TrangThai = (int)post.TrangThai
                             };

            // Get the total count of posts
            var totalCount = postsQuery.Count();

            // Apply pagination
            var posts = postsQuery.Skip((pageNumber - 1) * pageSize)
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
            return View(posts);
        }


        [HttpPost]
        public async Task<ActionResult> DeletePost(int id)
        {
            var post = await db.FITHOU_LIB_BaiDang.FindAsync(id);
            if (post == null)
            {
                return HttpNotFound();
            }

            // Xóa các thông báo liên quan đến bài đăng
            var notifications = db.FITHOU_LIB_ThongBao.Where(n => n.BaiDangID == id);
            db.FITHOU_LIB_ThongBao.RemoveRange(notifications);

            // Xóa bài đăng
            db.FITHOU_LIB_BaiDang.Remove(post);
            await db.SaveChangesAsync();

            return Json(new { success = true });
        }

    }
}
