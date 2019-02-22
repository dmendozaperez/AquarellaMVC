using CapaDato.Control;
using CapaDato.Menu;
using CapaEntidad.Control;
using CapaEntidad.Util;
using CapaPresentacion.Models.Control;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace CapaPresentacion.Controllers
{
    [Authorize]
    public class ControlController : Controller
    {
        #region<Validacion de acceso al sistema>
        IAuthenticationManager Authentication
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }
        // GET: Control
        [AllowAnonymous]
        public ActionResult Login(string returnUrl = null)
        {      

            //if (returnUrl == "Index|ArticuloStock") {
            //    string _error_con = "";
            //    Boolean _acceso = IsValid("Invitado", "Invitado123", ref _error_con);
            //    Ent_Usuario _usuario2 = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
            //    string return_action = ""; string return_controller = "";

            //    string[] controller_action = returnUrl.Split('|');
            //    return_action = controller_action[0].ToString();
            //    return_controller = controller_action[1].ToString();

            //    var data = new Dat_Menu();
            //    var items = data.navbarItems(_usuario2.usu_id).ToList();
            //    Session[Ent_Global._session_menu_user] = items;
            //    return RedirectToAction(return_action, return_controller);

            //}
           
            Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
            if (_usuario == null)
            {
                Authentication.SignOut();
                Session.Clear();
            }

            //ViewBag.returnUrl = returnUrl;

            LoginViewModel view = new LoginViewModel();
            view.returnUrl = returnUrl;
            //return View(new LoginViewModel());
            return View(view);
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            string _error_con = "";
            Boolean _acceso = IsValid(model.Usuario, model.Password, ref _error_con);

            string return_action = ""; string return_controller = "";

            if (_acceso)
            {
                if (returnUrl != null)
                {
                    if (returnUrl.Length > 0)
                    {
                        string[] controller_action = returnUrl.Split('|');
                        return_action = controller_action[0].ToString();
                        return_controller = controller_action[1].ToString();
                    }
                }


                Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
                var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, _usuario.usu_nombre), }, DefaultAuthenticationTypes.ApplicationCookie);
                Authentication.SignIn(new AuthenticationProperties
                {
                    IsPersistent = model.Recordar
                }, identity);


                if (return_action.Length == 0)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    /*validamos las opciones del menu de acceso*/
                    var data = new Dat_Menu();
                    var items = data.navbarItems(_usuario.usu_id).ToList();
                    Session[Ent_Global._session_menu_user] = items;
                    return RedirectToAction(return_action, return_controller);
                    /*************************************/
                }

            }
            else
            {
                if (_error_con == "1")
                {
                    ModelState.AddModelError("", "El intento de conexión no fue correcto. Inténtelo de nuevo");
                    return View(model);
                }
                else
                {
                    ModelState.AddModelError("", "Usuario y/o contraseña son incorrectos.");
                    return View(model);
                }
            }
        }
        private bool IsValid(string usuario, string password, ref string _error_con)
        {
            bool _valida = false;
            Dat_Usuario _usuario = new Dat_Usuario();
            Ent_Usuario _data_user= _usuario.get_login(usuario);


            if (_data_user == null) _valida = false;             

            if (_data_user != null)
            {
                if (usuario.ToUpper() == _data_user.usu_login.ToUpper() && password ==_data_user.usu_contraseña)
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

        public ActionResult LogOff()
        {
            Authentication.SignOut();
            Session.Clear();
            return RedirectToAction("Login", "Control");
        }
        #endregion

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

        /// <summary>
        /// Determines weather an IP Address is V4
        /// </summary>
        /// <param name="input">input string</param>
        /// <returns>Is IPV4 True or False</returns>
        private static bool IsIPV4(string input)
        {
            bool result = false;
            System.Net.IPAddress address = null;

            if (System.Net.IPAddress.TryParse(input, out address))
                result = IsIPV4(address);

            return result;
        }

        /// <summary>
        /// Determines weather an IP Address is V4
        /// </summary>
        /// <param name="address">input IP address</param>
        /// <returns>Is IPV4 True or False</returns>
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

    }
}