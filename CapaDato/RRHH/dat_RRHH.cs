using CapaEntidad.Util;
using CapaEntidad.RRHH;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace CapaDato.RRHH
{
    public class Dat_RRHH
    {
        public List<Ent_Promotor_Lider> ListarPromotorLider(Ent_Promotor_Lider _Ent)
        {
            List<Ent_Promotor_Lider> Listar = new List<Ent_Promotor_Lider>();
            string sqlquery = "[USP_MVC_BuscarPromotorXLider]";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@bas_Id", DbType.String).Value = _Ent.Bas_Id;
                        cmd.Parameters.AddWithValue("@fecha_ini", DbType.DateTime).Value = _Ent.FechaInicio;
                        cmd.Parameters.AddWithValue("@fecha_fin", DbType.DateTime).Value = _Ent.FechaFin;
                        cmd.Parameters.AddWithValue("@asesor", DbType.String).Value = _Ent.Asesor;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            Listar = new List<Ent_Promotor_Lider>();
                            Listar = (from DataRow fila in dt.Rows
                                      select new Ent_Promotor_Lider()
                                      {
                                          Asesor = (fila["Asesor"] is DBNull) ? string.Empty : (string)(fila["Asesor"]),
                                          Lider = (fila["Lider"] is DBNull) ? string.Empty : (string)(fila["Lider"]),
                                          Promotor = (fila["Promotor"] is DBNull) ? string.Empty : (string)(fila["Promotor"]),
                                          Documento = (fila["Documento"] is DBNull) ? string.Empty : (string)(fila["Documento"]),
                                          Departamento = (fila["Departamento"] is DBNull) ? string.Empty : (string)(fila["Departamento"]),
                                          Provincia = (fila["Provincia"] is DBNull) ? string.Empty : (string)(fila["Provincia"]),
                                          Distrito = (fila["Distrito"] is DBNull) ? string.Empty : (string)(fila["Distrito"]),
                                          Direccion = (fila["Direccion"] is DBNull) ? string.Empty : (string)(fila["Direccion"]),
                                          Telefono = (fila["Telefono"] is DBNull) ? string.Empty : (string)(fila["Telefono"]),
                                          Correo = (fila["Correo"] is DBNull) ? string.Empty : (string)(fila["Correo"]),
                                          Celular = (fila["Celular"] is DBNull) ? string.Empty : (string)(fila["Celular"]),
                                          Fecing = (fila["Fecing"] is DBNull) ? string.Empty : (string)(fila["Fecing"]),
                                          Fecactv = (fila["Fecactv"] is DBNull) ? (DateTime?)null : Convert.ToDateTime(fila["Fecactv"]),
                                          Fec_Nac = (fila["Fec_Nac"] is DBNull) ? string.Empty : (string)(fila["Fec_Nac"]),
                                          Zona = (fila["Zona"] is DBNull) ? string.Empty : (string)(fila["Zona"]),
                                          Activo = (fila["Activo"] is DBNull) ? string.Empty : (string)(fila["Activo"])
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

        public List<Ent_KPI_Asesor> ListarAsesor()
        {
            List<Ent_KPI_Asesor> Listar = new List<Ent_KPI_Asesor>();
            string sqlquery = "[USP_Leer_AsesorComercial]";
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

                            Listar = new List<Ent_KPI_Asesor>();
                            Listar = (from DataRow fila in dt.Rows
                                      select new Ent_KPI_Asesor()
                                      {
                                          Codigo = (fila["Bas_Aco_Id"] is DBNull) ? string.Empty : (string)(fila["Bas_Aco_Id"]),
                                          Descripcion = (fila["Nombres"] is DBNull) ? string.Empty : (string)(fila["Nombres"])                                          
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
        public List<Ent_KPI_Lider> ListarLider(Ent_KPI_Lider _Ent)
        {
            List<Ent_KPI_Lider> Listar = new List<Ent_KPI_Lider>();
            string sqlquery = "[USP_Leer_Area]";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@asesor", DbType.String).Value = _Ent.IdAsesor;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            Listar = new List<Ent_KPI_Lider>();
                            Listar = (from DataRow fila in dt.Rows
                                      select new Ent_KPI_Lider()
                                      {
                                          IdAsesor = (fila["Bas_Aco_Id"] is DBNull) ? string.Empty : (string)(fila["Bas_Aco_Id"]),
                                          Codigo = (fila["Are_Id"] is DBNull) ? string.Empty : (string)(fila["Are_Id"]),
                                          Descripcion = (fila["Are_Descripcion"] is DBNull) ? string.Empty : (string)(fila["Are_Descripcion"])
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

        public List<Ent_ConsultaKPI_Detalle> ListarConsultaKPI_Detalle(Ent_ConsultaKPI_Detalle _Ent)
        {
            List<Ent_ConsultaKPI_Detalle> Listar ;
            string sqlquery = "[USP_MVC_ConsultaKPI_Detallado]";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@asesor", DbType.String).Value = _Ent.IdAsesor;
                        cmd.Parameters.AddWithValue("@are_id", DbType.String).Value = _Ent.IdLider;
                        cmd.Parameters.AddWithValue("@fechaini", DbType.DateTime).Value = _Ent.FechaInicio;
                        cmd.Parameters.AddWithValue("@fechafin", DbType.DateTime).Value = _Ent.FechaFin;
                        cmd.Parameters.AddWithValue("@ase_lid", DbType.Boolean).Value = _Ent.IsAseOrLider;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            Listar = new List<Ent_ConsultaKPI_Detalle>();
                            Listar = (from DataRow fila in dt.Rows
                                      select new Ent_ConsultaKPI_Detalle()
                                      {
                                          Lider = (fila["Lider"] is DBNull) ? string.Empty : (string)(fila["Lider"]),
                                          Asesor = (fila["Asesor"] is DBNull) ? string.Empty : (string)(fila["Asesor"]),
                                          Anio = (fila["Anio"] is DBNull) ? (int?)null : Convert.ToInt32(fila["Anio"]),
                                          Mes = (fila["Mes"] is DBNull) ? string.Empty : (string)(fila["Mes"]),
                                          Facturacion = (fila["Facturacion"] is DBNull) ? (Decimal?)null : Convert.ToDecimal(fila["Facturacion"]),
                                          Margen = (fila["margen"] is DBNull) ? (Decimal?)null : Convert.ToDecimal(fila["margen"]),
                                          Continuas = (fila["Continuas"] is DBNull) ? (int?)null : Convert.ToInt32(fila["Continuas"]),
                                          Afiliadas = (fila["Afiliadas"] is DBNull) ? (int?)null : Convert.ToInt32(fila["Afiliadas"]),
                                          Reactivadas = (fila["Reactivadas"] is DBNull) ? (int?)null : Convert.ToInt32(fila["Reactivadas"]),
                                          Activasenmes = (fila["Activasenxmes"] is DBNull) ? (int?)null : Convert.ToInt32(fila["Activasenxmes"]),
                                          Desactivadas = (fila["Desactivadas"] is DBNull) ? (int?)null : Convert.ToInt32(fila["Desactivadas"]),
                                          PorDesact = (fila["% Desact"] is DBNull) ? (Decimal?)null : Convert.ToDecimal(fila["% Desact"]),
                                          Reg_Mes = (fila["Reg_Mes"] is DBNull) ?  (Decimal?)null : Convert.ToDecimal(fila["Reg_Mes"]),
                                          TactRegMes = (fila["TactRegMes"] is DBNull) ? (Decimal?)null : Convert.ToDecimal(fila["TactRegMes"]),
                                          PorAfiliadasMes = (fila["% Afiliadas Mes"] is DBNull) ?  (Decimal?)null : Convert.ToDecimal(fila["% Afiliadas Mes"]),
                                          ActivasOtroMes = (fila["Activas De Otro Mes"] is DBNull) ? (int?)null : Convert.ToInt32(fila["Activas De Otro Mes"]),
                                          TotalActivas = (fila["Total Activas"] is DBNull) ? (int?)null : Convert.ToInt32(fila["Total Activas"]),
                                          TicketProm = (fila["Ticket Prom"] is DBNull) ? (Decimal?)null : Convert.ToDecimal(fila["Ticket Prom"])
                                      }
                                    ).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Listar = new List<Ent_ConsultaKPI_Detalle>();
            }
            return Listar;
        }
        public DataTable ListarConsultaKPI(Ent_ConsultaKPI _Ent)
        {
            string sqlquery = "USP_MVC_ConsultaKPI";
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
                cmd.Parameters.AddWithValue("@asesor", DbType.String).Value = _Ent.IdAsesor;
                cmd.Parameters.AddWithValue("@are_id", DbType.String).Value = _Ent.IdLider;
                cmd.Parameters.AddWithValue("@fechaini", DbType.DateTime).Value = _Ent.FechaInicio;
                cmd.Parameters.AddWithValue("@fechafin", DbType.DateTime).Value = _Ent.FechaFin;
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
    }
}
