using CapaEntidad.Financiera;
using CapaEntidad.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDato.Financiera
{
    public class Dat_Financiera
    {
        public  string setCrearLiquidacionPremio(int basId, int premioId, string TipoPremio = "C")
        {
            //return "";
            string strLiqui = string.Empty;
            string sqlquery = "USP_Generar_LiquidacionPremio";
            SqlConnection cn = null;
            SqlCommand cmd = null;
            try
            {
                cn = new SqlConnection(Ent_Conexion.conexion);
                if (cn.State == 0) cn.Open();
                cmd = new SqlCommand(sqlquery, cn);
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@bas_id", basId);
                //cmd.Parameters.AddWithValue("@gru_id_devolver", DbType.String);
                cmd.Parameters.AddWithValue("@tipoRegalo", premioId);
                cmd.Parameters.AddWithValue("@tipoPremio", TipoPremio);
                //cmd.Parameters["@gru_id_devolver"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@gru_id_devolver", SqlDbType.VarChar, 20).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();

                strLiqui = Convert.ToString(cmd.Parameters["@gru_id_devolver"].Value);
                return strLiqui;

                //Database db = DatabaseFactory.CreateDatabase(_conn);
                ////                
                //string sqlCommand = "financiera.sp_pre_clear";
                ////
                //DbCommand dbCommandWrapper = db.GetStoredProcCommand(sqlCommand, _company, _list_liquidations, _list_documentrans, clearId);
                ////
                //db.ExecuteNonQuery(dbCommandWrapper);
                //clearId = db.GetParameterValue(dbCommandWrapper, "p_clv_clear_id").ToString();

                //return clearId;
            }
            catch (Exception e) { throw new Exception(e.Message, e.InnerException); }
        }
        public string setPreClear(string _list_liquidations, DataTable dt,ref string str_mensaje)
        {
            //return "";
            string clearId = string.Empty;
            string sqlquery = "USP_Pre_Grupo";
            SqlConnection cn = null;
            SqlCommand cmd = null;
            try
            {
                cn = new SqlConnection(Ent_Conexion.conexion);
                if (cn.State == 0) cn.Open();
                cmd = new SqlCommand(sqlquery, cn);
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@liq_id", _list_liquidations);
                //cmd.Parameters.AddWithValue("@gru_id_devolver", DbType.String);
                cmd.Parameters.AddWithValue("@Tmp_Pago", dt);
                //cmd.Parameters["@gru_id_devolver"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@gru_id_devolver", SqlDbType.VarChar, 80).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@str_mensaje", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;


                cmd.ExecuteNonQuery();

                clearId = Convert.ToString(cmd.Parameters["@gru_id_devolver"].Value);
                str_mensaje= Convert.ToString(cmd.Parameters["@str_mensaje"].Value);
                return clearId;

                //Database db = DatabaseFactory.CreateDatabase(_conn);
                ////                
                //string sqlCommand = "financiera.sp_pre_clear";
                ////
                //DbCommand dbCommandWrapper = db.GetStoredProcCommand(sqlCommand, _company, _list_liquidations, _list_documentrans, clearId);
                ////
                //db.ExecuteNonQuery(dbCommandWrapper);
                //clearId = db.GetParameterValue(dbCommandWrapper, "p_clv_clear_id").ToString();

                //return clearId;
            }
            catch (Exception e) { throw new Exception(e.Message, e.InnerException); }
        }
        /*Ajustar*/
        public string setvalidaclear(string _list_liquidations, ref string _ncredito, ref string _fecharef)
        {
            String sqlquery = "USP_Valida_Finanzas_PagoNc";
            SqlConnection cn = null;
            SqlCommand cmd = null;
            try
            {
                cn = new SqlConnection(Ent_Conexion.conexion);
                if (cn.State == 0) cn.Open();
                cmd = new SqlCommand(sqlquery, cn);
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@liq_id", _list_liquidations);
                cmd.Parameters.Add("@ncredito", SqlDbType.VarChar, 20);
                cmd.Parameters.Add("@fecha_ref", SqlDbType.VarChar, 20);
                cmd.Parameters["@ncredito"].Direction = ParameterDirection.Output;
                cmd.Parameters["@fecha_ref"].Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();

                _ncredito = cmd.Parameters["@ncredito"].Value.ToString();
                _fecharef = cmd.Parameters["@fecha_ref"].Value.ToString();

                return _ncredito;
            }
            catch (Exception e) { throw new Exception(e.Message, e.InnerException); }
        }
        public List<Ent_Pag_Liq> getPagsLiqs (string custId)
        {
            List<Ent_Pag_Liq> data = null;
            string sqlquery = "USP_Leer_PagoLiqXPersona";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    if (cn.State == 0) cn.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        DataTable dt = new DataTable();
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@bas_id", custId);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);
                        data = new List<Ent_Pag_Liq>();
                        data = (from DataRow dr in dt.Rows
                                select new Ent_Pag_Liq()
                                {
                                    dtv_transdoc_id         = Convert.ToString(dr["dtv_transdoc_id"]),    
                                    dtv_concept_id         = Convert.ToString(dr["dtv_concept_id"]),
                                    cov_description       = Convert.ToString(dr["cov_description"]),
                                    document_date_desc     = Convert.ToString(dr["document_date_desc"]),
                                    dtd_document_date      = Convert.ToDateTime(dr["dtd_document_date"]),
                                    debito                 = (dr["debito"] == DBNull.Value) ? 0 : Convert.ToDecimal(dr["debito"]),
                                    credito                = (dr["credito"] == DBNull.Value) ? 0 : Convert.ToDecimal(dr["credito"]),
                                    val                    = Convert.ToDecimal(dr["val"]),
                                    TIPO                   = Convert.ToString(dr["TIPO"]),
                                    active                 = Convert.ToBoolean(dr["active"]),
                                    checks                 = Convert.ToBoolean(dr["checks"]),
                                    von_increase           = Convert.ToDecimal(dr["von_increase"]),
                                    Flag                   =  (dr["Flag"] == DBNull.Value) ? 0 : Convert.ToDecimal(dr["Flag"])
                                }).ToList();
                            }
                }
            }
            catch (Exception ex)
            {
                data = null;
            }
            return data;
        }

        public List<Ent_Listar_Cliente_Pagos> Listar_Cliente_Pagos(Ent_Listar_Cliente_Pagos ent)
        {
            List<Ent_Listar_Cliente_Pagos> Listar = new List<Ent_Listar_Cliente_Pagos>();
            string sqlquery = "[USP_MVC_LEER_CLIENTES_PAGOS]";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@FechaInicio", DbType.String).Value = ent.FechaInicio;
                        cmd.Parameters.AddWithValue("@FechaFin", DbType.String).Value = ent.FechaFin;
                        cmd.Parameters.AddWithValue("@Idcliente", DbType.Int32).Value = ent.IdCliente;
                        cmd.Parameters.AddWithValue("@PagNumConsignacion", DbType.String).Value = ent.NumeroConsignacion;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            Listar = new List<Ent_Listar_Cliente_Pagos>();
                            Listar = (from DataRow dr in dt.Rows
                                      select new Ent_Listar_Cliente_Pagos()
                                      {
                                        PagoId = Convert.ToInt32(dr["PagoId"]),
                                        Documento = Convert.ToString(dr["Documento"]),
                                        NombreCompleto = Convert.ToString(dr["NombreCompleto"]),
                                        PrimerNombre = Convert.ToString(dr["PrimerNombre"]),
                                        SegundoNombre = Convert.ToString(dr["SegundoNombre"]),
                                        PrimeroApellido = Convert.ToString(dr["PrimeroApellido"]),
                                        SegundoApellido = Convert.ToString(dr["SegundoApellido"]),
                                        Correo = Convert.ToString(dr["Correo"]),
                                        NumeroConsignacion = Convert.ToString(dr["NumeroConsignacion"]),
                                        FechaConsignacion = Convert.ToString(dr["FechaConsignacion"]),
                                        FechaCreacion = Convert.ToString(dr["FechaCreacion"]),
                                        Monto = Convert.ToDecimal(dr["Monto"]),
                                        Estado = Convert.ToString(dr["Estado"]),
                                        EstadoNombre = Convert.ToString(dr["EstadoNombre"])
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
        /// <summary>
        /// lista los concepos de las operaciones gratuitas 
        /// </summary>
        /// <returns></returns>
        public List<Ent_Operacion_Gratuita> Listar_ConceptoOG()
        {
            List<Ent_Operacion_Gratuita> Listar = new List<Ent_Operacion_Gratuita>();
            string sqlquery = "[USP_Leer_Lista_Concepto_Gratuito]";
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

                            Listar = new List<Ent_Operacion_Gratuita>();
                            Listar = (from DataRow dr in dt.Rows
                                      select new Ent_Operacion_Gratuita()
                                      {
                                          Codigo = Convert.ToString(dr["Con_Id"]),
                                          Descripcion = Convert.ToString(dr["Con_Descripcion"])
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
        public List<Ent_Operacion_Gratuita> Listar_Liquidacion_Gratuita(Ent_Operacion_Gratuita _Ent)
        {
            List<Ent_Operacion_Gratuita> Listar = new List<Ent_Operacion_Gratuita>();
            string sqlquery = "[USP_Leer_Liquidacion_Gratuita]";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@tipo", DbType.String).Value = _Ent.Tipo;
                        cmd.Parameters.AddWithValue("@fecha_inicio", DbType.DateTime).Value = _Ent.FechaInicio;
                        cmd.Parameters.AddWithValue("@fecha_final", DbType.DateTime).Value = _Ent.FechaFin;
                                                
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            Listar = new List<Ent_Operacion_Gratuita>();
                            Listar = (from DataRow dr in dt.Rows
                                      select new Ent_Operacion_Gratuita()
                                      {
                                          Tipo = Convert.ToString(dr["Tipo"]),
                                          Fecha = Convert.ToString(dr["Fecha"]),
                                          TipoDocumento = Convert.ToString(dr["TipoDocumento"]),
                                          NroDocumento = Convert.ToString(dr["NroDocumento"]),
                                          Doc_cliente = Convert.ToString(dr["Doc_cliente"]),
                                          Cliente = Convert.ToString(dr["Cliente"]),
                                          EstadoDescripcion = Convert.ToString(dr["EstadoDescripcion"]),
                                          SubTotal = Convert.ToDecimal(dr["SubTotal"]),
                                          IGV = Convert.ToDecimal(dr["IGV"]),
                                          Total = Convert.ToDecimal(dr["Total"])
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
        public List<Ent_Saldo_Cliente> Leer_Clientes_Saldo()
        {
            List<Ent_Saldo_Cliente> listar = null;
            string sqlquery = "USP_Leer_Lista_Clientes";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            listar = new List<Ent_Saldo_Cliente>();
                            listar = (from DataRow fila in dt.Rows
                                      select new Ent_Saldo_Cliente()
                                      {
                                          Codigo = fila["Bas_Id"].ToString(),
                                          Descripcion = fila["nombres"].ToString()
                                      }
                                   ).ToList();
                        }
                    }
                }
            }
            catch (Exception exc)
            {

                listar = new List<Ent_Saldo_Cliente>();
            }
            return listar;
        }
        /// <summary>
        /// lista los concepos de los saldos de los clientes
        /// </summary>
        /// <returns></returns>
        public List<Ent_Saldo_Cliente> Listar_Concepto_Saldo()
        {
            List<Ent_Saldo_Cliente> Listar = new List<Ent_Saldo_Cliente>();
            string sqlquery = "[USP_Leer_Lista_Concepto]";
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

                            Listar = new List<Ent_Saldo_Cliente>();
                            Listar = (from DataRow dr in dt.Rows
                                      select new Ent_Saldo_Cliente()
                                      {
                                          Codigo = Convert.ToString(dr["Con_Id"]),
                                          Descripcion = Convert.ToString(dr["Con_Descripcion"])
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
        public List<Ent_Saldo_Cliente> Leer_Saldos_Pendientes(Ent_Saldo_Cliente _Ent)
        {
            List<Ent_Saldo_Cliente> Listar = null;
            string sqlquery = "[USP_Leer_Saldos_Pendientes]";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@BAS_ID", DbType.Int32).Value = _Ent.Bas_Id;
                        cmd.Parameters.AddWithValue("@CON_ID", DbType.String).Value = _Ent.Cod_Id;
                        cmd.Parameters.AddWithValue("@fecha_ini", DbType.DateTime).Value = _Ent.FechaInicio;
                        cmd.Parameters.AddWithValue("@fecha_fin", DbType.DateTime).Value = _Ent.FechaFin;
                        cmd.Parameters.AddWithValue("@Usu_Tipo", DbType.String).Value = _Ent.Usu_Tipo;

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            Listar = new List<Ent_Saldo_Cliente>();
                            Listar = (from DataRow fila in dt.Rows
                                      select new Ent_Saldo_Cliente()
                                      {
                                          Asesor = (fila["Asesor"] is DBNull) ? string.Empty : (string)(fila["Asesor"]),
                                          Dniruc = (fila["Dniruc"] is DBNull) ? string.Empty : (string)(fila["Dniruc"]),
                                          Lider = (fila["Lider"] is DBNull) ? string.Empty : (string)(fila["Lider"]),
                                          Cliente = (fila["Cliente"] is DBNull) ? string.Empty : (string)(fila["Cliente"]),
                                          Concepto = (fila["Concepto"] is DBNull) ? string.Empty : (string)(fila["Concepto"]),
                                          Documento = (fila["Documento"] is DBNull) ? string.Empty : (string)(fila["Documento"]),
                                          Fecha_Transac = (fila["Fecha_Transac"] is DBNull) ? (DateTime?)null : Convert.ToDateTime(fila["Fecha_Transac"]),
                                          Fecha_Doc = (fila["Fecha_Doc"] is DBNull) ? (DateTime?)null : Convert.ToDateTime(fila["Fecha_Doc"]),
                                          Monto = (fila["Monto"] is DBNull) ? (Decimal?)null : Convert.ToDecimal(fila["Monto"]),
                                          Valida = (fila["Valida"] is DBNull) ? string.Empty : (string)(fila["Valida"])
                                      }
                                    ).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Listar = new List<Ent_Saldo_Cliente>();
            }
            return Listar;
        }
    }
}
