using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyThuVien_DA.Models
{
    public partial class FITHOU_LIB_CategoriesView
    {
        public int  ID { get; set; }
        public string TenTheLoai { get; set; }
        public string MoTa { get; set; }
        public string HoTen { get; set; }
        public int UserID { get; set; }
        public int DeXuatTheLoaiID { get; set; }

        public bool TrangThai {  get; set; }

        
    }
}