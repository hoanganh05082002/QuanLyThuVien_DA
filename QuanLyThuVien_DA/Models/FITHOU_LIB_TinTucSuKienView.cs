using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyThuVien_DA.Models
{
    public class FITHOU_LIB_TinTucSuKienView
    {
        public int ID { get; set; }
        public int UserID {  get; set; }

        public string HoTen {  get; set; }
        public string TieuDe {  get; set; }

        public string NoiDung { get; set; }

        public string AnhNen {  get; set; }

        public DateTime NgayDang { get; set; }
    }
}