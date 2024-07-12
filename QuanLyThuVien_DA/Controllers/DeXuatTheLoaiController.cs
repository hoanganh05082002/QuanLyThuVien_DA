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
                    var newDexuat = new FITHOU_LIB_DeXuatTheLoai
                    {
                        UserID = dexuat.UserID,
                        TenTheLoai = dexuat.TenTheLoai,
                        LyDoDeXuat = dexuat.LyDoDeXuat,
                        NgayDeXuat = DateTime.Now,
                        TrangThai = false
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

    }
}

