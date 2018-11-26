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
    }
}