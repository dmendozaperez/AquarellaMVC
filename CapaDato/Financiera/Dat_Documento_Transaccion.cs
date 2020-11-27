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
                                          Contador = (fila["Contador"] is DBNull) ? (int?)null : Convert.ToInt32(fila["Contador"])
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
