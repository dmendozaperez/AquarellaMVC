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
    }
}
