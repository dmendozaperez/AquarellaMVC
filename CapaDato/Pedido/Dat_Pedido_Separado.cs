using CapaEntidad.Pedido;
using CapaEntidad.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDato.Pedido
{
    public class Dat_Pedido_Separado
    {
        public List<Ent_Pedido_Separado> lista(string lider)
        {
            string sqlquery = "USP_MVC_Leer_Liquidacion_Separados";
            List<Ent_Pedido_Separado> listar = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                        {
                            cmd.CommandTimeout = 0;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@usuario", lider);
                            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                            {
                                DataTable dt = new DataTable();
                                da.Fill(dt);
                                listar = new List<Ent_Pedido_Separado>();
                                listar = (from DataRow fila in dt.Rows
                                          select new Ent_Pedido_Separado()
                                          {
                                              asesor = fila["asesor"].ToString(),
                                              lider = fila["Are_Descripcion"].ToString(),
                                              pedido = fila["Liq_Id"].ToString(),
                                              promotor = fila["nombres"].ToString(),
                                              fecha_ing = fila["fecha_ing"].ToString(),
                                              fecha_cad = fila["fecha_cad"].ToString(),
                                              tcantidad =Convert.ToInt32(fila["totalliq"]),
                                              telefono = fila["bas_telefono"].ToString(),
                                              celular = fila["Bas_Celular"].ToString(),
                                              ubicacion = fila["ubicacion"].ToString(),
                                              dias_pedido = Convert.ToInt32(fila["dias_pedido"]),  
                                              subtotal=Convert.ToDecimal(fila["subtotal"].ToString()),
                                          }
                                        ).ToList();

                            }
                        }
                    }
                    catch 
                    {

                        listar = new List<Ent_Pedido_Separado>();
                    }
                }
            }
            catch
            {

                listar = new List<Ent_Pedido_Separado>();
            }
            return listar;
        }
    }
}
