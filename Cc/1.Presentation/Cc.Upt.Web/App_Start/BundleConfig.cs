using System.Web.Optimization;

namespace Cc.Upt.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/js/upt-js").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.validate.js",
                        "~/Scripts/jquery-browser-mobile/jquery.browser.mobile.js",
                        "~/Scripts/bootstrap/js/bootstrap.js",
                        "~/Scripts/nanoscroller/nanoscroller.js",
                        "~/Scripts/jquery.uploadfile.min.js",
                        "~/Scripts/bootstrap-datepicker/js/bootstrap-datepicker.js",
                        "~/Scripts/bootstrap-datepicker/js/locales/bootstrap-datepicker.es.js",
                        "~/Scripts/magnific-popup/magnific-popup.js",
                        "~/Scripts/jquery-placeholder/jquery.placeholder.js",
                        "~/Scripts/theme.js",
                        "~/Scripts/theme.custom.js",
                        "~/Scripts/theme.init.js"));


            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/css/upt-css").Include(
                      "~/Content/bootstrap/css/bootstrap.css",
                      "~/Content/font-awesome/css/font-awesome.css",
                      "~/Content/magnific-popup/magnific-popup.css",
                      "~/Content/bootstrap-datepicker/datepicker3.css",
                      "~/Content/theme.css",
                      "~/Content/skins/default.css",
                      "~/Content/theme-custom.css"));
        }
    }
}
