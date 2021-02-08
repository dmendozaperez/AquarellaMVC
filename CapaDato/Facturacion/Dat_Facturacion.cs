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
    }
}
