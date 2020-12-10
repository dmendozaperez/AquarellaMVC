using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad.Financiera;
using CapaEntidad.Util;

namespace CapaDato.Financiera
{
    public class Dat_Pago
    {
        public bool ValOperacion(Ent_Pago ent)
        {
            string sqlquery = "USP_Existe_OP";
            bool Resul = false;
            SqlConnection cn = null;
            SqlCommand cmd = null;
            Int32 _valor;
            try
            {
                cn = new SqlConnection(Ent_Conexion.conexion);
                if (cn.State == 0) cn.Open();
                cmd = new SqlCommand(sqlquery, cn);
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ban_id", DbType.String).Value = ent.Pag_BanId;
                cmd.Parameters.AddWithValue("@cli_id", DbType.Int32).Value = ent.Pag_BasId;
                cmd.Parameters.AddWithValue("@monto", DbType.Double).Value = ent.Pag_Monto;
                cmd.Parameters.AddWithValue("@fecha", DbType.String).Value = ent.Pag_Num_ConsFecha;
                cmd.Parameters.AddWithValue("@n_op", DbType.String).Value = ent.Pag_Num_Consignacion;
                cmd.Parameters.AddWithValue("@existe", SqlDbType.Int).Direction = ParameterDirection.Output;
                
                cmd.ExecuteNonQuery();
                _valor = Convert.ToInt32(cmd.Parameters["@existe"].Value);
                if (_valor == 0)
                {
                    Resul =false;
                }
                else
                {
                    Resul= true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return Resul;
        }

        public bool GrabarPagos(Ent_Pago ent)
        {
            string sqlquery = "USP_Insertar_Pago";
            bool Resul = false;
            SqlConnection cn = null;
            SqlCommand cmd = null;
            Int32 _valor;
            try
            {
                cn = new SqlConnection(Ent_Conexion.conexion);
                if (cn.State == 0) cn.Open();
                cmd = new SqlCommand(sqlquery, cn);
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.StoredProcedure; 
                cmd.Parameters.AddWithValue("@bas_id", DbType.Double).Value = ent.Pag_BasId;
                cmd.Parameters.AddWithValue("@pag_banid", DbType.String).Value = ent.Pag_BanId;
                cmd.Parameters.AddWithValue("@pag_num_consignacion", DbType.String).Value = ent.Pag_Num_Consignacion;
                cmd.Parameters.AddWithValue("@pag_fecha", DbType.String).Value = ent.Pag_Num_ConsFecha;
                cmd.Parameters.AddWithValue("@Total", DbType.Double).Value = ent.Pag_Monto;
                cmd.Parameters.AddWithValue("@Con_Id", DbType.String).Value = ent.Pag_ConId;
                cmd.Parameters.AddWithValue("@pag_comentario", DbType.String).Value = ent.Pag_Comentario;
                cmd.Parameters.AddWithValue("@pag_usu_creacion", DbType.String).Value = ent.Pag_Usu_Creacion;
                cmd.Parameters.AddWithValue("@pedido", DbType.String).Value = ent.Pag_Pedido;
                
                cmd.ExecuteNonQuery();
                Resul = true;
            }
            catch 
            {
                Resul = false;
            }
            return Resul;
        }

        public Ent_Pago ValPagoTransaccionInt(Ent_Pago ent)
        {
            Ent_Pago _EntResul = new Ent_Pago();
            string sqlquery = "[USP_MVC_VAL_PAGO_TRANSANCION]";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Pag_Id", DbType.String).Value = ent.Pag_Id;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            var results = from Row in dt.AsEnumerable()
                                          select new
                                          {
                                              RetVal = Row.Field<int>("RetVal"),
                                              Existe = Row.Field<int>("Existe")
                                          };
                            
                            foreach(var item in results)
                            {
                                _EntResul.RetVal = item.RetVal;
                                _EntResul.Existe = item.Existe;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return _EntResul;
        }

        public bool EliminarPago(Ent_Pago ent)
        {
            bool result = false;
            string sqlquery = "USP_MVC_ELIMINAR_PAGO";
            SqlConnection cn = null;
            SqlCommand cmd = null;
            try
            {
                cn = new SqlConnection(Ent_Conexion.conexion);
                if (cn.State == 0) cn.Open();
                cmd = new SqlCommand(sqlquery, cn);
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Pag_Id", DbType.Double).Value = ent.Pag_Id;
                cmd.ExecuteNonQuery();
                result = true;
            }
            catch (Exception ex)
            {                
                result = false;
            }
            return result;
        }

        public List<Ent_Listar_Verificar_Pagos> Listar_Verificar_Pagos(Ent_Listar_Verificar_Pagos ent)
        {
            List<Ent_Listar_Verificar_Pagos> Listar = new List<Ent_Listar_Verificar_Pagos>();
            string sqlquery = "[USP_Leer_PagosYEstado]";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@est_id", DbType.String).Value = ent.Est_Id;
                        cmd.Parameters.AddWithValue("@are_id", DbType.String).Value = ent.Are_Id;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            Listar = new List<Ent_Listar_Verificar_Pagos>();
                            Listar = (from DataRow dr in dt.Rows
                                      select new Ent_Listar_Verificar_Pagos()
                                      {
                                          Pag_Id = Convert.ToString(dr["Pag_Id"]),
                                          Lider = Convert.ToString(dr["Lider"]),
                                          Bas_Documento = Convert.ToString(dr["Bas_Documento"]),
                                          Promotor = Convert.ToString(dr["Promotor"]),
                                          Ban_Descripcion = Convert.ToString(dr["Ban_Descripcion"]),
                                          Pag_Num_Consignacion = Convert.ToString(dr["Pag_Num_Consignacion"]),
                                          Con_Descripcion = Convert.ToString(dr["Con_Descripcion"]),
                                          Pag_Num_ConsFecha = Convert.ToString(dr["Pag_Num_ConsFecha"]),
                                          Pag_Monto = Convert.ToDecimal(dr["Pag_Monto"]),
                                          Est_Id = Convert.ToString(dr["Est_Id"]),
                                          Con_Id = Convert.ToString(dr["Con_Id"])
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
        public bool Actualizar_Estado_Pago(Ent_Pago ent)
        {
            bool result = false;
            string sqlquery = "USP_Modificar_Pago_Estado";
            SqlConnection cn = null;
            SqlCommand cmd = null;
            try
            {
                cn = new SqlConnection(Ent_Conexion.conexion);
                if (cn.State == 0) cn.Open();
                cmd = new SqlCommand(sqlquery, cn);
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@pag_Id", DbType.String).Value = ent.Pag_Id;
                cmd.Parameters.AddWithValue("@est_id ", DbType.String).Value = ent.Pag_EstId;
                cmd.ExecuteNonQuery();
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }
    }
}
