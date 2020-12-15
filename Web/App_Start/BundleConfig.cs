using System.Web;
using System.Web.Optimization;

namespace SaludSA
{
    public class BundleConfig
    {
        // Para obtener más información sobre las uniones, visite https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            // *************************** BEGIN CORE PLUGINS ***************************//
            bundles.Add(new ScriptBundle("~/bundles/core_plugins_js").Include(
                      "~/Content/global/plugins/jquery.min.js",
                      "~/Content/global/plugins/bootstrap/js/bootstrap.min.js",
                      "~/Content/global/plugins/js.cookie.min.js",
                      "~/Content/global/plugins/bootstrap-hover-dropdown/bootstrap-hover-dropdown.min.js",
                      "~/Content/global/plugins/jquery-slimscroll/jquery.slimscroll.min.js",
                      "~/Content/global/plugins/jquery.blockui.min.js",
                      "~/Content/global/plugins/bootstrap-switch/js/bootstrap-switch.min.js",
                      "~/Scripts/jquery.IntOrDecimal.js"));
            // **************************** END CORE PLUGINS ****************************//


            // *********************** BEGIN PERSONAL PLUGINS ***************************//
            bundles.Add(new ScriptBundle("~/bundles/personal_plugins_js").Include(
                      "~/Content/global/plugins/bootbox/bootbox.min.js",
                      "~/Content/global/plugins/bootstrap-maxlength/bootstrap-maxlength.min.js",
                      "~/Content/global/plugins/bootstrap-toastr/toastr.min.js"));
            // ************************ END PERSONAL PLUGINS ****************************//


            // *********************** BEGIN THEME GLOBAL SCRIPTS ***********************//
            bundles.Add(new ScriptBundle("~/bundles/theme_global_scripts_js").Include(
                      "~/Content/global/scripts/app.min.js"));
            // ************************ END THEME GLOBAL SCRIPTS ************************//


            // *********************** BEGIN THEME LAYOUT SCRIPTS ***********************//
            bundles.Add(new ScriptBundle("~/bundles/theme_layout_scripts_js").Include(
                      "~/Content/layouts/layout/scripts/layout.min.js",
                      "~/Content/layouts/layout/scripts/demo.min.js",
                      "~/Content/layouts/global/scripts/quick-sidebar.min.js"));
            // ************************ END THEME LAYOUT SCRIPTS ************************//


            // *********************** BEGIN ANGULARJS FRAMEWORK ************************//
            bundles.Add(new ScriptBundle("~/bundles/angular_js").Include(
                      // core de angularjs
                      "~/AngularJS/lib/angular/angular.min.js",
                      "~/AngularJS/lib/angular-animate/angular-animate.min.js",
                      "~/AngularJS/lib/angular-sanitize/angular-sanitize.js",

                      "~/AngularJS/application.js",

                      // app_global
                      "~/AngularJS/app/Global/config/config.js",
                      "~/AngularJS/app/Global/directives/directives.js",
                      "~/AngularJS/app/Global/filters/filters.js",
                      "~/AngularJS/app/Global/services/services.js")
                      );
            // ************************ END ANGULARJS FRAMEWORK *************************//


            //*********************************************************************************************/
            //*********************************************************************************************/
            //*********************************************************************************************/

            bundles.Add(new StyleBundle("~/Content/global_mandatory_styles_css").Include(
                      "~/Content/font-face.css",
                      "~/Content/global/plugins/font-awesome/css/font-awesome.min.css",
                      "~/Content/global/plugins/simple-line-icons/simple-line-icons.min.css",
                      "~/Content/global/plugins/bootstrap/css/bootstrap.min.css",
                      "~/Content/global/plugins/bootstrap-switch/css/bootstrap-switch.min.css"));

            bundles.Add(new StyleBundle("~/Content/theme_global_styles_css").Include(
                      "~/Content/global/plugins/bootstrap-toastr/toastr.min.css",
                      "~/Content/global/css/components.min.css",
                      "~/Content/global/css/plugins.min.css"));

            bundles.Add(new StyleBundle("~/Content/theme_layout_styles_css").Include(
                      "~/Content/layouts/layout/css/layout.min.css",
                      "~/Content/layouts/layout/css/themes/blue.min.css",
                      "~/Content/layouts/layout/css/custom.min.css"));

            bundles.Add(new StyleBundle("~/Content/personal_styles_css").Include(
                      "~/Content/Site.css"));
        }
    }
}
