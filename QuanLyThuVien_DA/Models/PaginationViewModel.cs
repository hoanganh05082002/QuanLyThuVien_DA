using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyThuVien_DA.Models
{
    public class PaginationViewModel
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int PageTotal { get; set; }
    }
}