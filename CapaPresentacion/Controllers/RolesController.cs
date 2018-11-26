using CapaDato.Control;
using CapaEntidad.Control;
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
        public ActionResult Nuevo(string nombre)
        {

            if (nombre == null) return Json(new { estado = "0" });

            Ent_Roles _roles = new Ent_Roles();
            _roles.rol_nombre = nombre;

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
        public ActionResult Edit(string id, string nombre)
        {

            if (id == null) return Json(new { estado="0"});

            Ent_Roles _roles = new Ent_Roles();

            _roles.rol_id = id;
            _roles.rol_nombre = nombre;

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
    }
}