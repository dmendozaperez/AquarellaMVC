using CapaEntidad.Control;
using CapaEntidad.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDato.Control
{
    public class Dat_Usuario
    {
        #region<Region de Acceso al sistema>
        public Ent_Usuario get_login(string _usuario,ref string error_con)
        {
            string sqlquery = "[USP_Leer_Usuario_MVC]";
                Ent_Usuario usuario = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    if (cn.State == 0) cn.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Usu_Nombre", _usuario);

                        SqlDataReader dr = cmd.ExecuteReader();

                        if (dr.HasRows)
                        {
                            usuario = new Ent_Usuario();

                            while(dr.Read())
                            {
                                usuario.usu_id =(decimal)dr["usu_id"];
                                usuario.usu_nombre = dr["usu_nombre"].ToString();
                                usuario.usu_contraseña = dr["usu_contraseña"].ToString();
                                usuario.usu_est_id = dr["usu_est_id"].ToString();
                                usuario.usu_tip_nom = dr["usu_tip_nombre"].ToString();
                                usuario.usu_login = dr["usu_login"].ToString();
                                usuario.usu_postPago = dr["postpago"].ToString();
                                usuario.usu_nom_ape = dr["Nombre"].ToString();
                                usuario.usu_tip_id = dr["Usu_Tip_Id"].ToString();
                                usuario.usu_area= dr["bas_are_id"].ToString();
                                usuario.usu_flete = dr["usu_flete"].ToString();
                            }
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                error_con = "1";
                usuario = null;
               
            }
            return usuario;
        }
        #endregion

        #region<Region de Acceso al sistema por una tienda>
        public Ent_Tienda get_loginTienda(string codTienda)
        {
            string sqlquery = "[USP_Leer_Tienda]";
            Ent_Tienda tienda = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    if (cn.State == 0) cn.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@codTienda", codTienda);

                        SqlDataReader dr = cmd.ExecuteReader();

                        if (dr.HasRows)
                        {
                            tienda = new Ent_Tienda();

                            while (dr.Read())
                            {

                                tienda.tda_codigo = dr["cod_entid"].ToString();

                            }
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                tienda = null;
            }
            return tienda;
        }
        #endregion

        #region<Region de Mantenimiento de usuarios>

        public Ent_Usuario usu { get; set; }
        public Boolean existe_login { get; set; }

        public Boolean InsertarUsuario()
        {
            string sqlquery = "USP_Insertar_Usuario";
            Boolean valida = false;
            existe_login = false;
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    if (cn.State == 0) cn.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@usu_nombre",usu.usu_nombre);
                        cmd.Parameters.AddWithValue("@usu_login", usu.usu_login);
                        cmd.Parameters.AddWithValue("@usu_password",usu.usu_contraseña);
                        cmd.Parameters.AddWithValue("@usu_tip_id",usu.usu_tip_id);

                        cmd.Parameters.Add("@existe", SqlDbType.Bit);

                        cmd.Parameters["@existe"].Direction = ParameterDirection.Output;
                                              
                        cmd.ExecuteNonQuery();

                        existe_login =Convert.ToBoolean(cmd.Parameters["@existe"].Value);

                        valida = true;
                    }
                }

            }
            catch (Exception exc)
            {
                existe_login = false;
                valida = false;
            }
            return valida;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Editar usuario si recibe true es porque se va a mofificar su password"></param>
        /// <returns></returns>
        public Boolean EditarUsuario(Boolean edit_pass=false)
        {
            string sqlquery = "USP_Modificar_Usuario";
            Boolean valida = false;
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    if (cn.State == 0) cn.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@usu_id",usu.usu_id);
                        cmd.Parameters.AddWithValue("@usu_nombre",usu.usu_nombre);
                        cmd.Parameters.AddWithValue("@usu_password",usu.usu_contraseña);
                        cmd.Parameters.AddWithValue("@usu_est_id",usu.usu_est_id);
                        cmd.Parameters.AddWithValue("@usu_tip_id",usu.usu_tip_id);
                        cmd.Parameters.AddWithValue("@pass", edit_pass);
                        cmd.ExecuteNonQuery();
                        valida = true;
                    }
                }
            }
            catch
            {
                valida = false;
            }
            return valida;
        }

        public List<Ent_Usuario> get_lista(Boolean listar = false)
        {
            string sqlquery = "[USP_LEER_USUARIOS_MVC]";
            List<Ent_Usuario> list = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    if (cn.State == 0) cn.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlDataReader dr = cmd.ExecuteReader();

                        if (dr.HasRows)
                        {
                            list = new List<Ent_Usuario>();
                                                       
                            while (dr.Read())
                            {
                                Ent_Usuario _usu = new Ent_Usuario();
                                _usu.usu_id =Convert.ToDecimal(dr["Usu_Id"]);
                                _usu.usu_nombre= dr["Usu_Nombre"].ToString();
                                _usu.usu_login = dr["Usu_Login"].ToString();
                                _usu.usu_est_id = dr["Usu_Est_Id"].ToString();
                               // _usu.usu_tip_nom = dr["Usu_Tip_Nombre"].ToString();
                               // _usu.usu_tip_id= dr["Usu_Tip_Id"].ToString();
                                list.Add(_usu);
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                list = null;
            }
            return list;
        }
        #endregion
    }
    public class Dat_Usuario_Roles
    {
        public Boolean Eliminar_Rol_Usuario(Decimal _usu_rol_idusu, decimal _usu_rol_idrol)
        {
            Boolean valida = false;
            string sqlquery = "USP_Borrar_Usuario_Roles";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    if (cn.State == 0) cn.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0; cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@usu_rol_idusu", _usu_rol_idusu);
                        cmd.Parameters.AddWithValue("@usu_rol_idrol", _usu_rol_idrol);
                        cmd.ExecuteNonQuery();
                        valida = true;
                    }
                }
            }
            catch (Exception)
            {

                valida = false;
            }
            return valida;
        }
        public Boolean Insertar_Rol_Usuario(Decimal _usu_rol_idusu, decimal _usu_rol_idrol)
        {
            string sqlquery = "USP_Insertar_Usuario_Roles";
            Boolean valida = false;
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    if (cn.State == 0) cn.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@usu_rol_idusu", _usu_rol_idusu);
                        cmd.Parameters.AddWithValue("@usu_rol_idrol", _usu_rol_idrol);
                        cmd.ExecuteNonQuery();
                        valida = true;
                    }
                }
            }
            catch (Exception)
            {

                valida = false;
            }
            return valida;
        }
        public List<Ent_Roles> get_lista(decimal usu_id)
        {
            string sqlquery = "USP_Leer_Roles_Usuario";
            List<Ent_Roles> list = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    if (cn.State == 0) cn.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@usu_id", usu_id);
                        SqlDataReader dr = cmd.ExecuteReader();
                        list = new List<Ent_Roles>();
                        if (dr.HasRows)
                        {

                            while (dr.Read())
                            {
                                Ent_Roles fila = new Ent_Roles();
                                fila.rol_id = dr["rol_id"].ToString();
                                fila.rol_nombre = dr["rol_nombre"].ToString();
                                list.Add(fila);
                            }
                        }

                    }
                }
            }
            catch (Exception)
            {

                list = null;
            }
            return list;
        }
    }
}
