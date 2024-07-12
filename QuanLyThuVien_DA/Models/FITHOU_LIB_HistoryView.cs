using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyThuVien_DA.Models
{
    public class FITHOU_LIB_HistoryView
    {
        public int ID { get; set; }

        public int UserID { get; set; }

        public int TaiLieuID { get; set; }

        public string TenTaiLieu {  get; set; }

        public DateTime ThoiGian { get; set; }
    }
}