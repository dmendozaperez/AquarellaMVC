using CapaEntidad.Devolucion;
using CapaEntidad.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDato.Devolucion
{
    public class Dat_Documentos_Dev
    {
        public List<Ent_Documentos_Dev_Det_New> get_dev_det_new(string nrodoc, ref string _mensaje)
        {
            String sqlquery = "USP_GET_DEVOLUCION_PERSONA_DET";
            DataTable dt = null;
            List<Ent_Documentos_Dev_Det_New> listar = null;
            try
            {
               
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@NDOC", nrodoc);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            dt = new DataTable();
                            da.Fill(dt);
                            listar = new List<Ent_Documentos_Dev_Det_New>();
                            listar = (from DataRow fila in dt.Rows
                                         select new Ent_Documentos_Dev_Det_New()
                                         {
                                             ARTICULO = fila["ARTICULO"].ToString(),
                                             TALLA = fila["TALLA"].ToString(),
                                             CANTIDAD=Convert.ToDecimal(fila["CANTIDAD"])                                            
                                         }
                                    ).ToList();
                           
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                listar = null;
                _mensaje = exc.Message;
            }
            return listar;
        }
        public Ent_Info_Devolucion ListarDevolucion(decimal bas_id,ref string _mensaje)
        {
            String sqlquery = "USP_GET_DEVOLUCION_PERSONA";
            DataTable dt = null;
            Ent_Info_Devolucion listar = null;
            try
            {
                listar = new Ent_Info_Devolucion();
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@BAS_ID", bas_id);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            dt = new DataTable();
                            da.Fill(dt);
                            List<Ent_Documentos_Dev>  listardoc = new List<Ent_Documentos_Dev>();
                            listardoc = (from DataRow fila in dt.Rows
                                      select new Ent_Documentos_Dev()
                                      {
                                          TIPODOC =fila["TIPODOC"].ToString(),
                                          NDOC = fila["NDOC"].ToString(),
                                          FDOC = fila["FDOC"].ToString(),
                                          SUBTOTAL =Convert.ToDecimal(fila["SUBTOTAL"]),
                                          IGV =  Convert.ToDecimal(fila["IGV"]),
                                          TOTAL =  Convert.ToDecimal(fila["TOTAL"]),
                                      }
                                    ).ToList();
                            listar.documentosDev = listardoc;
                        }
                    }
                }
            }
            catch(Exception exc)
            {
                listar = null;
                _mensaje = exc.Message;
            }
            return listar;
       }
    
    }
}

