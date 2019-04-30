using CapaDato.Financiera;
using CapaEntidad.Control;
using CapaEntidad.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CapaPresentacion.Controllers
{
    public class ClearController : Controller
    {
        private Dat_Clear datclear = new Dat_Clear();
        
        private string _session_listPromotor_private = "_session_listPromotor_private";

        // GET: Clear
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        //public ActionResult ListarPedido()
        //{
        //    //Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
        //    string actionName = this.ControllerContext.RouteData.GetRequiredString("action");
        //    string controllerName = this.ControllerContext.RouteData.GetRequiredString("controller");
        //    string return_view = actionName + "|" + controllerName;

        //    if (_usuario == null)
        //    {
        //        return RedirectToAction("Login", "Control", new { returnUrl = return_view });
        //    }

        //   /// Ent_Pedido_Maestro maestros = datPedido.Listar_Maestros_Pedido(_usuario.usu_id, _usuario.usu_postPago, "");
        //    return View();
        //}

        public ActionResult ListaCrucePagosLiq()
        {
            Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
            string actionName = this.ControllerContext.RouteData.GetRequiredString("action");
            string controllerName = this.ControllerContext.RouteData.GetRequiredString("controller");
            string return_view = actionName + "|" + controllerName;

            if (_usuario == null)
            {
                return RedirectToAction("Login", "Control", new { returnUrl = return_view });
            }

          //  Ent_Promotor_Maestros maestros = datUtil.ListarEnt_Maestros_Promotor(_usuario.usu_id);
          //  ViewBag.listLider = maestros.combo_ListLider;

            return View();
        }


    }
}