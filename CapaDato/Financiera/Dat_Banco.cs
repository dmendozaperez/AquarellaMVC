using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad.Financiera;
using CapaEntidad.Util;


namespace CapaDato.Financiera
{
    public class Dat_Banco
    {
        public List<Ent_Banco> Listar_Bancos()
        {
            List<Ent_Banco> Listar = new List<Ent_Banco>();
            string sqlquery = "[USP_Leer_Banco]";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {

                        cmd.CommandType = CommandType.StoredProcedure;
                        
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            Listar = new List<Ent_Banco>();
                            Listar = (from DataRow fila in dt.Rows
                                      select new Ent_Banco()
                                      {
                                          Codigo = fila["Ban_Id"].ToString(),
                                          Descripcion = fila["Ban_Descripcion"].ToString()
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
