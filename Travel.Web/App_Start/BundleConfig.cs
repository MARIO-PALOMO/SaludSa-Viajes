using System.Web;
using System.Web.Optimization;

namespace Travel.Web
{
    public class BundleConfig
    {
        // Para obtener más información sobre las uniones, visite https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

            // Utilice la versión de desarrollo de Modernizr para desarrollar y obtener información sobre los formularios. De este modo, estará
            // para la producción, use la herramienta de compilación disponible en https://modernizr.com para seleccionar solo las pruebas que necesite.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include("~/Scripts/modernizr-*"));

            //bundles.Add(new ScriptBundle("~/bundles/css").Include("~/Content/css/bootstrap.min.css",
            //    "~/Content/css/bootstrap-select.min.css",
            //    "~/Content/css/line-awesome.css",
            //    "~/Content/css/owl.carousel.min.css",
            //    "~/Content/css/owl.theme.default.min.css",
            //    "~/Content/css/daterangepicker.css",
            //    "~/Content/css/animated-headline.css",
            //    "~/Content/css/flag-icon.min.css",
            //    "~/Content/css/style.css"));

            //bundles.Add(new ScriptBundle("~/bundles/js").Include("~/Content/js/jquery-3.4.1.min.js",
            //                            "~/Content/js/jquery-ui.js",
            //                            "~/Content/js/popper.min.js",
            //                            "~/Content/js/bootstrap.min.js",
            //                            "~/Content/js/bootstrap-select.min.js",
            //                            "~/Content/js/moment.min.js",
            //                            "~/Content/js/daterangepicker.js",
            //                            "~/Content/js/owl.carousel.min.js",
            //                            "~/Content/js/jquery.fancybox.min.js",
            //                            "~/Content/js/jquery.countTo.min.js",
            //                            "~/Content/js/animated-headline.js",
            //                            "~/Content/js/jquery.ripples-min.js",
            //                            "~/Content/js/main.js"));

            //bundles.IgnoreList.Clear();

            BundleTable.EnableOptimizations = true;

        }
    }
}





