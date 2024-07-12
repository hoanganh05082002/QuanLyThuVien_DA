using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace QuanLyThuVien_DA.Models
{
    public class FITHOU_LIB_DocumentsView
    {
        public int ID { get; set; }
        public string HoTen { get; set; }
        public string TenTheLoai { get; set; }

        public string TenTaiLieu { get; set; }
        public string AnhNen { get; set; }
        public string TieuDe { get; set; }
        public string MoTa { get; set; }
        public string TacGia { get; set; }
        public string FileTaiLieu { get; set; }
        public DateTime NgayTai { get; set; }
        public int UserID { get; set; }
        public int TheLoaiTaiLieuID { get; set; }
    }
}