using System.Web;
using System.Web.Optimization;

namespace CapaPresentacion
{
    public class BundleConfig
    {
        // Para obtener más información sobre Bundles, visite http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                     "~/Scripts/jquery-{version}.js",
                     "~/Scripts/jquery.unobtrusive-ajax.min.js",
                     "~/Scripts/toastr.js",
                     "~/Scripts/waitingfor.js",
                     "~/Scripts/bootbox.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Utilice la versión de desarrollo de Modernizr para desarrollar y obtener información. De este modo, estará
            // preparado para la producción y podrá utilizar la herramienta de compilación disponible en http://modernizr.com para seleccionar solo las pruebas que necesite.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap2").Include(
                     "~/Scripts2/bootstrap.js",
                     "~/Scripts2/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap-select").Include(
                                "~/Scripts/bootstrap-select.js",
                                "~/Scripts/bootstrap-select.min.js",
                                "~/Scripts/script-bootstrap-select.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap-select_Select").Include(
                               "~/ScriptsSelect/bootstrap-select.js",
                               "~/ScriptsSelect/bootstrap-select.min.js",
                               "~/Scripts/script-bootstrap-select.js"));

            bundles.Add(new StyleBundle("~/Content/Bootstrap-Select_Select/css").Include(
                             "~/ContentSelect/style/bootstrap-select.css",
                             "~/ContentSelect/style/bootstrap-select.min.css",
                               "~/ContentSelect/site.css"));


            bundles.Add(new StyleBundle("~/Content/css").Include(
                     "~/Content/bootstrap.css",
                     "~/Content/toastr.css",
                     "~/Content/site.css",
                     "~/Content/font-awesome-4.5.0/css/font-awesome.min.css",
                     //"~/Content/fontawesome-free-5.12.1-web/css/all.css",
                     "~/Scripts/AdminLTE/plugins/jvectormap/jquery-jvectormap-1.2.2.css",
                     "~/Content/AdminLTE/AdminLTE.min.css",
                     "~/Content/plugins/iCheck/custom.css",
                     "~/Content/css/style.css",
                     "~/Content/AdminLTE/skins/_all-skins.min.css",
                     "~/Content/sweetalert.css",
                     "~/Content/datepicker.css",
                    // "~/ContentSelect/bootstrap-select.css",
                     "~/Content/bootstrap-select.min.css",
                     "~/Scripts/DataTables/DataTables-1.10.20/css/dataTables.bootstrap.min.css",
                     "~/Scripts/DataTables/Buttons-1.6.1/css/buttons.dataTables.min.css",
                     "~/Scripts/DataTables/Responsive-2.2.3/css/dataTables.responsive.min.css"
                     ));

            bundles.Add(new StyleBundle("~/Content2/css").Include(
                   "~/Content2/bootstrap.css",
                   "~/Content2/site.css"));

            bundles.Add(new StyleBundle("~/Content/Bootstrap-Select/css").Include(
                              "~/Content/style/bootstrap-select.css",
                              "~/Content/style/bootstrap-select.min.css"));

            bundles.Add(new ScriptBundle("~/bundles/lib").Include(
                    "~/Scripts/bootstrap.min.js",
                    "~/Scripts/Plugins/iCheck/icheck.min.js",
                    "~/Scripts/sweetalert.min.js",
                    "~/Scripts/DataTables/DataTables-1.10.20/js/jquery.dataTables.min.js",
                    "~/Scripts/DataTables/DataTables-1.10.20/js/dataTables.bootstrap.min.js",
                    "~/Scripts/DataTables/Buttons-1.6.1/js/dataTables.buttons.min.js",
                    "~/Scripts/DataTables/Buttons-1.6.1/js/buttons.colVis.min.js",
                    "~/Scripts/DataTables/Responsive-2.2.3/js/dataTables.responsive.min.js",
                    "~/Scripts/MyJs/bootstrap-datepicker.js",
                    //"~/Scripts/bootstrap-select.min.js",
                    //"~/ScriptsSelect/bootstrap-select.js",
                    "~/Scripts/AdminLTE/plugins/fastclick/fastclick.min.js",
                    "~/Scripts/AdminLTE/app.js",
                    "~/Scripts/AdminLTE/plugins/sparkline/jquery.sparkline.min.js",
                    "~/Scripts/AdminLTE/plugins/jvectormap/jquery-jvectormap-1.2.2.min.js",
                    "~/Scripts/AdminLTE/plugins/jvectormap/jquery-jvectormap-world-mill-en.js",
                    "~/Scripts/AdminLTE/plugins/daterangepicker/daterangepicker.js",
                    "~/Scripts/AdminLTE/plugins/datepicker/bootstrap-datepicker.js",
                    "~/Scripts/AdminLTE/plugins/slimScroll/jquery.slimscroll.min.js",
                    "~/Scripts/AdminLTE/plugins/chartjs/Chart.min.js",
                    "~/Scripts/MyJs/my.js"
                ));



        }
    }
}
