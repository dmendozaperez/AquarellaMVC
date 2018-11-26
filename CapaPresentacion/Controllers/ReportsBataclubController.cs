using CapaDato.Reports;
using CapaEntidad.Control;
using CapaEntidad.General;
using CapaEntidad.Menu;
using CapaEntidad.Reports;
using CapaEntidad.Util;
using CapaPresentacion.Bll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CapaPresentacion.Controllers
{
    public class ReportsBataclubController : Controller
    {
        // GET: ReportsBataclub
        private Dat_Bataclub bataclub = new Dat_Bataclub();
        private string _session_listbataclub_private = "session_listbataclub_private";

        public ActionResult tabladata()
        {
            string fecini = "01-12-2017"; string fecfinc = "31-12-2017";
            //return View();
            return View(lista(Convert.ToDateTime(fecini), Convert.ToDateTime(fecfinc)));
        }
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
                    return View();
                }
                else
                {
                    return RedirectToAction("Login", "Control", new { returnUrl = return_view });
                }
            }


        }
        public PartialViewResult ListaBataclub(string fecini, string fecfinc)
        {
            return PartialView(lista(Convert.ToDateTime(fecini), Convert.ToDateTime(fecfinc)));
        }
        public List<Ent_Bataclub> lista(DateTime fechaini, DateTime fechafin)
        {
            List<Ent_Bataclub> listbataclub = bataclub.get_lista(fechaini, fechafin);
            Session[_session_listbataclub_private] = listbataclub;
            return listbataclub;
        }
        [HttpGet]
        public FileContentResult ExportToExcel()
        {
            List<Ent_Bataclub> listbataclub = (List<Ent_Bataclub>)Session[_session_listbataclub_private];


            //List<Technology> technologies = StaticData.Technologies;
            string[] columns = { "cod_tienda", "des_tienda", "semana", "fecha", "dni", "bolfac", "pares", "soles", "estado", "fecha_ing", "promocion" };
            byte[] filecontent = ExcelExportHelper.ExportExcel(listbataclub, "BATACLUB", true, columns);
            return File(filecontent, ExcelExportHelper.ExcelContentType, "bataclub.xlsx");
        }
        public ActionResult getBataclub(Ent_jQueryDataTableParams param)
        {

            /*verificar si esta null*/
            if (Session[_session_listbataclub_private] == null)
            {
                List<Ent_Bataclub> listbataclub = new List<Ent_Bataclub>();
                Session[_session_listbataclub_private] = listbataclub;
            }

            //Traer registros
            IQueryable<Ent_Bataclub> membercol = ((List<Ent_Bataclub>)(Session[_session_listbataclub_private])).AsQueryable();  //lista().AsQueryable();

            //Manejador de filtros
            int totalCount = membercol.Count();
            IEnumerable<Ent_Bataclub> filteredMembers = membercol;

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = membercol
                    .Where(m => m.cod_tienda.ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.des_tienda.ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.semana.ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.dni.ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.estado.ToUpper().Contains(param.sSearch.ToUpper()));
            }
            //Manejador de orden
            var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);
            Func<Ent_Bataclub, string> orderingFunction =
            (
            m => sortIdx == 0 ? m.cod_tienda :
            sortIdx == 1 ? m.des_tienda :
            sortIdx == 2 ? m.semana :
            sortIdx == 3 ? m.dni :
            m.estado
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
                             a.cod_tienda,
                             a.des_tienda,
                             a.semana,
                             a.fecha,
                             a.dni,
                             a.bolfac,
                             a.pares,
                             a.soles,
                             a.estado,
                             a.fecha_ing,
                             a.promocion
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