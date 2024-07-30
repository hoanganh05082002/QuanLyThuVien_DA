using QuanLyThuVien_DA.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace QuanLyThuVien_DA.Controllers
{
    public class NewsController : Controller
    {
        private readonly FITHOU_LIBEntities db = new FITHOU_LIBEntities();

        // GET: News/Index/{page}/{searchTerm}
        [HttpGet]
        public ActionResult Index(int? page, string searchTerm)
        {
            int pageSize = 5;
            int pageNumber = page ?? 1;

            var newsQuery = from user in db.FITHOU_LIB_Users
                            join nw in db.FITHOU_LIB_TinTucSuKien on user.ID equals nw.UserID
                            orderby nw.NgayDang descending
                            select new FITHOU_LIB_TinTucSuKienView
                            {
                                ID = nw.ID,
                                HoTen = user.HoTen,
                                NoiDung = nw.NoiDung,
                                AnhNen = nw.AnhNen,
                                TieuDe = nw.TieuDe,
                                NgayDang = nw.NgayDang ?? DateTime.MinValue
                            };

            if (!string.IsNullOrEmpty(searchTerm))
            {
                newsQuery = newsQuery.Where(x => x.TieuDe.Contains(searchTerm));
            }

            var totalCount = newsQuery.Count();

            var news = newsQuery.Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize)
                                .ToList();

            var pagination = new PaginationViewModel
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                PageTotal = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            ViewBag.Pagination = pagination;

            // Trả về dữ liệu dưới dạng JSON nếu yêu cầu là AJAX
            if (Request.IsAjaxRequest())
            {
                return Json(news, JsonRequestBehavior.AllowGet);
            }

            // Trả về view chứa dữ liệu
            return View(news);
        }


        // GET: News/Details/{id}
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            var newsQuery = await (from user in db.FITHOU_LIB_Users
                                   join nw in db.FITHOU_LIB_TinTucSuKien on user.ID equals nw.UserID
                                   where nw.ID == id
                                   select new FITHOU_LIB_TinTucSuKienView
                                   {
                                       ID = nw.ID,
                                       HoTen = user.HoTen,
                                       NoiDung = nw.NoiDung,
                                       AnhNen = nw.AnhNen,
                                       TieuDe = nw.TieuDe,
                                       NgayDang = nw.NgayDang ?? DateTime.MinValue
                                   }).FirstOrDefaultAsync();

            if (newsQuery == null)
            {
                return HttpNotFound();
            }

            return View(newsQuery);
        }

      
    }
}
