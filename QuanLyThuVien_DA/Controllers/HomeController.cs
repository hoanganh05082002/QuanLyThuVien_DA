using QuanLyThuVien_DA.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace QuanLyThuVien_DA.Controllers
{
    public class HomeController : Controller
    {
        FITHOU_LIBEntities db = new FITHOU_LIBEntities();
        public ActionResult Index(int? page, int? TheLoaiTaiLieuID, string searchTerm)
        {
            int pageSize = 5;
            int pageNumber = (page ?? 1);

            var query = from cate in db.FITHOU_LIB_TheLoaiTaiLieu
                        join docu in db.FITHOU_LIB_TaiLieu on cate.ID equals docu.TheLoaiTaiLieuID
                        select new FITHOU_LIB_DocumentsView
                        {
                            ID = docu.ID,
                            TheLoaiTaiLieuID = cate.ID,
                            TenTheLoai = cate.TenTheLoai,
                            TenTaiLieu = docu.TenTaiLieu,
                            AnhNen = docu.AnhNen,
                            TieuDe = docu.TieuDe,
                            MoTa = docu.MoTa,
                            TacGia = docu.TacGia,
                            FileTaiLieu = docu.FileTaiLieu,
                            NgayTai = (DateTime)docu.NgayTai,
                        };

            // Apply filter by category
            if (TheLoaiTaiLieuID.HasValue)
            {
                query = query.Where(x => x.TheLoaiTaiLieuID == TheLoaiTaiLieuID.Value);
            }

            // Apply search term filter
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(x => x.TenTaiLieu.Contains(searchTerm));
            }

            var totalCount = query.Count();
            var documents = query.OrderBy(x => x.ID)
                                 .Skip((pageNumber - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToList();

            var pagination = new PaginationViewModel
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                PageTotal = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            ViewBag.TheLoaiList = new SelectList(db.FITHOU_LIB_TheLoaiTaiLieu.ToList(), "ID", "TenTheLoai");
            ViewBag.Pagination = pagination;
            ViewBag.TheLoaiTaiLieuID = TheLoaiTaiLieuID;
            ViewBag.SearchTerm = searchTerm;

            return View(documents);
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var document = await (from docu in db.FITHOU_LIB_TaiLieu
                                  join cate in db.FITHOU_LIB_TheLoaiTaiLieu on docu.TheLoaiTaiLieuID equals cate.ID
                                  where docu.ID == id
                                  select new FITHOU_LIB_DocumentsDetailView
                                  {
                                      ID = docu.ID,
                                      TenTheLoai = cate.TenTheLoai,
                                      TenTaiLieu = docu.TenTaiLieu,
                                      AnhNen = docu.AnhNen,
                                      TieuDe = docu.TieuDe,
                                      MoTa = docu.MoTa,
                                      TacGia = docu.TacGia,
                                      TheLoaiTaiLieuID = cate.ID,
                                      FileTaiLieu = docu.FileTaiLieu,
                                      NgayTai = (DateTime)docu.NgayTai,
                                      SoLuotTruyCap = (int)docu.SoLuotTruyCap
                                  }).FirstOrDefaultAsync();

            if (document == null)
            {
                return HttpNotFound();
            }

            // Tăng số lượt truy cập của tài liệu lên 1
            document.SoLuotTruyCap++;

            // Cập nhật vào cơ sở dữ liệu
            var taiLieuToUpdate = await db.FITHOU_LIB_TaiLieu.FindAsync(id);
            if (taiLieuToUpdate != null)
            {
                taiLieuToUpdate.SoLuotTruyCap = document.SoLuotTruyCap;
                await db.SaveChangesAsync();
            }

            // Fetch related documents
            var relatedDocuments = await (from docu in db.FITHOU_LIB_TaiLieu
                                          where docu.TheLoaiTaiLieuID == document.TheLoaiTaiLieuID && docu.ID != id
                                          select new FITHOU_LIB_DocumentsView
                                          {
                                              ID = docu.ID,
                                              TenTaiLieu = docu.TenTaiLieu,
                                              AnhNen = docu.AnhNen
                                          }).Take(5).ToListAsync();

            document.RelatedDocuments = relatedDocuments; // Assign related documents

            // Lấy thông tin người dùng từ session
            var userID = Session["UserID"];
            if (userID != null)
            {
                // Tạo bản ghi lịch sử truy cập
                var lichSuTruyCap = new FITHOU_LIB_LichSuTruyCap
                {
                    UserID = (int)userID,
                    TaiLieuID = (int)id,
                    ThoiGian = DateTime.Now
                };

                // Lưu vào cơ sở dữ liệu
                db.FITHOU_LIB_LichSuTruyCap.Add(lichSuTruyCap);
                await db.SaveChangesAsync();
            }

            return View(document);
        }


        public ActionResult InforFITHOU()
        {
            return View();
        }
        public ActionResult ForgetPassword()
        {
            return View();
        }

        // GET: Home/Login

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult UserProfile(int? page)
        {
            // Retrieve userId from Session
            int? userId = Session["UserID"] as int?;
            int pageSize = 5;
            int pageNumber = (page ?? 1);

            if (userId == null)
            {
                // Handle case when userId is not found in Session
                // For example, redirect to login page or handle error
                return RedirectToAction("Login", "Home");
            }

            // Retrieve user information (Users model)
            var user = db.FITHOU_LIB_Users.Find(userId);

            if (user == null)
            {
                // Handle case when user is not found
                return HttpNotFound();
            }

            // Subquery to fetch latest access history for each document
            var latestAccessHistory = db.FITHOU_LIB_LichSuTruyCap
                                        .Where(history => history.UserID == userId)
                                        .GroupBy(history => history.TaiLieuID)
                                        .Select(group => group.OrderByDescending(history => history.ThoiGian).FirstOrDefault())
                                        .ToList();

            // Main query to join with document names
            var accessHistory = (from history in latestAccessHistory
                                 join document in db.FITHOU_LIB_TaiLieu on history.TaiLieuID equals document.ID
                                 join users in db.FITHOU_LIB_Users on history.UserID equals users.ID
                                 orderby history.ThoiGian descending
                                 select new FITHOU_LIB_HistoryView
                                 {
                                     ID = history.ID,
                                     UserID = users.ID,
                                     TaiLieuID = document.ID,
                                     TenTaiLieu = document.TenTaiLieu,
                                     ThoiGian = history.ThoiGian
                                 })
                                 .Skip((pageNumber - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToList(); // Retrieve a list of access history

            var totalCount = latestAccessHistory.Count(); // Total count based on latest access history

            var pagination = new PaginationViewModel
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                PageTotal = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            ViewBag.Pagination = pagination;

            // Store access history in ViewBag or use a ViewModel
            ViewBag.AccessHistory = accessHistory;

            // Return the view with the single Users model
            return View(user);
        }



        public ActionResult EditProfile()
        {
            return View();
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        public ActionResult GetUser()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(FITHOU_LIB_Users user)
        {
 
            var existingUser = db.FITHOU_LIB_Users
                .FirstOrDefault(u => u.MaSinhVien == user.MaSinhVien && u.TrangThai == true);

            if (existingUser != null)
            {
                // Verify the password
                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(user.MatKhauHash, existingUser.MatKhauHash);

                if (isPasswordValid)
                {
                    // Save user information in Session
                    Session["UserID"] = existingUser.ID;
                    Session["UserName"] = existingUser.HoTen;
                    Session["UserRole"] = existingUser.VaiTro;
                    Session["UserEmail"] = existingUser.Email;
                    Session["IsFirstLogin"] = existingUser.isFirstLogin;

                    if (existingUser.TrangThai == false)
                    {
                        ViewBag.ErrorMessage = "Tài khoản này đã bị vô hiệu hóa";
                        return View();
                    }

                    // Check if it's the user's first login
                    if (existingUser.isFirstLogin == false)
                    {
                        return RedirectToAction("UserProfile", "Home");
                    }

                    if (existingUser.VaiTro.ToString().Equals("Sinh viên") || existingUser.VaiTro.ToString().Equals("Giảng viên"))
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return RedirectToAction("TongQuan", "Default", new { area = "Admin" });
                    }
                }
            }

            // Handle incorrect login information (e.g., display an error message)
            ViewBag.ErrorMessage = "Mã sinh viên hoặc mật khẩu không đúng.";
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ChangePassword(FITHOU_LIB_Users user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await db.FITHOU_LIB_Users.FindAsync(user.ID);
                if (existingUser == null)
                {
                    return HttpNotFound();
                }

                // Hash the password if it has changed
                if (!string.IsNullOrEmpty(user.MatKhauHash))
                {
                    existingUser.MatKhauHash = BCrypt.Net.BCrypt.HashPassword(user.MatKhauHash);
                    existingUser.isFirstLogin = true; // Update the isFirstLogin flag
                }

                db.Entry(existingUser).State = EntityState.Modified;
                await db.SaveChangesAsync();

                // Update session to reflect the change
                Session["IsFirstLogin"] = true;

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
                existingUser.SoDienThoai = user.SoDienThoai;
                existingUser.Email = user.Email;

                db.Entry(existingUser).State = EntityState.Modified;
                await db.SaveChangesAsync();

                // Cập nhật lại thông tin trong session
                Session["UserName"] = existingUser.HoTen;

                return Json(new { success = true, userName = existingUser.HoTen });
            }

            return Json(new { success = false });
        }


        // GET: Home/Logout
        public ActionResult Logout()
        {
            // Xóa thông tin tài khoản khỏi Session
            Session.Remove("UserID");
            Session.Remove("UserName");
            Session.Remove("UserRole");
            Session.Remove("UserEmail");

            // Chuyển hướng đến trang đăng nhập hoặc trang chính
            return RedirectToAction("Login", "Home");
        }

        [HttpPost]
        public async Task<ActionResult> ForgetPassword(string Email)
        {
            var user = await db.FITHOU_LIB_Users.FirstOrDefaultAsync(u => u.Email == Email);
            if (user != null)
            {
                // Tạo mật khẩu mới
                string newPassword = GenerateRandomPassword();

                // Mã hóa mật khẩu mới
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

                // Cập nhật mật khẩu mới vào cơ sở dữ liệu
                user.MatKhauHash = hashedPassword;
                user.isFirstLogin = false;
                await db.SaveChangesAsync();

                // Gửi email chứa mật khẩu mới
                SendNewPasswordEmail(user.Email, newPassword);

                ViewBag.SuccessMessage = "Chúng tôi đã gửi mật khẩu mới đến địa chỉ email của bạn.";
            }
            else
            {
                ViewBag.ErrorMessage = "Không tìm thấy người dùng với địa chỉ email này.";
            }

            return View();
        }
        private void SendNewPasswordEmail(string email, string newPassword)
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("20a10010172@students.hou.edu.vn", "xfod riju hmuo iryd"),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("20a10010172@students.hou.edu.vn"),
                    Subject = "Thông báo đổi mật khẩu đến " + email,
                    Body = $"Xin chào,\n\nMật khẩu mới của bạn là : {newPassword}\n\nVui lòng đổi mật khẩu sau khi đăng nhập lại.",
                    IsBodyHtml = false,
                };

                mailMessage.To.Add(email);

                smtpClient.Send(mailMessage);

               
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Lỗi khi gửi email: " + ex.Message;
            }
        }


        private string GenerateRandomPassword(int length = 7)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            var random = new Random();
            var password = new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            return password;
        }

        public ActionResult GetDocumentsByCategory(int? TheLoaiTaiLieuID, string searchTerm, int? page)
        {
            int pageSize = 5;
            int pageNumber = (page ?? 1);

            var query = from cate in db.FITHOU_LIB_TheLoaiTaiLieu
                        join docu in db.FITHOU_LIB_TaiLieu on cate.ID equals docu.TheLoaiTaiLieuID
                        select new FITHOU_LIB_DocumentsView
                        {
                            ID = docu.ID,
                            TheLoaiTaiLieuID = cate.ID,
                            TenTheLoai = cate.TenTheLoai,
                            TenTaiLieu = docu.TenTaiLieu,
                            AnhNen = docu.AnhNen,
                            TieuDe = docu.TieuDe,
                            MoTa = docu.MoTa,
                            TacGia = docu.TacGia,
                            FileTaiLieu = docu.FileTaiLieu,
                            NgayTai = (DateTime)docu.NgayTai,
                        };

            if (TheLoaiTaiLieuID.HasValue)
            {
                query = query.Where(x => x.TheLoaiTaiLieuID == TheLoaiTaiLieuID.Value);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(x => x.TenTaiLieu.Contains(searchTerm));
            }

            var totalCount = query.Count();
            var documents = query.OrderBy(x => x.ID)
                                 .Skip((pageNumber - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToList();

            var pagination = new PaginationViewModel
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                PageTotal = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            return Json(new { documents, pagination }, JsonRequestBehavior.AllowGet);
        }



    }
}