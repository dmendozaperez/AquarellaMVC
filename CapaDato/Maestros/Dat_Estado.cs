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
    }
}
