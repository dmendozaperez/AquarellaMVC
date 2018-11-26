using CapaDato.ValeCompra;
using CapaDato.ReportsValeCompra;
using CapaEntidad.ValeCompra;
using CapaEntidad.ReportsValeCompra;
using CapaEntidad.Control;
using CapaEntidad.Menu;
using CapaEntidad.Util;
using CapaPresentacion.Bll;
using System;
using CapaEntidad.General;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

using System.IO;
using System.IO.Compression;
using System.Text;

namespace CapaPresentacion.Controllers
{
    public class ReportsValeCompraController : Controller
    {
        // GET: ReportsValeCompra
        private Dat_ValeCompra datvalecompra = new Dat_ValeCompra();
        private Dat_ReportValeCompra datReportvalecompra = new Dat_ReportValeCompra();
        private Dat_Cliente datCliente = new Dat_Cliente();
        private string _session_listValeCompraDetalle_private = "_session_listValeCompraDetalle_private";

        public ActionResult Index()
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
                #region<VALIDACION DE ROLES DE USUARIO>
                Boolean valida_rol = true;
                Basico valida_controller = new Basico();
                List<Ent_Menu_Items> menu = (List<Ent_Menu_Items>)Session[Ent_Global._session_menu_user];
                valida_rol = valida_controller.AccesoMenu(menu, this);
                #endregion
                if (valida_rol)
                {
                    ViewBag.cliente = datCliente.get_lista();
                    return View(lista("",""));
                }
                else
                {
                    return RedirectToAction("Login", "Control", new { returnUrl = return_view });
                }
            }
        }

        public PartialViewResult ListaValeCompra(string []dwcliente, string  estado)
        {
            Reporte_Filtro filtro = new Reporte_Filtro();
            string strListRuc = "";
            if(estado == null)
                estado = "";

            if (dwcliente == null)
                strListRuc = "";
            else {

                for (int i = 0; i < dwcliente.Length; i++) {
                    strListRuc = strListRuc + ',' + dwcliente[i];
                }
                strListRuc = strListRuc.Substring(1);

            }

            filtro.report_Estado = estado;
            filtro.report_listRuc = "";

            return PartialView(lista(strListRuc, estado));
        }

        public List<Reporte_Resultado> lista(string rucCliente, string estado)//(Reporte_Filtro filtro)
        {
            Reporte_Filtro filtro =  new Reporte_Filtro();
            filtro.report_listRuc = rucCliente;
            filtro.report_Estado = estado;
            List<Reporte_Resultado> list = datReportvalecompra.listarReporte(filtro);

            if (list != null && list.Count() > 0)
            {
                ViewBag.TotalDisponible = list[0].total_disponible;
                ViewBag.TotalConsumido = list[0].total_consumido;
            }
            else {
                ViewBag.TotalDisponible = "0.00";
                ViewBag.TotalConsumido = "0.00";
            }

            Session[_session_listValeCompraDetalle_private] = list;
            return list;
        }

        [HttpGet]
        public FileContentResult ExportToExcel()
        {
            List<Reporte_Resultado> listValeCompra = (List<Reporte_Resultado>)Session[_session_listValeCompraDetalle_private];


            //List<Technology> technologies = StaticData.Technologies;
            string[] columns = { "Institucion","Codigo", "Numero", "soles", "Estado", "Codigo_tda", "Desc_tda", "Documento", "Fecha_doc", "DNI", "Cliente" };
            byte[] filecontent = ExcelExportHelper.ExportExcel(listValeCompra, "Vales de Compra", true, columns);
            return File(filecontent, ExcelExportHelper.ExcelContentType, "ValeCompra.xlsx");
        }

        public ActionResult getValeCompra(Ent_jQueryDataTableParams param)
        {

            /*verificar si esta null*/
            if (Session[_session_listValeCompraDetalle_private] == null)
            {
                List<Reporte_Resultado> listValeCompra = new List<Reporte_Resultado>();
                Session[_session_listValeCompraDetalle_private] = listValeCompra;
            }

            //Traer registros
            IQueryable<Reporte_Resultado> membercol = ((List<Reporte_Resultado>)(Session[_session_listValeCompraDetalle_private])).AsQueryable();  //lista().AsQueryable();

            //Manejador de filtros
            int totalCount = membercol.Count();
            IEnumerable<Reporte_Resultado> filteredMembers = membercol;

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = membercol
                    .Where(m => m.Codigo_tda.ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.Desc_tda.ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.Codigo.ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.Numero.ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.Estado.ToUpper().Contains(param.sSearch.ToUpper()));
            }
            //Manejador de orden
            var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);
            Func<Reporte_Resultado, string> orderingFunction =
            (
            m => sortIdx == 0 ? m.Codigo_tda :
            sortIdx == 1 ? m.Desc_tda :
            sortIdx == 2 ? m.Codigo :
            sortIdx == 3 ? m.Numero :
            m.Estado
            );
            var sortDirection = Request["sSortDir_0"];
            if (sortDirection == "asc")
                filteredMembers = filteredMembers.OrderBy(orderingFunction);
            else
                filteredMembers = filteredMembers.OrderByDescending(orderingFunction);
            var displayMembers = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);
            var result = from a in displayMembers
                         select new
                         {
                             a.Institucion,
                             a.Codigo,
                             a.Numero,
                             a.soles,
                             a.Estado,
                             a.Codigo_tda,
                             a.Desc_tda,
                             a.Documento,
                             a.Fecha_doc,
                             a.DNI,
                             a.Cliente
                         };
            //Se devuelven los resultados por json
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalCount,
                iTotalDisplayRecords = filteredMembers.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
    }
}