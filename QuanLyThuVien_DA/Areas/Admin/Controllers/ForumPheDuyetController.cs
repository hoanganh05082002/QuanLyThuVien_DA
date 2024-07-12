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

            post.TrangThai = true; // Phê duyệt bài đăng
            await db.SaveChangesAsync();
            return Json(new { success = true });
        }

        public ActionResult Index(int? page)
        {
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            var totalCount = db.FITHOU_LIB_BaiDang.Count();
            var posts = (from user in db.FITHOU_LIB_Users
                         join post in db.FITHOU_LIB_BaiDang on user.ID equals post.UserID
                         orderby post.ID
                         select new FITHOU_LIB_PostView
                         {
                             ID = post.ID,
                             HoTen = user.HoTen,
                             NoiDung = post.NoiDung,
                             TieuDe = post.TieuDe,
                             NgayTao = post.NgayTao ?? DateTime.MinValue,
                             TrangThai = post.TrangThai ?? false,
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

            db.FITHOU_LIB_BaiDang.Remove(post);
            await db.SaveChangesAsync();
            return Json(new { success = true });
        }
    }
}
