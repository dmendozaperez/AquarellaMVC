using CapaEntidad.Logistica;
using CapaEntidad.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDato.Logistica
{
    public class Dat_Despacho
    {
        public List<Ent_Lista_Despacho> get_lista_despacho(DateTime fec_ini,DateTime fec_fin,string tipo_des)
        {
            List<Ent_Lista_Despacho> listar = null;
            string sqlquery = "USP_MVC_Listar_Despacho_almacen";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@fecha_inicio", fec_ini);
                        cmd.Parameters.AddWithValue("@fecha_final", fec_fin);
                        cmd.Parameters.AddWithValue("@tipo_des", tipo_des);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            listar = new List<Ent_Lista_Despacho>();
                            listar = (from DataRow fila in dt.Rows
                                      select new Ent_Lista_Despacho()
                                      {
                                          desp_id=fila["desp_id"].ToString(),
                                          desp_nrodoc = fila["desp_nrodoc"].ToString(),
                                          desp_descripcion = fila["desp_descripcion"].ToString(),
                                          totalparesenviado = fila["totalparesenviado"].ToString(),
                                          estado = fila["estado"].ToString(),
                                          desp_fechacre = fila["desp_fechacre"].ToString(),
                                          desp_tipo = fila["desp_tipo"].ToString(),                                          
                                      }
                                    ).ToList();
                        }
                    }
                }
            }
            catch 
            {
                listar = new List<Ent_Lista_Despacho>();               
            }
            return listar;
        }
        public Ent_Despacho_Almacen get_despacho_almacen(String tipo_des,DateTime fec_ini,DateTime fec_fin)
        {
            string sqlquery = "USP_MVC_Buscar_Despacho_Almacen";
            Ent_Despacho_Almacen obj = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@tipo_des", tipo_des);
                        cmd.Parameters.AddWithValue("@fecha_inicio", fec_ini);
                        cmd.Parameters.AddWithValue("@fecha_final", fec_fin);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataSet ds = new DataSet();
                            da.Fill(ds);
                            DataTable dtcab = ds.Tables[0];
                            DataTable dtliq = ds.Tables[1];
                            List<Ent_Despacho_Almacen_Cab> lista_cab = new List<Ent_Despacho_Almacen_Cab>();
                            lista_cab = (from DataRow fila in dtcab.Rows
                                         select new Ent_Despacho_Almacen_Cab()
                                         {
                                             Asesor=fila["Asesor"].ToString(),
                                             area_id = fila["area_id"].ToString(),
                                             NombreLider = fila["NombreLider"].ToString(),
                                             Promotor = fila["Promotor"].ToString(),
                                             Rotulo = fila["Rotulo"].ToString(),
                                             rotulo_courier = fila["rotulo_courier"].ToString(),
                                             Agencia = fila["Agencia"].ToString(),
                                             Destino = fila["Destino"].ToString(),
                                             Pedido = fila["Pedido"].ToString(),
                                             TotalPares =Convert.ToInt32(fila["TotalPares"]),
                                             TotalCatalogo = Convert.ToInt32(fila["TotalCatalogo"]),
                                             TotalPremio = Convert.ToInt32(fila["TotalPremio"]),
                                             TotalCantidad = Convert.ToInt32(fila["TotalCantidad"]),
                                             TotalVenta = Convert.ToDecimal(fila["TotalVenta"]),
                                             Igv = Convert.ToDecimal(fila["Igv"]),
                                             McaFlete = fila["McaFlete"].ToString(),
                                             Flete = fila["Flete"].ToString(),
                                             Lid_Prom = fila["Lid_Prom"].ToString()                                             
                                         }
                                       ).ToList();

                            List<Ent_Despacho_Almacen_Liquidacion> lista_liq = new List<Ent_Despacho_Almacen_Liquidacion>();
                            lista_liq = (from DataRow fila in dtliq.Rows
                                         select new Ent_Despacho_Almacen_Liquidacion
                                         {
                                             area_id=fila["area_id"].ToString(),
                                             liq_id = fila["liq_id"].ToString(),
                                             lid_prom = fila["lid_prom"].ToString(),
                                             bas_tip_des = fila["bas_tip_des"].ToString(),
                                         }
                                       ).ToList();

                            obj = new Ent_Despacho_Almacen();
                            obj.despacho_cab = lista_cab;
                            obj.despacho_liq = lista_liq;


                        }

                    }
                }
            }
            catch(Exception exc)
            {

                obj = new Ent_Despacho_Almacen();
            }
            return obj;
        }
        public List<Ent_Lista_Rotulo> listar_rotulo_x_lider(string id_lider)
        {
            string sqlquery = "USP_MVC_Despacho_listarRotulo";
            List<Ent_Lista_Rotulo> listar = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@idLider", id_lider);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            listar = new List<Ent_Lista_Rotulo>();
                            listar = (from DataRow fila in dt.Rows
                                      select new Ent_Lista_Rotulo()
                                      {
                                          idlider=fila["idlider"].ToString(),
                                          bas_id = fila["bas_id"].ToString(),
                                          descripcion = fila["descripcion"].ToString(),
                                          documento = fila["documento"].ToString(),
                                          direccion = fila["direccion"].ToString(),
                                          destino = fila["destino"].ToString(),
                                          telefono = fila["telefono"].ToString(),
                                          lid_prom= fila["LidProm"].ToString(),
                                      }
                                    ).ToList();


                        }
                    }
                }
            }
            catch 
            {
                listar = new List<Ent_Lista_Rotulo>();
            }
            return listar;
        }
        public string insertar_despacho(Decimal _usu, ref int IdDespacho, string strListDetalle, string strListLiderDespachoLiquidacion, string Descripcion)
        {
            string sqlquery = "[USP_MVC_Insertar_Despacho]";
            string valida = "";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    try
                    {
                        if (cn.State == 0) cn.Open();
                        using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                        {
                            cmd.CommandTimeout = 0;
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@strListDetalle", strListDetalle);
                            cmd.Parameters.AddWithValue("@strListLiqui", strListLiderDespachoLiquidacion);
                            cmd.Parameters.AddWithValue("@strDescripcion", Descripcion);
                            cmd.Parameters.AddWithValue("@UsuCrea", _usu);

                            cmd.Parameters.Add("@IdDespacho", SqlDbType.Int);
                            cmd.Parameters["@IdDespacho"].SqlValue = IdDespacho;
                            cmd.Parameters["@IdDespacho"].Direction = ParameterDirection.InputOutput;

                            cmd.ExecuteNonQuery();

                            IdDespacho = Convert.ToInt32(cmd.Parameters["@IdDespacho"].Value);
                        }

                    }
                    catch(Exception exc)
                    {
                        valida = exc.Message;
                    }
                    if (cn != null)
                        if (cn.State == ConnectionState.Open) cn.Close();
                }
            }
            catch (Exception exc)
            {
                valida = exc.Message;
            }
            return valida;
        }

        public string update_despacho(Decimal _usu, int IdDespacho, string strListDetalle, string Descripcion)
        {
            string sqlquery = "[USP_MVC_Actualizar_Despacho]";
            string valida = "";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    try
                    {
                        if (cn.State == 0) cn.Open();
                        using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                        {
                            cmd.CommandTimeout = 0;
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@strListDetalle", strListDetalle);                            
                            cmd.Parameters.AddWithValue("@strDescripcion", Descripcion);
                            cmd.Parameters.AddWithValue("@UsuCrea", _usu);

                            cmd.Parameters.AddWithValue("@IdDespacho", IdDespacho);
                            
                            cmd.ExecuteNonQuery();
                            
                        }

                    }
                    catch (Exception exc)
                    {
                        valida = exc.Message;
                    }
                    if (cn != null)
                        if (cn.State == ConnectionState.Open) cn.Close();
                }
            }
            catch (Exception exc)
            {
                valida = exc.Message;
            }
            return valida;
        }
        public string eliminar_despacho_item(Decimal _usu, int IdDespacho_detalle, int lid_prom)
        {
            string sqlquery = "[USP_MVC_Anular_DespachoDetalle]";
            string valida = "";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    try
                    {
                        if (cn.State == 0) cn.Open();
                        using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                        {
                            cmd.CommandTimeout = 0;
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@usuario", _usu);
                            cmd.Parameters.AddWithValue("@IdDespachoDetalle", IdDespacho_detalle);
                            cmd.Parameters.AddWithValue("@lid_prom", lid_prom);
                                                        
                            cmd.ExecuteNonQuery();
                            
                        }

                    }
                    catch (Exception exc)
                    {
                        valida = exc.Message;
                    }
                    if (cn != null)
                        if (cn.State == ConnectionState.Open) cn.Close();
                }
            }
            catch (Exception exc)
            {
                valida = exc.Message;
            }
            return valida;
        }


        public Ent_Despacho_Almacen_Editar get_despacho_almacen_editar(Int32 id_despacho)
        {
            string sqlquery = "USP_MVC_obtener_Despacho";
            Ent_Despacho_Almacen_Editar obj = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_despacho", id_despacho);
                        
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataSet ds = new DataSet();
                            da.Fill(ds);
                            DataTable dtcab = ds.Tables[0];
                            DataTable dtdet = ds.Tables[1];
                            List<Ent_Despacho_Almacen_Cab_Update> lista_cab = new List<Ent_Despacho_Almacen_Cab_Update>();
                            lista_cab = (from DataRow fila in dtcab.Rows
                                         select new Ent_Despacho_Almacen_Cab_Update
                                         {
                                             Desp_NroDoc = fila["Desp_NroDoc"].ToString(),
                                             Desp_Descripcion = fila["Desp_Descripcion"].ToString(),
                                             estado = fila["estado"].ToString(),
                                             Desp_FechaCre = fila["Desp_FechaCre"].ToString(),
                                             asesor = fila["asesor"].ToString(),
                                             NombreLider = fila["NombreLider"].ToString(),
                                             Promotor = fila["Promotor"].ToString(),
                                             Rotulo = fila["Rotulo"].ToString(),
                                             Rotulo_Courier = fila["Rotulo_Courier"].ToString(),
                                             Agencia = fila["Agencia"].ToString(),
                                             Destino = fila["Destino"].ToString(),
                                             Pedido = fila["Pedido"].ToString(),
                                             TotalPremio = Convert.ToInt32(fila["TotalPremio"]),
                                             TotalPremioEnviado = Convert.ToInt32(fila["TotalPremioEnviado"]),
                                             TotalCatalogo = Convert.ToInt32(fila["TotalCatalogo"]),
                                             TotalCatalogEnviado = Convert.ToInt32(fila["TotalCatalogEnviado"]),
                                             TotalPares = Convert.ToInt32(fila["TotalPares"]),
                                             TotalParesEnviado = Convert.ToInt32(fila["TotalParesEnviado"]),
                                             Total_Cantidad = Convert.ToInt32(fila["Total_Cantidad"]),
                                             Total_Cantidad_Envio = Convert.ToInt32(fila["Total_Cantidad_Envio"]),
                                             TotalVenta = Convert.ToDecimal(fila["TotalVenta"]),
                                             CobroFlete = fila["CobroFlete"].ToString(),
                                             Courier = fila["Courier"].ToString(),
                                             Observacion = fila["Observacion"].ToString(),
                                             Detalle = fila["Detalle"].ToString(),
                                             McaCourier = fila["McaCourier"].ToString(),
                                             McaFlete = fila["McaFlete"].ToString(),
                                             Enviado = Convert.ToInt32(fila["Enviado"]),
                                             Desp_IdDetalle = fila["Desp_IdDetalle"].ToString(),
                                             Desp_id = fila["Desp_id"].ToString(),
                                             TotalParesEnviadoEdit =Convert.ToInt32(fila["TotalParesEnviadoEdit"]),
                                             TotalCatalogEnviadoEdit = Convert.ToInt32(fila["TotalCatalogEnviadoEdit"]),
                                             TotalPremioEnviadoEdit = Convert.ToInt32(fila["TotalPremioEnviadoEdit"]),
                                             IdEstado = fila["IdEstado"].ToString(),
                                             atendido = fila["atendido"].ToString(),
                                             IdLider = fila["IdLider"].ToString(),
                                             Lid_Prom = fila["Lid_Prom"].ToString(),
                                         }
                                       ).ToList();

                            List<Ent_Despacho_Almacen_Det_Update> lista_det = new List<Ent_Despacho_Almacen_Det_Update>();
                            lista_det = (from DataRow fila in dtdet.Rows
                                         select new Ent_Despacho_Almacen_Det_Update
                                         {
                                             NroPedidos =Convert.ToInt32(fila["NroPedidos"]),
                                             NroEnviados = Convert.ToInt32(fila["NroEnviados"]),
                                             NroPremio = Convert.ToInt32(fila["NroPremio"]),
                                             PremioEnviados = Convert.ToInt32(fila["PremioEnviados"]),
                                             CatalogEnviados = Convert.ToInt32(fila["CatalogEnviados"]),
                                             CatalogPedidos = Convert.ToInt32(fila["CatalogPedidos"]),
                                             MontoTotal =Convert.ToDecimal(fila["MontoTotal"]),                                            

                                         }
                                       ).ToList();

                            obj = new Ent_Despacho_Almacen_Editar();
                            obj.Almacen_Cab_Update = lista_cab;
                            obj.Almacen_Det_Update = lista_det;


                        }

                    }
                }
            }
            catch (Exception exc)
            {
                throw exc;
                
            }
            return obj;
        }
    }
}
