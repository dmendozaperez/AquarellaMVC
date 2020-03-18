using CapaDato.Control;
using CapaDato.Util;
using CapaDato.Persona;
using CapaDato.Promotor;
using CapaEntidad.Control;
using CapaEntidad.Persona;
using CapaEntidad.Menu;
using CapaEntidad.Util;
using CapaEntidad.Promotor;
using CapaPresentacion.Bll;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using CapaEntidad.General;

namespace CapaPresentacion.Controllers
{
    public class PromotorController : Controller
    {
        // GET: Funcion
        private Dat_Util datUtil = new Dat_Util();
        private Dat_Persona datPersona = new Dat_Persona();
        private string _session_listPromotor_private = "_session_listPromotor_private";

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

              
                    Ent_Promotor_Maestros maestros = datUtil.ListarEnt_Maestros_Promotor(_usuario.usu_id);
                    
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

        public ActionResult ListaPromotor()
        {
            Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
            string actionName = this.ControllerContext.RouteData.GetRequiredString("action");
            string controllerName = this.ControllerContext.RouteData.GetRequiredString("controller");
            string return_view = actionName + "|" + controllerName;

            if (_usuario == null)
            {
                return RedirectToAction("Login", "Control", new { returnUrl = return_view });
            }

            Ent_Promotor_Maestros maestros = datUtil.ListarEnt_Maestros_Promotor(_usuario.usu_id);
            ViewBag.listLider = maestros.combo_ListLider;

            return View();
        }

        public PartialViewResult ListaMovilPromotor(string idLider)
        {
            List<Ent_Promotor> lista_prom = lista(idLider);
                   
            return PartialView(lista_prom);
        }

        public PartialViewResult ListaPcPromotor(string dwlider)
        {
      
            List<Ent_Promotor> lista_prom = lista(dwlider);

            return PartialView(lista_prom);
        }

        public List<Ent_Promotor> lista(string idLider)
        {
            Dat_Promotor datprot = new Dat_Promotor();

            List<Ent_Promotor> listPromotor = datprot.get_lista(idLider);
            Session[_session_listPromotor_private] = listPromotor;
            return listPromotor;
        }

        public ActionResult getListPromotor(Ent_jQueryDataTableParams param)
        {
            /*verificar si esta null*/
            if (Session[_session_listPromotor_private] == null)
            {
                List<Ent_Promotor> listdoc = new List<Ent_Promotor>();
                Session[_session_listPromotor_private] = listdoc;
            }

            //Traer registros
            IQueryable<Ent_Promotor> membercol = ((List<Ent_Promotor>)(Session[_session_listPromotor_private])).AsQueryable();  //lista().AsQueryable();

            //Manejador de filtros
            int totalCount = membercol.Count();
            IEnumerable<Ent_Promotor> filteredMembers = membercol;

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = membercol
                    .Where(m => m.prmt_NroDoc.ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.prmt_ApePater.ToUpper().Contains(param.sSearch.ToUpper()));
            }
            //Manejador de orden
            var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);
            Func<Ent_Promotor, string> orderingFunction =
            (
            m => sortIdx == 0 ? m.prmt_NroDoc :
             m.prmt_ApePater
            );
            var sortDirection = Request["sSortDir_0"];
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
                             a.prmt_NroDoc,
                             a.prmt_Nombre1,
                             a.prmt_Nombre2,
                             a.prmt_ApePater,
                             a.prmt_ApeMater,
                             a.prmt_Correo,
                            
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
               jRespuesta = Json(serializer.Deserialize<List<Ent_Persona>>(strJson), JsonRequestBehavior.AllowGet);
            }
            else {

                DataTable dt = null;
                Ent_Persona persona = new Ent_Persona();
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
                List<Ent_Persona> list = new List<Ent_Persona>();
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

        public static string encrypt(string cadena)
        {
            // Create a new DES key.
            DESCryptoServiceProvider key = new DESCryptoServiceProvider();
            key.Key = Encoding.UTF8.GetBytes("_MANISOL");
            key.IV = Encoding.UTF8.GetBytes("_BATA_SA");

            // Encrypt a string to a byte array.
            byte[] buffer = encrypt(cadena, key);

            string cad;
            //CONVIERTE EN STRING EL ARREGLO DE BYTES
            cad = Convert.ToBase64String(buffer);

            return cad;
        }

        private static byte[] encrypt(string PlainText, SymmetricAlgorithm key)
        {
            // Create a memory stream.
            MemoryStream ms = new MemoryStream();

            // Create a CryptoStream using the memory stream and the 
            // CSP DES key.  
            CryptoStream encStream = new CryptoStream(ms, key.CreateEncryptor(), CryptoStreamMode.Write);

            // Create a StreamWriter to write a string
            // to the stream.
            StreamWriter sw = new StreamWriter(encStream);

            // Write the plaintext to the stream.
            sw.WriteLine(PlainText);

            // Close the StreamWriter and CryptoStream.
            sw.Close();
            encStream.Close();

            // Get an array of bytes that represents
            // the memory stream.
            byte[] buffer = ms.ToArray();

            // Close the memory stream.
            ms.Close();

            // Return the encrypted byte array.
            return buffer;
        }

        public JsonResult GuardarPromotor(Ent_Promotor _promotor)
        {
            var oJRespuesta = new JsonResponse();
            Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
            Dat_Promotor datprot = new Dat_Promotor();
            _promotor.usuId = _usuario.usu_id.ToString();
            _promotor.prmt_contrasenia = encrypt(_promotor.prmt_NroDoc);
            _promotor.prmt_UsuTipo = "02";

            Boolean bPromotor = datprot.InsertarPromotor(_promotor);

            oJRespuesta.Data = bPromotor;
            oJRespuesta.Message = bPromotor.ToString();

            return Json(oJRespuesta, JsonRequestBehavior.AllowGet);
        }


   }
}