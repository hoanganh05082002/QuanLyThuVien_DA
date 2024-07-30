using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyThuVien_DA.Models
{
    public class FITHOU_LIB_PostView
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public string HoTen { get; set; }   
        public string TieuDe {  get; set; }
        public string NoiDung { get; set; }

        public DateTime NgayTao { get; set; }
        public int TrangThai { get; set; }

    }
}