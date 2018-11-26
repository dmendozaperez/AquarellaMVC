using CapaEntidad.Menu;
using CapaEntidad.Util;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
namespace CapaDato.Menu
{
    public class Dat_Menu
    {
        private List<Ent_Menu> Menu_Acceso(decimal _usu_id)
        {
            string sqlquery = "USP_Leer_Funcion_Arbol_mvc";

            List<Ent_Menu> menu_list=null;
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    if (cn.State == 0) cn.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@usu_id", _usu_id);

                        SqlDataReader dr = cmd.ExecuteReader();

                        if (dr.HasRows)
                        {
                            menu_list = new List<Ent_Menu>();

                            while(dr.Read())
                            {
                                Ent_Menu items = new Ent_Menu()
                                {
                                    fun_id =Convert.ToInt32(dr["fun_id"]),
                                    fun_nombre = dr["fun_nombre"].ToString(),
                                    fun_padre = Convert.ToInt32(dr["fun_padre"]),
                                    fun_orden = Convert.ToInt32(dr["fun_orden"]),
                                    apl_id = Convert.ToInt32(dr["apl_id"]),
                                    apl_controller = dr["apl_controller"].ToString(),
                                    apl_action = dr["apl_action"].ToString(),
                                };
                                menu_list.Add(items);
                            }
                            
                        }

                    }
                }

            }
            catch (Exception exc)
            {
                menu_list = null;                
            }
            return menu_list;
        }

        public IEnumerable<Ent_Menu_Items> navbarItems(decimal _usu_id)
        {
            var menu = new List<Ent_Menu_Items>();
            try
            {
                List<Ent_Menu> menu_acceso = Menu_Acceso(_usu_id);

                if (menu_acceso != null)
                {                  
                    //recorre el padre de menu
                    foreach (Ent_Menu app_padre in menu_acceso.Where(menupadre => menupadre.fun_id == menupadre.fun_padre))
                    {
                        var menu_padre = menu_acceso.Where(menuv => menuv.fun_padre == app_padre.fun_id);
                        if (menu_padre.Count() != 1)
                        {
                            menu.Add(new Ent_Menu_Items { Id = app_padre.fun_id, nameOption = app_padre.fun_nombre, controller = "Home", action = "Index", imageClass = "fa fa-fw fa-dashboard", estatus = true, isParent = true, parentId = app_padre.fun_padre, activeli = "submenu" });
                        }
                       

                        foreach (Ent_Menu app_sub_padre in menu_acceso.Where(submenupadre => submenupadre.fun_padre == app_padre.fun_id && submenupadre.fun_padre != submenupadre.fun_id))
                        {
                            if (app_sub_padre.fun_id == 0)
                            {
                                menu.Add(new Ent_Menu_Items { Id = app_sub_padre.fun_id, nameOption = app_sub_padre.fun_nombre, controller = app_sub_padre.apl_controller, action = app_sub_padre.apl_action, imageClass = "fa fa-fw fa-dashboard", estatus = true, isParent = false, parentId = app_sub_padre.fun_padre });
                            }
                            else
                            {
                                menu.Add(new Ent_Menu_Items { Id = app_sub_padre.fun_id, nameOption = app_sub_padre.fun_nombre, controller = "Home", action = "Index", imageClass = "fa fa-fw fa-dashboard", estatus = true, isParent = true, parentId = app_sub_padre.fun_padre, activeli = "submenu" });

                                /*sumenu nivel 2 del menu*/
                                foreach (Ent_Menu app_sub_menu in menu_acceso.Where(submenu => submenu.fun_padre == app_sub_padre.fun_id))
                                {
                                    menu.Add(new Ent_Menu_Items { Id = app_sub_menu.fun_id, nameOption = app_sub_menu.fun_nombre, controller = app_sub_menu.apl_controller, action = app_sub_menu.apl_action, imageClass = "fa fa-fw fa-dashboard", estatus = true, isParent = false, parentId = app_sub_menu.fun_padre });
                                }

                            }
                          
                        }
                        

                    }

                }

            }
            catch (Exception)
            {

                
            }
            return menu;
        }
    }
}
