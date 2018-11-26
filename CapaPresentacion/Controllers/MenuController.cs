using CapaDato.Menu;
using CapaEntidad.Control;
using CapaEntidad.Util;
using CapaPresentacion.Models.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CapaPresentacion.Controllers
{
    public class MenuController : Controller
    {
        // GET: Menu
        [Authorize]
        public ActionResult Menu()
        {
            Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
            var data = new Dat_Menu();
            if (data == null) return View(new LoginViewModel());

            if (_usuario == null) return View(new LoginViewModel());

            var items = data.navbarItems(_usuario.usu_id).ToList();

            Session[Ent_Global._session_menu_user] = items;

            return PartialView("_AdminLteLeftMenu", items);
            
        }
    }
}