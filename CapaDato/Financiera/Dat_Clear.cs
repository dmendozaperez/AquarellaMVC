using CapaEntidad.Control;
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
    public class Dat_Clear
    {
        public static string setvalidaclear(string _list_liquidations, ref string _ncredito, ref string _fecharef)
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

        public static string setPreClear(Decimal _usuid, DataTable dt)
        {
            string clearId = string.Empty;
            string sqlquery = "USP_Pre_Grupo_CN";
            SqlConnection cn = null;
            SqlCommand cmd = null;
            try
            {
                cn = new SqlConnection(Ent_Conexion.conexion);
                if (cn.State == 0) cn.Open();
                cmd = new SqlCommand(sqlquery, cn);
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@usu_id", _usuid);
                cmd.Parameters.AddWithValue("@gru_id_devolver", DbType.String);
                cmd.Parameters.AddWithValue("@Tmp_Pago", dt);
                cmd.Parameters["@gru_id_devolver"].Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();

                clearId = Convert.ToString(cmd.Parameters["@gru_id_devolver"].Value);
                return clearId;
            }
            catch (Exception e) { throw new Exception(e.Message, e.InnerException); }
        }

        public static string setPreClear(string _list_liquidations, DataTable dt)
        {
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
                cmd.Parameters.AddWithValue("@Tmp_Pago", dt);
                cmd.Parameters.Add("@gru_id_devolver", SqlDbType.VarChar, 80).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();

                clearId = Convert.ToString(cmd.Parameters["@gru_id_devolver"].Value);
                return clearId;
            }
            catch (Exception e) { throw new Exception(e.Message, e.InnerException); }
        }

        public static string setCrearLiquidacionPremio(int basId, int premioId, string TipoPremio = "C")
        {
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
                cmd.Parameters.AddWithValue("@tipoRegalo", premioId);
                cmd.Parameters.AddWithValue("@tipoPremio", TipoPremio);
                cmd.Parameters.Add("@gru_id_devolver", SqlDbType.VarChar, 20).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();

                strLiqui = Convert.ToString(cmd.Parameters["@gru_id_devolver"].Value);
                return strLiqui;
            }
            catch (Exception e) { throw new Exception(e.Message, e.InnerException); }
        }

        public static string setClearingDoc(string _company, string _list_documentrans)
        {
            // ???
            return "";
        }

    }
}
