using CapaDato.Control;
using CapaDato.Util;
using CapaEntidad.Control;
using CapaEntidad.Menu;
using CapaEntidad.Util;
using CapaPresentacion.Bll;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace CapaPresentacion.Controllers
{
    public class PromotorController : Controller
    {
        // GET: Funcion
        private Dat_Funcion funcion = new Dat_Funcion();

        private Dat_Util datUtil = new Dat_Util();
        private string _session_listfuncion_private = "session_listfun_private";
        [Authorize]
        public ActionResult Nuevo()
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

              
                    Ent_Promotor_Maestros maestros = datUtil.ListarEnt_Maestros_Promotor("");
                    
                    List<Ent_Combo> listobj = new List<Ent_Combo>();
                    Ent_Combo cbo = new Ent_Combo();
                    cbo.codigo = "-1";
                    cbo.descripcion = "------Selecccione------";
                    listobj.Add(cbo);

                    ViewBag.listDepartamento = maestros.combo_ListDepartamento;
                    ViewBag.listLider = maestros.combo_ListLider;
                    ViewBag.listTipoDoc = maestros.combo_ListTipoDoc;
                    ViewBag.listTipoPersona = maestros.combo_ListTipoPersona;
                    ViewBag.listTipoUsuario = maestros.combo_ListTipoUsuario;





                    ViewBag.General = listobj;


                    return View();
                }
                else
                {
                    return RedirectToAction("Login", "Control", new { returnUrl = return_view });
                }
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
                    strJson = datUtil.listarStr_Provincia(Params);
                    jRespuesta = Json(serializer.Deserialize<List<Ent_Combo>>(strJson), JsonRequestBehavior.AllowGet);
                    break;
                case 2:
                    String[] substrings = Params.Split('|');
                    strJson = datUtil.listarStr_Distrito(Params);
                    jRespuesta = Json(serializer.Deserialize<List<Ent_Combo>>(strJson), JsonRequestBehavior.AllowGet);
                    break;
                default:
                    Console.WriteLine("Default case");
                    break;
            }
            return jRespuesta;
        }

    }
}