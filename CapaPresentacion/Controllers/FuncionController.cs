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
    public class FuncionController : Controller
    {
        // GET: Funcion
        private Dat_Funcion funcion = new Dat_Funcion();
        private string _session_listfuncion_private = "session_listfun_private";
        [Authorize]
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
        /*agreanfo controler*/
        public ActionResult Aplicacion(Decimal id)
        {
            List<Ent_Funcion> listfuncion = (List<Ent_Funcion>)Session[_session_listfuncion_private];
            if (listfuncion == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ent_Funcion filafuncion = listfuncion.Find(x => x.fun_id == id.ToString());
            ViewBag.funid = id.ToString();
            ViewBag.funnombre = filafuncion.fun_nombre.ToString();

            Dat_Aplicacion aplicacion = new Dat_Aplicacion();
            ViewBag.aplicacion = aplicacion.get_lista();

            return View(lista_fun_apl(id));
        }

        [HttpPost]
        public ActionResult Borrar_Apl(Decimal apl_id, Decimal fun_id)
        {
            Dat_Funcion_Aplicacion _funcion_apl = new Dat_Funcion_Aplicacion();

            Boolean _valida_borrar = _funcion_apl.Eliminar_App_Funcion(apl_id, fun_id);

            return Json(new { estado = (_valida_borrar) ? "1" : "-1", desmsg = (_valida_borrar) ? "Se borro correctamente." : "Hubo un error al borrar." });
        }

        [HttpPost]
        public ActionResult Agregar_Apl(Decimal apl_id, Decimal fun_id)
        {

            Dat_Funcion_Aplicacion _funcion_apl = new Dat_Funcion_Aplicacion();

            Boolean _valida_agregar = _funcion_apl.Insertar_App_Funcion(apl_id, fun_id);

            return Json(new { estado = (_valida_agregar) ? "1" : "-1", desmsg = (_valida_agregar) ? "Se agrego correctamente." : "Hubo un error al agregar." });
        }

        public ActionResult Edit(int? id)
        {

            List<Ent_Funcion> listfuncion = (List<Ent_Funcion>)Session[_session_listfuncion_private];
            if (id == null || listfuncion == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ent_Funcion filafuncion = listfuncion.Find(x => x.fun_id == id.ToString());

            Dat_Funcion funcion = new Dat_Funcion();
            ViewBag.funcion = funcion.get_lista();
            return View(filafuncion);
        }
        [HttpPost]
        public ActionResult Edit(string id, string nombre,string orden, string padre)
        {
            if (id == null) return Json(new { estado = 0 });

            Ent_Funcion _funcion = new Ent_Funcion();

            Int32 ord = 0;
            Int32.TryParse(orden, out ord);

            _funcion.fun_id = id;
            _funcion.fun_nombre = nombre;         
            _funcion.fun_orden = ord.ToString();
            _funcion.fun_padre = padre;

            Dat_Funcion funcion = new Dat_Funcion();
            funcion.fun = _funcion;

            Boolean _valida_editar = funcion.EditarFuncion();

            return Json(new { estado = (_valida_editar) ? "1" : "-1", desmsg = (_valida_editar) ? "Se actualizo satisfactoriamente." : "Hubo un error al actualizar." });
        }
        public ActionResult Nuevo()
        {
            Dat_Funcion funcion = new Dat_Funcion();
            ViewBag.funcion = funcion.get_lista();
            return View();
        }
        [HttpPost]
        public ActionResult Nuevo(string nombre, string orden, string padre)
        {
            if (nombre == null) return Json(new { estado = 0 });

            Ent_Funcion _funcion = new Ent_Funcion();

            Int32 ord = 0;
            Int32.TryParse(orden, out ord);

            _funcion.fun_id = "0";
            _funcion.fun_nombre = nombre;          
            _funcion.fun_orden = ord.ToString();
            _funcion.fun_padre = padre;

            Dat_Funcion funcion = new Dat_Funcion();
            funcion.fun = _funcion;

            Boolean _valida_nuevo = funcion.InsertarFuncion();

            return Json(new { estado = (_valida_nuevo) ? "1" : "-1", desmsg = (_valida_nuevo) ? "Se actualizo satisfactoriamente." : "Hubo un error al actualizar." });
        }
        public PartialViewResult ListaAplFun(Decimal funid)
        {
            ViewBag.funid = funid.ToString();
            return PartialView(lista_fun_apl(funid));
        }
        public PartialViewResult ListaFuncion()
        {
            return PartialView(lista());
        }
        public List<Ent_Funcion> lista()
        {
            List<Ent_Funcion> listfuncion = funcion.get_lista(true);
            Session[_session_listfuncion_private] = listfuncion;
            return listfuncion;
        }
        public List<Ent_Aplicacion> lista_fun_apl(Decimal id)
        {
            Dat_Funcion_Aplicacion lista = new Dat_Funcion_Aplicacion();

            List<Ent_Aplicacion> list_fun_apl = lista.get_lista(id);

            return list_fun_apl;

        }
    }
}