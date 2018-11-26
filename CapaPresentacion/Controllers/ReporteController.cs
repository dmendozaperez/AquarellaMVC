using CapaPresentacion.Bll;
using CapaDato.Reporte;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using CapaEntidad.Util;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;
using CapaEntidad.Control;

namespace CapaPresentacion.Controllers
{
    public class ReporteController : Controller
    {
       
        public string reportServerUrl = Ent_Conexion.servidorReporte;
        public ReportCredentials reportCredential = new ReportCredentials(Ent_Conexion.usuarioReporte, Ent_Conexion.passwordReporte, Ent_Conexion.dominioReporte);
        public string reportFolder = Ent_Conexion.CarpetaPlanillaReporte;
        private string gcodTda = "";
        private Dat_Combo datCbo = new Dat_Combo();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ReportePlanillaView()
        {

            Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
            string actionName = this.ControllerContext.RouteData.GetRequiredString("action");
            string controllerName = this.ControllerContext.RouteData.GetRequiredString("controller");
            string return_view = actionName + "|" + controllerName;

            if (_usuario == null)
            {
                return RedirectToAction("Login", "Control", new { returnUrl = return_view });
            }
            else
            {
                List<Ent_Combo> list = new List<Ent_Combo>();
                Ent_Combo entCombo = new Ent_Combo();
                entCombo.cbo_codigo = "0";
                entCombo.cbo_descripcion = "----Todos----";
                list.Add(entCombo);
                ViewBag.Title = "Reporte de Planilla";
                ViewBag.Grupo = datCbo.get_ListaGrupo();
                ViewBag.Estado = datCbo.get_ListaEstado();
                ViewBag.Tienda = datCbo.get_ListaTiendaXstore();
                ViewBag.Categoria = list;

                return View();
            }
        }
        public ActionResult ReportePlanillaTdaView()
        {

            Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
            string actionName = this.ControllerContext.RouteData.GetRequiredString("action");
            string controllerName = this.ControllerContext.RouteData.GetRequiredString("controller");
            string return_view = actionName + "|" + controllerName;

            if (_usuario == null)
            {
                return RedirectToAction("Login", "Control", new { returnUrl = return_view });
            }
            else
            {
                gcodTda = (String)Session["Tienda"];
                List<Ent_Combo> list = new List<Ent_Combo>();
                Ent_Combo entCombo = new Ent_Combo();
                entCombo.cbo_codigo = "0";
                entCombo.cbo_descripcion = "----Todos----";
                list.Add(entCombo);
                ViewBag.Title = "Reporte de Planilla";
                ViewBag.Grupo = datCbo.get_ListaGrupo();
                ViewBag.Estado = datCbo.get_ListaEstado();
                ViewBag.Categoria = list;
                return View();
            }

        }
        public JsonResult GenerarCombo(int Numsp, string Params)
        {
            string strJson = "";
            JsonResult jRespuesta = null;
            var serializer = new JavaScriptSerializer();
            

            switch (Numsp)
            {
                case 1:
                    strJson = datCbo.listarStr_ListaCategoria(Params);
                    jRespuesta = Json(serializer.Deserialize<List<Ent_Combo>>(strJson), JsonRequestBehavior.AllowGet);
                    break;
                case 2:
                    strJson = datCbo.listarStr_ListaSubCategoria(Params);
                    jRespuesta = Json(serializer.Deserialize<List<Ent_Combo>>(strJson), JsonRequestBehavior.AllowGet);
                    break;
                default:
                    Console.WriteLine("Default case");
                    break;
            }
            return jRespuesta;
        }

        public ActionResult ReportePlanilla(string tda, string grupo, string cate, string subcate, string estado)
        {

            Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
            string actionName = this.ControllerContext.RouteData.GetRequiredString("action");
            string controllerName = this.ControllerContext.RouteData.GetRequiredString("controller");
            string return_view = actionName + "|" + controllerName;

            if (_usuario == null)
            {
                return RedirectToAction("Login", "Control", new { returnUrl = return_view });
            }
            else
            {
                string Param_tienda = (tda == "") ? "" : tda;
                string Param_grupo = (grupo == "") ? "" : grupo;
                string Param_cate = (cate == "") ? "0" : cate;
                string Param_subcate = (subcate == "") ? "0" : subcate;
                string Param_estado = (estado == "") ? "0" : estado;

                gcodTda = (String)Session["Tienda"];

                if (gcodTda != "" && gcodTda != null)
                {
                    Param_tienda = gcodTda;
                }
                //Param_tienda = "50143";
                List<ReportParameter> paramList = new List<ReportParameter>();
                paramList.Add(new ReportParameter("cod_tda", Param_tienda, false));
                paramList.Add(new ReportParameter("Grupo", Param_grupo, false));
                paramList.Add(new ReportParameter("Categoria", Param_cate, false));
                paramList.Add(new ReportParameter("SubCategoria", Param_subcate, false));
                paramList.Add(new ReportParameter("Estado", Param_estado, false));

                // ** Procesar Reporte en Servidor
                ReportViewer reportViewer = new ReportViewer();
                reportViewer.ProcessingMode = ProcessingMode.Remote;
                reportViewer.SizeToReportContent = true;
                reportViewer.ZoomMode = ZoomMode.FullPage;
                reportViewer.Width = Unit.Percentage(300);
                reportViewer.Height = Unit.Percentage(300);
                reportViewer.AsyncRendering = true;

                reportViewer.ShowPrintButton = true;

                reportViewer.ServerReport.ReportServerCredentials = reportCredential;
                reportViewer.ServerReport.ReportServerUrl = new Uri(reportServerUrl);
                reportViewer.ServerReport.ReportPath = "/" + reportFolder + "/Rpt_PlanillaWeb";
                reportViewer.ShowParameterPrompts = false;
                reportViewer.ServerReport.SetParameters(paramList);
                reportViewer.ServerReport.Refresh();
                ViewBag.ReportViewer = reportViewer;
                ViewBag.Title = "Reporte de Planilla";

                return View();
            }

        }


    }
}