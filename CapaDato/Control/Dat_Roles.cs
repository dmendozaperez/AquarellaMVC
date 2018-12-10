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
    public class Dat_Roles
    {
        public Ent_Roles rol { get; set; }
        public Boolean InsertarRoles()
        {
            string sqlquery = "USP_Insertar_Roles_MVC";
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
                        cmd.Parameters.AddWithValue("@rol_id", 0);
                        cmd.Parameters.AddWithValue("@rol_nombre", rol.rol_nombre);                        
                        cmd.ExecuteNonQuery();
                        valida = true;
                    }
                }
            }
            catch (Exception exc)
            {

                valida = false;
            }
            return valida;
        }
        public List<Ent_Roles> get_lista()
        {
            List<Ent_Roles> list = null;
            string sqlquery = "USP_LEER_ROLES_MVC";
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
                            list = new List<Ent_Roles>();

                            while (dr.Read())
                            {
                                Ent_Roles rol = new Ent_Roles();
                                rol.rol_id = dr["rol_id"].ToString();
                                rol.rol_nombre = dr["rol_nombre"].ToString();                               
                                list.Add(rol);
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
        public Boolean EditarRoles()
        {
            Boolean valida = false;
            string sqlquery = "USP_Modificar_Roles";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    if (cn.State == 0) cn.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@rol_id", rol.rol_id);
                        cmd.Parameters.AddWithValue("@rol_nombre", rol.rol_nombre);                        
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
    }
    public class Dat_Roles_Funcion
    {        
        public Boolean Eliminar_Fun_Roles(Decimal _fun_id, decimal _rol_id)
        {
            Boolean valida = false;
            string sqlquery = "USP_Borrar_Roles_Funcion";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    if (cn.State == 0) cn.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0; cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@rol_fun_rolid", _rol_id);
                        cmd.Parameters.AddWithValue("@rol_fun_funid", _fun_id);
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
        public Boolean Insertar_Fun_Roles(Decimal _fun_id, decimal _rol_id)
        {
            string sqlquery = "USP_Insertar_Roles_Funcion";
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
                        cmd.Parameters.AddWithValue("@rol_fun_rolid", _rol_id);
                        cmd.Parameters.AddWithValue("@rol_fun_funid", _fun_id);
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
        public List<Ent_Funcion> get_lista(decimal rol_id)
        {
            string sqlquery = "USP_Leer_Funcion_Roles";
            List<Ent_Funcion> list = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    if (cn.State == 0) cn.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@rol_id", rol_id);
                        SqlDataReader dr = cmd.ExecuteReader();
                        list = new List<Ent_Funcion>();
                        if (dr.HasRows)
                        {

                            while (dr.Read())
                            {
                                Ent_Funcion fila = new Ent_Funcion();
                                fila.fun_id = dr["fun_id"].ToString();
                                fila.fun_nombre = dr["fun_nombre"].ToString();
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
