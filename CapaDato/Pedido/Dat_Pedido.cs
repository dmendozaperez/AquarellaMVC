﻿using System;
using CapaEntidad.Util;
using CapaEntidad.Pedido;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;


namespace CapaDato.Pedido
{
    public class Dat_Pedido
    {

        public Ent_Pedido_Maestro Listar_Maestros_Pedido(decimal usuarioId, string usu_postPago)
        {
            DataSet dsReturn = new DataSet();
            string sqlquery = "USP_LEER_MAESTROS_PEDIDO_MVC";
            List<Ent_Combo> ListPromotor = null;
            List<Ent_Combo> ListFormaPago = null;
           
            Ent_Pedido_Maestro Maestro = new Ent_Pedido_Maestro();

            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        SqlParameter oCodUsuario = cmd.Parameters.Add("@IdUsuario", SqlDbType.Decimal);
                        oCodUsuario.Direction = ParameterDirection.Input;
                        oCodUsuario.Value = usuarioId;

                        SqlParameter oCodPost = cmd.Parameters.Add("@post", SqlDbType.Decimal);
                        oCodPost.Direction = ParameterDirection.Input;
                        oCodPost.Value = usu_postPago;

                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {

                            da.Fill(dsReturn);
                            ListFormaPago = new List<Ent_Combo>();
                            ListFormaPago = (from DataRow dr in dsReturn.Tables[0].Rows
                                        select new Ent_Combo()
                                        {
                                            codigo = dr["Con_Id"].ToString(),
                                            descripcion = dr["Con_Descripcion"].ToString(),

                                        }).ToList();

                            ListPromotor = new List<Ent_Combo>();
                            ListPromotor = (from DataRow dr in dsReturn.Tables[1].Rows
                                                select new Ent_Combo()
                                                {
                                                    codigo = dr["bas_id"].ToString(),
                                                    descripcion = dr["Nombres"].ToString(),

                                                }).ToList();                        


                        }
                    }
                }

                Maestro.combo_ListPromotor = ListPromotor;
                Maestro.combo_ListFormaPago = ListFormaPago;
               

            }
            catch (Exception exc)
            {

                Maestro = null;
            }
            return Maestro;
        }

        public Ent_Pedido_Persona BuscarPersonaPedido(int basId)
        {
            DataSet dsReturn = new DataSet();
            string sqlquery = "USP_LEER_PERSONA_USUARIO_MVC";       

            Ent_Pedido_Maestro Maestro = new Ent_Pedido_Maestro();
            List<Ent_Pedido_Persona> ListPersona = null;
            Ent_Pedido_Persona entPersona = null;

            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {

                        SqlParameter oDocumento = cmd.Parameters.Add("@bas_id", SqlDbType.VarChar);
                        oDocumento.Direction = ParameterDirection.Input;
                        oDocumento.Value = basId;

                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {

                            da.Fill(dsReturn);
                            ListPersona = new List<Ent_Pedido_Persona>();
                            ListPersona = (from DataRow dr in dsReturn.Tables[0].Rows
                                             select new Ent_Pedido_Persona()
                                             {
                                                 comision = Convert.ToDecimal(dr["Con_Fig_PorcDesc"]),
                                                 idCust = Convert.ToDecimal(dr["bas_id"]),
                                                 taxRate = Convert.ToDecimal(dr["Con_Fig_Igv"]),
                                                 commission_POS_visaUnica = Convert.ToDecimal(dr["Con_Fig_PorcDescPos"]),
                                                 percepcion = Convert.ToDecimal(dr["Con_Fig_Percepcion"]),
                                                 email = dr["bas_correo"].ToString(),
                                                 nombrecompleto = dr["nombrecompleto"].ToString(),
                                                 premio = dr["Premio"].ToString(),
                                                 aplica_percepcion = Convert.ToBoolean(dr["aplica_percepcion"].ToString()),

                                             }).ToList();
                        }
                    }
                }

                entPersona = ListPersona.ElementAt(0);

            }
            catch (Exception exc)
            {

                entPersona = null;
            }
            return entPersona;
        }

        public string strBuscarPersonaPedido(int basId)
        {
            string strJson = "";
            try
            {
                SqlConnection cn = new SqlConnection(Ent_Conexion.conexion);
                cn.Open();
                SqlCommand oComando = new SqlCommand("USP_LEER_PERSONA_USUARIO_MVC", cn);
                oComando.CommandType = CommandType.StoredProcedure;

                SqlParameter oDocumento = oComando.Parameters.Add("@bas_id", SqlDbType.VarChar);
                oDocumento.Direction = ParameterDirection.Input;
                oDocumento.Value = basId;

                SqlDataReader oReader = oComando.ExecuteReader(CommandBehavior.SingleResult);
                DataTable dataTable = new DataTable("row");
                dataTable.Load(oReader);

                strJson = JsonConvert.SerializeObject(dataTable, Newtonsoft.Json.Formatting.Indented);
                strJson = strJson.Replace(Environment.NewLine, "");
                //strJson = strJson.Replace(" ", "");
                cn.Close();
            }
            catch (Exception ex)
            {
                return strJson;
            }
            //return oLista;
            return strJson;
        }


        public string listarStr_ArticuloTalla(string CodArticulo, decimal BasId)
        {
            string strJson = "";
            try
            {
                SqlConnection cn = new SqlConnection(Ent_Conexion.conexion);
                cn.Open();
                SqlCommand oComando = new SqlCommand("USP_Leer_Articulo_MVC", cn);
                oComando.CommandType = CommandType.StoredProcedure;

                SqlParameter oArticulo = oComando.Parameters.Add("@Art_Id", SqlDbType.VarChar);
                oArticulo.Direction = ParameterDirection.Input;
                oArticulo.Value = CodArticulo;


                SqlParameter oBasId = oComando.Parameters.Add("@bas_Id", SqlDbType.Int);
                oBasId.Direction = ParameterDirection.Input;
                oBasId.Value = BasId;

                SqlDataReader oReader = oComando.ExecuteReader(CommandBehavior.SingleResult);
                DataTable dataTable = new DataTable("row");
                dataTable.Load(oReader);

                strJson = JsonConvert.SerializeObject(dataTable, Newtonsoft.Json.Formatting.Indented);
                strJson = strJson.Replace(Environment.NewLine, "");
      
                cn.Close();
            }
            catch (Exception ex)
            {

                return strJson;
            }
                  
            return strJson;
        }


    }
}
