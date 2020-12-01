using CapaEntidad.Articulo;
using CapaEntidad.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDato.Articulo
{
    public class Dat_Marca
    {
        public List<Ent_Marca> listar()
        {
            List<Ent_Marca> listar = null;
            string sqlquery = "[USP_Leer_Marca]";
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
                            listar = new List<Ent_Marca>();
                            listar = (from DataRow fila in dt.Rows
                                      select new Ent_Marca()
                                      {
                                          Mar_Id = fila["Mar_Id"].ToString(),
                                          Mar_Descripcion = fila["Mar_Descripcion"].ToString(),
                                      }
                                    ).ToList();
                        }
                    }
                }
            }
            catch
            {
                listar = new List<Ent_Marca>();
            }
            return listar;
        }
    }
}
