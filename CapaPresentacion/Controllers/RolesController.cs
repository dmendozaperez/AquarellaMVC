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
    public class RolesController : Controller
    {
        // GET: Roles
        private string _session_listroles_private = "session_listroles_private";
        private Dat_Roles roles = new Dat_Roles();
        public ActionResult Index()
        {
            
            Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];

            string actionName = this.ControllerContext.RouteData.GetRequiredString("action");
            string controllerName = this.ControllerContext.RouteData.GetRequiredString("controller");
            string return_view = actionName + "|" + controllerName;

            if (_usuario == null)
            {
                return RedirectToAction("Login", "Control");
            }
            else
            {
                #region<VALIDACION DE ROLES DE USUARIO>
                Boolean valida_rol = true;
                Basico  valida_controller = new Basico();
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
        public List<Ent_Roles> lista()
        {
            List<Ent_Roles> listroles = roles.get_lista();
            Session[_session_listroles_private] = listroles;
            return listroles;
        }
        public PartialViewResult ListaRoles()
        {
            return PartialView(lista());
        }
        public ActionResult Nuevo()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Nuevo(string nombre,string descripcion)
        {

            if (nombre == null) return Json(new { estado = "0" });

            Ent_Roles _roles = new Ent_Roles();
            _roles.rol_nombre = nombre;
            _roles.rol_descripcion = descripcion;

            Dat_Roles roles = new Dat_Roles();
            roles.rol = _roles; 
            Boolean _valida_nuevo = roles.InsertarRoles();

            return Json(new { estado = (_valida_nuevo) ? "1" : "-1", desmsg = (_valida_nuevo) ? "Se actualizo satisfactoriamente." : "Hubo un error al actualizar." });
        }
        public ActionResult Edit(int? id)
        {
            List<Ent_Roles> listroles = (List<Ent_Roles>)Session[_session_listroles_private];
            if (id == null || listroles == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ent_Roles filaroles = listroles.Find(x => x.rol_id == id.ToString());

            return View(filaroles);
        }
        [HttpPost]
        public ActionResult Edit(string id, string nombre,string descripcion)
        {

            if (id == null) return Json(new { estado="0"});

            Ent_Roles _roles = new Ent_Roles();

            _roles.rol_id = id;
            _roles.rol_nombre = nombre;
            _roles.rol_descripcion = descripcion;

            Dat_Roles roles = new Dat_Roles();
            roles.rol = _roles;

            Boolean _valida_editar = roles.EditarRoles();

            return Json(new { estado = (_valida_editar) ? "1" : "-1", desmsg = (_valida_editar) ? "Se actualizo satisfactoriamente." : "Hubo un error al actualizar." });
        }
        public ActionResult Funcion(Decimal id)
        {
            List<Ent_Roles> listroles = (List<Ent_Roles>)Session[_session_listroles_private];
            if (listroles == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ent_Roles filaroles = listroles.Find(x => x.rol_id == id.ToString());
            ViewBag.rolid = id.ToString();
            ViewBag.rolnombre = filaroles.rol_nombre.ToString();

            Dat_Funcion funciones = new Dat_Funcion();
            ViewBag.funciones = funciones.get_lista(true);

            return View(lista_rol_fun(id));
        }
        public List<Ent_Funcion> lista_rol_fun(Decimal id)
        {
            Dat_Roles_Funcion lista = new Dat_Roles_Funcion();

            List<Ent_Funcion> list_rol_fun = lista.get_lista(id);

            return list_rol_fun;

        }
        public PartialViewResult ListaFunRol(Decimal rolid)
        {
            ViewBag.rolid = rolid.ToString();
            return PartialView(lista_rol_fun(rolid));
        }

        [HttpPost]
        public ActionResult Borrar_Fun(Decimal fun_id, Decimal rol_id)
        {
            Dat_Roles_Funcion _roles_fun = new Dat_Roles_Funcion();

            Boolean _valida_borrar = _roles_fun.Eliminar_Fun_Roles(fun_id, rol_id);

            return Json(new { estado = (_valida_borrar) ? "1" : "-1", desmsg = (_valida_borrar) ? "Se borro correctamente." : "Hubo un error al borrar." });
        }

        [HttpPost]
        public ActionResult Agregar_Fun(Decimal fun_id, Decimal rol_id)
        {

            Dat_Roles_Funcion _roles_fun = new Dat_Roles_Funcion();

            Boolean _valida_agregar = _roles_fun.Insertar_Fun_Roles(fun_id, rol_id);

            return Json(new { estado = (_valida_agregar) ? "1" : "-1", desmsg = (_valida_agregar) ? "Se agrego correctamente." : "Hubo un error al agregar." });
        }
        public ActionResult getListaRolesAjax(Ent_jQueryDataTableParams param, string actualizar)
        {

            List<Ent_Roles> listroles = new List<Ent_Roles>();

            if (!String.IsNullOrEmpty(actualizar))
            {
                listroles = lista();
                //listAtributos = datOE.get_lista_atributos();
                Session[_session_listroles_private] = listroles;
            }

            /*verificar si esta null*/
            if (Session[_session_listroles_private] == null)
            {
                listroles = new List<Ent_Roles>();
                listroles = lista(); //datOE.get_lista_atributos();
                if (listroles == null)
                {
                    listroles = new List<Ent_Roles>();
                }
                Session[_session_listroles_private] = listroles;
            }

            //Traer registros

            IQueryable<Ent_Roles> membercol = ((List<Ent_Roles>)(Session[_session_listroles_private])).AsQueryable();  //lista().AsQueryable();

            //Manejador de filtros
            int totalCount = membercol.Count();
            IEnumerable<Ent_Roles> filteredMembers = membercol;

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = membercol
                    .Where(m => m.rol_id.ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.rol_nombre.ToString().ToUpper().Contains(param.sSearch.ToUpper()));
            }
            //Manejador de orden
            var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);
            var sortDirection = Request["sSortDir_0"];
            if (param.iSortingCols > 0)
            {
                if (sortDirection == "asc")
                {
                    if (sortIdx == 0) filteredMembers = filteredMembers.OrderBy(o => o.rol_id);
                    else if (sortIdx == 1) filteredMembers = filteredMembers.OrderBy(o => o.rol_nombre);
                    else if (sortIdx == 2) filteredMembers = filteredMembers.OrderBy(o => o.rol_descripcion);
                }
                else
                {
                    if (sortIdx == 0) filteredMembers = filteredMembers.OrderByDescending(o => o.rol_id);
                    else if (sortIdx == 1) filteredMembers = filteredMembers.OrderByDescending(o => o.rol_nombre);
                    else if (sortIdx == 2) filteredMembers = filteredMembers.OrderByDescending(o => o.rol_descripcion);
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
                             a.rol_id,
                             a.rol_nombre, 
                             a.rol_descripcion,                            
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