using CapaDato.Control;
using CapaDato.Util;
using CapaDato.Persona;
using CapaEntidad.Control;
using CapaEntidad.Persona;
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
        private Dat_Util datUtil = new Dat_Util();
        private Dat_Persona datPersona = new Dat_Persona();
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

        public static DataTable _consultaReniec(string _dni)
        {
            DataTable dt = null;
            try
            {

                ws_clientedniruc.Cons_ClienteSoapClient ws_cliente = new ws_clientedniruc.Cons_ClienteSoapClient();
                dt = ws_cliente.ws_persona_reniec(_dni);

            }
            catch (Exception exc)
            {
                dt = null;
            }
            return dt;
        }

        public JsonResult ConsultaReniec(string nroDocumento)
        {
            string strJson = "";
            JsonResult jRespuesta = null;
            var serializer = new JavaScriptSerializer();

            strJson = datPersona.strBuscarPersona(nroDocumento);

            if (strJson != "[]")
            {
               jRespuesta = Json(serializer.Deserialize<List<Persona>>(strJson), JsonRequestBehavior.AllowGet);
            }
            else {

                DataTable dt = null;
                Persona persona = new Persona();
                ws_clientedniruc.Cons_ClienteSoapClient ws_cliente = new ws_clientedniruc.Cons_ClienteSoapClient();
                dt = ws_cliente.ws_persona_reniec(nroDocumento);
                Int32 EstadoReniec = Convert.ToInt32(dt.Rows[0]["estado"]);
                string state = "";
                switch (EstadoReniec)
                {
                    case 217:
                        state = "2";//error de Capcha
                        break;
                    case 231:
                        state = "0";//todo bien
                        break;
                    case 232:
                        state = "0";//todo bien
                        break;
                    case 222:
                        state = "1";//no se encontre a la persona
                        break;
                    default:
                        state = "3";//Error
                        break;
                }

                string nombres = (dt.Rows[0]["nombres"]).ToString();
                string[] arrNombres = splitString(nombres, ' ');

                if (state == "0") {
                    string strDni = (dt.Rows[0]["dni"]).ToString();
                    string apepat = (dt.Rows[0]["apepat"]).ToString();

                    if (nombres != "" && apepat != "")
                    {
                        persona.Bas_Documento = (dt.Rows[0]["dni"]).ToString();

                        persona.Bas_Primer_Nombre = arrNombres[0].ToString();
                        persona.Bas_Primer_Apellido = (dt.Rows[0]["apepat"]).ToString();
                        persona.Bas_Segundo_Apellido = (dt.Rows[0]["apemat"]).ToString();
                        if (arrNombres.Length > 1)
                            persona.Bas_segundo_nombre = arrNombres[1].ToString();

                        state = "3";
                    }
                    persona.Estado = state;
                    persona.Bas_id = "0";

                    
                }
                List<Persona> list = new List<Persona>();
                list.Add(persona);
                jRespuesta = Json(list, JsonRequestBehavior.AllowGet);

            }
          
            return jRespuesta;
        }

        private static string[] splitString(string _textString, char _character)
        {
            string[] split = null;
            if (!string.IsNullOrEmpty(_textString))
            {
                split = _textString.Split(new Char[] { _character });
            }
            return split;
        }


    }
}