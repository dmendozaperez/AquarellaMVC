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
    public class Dat_Categoria
    {
        public List<Ent_Categoria> listar()
        {
            List<Ent_Categoria> listar = null;
            string sqlquery = "[USP_LeerCategoria]";
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
                            listar = new List<Ent_Categoria>();
                            listar = (from DataRow fila in dt.Rows
                                      select new Ent_Categoria()
                                      {
                                          Cat_Id= fila["Cat_Id"].ToString(),
                                          Cat_Descripcion = fila["Cat_Descripcion"].ToString(),
                                      }
                                    ).ToList();
                        }
                    }
                }
            }
            catch
            {
                listar = new List<Ent_Categoria>();
            }
            return listar;
        }
    }
}
