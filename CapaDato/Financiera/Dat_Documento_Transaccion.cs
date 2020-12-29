using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad.Financiera;
using CapaEntidad.Util;

namespace CapaDato.Financiera
{
    public class Dat_Documento_Transaccion
    {
        public List<Ent_Lista_Cuenta_Contables> Listar_Asientos_Adonis(Ent_Lista_Cuenta_Contables ent)
        {
            List<Ent_Lista_Cuenta_Contables> Listar = new List<Ent_Lista_Cuenta_Contables>();
            string sqlquery = "[USP_MVC_Leer_Asientos_Adonis]";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@var_fechaini", DbType.DateTime).Value = ent.FechaInicio;
                        cmd.Parameters.AddWithValue("@var_fechafin", DbType.DateTime).Value = ent.FechaFin;
                        cmd.Parameters.AddWithValue("@var_cliente", DbType.Int16).Value = ent.IdCliente;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            Listar = new List<Ent_Lista_Cuenta_Contables>();
                            Listar = (from DataRow fila in dt.Rows
                                      select new Ent_Lista_Cuenta_Contables()
                                      {
                                          Clear_id = fila["Clear_id"].ToString(),
                                          Cuenta = (fila["cuenta"] is DBNull) ? string.Empty : (string)(fila["cuenta"]),
                                          CuentaDes = (fila["CuentaDes"] is DBNull) ? string.Empty : (string)(fila["CuentaDes"]),
                                          TipoEntidad = (fila["TipoEntidad"] is DBNull) ? string.Empty : (string)(fila["TipoEntidad"]),
                                          CodigoEntidad = (fila["CodigoEntidad"] is DBNull) ? string.Empty : (string)(fila["CodigoEntidad"]),
                                          DesEntidad = (fila["DesEntidad"] is DBNull) ? string.Empty : (string)(fila["DesEntidad"]),
                                          Tipo = (fila["Tipo"] is DBNull) ? string.Empty : (string)(fila["Tipo"]),
                                          Serie = (fila["Serie"] is DBNull) ? string.Empty : (string)(fila["Serie"]),
                                          Numero = (fila["Numero"] is DBNull) ? string.Empty : (string)(fila["Numero"]),
                                          Fecha = (fila["Fecha"] is DBNull) ? (DateTime?)null : Convert.ToDateTime(fila["Fecha"]),
                                          Debe = (fila["Debe"] is DBNull) ? (Decimal?)null : Convert.ToDecimal(fila["Debe"]),
                                          Haber = (fila["Haber"] is DBNull) ? (Decimal?)null : Convert.ToDecimal(fila["Haber"]),
                                          devito = (fila["devito"] is DBNull) ? string.Empty : (string)(fila["devito"]),
                                          Amount = (fila["Amount"] is DBNull) ? (double?)null : Convert.ToDouble(fila["Amount"]),
                                          Concepto = (fila["Concepto"] is DBNull) ? string.Empty : (string)(fila["Concepto"]),
                                          Ad_Co = (fila["Ad_Co"] is DBNull) ? (int?)null : Convert.ToInt32(fila["Ad_Co"]),
                                          Pad_Pay_Date = (fila["Pad_Pay_Date"] is DBNull) ? (DateTime?)null : Convert.ToDateTime(fila["Pad_Pay_Date"]),
                                          Contador = (fila["Contador"] is DBNull) ? 0 : Convert.ToInt32(fila["Contador"])
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

        public List<Ent_Listar_Cliente_Banco> Listar_Cliente_Banco(Ent_Listar_Cliente_Banco ent)
        {
            List<Ent_Listar_Cliente_Banco> Listar = new List<Ent_Listar_Cliente_Banco>();
            string sqlquery = "[USP_MVC_Exportar_Clientes_Por_Banco]";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Ban_Id", DbType.String).Value = ent.Ban_Id;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            Listar = new List<Ent_Listar_Cliente_Banco>();
                            Listar = (from DataRow fila in dt.Rows
                                      select new Ent_Listar_Cliente_Banco()
                                      {
                                          Campo = (fila["Campo"] is DBNull) ? string.Empty : (string)(fila["Campo"])
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

        public List<Ent_Venta_Semanal> Listar_Venta_Semanal(Ent_Venta_Semanal ent)
        {
            List<Ent_Venta_Semanal> Listar = new List<Ent_Venta_Semanal>();
            string sqlquery = "[USP_Leer_VentaFinanzas]";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@var_fechaini", DbType.DateTime).Value = ent.FechaInicio;
                        cmd.Parameters.AddWithValue("@var_fechafin", DbType.DateTime).Value = ent.FechaFin;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            Listar = new List<Ent_Venta_Semanal>();
                            Listar = (from DataRow fila in dt.Rows
                                      select new Ent_Venta_Semanal()
                                      {
                                          Dtv_Clear = (fila["Dtv_Clear"] is DBNull) ? string.Empty : (string)(fila["Dtv_Clear"]),
                                          Promotor = (fila["Promotor"] is DBNull) ? string.Empty : (string)(fila["Promotor"]),
                                          DniRuc = (fila["DniRuc"] is DBNull) ? string.Empty : (string)(fila["DniRuc"]),
                                          Ped = (fila["Ped"] is DBNull) ? string.Empty : (string)(fila["Ped"]),
                                          BolFact = (fila["BolFact"] is DBNull) ? string.Empty : (string)(fila["BolFact"]),
                                          FechaDoc = (fila["FechaDoc"] is DBNull) ? string.Empty : (string)(fila["FechaDoc"]),
                                          MontoFac = (fila["MontoFac"] is DBNull) ? (Decimal?)null : Convert.ToDecimal(fila["MontoFac"]),
                                          NroVouBcp = (fila["NroVouBcp"] is DBNull) ? string.Empty : (string)(fila["NroVouBcp"]),
                                          FechavouBcp = (fila["FechavouBcp"] is DBNull) ? string.Empty : (string)(fila["FechavouBcp"]),
                                          MontoVouBcp = (fila["MontoVouBcp"] is DBNull) ? (Decimal?)null : Convert.ToDecimal(fila["MontoVouBcp"]),
                                          NroVisa = (fila["NroVisa"] is DBNull) ? string.Empty : (string)(fila["NroVisa"]),
                                          FechaVisa = (fila["FechaVisa"] is DBNull) ? string.Empty : (string)(fila["FechaVisa"]),
                                          MontoVisa = (fila["MontoVisa"] is DBNull) ? (Decimal?)null : Convert.ToDecimal(fila["MontoVisa"]),
                                          Nronc = (fila["Nronc"] is DBNull) ? string.Empty : (string)(fila["Nronc"]),
                                          Fechanc = (fila["Fechanc"] is DBNull) ? string.Empty : (string)(fila["Fechanc"]),
                                          Montonc = (fila["Montonc"] is DBNull) ? (Decimal?)null : Convert.ToDecimal(fila["Montonc"]),
                                          FechaSaldoant = (fila["FechaSaldoant"] is DBNull) ? string.Empty : (string)(fila["FechaSaldoant"]),
                                          MontoSaldoant = (fila["MontoSaldoant"] is DBNull) ? (Decimal?)null : Convert.ToDecimal(fila["MontoSaldoant"]),
                                          TotalPagos = (fila["TotalPagos"] is DBNull) ? (Decimal?)null : Convert.ToDecimal(fila["TotalPagos"]),
                                          SaldoFavor = (fila["SaldoFavor"] is DBNull) ? (Decimal?)null : Convert.ToDecimal(fila["SaldoFavor"])
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


        public List<Ent_Saldos_Anticipos> Listar_Saldos_Anticipos()
        {
            List<Ent_Saldos_Anticipos> Listar = new List<Ent_Saldos_Anticipos>();
            string sqlquery = "[USP_Leer_Genera_Anticipos]";
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

                            Listar = new List<Ent_Saldos_Anticipos>();
                            Listar = (from DataRow fila in dt.Rows
                                      select new Ent_Saldos_Anticipos()
                                      {
                                          Documento = (fila["Documento"] is DBNull) ? string.Empty : (string)(fila["Documento"]),
                                          Cliente = (fila["Cliente"] is DBNull) ? string.Empty : (string)(fila["Cliente"]),
                                          Saldo = (fila["Saldo"] is DBNull) ? (Decimal?)null : (Convert.ToDecimal(fila["Saldo"])),
                                          SerieFac = (fila["SerieFac"] is DBNull) ? string.Empty : (string)(fila["SerieFac"]),
                                          NumeroFac = (fila["NumeroFac"] is DBNull) ? string.Empty : (string)(fila["NumeroFac"]),
                                          Fec_Fac = (fila["Fec_Fac"] is DBNull) ? string.Empty : (string)(fila["Fec_Fac"]),
                                          MontoFac = (fila["MontoFac"] is DBNull) ? (Decimal?)null : (Convert.ToDecimal(fila["MontoFac"])),
                                          SerieNc = (fila["SerieNc"] is DBNull) ? string.Empty : (string)(fila["SerieNc"]),
                                          NumeroNc = (fila["NumeroNc"] is DBNull) ? string.Empty : (string)(fila["NumeroNc"]),
                                          Fec_Nc = (fila["Fec_Nc"] is DBNull) ? string.Empty : (string)(fila["Fec_Nc"]),
                                          MontoNc = (fila["MontoNc"] is DBNull) ? (Decimal?)null : (Convert.ToDecimal(fila["MontoNc"])),
                                          Monto_Util = (fila["Monto_Util"] is DBNull) ? (Decimal?)null : (Convert.ToDecimal(fila["Monto_Util"])),
                                          Percepcion = (fila["Percepcion"] is DBNull) ? (Decimal?)null : (Convert.ToDecimal(fila["Percepcion"])),
                                          Chk = (fila["Chk"] is DBNull) ? (bool?)null : (Convert.ToBoolean(fila["Chk"])),
                                          Bas_Id = (fila["Bas_Id"] is DBNull) ? (int?)null : (Convert.ToInt32(fila["Bas_Id"]))
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
        public bool Genera_Provisiones(DataTable dt, Ent_Saldos_Anticipos ent)
        {
            string sqlquery = "USP_Genera_Provisiones";
            bool result = false;
            SqlConnection cn = null;
            SqlCommand cmd = null;            
            try
            {
                cn = new SqlConnection(Ent_Conexion.conexion);
                if (cn.State == 0) cn.Open();
                cmd = new SqlCommand(sqlquery, cn);
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@usu_ing", ent.usu_ingreso);
                cmd.Parameters.AddWithValue("@tmp_genera", dt);
                cmd.ExecuteNonQuery();
                 result = true;
            }
            catch (Exception exc)
            {
                result = false;
            }
            if (cn.State == ConnectionState.Open) cn.Close();
            return result;
        }

        public bool AnularSaldos(Ent_Saldos_Anticipos ent)
        {
            string sqlquery = "USP_Anular_Saldos";
            bool result = false;
            //string sqlquery = "";
            SqlConnection cn = null;
            SqlCommand cmd = null;
            try
            {
                cn = new SqlConnection(Ent_Conexion.conexion);
                if (cn.State == 0) cn.Open();
                cmd = new SqlCommand(sqlquery, cn);
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@user_ing", ent.usu_ingreso);
                cmd.Parameters.AddWithValue("@dniruc", ent.Documento);
                cmd.ExecuteNonQuery();
                result = true;
            }
            catch (Exception exc)
            {
                result = false;
            }
            if (cn.State == ConnectionState.Open) cn.Close();
            return result;
        }

        public DataTable getvalida_inter(DataTable dt)
        {
            string sqlquery = "USP_ConsultaValidaInter";
            DataTable dtinter = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@tmpinter", dt);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            dtinter = new DataTable();
                            da.Fill(dtinter);
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return dtinter;
        }

        public bool SaveValidaInter(DataTable dt, Ent_Validar_Pagos ent)
        {
            string sqlquery = "USP_Valida_Archivo_Banco_Inter";
            bool result = false;
            SqlConnection cn = null;
            SqlCommand cmd = null;
            try
            {
                cn = new SqlConnection(Ent_Conexion.conexion);
                if (cn.State == 0) cn.Open();
                cmd = new SqlCommand(sqlquery, cn);
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ban_id_b", Convert.ToString(ent.NumBanco));
                cmd.Parameters.AddWithValue("@usu_validar", ent.Usu_Validar);
                cmd.Parameters.AddWithValue("@tmpvalida", dt);
                cmd.ExecuteNonQuery();
                result = true;
            }
            catch (Exception exc)
            {
                result = false;
            }
            if (cn.State == ConnectionState.Open) cn.Close();
            return result;
        }

        public bool SaveValidateBank(DataTable dt, Ent_Validar_Pagos ent)
        {
            string sqlquery = "USP_Valida_Archivo_Banco";
            bool result = false;
            SqlConnection cn = null;
            SqlCommand cmd = null;
            try
            {
                cn = new SqlConnection(Ent_Conexion.conexion);
                if (cn.State == 0) cn.Open();
                cmd = new SqlCommand(sqlquery, cn);
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@usu_validar", ent.Usu_Validar);
                cmd.Parameters.AddWithValue("@Pago_Valida", dt);
                cmd.ExecuteNonQuery();
                result = true;
            }
            catch (Exception exc)
            {
                result = false;
            }
            if (cn.State == ConnectionState.Open) cn.Close();
            return result;
        }
    }
}
