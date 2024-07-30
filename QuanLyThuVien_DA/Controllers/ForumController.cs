﻿using DocumentFormat.OpenXml.Office2010.Excel;
using QuanLyThuVien_DA.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using System.Web.UI;

namespace QuanLyThuVien_DA.Controllers
{
    public class ForumController : Controller
    {
        FITHOU_LIBEntities db = new FITHOU_LIBEntities();
        // GET: Forum
        [HttpGet]

        public ActionResult Index(string searchTerm, int? page)
        {
            int pageSize = 5;
            int pageNumber = (page ?? 1);

            var query = db.FITHOU_LIB_BaiDang.Where(post => post.TrangThai == 1);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(post => post.TieuDe.Contains(searchTerm));
            }

            var totalCount = query.Count();

            var posts = (from user in db.FITHOU_LIB_Users
                         join post in query on user.ID equals post.UserID
                         orderby post.NgayTao descending
                         select new FITHOU_LIB_PostView
                         {
                             ID = post.ID,
                             HoTen = user.HoTen,
                             NoiDung = post.NoiDung,
                             TieuDe = post.TieuDe,
                             NgayTao = post.NgayTao ?? DateTime.MinValue,
                             TrangThai = (int)post.TrangThai
                         })
                         .Skip((pageNumber - 1) * pageSize)
                         .Take(pageSize)
                         .ToList();

            var pagination = new PaginationViewModel
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                PageTotal = (int)Math.Ceiling((double)totalCount / pageSize)
            };
            // Trả về dữ liệu dưới dạng JSON nếu yêu cầu là AJAX
            if (Request.IsAjaxRequest())
            {
                return Json(posts, JsonRequestBehavior.AllowGet);
            }

            ViewBag.Pagination = pagination;
            return View(posts);
        }


        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            // Lấy thông tin bài đăng và các bình luận, sử dụng phân trang
            var posts = await (from user in db.FITHOU_LIB_Users
                               join post in db.FITHOU_LIB_BaiDang on user.ID equals post.UserID
                               where post.ID == id
                               orderby post.ID
                               select new FITHOU_LIB_CommentPostDetails
                               {
                                   BaiDangID = post.ID,
                                   HoTen = user.HoTen,
                                   NoiDung = post.NoiDung,
                                   TieuDe = post.TieuDe,
                                   NgayTao = post.NgayTao ?? DateTime.MinValue,
                                   Comments = (from comment in db.FITHOU_LIB_BinhLuan
                                               where comment.BaiDangID == post.ID
                                               join commenter in db.FITHOU_LIB_Users on comment.UserID equals commenter.ID
                                               orderby comment.NgayTao // Sắp xếp bình luận theo ID hoặc ngày tạo
                                               select new CommentViewModel
                                               {
                                                   ID = comment.ID,
                                                   BinhLuan = comment.BinhLuan,
                                                   HoTen = commenter.HoTen,
                                                   NgayTao = comment.NgayTao ?? DateTime.MinValue
                                               })
                                               .ToList()
                               })
                               .FirstOrDefaultAsync();

            if (posts == null)
            {
                return HttpNotFound();
            }
            return View(posts);
        }

        public async Task<ActionResult> RefuseDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            // Lấy thông tin bài đăng và các bình luận, sử dụng phân trang
            var posts = await (from user in db.FITHOU_LIB_Users
                               join post in db.FITHOU_LIB_BaiDang on user.ID equals post.UserID
                               where post.ID == id
                               orderby post.ID
                               select new FITHOU_LIB_CommentPostDetails
                               {
                                   BaiDangID = post.ID,
                                   HoTen = user.HoTen,
                                   NoiDung = post.NoiDung,
                                   TieuDe = post.TieuDe,
                                   NgayTao = post.NgayTao ?? DateTime.MinValue,
                                   Comments = (from comment in db.FITHOU_LIB_BinhLuan
                                               where comment.BaiDangID == post.ID
                                               join commenter in db.FITHOU_LIB_Users on comment.UserID equals commenter.ID
                                               orderby comment.NgayTao // Sắp xếp bình luận theo ID hoặc ngày tạo
                                               select new CommentViewModel
                                               {
                                                   ID = comment.ID,
                                                   BinhLuan = comment.BinhLuan,
                                                   HoTen = commenter.HoTen,
                                                   NgayTao = comment.NgayTao ?? DateTime.MinValue
                                               })
                                               .ToList()
                               })
                               .FirstOrDefaultAsync();

            if (posts == null)
            {
                return HttpNotFound();
            }
            return View(posts);
        }
        public async Task<ActionResult> LoadComments(int? postId)
        {
            if (postId == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            var comments = await (from comment in db.FITHOU_LIB_BinhLuan
                                  where comment.BaiDangID == postId
                                  join commenter in db.FITHOU_LIB_Users on comment.UserID equals commenter.ID
                                  orderby comment.NgayTao // Sắp xếp bình luận theo ngày tạo
                                  select new CommentViewModel
                                  {
                                      ID = comment.ID,
                                      BinhLuan = comment.BinhLuan,
                                      HoTen = commenter.HoTen,
                                      NgayTao = comment.NgayTao ?? DateTime.MinValue
                                  }).ToListAsync();

            return PartialView("_LoadComments", comments);
        }



        [HttpPost]
        public async Task<ActionResult> AddComment(FITHOU_LIB_CommentPostDetails model)
        {
            if (ModelState.IsValid)
            {
                var newComment = new FITHOU_LIB_BinhLuan
                {
                    BaiDangID = model.BaiDangID,
                    UserID = Convert.ToInt32(Session["UserID"]), // Ensure the user ID is retrieved correctly
                    BinhLuan = model.BinhLuan,
                    NgayTao = DateTime.Now
                };

                db.FITHOU_LIB_BinhLuan.Add(newComment);
                await db.SaveChangesAsync();

                var comments = (from comment in db.FITHOU_LIB_BinhLuan
                                where comment.BaiDangID == model.BaiDangID
                                join user in db.FITHOU_LIB_Users on comment.UserID equals user.ID
                                select new CommentViewModel
                                {
                                    ID = comment.ID,
                                    BinhLuan = comment.BinhLuan,
                                    HoTen = user.HoTen,
                                    NgayTao = comment.NgayTao ?? DateTime.MinValue
                                }).ToList();

                // Create the HTML for the comments section
                var commentsHtml = "";
                foreach (var comment in comments)
                {
                    commentsHtml += "<div><p>" + comment.BinhLuan + "</p><p>By: " + comment.HoTen + " on " + comment.NgayTao + "</p></div>";
                }

                return Json(new { success = true, commentsHtml });
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }



        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(FITHOU_LIB_BaiDang posts)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    posts.NgayTao = DateTime.Now;
                    db.FITHOU_LIB_BaiDang.Add(posts);
                    posts.TrangThai = 0;
                    db.SaveChanges();
                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    // Handle error and log if necessary
                    return Json(new { success = false, error = ex.Message });
                }
            }

            // Return validation errors
            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }


        [HttpPost]
        [ValidateInput(false)] // Bỏ qua kiểm tra dữ liệu (nếu cần thiết)
        public async Task<ActionResult> Edit(FITHOU_LIB_BaiDang posts)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingPost = await db.FITHOU_LIB_BaiDang.FindAsync(posts.ID);
                    if (existingPost != null)
                    {
                        // Lấy danh sách các bình luận của bài đăng
                        var comments = db.FITHOU_LIB_BinhLuan.Where(c => c.BaiDangID == posts.ID).ToList();

                        // Xóa các bình luận
                        db.FITHOU_LIB_BinhLuan.RemoveRange(comments);

                        // Cập nhật thông tin bài đăng
                        existingPost.NoiDung = posts.NoiDung;
                        existingPost.TieuDe = posts.TieuDe;
                        existingPost.UserID = posts.UserID;
                        existingPost.NgayTao = DateTime.Now;
                        // Nếu sửa thì Trạng Thái sẽ là chưa duyệt
                        existingPost.TrangThai = 0;

                        // Xóa các thông báo liên quan đến bài đăng
                        var notifications = db.FITHOU_LIB_ThongBao.Where(n => n.BaiDangID == posts.ID);
                        db.FITHOU_LIB_ThongBao.RemoveRange(notifications);

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
        public ActionResult GetPost(int id)
        {
            var news = db.FITHOU_LIB_BaiDang.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }

            var result = new FITHOU_LIB_TinTucSuKienView
            {
                ID = news.ID,
                TieuDe = news.TieuDe,
                NoiDung = news.NoiDung, // Ensure the content is correctly retrieved
                UserID = (int)news.UserID,
                HoTen = db.FITHOU_LIB_Users.Find(news.UserID)?.HoTen
            };

            return Json(result, JsonRequestBehavior.AllowGet);
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

        [HttpPost]
        public async Task<ActionResult> DeleteComment(int ID)
        {
            var comments = await db.FITHOU_LIB_BinhLuan.FindAsync(ID);
            if (comments == null)
            {
                return HttpNotFound();
            }

            db.FITHOU_LIB_BinhLuan.Remove(comments);
            await db.SaveChangesAsync();
            return Json(new { success = true });
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