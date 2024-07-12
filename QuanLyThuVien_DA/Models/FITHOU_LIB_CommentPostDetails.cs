using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyThuVien_DA.Models
{
    public class FITHOU_LIB_CommentPostDetails
    {
        public int BaiDangID { get; set; }

        public int UserID { get; set; }
        public string HoTen { get; set; }
        public string TieuDe { get; set; }
        public string NoiDung { get; set; }
        public DateTime NgayTao { get; set; }

        public List<CommentViewModel> Comments { get; set; }

        public string BinhLuan { get; set; }
    }

    public class CommentViewModel
    {
        public int ID { get; set; }
        public int BaiDangID { get; set; }
        public string BinhLuan { get; set; }

        public int UserID { get; set; }
        public string HoTen { get; set; }
        public DateTime NgayTao { get; set; }
    }

}