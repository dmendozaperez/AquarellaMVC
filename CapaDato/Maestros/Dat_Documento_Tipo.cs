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
    public class Dat_Documento_Tipo
    {
        public List<Ent_Documento_Tipo> get_lista()
        {
            List<Ent_Documento_Tipo> listar = null;
            string sqlquery = "USP_MVC_LEER_DOCUMENTO_TIPO";
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
                            listar = new List<Ent_Documento_Tipo>();
                            listar = (from DataRow fila in dt.Rows
                                      select new Ent_Documento_Tipo()
                                      {
                                          Doc_Tip_Id = fila["Doc_Tip_Id"].ToString(),
                                          Doc_Tip_Descripcion= fila["Doc_Tip_Descripcion"].ToString(),
                                      }).ToList();
                        }
                    }
                }

            }
            catch
            {

                listar = new List<Ent_Documento_Tipo>();
            }
            return listar;
        }
    }
}
