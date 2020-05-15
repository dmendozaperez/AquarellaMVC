using CapaDato.Control;
using CapaEntidad.Control;
using CapaEntidad.General;
using CapaEntidad.Menu;
using CapaEntidad.Util;
using CapaPresentacion.Bll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace CapaPresentacion.Controllers
{
    public class AplicacionController : Controller
    {
        // GET: Aplicacion
        private Dat_Aplicacion aplicacion = new Dat_Aplicacion();
        private string _session_listaplicacion_private = "session_listapl_private";
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
                    return View(lista());
                }
                else
                {
                    return RedirectToAction("Login", "Control", new { returnUrl = return_view });
                }
            }
        }
        public List<Ent_Aplicacion> lista()
        {
            List<Ent_Aplicacion> listaplicacion = aplicacion.get_lista();
            Session[_session_listaplicacion_private] = listaplicacion;
            return listaplicacion;
        }
        public ActionResult Nuevo()
        {            
            return View();
        }
        [HttpPost]
        public ActionResult Nuevo(string apl_nombre, string apl_orden,
                                  string apl_controller, string apl_action)
        {

            if (apl_nombre == null) return Json(new { estado = 0 });            

             Ent_Aplicacion _aplicacion = new Ent_Aplicacion();
            Int32 ord = 0;
            Int32.TryParse(apl_orden, out ord);

            _aplicacion.apl_id = "0";
            _aplicacion.apl_nombre = apl_nombre;           
            _aplicacion.apl_orden = ord.ToString();            
            _aplicacion.apl_controller = apl_controller;
            _aplicacion.apl_action = apl_action;

            aplicacion.apl = _aplicacion;

            Boolean _valida_nuevo =aplicacion.InsertarAplicacion();
            return Json(new { estado = (_valida_nuevo) ? "1" : "-1", desmsg = (_valida_nuevo) ? "Se actualizo satisfactoriamente." : "Hubo un error al actualizar." });
        }
        public PartialViewResult ListaAplicacion()
        {
            return PartialView(lista());
        }
        public ActionResult Edit(int? id)
        {
            List<Ent_Aplicacion> listaplicacion = (List<Ent_Aplicacion>)Session[_session_listaplicacion_private];
            if (id == null || listaplicacion == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ent_Aplicacion filaaplicacion = listaplicacion.Find(x => x.apl_id == id.ToString());
                        
            return View(filaaplicacion);
        }

        [HttpPost]
        public ActionResult Edit(string apl_id, string apl_nombre, string apl_orden,
                                  string apl_controller, string apl_action)
        {

            if (apl_id == null) return Json(new { estado = 0 });

            Ent_Aplicacion _aplicacion = new Ent_Aplicacion();
            Int32 ord = 0;
            Int32.TryParse(apl_orden, out ord);

            _aplicacion.apl_id = apl_id;
            _aplicacion.apl_nombre = apl_nombre;            
            _aplicacion.apl_orden = ord.ToString();            
            _aplicacion.apl_controller = apl_controller;
            _aplicacion.apl_action = apl_action;

            aplicacion.apl = _aplicacion;

            Boolean _valida_editar = aplicacion.UpdateAplicacion();
            return Json(new { estado = (_valida_editar) ? "1" : "-1", desmsg = (_valida_editar) ? "Se actualizo satisfactoriamente." : "Hubo un error al actualizar." });
        }

        public ActionResult getListaAplicacionAjax(Ent_jQueryDataTableParams param, string actualizar)
        {

            List<Ent_Aplicacion> listaplicacion = new List<Ent_Aplicacion>();

            if (!String.IsNullOrEmpty(actualizar))
            {
                listaplicacion = lista();
                //listAtributos = datOE.get_lista_atributos();
                Session[_session_listaplicacion_private] = listaplicacion;
            }

            /*verificar si esta null*/
            if (Session[_session_listaplicacion_private] == null)
            {
                listaplicacion = new List<Ent_Aplicacion>();
                listaplicacion = lista(); //datOE.get_lista_atributos();
                if (listaplicacion == null)
                {
                    listaplicacion = new List<Ent_Aplicacion>();
                }
                Session[_session_listaplicacion_private] = listaplicacion;
            }

            //Traer registros

            IQueryable<Ent_Aplicacion> membercol = ((List<Ent_Aplicacion>)(Session[_session_listaplicacion_private])).AsQueryable();  //lista().AsQueryable();

            //Manejador de filtros
            int totalCount = membercol.Count();
            IEnumerable<Ent_Aplicacion> filteredMembers = membercol;

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = membercol
                    .Where(m => m.apl_id.ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.apl_nombre.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.apl_orden.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.apl_controller.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.apl_action.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                     );
            }
            //Manejador de orden
            var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);
            var sortDirection = Request["sSortDir_0"];
            if (param.iSortingCols > 0)
            {
                if (sortDirection == "asc")
                {
                    if (sortIdx == 0) filteredMembers = filteredMembers.OrderBy(o => o.apl_id);
                    else if (sortIdx == 1) filteredMembers = filteredMembers.OrderBy(o => o.apl_nombre);
                    else if (sortIdx == 2) filteredMembers = filteredMembers.OrderBy(o => o.apl_orden);
                    else if (sortIdx == 3) filteredMembers = filteredMembers.OrderBy(o => o.apl_controller);
                    else if (sortIdx == 4) filteredMembers = filteredMembers.OrderBy(o => o.apl_action);
                }
                else
                {
                    if (sortIdx == 0) filteredMembers = filteredMembers.OrderByDescending(o => o.apl_id);
                    else if (sortIdx == 1) filteredMembers = filteredMembers.OrderByDescending(o => o.apl_nombre);
                    else if (sortIdx == 2) filteredMembers = filteredMembers.OrderByDescending(o => o.apl_orden);
                    else if (sortIdx == 3) filteredMembers = filteredMembers.OrderByDescending(o => o.apl_controller);
                    else if (sortIdx == 4) filteredMembers = filteredMembers.OrderByDescending(o => o.apl_action);
                }
            }

            //Func<Ent_Funcion, DateTime> orderingFunction =
            //    (
            //        m => Convert.ToDateTime(m.FECHA_CREACION)
            //    );
            //var sortDirection = Request["sSortDir_0"];
            //if (sortDirection == "asc")
            //    filteredMembers = filteredMembers.OrderBy(orderingFunction);
            //else
            //    filteredMembers = filteredMembers.OrderByDescending(orderingFunction);
            var displayMembers = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);
            var result = from a in displayMembers
                         select new
                         {
                             a.apl_id,
                             a.apl_nombre,
                             a.apl_orden,
                             a.apl_controller,
                             a.apl_action,
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