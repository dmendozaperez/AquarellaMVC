using CapaEntidad.ValeCompra;
using CapaEntidad.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDato.ValeCompra
{
    public class Dat_Cliente
    {
       public List<Ent_Cliente> get_lista()
        {
            List<Ent_Cliente> list = null;
            string sqlquery = "USP_Leer_Cliente";
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
                            list = new List<Ent_Cliente>();

                            while (dr.Read())
                            {
                                Ent_Cliente cliente = new Ent_Cliente();
                                //cliente.cli_id = Int32.Parse(dr["Cli_ID"].ToString());
                                cliente.cli_Ruc = dr["ruc"].ToString();
                                //cliente.cli_nombreComercial = dr["Cli_Nombre"].ToString();
                                cliente.cli_razonSocial = dr["razon"].ToString();
                                //cliente.cli_codigo = dr["Cli_Codigo"].ToString();
                                //cliente.cli_Direccion = dr["Cli_Direccion"].ToString();
                                //cliente.cli_Estado = dr["Cli_Estado"].ToString();
                                
                                list.Add(cliente);

                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

                list = null;
            }
            return list;
        }
    }
}
