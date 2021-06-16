using CapaEntidad.Facturacion;
using CapaEntidad.Control;
using CapaEntidad.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace CapaDato.Facturacion
{
    public class Dat_Facturacion
    {

        public List<Ent_Movimientos_Ventas> ListarTipoArticulo()
        {
            List<Ent_Movimientos_Ventas> Listar = new List<Ent_Movimientos_Ventas>();
            string sqlquery = "[USP_Leer_TipoArticulo]";
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

                            Listar = new List<Ent_Movimientos_Ventas>();
                            Listar = (from DataRow fila in dt.Rows
                                      select new Ent_Movimientos_Ventas()
                                      {
                                          Codigo = Convert.ToString(fila["Codigo"]),
                                          Descripcion = Convert.ToString(fila["Nombres"])
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

        public List<Ent_Movimientos_Ventas> ListarVenPorCategoria(Ent_Movimientos_Ventas _Ent)
        {
            List<Ent_Movimientos_Ventas> Listar = new List<Ent_Movimientos_Ventas>();
            string sqlquery = "[USP_Leer_Venta_MajorCategoria]";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@fechaini", DbType.DateTime).Value = _Ent.FechaInicio;
                        cmd.Parameters.AddWithValue("@fechafin", DbType.DateTime).Value = _Ent.FechaFin;
                        cmd.Parameters.AddWithValue("@idtipoarticulo", DbType.String).Value = _Ent.IdTipoArticulo;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            Listar = new List<Ent_Movimientos_Ventas>();
                            Listar = (from DataRow fila in dt.Rows
                                      select new Ent_Movimientos_Ventas()
                                      {
                                          Mcv_Description = (fila["Mcv_Description"] is DBNull) ? string.Empty : (string)(fila["Mcv_Description"]),
                                          Anno = (fila["Anno"] is DBNull) ? (int?)null : Convert.ToInt32(fila["Anno"]),
                                          Can_Week_No = (fila["Can_Week_No"] is DBNull) ? (int?)null : Convert.ToInt32(fila["Can_Week_No"]),
                                          Ventas = Convert.ToDecimal(fila["Ventas"]),
                                          Podv = Convert.ToDecimal(fila["Podv"]),
                                          Pventas =  Convert.ToDecimal(fila["Pventas"]),
                                          Pventasneto =  Convert.ToDecimal(fila["Pventasneto"]),
                                          Pmargen = Convert.ToDecimal(fila["Pmargen"]),
                                          Pmargenpor = Convert.ToDecimal(fila["Pmargenpor"])
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

        public List<Ent_Comisiones> ListarComisiones(Ent_Comisiones _Ent)
        {
            List<Ent_Comisiones> Listar = new List<Ent_Comisiones>();
            string sqlquery = "[USP_Reporte_Comision]";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@fecha_ini", DbType.DateTime).Value = _Ent.FechaInicio;
                        cmd.Parameters.AddWithValue("@fecha_fin", DbType.DateTime).Value = _Ent.FechaFin;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            Listar = new List<Ent_Comisiones>();
                            Listar = (from DataRow fila in dt.Rows
                                      select new Ent_Comisiones()
                                      {
                                          AreaId = Convert.ToInt32(fila["AreaId"]),
                                          Asesor = (fila["Asesor"] is DBNull) ? string.Empty : (string)(fila["Asesor"]),
                                          Lider = (fila["Lider"] is DBNull) ? string.Empty : (string)(fila["Lider"]),
                                          LiderDni = (fila["LiderDni"] is DBNull) ? string.Empty : (string)(fila["LiderDni"]),
                                          TotPares = (fila["Total Pares"] is DBNull) ? (Decimal?)null : Convert.ToDecimal(fila["Total Pares"]),
                                          TotVenta = (fila["Total Venta"] is DBNull) ? (Decimal?)null : Convert.ToDecimal(fila["Total Venta"]),
                                          PorComision = (fila["% de Comision"] is DBNull) ? (Decimal?)null : Convert.ToDecimal(fila["% de Comision"]),
                                          Comision = (fila["Comision"] is DBNull) ? (Decimal?)null : Convert.ToDecimal(fila["Comision"]),
                                          Bonosnuevas = (fila["Bonos nuevas"] is DBNull) ? (Decimal?)null : Convert.ToDecimal(fila["Bonos nuevas"]),
                                          SubTotalSinIGV = (fila["SubTotal Sin IGV"] is DBNull) ? (Decimal?)null : Convert.ToDecimal(fila["SubTotal Sin IGV"])
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

        public List<Ent_Resumen_Ventas> ListarResumenVenta(Ent_Resumen_Ventas _Ent)
        {
            List<Ent_Resumen_Ventas> Listar = new List<Ent_Resumen_Ventas>();
            string sqlquery = "[USP_LeerResventa]";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ANIOACT", DbType.Int32).Value = _Ent.Anno;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            Listar = new List<Ent_Resumen_Ventas>();
                            Listar = (from DataRow fila in dt.Rows
                                      select new Ent_Resumen_Ventas()
                                      {
                                          Anno = (fila["Año"] is DBNull) ? (int?)null : Convert.ToInt32(fila["Año"]),
                                          Semana = (fila["Semana"] is DBNull) ? (int?)null : Convert.ToInt32(fila["Semana"]),
                                          TotalTickets = (fila["Total Tickets"] is DBNull) ? (int?)null : Convert.ToInt32(fila["Total Tickets"]),
                                          Pares = (fila["Pares"] is DBNull) ? (int?)null : Convert.ToInt32(fila["Pares"]),
                                          TotalIgv = (fila["Total + Igv"] is DBNull) ? (Decimal?)null : Convert.ToDecimal(fila["Total + Igv"]),
                                          PrecioPromedio = (fila["Precio Promedio"] is DBNull) ? (Decimal?)null : Convert.ToDecimal(fila["Precio Promedio"]),
                                          NParesTicket = (fila["N Pares por Ticket"] is DBNull) ? (Decimal?)null : Convert.ToDecimal(fila["N Pares por Ticket"]),
                                          Anno1 = (fila["Año1"] is DBNull) ? (int?)null : Convert.ToInt32(fila["Año1"]),
                                          Semana1 = (fila["Semana1"] is DBNull) ? (int?)null : Convert.ToInt32(fila["Semana1"]),
                                          TotalTickets1 = (fila["Total Tickets1"] is DBNull) ? (int?)null : Convert.ToInt32(fila["Total Tickets1"]),
                                          Pares1 = (fila["Pares1"] is DBNull) ? (int?)null : Convert.ToInt32(fila["Pares1"]),
                                          TotalIgv1 = (fila["Total + Igv1"] is DBNull) ? (Decimal?)null : Convert.ToDecimal(fila["Total + Igv1"]),
                                          PrecioPromedio1 = (fila["Precio Promedio1"] is DBNull) ? (Decimal?)null : Convert.ToDecimal(fila["Precio Promedio1"]),
                                          NParesTicket1 = (fila["N Pares por Ticket1"] is DBNull) ? (Decimal?)null : Convert.ToDecimal(fila["N Pares por Ticket1"])
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
        public List<Ent_Resumen_Ventas> ListarAnno()
        {
            List<Ent_Resumen_Ventas> Listar = new List<Ent_Resumen_Ventas>();
            string sqlquery = "[USP_Leer_Año]";
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

                            Listar = new List<Ent_Resumen_Ventas>();
                            Listar = (from DataRow fila in dt.Rows
                                      select new Ent_Resumen_Ventas()
                                      {
                                          Codigo =  Convert.ToInt32(fila["idanio"]),
                                          Descripcion = Convert.ToInt32(fila["anio"])
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

        public DataTable Consulta_Lider_N(Ent_Lider_Ventas _Ent)
        {
            string sqlquery = "USP_Consulta_Venta_LiderN";
            SqlConnection cn = null;
            SqlCommand cmd = null;
            SqlDataAdapter da = null;
            DataTable dt = null;
            try
            {
                cn = new SqlConnection(Ent_Conexion.conexion);
                cmd = new SqlCommand(sqlquery, cn);
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@fecha_ini", DbType.DateTime).Value = _Ent.FechaInicio;
                cmd.Parameters.AddWithValue("@fecha_fin", DbType.DateTime).Value = _Ent.FechaFin;
                da = new SqlDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);
            }
            catch
            {
                dt = null;
            }
            return dt;
        }

        public List<Ent_Ventas_Tallas> ListarVentaTalla(Ent_Ventas_Tallas _Ent)
        {
            List<Ent_Ventas_Tallas> Listar = new List<Ent_Ventas_Tallas>();
            string sqlquery = "[USP_MVC_ConsultaVentTalla_Stk]";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@fechaini", DbType.String).Value = _Ent.FechaInicio;
                        cmd.Parameters.AddWithValue("@fechafin", DbType.String).Value = _Ent.FechaFin;
                        cmd.Parameters.AddWithValue("@articulo", DbType.String).Value = _Ent.Articulo;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            Listar = new List<Ent_Ventas_Tallas>();
                            Listar = (from row in dt.AsEnumerable()
                                      group row by new
                                      {
                                          Articulo = row.Field<string>("articulo"),
                                          Pares_Venta = row.Field<Decimal>("pares_venta")
                                      } into g
                                      select new Ent_Ventas_Tallas()
                                      {
                                          Articulo = g.Key.Articulo,
                                          Pares_Venta = g.Key.Pares_Venta,
                                          TotalParesStock = g.Sum(s => s.Field<Decimal>("pares_stock")),
                                          _ListarDetalle = (from DataRow fila in
                                                            dt.AsEnumerable().Where(b => b.Field<string>("articulo") == g.Key.Articulo &&
                                                                                         b.Field<Decimal>("pares_venta") == g.Key.Pares_Venta)
                                                            select new Ent_Ventas_Talla_Detalle()
                                                            {
                                                                Talla = fila["talla"].ToString(),
                                                                Pares_Stock = Convert.ToInt32(fila["pares_stock"]),
                                                            }
                                                       ).ToList()
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
        public List<Ent_Salida_Almacen> ListarSalidaDespacho(Ent_Salida_Almacen _Ent)
        {
            List<Ent_Salida_Almacen> Listar = new List<Ent_Salida_Almacen>();
            string sqlquery = "[USP_MVC_Listar_Despacho_almacen]";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@fecha_inicio", DbType.DateTime).Value = _Ent.FechaInicio;
                        cmd.Parameters.AddWithValue("@fecha_final", DbType.DateTime).Value = _Ent.FechaFin;
                        cmd.Parameters.AddWithValue("@tipo_des", DbType.String).Value = _Ent.Tipo;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            Listar = new List<Ent_Salida_Almacen>();
                            Listar = (from DataRow fila in dt.Rows
                                      select new Ent_Salida_Almacen()
                                      {
                                          IdDespacho =  Convert.ToInt32(fila["Desp_Id"]),
                                          Desp_Nrodoc = (fila["Desp_Nrodoc"] is DBNull) ? string.Empty : (string)(fila["Desp_Nrodoc"]),
                                          Desp_Descripcion = (fila["Desp_Descripcion"] is DBNull) ? string.Empty : (string)(fila["Desp_Descripcion"]),
                                          Desp_Tipo_Descripcion = (fila["Desp_Tipo_Descripcion"] is DBNull) ? string.Empty : (string)(fila["Desp_Tipo_Descripcion"]),
                                          Desp_Tipo = (fila["Desp_Tipo"] is DBNull) ? string.Empty : (string)(fila["Desp_Tipo"]),
                                          TotalParesEnviado = (fila["TotalParesEnviado"] is DBNull) ? (Decimal?)null : Convert.ToDecimal(fila["TotalParesEnviado"]),
                                          Estado = (fila["Estado"] is DBNull) ? string.Empty : (string)(fila["Estado"]),
                                          Desp_FechaCre = (fila["Desp_FechaCre"] is DBNull) ? string.Empty : (string)(fila["Desp_FechaCre"])
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

        public Ent_Salida_Almacen ListarDespacho(Ent_Salida_Almacen _Ent)
        {
            string sqlquery = "USP_obtener_Despacho";
            Ent_Salida_Almacen obj = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_despacho", DbType.String).Value = _Ent.IdDespacho;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataSet ds = new DataSet();
                            da.Fill(ds);
                            DataTable Dt1 = ds.Tables[0];
                            DataTable Dt2 = ds.Tables[1];
                            List<Ent_Edit_Salida_Almacen_Detalle> _Detalle = new List<Ent_Edit_Salida_Almacen_Detalle>();
                            _Detalle = (from DataRow row in Dt1.Rows
                                         select new Ent_Edit_Salida_Almacen_Detalle()
                                         {
                                             Desp_NroDoc = row.Field<string>("Desp_NroDoc"),
                                             Desp_Descripcion = row.Field<string>("Desp_Descripcion"),
                                             Estado = row.Field<string>("Estado"),
                                             Desp_FechaCre = row.Field<string>("Desp_FechaCre"),
                                             Asesor = row.Field<string>("Asesor"),
                                             NombreLider = row.Field<string>("NombreLider"),
                                             Promotor = row.Field<string>("Promotor"),
                                             Rotulo = row.Field<string>("Rotulo"),
                                             Rotulo_Courier = row.Field<string>("Rotulo_Courier"),
                                             Agencia = row.Field<string>("Agencia"),
                                             Destino = row.Field<string>("Destino"),
                                             Pedido = row.Field<string>("Pedido"),
                                             TotalPremio = row.Field<Decimal?>("TotalPremio"),
                                             TotalPremioEnviado = row.Field<Decimal?>("TotalPremioEnviado"),
                                             TotalCatalogo = row.Field<Decimal?>("TotalCatalogo"),
                                             TotalCatalogEnviado = row.Field<Decimal?>("TotalCatalogEnviado"),
                                             TotalPares = row.Field<Decimal?>("TotalPares"),
                                             TotalParesEnviado = row.Field<Decimal?>("TotalParesEnviado"),
                                             Total_Cantidad = row.Field<Decimal?>("Total_Cantidad"),
                                             Total_Cantidad_Envio = row.Field<Decimal?>("Total_Cantidad_Envio"),
                                             TotalVenta = row.Field<Decimal?>("TotalVenta"),
                                             CobroFlete = row.Field<string>("CobroFlete"),
                                             Courier = row.Field<string>("Courier"),
                                             Observacion = row.Field<string>("Observacion"),
                                             Detalle = row.Field<string>("Detalle"),
                                             McaCourier = row.Field<string>("McaCourier"),
                                             McaFlete = row.Field<string>("McaFlete"),
                                             Enviado = row.Field<Decimal?>("Enviado"),
                                             Desp_IdDetalle = row.Field<Decimal?>("Desp_IdDetalle"),
                                             Desp_id = row.Field<Decimal?>("Desp_id"),
                                             TotalParesEnviadoEdit = row.Field<Decimal?>("TotalParesEnviadoEdit"),
                                             TotalCatalogEnviadoEdit = row.Field<Decimal?>("TotalCatalogEnviadoEdit"),
                                             TotalPremioEnviadoEdit = row.Field<Decimal?>("TotalPremioEnviadoEdit"),
                                             IdEstado = row.Field<string>("IdEstado"),
                                             Atendido = row.Field<string>("Atendido"),
                                             IdLider = row.Field<Decimal?>("IdLider"),
                                             Lid_Prom = row.Field<string>("Lid_Prom"),
                                             Desp_Tipo_Des = row.Field<string>("Desp_Tipo_Des"),
                                             Desp_Tipo = row.Field<string>("Desp_Tipo"),
                                             Delivery = row.Field<string>("Delivery"),
                                             Dni_Promotor = row.Field<string>("DniPromotor")
                                         }
                                       ).ToList();

                            Ent_Edit_Salida_Almacen_Cabecera _Cabecera = new Ent_Edit_Salida_Almacen_Cabecera() {

                                Desp_NroDoc = Dt1.Rows[0]["Desp_NroDoc"].ToString(),
                                Desp_id = Convert.ToDecimal(Dt1.Rows[0]["Desp_id"]),
                                Estado = Dt1.Rows[0]["Estado"].ToString(),
                                Desp_FechaCre = Dt1.Rows[0]["Desp_FechaCre"].ToString(),
                                Desp_Tipo_Des = Dt1.Rows[0]["Desp_Tipo_Des"].ToString(),
                                Desp_Tipo = Dt1.Rows[0]["Desp_Tipo"].ToString(),
                                Desp_Descripcion = Dt1.Rows[0]["Desp_Descripcion"].ToString(),
                                Atendido = Dt1.Rows[0]["Atendido"].ToString(),
                                IdEstado = Dt1.Rows[0]["IdEstado"].ToString(),
                                NroPedidos = Convert.ToInt32(Dt2.Rows[0]["NroPedidos"]),
                                NroEnviados = Convert.ToInt32(Dt2.Rows[0]["NroEnviados"]),
                                NroPremio = Convert.ToInt32(Dt2.Rows[0]["NroPremio"]),
                                PremioEnviados = Convert.ToInt32(Dt2.Rows[0]["PremioEnviados"]),
                                CatalogEnviados = Convert.ToInt32(Dt2.Rows[0]["CatalogEnviados"]),
                                CatalogPedidos = Convert.ToInt32(Dt2.Rows[0]["CatalogPedidos"]),
                                MontoTotal = Convert.ToDecimal(Dt2.Rows[0]["MontoTotal"])
                            };
                            obj = new Ent_Salida_Almacen();
                            obj._Cabecera = _Cabecera;
                            obj._Detalle = _Detalle;
                        }

                    }
                }
            }
            catch (Exception exc)
            {

                obj = new Ent_Salida_Almacen();
            }
            return obj;
        }

        public bool ActualizarSalidaDespacho(Ent_Edit_Salida_Almacen_Cabecera _Ent)
        {
            string sqlquery = "USP_Actualizar_Salida_Despacho";
            SqlConnection cn = null;
            SqlCommand cmd = null;
            bool _valida = false;
            try
            {
                cn = new SqlConnection(Ent_Conexion.conexion);
                if (cn.State == 0) cn.Open();
                cmd = new SqlCommand(sqlquery, cn);
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdDespacho", _Ent.Desp_id);
                cmd.Parameters.AddWithValue("@strEstadoDespacho", _Ent.Estado);
                cmd.Parameters.AddWithValue("@strListDetalle", _Ent.strDataDetalle);
                cmd.Parameters.AddWithValue("@strFlgAtendido", _Ent.Atendido);
                cmd.Parameters.AddWithValue("@UsuCrea",_Ent.UsuarioCrea);
                cmd.ExecuteNonQuery();
                _valida = true;
            }
            catch (Exception ex)
            {
                _valida = false;
                throw;
            }
            return _valida;
        }

        public  DataTable ListarVentaSemanal(Ent_Ventas_Semanales _Ent)
        {
            string sqlquery = "USP_Leer_Venta_Semanal";
            SqlConnection cn = null;
            SqlCommand cmd = null;
            SqlDataAdapter da = null;
            DataTable dt = null;
            try
            {
                cn = new SqlConnection(Ent_Conexion.conexion);
                cmd = new SqlCommand(sqlquery, cn);
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@var_fechaini", DbType.DateTime).Value = _Ent.FechaInicio;
                cmd.Parameters.AddWithValue("@var_fechafin", DbType.DateTime).Value = _Ent.FechaFin;
                da = new SqlDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);
            }
            catch
            {
                dt = null;
            }
            return dt;
        }

        public DataTable ListarVentaLider(Ent_Ventas_Lider _Ent)
        {
            string sqlquery = "USP_MVC_Leer_Venta_UniMon";
            SqlConnection cn = null;
            SqlCommand cmd = null;
            SqlDataAdapter da = null;
            DataTable dt = null;
            try
            {
                cn = new SqlConnection(Ent_Conexion.conexion);
                cmd = new SqlCommand(sqlquery, cn);
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@bas_id", DbType.String).Value = _Ent.Bas_Id;
                cmd.Parameters.AddWithValue("@fecha_inicio", DbType.DateTime).Value = _Ent.FechaInicio;
                cmd.Parameters.AddWithValue("@fecha_final", DbType.DateTime).Value = _Ent.FechaFin;
                cmd.Parameters.AddWithValue("@asesor", DbType.String).Value = _Ent.Asesor;
                da = new SqlDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);
            }
            catch
            {
                dt = null;
            }
            return dt;
        }

        public List<Ent_Campaña_Fecha> ListarCampaniaFecha()
        {
            List<Ent_Campaña_Fecha> Listar = new List<Ent_Campaña_Fecha>();
            string sqlquery = "[USP_MVC_LISTAR_CAMPFECHA]";
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
                            Listar = new List<Ent_Campaña_Fecha>();
                            Listar = (from DataRow fila in dt.Rows
                                      select new Ent_Campaña_Fecha()
                                      {
                                          Anio = Convert.ToInt32(fila["Anio"]),
                                          CamFec_Num = Convert.ToInt32(fila["CamFec_Num"]),
                                          CamFec_Ini = Convert.ToString(fila["CamFec_Ini"]),
                                          CamFec_Fin = Convert.ToString(fila["CamFec_Fin"]),
                                          CamFec_Nom = Convert.ToString(fila["CamFec_Nom"])
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

        public List<Ent_Consulta_Premios> List_ConsultaPremio(Ent_Consulta_Premios _Ent)
        {
            List<Ent_Consulta_Premios> Listar = new List<Ent_Consulta_Premios>();
            string sqlquery = "[USP_MVC_ConsultaPremios]";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@FECHA_INI_3", DbType.DateTime).Value = _Ent.FechaIni;
                        cmd.Parameters.AddWithValue("@FECHA_FIN_3", DbType.DateTime).Value = _Ent.FechaFin;
                        cmd.Parameters.AddWithValue("@VALIDA", DbType.Boolean).Value = _Ent.Valida;
                        cmd.Parameters.AddWithValue("@BAS_ID", DbType.String).Value = _Ent.Bas_Id;
                        cmd.Parameters.AddWithValue("@ASESOR", DbType.String).Value = _Ent.Bas_Aco_Id;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            Listar = new List<Ent_Consulta_Premios>();
                            Listar = (from DataRow fila in dt.Rows
                                      select new Ent_Consulta_Premios()
                                      {
                                          Asesor = (fila["Asesor"] is DBNull) ? string.Empty : (string)(fila["Asesor"]),
                                          Lider = (fila["Lider"] is DBNull) ? string.Empty : (string)(fila["Lider"]),
                                          Promotor = (fila["Promotor"] is DBNull) ? string.Empty : (string)(fila["Promotor"]),
                                          Documento = (fila["Documento"] is DBNull) ? string.Empty : (string)(fila["Documento"]),
                                          Total = (fila["Total"] is DBNull) ? (Decimal?)null : Convert.ToDecimal(fila["Total"]),
                                          Limite = (fila["Limite"] is DBNull) ? (Decimal?)null : Convert.ToDecimal(fila["Limite"]),
                                          Saldo = (fila["Saldo"] is DBNull) ? (Decimal?)null : Convert.ToDecimal(fila["Saldo"]),
                                          Descripcion = (fila["Descripcion"] is DBNull) ? string.Empty : (string)(fila["Descripcion"]),
                                          Liqprem = (fila["Liqprem"] is DBNull) ? string.Empty : (string)(fila["Liqprem"]),
                                          Liqpremiori = (fila["Liqpremiori"] is DBNull) ? string.Empty : (string)(fila["Liqpremiori"]),
                                          Xentrega = (fila["Xentrega"] is DBNull) ? string.Empty : (string)(fila["Xentrega"])
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

        public List<Ent_Comision_Lider> Listar_Comision_Lider(Ent_Comision_Lider _Ent)
        {
            List<Ent_Comision_Lider> Listar = new List<Ent_Comision_Lider>();
            string sqlquery = "[USP_MVC_Leer_ComisionPersona]";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@FECHA_INI", DbType.DateTime).Value = _Ent.FechaIni;
                        cmd.Parameters.AddWithValue("@FECHA_FIN", DbType.DateTime).Value = _Ent.FechaFin;
                        cmd.Parameters.AddWithValue("@BAS_ID", DbType.String).Value = _Ent.Bas_Id;
                        cmd.Parameters.AddWithValue("@ASESOR", DbType.String).Value = _Ent.Bas_Aco_Id;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            Listar = new List<Ent_Comision_Lider>();
                            Listar = (from DataRow fila in dt.Rows
                                      select new Ent_Comision_Lider()
                                      {
                                          AreaId = (fila["AreaId"] is DBNull) ? string.Empty : Convert.ToString(fila["AreaId"]),
                                          Asesor = (fila["Asesor"] is DBNull) ? string.Empty : Convert.ToString(fila["Asesor"]),
                                          Lider = (fila["Lider"] is DBNull) ? string.Empty : Convert.ToString(fila["Lider"]),
                                          LiderDni = (fila["LiderDni"] is DBNull) ? string.Empty : Convert.ToString(fila["LiderDni"]),
                                          TotalPares = (fila["TotalPares"] is DBNull) ? (Decimal?)null : Convert.ToDecimal(fila["TotalPares"]),
                                          TotalVenta = (fila["TotalVenta"] is DBNull) ? (Decimal?)null : Convert.ToDecimal(fila["TotalVenta"]),
                                          PorcentajeComision = (fila["PorcentajeComision"] is DBNull) ? (Decimal?)null : Convert.ToDecimal(fila["PorcentajeComision"]),
                                          Comision = (fila["Comision"] is DBNull) ? (Decimal?)null : Convert.ToDecimal(fila["Comision"]),
                                          BonosNuevas = (fila["BonosNuevas"] is DBNull) ? (Decimal?)null : Convert.ToDecimal(fila["BonosNuevas"]),
                                          SubTotalSinIGV = (fila["SubTotalSinIGV"] is DBNull) ? (Decimal?)null : Convert.ToDecimal(fila["SubTotalSinIGV"]),
                                          CostoT = (fila["CostoT"] is DBNull) ? (Decimal?)null : Convert.ToDecimal(fila["CostoT"]),
                                          Margen = (fila["Margen"] is DBNull) ? (Decimal?)null : Convert.ToDecimal(fila["Margen"])
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
