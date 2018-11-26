using CapaEntidad.Reports;
using CapaEntidad.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDato.Reports
{
    public class Dat_Bataclub
    {
        public List<Ent_Bataclub> get_lista(DateTime fechaini,DateTime fechafin)
        {
            string sqlquery = "USP_GetVentaBataclub";
            List<Ent_Bataclub> listar = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@fechaini", fechaini);
                        cmd.Parameters.AddWithValue("@fechafin", fechafin);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            listar = new List<Ent_Bataclub>();
                            listar = (from DataRow dr in dt.Rows
                                      select new Ent_Bataclub()
                                      {                                        
                                          cod_tienda = dr["COD_TIENDA"].ToString(),
                                          des_tienda = dr["DES_TIENDA"].ToString(),                                          
                                          semana= dr["SEMANA"].ToString(),
                                          fecha=(dr["FECHA"]).ToString(),
                                          dni = dr["DNI"].ToString(),
                                          bolfac= dr["BOLFAC"].ToString(),
                                          soles =Convert.ToDecimal(dr["SOLES"]),
                                          pares  =Convert.ToInt32(dr["PARES"]),
                                          estado  = dr["ESTADO"].ToString(),
                                          fecha_ing= dr["FECHA_ING"].ToString(),
                                          promocion= dr["PROMOCION"].ToString(),
                                      }).ToList();

                        }
                    }
                }
            }
            catch (Exception exc)
            {

                listar=null;
            }
            return listar;
        }
    }
}
