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
                        using (SqlDataReader _Leer = cmd.ExecuteReader())
                        {
                            if (_Leer.HasRows)
                            {
                                while (_Leer.Read())
                                {
                                    Ent_Lista_Cuenta_Contables Item = new Ent_Lista_Cuenta_Contables
                                    {
                                        Clear_id = (string)(_Leer["Clear_id"]),
                                        Cuenta = (_Leer["cuenta"] is DBNull) ? string.Empty : (string)(_Leer["cuenta"]),
                                        CuentaDes = (_Leer["CuentaDes"] is DBNull) ? string.Empty : (string)(_Leer["CuentaDes"]),
                                        TipoEntidad = (_Leer["TipoEntidad"] is DBNull) ? string.Empty : (string)(_Leer["TipoEntidad"]),
                                        CodigoEntidad = (_Leer["CodigoEntidad"] is DBNull) ? string.Empty : (string)(_Leer["CodigoEntidad"]),
                                        DesEntidad = (_Leer["DesEntidad"] is DBNull) ? string.Empty : (string)(_Leer["DesEntidad"]),
                                        Tipo = (_Leer["Tipo"] is DBNull) ? string.Empty : (string)(_Leer["Tipo"]),
                                        Serie = (_Leer["Serie"] is DBNull) ? string.Empty : (string)(_Leer["Serie"]),
                                        Numero = (_Leer["Numero"] is DBNull) ? string.Empty : (string)(_Leer["Numero"]),
                                        Fecha = (_Leer["Fecha"] is DBNull) ? (DateTime?)null : Convert.ToDateTime(_Leer["Fecha"]),
                                        Debe = (_Leer["Debe"] is DBNull) ? (double?)null : Convert.ToDouble(_Leer["Debe"]),
                                        Haber = (_Leer["Haber"] is DBNull) ? (double?)null : Convert.ToDouble(_Leer["Haber"]),
                                        devito = (_Leer["devito"] is DBNull) ? string.Empty : (string)(_Leer["devito"]),
                                        Amount = (_Leer["Amount"] is DBNull) ? (double?)null : Convert.ToDouble(_Leer["Amount"]),
                                        Concepto = (_Leer["Concepto"] is DBNull) ? string.Empty : (string)(_Leer["Concepto"]),
                                        Ad_Co = (_Leer["Ad_Co"] is DBNull) ? (int?)null : Convert.ToInt32(_Leer["Ad_Co"]),
                                        Pad_Pay_Date = (_Leer["Pad_Pay_Date"] is DBNull) ? (DateTime?)null : Convert.ToDateTime(_Leer["Pad_Pay_Date"]),
                                        Contador = (_Leer["Contador"] is DBNull) ? (int?)null : Convert.ToInt32(_Leer["Contador"])
                                    };
                                    Listar.Add(Item);
                                }
                            }
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
