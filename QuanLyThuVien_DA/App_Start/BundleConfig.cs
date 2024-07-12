using System.Web;
using System.Web.Optimization;

namespace QuanLyThuVien_DA
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new Bundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            // Bundle for JavaScript files
            bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                "~/Interface/vendor/jquery/jquery.min.js",
                "~/Interface/vendor/bootstrap/js/bootstrap.bundle.min.js",
                "~/Interface/vendor/jquery-easing/jquery.easing.min.js",
                "~/Interface/js/sb-admin-2.min.js",
                "~/Interface/vendor/chart.js/Chart.min.js",
                "~/Interface/js/demo/chart-area-demo.js",
                "~/Interface/js/demo/chart-pie-demo.js"
              ));
        }
    }
}
