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
    public class Dat_Aplicacion
    {
        public Ent_Aplicacion apl { get; set; }

        public bool valida()
        {
            string v = "ddddddd";
            return false;
        }
        public Boolean UpdateAplicacion()
        {
            Boolean valida = false;
            string sqlquery = "[USP_Modificar_Aplicacion_MVC]";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    if (cn.State == 0) cn.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@apl_id", apl.apl_id);
                        cmd.Parameters.AddWithValue("@apl_nombre", apl.apl_nombre);                      
                        cmd.Parameters.AddWithValue("@apl_orden", apl.apl_orden);                       
                        cmd.Parameters.AddWithValue("@apl_controller", apl.apl_controller);
                        cmd.Parameters.AddWithValue("@apl_action", apl.apl_action);
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
        public Boolean InsertarAplicacion()
        {
            string sqlquery = "USP_Insertar_Aplicacion_MVC";
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
                        cmd.Parameters.AddWithValue("@apl_id", apl.apl_id);
                        cmd.Parameters.AddWithValue("@apl_nombre", apl.apl_nombre);                     
                        cmd.Parameters.AddWithValue("@apl_orden", apl.apl_orden);                        
                        cmd.Parameters.AddWithValue("@apl_controller", apl.apl_controller);
                        cmd.Parameters.AddWithValue("@apl_action", apl.apl_action);
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
        public List<Ent_Aplicacion> get_lista()
        {
            string sqlquery = "USP_Leer_Aplicacion_MVC";
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
                        SqlDataReader dr = cmd.ExecuteReader();

                        if (dr.HasRows)
                        {
                            list = new List<Ent_Aplicacion>();

                            while (dr.Read())
                            {
                                Ent_Aplicacion apl = new Ent_Aplicacion();
                                apl.apl_id =dr["apl_id"].ToString();
                                apl.apl_nombre = dr["apl_nombre"].ToString();
                                apl.apl_orden = dr["apl_orden"].ToString();
                                apl.apl_action = dr["apl_action"].ToString();
                                apl.apl_controller = dr["apl_controller"].ToString();
                                list.Add(apl);
                            }
                        }

                    }
                }
            }
            catch
            {
                list = null;
            }
            return list;
        }
    }
}
