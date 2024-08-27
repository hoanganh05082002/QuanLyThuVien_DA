using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using BoldReports.Web.ReportViewer;
using ClosedXML.Excel;
using QuanLyThuVien_DA.Models;
using Syncfusion.XlsIO;

namespace QuanLyThuVien_DA.Areas.Admin.Controllers
{
    public class DefaultController : Controller
    {
        FITHOU_LIBEntities db = new FITHOU_LIBEntities();

        // GET: Admin/Default
      public ActionResult Index(DateTime? startDate, DateTime? endDate)
        {
            // Query to get access data from the history table with optional date filtering
            var accessDataQuery = db.FITHOU_LIB_LichSuTruyCap
                .Join(db.FITHOU_LIB_TaiLieu,
                      ls => ls.TaiLieuID,
                      tl => tl.ID,
                      (ls, tl) => new { ls, tl })
                .Join(db.FITHOU_LIB_TheLoaiTaiLieu,
                      combined => combined.tl.TheLoaiTaiLieuID,
                      cate => cate.ID,
                      (combined, cate) => new
                      {
                          TheLoai = cate.TenTheLoai,
                          ThoiGian = combined.ls.ThoiGian
                      });

            if (startDate.HasValue)
            {
                accessDataQuery = accessDataQuery.Where(d => d.ThoiGian >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                accessDataQuery = accessDataQuery.Where(d => d.ThoiGian <= endDate.Value);
            }

            var accessData = accessDataQuery
                .GroupBy(d => d.TheLoai)
                .Select(g => new
                {
                    TheLoai = g.Key,
                    AccessCount = g.Count() // Count each access record
                })
                .ToList();

            ViewBag.Labels = accessData.Select(d => d.TheLoai).ToList();
            ViewBag.Data = accessData.Select(d => d.AccessCount).ToList();

            return View();
        }




        public ActionResult ExportToExcel(DateTime? startDate, DateTime? endDate)
        {
            // If no start date is provided, use the minimum date from the database
            if (!startDate.HasValue)
            {
                startDate = db.FITHOU_LIB_LichSuTruyCap.Min(ls => ls.ThoiGian);
            }

            // If no end date is provided, use the current date
            if (!endDate.HasValue)
            {
                endDate = DateTime.Now;
            }

            var accessDataQuery = db.FITHOU_LIB_LichSuTruyCap
               .Join(db.FITHOU_LIB_TaiLieu,
                     ls => ls.TaiLieuID,
                     tl => tl.ID,
                     (ls, tl) => new { ls, tl })
               .Join(db.FITHOU_LIB_TheLoaiTaiLieu,
                     combined => combined.tl.TheLoaiTaiLieuID,
                     cate => cate.ID,
                     (combined, cate) => new
                     {
                         TheLoai = cate.TenTheLoai,
                         ThoiGian = combined.ls.ThoiGian
                     });

            if (startDate.HasValue)
            {
                accessDataQuery = accessDataQuery.Where(d => d.ThoiGian >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                accessDataQuery = accessDataQuery.Where(d => d.ThoiGian <= endDate.Value);
            }

            var accessData = accessDataQuery
                .GroupBy(d => d.TheLoai)
                .Select(g => new
                {
                    TheLoai = g.Key,
                    AccessCount = g.Count() // Count each access record
                })
                .ToList();

            ViewBag.Labels = accessData.Select(d => d.TheLoai).ToList();
            ViewBag.Data = accessData.Select(d => d.AccessCount).ToList();
            string userName = (string)Session["UserName"];

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Báo cáo");

                // Set default font and size for entire worksheet
                worksheet.Style.Font.FontName = "Times New Roman";
                worksheet.Style.Font.FontSize = 14; // Increase font size for the entire form

                // Add title
                var titleCell = worksheet.Cell(1, 4);
                titleCell.Value = "Thư viện Khoa Công Nghệ Thông Tin FITHOU LIB";
                titleCell.Style.Fill.BackgroundColor = XLColor.Blue;
                titleCell.Style.Font.FontColor = XLColor.White;
                titleCell.Style.Font.Bold = true;
                titleCell.Style.Font.FontSize = 20;
                titleCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range(1, 4, 1, 5).Merge();
                worksheet.Range(1, 4, 1, 5).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                // Add "Ngày lập" and "Người lập" rows
                worksheet.Cell(2, 4).Value = "Ngày lập";
                worksheet.Cell(2, 5).Value = DateTime.Now.ToString("HH:mm dd-MM-yyyy");
                worksheet.Range(2, 4, 2, 5).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                worksheet.Cell(3, 4).Value = "Người lập";
                worksheet.Cell(3, 5).Value = userName;
                worksheet.Range(3, 4, 3, 5).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                // Add date range information
                worksheet.Cell(4, 4).Value = "Từ ngày";
                worksheet.Cell(4, 5).Value = startDate.HasValue ? startDate.Value.ToString("dd-MM-yyyy") : "N/A";
                worksheet.Range(4, 4, 4, 5).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                worksheet.Cell(5, 4).Value = "Đến ngày";
                worksheet.Cell(5, 5).Value = endDate.HasValue ? endDate.Value.ToString("dd-MM-yyyy") : "N/A";
                worksheet.Range(5, 4, 5, 5).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                // Add headers
                worksheet.Cell(6, 4).Value = "Tên thể loại tài liệu";
                worksheet.Cell(6, 5).Value = "Số lượt truy cập";
                var headerRange = worksheet.Range(6, 4, 6, 5);
                headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                worksheet.Column(4).Width = 50;
                worksheet.Column(5).Width = 50;

                // Populate data starting from row 7
                for (int i = 0; i < accessData.Count; i++)
                {
                    var dataRow = worksheet.Row(i + 7);
                    dataRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    worksheet.Cell(i + 7, 4).Value = accessData[i].TheLoai;
                    worksheet.Cell(i + 7, 5).Value = accessData[i].AccessCount;

                    worksheet.Range(i + 7, 4, i + 7, 5).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                }

                // Save workbook to MemoryStream
                MemoryStream stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                // Return the Excel file to the user
                var fileName = $"BaoCao_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }



        public JsonResult GetFilteredData(DateTime? startDate, DateTime? endDate)
        {
            var accessDataQuery = db.FITHOU_LIB_LichSuTruyCap
                .Join(db.FITHOU_LIB_TaiLieu,
                      ls => ls.TaiLieuID,
                      tl => tl.ID,
                      (ls, tl) => new { ls, tl })
                .Join(db.FITHOU_LIB_TheLoaiTaiLieu,
                      combined => combined.tl.TheLoaiTaiLieuID,
                      cate => cate.ID,
                      (combined, cate) => new
                      {
                          TheLoai = cate.TenTheLoai,
                          ThoiGian = combined.ls.ThoiGian
                      });

            if (startDate.HasValue)
            {
                accessDataQuery = accessDataQuery.Where(d => d.ThoiGian >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                accessDataQuery = accessDataQuery.Where(d => d.ThoiGian <= endDate.Value);
            }

            var accessData = accessDataQuery
                .GroupBy(d => d.TheLoai)
                .Select(g => new
                {
                    TheLoai = g.Key,
                    AccessCount = g.Count() // Count each access record
                })
                .ToList();

            var response = new
            {
                Labels = accessData.Select(d => d.TheLoai).ToList(),
                Data = accessData.Select(d => d.AccessCount).ToList()
            };

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TongQuan()
        {
            // Tính tổng số lượt truy cập tài liệu
            var totalAccessCount = db.FITHOU_LIB_TaiLieu.Sum(tl => tl.SoLuotTruyCap);

            // Đếm số lượng tài liệu
            var totalDocuments = db.FITHOU_LIB_TaiLieu.Count();

            // Đếm số lượng tin tức và sự kiện
            var totalNewsEvents = db.FITHOU_LIB_TinTucSuKien.Count();

            // Đếm số lượng bài đăng với trạng thái là 1 
            var totalPosts = db.FITHOU_LIB_BaiDang.Where(post => post.TrangThai == 1).Count();


            // Đưa các giá trị tính được vào ViewBag để hiển thị trên View
            ViewBag.TotalAccessCount = totalAccessCount;
            ViewBag.TotalDocuments = totalDocuments;
            ViewBag.TotalNewsEvents = totalNewsEvents;
            ViewBag.TotalPosts = totalPosts;

            // Lấy dữ liệu thống kê số lượt truy cập tài liệu theo thể loại
            var accessData = db.FITHOU_LIB_TaiLieu
                .Join(db.FITHOU_LIB_TheLoaiTaiLieu,
                      tl => tl.TheLoaiTaiLieuID,
                      cate => cate.ID,
                      (tl, cate) => new
                      {
                          TheLoai = cate.TenTheLoai,
                          SLTruyCap = tl.SoLuotTruyCap
                      })
                .GroupBy(d => d.TheLoai)
                .Select(g => new
                {
                    TheLoai = g.Key,
                    AccessCount = g.Sum(d => d.SLTruyCap)
                })
                .ToList();

            // Chuyển dữ liệu thành danh sách hoặc từ điển
            ViewBag.Labels = accessData.Select(d => d.TheLoai).ToList();
            ViewBag.Data = accessData.Select(d => d.AccessCount).ToList();

            return View();
        }
    }
}
