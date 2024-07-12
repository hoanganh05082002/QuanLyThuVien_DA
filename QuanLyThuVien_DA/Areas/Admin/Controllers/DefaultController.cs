using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using QuanLyThuVien_DA.Models;

namespace QuanLyThuVien_DA.Areas.Admin.Controllers
{
    public class DefaultController : Controller
    {
        FITHOU_LIBEntities db = new FITHOU_LIBEntities();

        // GET: Admin/Default
        public ActionResult Index()
        {
            // Lấy dữ liệu thống kê số lượt truy cập tài liệu theo thể loại
            var accessData = db.FITHOU_LIB_TaiLieu
                .Join(db.FITHOU_LIB_TheLoaiTaiLieu,
                      tl => tl.TheLoaiTaiLieuID,
                      cate => cate.ID,
                      (tl, cate) => new
                      {
                          TheLoai = cate.TenTheLoai,
                          SoLuotTruyCap = tl.SoLuotTruyCap ?? 0
                      })
                .GroupBy(d => d.TheLoai)
                .Select(g => new
                {
                    TheLoai = g.Key,
                    AccessCount = g.Sum(d => d.SoLuotTruyCap)
                })
                .ToList();

            // Chuyển dữ liệu thành danh sách hoặc từ điển
            ViewBag.Labels = accessData.Select(d => d.TheLoai).ToList();
            ViewBag.Data = accessData.Select(d => d.AccessCount).ToList();

            return View();
        }

        public ActionResult ExportReport()
        {
            return  View();
        }

        public ActionResult TongQuan()
        {
                // Tính tổng số lượt truy cập tài liệu
                var totalAccessCount = db.FITHOU_LIB_TaiLieu.Sum(tl => tl.SoLuotTruyCap ?? 0);

                // Đếm số lượng tài liệu
                var totalDocuments = db.FITHOU_LIB_TaiLieu.Count();

                // Đếm số lượng tin tức và sự kiện
                var totalNewsEvents = db.FITHOU_LIB_TinTucSuKien.Count();

                // Đếm số lượng bài đăng
                var totalPosts = db.FITHOU_LIB_BaiDang.Count();

                // Đưa các giá trị tính được vào ViewBag để hiển thị trên View
                ViewBag.TotalAccessCount = totalAccessCount;
                ViewBag.TotalDocuments = totalDocuments;
                ViewBag.TotalNewsEvents = totalNewsEvents;
                ViewBag.TotalPosts = totalPosts;

            return View();
        }

    }
}
