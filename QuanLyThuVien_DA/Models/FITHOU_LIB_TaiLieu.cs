//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QuanLyThuVien_DA.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class FITHOU_LIB_TaiLieu
    {
        public int ID { get; set; }
        public Nullable<int> UserID { get; set; }
        public Nullable<int> TheLoaiTaiLieuID { get; set; }
        public string TenTaiLieu { get; set; }
        public string AnhNen { get; set; }
        public string TieuDe { get; set; }
        public string MoTa { get; set; }
        public string TacGia { get; set; }
        public string FileTaiLieu { get; set; }
        public Nullable<System.DateTime> NgayTai { get; set; }
    }
}
