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

using System.IO;
using System.IO.Compression;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.Routing;

namespace CapaPresentacion.Controllers
{
    public class IntermedioController : Controller
    {     
    
        public ActionResult Index()
        {
            Response.Cookies["User"].Value = "Invitado";
            Response.Cookies["Pass"].Value = "Invitado123";
            Response.Redirect("../ArticuloStock/Index");

            return View();
        }

        public JsonResult UpdateStats(string usuario,string contraseña)
        {
            Dat_Usuario _usuario = new Dat_Usuario();
            Ent_Usuario _data_user = _usuario.get_login("Invitado");
            Session[Ent_Constantes.NameSessionUser] = _data_user;

            return Json("Success", JsonRequestBehavior.AllowGet);
        }

    }
   
}