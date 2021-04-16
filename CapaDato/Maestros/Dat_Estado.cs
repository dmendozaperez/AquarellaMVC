using CapaEntidad.Maestros;
using CapaEntidad.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDato.Maestros
{
    public class Dat_Estado
    {
        public List<Ent_Estado> get_lista()
        {
            List<Ent_Estado> list = null;
            string sqlquery = "USP_Leer_Estado";
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
                            list = new List<Ent_Estado>();
                            //Ent_Estado est = new Ent_Estado();                            
                            //est.est_id = "0";
                            //est.est_descripcion = "(Vacio)";
                            //list.Add(est);
                            
                            while (dr.Read())
                            {
                                Ent_Estado est = new Ent_Estado();
                                est.est_id = dr["Est_Id"].ToString();
                                est.est_descripcion = dr["Est_Descripcion"].ToString();
                                list.Add(est);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                list=null;
            }
            return list;
        }

        public List<Ent_Estado_Modulo> ListarEstadoModulo(Ent_Estado_Modulo _Ent)
        {
            List<Ent_Estado_Modulo> Listar = new List<Ent_Estado_Modulo>();
            string sqlquery = "[USP_Leer_EstadoModulo]";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@est_mod_id", DbType.Int16).Value = _Ent.Est_Mod_Id;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            Listar = new List<Ent_Estado_Modulo>();
                            Listar = (from DataRow fila in dt.Rows
                                      select new Ent_Estado_Modulo()
                                      {
                                          Codigo = (string)(fila["Est_Id"]),
                                          Descripcion = (string)(fila["Est_Descripcion"]),
                                      }
                                    ).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return Listar;
        }
    }
}
