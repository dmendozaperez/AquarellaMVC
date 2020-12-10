using CapaEntidad.Financiera;
using CapaEntidad.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDato.Financiera
{
    public class Dat_Concepto
    {
        public List<Ent_Concepto> Listar_Conceptos()
        {
            List<Ent_Concepto> Listar = new List<Ent_Concepto>();
            string sqlquery = "[USP_Leer_ConceptoTipo]";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            Listar = new List<Ent_Concepto>();
                            Listar = (from DataRow dr in dt.Rows
                                      select new Ent_Concepto()
                                      {
                                          Codigo = Convert.ToString(dr["Con_Id"]),
                                          Descripcion = Convert.ToString(dr["Con_Descripcion"])                                          
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
