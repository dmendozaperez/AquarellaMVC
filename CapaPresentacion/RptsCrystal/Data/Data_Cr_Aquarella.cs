using CapaEntidad.Util;
using CapaPresentacion.Models.Crystal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CapaPresentacion.Data.RptsCrystal
{
    public class Data_Cr_Aquarella
    {
        public DataSet data_reporte_liquidacion(string liq)
        {
            string sqlquery = "USP_Leer_Liquidacion_Reporte";
            DataSet ds = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                        {
                            cmd.CommandTimeout = 0;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@liq_id", liq);
                            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                            {
                                ds = new DataSet();
                                da.Fill(ds);
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            catch
            {

            }
            return ds;
        }
        public  DataSet getInvoiceHdr(string inv)
        {
            string sqlquery = "USP_Leer_Venta_Reporte";
            DataSet ds = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                        {
                            cmd.CommandTimeout = 0;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@Ven_Id", inv);
                            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                            {
                                ds = new DataSet();
                                da.Fill(ds);
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            catch
            {

            }
            return ds;
            
        }
    }
}