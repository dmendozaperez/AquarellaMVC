using CapaDato.ValeCompra;
using CapaDato.ReportsValeCompra;
using CapaEntidad.ValeCompra;
using CapaEntidad.ArticuloStock;
using CapaEntidad.Control;
using CapaDato.Control;
using CapaEntidad.Menu;
using CapaEntidad.Util;
using CapaPresentacion.Bll;
using System;
using CapaEntidad.General;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CapaPresentacion.Models.Control;

using System.IO;
using System.IO.Compression;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.Routing;

namespace CapaPresentacion.Controllers
{
    public class LoginIntermedioController : Controller
    {

        public  ActionResult Index()
        {
             string campo = Post("p1");
            string ip = Request.ServerVariables["REMOTE_ADDR"];
            string nombrePc = System.Net.Dns.GetHostEntry(Request.ServerVariables["REMOTE_ADDR"]).HostName;
            //nombrePc = "TIENDA-109-1";
            Dat_Usuario _usuario = new Dat_Usuario();
            Ent_Tienda _data_tda = _usuario.get_loginTienda(nombrePc);
            LoginViewModel view = new LoginViewModel();

            if (_data_tda != null)
            {
                Session["Tienda"] = _data_tda.tda_codigo;
                Ent_Usuario _data_user = _usuario.get_login("Tienda");
                view.Usuario = _data_user.usu_login;
                view.Password = _data_user.usu_contraseña;
            }

            view.returnUrl = "";

            return View(view);
        }

        public ActionResult Login(string variable)
        {
            string strCambiante = DateTime.Now.ToString("M/d/yyyy")+"_";
            string resultVariable = DesEncriptar(variable);
            string nombrePc = resultVariable.Replace(strCambiante, "");
              //nombrePc = "TIENDA-109-1";
            Dat_Usuario _usuario = new Dat_Usuario();
            Ent_Tienda _data_tda = _usuario.get_loginTienda(nombrePc);
            LoginViewModel view = new LoginViewModel();

            if (_data_tda != null)
            {
                Session["Tienda"] = _data_tda.tda_codigo;
                Ent_Usuario _data_user = _usuario.get_login("Tienda");
                view.Usuario = _data_user.usu_login;
                view.Password = _data_user.usu_contraseña;
            }

            view.returnUrl = "";

            return View(view);
        }

        public static string Post(string campo)
        {

            bool existeParametro = System.Web.HttpContext.Current.Request.Form[campo] != null;
            string parametro = existeParametro ? System.Web.HttpContext.Current.Request.Form[campo].ToString().Trim() : string.Empty;
            return parametro;
        }

        public static string DesEncriptar(string _cadenaAdesencriptar)
        {
            string result = string.Empty;
            byte[] decryted = Convert.FromBase64String(_cadenaAdesencriptar);
            result = System.Text.Encoding.Unicode.GetString(decryted);
            return result;
        }


    }

}