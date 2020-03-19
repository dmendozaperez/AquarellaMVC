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
                                    document_date_desc     = Convert.ToDateTime(dr["document_date_desc"]),
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
