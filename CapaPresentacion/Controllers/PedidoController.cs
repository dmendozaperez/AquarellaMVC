using CapaDato.Util;
using CapaDato.Persona;
using CapaDato.Pedido;
using CapaEntidad.Control;
using CapaEntidad.Menu;
using CapaEntidad.Util;
using CapaEntidad.Pedido;
using CapaPresentacion.Bll;
using System;
using System.Collections.Generic;

using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Data;
using CapaEntidad.General;
using System.Linq;
using CapaEntidad.Persona;
using Newtonsoft.Json;
using System.Web.SessionState;
using CapaPresentacion.Models.Crystal;
using CapaPresentacion.Data.RptsCrystal;
using CapaDato.Cliente;
using CapaEntidad.Cliente;

namespace CapaPresentacion.Controllers
{
    public class PedidoController : Controller
    {
        // GET: Funcion
        private Dat_Pedido datPedido = new Dat_Pedido();
        private Dat_Persona datPersona = new Dat_Persona();
        private string _session_listPedido_private = "_session_listPedido_private";
        private string _session_list_NotaCredito = "_session_list_NotaCredito";
        private string _session_list_consignaciones = "_session_list_consignaciones";
        private string _session_list_saldos = "_session_list_saldos";
        private string _session_list_detalle_pedido = "_session_list_detalle_pedido";
        private string _session_nuevo_item_pedido = "_session_nuevo_item_pedido";
        private string _session_Tran_Ofertas = "_session_TranOfertas";
        private string _session_customer = "_session_customer";
        private string _session_lnfo_liquidacion = "_session_lnfo_liquidacion";
        private string _session_notas_persona = "_session_notas_persona";
        private Dat_Cliente dat_cliente = new Dat_Cliente();

        public ActionResult CrearEditar(string custId = "" , string liqId = "" , string pedId = "", string liq_tipo_prov="",string liq_tipo_des="",
                                       string liq_agencia="",string liq_agencia_direccion="",string liq_destino="",string liq_direccion="",
                                       string liq_referencia="")
        {
            Session[_session_list_detalle_pedido] = null;
            Session[_session_Tran_Ofertas] = null;
            Session[_session_nuevo_item_pedido] = null;
            Session[_session_customer] = null;
            Session[_session_notas_persona] = null;
            Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];

            string actionName = this.ControllerContext.RouteData.GetRequiredString("action");
            string controllerName = this.ControllerContext.RouteData.GetRequiredString("controller");
            string return_view = actionName + "|" + controllerName;

            if (_usuario == null)
            {
                return RedirectToAction("Login", "Control", new { returnUrl = return_view });
            }
            else
            {
                #region<VALIDACION DE ROLES DE USUARIO>
                //Boolean valida_rol = true;
                //Basico valida_controller = new Basico();
                //List<Ent_Menu_Items> menu = (List<Ent_Menu_Items>)Session[Ent_Global._session_menu_user];
                //valida_rol = valida_controller.AccesoMenu(menu, this);
                #endregion
                if (custId == "")
                {
                    return RedirectToAction("Lista", "Pedido");
                }
                //if (valida_rol)
                //{
                    string IdPedido = "";
                    string IdCustomer = "";
                    string strLiqId = "";
                    Ent_Persona customer = datPersona.GET_INFO_PERSONA(custId);
                    if (liqId != "")
                    {
                        IdPedido = pedId;
                        IdCustomer = custId;
                        strLiqId = liqId;
                        ViewBag.operacion = "Editar";
                        Session[_session_list_detalle_pedido] = listaDetalleLiquidacion(liqId, customer._commission);
                    }
                    else
                    {
                        ViewBag.operacion = "Nuevo";
                    }

                    
                    List<Ent_Pago_NCredito> notas = datPersona.get_nota_credito(custId, strLiqId);

                    Session[_session_notas_persona] = notas;

                    ViewBag.infoProm = customer;
                    ViewBag.CountNotas = notas.Count();
                    Session[_session_customer] = customer;

                    

                    Ent_Pedido_Maestro maestros = datPedido.Listar_Maestros_Pedido(_usuario.usu_id, _usuario.usu_postPago, IdCustomer);


                    List<Ent_Cliente_Despacho> lis_des = dat_cliente.lista_despacho();

                    //var select = lis_des.Select(a => a.desp_cod == "0").ToList();

                    //lis_des.RemoveAt(0);

                    ViewBag.listPromotor = maestros.combo_ListPromotor;
                    ViewBag.listFormaPago = maestros.combo_ListFormaPago;
                    ViewBag.IdLiquidacion = liqId;
                    ViewBag.despacho = lis_des;// dat_cliente.lista_despacho();

                    Ent_Liquidacion oLiquidacion = new Ent_Liquidacion();                   
                                
                    oLiquidacion.liq_Id = strLiqId;
                    oLiquidacion.ped_Id = IdPedido;
                    oLiquidacion.cust_Id = IdCustomer;

                    oLiquidacion.liq_tipo_prov = liq_tipo_prov;
                    oLiquidacion.liq_tipo_des = liq_tipo_des;
                    oLiquidacion.liq_agencia = liq_agencia;
                    oLiquidacion.liq_agencia_direccion = liq_agencia_direccion;
                    oLiquidacion.liq_destino = liq_destino;
                    oLiquidacion.liq_direccion = liq_direccion;
                    oLiquidacion.liq_referencia = liq_referencia;


                    ViewBag.Liqui = oLiquidacion;

                    Session[_session_lnfo_liquidacion] = oLiquidacion;

                    return View("CrearEditar", oLiquidacion);
                //}
                //else
                //{
                //    return RedirectToAction("Login", "Control", new { returnUrl = return_view });
                //}
            }

        }
        public ActionResult VerLiquidacion(string liquidacion)
        {
            GetCRLiquidacion(liquidacion, false);
            return Json(new { estado = 0 });
        }
        public ActionResult VerFactura(string liquidacion, string invoice)
        {
            try
            {
                GetCRInvoice(invoice, liquidacion);
                return Json(new { estado = 0 });
            }
            catch (Exception ex)
            {
                return Json(new { estado = 1 });
            }
            
        }
        public ActionResult PersonaPedido(int basId) {
            
            Ent_Pedido_Persona Persona = new Ent_Pedido_Persona();
            Persona = datPedido.BuscarPersonaPedido(basId);

            return Json(Persona, JsonRequestBehavior.AllowGet);
        }

        public ActionResult listarStr_ArticuloTalla(string codArticulo)
        {                       
            Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
            Ent_Persona cust = (Ent_Persona)Session[_session_customer];
            Ent_Articulo_pedido articulo = new Ent_Articulo_pedido();
            List<Ent_Articulo_Tallas> tallas = new List<Ent_Articulo_Tallas>();
            datPedido.listarStr_ArticuloTalla(codArticulo, Convert.ToDecimal(cust.Bas_id), ref articulo, ref tallas);

            Ent_Order_Dtl newLineOrder = new Ent_Order_Dtl();
            //List<Ent_Order_Dtl> order = new List<Ent_Order_Dtl>();
                        
            Session[_session_nuevo_item_pedido] = articulo;
            
            return Json(new { articulo = articulo, tallas = tallas } , JsonRequestBehavior.AllowGet);
        }     

        public JsonResult GenerarCombo(int Numsp, string Params)
        {
            string strJson = "";
            JsonResult jRespuesta = null;
            var serializer = new JavaScriptSerializer();


            //switch (Numsp)
            //{
            //    case 1:
            //        strJson = datUtil.listarStr_Provincia(Params);
            //        jRespuesta = Json(serializer.Deserialize<List<Ent_Combo>>(strJson), JsonRequestBehavior.AllowGet);
            //        break;
            //    case 2:
            //        String[] substrings = Params.Split('|');
            //        strJson = datUtil.listarStr_Distrito(Params);
            //        jRespuesta = Json(serializer.Deserialize<List<Ent_Combo>>(strJson), JsonRequestBehavior.AllowGet);
            //        break;
            //    default:
            //        Console.WriteLine("Default case");
            //        break;
            //}
            return jRespuesta;
        }

        private static string[] splitString(string _textString, char _character)
        {
            string[] split = null;
            if (!string.IsNullOrEmpty(_textString))
            {
                split = _textString.Split(new Char[] { _character });
            }
            return split;
        }
        public ActionResult LiquidarPedido(string tipo_des,string agencia,string destino,string direccion_agencia,string direccion,string referencia,
                                           string liq_tipo_prov,string liq_provincia, decimal _usu = 0, 
                                           decimal _idCust = 0, string _reference = "", decimal _discCommPctg = 0,
                                           decimal _discCommValue = 0, string _shipTo = "", string _specialInstr = "", List<Ent_Order_Dtl> _itemsDetail = null,
                                           decimal _varpercepcion = 0, Int32 _estado = 1, string _ped_id = "", string _liq = "", Int32 _liq_dir = 0,
                                           Int32 _PagPos = 0, string _PagoPostarjeta = "", string _PagoNumConsignacion = "", decimal _PagoTotal = 0,
                                           /*DataTable dtpago = null*/ List<Ent_Documents_Trans> ListPago=null, Boolean _pago_credito = false, Decimal _porc_percepcion = 0, List<Order_Dtl_Temp>
                                           order_dtl_temp = null, string strTipoPago = "N")
        {

            string[] noOrder;
            Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
            Ent_Persona cust = (Ent_Persona)Session[_session_customer];
            List<Ent_Order_Dtl> order = (List<Ent_Order_Dtl>)Session[_session_list_detalle_pedido];
            Ent_Order_Hdr header = getTotals();
            order_dtl_temp = new List<Order_Dtl_Temp>();



            int index = 0;
            foreach (Ent_Order_Dtl item in order)
            {
                index++;
                order_dtl_temp.Add(new Order_Dtl_Temp()
                {
                    items = index,
                    articulo = item._code,
                    cantidad = item._qty,
                    premId = null,
                    premio = "",
                    talla = item._size
                });
            }
            Ent_Liquidacion liq = (Ent_Liquidacion)Session[_session_lnfo_liquidacion];
            /*Armamos los documentos de pago (nota de credito) utilizados en la liquidacion*/
            DataTable dtpago = new DataTable();
            dtpago.Columns.Add("Liq_Id", typeof(string));
            dtpago.Columns.Add("Doc_Tra_Id", typeof(string));
            dtpago.Columns.Add("Monto", typeof(Double));

            if (Session[_session_notas_persona] == null)
            {
                List<Ent_Pago_NCredito> _listNC = new List<Ent_Pago_NCredito>();
                Session[_session_notas_persona] = _listNC;
            }
            List<Ent_Pago_NCredito> listNC = new List<Ent_Pago_NCredito>();
            listNC = (List<Ent_Pago_NCredito>)Session[_session_notas_persona];


            if (listNC != null) { 

                foreach (Ent_Pago_NCredito dTx in listNC.Where(n=>n.Consumido==true).ToList())
                {
                    dtpago.Rows.Add("", dTx.Rhv_return_nro, dTx.Importe);
                }
            }

            /*fin de los documentos de pago*/
            if (string.IsNullOrEmpty(liq.liq_Id))
            {
                noOrder = datPedido.Gua_Mod_Liquidacion(tipo_des,agencia,destino,direccion_agencia,direccion,referencia,
                                                     liq_tipo_prov,liq_provincia, _usuario.usu_id, Convert.ToDecimal(cust.Bas_id), string.Empty, cust._commission, 0, string.Empty, string.Empty, order, header._percepcion,
                                                    _estado, _ped_id, _liq, _liq_dir, _PagPos, _PagoPostarjeta, _PagoNumConsignacion, _PagoTotal, dtpago, _pago_credito,
                                                    cust._percepcion, order_dtl_temp, cust._vartipopago);
            }
            else
            {
                noOrder = datPedido.Gua_Mod_Liquidacion(tipo_des,agencia,destino, direccion_agencia,direccion, referencia,
                                           liq_tipo_prov,liq_provincia, _usuario.usu_id, Convert.ToDecimal(cust.Bas_id), string.Empty, cust._commission, 0, string.Empty, string.Empty, order, header._percepcion,
                                                    2, liq.ped_Id, liq.liq_Id, _liq_dir, _PagPos, _PagoPostarjeta, _PagoNumConsignacion, _PagoTotal, dtpago, _pago_credito,
                                                    cust._percepcion, order_dtl_temp, cust._vartipopago);
            }
            var oJRespuesta = new JsonResponse();

            if ((noOrder[0]).ToString() != "-1")
            {
                oJRespuesta.Message = (noOrder[0]).ToString();
                oJRespuesta.Data = true;
                oJRespuesta.Success = true;
                GetCRLiquidacion((noOrder[0]).ToString());
            }
            else {

                oJRespuesta.Message = (noOrder[1]).ToString();
                oJRespuesta.Data = false;
                oJRespuesta.Success = false;
            }
            return Json(oJRespuesta, JsonRequestBehavior.AllowGet);
        }

        private void GetCRLiquidacion(string liq, bool download = false)
        {
            
                Data_Cr_Aquarella datCrAq = new Data_Cr_Aquarella();
                List<Liquidation> _liqValsReport = new List<Liquidation>();

                DataSet dsLiqInfo = datCrAq.data_reporte_liquidacion(liq);

                if (dsLiqInfo == null)
                    return;

                //DataSet dsLiqDtl =  Liquidation_Dtl.getLiquidationDtl(_user._usv_co, _noLiq);
                DataSet dsLiqDtl = new DataSet();
                dsLiqDtl.Tables.Add(dsLiqInfo.Tables[1].Copy());

                if (dsLiqDtl == null)
                    return;

                DataRow dRow = dsLiqInfo.Tables[0].Rows[0];

                foreach (DataRow dRowDtl in dsLiqDtl.Tables[0].Rows)
                {
                    string vncredito = ""; decimal VtotalcreditoTotal = 0;
                    string vfecha = DateTime.Today.ToString("dd/MM/yyyy");



                    //Bata.Aquarella.BLL.Reports.Liquidation objLiqReport = new BLL.Reports.Liquidation(dRow["ohv_warehouseid"].ToString(), dRow["wav_description"].ToString(),
                    //    dRow["wav_address"].ToString(), dRow["wav_telephones"].ToString(), dRow["ubicationwav"].ToString(), dRow["con_coordinator_id"].ToString(), dRow["bdv_document_no"].ToString(),
                    //    dRow["name"].ToString(), dRow["bdv_address"].ToString(), dRow["bdv_phone"].ToString(), dRow["bdv_movil_phone"].ToString(), dRow["bdv_email"].ToString(),
                    //    dRow["ubicationcustomer"].ToString(), dRow["lhv_liquidation_no"].ToString(), Convert.ToDateTime(dRow["lhd_date"]), Convert.ToDateTime(dRow["lhd_expiration_date"].ToString()),
                    //    dRow["stv_description"].ToString(), Convert.ToDecimal(dRow["lon_disscount"]), Convert.ToDecimal(dRow["tax_rate"]), Convert.ToDecimal(dRow["lhn_tax_rate"]), Convert.ToDecimal(dRow["lhn_handling"]),
                    //    dRowDtl["ldv_article"].ToString(), dRowDtl["brv_description"].ToString(), dRowDtl["cov_description"].ToString(), dRowDtl["arv_name"].ToString(), dRowDtl["ldv_size"].ToString(), Convert.ToDecimal(dRowDtl["ldn_qty"]),
                    //    Convert.ToDecimal(dRowDtl["ldn_sell_price"]), Convert.ToDecimal(dRowDtl["ldn_commission"]), Convert.ToDecimal(dRowDtl["ldn_disscount"]), Convert.ToDecimal(dRow["percepcion"]), Convert.ToDecimal(dRow["porc_percepcion"]),
                    //    Convert.ToDecimal(dRow["ncredito"]), vncredito, Convert.ToDateTime(vfecha), VtotalcreditoTotal, _noLiq, Convert.ToDecimal(dRow["totalop"]));


                    Liquidation objLiqReport = new Liquidation("1", dRow["almacen"].ToString(),
                     dRow["alm_direccion"].ToString(), dRow["Alm_Telefono"].ToString(), "", dRow["Bas_Id"].ToString(), dRow["Bas_Documento"].ToString(),
                     dRow["nombres"].ToString(), dRow["Bas_Direccion"].ToString(), dRow["Bas_Telefono"].ToString(), dRow["Bas_Celular"].ToString(), dRow["Bas_Correo"].ToString(),
                     dRow["ubicacion"].ToString(), dRow["Liq_Id"].ToString(), Convert.ToDateTime(dRow["Liq_FechaIng"]), Convert.ToDateTime(dRow["Liq_Fecha_Expiracion"].ToString()),
                     dRow["estado"].ToString(), 0, Convert.ToDecimal(dRow["igvporc"]), Convert.ToDecimal(dRow["igvmonto"]), 0,
                     dRowDtl["Art_Id"].ToString(), dRowDtl["Mar_Descripcion"].ToString(), dRowDtl["Col_Descripcion"].ToString(), dRowDtl["art_descripcion"].ToString(), dRowDtl["Liq_Det_TalId"].ToString(), Convert.ToDecimal(dRowDtl["Liq_Det_Cantidad"]),
                     Convert.ToDecimal(dRowDtl["Liq_Det_Precio"]), Convert.ToDecimal(dRowDtl["Liq_Det_Comision"]), 0, Convert.ToDecimal(dRow["Percepcionm"]), Convert.ToDecimal(dRow["Percepcionp"]),
                     Convert.ToDecimal(dRow["ncredito"]), vncredito, Convert.ToDateTime(vfecha), VtotalcreditoTotal, liq, Convert.ToDecimal(dRow["totalop"]), Convert.ToDecimal(dRowDtl["Liq_Det_OfertaM"]), dRow["Opg"].ToString());

                    _liqValsReport.Add(objLiqReport);
                }




                List<LiqNcSubinforme>_liqValsSubReport = new List<LiqNcSubinforme>();

                //DataSet dsLiqpagoInfo = Liquidations_Hdr.getpagoncreditoliqui(_noLiq);
                DataSet dsLiqpagoInfo = new DataSet();
                dsLiqpagoInfo.Tables.Add(dsLiqInfo.Tables[2].Copy());

                if (dsLiqpagoInfo == null)
                    return;

                foreach (DataRow dRowDtl in dsLiqpagoInfo.Tables[0].Rows)
                {
                    string vncredito = dRowDtl["ncredito"].ToString();
                    decimal VtotalcreditoTotal = Convert.ToDecimal(dRowDtl["Total"].ToString());
                    DateTime vfecha = Convert.ToDateTime(dRowDtl["fecha"].ToString());

                    LiqNcSubinforme objLiqpagoReport = new LiqNcSubinforme("", vncredito, vfecha, VtotalcreditoTotal);

                    _liqValsSubReport.Add(objLiqpagoReport);
                }

                List<VentaPagoSubInforme> _liqValsPagoSubReport = new List<VentaPagoSubInforme>();
                //DataSet dsLiqpagoformainfo = Liquidations_Hdr.getpagonformaliqui(_noLiq);
                DataSet dsLiqpagoformainfo = new DataSet();
                dsLiqpagoformainfo.Tables.Add(dsLiqInfo.Tables[3].Copy());
                if (dsLiqpagoformainfo == null)
                    return;
                foreach (DataRow drowdtl in dsLiqpagoformainfo.Tables[0].Rows)
                {
                    string vpago = drowdtl["pago"].ToString();
                    string vdocumento = drowdtl["Documento"].ToString();
                    DateTime vfecha = Convert.ToDateTime(drowdtl["fecha"].ToString());
                    Decimal vtotal = Convert.ToDecimal(drowdtl["Total"].ToString());
                    VentaPagoSubInforme objLiqpagoformaReport = new  VentaPagoSubInforme(vpago, vdocumento, vfecha, vtotal);
                    _liqValsPagoSubReport.Add(objLiqpagoformaReport);
                }

            this.HttpContext.Session["ReportName"] = "liquidationReport.rpt";
            this.HttpContext.Session["rptSource"] = _liqValsReport;
            this.HttpContext.Session["rptSource1"] = _liqValsSubReport;
            this.HttpContext.Session["rptSource2"] = _liqValsPagoSubReport;
            this.HttpContext.Session["rptDownload"] = download;
        }
        public void GetCRInvoice(string invoice,string noOrder) 
        {
            
                Data_Cr_Aquarella datCrAq = new Data_Cr_Aquarella();
                DataSet ds_venta = datCrAq.getInvoiceHdr(invoice);
                DataTable invoiceHdr = ds_venta.Tables[0].Copy();
                //DataTable invoiceHdr = Facturacion.getInvoiceHdr(this._user._usv_co, this._noInvoice, this._noOrderUrl);
                if (invoiceHdr.Rows.Count > 0)
                {
                    //DataTable warehouseByPk = new warehouses(this._user._usv_co, invoiceHdr.Rows[0]["stv_warehouse"].ToString()).getWarehouseByPk();
                    string wavDescription = "";
                    string wavAddress = "";
                    string wavPhone = "";
                    string wavUbication = "";
                    //if (warehouseByPk != null && warehouseByPk.Rows.Count > 0)
                    //{
                    wavDescription = invoiceHdr.Rows[0]["almacen"].ToString().ToUpper();
                    wavAddress = invoiceHdr.Rows[0]["alm_direccion"].ToString();
                    wavPhone = invoiceHdr.Rows[0]["Alm_Telefono"].ToString();
                    wavUbication = "";
                    //}
                    string typeresolution = "";

                    //DataTable invoiceDtl = Facturacion.getInvoiceDtl(this._user._usv_co, this._noInvoice);

                    DataTable invoiceDtl = ds_venta.Tables[1].Copy();

                    string str = "";
                    Decimal descuentoGnral = 0;
                    string numeroRemision = "";
                    string destinatario = invoiceHdr.Rows[0]["nombres"].ToString();
                    string cedula = invoiceHdr.Rows[0]["Bas_Documento"].ToString();
                    string ubicacionDestinatario = invoiceHdr.Rows[0]["ubicacion"].ToString();
                    string telefono = invoiceHdr.Rows[0]["Bas_Telefono"].ToString();
                    string trasportadora = invoiceHdr.Rows[0]["Tra_Descripcion"].ToString();
                    string numeroGuia = invoiceHdr.Rows[0]["Tra_Gui_No"].ToString();
                    Decimal porc_percepcion = Convert.ToDecimal(invoiceHdr.Rows[0]["Percepcionp"].ToString());
                    Decimal iva = Convert.ToDecimal(invoiceHdr.Rows[0]["igvmonto"].ToString());
                    Decimal flete = 0;
                    DateTime fechaRemision = Convert.ToDateTime(invoiceHdr.Rows[0]["Ven_Fecha"].ToString());
                    Decimal ncredito = Convert.ToDecimal(invoiceHdr.Rows[0]["ncredito"].ToString());
                    Decimal totalop = Convert.ToDecimal(invoiceHdr.Rows[0]["totalop"].ToString());
                    List<ReporteFacturacion> _invoice = new List<ReporteFacturacion>();

                    foreach (DataRow dataRow in (InternalDataCollectionBase)invoiceDtl.Rows)
                    {
                        string numFactura = dataRow["Ven_Det_Id"].ToString();
                        string esCopia = str;
                        string msgs = "";// invoiceHdr.Rows[0]["imv_text"].ToString();
                        string codigoArticulo = dataRow["Art_Id"].ToString();
                        string nomArticulo = dataRow["art_descripcion"].ToString();
                        Decimal cantidad = Convert.ToDecimal(dataRow["Ven_Det_Cantidad"].ToString());
                        string talla = dataRow["Ven_Det_TalId"].ToString();
                        Decimal precio = Convert.ToDecimal(dataRow["Ven_Det_Precio"].ToString());
                        Decimal valorLinea = Convert.ToDecimal(dataRow["articulo_value"].ToString());
                        Decimal descuentoArticulo = 0;
                        Decimal comisionLineal = Convert.ToDecimal(dataRow["Ven_Det_ComisionM"].ToString());
                        string descripcionArtic = dataRow["Col_Descripcion"].ToString();
                        _invoice.Add(new ReporteFacturacion(destinatario, ubicacionDestinatario, telefono, "", "", cedula, "", noOrder, numFactura, fechaRemision, numeroRemision, "", esCopia, typeresolution, codigoArticulo, nomArticulo, descripcionArtic, cantidad, talla, precio, descuentoArticulo, comisionLineal, valorLinea, iva, flete, numeroGuia, trasportadora, msgs, descuentoGnral, wavDescription, wavAddress, wavPhone, wavUbication, porc_percepcion, ncredito, totalop));
                    }
                this.HttpContext.Session["rptSource"] = _invoice;
                List<LiqNcSubinforme> subin1 = new List<LiqNcSubinforme>();

                    //DataSet dsLiqpagoInfo = Liquidations_Hdr.getpagoncreditoliqui(this._noOrderUrl);
                    DataSet dsLiqpagoInfo = new DataSet();
                    dsLiqpagoInfo.Tables.Add(ds_venta.Tables[2].Copy());

                    if (dsLiqpagoInfo == null)
                        return;

                    foreach (DataRow dRowDtl in dsLiqpagoInfo.Tables[0].Rows)
                    {
                        string vncredito = dRowDtl["ncredito"].ToString();
                        decimal VtotalcreditoTotal = Convert.ToDecimal(dRowDtl["Total"].ToString());
                        DateTime vfecha = Convert.ToDateTime(dRowDtl["fecha"].ToString());

                        LiqNcSubinforme objLiqpagoReport = new LiqNcSubinforme("", vncredito, vfecha, VtotalcreditoTotal);

                        subin1.Add(objLiqpagoReport);
                    }
                this.HttpContext.Session["rptSource1"] = subin1;
                List<VentaPagoSubInforme> subin2 = new List<VentaPagoSubInforme>();
                    //DataSet dsLiqpagoformaInfo = Liquidations_Hdr.getpagonformaliqui(this._noOrderUrl);
                    DataSet dsLiqpagoformaInfo = new DataSet();
                    dsLiqpagoformaInfo.Tables.Add(ds_venta.Tables[3].Copy());
                    if (dsLiqpagoInfo == null)
                        return;

                    foreach (DataRow dRowDtl in dsLiqpagoformaInfo.Tables[0].Rows)
                    {
                        string vpago = dRowDtl["pago"].ToString();
                        string vdocumento = dRowDtl["Documento"].ToString();
                        DateTime vfecha = Convert.ToDateTime(dRowDtl["fecha"].ToString());
                        Decimal vtotal = Convert.ToDecimal(dRowDtl["Total"].ToString());
                        VentaPagoSubInforme objLiqpagoformaReport = new VentaPagoSubInforme(vpago, vdocumento, vfecha, vtotal);
                        subin2.Add(objLiqpagoformaReport);
                    }
                this.HttpContext.Session["rptSource2"] = subin2;
            }
            this.HttpContext.Session["ReportName"] = "InvoiceReport.rpt";
        }
        public ActionResult Lista()
        {
            Session[_session_listPedido_private] = null;
            Session[_session_list_NotaCredito] = null;
            Session[_session_list_consignaciones] = null;
            Session[_session_list_saldos] = null;
            Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
            string actionName = this.ControllerContext.RouteData.GetRequiredString("action");
            string controllerName = this.ControllerContext.RouteData.GetRequiredString("controller");
            string return_view = actionName + "|" + controllerName;

            if (_usuario == null)
            {
                return RedirectToAction("Login", "Control", new { returnUrl = return_view });
            }

            Ent_Pedido_Maestro maestros = datPedido.Listar_Maestros_Pedido(_usuario.usu_id, _usuario.usu_postPago,"");
            ViewBag.listPromotor = maestros.combo_ListPromotor;
            ViewBag.usutipo = _usuario.usu_tip_id.ToString();
            
            //Ent_Promotor_Maestros maestros = datUtil.ListarEnt_Maestros_Promotor(_usuario.usu_id);
            //ViewBag.listLider = maestros.combo_ListLider;

            return View();
        }        
        public ActionResult GET_INFO_PERSONA_PEDIDO(string codigo)
        {
            try
            {
                Ent_Persona info = datPersona.GET_INFO_PERSONA(codigo);
                string _mensaje = "";
                Ent_Info_Promotor infoGeneralPedidos = datPedido.ListarPedidos(Convert.ToDecimal(codigo),ref _mensaje);
                Session[_session_listPedido_private] = infoGeneralPedidos.liquidacion;
                Session[_session_list_NotaCredito] = infoGeneralPedidos.notaCredito;
                Session[_session_list_consignaciones] = infoGeneralPedidos.consignaciones;
                Session[_session_list_saldos] = infoGeneralPedidos.saldos;

                return Json(new { estado = 0, info = info, mensaje = _mensaje });
            }
            catch (Exception ex)
            {
                return Json(new { estado = 2, mensaje = ex.Message });
            }            
        }

        //public ActionResult ListaNC(string BasId,string LiqId)
        //{
        //    List<Ent_Pago_NCredito> listNotaC = listaNotaCredito(BasId, LiqId);
        //    return View(listNotaC);
        //}
        public JsonResult getListaNC(string BasId, string LiqId)
        {
            List<Ent_Pago_NCredito> listNotaC = listaNotaCredito(BasId, LiqId);

          
            return Json(listNotaC, JsonRequestBehavior.AllowGet);// Json(new { listNotaC = listNotaC }, JsonRequestBehavior.AllowGet);
        }
        public List<Ent_Pago_NCredito> listaNotaCredito(string BasId, string LiqId)
        {
            List<Ent_Pago_NCredito> listNotaC = datPedido.ListarNotaCredito(BasId, LiqId);

            return listNotaC;
        }
        public JsonResult getLiquidacionDetalle(string LiqId)
        {
            List<Ent_Order_Dtl> listLiqDetalle = listaDetalleLiquidacion(LiqId);


            return Json(listLiqDetalle, JsonRequestBehavior.AllowGet);// Json(new { listNotaC = listNotaC }, JsonRequestBehavior.AllowGet);
        }
        public List<Ent_Order_Dtl> listaDetalleLiquidacion(string LiqId , decimal comm = 0)
        {
            List<Ent_Order_Dtl> listLiqDetalle = datPedido.getLiquidacionDetalle(LiqId);
            List<Ent_Order_Dtl> order = new List<Ent_Order_Dtl>();
            foreach (Ent_Order_Dtl item in listLiqDetalle)
            {
                loadOrderDtl(ref order, item, comm);
            }

            return order;
        }
        public ActionResult udateTipoPago(string tipoPago)
        {
            try
            {
                List<Ent_Order_Dtl> pedidoCompleto = (List<Ent_Order_Dtl>)Session[_session_list_detalle_pedido];
                for (int i = 0; i < pedidoCompleto.Count; i++)
                {
                    _addArticle(pedidoCompleto[i], 0, tipoPago, true);
                }              
                return Json(new { estado = 0 });
            }
            catch (Exception ex)
            {
                return Json(new { estado = 1 , mensaje = "Error al cambiar tipo de pago; " + ex.Message  });
            }            
        }
        /** Lista Liquidaciones **/
        public ActionResult getListPedido(Ent_jQueryDataTableParams param)
        {
            /*verificar si esta null*/
            if (Session[_session_listPedido_private] == null)
            {
                List<Ent_Liquidacion>  listPed = new List<Ent_Liquidacion>();
                Session[_session_listPedido_private] = listPed;
            }
            
            //Traer registros
            IQueryable<Ent_Liquidacion> membercol = ((List<Ent_Liquidacion>)(Session[_session_listPedido_private])).AsQueryable();  //lista().AsQueryable();

            //Manejador de filtros
            int totalCount = membercol.Count();
            IEnumerable<Ent_Liquidacion> filteredMembers = membercol;

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = membercol
                    .Where(m => m.liq_Id.ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.liq_Id.ToUpper().Contains(param.sSearch.ToUpper()));
            }
            //Manejador de orden
            var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);

            if (param.iSortingCols > 0)
            {
                if (Request["sSortDir_0"].ToString() == "asc")
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderBy(o => Convert.ToDecimal(o.liq_Id)); break;
                        case 1: filteredMembers = filteredMembers.OrderBy(o => o.ventaId); break;
                        case 2: filteredMembers = filteredMembers.OrderBy(o => Convert.ToDateTime(o.liq_Fecha)).ThenBy(to => Convert.ToDecimal(to.liq_Id)); break;
                        case 3: filteredMembers = filteredMembers.OrderBy(o => o.Pares); break;
                        case 4: filteredMembers = filteredMembers.OrderBy(o => o.Estado); break;
                        case 5: filteredMembers = filteredMembers.OrderBy(o => o.Ganancia); break;
                        case 6: filteredMembers = filteredMembers.OrderBy(o => o.Subtotal); break;
                        case 7: filteredMembers = filteredMembers.OrderBy(o => o.N_C); break;
                        case 8: filteredMembers = filteredMembers.OrderBy(o => o.Total); break;
                        case 9: filteredMembers = filteredMembers.OrderBy(o => o.Percepcion); break;
                        case 10: filteredMembers = filteredMembers.OrderBy(o => o.TotalPagar); break;
                    }
                }
                else
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderByDescending(o => Convert.ToDecimal(o.liq_Id)); break;
                        case 1: filteredMembers = filteredMembers.OrderByDescending(o => o.ventaId); break;
                        case 2: filteredMembers = filteredMembers.OrderByDescending(o => Convert.ToDateTime(o.liq_Fecha)).ThenByDescending(to => Convert.ToDecimal(to.liq_Id)); break;
                        case 3: filteredMembers = filteredMembers.OrderByDescending(o => o.Pares); break;
                        case 4: filteredMembers = filteredMembers.OrderByDescending(o => o.Estado); break;
                        case 5: filteredMembers = filteredMembers.OrderByDescending(o => o.Ganancia); break;
                        case 6: filteredMembers = filteredMembers.OrderByDescending(o => o.Subtotal); break;
                        case 7: filteredMembers = filteredMembers.OrderByDescending(o => o.N_C); break;
                        case 8: filteredMembers = filteredMembers.OrderByDescending(o => o.Total); break;
                        case 9: filteredMembers = filteredMembers.OrderByDescending(o => o.Percepcion); break;
                        case 10: filteredMembers = filteredMembers.OrderByDescending(o => o.TotalPagar); break;
                    }
                }
            }
            var displayMembers = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);
            var result = from a in displayMembers
                         select new
                         {
                             a.liq_Id,
                             a.ventaId,
                             a.liq_Fecha,
                             a.Pares,
                             a.Estado,
                             a.Ganancia,
                             a.Subtotal,
                             a.N_C,
                             a.Total,
                             a.Percepcion,
                             a.TotalPagar,
                             a.ped_Id,
                             a.cust_Id,
                             a.estId,
                             a.liq_opg,
                             a.liq_tipo_prov,
                             a.liq_tipo_des,
                             a.liq_agencia,
                             a.liq_agencia_direccion,
                             a.liq_destino,
                             a.liq_direccion,
                             a.liq_referencia,

                         };
            //Se devuelven los resultados por json
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalCount,
                iTotalDisplayRecords = filteredMembers.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        /** Lista Nota de cRedito **/
        public ActionResult getListNotaCredito(Ent_jQueryDataTableParams param)
        {
            /*verificar si esta null*/
            if (Session[_session_list_NotaCredito] == null)
            {
                List<Ent_NotaCredito> listPed = new List<Ent_NotaCredito>();
                Session[_session_list_NotaCredito] = listPed;
            }

            //Traer registros
            IQueryable<Ent_NotaCredito> membercol = ((List<Ent_NotaCredito>)(Session[_session_list_NotaCredito])).AsQueryable();  //lista().AsQueryable();

            //Manejador de filtros
            int totalCount = membercol.Count();
            IEnumerable<Ent_NotaCredito> filteredMembers = membercol;

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = membercol
                    .Where(m => m.Not_Numero.ToUpper().Contains(param.sSearch.ToUpper()));
            }
            //Manejador de orden
            var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);

            if (param.iSortingCols > 0)
            {
                if (Request["sSortDir_0"].ToString() == "asc")
                {
                    switch (sortIdx)
                    {
                        case 1: filteredMembers = filteredMembers.OrderBy(o => Convert.ToDateTime(o.Not_Fecha)); break;
                    }
                }
                else
                {
                    switch (sortIdx)
                    {
                        case 1: filteredMembers = filteredMembers.OrderByDescending(o => Convert.ToDateTime(o.Not_Fecha)); break;
                    }
                }
            }
            var displayMembers = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);
            var result = from a in displayMembers
                         select new
                         {
                             a.Not_Numero,
                             a.Not_Fecha,
                             a.Not_Det_Cantidad,
                             a.Total,                             
                         };
            //Se devuelven los resultados por json
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalCount,
                iTotalDisplayRecords = filteredMembers.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        /** Lista Consignaciones **/
        public ActionResult getListConsignaciones(Ent_jQueryDataTableParams param)
        {
            /*verificar si esta null*/
            if (Session[_session_list_consignaciones] == null)
            {
                List<Ent_Consignacioes> listPed = new List<Ent_Consignacioes>();
                Session[_session_list_consignaciones] = listPed;
            }

            //Traer registros
            IQueryable<Ent_Consignacioes> membercol = ((List<Ent_Consignacioes>)(Session[_session_list_consignaciones])).AsQueryable();  //lista().AsQueryable();

            //Manejador de filtros
            int totalCount = membercol.Count();
            IEnumerable<Ent_Consignacioes> filteredMembers = membercol;

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = membercol
                    .Where(m => m.Pag_Num_Consignacion.ToUpper().Contains(param.sSearch.ToUpper()));
            }
            //Manejador de orden
            var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);

            if (param.iSortingCols > 0)
            {
                if (Request["sSortDir_0"].ToString() == "asc")
                {
                    switch (sortIdx)
                    {
                        case 2: filteredMembers = filteredMembers.OrderBy(o => o.Pag_Monto); break;
                        case 3: filteredMembers = filteredMembers.OrderBy(o => Convert.ToDateTime(o.Pag_Num_ConsFecha)); break;
                    }
                }
                else
                {
                    switch (sortIdx)
                    {
                        case 2: filteredMembers = filteredMembers.OrderByDescending(o => o.Pag_Monto); break;
                        case 3: filteredMembers = filteredMembers.OrderByDescending(o => Convert.ToDateTime(o.Pag_Num_ConsFecha)); break;
                    }
                }
            }
            var displayMembers = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);
            var result = from a in displayMembers
                         select new
                         {
                             a.Ban_Descripcion,
                             a.Pag_Num_Consignacion,
                             a.Pag_Monto,
                             a.Pag_Num_ConsFecha,
                         };
            //Se devuelven los resultados por json
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalCount,
                iTotalDisplayRecords = filteredMembers.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        /** Lista Saldos **/
        public ActionResult getListSaldos(Ent_jQueryDataTableParams param)
        {
            /*verificar si esta null*/
            if (Session[_session_list_saldos] == null)
            {
                List<Ent_Saldos> listPed = new List<Ent_Saldos>();
                Session[_session_list_saldos] = listPed;
            }

            //Traer registros
            IQueryable<Ent_Saldos> membercol = ((List<Ent_Saldos>)(Session[_session_list_saldos])).AsQueryable();  //lista().AsQueryable();

            //Manejador de filtros
            int totalCount = membercol.Count();
            IEnumerable<Ent_Saldos> filteredMembers = membercol;

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = membercol
                    .Where(m => m.Descipcion.ToUpper().Contains(param.sSearch.ToUpper()));
            }
            //Manejador de orden
            var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);

            if (param.iSortingCols > 0)
            {
                if (Request["sSortDir_0"].ToString() == "asc")
                {
                    switch (sortIdx)
                    {
                        case 1: filteredMembers = filteredMembers.OrderBy(o => o.Monto); break;
                    }
                }
                else
                {
                    switch (sortIdx)
                    {
                        case 1: filteredMembers = filteredMembers.OrderByDescending(o => o.Monto); break;
                    }
                }
            }
            var displayMembers = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);
            var result = from a in displayMembers
                         select new
                         {
                             a.Descipcion,
                             a.Monto,
                             a.Percepcion,
                             a.Saldo,
                         };
            //Se devuelven los resultados por json
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalCount,
                iTotalDisplayRecords = filteredMembers.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult setDetallePedido(List<Ent_Nueva_Linea_Talla> listTallasCantidad = null, bool editar = false, string tipoPago = "005" )
        {
            try
            {
                if (listTallasCantidad == null)
                {
                    return Json(new { estado = 0  , mensaje ="Nada para agregar a la lista."});
                }               
                //Ent_Order_Dtl nuevaLinea = new Ent_Order_Dtl();
                Ent_Articulo_pedido articulo = (Ent_Articulo_pedido)(Session[_session_nuevo_item_pedido]);
                foreach (Ent_Nueva_Linea_Talla item in listTallasCantidad)
                {

                    
                    List<Tran_Ofertas> listOfertas = new List<Tran_Ofertas>();
                    int _idOfe = 0;
                    if (!editar) {
                        /* Session con las ofertas */
                        if (Session[_session_Tran_Ofertas] != null)
                        {
                            listOfertas = (List<Tran_Ofertas>)Session[_session_Tran_Ofertas];
                            _idOfe = listOfertas.Max(m => m.id);
                        }
                        List<Tran_Ofertas> list = new List<Tran_Ofertas>();
                        list = (from Ent_Articulo_Ofertas art in articulo._ofertas
                                select new Tran_Ofertas()
                                {
                                    id = _idOfe + 1,
                                    idArt = articulo.Art_id,
                                    ofe_id = art.Ofe_Id,
                                    max_pares = art.Ofe_MaxPares,
                                    ofe_porc = art.Ofe_Porc,
                                    ofe_tipo = art.Ofe_Tipo,
                                    ofe_artventa = art.Ofe_ArtVenta,
                                    ofe_prioridad = art.Ofe_Prioridad,
                                }).ToList();
                        listOfertas = listOfertas.Union(list).ToList();

                        Session[_session_Tran_Ofertas] = listOfertas;
                        /* Session con las ofertas */
                    }else
                    {
                        if (item.CantTalla == 0)
                        {
                            List<Ent_Order_Dtl> _listActual = (List<Ent_Order_Dtl>)Session[_session_list_detalle_pedido];
                            _listActual.Remove(_listActual.Where(w => w._code == articulo.Art_id && w._size == item.codTalla).FirstOrDefault());
                            continue;
                        }
                    }

                    //line = newLine;
                    Ent_Order_Dtl _nuevo = new Ent_Order_Dtl() {
                        _code = articulo.Art_id,
                        _artName = articulo.Art_Descripcion,
                        _brand = articulo.Mar_Descripcion,
                        //_brandImg = dr["brv_image"].ToString(),
                        _color = articulo.Col_Descripcion,
                        _majorCat = articulo.Cat_Pri_Descripcion,
                        _cat = articulo.Cat_Descripcion,
                        _subcat = articulo.Sca_Descripcion,
                        //_origin = dr["arv_origin"].ToString(),
                        //_originDesc = dr["arv_origin"].ToString().Equals(Constants.IdOriginImported) ? "Artículo importado" : "Artículo nacional",
                        _comm = (int)articulo.Art_Comision,
                        _uriPhoto = articulo.Art_Foto,
                        _price = articulo.Art_Pre_Sin_Igv,
                        //_priceDesc = Convert.ToDecimal(dr["Art_Pre_Sin_Igv"]).ToString(currency),
                        //_dsctoDesc = order._dscto.ToString(currency),
                        _priceigv = articulo.Art_Pre_Con_Igv,
                        //_priceigvDesc = Convert.ToDecimal(dr["Art_Pre_Con_Igv"]).ToString(currency),
                        _ap_percepcion = articulo.Afec_Percepcion.ToString(),
                        _ofe_Tipo = articulo.Ofe_Tipo,
                        _ofe_PrecioPack = articulo.Ofe_ArtVenta,
                        _ofe_id = articulo.Ofe_Id,
                        _ofe_maxpares = articulo.Ofe_MaxPares,
                        _ofe_porc = articulo.Ofe_Porc,
                        _premio = "",
                        _premId = "",
                        _premioDesc = "",
                        id_tran_ofe = _idOfe + 1
                        //nroProms = dtArt.Rows.Count,
                    };
                    _nuevo._size = item.codTalla;
                    _addArticle(_nuevo, item.CantTalla, tipoPago, editar);                    
                }
                return Json(new { estado = 1  });
            }
            catch (Exception ex)
            {
                return Json(new { estado = 0 , mensaje = ex.Message });
            }           
        }
        public void fupdateitemoferta()
        {
            List<Ent_Order_Dtl> orderLines = (List<Ent_Order_Dtl>)(((object)Session[_session_list_detalle_pedido]) != null ? (object)Session[_session_list_detalle_pedido] : new List<Ent_Order_Dtl>()); ;

            orderLines = pre_promocion(orderLines);

            try
            {
                Ent_Persona cust = (Ent_Persona)Session[_session_customer];
                string varTipoPago = cust._vartipopago;
                // se agregó varPagoxOPG.06-06-19
                if (orderLines != null && varTipoPago != "008" && varTipoPago != "OPG")
                {

                    /*formatear las promociones*/
                    for (Int32 c = 0; c < orderLines.Count; ++c)
                    {
                        //verificar las promociones
                        if (orderLines[c]._ofe_id != 0)
                        {
                            orderLines[c]._dscto = 0;
                           // orderLines[c]._dsctoDesc = orderLines[c]._dscto.ToString(_currency);

                            orderLines[c]._lineTotal = Math.Round((orderLines[c]._qty * orderLines[c]._price) - (orderLines[c]._commission) - (orderLines[c]._dscto), 2, MidpointRounding.AwayFromZero);
                            orderLines[c]._lineTotDesc = orderLines[c]._lineTotal;
                           // orderLines[c]._lineTotDesc = orderLines[c]._lineTotal.ToString(_currency);
                        }
                    }
                    /********************/

                    /*si es que tiee oferta POR PORCENTAJE entonces vamos a filtrar*/
                    List<Ent_Order_Dtl> orderLinesOferta_filter = orderLines.Where(c => c._ofe_id != 0 && c._ofe_Tipo == "N").ToList();

                    List<Ent_Order_Dtl> orderLinesOferta_filterPack = orderLines.Where(c => c._ofe_id != 0 && c._ofe_Tipo == "P").ToList();

                    /*si es que tiee oferta POR PORCENTAJE entonces vamos a filtrar*/
                    List<Ent_Order_Dtl> orderLinesOferta_filterCombo = orderLines.Where(c => c._ofe_id != 0 && c._ofe_Tipo == "C").ToList();

                    List<Ent_Order_Dtl> orderLinesOferta_filterMochila = orderLines.Where(c => c._ofe_id != 0 && c._ofe_Tipo == "M").ToList();


                    /*INICIO DE LAS OFERTAS DESCUENTO POR PORCENTAJE*/

                    if (orderLinesOferta_filter.Count > 0)
                    {
                        //var grupo_oferta = orderLines.GroupBy(c => c._ofe_id != 0).ToList();
                        var lista_gr = from item in orderLines
                                       where item._ofe_id != 0
                                       group item by
                                       new
                                       {
                                           ofertaid = item._ofe_id,
                                           ofemaxpar = item._ofe_maxpares,
                                           oferporc = item._ofe_porc,
                                       } into g
                                       select new
                                       {
                                           ofertaid = g.Key.ofertaid,
                                           ofemaxpar = g.Key.ofemaxpar,
                                           oferporc = g.Key.oferporc,

                                       };
                        foreach (var it in lista_gr)
                        {



                            /*capturamos el maximo de pares y por descuento*/
                            Decimal _max_pares = it.ofemaxpar;//  orderLinesOferta_filter[0]._ofe_maxpares;
                            Decimal _por_desc = it.oferporc / 100 /* orderLinesOferta_filter[0]._ofe_porc/100*/;



                            Decimal _total = orderLinesOferta_filter.Where(r => r._ofe_id == it.ofertaid).Sum(x => x._qty);

                            /*ahora capturado el total de pares le hacemos un for para */

                            decimal _res = _total / _max_pares;
                            //if (_max_pares == 1)
                            //{
                            //    _res = 1;
                            //}
                            /*para saber si es un entero es true si no es false decimal*/
                            bool isInt = (int)_res == _res;

                            DataTable dt = new DataTable();
                            dt.Columns.Add("articulo", typeof(string));
                            dt.Columns.Add("talla", typeof(string));
                            dt.Columns.Add("precio", typeof(Decimal));
                            dt.Columns.Add("cantidad", typeof(Decimal));
                            dt.Columns.Add("porc_comision", typeof(Decimal));
                            dt.Columns.Add("descuento", typeof(Decimal));
                            dt.Columns.Add("oferta", typeof(string));


                            foreach (var filas in orderLinesOferta_filter.Where(r => r._ofe_id == it.ofertaid).ToList())
                            {
                                for (Int32 c = 0; c < filas._qty; ++c)
                                {
                                    dt.Rows.Add(filas._code.ToString(),
                                         filas._size.ToString(),
                                         filas._price,
                                         1,
                                         filas._commissionPctg,
                                         0, filas._ofe_id.ToString());


                                }
                            }

                            if (!isInt)
                                _res = Convert.ToInt32((_res) - Convert.ToDecimal(0.1));


                            if (_res != 0)
                            {
                                DataRow[] _filas = dt.Select("len(articulo)>0 and oferta='" + it.ofertaid.ToString() + "'", "precio asc");
                                if (_filas.Length > 0)
                                {
                                    if (_por_desc == 1)
                                    {
                                        _por_desc = 0.5M;
                                        _res = 2;
                                    }

                                    Decimal _des_oferta = 0;
                                    for (Int32 i = 0; i < _res; ++i)
                                    {
                                        string _articulo = _filas[i]["articulo"].ToString();
                                        string _talla = _filas[i]["talla"].ToString();
                                        Decimal _precio = Convert.ToDecimal(_filas[i]["precio"]);
                                        Decimal _com_porc = Convert.ToDecimal(_filas[i]["porc_comision"]);
                                        Decimal _cant = Convert.ToDecimal(_filas[i]["cantidad"]);
                                        decimal _com_mon = Math.Round((_precio * _cant) * _com_porc, 2, MidpointRounding.AwayFromZero);

                                        if (i == 0 & _max_pares > 1)
                                        {
                                            _des_oferta = Math.Round(((_precio * _cant) - _com_mon) * (_por_desc), 2, MidpointRounding.AwayFromZero);
                                        }

                                        if (_max_pares == 1)
                                        {
                                            _des_oferta = Math.Round(((_precio * _cant) - _com_mon) * (_por_desc), 2, MidpointRounding.AwayFromZero);
                                        }

                                        _filas[i]["descuento"] = _des_oferta;
                                    }

                                    for (Int32 i = 0; i < orderLines.Count; ++i)
                                    {
                                        string _articulo = orderLines[i]._code.ToString();
                                        string _talla = orderLines[i]._size.ToString();
                                        string _oferta_id = orderLines[i]._ofe_id.ToString();
                                        foreach (DataRow vfila in _filas)
                                        {
                                            if (_articulo == vfila["articulo"].ToString() && _talla == vfila["talla"].ToString() && _oferta_id == vfila["oferta"].ToString())
                                            {
                                                orderLines[i]._dscto += Convert.ToDecimal(vfila["descuento"]);
                                                //orderLines[i]._dsctoDesc = orderLines[i]._dscto.ToString(_currency);

                                                orderLines[i]._lineTotal = Math.Round((orderLines[i]._qty * orderLines[i]._price) - (orderLines[i]._commission) - (orderLines[i]._dscto), 2, MidpointRounding.AwayFromZero);
                                                orderLines[i]._lineTotDesc = orderLines[i]._lineTotal;
                                                //orderLines[i]._lineTotDesc = orderLines[i]._lineTotal.ToString(_currency);
                                            }
                                        }
                                    }
                                }/******/
                            }
                        }

                    }

                    /*FIN DE LAS OFERTAS DESCUENTO POR PORCENTAJE*/

                    /*INICIO DE LAS OFERTAS PRECIO POR PACK*/

                    if (orderLinesOferta_filterPack.Count > 0)
                    {

                        //var grupo_oferta = orderLines.GroupBy(c => c._ofe_id != 0).ToList();
                        var lista_gr = from item in orderLines
                                       where item._ofe_id != 0
                                       group item by
                                       new
                                       {
                                           ofertaid = item._ofe_id,
                                           ofemaxpar = item._ofe_maxpares,
                                           _ofe_PrecioPack = item._ofe_PrecioPack,
                                       } into g
                                       select new
                                       {
                                           ofertaid = g.Key.ofertaid,
                                           ofemaxpar = g.Key.ofemaxpar,
                                           _ofe_PrecioPack = g.Key._ofe_PrecioPack,

                                       };
                        foreach (var it in lista_gr)
                        {



                            /*capturamos el maximo de pares y por descuento*/
                            Decimal _max_pares = it.ofemaxpar;//  orderLinesOferta_filter[0]._ofe_maxpares;
                            Decimal _por_desc = it._ofe_PrecioPack /* orderLinesOferta_filter[0]._ofe_porc/100*/;

                            Decimal _ofe_PrecioPack = it._ofe_PrecioPack;


                            Decimal _total = orderLinesOferta_filterPack.Where(r => r._ofe_id == it.ofertaid).Sum(x => x._qty);

                            /*ahora capturado el total de pares le hacemos un for para */

                            decimal _res = _total / _max_pares;
                            /*para saber si es un entero es true si no es false decimal*/
                            bool isInt = (int)_res == _res;

                            DataTable dt = new DataTable();
                            dt.Columns.Add("articulo", typeof(string));
                            dt.Columns.Add("talla", typeof(string));
                            dt.Columns.Add("precio", typeof(Decimal));
                            dt.Columns.Add("cantidad", typeof(Decimal));
                            dt.Columns.Add("porc_comision", typeof(Decimal));
                            dt.Columns.Add("descuento", typeof(Decimal));
                            dt.Columns.Add("oferta", typeof(string));


                            foreach (var filas in orderLinesOferta_filterPack.Where(r => r._ofe_id == it.ofertaid).ToList())
                            {
                                for (Int32 c = 0; c < filas._qty; ++c)
                                {
                                    dt.Rows.Add(filas._code.ToString(),
                                         filas._size.ToString(),
                                         filas._price,
                                         1,
                                         filas._commissionPctg,
                                         0, filas._ofe_id.ToString());


                                }
                            }

                            if (!isInt)
                                _res = Convert.ToInt32((_res) - Convert.ToDecimal(0.1));


                            if (_res != 0)
                            {
                                DataRow[] _filas = dt.Select("len(articulo)>0 and oferta='" + it.ofertaid.ToString() + "'", "precio asc");
                                if (_filas.Length > 0)
                                {
                                    if (_por_desc == 1)
                                    {
                                        _por_desc = 0.5M;
                                        _res = 2;
                                    }

                                    Decimal _des_oferta = 0;
                                    Decimal cantColec = 0;
                                    Decimal montoColec = 0;
                                    Decimal residuoCant = 0;
                                    Decimal montoColecResiduo = 0;

                                    int Colect = 0;
                                    for (Int32 i = 0; i < _filas.Length; ++i)
                                    {
                                        Colect++;

                                        string _articulo = _filas[i]["articulo"].ToString();
                                        string _talla = _filas[i]["talla"].ToString();
                                        Decimal _precio = Convert.ToDecimal(_filas[i]["precio"]);
                                        Decimal _com_porc = Convert.ToDecimal(_filas[i]["porc_comision"]);
                                        Decimal _cant = Convert.ToDecimal(_filas[i]["cantidad"]);
                                        cantColec += _cant;

                                        if (cantColec > _max_pares)
                                        {

                                            decimal _tPremios = cantColec / _max_pares;
                                            residuoCant = cantColec - (_max_pares * _tPremios);
                                            _cant = _max_pares * _tPremios;
                                            decimal _com_mon_res = Math.Round((_precio * residuoCant) * _com_porc, 2, MidpointRounding.AwayFromZero);
                                            montoColecResiduo = (_precio * residuoCant) - _com_mon_res;
                                        }

                                        decimal _com_mon = Math.Round((_precio * _cant) * _com_porc, 2, MidpointRounding.AwayFromZero);


                                        montoColec += (_precio * _cant) - _com_mon;

                                        if (cantColec >= _max_pares)
                                        {

                                            Decimal TotalDesc = montoColec - _ofe_PrecioPack;

                                            for (Int32 j = 0; j < Colect; ++j)
                                            {

                                                Decimal _preciopk = Convert.ToDecimal(_filas[i - j]["precio"]);
                                                Decimal _cantpk = Convert.ToDecimal(_filas[i - j]["cantidad"]);

                                                if (_cantpk > _max_pares)
                                                    _cantpk = _cant;

                                                Decimal _com_porcpk = Convert.ToDecimal(_filas[i - j]["porc_comision"]);
                                                decimal _com_monpk = Math.Round((_preciopk * _cantpk) * _com_porcpk, 2, MidpointRounding.AwayFromZero);
                                                Decimal totallineapk = _preciopk - _com_monpk;
                                                _des_oferta = Math.Round((totallineapk / montoColec) * TotalDesc, 2, MidpointRounding.AwayFromZero);

                                                //Decimal descuento = Convert.ToDecimal(_filas[i - j]["descuento"]);
                                                _filas[i - j]["descuento"] = _des_oferta;
                                            }

                                            cantColec = 0;
                                            montoColec = 0;
                                            Colect = 0;

                                        }


                                    }

                                    for (Int32 i = 0; i < orderLines.Count; ++i)
                                    {
                                        string _articulo = orderLines[i]._code.ToString();
                                        string _talla = orderLines[i]._size.ToString();
                                        string _oferta_id = orderLines[i]._ofe_id.ToString();
                                        foreach (DataRow vfila in _filas)
                                        {
                                            if (_articulo == vfila["articulo"].ToString() && _talla == vfila["talla"].ToString() && _oferta_id == vfila["oferta"].ToString())
                                            {
                                                orderLines[i]._dscto += Convert.ToDecimal(vfila["descuento"]);
                                                //orderLines[i]._dsctoDesc = orderLines[i]._dscto.ToString(_currency);

                                                orderLines[i]._lineTotal = Math.Round((orderLines[i]._qty * orderLines[i]._price) - (orderLines[i]._commission) - (orderLines[i]._dscto), 2, MidpointRounding.AwayFromZero);
                                                //orderLines[i]._lineTotDesc = orderLines[i]._lineTotal.ToString(_currency);
                                            }
                                        }
                                    }
                                }
                            }/****/
                        }

                    }


                    /*FIN DE LAS OFERTAS PRECIO POR PACK*/


                    #region Ofertas combos

                    if (orderLinesOferta_filterCombo.Count > 0)
                    {

                        //var grupo_oferta = orderLines.GroupBy(c => c._ofe_id != 0).ToList();
                        var lista_gr = from item in orderLines
                                       where item._ofe_id != 0
                                       group item by
                                       new
                                       {
                                           ofertaid = item._ofe_id,
                                           ofemaxpar = item._ofe_maxpares,
                                           _ofe_PrecioPack = item._ofe_PrecioPack,
                                           // _grupo = item._ofe_porc
                                       } into g
                                       select new
                                       {
                                           ofertaid = g.Key.ofertaid,
                                           ofemaxpar = g.Key.ofemaxpar,
                                           _ofe_PrecioPack = g.Key._ofe_PrecioPack,
                                           //   _grupo = g.Key._grupo    
                                       };
                        foreach (var it in lista_gr)
                        {
                            /*capturamos el maximo de pares y por descuento*/
                            Decimal _max_pares = it.ofemaxpar;//  orderLinesOferta_filter[0]._ofe_maxpares;
                            Decimal _por_desc = it._ofe_PrecioPack /* orderLinesOferta_filter[0]._ofe_porc/100*/;

                            Decimal _ofe_PrecioPack = it._ofe_PrecioPack;

                            Decimal _total1 = orderLinesOferta_filterCombo.Where(r => r._ofe_id == it.ofertaid && r._ofe_porc == 1).Sum(x => x._qty);
                            Decimal _total2 = orderLinesOferta_filterCombo.Where(r => r._ofe_id == it.ofertaid && r._ofe_porc == 2).Sum(x => x._qty);
                            Decimal _total = (new decimal[] { _total1, _total2 }).Min();

                            /*ahora capturado el total de pares le hacemos un for para */

                            decimal _res = _total / _max_pares;
                            /*para saber si es un entero es true si no es false decimal*/

                            if (_total > 0)
                            {
                                DataTable dt = new DataTable();

                                dt.Columns.Add("articulo", typeof(string));
                                dt.Columns.Add("talla", typeof(string));
                                dt.Columns.Add("precio", typeof(Decimal));
                                dt.Columns.Add("cantidad", typeof(Decimal));
                                dt.Columns.Add("porc_comision", typeof(Decimal));
                                dt.Columns.Add("descuento", typeof(Decimal));
                                dt.Columns.Add("oferta", typeof(string));
                                dt.Columns.Add("grupo", typeof(string));
                                dt.Columns.Add("promo", typeof(bool));
                                dt.Columns.Add("index", typeof(int));

                                int _ind = 0;
                                foreach (var filas in orderLinesOferta_filterCombo.Where(r => r._ofe_id == it.ofertaid).ToList())
                                {
                                    for (Int32 c = 0; c < filas._qty; ++c)
                                    {
                                        _ind++;
                                        dt.Rows.Add(filas._code.ToString(),
                                                filas._size.ToString(),
                                                filas._price,
                                                1,
                                                filas._commissionPctg,
                                                0, filas._ofe_id.ToString(), filas._ofe_porc, false, _ind);
                                    }
                                }

                                for (int k = 0; k < _total; k++)
                                {
                                    DataRow[] _filas = new DataRow[] { dt.Select("len(articulo)>0 and grupo=1.0000 and promo = 0", "").FirstOrDefault(),
                                        dt.Select("len(articulo)>0 and grupo=2.0 and promo = 0", "").FirstOrDefault() };

                                    if (_filas.Length > 0)
                                    {
                                        if (_por_desc == 1)
                                        {
                                            _por_desc = 0.5M;
                                            _res = 2;
                                        }

                                        Decimal _des_oferta = 0;
                                        Decimal cantColec = 0;
                                        Decimal montoColec = 0;
                                        Decimal residuoCant = 0;
                                        Decimal montoColecResiduo = 0;

                                        int Colect = 0;
                                        for (Int32 i = 0; i < _filas.Length; ++i)
                                        {
                                            Colect++;

                                            string _articulo = _filas[i]["articulo"].ToString();
                                            string _talla = _filas[i]["talla"].ToString();
                                            Decimal _precio = Convert.ToDecimal(_filas[i]["precio"]);
                                            Decimal _com_porc = Convert.ToDecimal(_filas[i]["porc_comision"]);
                                            Decimal _cant = Convert.ToDecimal(_filas[i]["cantidad"]);
                                            cantColec += _cant;

                                            if (cantColec > _max_pares)
                                            {

                                                decimal _tPremios = cantColec / _max_pares;
                                                residuoCant = cantColec - (_max_pares * _tPremios);
                                                _cant = _max_pares * _tPremios;
                                                decimal _com_mon_res = Math.Round((_precio * residuoCant) * _com_porc, 2, MidpointRounding.AwayFromZero);
                                                montoColecResiduo = (_precio * residuoCant) - _com_mon_res;
                                            }

                                            decimal _com_mon = Math.Round((_precio * _cant) * _com_porc, 2, MidpointRounding.AwayFromZero);


                                            montoColec += (_precio * _cant) - _com_mon;

                                            if (cantColec >= _max_pares)
                                            {

                                                Decimal TotalDesc = montoColec - _ofe_PrecioPack;

                                                for (Int32 j = 0; j < Colect; ++j)
                                                {

                                                    Decimal _preciopk = Convert.ToDecimal(_filas[i - j]["precio"]);
                                                    Decimal _cantpk = Convert.ToDecimal(_filas[i - j]["cantidad"]);

                                                    if (_cantpk > _max_pares)
                                                        _cantpk = _cant;

                                                    Decimal _com_porcpk = Convert.ToDecimal(_filas[i - j]["porc_comision"]);
                                                    decimal _com_monpk = Math.Round((_preciopk * _cantpk) * _com_porcpk, 2, MidpointRounding.AwayFromZero);
                                                    Decimal totallineapk = _preciopk - _com_monpk;
                                                    _des_oferta = Math.Round((totallineapk / montoColec) * TotalDesc, 2, MidpointRounding.AwayFromZero);

                                                    //Decimal descuento = Convert.ToDecimal(_filas[i - j]["descuento"]);
                                                    _filas[i - j]["descuento"] = _des_oferta;
                                                }

                                                cantColec = 0;
                                                montoColec = 0;
                                                Colect = 0;

                                            }
                                            for (int l = 0; l < dt.Rows.Count; l++)
                                            {
                                                if (Convert.ToInt32(dt.Rows[l]["index"]) == Convert.ToDecimal(_filas[i]["index"]))
                                                {
                                                    dt.Rows[l]["promo"] = true;
                                                }
                                            }

                                        }

                                        for (Int32 i = 0; i < orderLines.Count; ++i)
                                        {
                                            string _articulo = orderLines[i]._code.ToString();
                                            string _talla = orderLines[i]._size.ToString();
                                            string _oferta_id = orderLines[i]._ofe_id.ToString();
                                            foreach (DataRow vfila in _filas)
                                            {
                                                if (_articulo == vfila["articulo"].ToString() && _talla == vfila["talla"].ToString() && _oferta_id == vfila["oferta"].ToString())
                                                {
                                                    orderLines[i]._dscto += Convert.ToDecimal(vfila["descuento"]);
                                                    //orderLines[i]._dsctoDesc = orderLines[i]._dscto.ToString(_currency);

                                                    orderLines[i]._lineTotal = Math.Round((orderLines[i]._qty * orderLines[i]._price) - (orderLines[i]._commission) - (orderLines[i]._dscto), 2, MidpointRounding.AwayFromZero);
                                                    //orderLines[i]._lineTotDesc = orderLines[i]._lineTotal.ToString(_currency);
                                                }
                                            }
                                        }
                                    }
                                }
                            }                          
                        }
                    }

                    #endregion

                    #region Mochila
                    if (orderLinesOferta_filterMochila.Count > 0)
                    {

                        //var grupo_oferta = orderLines.GroupBy(c => c._ofe_id != 0).ToList();
                        var lista_gr = from item in orderLines
                                       where item._ofe_id != 0
                                       group item by
                                       new
                                       {
                                           ofertaid = item._ofe_id,
                                           ofemaxpar = item._ofe_maxpares,
                                           //oferporc = item._ofe_porc,
                                       } into g
                                       select new
                                       {
                                           ofertaid = g.Key.ofertaid,
                                           ofemaxpar = g.Key.ofemaxpar,
                                           //oferporc = g.Key.oferporc,

                                       };
                        foreach (var it in lista_gr)
                        {
                            /*capturamos el maximo de pares y por descuento*/
                            Decimal _max_pares = it.ofemaxpar;//  orderLinesOferta_filter[0]._ofe_maxpares;
                            //Decimal _por_desc = it.oferporc / 100 /* orderLinesOferta_filter[0]._ofe_porc/100*/;

                            //Decimal _ofe_PrecioPack = it._ofe_PrecioPack;

                            Decimal _total1 = orderLinesOferta_filterMochila.Where(r => r._ofe_id == it.ofertaid && r._ofe_PrecioPack == 1).Sum(x => x._qty);
                            Decimal _total2 = orderLinesOferta_filterMochila.Where(r => r._ofe_id == it.ofertaid && r._ofe_PrecioPack == 2).Sum(x => x._qty);
                            Decimal _total = (new decimal[] { _total1, _total2 }).Min();

                            /*ahora capturado el total de pares le hacemos un for para */

                            decimal _res = _total / _max_pares;
                            /*para saber si es un entero es true si no es false decimal*/

                            if (_total > 0)
                            {
                                DataTable dt = new DataTable();

                                dt.Columns.Add("articulo", typeof(string));
                                dt.Columns.Add("talla", typeof(string));
                                dt.Columns.Add("precio", typeof(Decimal));
                                dt.Columns.Add("cantidad", typeof(Decimal));
                                dt.Columns.Add("porc_comision", typeof(Decimal));
                                dt.Columns.Add("descuento", typeof(Decimal));
                                dt.Columns.Add("oferta", typeof(string));
                                dt.Columns.Add("grupo", typeof(string));
                                dt.Columns.Add("promo", typeof(bool));
                                dt.Columns.Add("index", typeof(int));
                                dt.Columns.Add("porc_desc", typeof(Decimal));

                                int _ind = 0;
                                foreach (var filas in orderLinesOferta_filterMochila.Where(r => r._ofe_id == it.ofertaid).ToList())
                                {
                                    for (Int32 c = 0; c < filas._qty; ++c)
                                    {
                                        _ind++;
                                        dt.Rows.Add(filas._code.ToString(),
                                                filas._size.ToString(),
                                                filas._price,
                                                1,
                                                filas._commissionPctg,
                                                0, filas._ofe_id.ToString(), filas._ofe_PrecioPack, false, _ind, filas._ofe_porc);
                                    }
                                }

                                for (int k = 0; k < _total; k++)
                                {
                                    DataRow[] _filas = (new DataRow[] { dt.Select("len(articulo)>0 and grupo=1.0000 and promo = 0", "precio asc").FirstOrDefault(),
                                        dt.Select("len(articulo)>0 and grupo=2.0 and promo = 0", "precio asc").FirstOrDefault() }).OrderByDescending(r => r.Field<string>("grupo")).ToArray<DataRow>();

                                    if (_filas.Length > 0)
                                    {
                                        DataRow _dtM = _filas.FirstOrDefault();
                                        Decimal _por_desc = Convert.ToDecimal(_dtM["porc_desc"]) / 100;
                                        if (_por_desc == 1)
                                        {
                                            _por_desc = 0.5M;
                                            _res = 2;
                                        }

                                        Decimal _des_oferta = 0;
                                        for (Int32 i = 0; i < _res; ++i)
                                        {
                                            string _articulo = _filas[i]["articulo"].ToString();
                                            string _talla = _filas[i]["talla"].ToString();
                                            Decimal _precio = Convert.ToDecimal(_filas[i]["precio"]);
                                            Decimal _com_porc = Convert.ToDecimal(_filas[i]["porc_comision"]);
                                            Decimal _cant = Convert.ToDecimal(_filas[i]["cantidad"]);
                                            decimal _com_mon = Math.Round((_precio * _cant) * _com_porc, 2, MidpointRounding.AwayFromZero);

                                            if (i == 0 & _max_pares > 1)
                                            {
                                                _des_oferta = Math.Round(((_precio * _cant) - _com_mon) * (_por_desc), 2, MidpointRounding.AwayFromZero);
                                            }

                                            if (_max_pares == 1)
                                            {
                                                _des_oferta = Math.Round(((_precio * _cant) - _com_mon) * (_por_desc), 2, MidpointRounding.AwayFromZero);
                                            }

                                            _filas[i]["descuento"] = _des_oferta;

                                            for (int l = 0; l < dt.Rows.Count; l++)
                                            {
                                                if (Convert.ToInt32(dt.Rows[l]["index"]) == Convert.ToDecimal(_filas[i]["index"]))
                                                {
                                                    dt.Rows[l]["promo"] = true;
                                                }
                                            }
                                        }

                                        for (Int32 i = 0; i < orderLines.Count; ++i)
                                        {
                                            string _articulo = orderLines[i]._code.ToString();
                                            string _talla = orderLines[i]._size.ToString();
                                            string _oferta_id = orderLines[i]._ofe_id.ToString();
                                            foreach (DataRow vfila in _filas)
                                            {
                                                if (_articulo == vfila["articulo"].ToString() && _talla == vfila["talla"].ToString() && _oferta_id == vfila["oferta"].ToString())
                                                {
                                                    orderLines[i]._dscto += Convert.ToDecimal(vfila["descuento"]);
                                                    //orderLines[i]._dsctoDesc = orderLines[i]._dscto.ToString(_currency);

                                                    orderLines[i]._lineTotal = Math.Round((orderLines[i]._qty * orderLines[i]._price) - (orderLines[i]._commission) - (orderLines[i]._dscto), 2, MidpointRounding.AwayFromZero);
                                                    //orderLines[i]._lineTotDesc = orderLines[i]._lineTotal.ToString(_currency);
                                                }
                                            }
                                        }
                                    }/******/

                                }
                            }
                        }
                    }
                    #endregion




                    //Decimal _max_pares=


                    //for (Int32 i=0;i<orderLines.Count;++i)
                    //{

                    //}
                }

            }
            catch { }            
        }
        public List<Ent_Order_Dtl> pre_promocion(List<Ent_Order_Dtl> orderLines)
        {
            /* promociones */

            List<Tran_Ofertas> listOfertas = new List<Tran_Ofertas>();

            List<int> hechos = new List<int>();
            if (Session[_session_Tran_Ofertas] != null)
            {
                listOfertas = (List<Tran_Ofertas>)Session[_session_Tran_Ofertas];
            }
            listOfertas.Select(a =>
            {
                a.hecho = "";
                return a;
            }).ToList();
            /* get ofertas */
            if (listOfertas.Count == 0 && orderLines.Count > 0)
            {
                int _idOfe = 0;
                foreach (Ent_Order_Dtl item in orderLines)
                {
                    
                    Ent_Persona cust = (Ent_Persona)Session[_session_customer];
                    Ent_Articulo_pedido articulo = new Ent_Articulo_pedido();
                    List<Ent_Articulo_Tallas> tallas = new List<Ent_Articulo_Tallas>();
                    datPedido.listarStr_ArticuloTalla(item._code, Convert.ToDecimal(cust.Bas_id), ref articulo, ref tallas);
                    if (articulo._ofertas.Count> 0)
                    {
                        _idOfe++;
                        List<Tran_Ofertas> list = new List<Tran_Ofertas>();
                        list = (from Ent_Articulo_Ofertas art in articulo._ofertas
                                select new Tran_Ofertas()
                                {
                                    id = _idOfe,
                                    idArt = articulo.Art_id,
                                    ofe_id = art.Ofe_Id,
                                    max_pares = art.Ofe_MaxPares,
                                    ofe_porc = art.Ofe_Porc,
                                    ofe_tipo = art.Ofe_Tipo,
                                    ofe_artventa = art.Ofe_ArtVenta,
                                    ofe_prioridad = art.Ofe_Prioridad,
                                }).ToList();
                        listOfertas = listOfertas.Union(list).ToList();
                        orderLines.Where(w => w._code == item._code && w._size == item._size).Select(s => { s.id_tran_ofe = _idOfe; return s; }).ToList();
                    }
                }               
            }

            listOfertas = listOfertas.Where(w => (orderLines.Select(s => s.id_tran_ofe).Distinct().ToArray()).Contains(w.id)).ToList();
            listOfertas = listOfertas.OrderBy(o => o.ofe_prioridad).ToList();//.OrderBy(a => a.ofe_id).ToList();

            foreach (Tran_Ofertas item in listOfertas)
            {
                decimal ofe_id = item.ofe_id;
                decimal max_pares = item.max_pares; // item.max_pares; //deberia ser la suma de max pares en general de toda la lista por item.ofe_id
                int max_ofe = 0; // (int)(listOfertas.Where(w => w.ofe_id == ofe_id).Count() / 2); // item.max_pares; //deberia ser la suma de max pares en general de toda la lista por item.ofe_id
                decimal _max_pares_valida = 0;
                string ofe_tipo = item.ofe_tipo;
                decimal ofe_grupo = item.ofe_artventa;

                if ((new String[] { "M" }).Contains(ofe_tipo))
                {
                    int gru1 = listOfertas.Count(c => c.ofe_id == ofe_id && c.hecho == "" && c.ofe_artventa == 1 && !hechos.Contains(c.id));
                    int gru2 = listOfertas.Count(c => c.ofe_id == ofe_id && c.hecho == "" && c.ofe_artventa == 2 && !hechos.Contains(c.id));
                    max_ofe = (new int[] { gru1, gru2 }).Min();
                    if (max_ofe >= 1)
                    {
                        for (int i = 0; i < max_ofe; i++)
                        {

                            Tran_Ofertas subItem1 = listOfertas.Where(w => w.hecho == "" && w.ofe_id == ofe_id && w.ofe_artventa == 1 && !hechos.Contains(w.id)).Take(1).FirstOrDefault();
                            Tran_Ofertas subItem2 = listOfertas.Where(w => w.hecho == "" && w.ofe_id == ofe_id && w.ofe_artventa == 2 && !hechos.Contains(w.id)).Take(1).FirstOrDefault();

                            orderLines.Where(w => w.id_tran_ofe == subItem1.id).Select(a =>
                            {
                                a._ofe_id = subItem1.ofe_id;
                                a._ofe_maxpares = subItem1.max_pares;
                                a._ofe_porc = subItem1.ofe_porc;
                                a._ofe_Tipo = subItem1.ofe_tipo;
                                a._ofe_PrecioPack = subItem1.ofe_artventa;
                                return a;
                            }).ToList();
                            listOfertas.Where(w => w.id == subItem1.id).Select(a =>
                            {
                                a.hecho = "x";
                                return a;
                            }).ToList();
                            hechos.Add(subItem1.id);

                            orderLines.Where(w => w.id_tran_ofe == subItem2.id).Select(a =>
                            {
                                a._ofe_id = subItem2.ofe_id;
                                a._ofe_maxpares = subItem2.max_pares;
                                a._ofe_porc = subItem2.ofe_porc;
                                a._ofe_Tipo = subItem2.ofe_tipo;
                                a._ofe_PrecioPack = subItem2.ofe_artventa;
                                return a;
                            }).ToList();
                            listOfertas.Where(w => w.id == subItem2.id).Select(a =>
                            {
                                a.hecho = "x";
                                return a;
                            }).ToList();
                            hechos.Add(subItem2.id);
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                else if ((new String[] { "C" }).Contains(ofe_tipo))
                {
                    int gru1 = listOfertas.Count(c => c.ofe_id == ofe_id && c.hecho == "" && c.ofe_porc == 1 && !hechos.Contains(c.id));
                    int gru2 = listOfertas.Count(c => c.ofe_id == ofe_id && c.hecho == "" && c.ofe_porc == 2 && !hechos.Contains(c.id));
                    max_ofe = (new int[] { gru1, gru2 }).Min();
                    if (max_ofe >= 1)
                    {
                        for (int i = 0; i < max_ofe; i++)
                        {

                            Tran_Ofertas subItem1 = listOfertas.Where(w => w.hecho == "" && w.ofe_id == ofe_id && w.ofe_porc == 1 && !hechos.Contains(w.id)).Take(1).FirstOrDefault();
                            Tran_Ofertas subItem2 = listOfertas.Where(w => w.hecho == "" && w.ofe_id == ofe_id && w.ofe_porc == 2 && !hechos.Contains(w.id)).Take(1).FirstOrDefault();

                            orderLines.Where(w => w.id_tran_ofe == subItem1.id).Select(a =>
                            {
                                a._ofe_id = subItem1.ofe_id;
                                a._ofe_maxpares = subItem1.max_pares;
                                a._ofe_porc = subItem1.ofe_porc;
                                a._ofe_Tipo = subItem1.ofe_tipo;
                                a._ofe_PrecioPack = subItem1.ofe_artventa;
                                return a;
                            }).ToList();
                            listOfertas.Where(w => w.id == subItem1.id).Select(a =>
                            {
                                a.hecho = "x";
                                return a;
                            }).ToList();
                            hechos.Add(subItem1.id);

                            orderLines.Where(w => w.id_tran_ofe == subItem2.id).Select(a =>
                            {
                                a._ofe_id = subItem2.ofe_id;
                                a._ofe_maxpares = subItem2.max_pares;
                                a._ofe_porc = subItem2.ofe_porc;
                                a._ofe_Tipo = subItem2.ofe_tipo;
                                a._ofe_PrecioPack = subItem2.ofe_artventa;
                                return a;
                            }).ToList();
                            listOfertas.Where(w => w.id == subItem2.id).Select(a =>
                            {
                                a.hecho = "x";
                                return a;
                            }).ToList();
                            hechos.Add(subItem2.id);
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    foreach (Tran_Ofertas subItem in listOfertas.Where(w => w.hecho == "" && w.ofe_id == ofe_id && !hechos.Contains(w.id)))
                    {
                        int _id_linea = subItem.id;
                        orderLines.Where(w => w.id_tran_ofe == _id_linea).Select(a =>
                        {
                            a._ofe_id = subItem.ofe_id;
                            a._ofe_maxpares = subItem.max_pares;
                            a._ofe_porc = subItem.ofe_porc;
                            a._ofe_Tipo = subItem.ofe_tipo;
                            a._ofe_PrecioPack = subItem.ofe_artventa;
                            //a.prom_menor = (a._qty > 1 ? (listOfertas)  : false);
                            return a;
                        }).ToList();
                        listOfertas.Where(w => w.id == _id_linea).Select(a =>
                        {
                            a.hecho = "x";
                            return a;
                        }).ToList();
                    }
                }
            }

            return orderLines;
            /* promociones */
        }
        public void _addArticle( Ent_Order_Dtl newLine, int qty, string varTipoPago , bool editar = false)//, string tipopago)
        {
            decimal commPercent;
            //decimal ofertporcentaje;
            //decimal ofertamaxpares;
            Ent_Persona cust = (Ent_Persona)Session[_session_customer];
            cust._vartipopago = varTipoPago;
            if (varTipoPago == "004")
            {
                commPercent = cust._commission / 100;
            }
            else
            {
                commPercent = cust._commission / 100;
            }


            if (newLine._ap_percepcion == "0")
            {
                commPercent = 0;
            }


            //este quiere decir que tiene oferta
            //if (newLine._ofe_id!=0)
            //{
            //    ofertporcentaje = newLine._ofe_porc;
            //    ofertamaxpares = newLine._ofe_maxpares;
            //}

            //decimal commPercent = (cust._commission / 100);
            int newQty = 0;

            // Lista de pedido
            List<Ent_Order_Dtl> orderLines = (List<Ent_Order_Dtl>)(((object)Session[_session_list_detalle_pedido]) != null ? (object)Session[_session_list_detalle_pedido] : new List<Ent_Order_Dtl>()); ;


            //verificamos que no exista un premio en la lista
            string codArt = newLine._code;
            string tallArt = newLine._size;

            Ent_Order_Dtl resultLinePrem = null;// orderLines.Where(x => x._code.Equals(codArt) && x._premio.Equals("S")).FirstOrDefault();

            //fin de verificacion


            Ent_Order_Dtl resultLine;
            if (resultLinePrem == null)
            {


                if (orderLines != null)
                    resultLine = orderLines.Where(x => x._code.Equals(newLine._code) && x._size.Equals(newLine._size) && x._premio.Equals(newLine._premio)).FirstOrDefault();
                else
                    resultLine = new Ent_Order_Dtl();

                if (resultLine != null)
                {
                    if (orderLines.Remove(resultLine))
                    {
                        if (editar)
                        {
                            if (qty > 0)
                            qty = qty - resultLine._qty;
                        }
                        newQty = resultLine._qty + qty;
                        resultLine._qty = newQty;
                        resultLine._commission = Math.Round((((resultLine._price * newQty) - (resultLine._dscto * newQty)) * commPercent) * resultLine._comm, 2, MidpointRounding.AwayFromZero);
                        resultLine._commissionPctg = commPercent;
                        //resultLine._commissionDesc = resultLine._commission.ToString(_currency);
                        int num = 1;

                        //  resultLine._dsctoDesc = newLine._commission.ToString(_currency);
                        //resultLine._commissionigv = Math.Round((((resultLine._priceigv * newQty) - (resultLine._dscto * newQty)) * commPercent) * resultLine._comm, 2, MidpointRounding.AwayFromZero);
                        //resultLine._commissionigvDesc = resultLine._commissionigv.ToString(_currency);
                        resultLine._lineTotal = Math.Round((resultLine._price * newQty) - (resultLine._dscto * newQty) - resultLine._commission, 2, MidpointRounding.AwayFromZero);
                        //resultLine._lineTotDesc = (num * ((resultLine._price * newQty) - (resultLine._dscto * newQty) - resultLine._commission)).ToString(_currency);
                        // resultLine._lineTotDesc = ((resultLine._priceigv * newQty) - (resultLine._dscto * newQty) - resultLine._commissionigv).ToString(_currency);
                        resultLine._lineTotDesc = resultLine._lineTotal;
                        if (varTipoPago == "008" || varTipoPago == "OPG")
                        {
                            resultLine._lineTotal = (resultLine._price * newQty);
                            resultLine._commissionPctg = 0m;
                            //resultLine._commissionDesc = (0).ToString(_currency);
                            resultLine._commission = 0;
                            resultLine._dscto = 0;
                            //resultLine._lineTotal = 0m;
                            resultLine._lineTotDesc = (0);

                            //resultLine._priceigvDesc = (0).ToString(_currency);

                        }
                        orderLines.Add(resultLine);
                    }
                }
                else
                {
                    newLine._qty = qty;
                    newLine._commission = Math.Round((((newLine._price * qty) - (newLine._dscto * qty)) * commPercent) * newLine._comm, 2, MidpointRounding.AwayFromZero);
                  //  newLine._commissionDesc = newLine._commission.ToString(_currency);
                    //newLine._commissionigv = Math.Round((((newLine._priceigv * qty) - (newLine._dscto * qty)) * commPercent) * newLine._comm, 2, MidpointRounding.AwayFromZero);
                    //newLine._commissionigvDesc = newLine._commissionigv.ToString(_currency);
                    newLine._commissionPctg = commPercent;

                    //newLine._dscto =Math.Round (((newLine._price * qty) - newLine._commission) * ((newLine._ofe_porc / 100)));

                    //newLine._dsctoDesc = (((newLine._price * qty) - newLine._commission)*((newLine._ofe_porc/100))).ToString(_currency);
                    int num2 = 1;

                    newLine._lineTotal = Math.Round((newLine._price * qty) - (newLine._dscto * qty) - newLine._commission, 2, MidpointRounding.AwayFromZero);
                    //newLine._lineTotDesc = (num2 * ((newLine._price * qty) - (newLine._dscto * qty) - newLine._commission)).ToString(_currency);
                    //newLine._lineTotDesc = ((newLine._priceigv * qty) - (newLine._dscto * qty) - newLine._commissionigv).ToString(_currency);
                    newLine._lineTotDesc = newLine._lineTotal;
                    if (varTipoPago == "008" || varTipoPago == "OPG")
                    {
                        newLine._lineTotal = (newLine._price * qty);
                        newLine._commissionPctg = 0m;
                      //  newLine._commissionDesc = (0).ToString(_currency);
                        newLine._commission = 0;
                        newLine._dscto = 0;
                        //newLine._lineTotal = 0m;
                        newLine._lineTotDesc = 0;
                        //newLine._priceigvDesc = (0).ToString(_currency);

                    }
                    orderLines.Add(newLine);
                }

            }

            Session[_session_list_detalle_pedido] = orderLines;
        }
        public void loadOrderDtl(ref List<Ent_Order_Dtl> order, Ent_Order_Dtl newLine, decimal custComm)
        {
            //
            Ent_Order_Dtl resultLine = order.Where(x => x._code.Equals(newLine._code) && x._size.Equals(newLine._size)).FirstOrDefault();
            //
            decimal commPercent = (custComm / 100);

            //if (fvalidaartcatalogo())
            //{
            //    commPercent = 0;
            //}

            //recalcular el descuento de catalogo 
            if (!(newLine == null))
            {
                if (newLine._ap_percepcion == "0")
                {
                    commPercent = 0;
                }
            }
            //***************************


            if (resultLine != null)
            {
                if (order.Remove(resultLine))
                {
                    int newQty = resultLine._qty + newLine._qty;
                    resultLine._qty = newQty;
                    resultLine._commission = Math.Round((((resultLine._price * newQty)) * commPercent) * resultLine._comm, 2, MidpointRounding.AwayFromZero);
                    //resultLine._commissionigv = (((resultLine._priceigv * newQty) - (resultLine._dscto * newQty)) * commPercent) * resultLine._comm;
                    resultLine._commissionPctg = commPercent;
                    //resultLine._commissionDesc = resultLine._commission.ToString(_currency);
                    //resultLine._commissionigvDesc = resultLine._commissionigv.ToString(_currency);
                    //resultLine._lineTotal =Math.Round ((resultLine._price * newQty) - (resultLine._dscto * newQty) - resultLine._commission,2,MidpointRounding.AwayFromZero);

                    resultLine._lineTotal = Math.Round((resultLine._price * newQty) - (resultLine._dscto + resultLine._commission), 2, MidpointRounding.AwayFromZero);

                    //resultLine._lineTotDesc = ((resultLine._price * newQty) - (resultLine._dscto * newQty) - resultLine._commission).ToString(_currency);

                    //resultLine._lineTotDesc = resultLine._lineTotal.ToString(_currency);
                    //resultLine._lineTotDesc = ((resultLine._priceigv * newQty) - (resultLine._dscto * newQty) - resultLine._commissionigv).ToString(_currency);
                    resultLine._lineTotDesc = resultLine._lineTotal;

                    order.Add(resultLine);
                }
            }
            else
            {
                newLine._commission = Math.Round((((newLine._price * newLine._qty)) * commPercent) * newLine._comm,2,MidpointRounding.AwayFromZero);
                // newLine._commissionigv = (((newLine._priceigv * newLine._qty) - (newLine._dscto * newLine._qty)) * commPercent) * newLine._comm;
                //newLine._commissionDesc = newLine._commission.ToString(_currency);
                //newLine._commissionigvDesc = newLine._commissionigv.ToString(_currency);
                newLine._commissionPctg = commPercent;
                //newLine._lineTotal =Math.Round( (newLine._price * newLine._qty) - (newLine._dscto * newLine._qty) - newLine._commission,2,MidpointRounding.AwayFromZero);

                newLine._lineTotal = Math.Round((newLine._price * newLine._qty) - (newLine._dscto + newLine._commission), 2, MidpointRounding.AwayFromZero);

                //newLine._lineTotDesc = newLine._lineTotal.ToString(_currency);

                //newLine._lineTotDesc = ((newLine._price * newLine._qty) - (newLine._dscto * newLine._qty) - newLine._commission).ToString(_currency);
                //newLine._lineTotDesc = ((newLine._priceigv * newLine._qty) - (newLine._dscto * newLine._qty) - newLine._commissionigv).ToString(_currency);
                newLine._lineTotDesc = newLine._lineTotDesc;
                order.Add(newLine);
            }
        }
        public Ent_Order_Hdr getTotals(string vnc= "")
        {
            Ent_Order_Hdr orderHdr;


            try
            {
                // Lista de pedido
                List<Ent_Order_Dtl> order = (List<Ent_Order_Dtl>)(((object)Session[_session_list_detalle_pedido]) != null ? (object)Session[_session_list_detalle_pedido] : new List<Ent_Order_Dtl>());

                if (order != null)
                {
                    Ent_Persona cust = (Ent_Persona)Session[_session_customer];
                    decimal taxRate = (cust._taxRate / 100);
                    int totalQty = order.Sum(q => q._qty);
                    decimal subTotal = Math.Round(order.Sum(x => x._lineTotal), 2, MidpointRounding.AwayFromZero);
                    decimal subTotalDesc = subTotal;
                    decimal taxes = Math.Round((order.Sum(x => x._lineTotal)) * taxRate, 2, MidpointRounding.AwayFromZero);
                    decimal taxesDesc = taxes;

                    if (Session[_session_notas_persona] == null)
                    {
                        List<Ent_Pago_NCredito> _listNC = new List<Ent_Pago_NCredito>();
                        Session[_session_notas_persona] = _listNC;
                    }
                    List<Ent_Pago_NCredito> listNC = new List<Ent_Pago_NCredito>();
                    listNC = (List<Ent_Pago_NCredito>)Session[_session_notas_persona];

                    decimal mtoncredito = listNC.Where(w => w.Consumido ==true).Sum(s=>s.Importe);//  0;//Convert.ToDecimal(vnc);
                    //string mtoncreditodesc = mtoncredito.ToString(_currency);

                    //Session[_valor_subtotal] = subTotal + taxes;

                    decimal grandTotal = (subTotal + taxes) - mtoncredito;

                    //si el paso es mayor a la venta
                    if (grandTotal < 0)
                    {
                        grandTotal = 0;
                    }
                    //

                    //string grandTotalDesc = grandTotal.ToString(_currency);

                   //// cust._mtoimporte = (subTotal + taxes);


                    Boolean aplicap = true;

                    //verificar si estos articulos tiene percepcion 0
                    for (Int32 i = 0; i < order.Count; ++i)
                    {
                        string vaplicap = order[i]._ap_percepcion;
                        if (vaplicap == "0")
                        {
                            aplicap = false;
                            break;
                        }
                    }

                    decimal Percepcionrate = (aplicap) ? cust._percepcion / 100 : 0;

                    //decimal Percepcionrate = (cust._percepcion / 100);

                    decimal percepcion = Math.Round(grandTotal * Percepcionrate, 2, MidpointRounding.AwayFromZero);



                    //string percepciondesc = percepcion.ToString(_currency);

                    decimal mtopercepcion = grandTotal + percepcion;
                    //string mtopercepciondesc = mtopercepcion.ToString(_currency);



                    //variable de percepcion*********
                    //cust._mtopercepcion = percepcion;


                    ////Session[_opcional_percepcion] = percepcion;
                    //*******************************

                    decimal _totalOPG = 0.00m;
                    if (cust._vartipopago == "008" || cust._vartipopago == "OPG")
                    {
                        _totalOPG = grandTotal;
                        subTotalDesc = (0);
                        //grandTotalDesc = (0).ToString(_currency);
                        grandTotal = 0m;
                        percepcion = 0m;
                        //percepciondesc = (0).ToString(_currency);
                        mtopercepcion = 0m;
                        //mtopercepciondesc = (0).ToString(_currency);
                        mtoncredito = 0m;
                        //mtoncreditodesc = (0).ToString(_currency);
                        taxesDesc = (0);
                    }


                    orderHdr = new Ent_Order_Hdr
                    {
                        _qtys = totalQty,
                        _subTotalOPG = _totalOPG,
                        _taxes = taxes,
                        _subTotal = subTotal,
                        _subTotalDesc = subTotalDesc,
                        //_grandTotalDesc = grandTotalDesc,
                        _taxesDesc = taxesDesc,
                        _grandTotal = grandTotal,
                        _percepcion = percepcion,
                        //_percepciondesc = percepciondesc,
                        _mtopercepcion = mtopercepcion,
                        //_mtopercepciondesc = mtopercepciondesc,
                        _mtoncredito = mtoncredito,
                        //_mtoncreditodesc = mtoncreditodesc,
                    };
                }
                else
                    orderHdr = new Ent_Order_Hdr();
            }
            catch
            {
                orderHdr = new Ent_Order_Hdr();
            }

            return orderHdr;

        }        
        /** Lista de detalle de pedidos **/
        public ActionResult eliminarFilePedido(string articulo , string size)
        {
            try
            {
                if (Session[_session_list_detalle_pedido] == null)
                {
                    List<Ent_Order_Dtl> listPed = new List<Ent_Order_Dtl>();
                    Session[_session_list_detalle_pedido] = listPed;
                }
                List<Ent_Order_Dtl> _listActual = (List<Ent_Order_Dtl>)Session[_session_list_detalle_pedido];
                foreach (Ent_Order_Dtl item in _listActual.Where(w => w._code == articulo && size.Split(',').Contains(w._size)).ToList())
                {
                    _listActual.Remove(_listActual.Where(w => w._code == articulo && size.Split(',').Contains(w._size)).FirstOrDefault());
                }               
                return Json(new { estado = 0});
            }
            catch (Exception ex)
            {
                return Json(new { estado = 1, mensaje = "Error al eliminar el articulo.\n" + ex.Message });
            }                     
        }
        public ActionResult getListaDetPedido(Ent_jQueryDataTableParams param)
        {
            Ent_Order_Hdr header = new Ent_Order_Hdr();
            /*verificar si esta null*/
            if (Session[_session_list_detalle_pedido] == null)
            {
                List<Ent_Order_Dtl> listPed = new List<Ent_Order_Dtl>();
                Session[_session_list_detalle_pedido] = listPed;
            } else {
                fupdateitemoferta();
                header = getTotals();
            }
            //Traer registros

            List<Ent_Order_Dtl> pedidoCompleto = (List<Ent_Order_Dtl>)Session[_session_list_detalle_pedido];
            /*
                a._code,
                a._artName,
                a._brand,
                a._color,
                a._size,
                a._qty,
                a._price,
                a._commission,
                a._det_dcto_sigv,
                a._lineTotal,
                a._dscto,
            */

            List<Ent_Order_Dtl> VistaLista =(from fila in pedidoCompleto
                             group fila by new
                             {
                                 _code = fila._code,
                                 _artName = fila._artName,
                                 _brand = fila._brand,
                                 _color = fila._color,
                                 _uriPhoto = fila._uriPhoto,
                             }
                             into g
                             select new Ent_Order_Dtl
                             {
                                 _code = g.Key._code,
                                 _artName = g.Key._artName,
                                 _brand = g.Key._brand,
                                 _color = g.Key._color,
                                 _tallas = pedidoCompleto.Where(w => w._code == g.Key._code).Select(s => s._size).ToArray(),
                                 _qtys = pedidoCompleto.Where(w => w._code == g.Key._code).Select(s => Convert.ToDecimal(s._qty)).ToArray(),
                                 _price = g.Average(a => a._price),
                                 _commission = g.Sum(s => s._commission),
                                 _lineTotal = g.Sum(s => s._lineTotal),
                                 _dscto = g.Sum(s => s._dscto),
                                 _lineTotDesc = g.Sum(s => s._lineTotDesc),
                                 _uriPhoto = g.Key._uriPhoto
                             }).ToList();
            //IQueryable <Ent_Order_Dtl> membercol = ((List<Ent_Order_Dtl>)(Session[_session_list_detalle_pedido])).AsQueryable();  //lista().AsQueryable();

            //List<Ent_Order_Dtl> _vislis = VistaLista.ToList<Ent_Order_Dtl>();

             IQueryable<Ent_Order_Dtl> membercol = VistaLista.AsQueryable();  //lista().AsQueryable();

            //Manejador de filtros
            int totalCount = membercol.Count();
            IEnumerable<Ent_Order_Dtl> filteredMembers = membercol;

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = membercol
                    .Where(m => m._code.ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m._artName.ToUpper().Contains(param.sSearch.ToUpper()));
            }
            //Manejador de orden
            var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);

            if (param.iSortingCols > 0)
            {
                if (Request["sSortDir_0"].ToString() == "asc")
                {
                    switch (sortIdx)
                    {
                        //case 0: filteredMembers = filteredMembers.OrderBy(o => Convert.ToDecimal(o.liq_Id)); break;
                        //case 1: filteredMembers = filteredMembers.OrderBy(o => o.ventaId); break;
                        //case 2: filteredMembers = filteredMembers.OrderBy(o => Convert.ToDateTime(o.liq_Fecha)); break;
                        //case 3: filteredMembers = filteredMembers.OrderBy(o => o.Pares); break;
                        //case 4: filteredMembers = filteredMembers.OrderBy(o => o.Estado); break;
                        //case 5: filteredMembers = filteredMembers.OrderBy(o => o.Ganancia); break;
                        //case 6: filteredMembers = filteredMembers.OrderBy(o => o.Subtotal); break;
                        //case 7: filteredMembers = filteredMembers.OrderBy(o => o.N_C); break;
                        //case 8: filteredMembers = filteredMembers.OrderBy(o => o.Total); break;
                        //case 9: filteredMembers = filteredMembers.OrderBy(o => o.Percepcion); break;
                        //case 10: filteredMembers = filteredMembers.OrderBy(o => o.TotalPagar); break;
                    }
                }
                else
                {
                    switch (sortIdx)
                    {
                        //case 0: filteredMembers = filteredMembers.OrderByDescending(o => Convert.ToDecimal(o.liq_Id)); break;
                        //case 1: filteredMembers = filteredMembers.OrderByDescending(o => o.ventaId); break;
                        //case 2: filteredMembers = filteredMembers.OrderByDescending(o => Convert.ToDateTime(o.liq_Fecha)); break;
                        //case 3: filteredMembers = filteredMembers.OrderByDescending(o => o.Pares); break;
                        //case 4: filteredMembers = filteredMembers.OrderByDescending(o => o.Estado); break;
                        //case 5: filteredMembers = filteredMembers.OrderByDescending(o => o.Ganancia); break;
                        //case 6: filteredMembers = filteredMembers.OrderByDescending(o => o.Subtotal); break;
                        //case 7: filteredMembers = filteredMembers.OrderByDescending(o => o.N_C); break;
                        //case 8: filteredMembers = filteredMembers.OrderByDescending(o => o.Total); break;
                        //case 9: filteredMembers = filteredMembers.OrderByDescending(o => o.Percepcion); break;
                        //case 10: filteredMembers = filteredMembers.OrderByDescending(o => o.TotalPagar); break;
                    }
                }
            }
            var displayMembers = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);
            var result = from a in displayMembers
                         select new
                         {
                             a._code,
                             a._artName,
                             a._brand,
                             a._color,
                             a._size,
                             a._qty,
                             a._price,
                             a._commission,
                             a._det_dcto_sigv,
                             a._lineTotal,
                             a._dscto,
                             a._tallas , 
                             a._qtys,
                             a._lineTotDesc,
                             a._uriPhoto
                         };
            //Se devuelven los resultados por json
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalCount,
                iTotalDisplayRecords = filteredMembers.Count(),
                aaData = result,
                header = header
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getPedidoNC(Ent_jQueryDataTableParams param)
        {
            /*verificar si esta null*/
            if (Session[_session_notas_persona] == null)
            {
                List<Ent_Pago_NCredito> listPed = new List<Ent_Pago_NCredito>();
                Session[_session_notas_persona] = listPed;
            }
            //Traer registros

            IQueryable <Ent_Pago_NCredito> membercol = ((List<Ent_Pago_NCredito>)(Session[_session_notas_persona])).AsQueryable();  //lista().AsQueryable();

            //Manejador de filtros
            int totalCount = membercol.Count();
            IEnumerable<Ent_Pago_NCredito> filteredMembers = membercol;

           
            var displayMembers = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);
            var result = from a in displayMembers
                         select new
                         {
                             a.Consumido,
                             a.Activado,
                             a.Ncredito,
                             a.Importe,
                             a.Rhv_return_nro,
                             a.Fecha_documento
                         };
            //Se devuelven los resultados por json
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalCount,
                iTotalDisplayRecords = filteredMembers.Count(),
                aaData = result,
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AnularLiquidacion(string liq, string cliente)
        {
            string mensaje = "";
            string mensaje2 = "";
            bool ret = false;           
            ret = datPedido.AnularLiquidacion(liq, ref mensaje);
            Ent_Info_Promotor infoGeneralPedidos = datPedido.ListarPedidos(Convert.ToDecimal(cliente), ref mensaje2);
            Session[_session_listPedido_private] = infoGeneralPedidos.liquidacion;
            return Json(new { estado = ret ? 0 : 1 , mensaje = mensaje });
        }
        public ActionResult MarcarNC(string[] ncs)
        {
            List<Ent_Pago_NCredito> listNC = new List<Ent_Pago_NCredito>();
            listNC = (List<Ent_Pago_NCredito>)Session[_session_notas_persona];
            listNC.Select(s => { s.Consumido = false; return s; }).ToList();

            Int32 estado = 1;
            string mensaje = "Ninguna nota de credito aplicada";

            if (ncs != null) {
                mensaje = "Nota de credito aplicada.";
                estado = 1;
                listNC.Where(w => ncs.Contains(w.Ncredito)).Select(s => { s.Consumido = true; return s; }).ToList();
            }


            return Json(new { estado = estado, mensaje= mensaje });
        }

        public ActionResult get_valida_pedido()
        {
            string prom = "0";
            try
            {
                List<Ent_Order_Dtl> info = null;
                if (Session[_session_list_detalle_pedido] == null)
                {
                    info = new List<Ent_Order_Dtl>();
                }
                else
                {
                    info = (List<Ent_Order_Dtl>)Session[_session_list_detalle_pedido];
                }

                if (info.Count==0) return Json(new { estado = 0, info = info });


                var validapercep = info.GroupBy(g => g._ap_percepcion).ToList();

                if (validapercep.Count>=2) return Json(new { estado = 0, info = info });

                /*validacion de oferta*/
                DataTable dt = new DataTable();
                dt.Columns.Add("cod_artic", typeof(string));
                dt.Columns.Add("precio", typeof(decimal));
                dt.Columns.Add("cantidad", typeof(decimal));
                string valida_descuento = "0";

                Int32 i = 1;
                foreach (Ent_Order_Dtl item in info)
                {
                    if (item._dscto < 0) valida_descuento = "1";

                    dt.Rows.Add(item._code, item._price, item._qty);
                    i++;
                }

                if (valida_descuento=="1") return Json(new { estado = 0, info = info, valida_descuento = valida_descuento });

                if (dt.Rows.Count>0)
                {
                    Dat_Pedido dat_prom = new Dat_Pedido();
                    Boolean val_promo = dat_prom._return_valida_promo_exists(dt);
                    if (val_promo) return Json(new { estado = 0, info = info, prom = prom });
                }
                Ent_Persona cust = (Ent_Persona)Session[_session_customer];

                if (cust.Bas_id=="0") return Json(new { estado = 0, info = info, prom = prom, user = cust.Bas_id.ToString() });

                string articulo = "";
                string talla = "";
                Ent_Order_Stk_Disponible stk = new Ent_Order_Stk_Disponible();
                if (!(fvalidastock(ref articulo, ref talla)))
                {
                   
                    stk.disponible = "1";
                    stk.articulo = articulo;
                    stk.talla = talla;

                    return Json(new { estado = 0, info = info, prom = prom, user = cust.Bas_id.ToString(), stk= stk });

                }
                else
                {
                    stk.disponible = "0";
                }



                return Json(new { estado = 0, info = info, prom = prom,user=cust.Bas_id.ToString(), stk = stk });
            }
            catch (Exception ex)
            {
                return Json(new { estado = 2, mensaje = ex.Message });
            }
        }

        private Boolean fvalidastock(ref string articulo, ref string talla)
        {

            Dat_Pedido dat_ped = new Dat_Pedido();
            Ent_Liquidacion liq = (Ent_Liquidacion)Session[_session_lnfo_liquidacion];

            Boolean valida = true;
            string estadoliquid = liq.estId;//  (string)Session[_estadoliqui];
            string nroliq = liq.liq_Id; // (string)Session[_nSOrderUrl];

            List<Ent_Order_Dtl> order = (List<Ent_Order_Dtl>)Session[_session_list_detalle_pedido];

            //string.IsNullOrEmpty(liq.liq_Id)

            foreach (Ent_Order_Dtl item in order)
            {

                //Int32 vcantidad = dat_ped.fvalidastock(item._code, item._size, item._qty, (!(string.IsNullOrEmpty(estadoliquid))) ? nroliq : "");
                Int32 vcantidad = dat_ped.fvalidastock(item._code, item._size, item._qty, (!(string.IsNullOrEmpty(liq.liq_Id))) ? nroliq : "");
                if (vcantidad == 0)
                {
                    articulo = item._code;
                    talla = item._size;
                    valida = false;
                    break;
                }

            }
            return valida;
        }

        // [HttpPost]        
        //public ActionResult get_detalle_pedido()
        //{
        //    List<Ent_Order_Dtl> listar = null;
        //    if (Session[_session_list_detalle_pedido]==null)
        //    {
        //        listar = new List<Ent_Order_Dtl>();
        //    }
        //    else
        //    {
        //        listar = (List<Ent_Order_Dtl>) Session[_session_list_detalle_pedido];
        //    }
        //    //return View(listar);
        //    // return listar;
        //    return Json(new
        //    {
        //        sEcho = param.sEcho,
        //        iTotalRecords = totalCount,
        //        iTotalDisplayRecords = filteredMembers.Count(),
        //        aaData = result
        //    }, JsonRequestBehavior.AllowGet);
        //}
    }
}