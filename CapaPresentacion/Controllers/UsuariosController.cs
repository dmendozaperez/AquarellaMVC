using CapaDato.Control;
using CapaDato.Maestros;
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
    public class UsuariosController : Controller
    {
        // GET: Usuarios
        private Dat_Usuario usuario = new Dat_Usuario();
        private string _session_listusu_private = "session_listusu_private";
        //[Authorize]
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

        public PartialViewResult ListaUsuario()
        {
            return PartialView(lista());
        }

        public List<Ent_Usuario> lista()
        {
            List<Ent_Usuario> listusuario = usuario.get_lista();
            Session[_session_listusu_private] = listusuario;
            return listusuario;
        }

        public ActionResult Nuevo()
        {
            //Dat_Estado estado = new Dat_Estado();
            //ViewBag.estado = estado.get_lista();

            Dat_Usuario_Tipo usuariotipo = new Dat_Usuario_Tipo();
            ViewBag.usuariotipo = usuariotipo.get_lista();
            return View();
        }
        [HttpPost]
        public ActionResult Nuevo(string nombre, string login, string password, string tipo)
        {
            if (nombre == null) return Json(new { estado = "0" });

            if (tipo=="0" || tipo==null) return Json(new { estado = "-1", desmsg="seleccione el tipo de usuario" });

            Ent_Usuario _usuario = new Ent_Usuario();

            _usuario.usu_nombre = nombre;
            _usuario.usu_login = login;
            _usuario.usu_contraseña = password;
            _usuario.usu_tip_id = tipo;

            Dat_Usuario usuario = new Dat_Usuario();
            usuario.usu = _usuario;



            Boolean _valida_nuevo =usuario.InsertarUsuario();

            if (usuario.existe_login)
            {
                _valida_nuevo = false;
                return Json(new { estado ="-1", desmsg = "El login del usuario ya existe." });
            }
            else
            {
                return Json(new { estado = (_valida_nuevo) ? "1" : "-1", desmsg = (_valida_nuevo) ? "Se actualizo satisfactoriamente." : "Hubo un error al actualizar." });
            }
            
        }

        public ActionResult Edit(int? id)
        {

            List<Ent_Usuario> listusuario = (List<Ent_Usuario>)Session[_session_listusu_private];
            if (id == null || listusuario == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ent_Usuario filafuncion = listusuario.Find(x => x.usu_id == id);

            Dat_Estado estado = new Dat_Estado();
            ViewBag.estado = estado.get_lista();

            Dat_Usuario_Tipo usuariotipo = new Dat_Usuario_Tipo();
            ViewBag.usuariotipo = usuariotipo.get_lista();


            return View(filafuncion);
        }

        [HttpPost]
        public ActionResult Edit(string id, string nombre, string password, string tipo,string estado,string pass)
        {
            if (id == null) return Json(new { estado = 0 });

            Ent_Usuario _usuario = new Ent_Usuario();

            _usuario.usu_id =Convert.ToDecimal(id);
            _usuario.usu_nombre = nombre;
            _usuario.usu_contraseña = password;
            _usuario.usu_tip_id = tipo;
            _usuario.usu_est_id = estado;
                        

            Dat_Usuario usuario = new Dat_Usuario();
            usuario.usu = _usuario;

            Boolean _valida_editar =usuario.EditarUsuario((pass=="1")?true:false);

            return Json(new { estado = (_valida_editar) ? "1" : "-1", desmsg = (_valida_editar) ? "Se actualizo satisfactoriamente." : "Hubo un error al actualizar." });
        }

        public ActionResult Roles(Decimal id)
        {
            List<Ent_Usuario> listusuarios = (List<Ent_Usuario>)Session[_session_listusu_private];
            if (listusuarios == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ent_Usuario filausuario = listusuarios.Find(x => x.usu_id == id);
            ViewBag.usuid = id.ToString();
            ViewBag.usunombre = filausuario.usu_nombre.ToString();

            Dat_Roles roles = new Dat_Roles();
            ViewBag.roles = roles.get_lista();

            return View(lista_usu_rol(id));

        }
        public PartialViewResult ListaRolUsu(Decimal usuid)
        {
            ViewBag.usuid = usuid.ToString();
            return PartialView(lista_usu_rol(usuid));
        }
        public List<Ent_Roles> lista_usu_rol(Decimal id)
        {
            Dat_Usuario_Roles lista = new Dat_Usuario_Roles();

            List<Ent_Roles> list_usu_rol = lista.get_lista(id);

            return list_usu_rol;

        }
        [HttpPost]
        public ActionResult Borrar_Rol(Decimal usu_id, Decimal rol_id)
        {
            Dat_Usuario_Roles _usuario_rol = new Dat_Usuario_Roles();

            Boolean _valida_borrar = _usuario_rol.Eliminar_Rol_Usuario(usu_id, rol_id);

            return Json(new { estado = (_valida_borrar) ? "1" : "-1", desmsg = (_valida_borrar) ? "Se borro correctamente." : "Hubo un error al borrar." });
        }
        [HttpPost]
        public ActionResult Agregar_Rol(Decimal usu_id, Decimal rol_id)
        {

            Dat_Usuario_Roles _usuario_rol = new Dat_Usuario_Roles();

            Boolean _valida_agregar = _usuario_rol.Insertar_Rol_Usuario(usu_id, rol_id);

            return Json(new { estado = (_valida_agregar) ? "1" : "-1", desmsg = (_valida_agregar) ? "Se agrego correctamente." : "Hubo un error al agregar." });
        }
    }
}