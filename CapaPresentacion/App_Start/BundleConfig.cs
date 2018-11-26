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
                     "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content2/css").Include(
                   "~/Content2/bootstrap.css",
                   "~/Content2/site.css"));

            bundles.Add(new StyleBundle("~/Content/Bootstrap-Select/css").Include(
                              "~/Content/style/bootstrap-select.css",
                              "~/Content/style/bootstrap-select.min.css"));
        }
    }
}
