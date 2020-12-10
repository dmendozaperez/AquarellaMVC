using CapaEntidad.Financiera;
using CapaEntidad.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace CapaDato.Financiera
{
    public class Dat_Estado
    {
        public List<Ent_Estado> Listar_Estados(Ent_Estado ent)
        {
            List<Ent_Estado> Listar = new List<Ent_Estado>();
            string sqlquery = "[USP_Leer_EstadoModulo]";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@est_mod_id", DbType.String).Value = ent.Est_Mod_Id;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable(); 
                            da.Fill(dt);

                            Listar = new List<Ent_Estado>();
                            Listar = (from DataRow dr in dt.Rows
                                      select new Ent_Estado()
                                      {
                                          Codigo = Convert.ToString(dr["Est_Id"]),
                                          Descripcion = Convert.ToString(dr["Est_Descripcion"])
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
