using System;
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

        public Ent_Pedido_Maestro Listar_Maestros_Pedido(decimal usuarioId, string usu_postPago, string IdCustomer)
        {
            DataSet dsReturn = new DataSet();
            string sqlquery = "USP_LEER_MAESTROS_PEDIDO_MVC";
            List<Ent_Combo> ListPromotor = null;
            List<Ent_Combo> ListFormaPago = null;
           
            Ent_Pedido_Maestro Maestro = new Ent_Pedido_Maestro();
            if (IdCustomer == "") {IdCustomer = "0";}

            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        SqlParameter oCodUsuario = cmd.Parameters.Add("@IdUsuario", SqlDbType.Decimal);
                        oCodUsuario.Direction = ParameterDirection.Input;
                        oCodUsuario.Value = usuarioId;

                        SqlParameter oCodPost = cmd.Parameters.Add("@post", SqlDbType.VarChar);
                        oCodPost.Direction = ParameterDirection.Input;
                        oCodPost.Value = usu_postPago;

                        SqlParameter oCustt = cmd.Parameters.Add("@customer", SqlDbType.Decimal);
                        oCustt.Direction = ParameterDirection.Input;
                        oCustt.Value = Convert.ToDecimal(IdCustomer);

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
                                                 cant_nota = Convert.ToDecimal(dr["cant_Nota"]),
                                                 

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

        public string[] Gua_Mod_Liquidacion(decimal _usu, decimal _idCust, string _reference, decimal _discCommPctg,
                                               decimal _discCommValue, string _shipTo, string _specialInstr, List<Ent_Order_Dtl> _itemsDetail,
                                               decimal _varpercepcion, Int32 _estado, string _ped_id = "", string _liq = "", Int32 _liq_dir = 0,
                                               Int32 _PagPos = 0, string _PagoPostarjeta = "", string _PagoNumConsignacion = "", decimal _PagoTotal = 0,
                                               DataTable dtpago = null, Boolean _pago_credito = false, Decimal _porc_percepcion = 0, List<Order_Dtl_Temp>
                                                order_dtl_temp = null, string strTipoPago = "N")
        {
            string[] resultDoc = new string[2];
            string sqlquery = "USP_Insertar_Modifica_Liquidacion";
            SqlConnection cn = null;
            SqlCommand cmd = null;
            SqlDataAdapter da = new SqlDataAdapter();
            DataSet ds = new DataSet();
            try
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("Ped_Det_Id", typeof(string));
                dt.Columns.Add("Ped_Det_Items", typeof(Int32));
                dt.Columns.Add("Ped_Det_ArtId", typeof(string));
                dt.Columns.Add("Ped_Det_TalId", typeof(string));
                dt.Columns.Add("Ped_Det_Cantidad", typeof(Int32));
                dt.Columns.Add("Ped_Det_Costo", typeof(decimal));
                dt.Columns.Add("Ped_Det_Precio", typeof(decimal));
                dt.Columns.Add("Ped_Det_ComisionP", typeof(decimal));
                dt.Columns.Add("Ped_Det_ComisionM", typeof(decimal));

                dt.Columns.Add("Ped_Det_OfertaP", typeof(decimal));
                dt.Columns.Add("Ped_Det_OfertaM", typeof(decimal));
                dt.Columns.Add("Ped_Det_OfeID", typeof(decimal));
                dt.Columns.Add("Ped_Det_PremID", typeof(Int32));

                int i = 1;
                // Recorrer todas las lineas adicionAQUARELLAs al detalle

                if (_itemsDetail != null)
                {
                    foreach (Ent_Order_Dtl item in _itemsDetail)
                    {
                        dt.Rows.Add(_ped_id, i, item._code, item._size, item._qty, 0, item._price, item._commissionPctg, Math.Round(item._commission, 2, MidpointRounding.AwayFromZero), item._ofe_porc, item._dscto, item._ofe_id, Convert.ToInt32(item._premId));
                        i++;
                    }
                }

                /*pedido original*/
                DataTable dtordertmp = new DataTable();
                dtordertmp.Columns.Add("items", typeof(Int32));
                dtordertmp.Columns.Add("articulo", typeof(string));
                dtordertmp.Columns.Add("talla", typeof(string));
                dtordertmp.Columns.Add("cantidad", typeof(Int32));




                if (order_dtl_temp != null)
                {
                    foreach (Order_Dtl_Temp item in order_dtl_temp)
                    {
                        dtordertmp.Rows.Add(item.items, item.articulo, item.talla, item.cantidad);
                    }
                }


                //grabar pedido
                cn = new SqlConnection(Ent_Conexion.conexion);
                if (cn.State == 0) cn.Open();
                cmd = new SqlCommand(sqlquery, cn);
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@strTipoPago", strTipoPago);

                cmd.Parameters.AddWithValue("@Estado", _estado);
                cmd.Parameters.AddWithValue("@Ped_Id", _ped_id);
                //cmd.Parameters.AddWithValue("@LiqId", _liq);
                cmd.Parameters.Add("@LiqId", SqlDbType.VarChar, 12);
                cmd.Parameters["@LiqId"].Value = _liq;
                cmd.Parameters["@LiqId"].Direction = ParameterDirection.InputOutput;
                cmd.Parameters.AddWithValue("@Liq_BasId", _idCust);
                cmd.Parameters.AddWithValue("@Liq_ComisionP", _discCommPctg);
                cmd.Parameters.AddWithValue("@Liq_PercepcionM", _varpercepcion);
                cmd.Parameters.AddWithValue("@Liq_Usu", _usu);
                cmd.Parameters.AddWithValue("@Detalle_Pedido", dt);
                cmd.Parameters.AddWithValue("@Liquidacion_Directa", _liq_dir);

                /*PEDIDO ORIGINAL*/
                cmd.Parameters.AddWithValue("@pedido_original", dtordertmp);

                //opcional pago por pos liquidacion directa
                cmd.Parameters.AddWithValue("@Pago_Pos", _PagPos);
                cmd.Parameters.AddWithValue("@Pago_PosTarjeta", _PagoPostarjeta);
                cmd.Parameters.AddWithValue("@Pago_numconsigacion", _PagoNumConsignacion);
                cmd.Parameters.AddWithValue("@Pago_Total", _PagoTotal);


                //pago directo de la liquidacion
                cmd.Parameters.AddWithValue("@DetallePago", dtpago);
                cmd.Parameters.AddWithValue("@Pago_Credito", _pago_credito);

                //porcentaje percepcion
                cmd.Parameters.AddWithValue("@Ped_Por_Perc", _porc_percepcion);
                //da = new SqlDataAdapter(cmd);
                //da.Fill(ds);

                cmd.ExecuteNonQuery();
                resultDoc[0] = cmd.Parameters["@LiqId"].Value.ToString();
            }
            catch (Exception ex)
            {
                if (cn != null)
                    if (cn.State == ConnectionState.Open) cn.Close();
                resultDoc[0] = "-1";
                resultDoc[1] = ex.Message;
            }
            if (cn != null)
                if (cn.State == ConnectionState.Open) cn.Close();
            return resultDoc;
        }

        public List<Ent_Order_Dtl> getLiquidacionDetalle(string  idLiquidacion)
        {
            string sqlquery = "USP_LEER_LIQUIDACION_MVC";
            SqlConnection cn = null;
            SqlCommand cmd = null;
            SqlDataAdapter da = null;
            DataSet ds = null;
            List<Ent_Order_Dtl> ListPedido = null;
            Ent_Order_Dtl entPedido = null;

            try
            {

                cn = new SqlConnection(Ent_Conexion.conexion);
                cmd = new SqlCommand(sqlquery, cn);
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Liq_Id", idLiquidacion);
                da = new SqlDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);
                ListPedido = new List<Ent_Order_Dtl>();
                ListPedido = (from DataRow dr in ds.Tables[0].Rows
                              select new Ent_Order_Dtl()
                              {
                                  _idPedido = dr["Liq_PedId"].ToString(),
                                  _code = dr["Art_Id"].ToString(),
                                  _brand = dr["Mar_Descripcion"].ToString(),
                                  _artName = dr["Art_Descripcion"].ToString(),
                                  _ArtImg = dr["Ped_Imagen"].ToString(),
                                  _size = dr["Tal_Descripcion"].ToString(),
                                  _color = dr["Col_Descripcion"].ToString(),
                                  _qty = Convert.ToInt16(dr["Ped_Det_Cantidad"]),
                                  _Stkqty = Convert.ToInt16(dr["stk_Cant"]),
                                  _price = Convert.ToDecimal(dr["Ped_Det_Precio"]),
                                  _commission = Convert.ToDecimal(dr["Ped_Det_ComisionM"]),
                                  _Mto_percepcion = Convert.ToDecimal(dr["Ped_Mto_Perc"]),
                                  _Pctg_percepcion = Convert.ToDecimal(dr["Ped_Por_Perc"]),

                                  _ofe_Tipo = dr["Ofe_tipo"].ToString(),
                                  _ofe_PrecioPack = Convert.ToDecimal(dr["Ofe_ArtVenta"]),
                                  _ofe_id = Convert.ToDecimal(dr["Ped_Det_OfeID"]),
                                  _ofe_porc = Convert.ToDecimal(dr["Ped_Det_OfertaP"]),
                                  _ofe_maxpares = Convert.ToDecimal(dr["Ofe_MaxPares"]),
                                  _dscto = Convert.ToDecimal(dr["Ped_Det_OfertaM"]),
                                  
                             
                                  _comm = Convert.ToInt16(dr["Ped_Por_Com"]),
                                  _premio = dr["Premio"].ToString(),
                                  _premId = dr["PremioId"].ToString(),
                                  _ap_percepcion = dr["Ped_Por_Perc"].ToString(),
                                  _premioDesc = dr["Regalo"].ToString()

                              }).ToList();

                return ListPedido;
            }
            catch (Exception e) {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public List<Ent_Liquidacion> ListarPedidos(decimal IdPromotor)
        {
            string sqlquery = "USP_LEER_PEDIDO_USUARIO_MVC";
            SqlConnection cn = null;
            SqlCommand cmd = null;
            SqlDataAdapter da = null;
            DataSet ds = null;
            List<Ent_Liquidacion> ListPedido = null;
            Ent_Liquidacion entPedido = null;

            try
            {

                cn = new SqlConnection(Ent_Conexion.conexion);
                cmd = new SqlCommand(sqlquery, cn);
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@bas_id", IdPromotor);
                da = new SqlDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);
                ListPedido = new List<Ent_Liquidacion>();
                ListPedido = (from DataRow dr in ds.Tables[1].Rows
                               select new Ent_Liquidacion()
                               {
                                   liq_Id = dr["Liq_Id"].ToString(),
                                   cust_Id = dr["Liq_BasId"].ToString(),
                                   ped_Id = dr["liq_PedId"].ToString(),
                                   liq_Fecha = dr["Fecha"].ToString(),
                                   Pares = Convert.ToDecimal(dr["Liq_Det_Cantidad"]),
                                   Estado = dr["Est_Descripcion"].ToString(),
                                   TotalPagar = Convert.ToDecimal(dr["Tpagar"]),
                               }).ToList();

                return ListPedido;
            }
            catch (Exception e) { throw new Exception(e.Message, e.InnerException); }
        }


        public List<Ent_Pago_NCredito> ListarNotaCredito(string BasId, string LiqId) {

            DataSet dsReturn = new DataSet();
            string sqlquery = "USP_LEER_PAGO_LIQ_MVC";
            List<Ent_Pago_NCredito> ListNotaCredito = null;          

            try
            {
                if (LiqId == null) LiqId = "";

                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        SqlParameter oBasId = cmd.Parameters.Add("@bas_id", SqlDbType.Decimal);
                        oBasId.Direction = ParameterDirection.Input;
                        oBasId.Value = BasId;

                        SqlParameter oLiqId = cmd.Parameters.Add("@liq_id", SqlDbType.VarChar);
                        oLiqId.Direction = ParameterDirection.Input;
                        oLiqId.Value = LiqId;

                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {

                            da.Fill(dsReturn);

                            ListNotaCredito = new List<Ent_Pago_NCredito>();
                            ListNotaCredito = (from DataRow dr in dsReturn.Tables[0].Rows
                                             select new Ent_Pago_NCredito()
                                             {
                                                 Consumido = Convert.ToBoolean(dr["checks"]),
                                                 Activado = Convert.ToBoolean(dr["active"]),
                                                 Ncredito = dr["ncredito"].ToString(),
                                                 Importe = Convert.ToDecimal(dr["importe"]),
                                                 Rhv_return_nro = dr["rhv_return_no"].ToString(),
                                                 Fecha_documento = Convert.ToDateTime(dr["dtd_document_date"]),

                                             }).ToList();      
                        }
                    }
                }                
            }
            catch (Exception exc)
            {
                ListNotaCredito = null;
            }
            return ListNotaCredito;
        }

    }
}
