using CapaEntidad.Facturacion;
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
    }
}
