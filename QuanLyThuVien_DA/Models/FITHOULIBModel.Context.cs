﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class FITHOU_LIBEntities : DbContext
    {
        public FITHOU_LIBEntities()
            : base("name=FITHOU_LIBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<FITHOU_LIB_Users> FITHOU_LIB_Users { get; set; }
        public virtual DbSet<FITHOU_LIB_TheLoaiTaiLieu> FITHOU_LIB_TheLoaiTaiLieu { get; set; }
        public virtual DbSet<FITHOU_LIB_TaiLieu> FITHOU_LIB_TaiLieu { get; set; }
        public virtual DbSet<FITHOU_LIB_VaiTro> FITHOU_LIB_VaiTro { get; set; }
        public virtual DbSet<FITHOU_LIB_TinTucSuKien> FITHOU_LIB_TinTucSuKien { get; set; }
        public virtual DbSet<FITHOU_LIB_BinhLuan> FITHOU_LIB_BinhLuan { get; set; }
        public virtual DbSet<FITHOU_LIB_LichSuTruyCap> FITHOU_LIB_LichSuTruyCap { get; set; }
        public virtual DbSet<FITHOU_LIB_DeXuatTheLoai> FITHOU_LIB_DeXuatTheLoai { get; set; }
        public virtual DbSet<FITHOU_LIB_BaiDang> FITHOU_LIB_BaiDang { get; set; }
        public virtual DbSet<FITHOU_LIB_ThongBao> FITHOU_LIB_ThongBao { get; set; }
    }
}
