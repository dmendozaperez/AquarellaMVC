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
    }
}
