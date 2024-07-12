using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyThuVien_DA.Models
{
    public class FITHOU_LIB_DeXuatTheLoaiView
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public string HoTen {  get; set; }
        public string Email { get; set; }
        public string TenTheLoai { get; set; }
        public string LyDoDeXuat { get; set; }
        public DateTime NgayDeXuat { get; set; }
        public bool TrangThai {  get; set; }    
    }
    
}