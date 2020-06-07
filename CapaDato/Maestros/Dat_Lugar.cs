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
    public class Dat_Lugar
    {
        public List<Ent_Lugar> get_lista()
        {
            List<Ent_Lugar> listar = null;
            string sqlquery = "USP_MVC_LEER_LUGAR";
            try
            {

                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            listar = new List<Ent_Lugar>();
                            listar = (from DataRow fila in dt.Rows
                                      select new Ent_Lugar()
                                      {
                                          dep_id = fila["dep_id"].ToString(),
                                          dep_descripcion = fila["dep_descripcion"].ToString(),
                                          prv_cod = fila["prv_cod"].ToString(),
                                          Prv_Descripcion = fila["Prv_Descripcion"].ToString(),
                                          Dis_Id = fila["Dis_Id"].ToString(),
                                          Dis_Descripcion = fila["Dis_Descripcion"].ToString(),
                                      }).ToList();
                        }
                    }
                }

            }
            catch
            {

                listar = new List<Ent_Lugar>();
            }
            return listar;
        }
    }
}
