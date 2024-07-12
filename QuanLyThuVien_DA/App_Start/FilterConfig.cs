using System.Web;
using System.Web.Mvc;

namespace QuanLyThuVien_DA
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
