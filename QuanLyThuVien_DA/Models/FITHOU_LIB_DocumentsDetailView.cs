using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyThuVien_DA.Models
{
  
        public class FITHOU_LIB_DocumentsDetailView
        {
            public int ID { get; set; }
            public int TheLoaiTaiLieuID { get; set; }
            public string TenTheLoai { get; set; }
            public string TenTaiLieu { get; set; }
            public string AnhNen { get; set; }
            public string TieuDe { get; set; }
            public string MoTa { get; set; }
            public string TacGia { get; set; }
            public string FileTaiLieu { get; set; }
            public DateTime NgayTai { get; set; }

            public int SoLuotTruyCap { get; set; }
            public List<FITHOU_LIB_DocumentsView> RelatedDocuments { get; set; } // Add this line
        }

    
}