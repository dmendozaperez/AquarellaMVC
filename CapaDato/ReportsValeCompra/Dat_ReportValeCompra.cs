using System;
using CapaEntidad.ReportsValeCompra;
using CapaEntidad.Util;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace CapaDato.ReportsValeCompra
{
    public class Dat_ReportValeCompra
    {
        public List<Reporte_Resultado> listarReporte(Reporte_Filtro filtro)  
        {
            string sqlquery = "USP_ListarValesCompra";
            List<Reporte_Resultado> lista = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {


                        SqlParameter oEstado = cmd.Parameters.Add("@Estado", SqlDbType.VarChar);
                        oEstado.Direction = ParameterDirection.Input;
                        oEstado.Value = filtro.report_Estado;

                        SqlParameter ostrRuc = cmd.Parameters.Add("@strRucCliente", SqlDbType.VarChar);
                        ostrRuc.Direction = ParameterDirection.Input;
                        ostrRuc.Value = filtro.report_listRuc;


                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            lista = new List<Reporte_Resultado>();
                            lista = (from DataRow dr in dt.Rows
                                     select new Reporte_Resultado()
                                     {

                                         Institucion = dr["Institucion"].ToString(),
                                         DNI = dr["rep_dni"].ToString(),
                                         Cliente = dr["Cliente"].ToString(),
                                       
                                         Codigo = dr["rep_CupBarra"].ToString(),
                                         Numero = dr["rep_CupNumero"].ToString(),
                                         soles = dr["rep_CupMonto"].ToString(),
                                         Estado = dr["rep_CupEstado"].ToString(),

                                        
                                         Documento = dr["rep_docSerie"].ToString() + dr["rep_docNro"].ToString(),
                                  
                                         Fecha_doc = dr["rep_docfecha"].ToString(),

                                         Codigo_tda = dr["rep_tdaCodigo"].ToString(),
                                         Desc_tda = dr["rep_tdaDesc"].ToString(),
                                         total_disponible = dr["totalDisponible"].ToString(),
                                         total_consumido = dr["totalConsumido"].ToString(),




                                     }).ToList();

                        }
                    }
                }
            }
            catch (Exception exc)
            {

                lista = null;
            }
            return lista;
        }

    }
}
