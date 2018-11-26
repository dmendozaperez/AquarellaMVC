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
    public class Dat_Funcion
    {
        public Ent_Funcion fun { get; set; }
        public Boolean InsertarFuncion()
        {
            string sqlquery = "USP_Insertar_Funcion_MVC";
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
                        cmd.Parameters.AddWithValue("@fun_id", fun.fun_id);
                        cmd.Parameters.AddWithValue("@fun_nombre", fun.fun_nombre);                       
                        cmd.Parameters.AddWithValue("@fun_orden", fun.fun_orden);
                        cmd.Parameters.AddWithValue("@fun_padre", fun.fun_padre);                        
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

        public Boolean EditarFuncion()
        {
            string sqlquery = "USP_Modificar_Funcion_MVC";
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
                        cmd.Parameters.AddWithValue("@fun_id", fun.fun_id);
                        cmd.Parameters.AddWithValue("@fun_nombre", fun.fun_nombre);                        
                        cmd.Parameters.AddWithValue("@fun_orden", fun.fun_orden);
                        cmd.Parameters.AddWithValue("@fun_padre", fun.fun_padre);
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

        public List<Ent_Funcion> get_lista(Boolean listar = false)
        {
            string sqlquery = "[USP_Leer_Funcion_Sistema_MVC]";
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
                        SqlDataReader dr = cmd.ExecuteReader();

                        if (dr.HasRows)
                        {
                            list = new List<Ent_Funcion>();
                            Ent_Funcion fun = new Ent_Funcion();
                            if (!listar)
                            {
                                fun.fun_id = "0";
                                fun.fun_nombre = "(Vacio)";
                                list.Add(fun);
                            }

                            while (dr.Read())
                            {
                                fun = new Ent_Funcion();
                                fun.fun_id = dr["Fun_Id"].ToString();
                                fun.fun_nombre = dr["Fun_Nombre"].ToString();                                
                                fun.fun_orden = dr["Fun_Orden"].ToString();
                                fun.fun_padre = dr["Fun_Padre"].ToString();
                                list.Add(fun);
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
    }
    public class Dat_Funcion_Aplicacion
    {
        public Boolean Eliminar_App_Funcion(Decimal _apl_id, decimal _fun_id)
        {
            Boolean valida = false;
            string sqlquery = "USP_Borrar_Apl_Fun";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    if (cn.State == 0) cn.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0; cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@apl_fun_aplid", _apl_id);
                        cmd.Parameters.AddWithValue("@apl_fun_funid", _fun_id);
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

        public Boolean Insertar_App_Funcion(Decimal _apl_id, decimal _fun_id)
        {
            string sqlquery = "USP_Insertar_Apl_Fun";
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
                        cmd.Parameters.AddWithValue("@apl_fun_aplid", _apl_id);
                        cmd.Parameters.AddWithValue("@apl_fun_funid", _fun_id);
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

        public List<Ent_Aplicacion> get_lista(decimal fun_id)
        {
            string sqlquery = "USP_Leer_Apl_Fun";
            List<Ent_Aplicacion> list = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    if (cn.State == 0) cn.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Fun_Id", fun_id);
                        SqlDataReader dr = cmd.ExecuteReader();
                        list = new List<Ent_Aplicacion>();
                        if (dr.HasRows)
                        {

                            while (dr.Read())
                            {
                                Ent_Aplicacion fila = new Ent_Aplicacion();
                                fila.apl_id = dr["apl_id"].ToString();
                                fila.apl_nombre = dr["apl_nombre"].ToString();
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
