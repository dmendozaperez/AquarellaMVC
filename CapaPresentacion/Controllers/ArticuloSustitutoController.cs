
using CapaEntidad.ArticuloSustituto;
using CapaEntidad.Control;
using CapaDato.Control;
using CapaEntidad.Menu;
using CapaEntidad.Util;
using CapaPresentacion.Bll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using CapaDato.Menu;
using CapaDato.Articulosustituto;
using CapaDato.Reporte;

namespace CapaPresentacion.Controllers
{
    public class ArticuloSustitutoController : Controller
    {
     
    
        private Dat_ArticuloSustituto datArticuloSusti = new Dat_ArticuloSustituto();
     
        private string _session_liststockArticulo = "_session_listArticuloTienda";

        private bool IsValid(string usuario, string password, ref string _error_con)
        {
            bool _valida = false;
            Dat_Usuario _usuario = new Dat_Usuario();
            Ent_Usuario _data_user = _usuario.get_login(usuario);


            if (_data_user == null) _valida = false;

            if (_data_user != null)
            {
                if (usuario.ToUpper() == _data_user.usu_login.ToUpper() && password == _data_user.usu_contraseña)
                {
                    if (_data_user.usu_est_id == "A")
                    {

                        string strIp = GetIPAddress(System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"],
                                        System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"],
                                        System.Web.HttpContext.Current.Request.UserHostAddress);

                        _data_user.usu_ip = strIp;

                        Session[Ent_Constantes.NameSessionUser] = _data_user;

                        _valida = true;
                    }
                    else
                    {
                        _valida = false;
                    }
                }
                else
                {
                    _valida = false;
                }
            }

            return _valida;
        }

        public static string GetIPAddress(string HttpVia, string HttpXForwardedFor, string RemoteAddr)
        {
            // Use a default address if all else fails.
            string result = "127.0.0.1";

            // Web user - if using proxy
            string tempIP = string.Empty;
            if (HttpVia != null)
                tempIP = HttpXForwardedFor;
            else // Web user - not using proxy or can't get the Client IP
                tempIP = RemoteAddr;

            // If we can't get a V4 IP from the above, try host address list for internal users.
            if (!IsIPV4(tempIP) || tempIP == "127.0.0.1 ")
            {
                try
                {
                    string hostName = System.Net.Dns.GetHostName();
                    foreach (System.Net.IPAddress ip in System.Net.Dns.GetHostAddresses(hostName))
                    {
                        if (IsIPV4(ip))
                        {
                            result = ip.ToString();
                            break;
                        }
                    }
                }
                catch { }
            }
            else
            {
                result = tempIP;
            }

            return result;
        }

        private static bool IsIPV4(string input)
        {
            bool result = false;
            System.Net.IPAddress address = null;

            if (System.Net.IPAddress.TryParse(input, out address))
                result = IsIPV4(address);

            return result;
        }

        private static bool IsIPV4(System.Net.IPAddress address)
        {
            bool result = false;

            switch (address.AddressFamily)
            {
                case System.Net.Sockets.AddressFamily.InterNetwork:   // we have IPv4
                    result = true;
                    break;
                case System.Net.Sockets.AddressFamily.InterNetworkV6: // we have IPv6
                    break;
                default:
                    break;
            }

            return result;
        }

        public ActionResult Index_Acceso()
        {
            string actionName = this.ControllerContext.RouteData.GetRequiredString("action");
            string controllerName = this.ControllerContext.RouteData.GetRequiredString("controller");
            string return_view = actionName + "|" + controllerName;
            Boolean _acceso = false;
            try
            {


                if (this.Request.Params["varemp"].ToString() == "1FCCD4D0-19C6-45AC-AFAC-FC0EF4AF32D5")
                {
                    Response.Cookies["User"].Value = "Invitado";
                    Response.Cookies["Pass"].Value = "Invitado123";
                    _acceso = true;
                    Response.Redirect("../ArticuloStock/Index");
                }
                else
                {
                    _acceso = false;
                }
            }
            catch (Exception)
            {
                _acceso = false;
            }

            if (!_acceso)
            {
                return RedirectToAction("Login", "Control", new { returnUrl = return_view });
            }
            else
            {
                return View();
            }

        }

        public ActionResult Index()
        {
            string usuario_nom = "";
            string usuario_con = "";
            Boolean _acceso = false;
            Boolean _accesoMenu = true;
            Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];

            try
            {
                if (!DBNull.Value.Equals(Request.Cookies["User"].Value) && _usuario == null)
                {

                    usuario_nom = Request.Cookies["User"].Value;
                    usuario_con = Request.Cookies["Pass"].Value;
                    string _error_con = "";
                    _acceso = IsValid(usuario_nom, usuario_con, ref _error_con);
                    if(_acceso)
                        _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];

                    _accesoMenu = false;
                }

            }
            catch (Exception ex) {
                _acceso = false;
            }
           
            string actionName = this.ControllerContext.RouteData.GetRequiredString("action");
            string controllerName = this.ControllerContext.RouteData.GetRequiredString("controller");
            string return_view = actionName + "|" + controllerName;

            if ((_usuario == null && _acceso == false)||(_usuario == null && _accesoMenu == true))
            {
                return RedirectToAction("Login", "Control", new { returnUrl = return_view });
            }
            else
            {
                if (_accesoMenu == true)
                {
                  
                    ViewBag.Usuario = _usuario.usu_nombre;

                    return View();
                }
                else {
                    var data = new Dat_Menu();
                    var items = data.navbarItems(_usuario.usu_id).ToList();
                    Session[Ent_Global._session_menu_user] = items;

                    #region<VALIDACION DE ROLES DE USUARIO>
                    Boolean valida_rol = true;
                    Basico valida_controller = new Basico();
                    List<Ent_Menu_Items> menu = (List<Ent_Menu_Items>)Session[Ent_Global._session_menu_user];
                    valida_rol = valida_controller.AccesoMenu(menu, this);
                    #endregion
                    if (valida_rol)
                    {
                     
                        ViewBag.Usuario = _usuario.usu_nombre;
                    
                        return View();
                    }
                    else
                    {
                        return RedirectToAction("Login", "Control", new { returnUrl = return_view });
                    }
                }
            }
        }

        public ActionResult IndexAdmin()
        {
            string usuario_nom = "";
            string usuario_con = "";
            Boolean _acceso = false;
            Boolean _accesoMenu = true;
            Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
            Dat_Combo datCbo = new Dat_Combo();

            try
            {
                if (!DBNull.Value.Equals(Request.Cookies["User"].Value) && _usuario == null)
                {

                    usuario_nom = Request.Cookies["User"].Value;
                    usuario_con = Request.Cookies["Pass"].Value;
                    string _error_con = "";
                    _acceso = IsValid(usuario_nom, usuario_con, ref _error_con);
                    if (_acceso)
                        _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];

                    _accesoMenu = false;
                }

            }
            catch (Exception ex)
            {
                _acceso = false;
            }

            string actionName = this.ControllerContext.RouteData.GetRequiredString("action");
            string controllerName = this.ControllerContext.RouteData.GetRequiredString("controller");
            string return_view = actionName + "|" + controllerName;

            if ((_usuario == null && _acceso == false) || (_usuario == null && _accesoMenu == true))
            {
                return RedirectToAction("Login", "Control", new { returnUrl = return_view });
            }
            else
            {
                if (_accesoMenu == true)
                {

                    ViewBag.Usuario = _usuario.usu_nombre;
                    ViewBag.Tienda = datCbo.get_ListaTiendaXstore();

                    return View();
                }
                else
                {
                    var data = new Dat_Menu();
                    var items = data.navbarItems(_usuario.usu_id).ToList();
                    Session[Ent_Global._session_menu_user] = items;

                    #region<VALIDACION DE ROLES DE USUARIO>
                    Boolean valida_rol = true;
                    Basico valida_controller = new Basico();
                    List<Ent_Menu_Items> menu = (List<Ent_Menu_Items>)Session[Ent_Global._session_menu_user];
                    valida_rol = valida_controller.AccesoMenu(menu, this);
                    #endregion
                    if (valida_rol)
                    {

                        ViewBag.Usuario = _usuario.usu_nombre;

                        return View();
                    }
                    else
                    {
                        return RedirectToAction("Login", "Control", new { returnUrl = return_view });
                    }
                }
            }
        }

        public JsonResult UpdateStats()
        {
            Dat_Usuario _usuario = new Dat_Usuario();
            Ent_Usuario _data_user = _usuario.get_login("Invitado");
            Session[Ent_Constantes.NameSessionUser] = _data_user;

            return Json("Success", JsonRequestBehavior.AllowGet);
        }
      

        public string listarStr_ArticuloSustituto(string codTda, string codArticulo, string codTalla)
        {
            string strJson = "";
            if (codTda == null)
                codTda = "50339";

            string gcodTda = (String)Session["Tienda"];
            if (gcodTda != "" && gcodTda != null)
            {
                codTda = gcodTda;
            }


            JsonResult jRespuesta = null;
            strJson = datArticuloSusti.listarStr_ArticuloSustituto(codTda, codArticulo, codTalla);
            var serializer = new JavaScriptSerializer();
            jRespuesta = Json(serializer.Deserialize<List<Articulo_Sustituto_Tienda>>(strJson), JsonRequestBehavior.AllowGet);

            return strJson;
        }


    }
}