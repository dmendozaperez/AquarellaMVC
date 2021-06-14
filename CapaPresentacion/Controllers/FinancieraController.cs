using System.Text;
using CapaDato.Financiera;
using CapaDato.Pedido;
using CapaDato.Persona;
using CapaEntidad.Control;
using CapaEntidad.Financiera;
using CapaEntidad.General;
using CapaEntidad.Menu;
using CapaEntidad.Pedido;
using CapaEntidad.Persona;
using CapaEntidad.Util;
using CapaPresentacion.Bll;
using CapaPresentacion.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.Data.OleDb;
using System.Globalization;

namespace CapaPresentacion.Controllers
{
    public class FinancieraController : Controller
    {
        private Dat_Pedido datPedido = new Dat_Pedido();
        private Dat_Persona datPersona = new Dat_Persona();
        private Dat_Financiera datFinanciera = new Dat_Financiera();
        private Dat_Documento_Transaccion datDocumento_Transaccion = new Dat_Documento_Transaccion();
        private Dat_Banco datBanco = new Dat_Banco();
        private Dat_Concepto datConcepto = new Dat_Concepto();
        private Dat_Pago datPago = new Dat_Pago();
        private Dat_Estado datEstado = new Dat_Estado();
        private string _sessionPagsLiqs = "_SessionPagsLiqs";
        private string _sessin_customer = "_sessin_customer";
        private string _session_listCuentasContables = "_session_listCuentasContables";
        private string _session_ListCuentasContables_Excel = "_session_listCuentasContables_Excel";
        private string _session_listClienteBanco = "_session_listClienteBanco";
        private string _session_listClienteBanco_Txt = "_session_listClienteBanco_Txt";
        private string _session_ListarClientePagos = "_session_ListarClientePagos";
        private string _session_ListarVerificarPagos = "_session_ListarVerificarPagos";
        private string _session_ListarVentaSemanal = "_session_ListarVentaSemanal";
        private string _session_ListarVentaSemanal_Excel = "_session_ListarVentaSemanal_Excel";
        private string _session_listSaldosAnticipos = "_session_listSaldosAnticipos";
        private string _session_ListarValidarPagos = "_session_ListarValidarPagos";
        private string _session_ListarOpeGratuitas = "_session_ListarOpeGratuitas";
        private string _session_ListarOpeGratuitas_Excel = "_session_ListarOpeGratuitas_Excel";
        private string _session_ListarSaldoCliente = "_session_ListarSaldoCliente";
        private string _session_ListarSaldoCliente_Excel = "_session_ListarSaldoCliente_Excel";
        private string _session_ListarMovimientosPagos = "_session_ListarMovimientosPagos";
        private string _session_ListarMovimientosPagos_Excel = "_session_ListarMovimientosPagos_Excel";
        // GET: Financiera
        public ActionResult Index()
        {
            return View();
        }
        #region Cruces

        public ActionResult CrucePago()
        {
            


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
                Boolean valida_rol = true;
                Basico valida_controller = new Basico();
                List<Ent_Menu_Items> menu = (List<Ent_Menu_Items>)Session[Ent_Global._session_menu_user];
                valida_rol = valida_controller.AccesoMenu(menu, this);
                #endregion
                if (valida_rol)
                {
                    Session[_sessionPagsLiqs] = null;
                    //Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
                    Ent_Pedido_Maestro maestros = datPedido.Listar_Maestros_Pedido(_usuario.usu_id, _usuario.usu_postPago, "");
                    ViewBag.listPromotor = maestros.combo_ListPromotor;
                    ViewBag.usutipo = _usuario.usu_tip_id.ToString();
                    return View();
                }
                else
                {
                    return RedirectToAction("Login", "Control", new { returnUrl = return_view });
                }
            }
        }
        public ActionResult GET_INFO_PERSONA(string codigo)
        {
            try
            {
                Ent_Persona info = datPersona.GET_INFO_PERSONA(codigo);
                Session[_sessin_customer] = info;
                Session[_sessionPagsLiqs] = getPagsLiqs(info.Bas_id);
                
                return Json(new { estado = 0, info = info});
            }
            catch (Exception ex)
            {
                return Json(new { estado = 2, mensaje = ex.Message });
            }
        }

        public List<Ent_Pag_Liq> getPagsLiqs(string custId)
        {
            return datFinanciera.getPagsLiqs(custId);
        }        
        public ActionResult getDataPagsLiqs(Ent_jQueryDataTableParams param, bool checking = false , string checkVal = "") 
        {
            /*verificar si esta null*/
            if (Session[_sessionPagsLiqs] == null)
            {
                List<Ent_Pag_Liq> listPed = new List<Ent_Pag_Liq>();
                Session[_sessionPagsLiqs] = listPed;
            }
            if (checking)
            {
                List<Ent_Pag_Liq> list = (List<Ent_Pag_Liq>)Session[_sessionPagsLiqs];
                list.Where(w => w.dtv_transdoc_id == checkVal).Select(s => { s.checks = !s.checks; return s; }).ToList();
                Session[_sessionPagsLiqs] = list;
            }

            //Traer registros

            IQueryable<Ent_Pag_Liq> membercol = ((List<Ent_Pag_Liq>)(Session[_sessionPagsLiqs])).AsQueryable();  //lista().AsQueryable();

            //Manejador de filtros
            int totalCount = membercol.Count();
            IEnumerable<Ent_Pag_Liq> filteredMembers = membercol;


            var displayMembers = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);
            var result = from a in displayMembers
                         select new
                         {
                             a.dtv_transdoc_id     ,
                             a.dtv_concept_id     ,
                             a.cov_description   ,
                             //document_date_desc = a.document_date_desc.ToString("dd/MM/yyyy hh:mm"),
                             a.document_date_desc,
                             a.dtd_document_date  ,
                             a.debito             ,
                             a.credito            ,
                             a.val                ,
                             a.TIPO               ,
                             a.active             ,
                             a.checks             ,
                             a.von_increase       ,
                             a.Flag,
                             monto = a.val * a.von_increase
                         };
            //Se devuelven los resultados por json
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalCount,
                iTotalDisplayRecords = filteredMembers.Count(),
                aaData = result,
                total = membercol.Where(w => w.checks).Sum(s => s.val * s.von_increase)
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult valida_cruce()
        {
            #region<REGION DE VALIDACIONES>

            string _mensaje = "";
            int estado = 0;
            if (Session[_sessionPagsLiqs] == null)
            {
                List<Ent_Pag_Liq> listPed = new List<Ent_Pag_Liq>();
                Session[_sessionPagsLiqs] = listPed;
            }

            Ent_Persona cust = (Ent_Persona)Session[_sessin_customer];

            List<Ent_Pag_Liq> list = (List<Ent_Pag_Liq>)Session[_sessionPagsLiqs];
            list = list.Where(w => w.checks).ToList();

            int countLiqSel = list.Where(w => w.dtv_concept_id == "LIQUIDACIONES").Count();
            int countPagSel = list.Where(w => w.dtv_concept_id == "PAGOS").Count();
            DataTable dtpagos = new DataTable();
            dtpagos.Columns.Add("Doc_Tra_Id", typeof(string));

            if (list.Count == 0)
            {
                estado = 1;
                _mensaje = "No ha seleccionado ningun item.";
                return Json(new { estado = estado, mensaje = _mensaje });
            }
            if (countLiqSel > 1)
            {
                estado = 1;
                _mensaje = "No se puede realizar cruce de pagos con 2 o más Pedidos, por favor seleccione solo 1 pedido.";
                return Json(new { estado = estado, mensaje = _mensaje });
            }
            if (countLiqSel == 0)
            {
                estado = 1;
                _mensaje = "no ha seleccionado ningun pedido para cruzar el pago";
                return Json(new { estado = estado, mensaje = _mensaje });
            }
            if (countPagSel > 1)
            {
                decimal _sum_pag = 0;
                decimal _liq_val = list.Where(w => w.dtv_concept_id == "LIQUIDACIONES").Select(s => s.val).FirstOrDefault();
                Int32 _limite = 0;
                foreach (Ent_Pag_Liq item in list.OrderByDescending(o => o.val))
                {
                    if (item.dtv_concept_id == "PAGOS")
                    {
                        _sum_pag += item.val;
                        if (_sum_pag > _liq_val)
                        {
                            if (_limite == 0)
                            {
                                _limite = 1;
                            }
                            else
                            {
                                estado = 1;
                                _mensaje = "por favor solo seleccione el pago necesario para pagar su pedido";
                                return Json(new { estado = estado, mensaje = _mensaje });
                            }


                        }
                    }
                }
            }
            return Json(new { estado = estado, mensaje = _mensaje });
            #endregion
        }

        public ActionResult GuardarCruce()
        {

            #region<REGION DE VALIDACIONES>

            string _mensaje = "";
            int estado = 1;
            if (Session[_sessionPagsLiqs] == null)
            {
                List<Ent_Pag_Liq> listPed = new List<Ent_Pag_Liq>();
                Session[_sessionPagsLiqs] = listPed;
            }

            Ent_Persona cust = (Ent_Persona)Session[_sessin_customer];

            List<Ent_Pag_Liq> list = (List<Ent_Pag_Liq>)Session[_sessionPagsLiqs];
            list = list.Where(w => w.checks).ToList();

            int countLiqSel = list.Where(w => w.dtv_concept_id == "LIQUIDACIONES").Count();
            int countPagSel = list.Where(w => w.dtv_concept_id == "PAGOS").Count();
            DataTable dtpagos = new DataTable();
            dtpagos.Columns.Add("Doc_Tra_Id", typeof(string));

            if (list.Count == 0)
            {
                _mensaje = "No ha seleccionado ningun item.";
                return Json(new { estado = estado, mensaje = _mensaje });
            }
            if (countLiqSel > 1)
            {
                _mensaje = "No se puede realizar cruce de pagos con 2 o más Pedidos, por favor seleccione solo 1 pedido.";
                return Json(new { estado = estado, mensaje = _mensaje });
            }
            if (countLiqSel == 0)
            {
                _mensaje = "no ha seleccionado ningun pedido para cruzar el pago";
                return Json(new { estado = estado, mensaje = _mensaje });
            }
            if (countPagSel > 1)
            {
                decimal _sum_pag = 0;
                decimal _liq_val = list.Where(w => w.dtv_concept_id == "LIQUIDACIONES").Select(s => s.val).FirstOrDefault();
                Int32 _limite = 0;
                foreach (Ent_Pag_Liq item in list.OrderByDescending(o=> o.val))
                {
                    if ( item.dtv_concept_id == "PAGOS")
                    {                        
                        _sum_pag += item.val;
                        if (_sum_pag > _liq_val)
                        {
                            if (_limite == 0)
                            {
                                _limite = 1;
                            }
                            else
                            {
                                _mensaje = "por favor solo seleccione el pago necesario para pagar su pedido";
                                return Json(new { estado = estado, mensaje = _mensaje });
                            }

                                                     
                        }
                    }
                }                    
             }
            #endregion

            if (countLiqSel > 0 && countPagSel > 0)
            {
                string listLiq = list.Where(w => w.dtv_concept_id == "LIQUIDACIONES").Select(s => s.dtv_transdoc_id).FirstOrDefault(); 
                try
                {
                    string vrefnc = "";
                    string vreffec = "";
                    string _validaref = string.Empty;

                    _validaref = datFinanciera.setvalidaclear(listLiq, ref vrefnc, ref vreffec);
                    if (!(string.IsNullOrEmpty(_validaref)))
                    {
                        _mensaje = "No se puede realizar cruce de pagos; porque la fecha de referencia de la nota de credito N " + vrefnc +
                            " pertenece a otro mes con fecha " + vreffec + "  ,por favor anule este pedido y vuelva a generar otro pedido" ;
                        return Json(new { estado = estado, mensaje = _mensaje });
                    }
                    foreach (Ent_Pag_Liq item in list.Where(w=>w.dtv_concept_id == "PAGOS"))
                    {
                        dtpagos.Rows.Add(item.dtv_transdoc_id);
                    }
                    
                    string strIdPromotor = cust.Bas_id;

                    string str_mensaje = "";

                    string clear = datFinanciera.setPreClear(listLiq, dtpagos,ref str_mensaje);

                    if (!String.IsNullOrEmpty(clear))
                    {
                        string[] prems = clear.Split('|');
                        string strpremio = prems[1].ToString();
                        string strpremio2 = prems[2].ToString();
                        //string strmensaje = "";
                        string strmensajePremio = "";

                        if (strpremio != "N" && strpremio != "0")
                        {
                           // string strIdLiquidacion = datFinanciera.setCrearLiquidacionPremio(Convert.ToInt32(strIdPromotor), Convert.ToInt32(strpremio), "C");
                           // strmensajePremio = "Premio generado en el pedido:" + strIdLiquidacion + " ";
                        }

                        if (strpremio2 != "N" && strpremio2 != "0")
                        {
                            //string strIdLiquidacion = datFinanciera.setCrearLiquidacionPremio(Convert.ToInt32(strIdPromotor), Convert.ToInt32(strpremio2), "P");
                            string cadena = "";
                            if (strmensajePremio != "") { cadena = "y"; }

                           // strmensajePremio = strmensajePremio + " " + cadena + " en el pedido:" + strIdLiquidacion + " ";
                        }

                        if (strmensajePremio != "") { strmensajePremio = " ( " + strmensajePremio + " ) "; }

                        _mensaje = str_mensaje; //"El cruce de información fue grabado correctamente " + strmensajePremio + ", su pedido sera enviado  marcación y posterior facturación; número del cruce: " + prems[0].ToString() + strmensaje;
                        estado = 0;
                    }
                }
                catch (Exception ex)
                {
                    _mensaje = ex.Message;                    
                }
            }
            if (estado == 0)
            {
                Session[_sessionPagsLiqs] = getPagsLiqs(cust.Bas_id);
            }

            return Json(new { estado = estado, mensaje = _mensaje });      
        }
        #endregion

        #region <Lista de cuenta contables>

        public ActionResult MovPago()
        {
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
                Ent_Pedido_Maestro maestros = datPedido.Listar_Maestros_Pedido(_usuario.usu_id, _usuario.usu_postPago, "");
                ViewBag.listPromotor = maestros.combo_ListPromotor;
                return View();               
            }
        }

        [HttpGet]
        public JsonResult getListaCuentaContablesAjax(Ent_jQueryDataTableParams param, string FechaInicio,string FechaFin,int IdCliente,bool isOkUpdate)
        {
            Ent_Lista_Cuenta_Contables Ent_Lista_Cuenta_Contables = new Ent_Lista_Cuenta_Contables();
            JsonResponse objResult = new JsonResponse();
            DateTime time = new DateTime();
            if (isOkUpdate)
            {
                Ent_Lista_Cuenta_Contables.FechaInicio = DateTime.Parse(FechaInicio);
                Ent_Lista_Cuenta_Contables.FechaFin = DateTime.Parse(FechaFin);
                Ent_Lista_Cuenta_Contables.IdCliente = IdCliente;
                Session[_session_listCuentasContables] = datDocumento_Transaccion.Listar_Asientos_Adonis(Ent_Lista_Cuenta_Contables).ToList(); ;
            }

            /*verificar si esta null*/
            if (Session[_session_listCuentasContables] == null)
            {
                List<Ent_Lista_Cuenta_Contables> Lista_Cuenta_Contables = new List<Ent_Lista_Cuenta_Contables>();
                Session[_session_listCuentasContables] = Lista_Cuenta_Contables;
            }

            IQueryable<Ent_Lista_Cuenta_Contables> entDocTrans = ((List<Ent_Lista_Cuenta_Contables>)(Session[_session_listCuentasContables])).AsQueryable();
            
            //Manejador de filtros
            int totalCount = entDocTrans.Count();
            IEnumerable<Ent_Lista_Cuenta_Contables> filteredMembers = entDocTrans;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = entDocTrans
                    .Where(m =>
                       m.Clear_id.ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.Cuenta.ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.CuentaDes.ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.TipoEntidad.ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.CodigoEntidad.ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.DesEntidad.ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.Tipo.ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.Serie.ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.Numero.ToUpper().Contains(param.sSearch.ToUpper())
                     );
            }

            //Manejador de orden
            var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);

            if (param.iSortingCols > 0)
            {
                if (Request["sSortDir_0"].ToString() == "asc")
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderBy(o => o.Clear_id); break;
                        case 1: filteredMembers = filteredMembers.OrderBy(o => o.Cuenta); break;
                        case 2: filteredMembers = filteredMembers.OrderBy(o => o.CuentaDes); break;
                        case 3: filteredMembers = filteredMembers.OrderBy(o => o.TipoEntidad); break;
                        case 4: filteredMembers = filteredMembers.OrderBy(o => o.CodigoEntidad); break;
                        case 5: filteredMembers = filteredMembers.OrderBy(o => o.DesEntidad); break;
                        case 6: filteredMembers = filteredMembers.OrderBy(o => o.Tipo); break;
                        case 7: filteredMembers = filteredMembers.OrderBy(o => o.Serie); break;
                        case 8: filteredMembers = filteredMembers.OrderBy(o => o.Numero); break;
                        case 9: filteredMembers = filteredMembers.OrderBy(o => o.Fecha); break;
                        case 10: filteredMembers = filteredMembers.OrderBy(o => o.Debe); break;
                        case 11: filteredMembers = filteredMembers.OrderBy(o => o.Haber); break;
                    }
                }
                else
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderByDescending(o => o.Clear_id); break;
                        case 1: filteredMembers = filteredMembers.OrderByDescending(o => o.Cuenta); break;
                        case 2: filteredMembers = filteredMembers.OrderByDescending(o => o.CuentaDes); break;
                        case 3: filteredMembers = filteredMembers.OrderByDescending(o => o.TipoEntidad); break;
                        case 4: filteredMembers = filteredMembers.OrderByDescending(o => o.CodigoEntidad); break;
                        case 5: filteredMembers = filteredMembers.OrderByDescending(o => o.DesEntidad); break;
                        case 6: filteredMembers = filteredMembers.OrderByDescending(o => o.Tipo); break;
                        case 7: filteredMembers = filteredMembers.OrderByDescending(o => o.Serie); break;
                        case 8: filteredMembers = filteredMembers.OrderByDescending(o => o.Numero); break;
                        case 9: filteredMembers = filteredMembers.OrderByDescending(o => o.Fecha); break;
                        case 10: filteredMembers = filteredMembers.OrderByDescending(o => o.Debe); break;
                        case 11: filteredMembers = filteredMembers.OrderByDescending(o => o.Haber); break;
                    }
                }
            }

            var Result = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);

            //Se devuelven los resultados por json
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalCount,
                iTotalDisplayRecords = filteredMembers.Count(),
                aaData = Result
            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult get_exporta_ListCuentasContables_excel()
        {
            JsonResponse objResult = new JsonResponse();
            try
            {
                Session[_session_ListCuentasContables_Excel] = null;
                string cadena = "";
                if (Session[_session_listCuentasContables] != null)
                {

                    List<Ent_Lista_Cuenta_Contables> ListarArticulo = (List<Ent_Lista_Cuenta_Contables>)Session[_session_listCuentasContables];
                    if (ListarArticulo.Count == 0)
                    {
                        objResult.Success = false;
                        objResult.Message = "No hay filas para exportar";
                    }
                    else
                    {
                        cadena = get_html_ListCuentasContables_str((List<Ent_Lista_Cuenta_Contables>)Session[_session_listCuentasContables]);
                        if (cadena.Length == 0)
                        {
                            objResult.Success = false;
                            objResult.Message = "Error del formato html";
                        }
                        else
                        {
                            objResult.Success = true;
                            objResult.Message = "Se genero el excel correctamente";
                            Session[_session_ListCuentasContables_Excel] = cadena;
                        }
                    }
                }
                else
                {
                    objResult.Success = false;
                    objResult.Message = "No hay filas para exportar";
                }

            }
            catch (Exception ex)
            {
                objResult.Success = false;
                objResult.Message = "No hay filas para exportar";
            }

            var JSON = JsonConvert.SerializeObject(objResult);

            return Json(JSON, JsonRequestBehavior.AllowGet);
        }

        public string get_html_ListCuentasContables_str(List<Ent_Lista_Cuenta_Contables> ListCuentasContables)
        {
            StringBuilder sb = new StringBuilder();
            int intcounT = 0;
            int intCell = 1;
            try
            {
                //Lista por grupos
                var Lista = ListCuentasContables.GroupBy(x => x.Clear_id).Select(y => new
                {
                    Padre = y.Key,
                    Hijos = y.Select(m => new
                    {
                        Clear_id = m.Clear_id,
                        Cuenta = m.Cuenta,
                        CuentaDes = m.CuentaDes,
                        TipoEntidad = m.TipoEntidad,
                        CodigoEntidad = m.CodigoEntidad,
                        DesEntidad = m.DesEntidad,
                        Tipo = m.Tipo,
                        Serie = m.Serie,
                        Numero = m.Numero,
                        Fecha = m.Fecha,
                        Debe = m.Debe,
                        Haber = m.Haber,
                    })
                }).ToList();

                sb.Append("<div><table cellspacing='0' rules='all' border='0' style='border-collapse:collapse;'><td Colspan='12' valign='middle' align='center' style='font-size: 18px;font-weight: bold;color:#285A8F'>CUENTAS CONTABLE - CATALOGO - BATA</td></table>");
                sb.Append("<table border='2' bgColor='#ffffff' borderColor='#FFFFFF' cellSpacing='2' cellPadding='2' style='font-size:10.0pt; font-family:Calibri; background:white;'><tr  bgColor='#5799bf'><th colspan='6'></th><th colspan='4' style='text-align: center;'><font color='#FFFFFF'>Docummento</fonr></th><th colspan='2'></th></tr><tr bgColor='#5799bf'><th style='text-align: center;'><font color='#FFFFFF'>Clear ID</font></th><th style='text-align: center;'><font color='#FFFFFF'>Cuenta Contable</font></th><th style='text-align: center;'><font color='#FFFFFF'>Descripción Cuenta</font></th><th style='text-align: center;'><font color='#FFFFFF'>Tipo de Entidad</font></th><th style='text-align: center;'><font color='#FFFFFF'>Codigo entidad</font></th><th style='text-align: center;'><font color='#FFFFFF'>Descripción Entidad</font></th><th style='text-align: center;'><font color='#FFFFFF'>Tipo</font></th><th style='text-align: center;'><font color='#FFFFFF'>Serie</font></th><th style='text-align: center;'><font color='#FFFFFF'>Número</font></th><th style='text-align: center;'><font color='#FFFFFF'>Fecha</font></th><th style='text-align: center;'><font color='#FFFFFF'>Debe</font></th><th style='text-align: center;'><font color='#FFFFFF'>Haber</font></th></tr>\n");
                string tdSColor = "";
                foreach (var itemP in Lista)
                {
                    foreach (var itemH in itemP.Hijos)
                    {
                        sb.Append("<tr>\n");
                        if (intcounT == 0)
                        {
                            intcounT = itemP.Hijos.Count();
                            sb.Append("<td bgcolor='#83c5ea' style='text-align: center; vertical-align: middle;' rowspan='" + intcounT + "'>" + itemH.Clear_id + "</td>");                                                      
                        }
                        if (intCell == itemP.Hijos.Count()) tdSColor = " bgcolor='#b1dbf3' ";
                        sb.Append("<td " + tdSColor + " align='center'>" + itemH.Cuenta + "</td>");
                        sb.Append("<td " + tdSColor + ">" + itemH.CuentaDes + "</td>");
                        sb.Append("<td " + tdSColor + " align='center'>" + itemH.TipoEntidad + "</td>");
                        sb.Append("<td " + tdSColor + ">" + itemH.CodigoEntidad + "</td>");
                        sb.Append("<td " + tdSColor + ">" + itemH.DesEntidad + "</td>");
                        sb.Append("<td " + tdSColor + " align='center'>" + itemH.Tipo + "</td>");
                        sb.Append("<td " + tdSColor + " align='center'>" + itemH.Serie + "</td>");
                        sb.Append("<td " + tdSColor + " align='center'>" + itemH.Numero + "</td>");
                        sb.Append("<td " + tdSColor + " align='center'>" + ((itemH.Fecha == null) ? (DateTime?)null : Convert.ToDateTime(String.Format("{0:d}", itemH.Fecha))) + " </td>");
                        sb.Append("<td " + tdSColor + " align='right'>" + ((itemH.Debe == null) ? (Decimal?)null : Convert.ToDecimal(string.Format("{0:F2}", itemH.Debe))) + "</td>");
                        sb.Append("<td " + tdSColor + " align='right'>" + ((itemH.Haber == null) ? (Decimal?)null : Convert.ToDecimal(string.Format("{0:F2}", itemH.Haber))) + "</td>");
                        sb.Append("</tr>\n");
                        intCell++;
                    }
                    intcounT = 0;
                    tdSColor = "";
                    intCell = 1;
                }
                sb.Append("</table></div>");
            }
            catch
            {

            }
            return sb.ToString();
        }


        public ActionResult listCuentasContablesExcel()
        {
            string NombreArchivo = "Lista_Cuenta_Contables";
            String style = style = @"<style> .textmode { mso-number-format:\@; } </script> ";
            try
            {
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", "attachment;filename=" + NombreArchivo + ".xls");
                Response.Charset = "UTF-8";
                Response.ContentEncoding = Encoding.Default;
                Response.Write(style);
                Response.Write(Session[_session_ListCuentasContables_Excel].ToString());
                Response.End();
            }
            catch
            {

            }
            return Json(new { estado = 0, mensaje = 1 });
        }
        #endregion

        #region <ARCHIVO CLIENTE BANCO EN FORMATO TXT>

        public ActionResult ListarClienteBanco()
        {
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
                var ListarBancos = datBanco.Listar_Bancos().Where(x => x.Codigo == "1" || x.Codigo == "4").ToList();
                ViewBag.ListarBancos = ListarBancos;
                return View();
            }            
        }

        public ActionResult get_exporta_Listar_Cliente_Banco_Txt(string IdBanco)
        {
            JsonResponse objResult = new JsonResponse();         

            try
            {
                Ent_Listar_Cliente_Banco ent = new Ent_Listar_Cliente_Banco();
                ent.Ban_Id = IdBanco;
                List<Ent_Listar_Cliente_Banco> Listar_Cliente_Banco = datDocumento_Transaccion.Listar_Cliente_Banco(ent).ToList();
                Session[_session_listClienteBanco_Txt] = null;

                string cadena = "";

                if (Listar_Cliente_Banco.Count == 0)
                {
                    objResult.Success = false;
                    objResult.Message = "No hay filas para exportar";
                }
                else
                {
                    cadena = get_Txt_Listar_Cliente_Banco_str(Listar_Cliente_Banco);
                    if (cadena.Length == 0)
                    {
                        objResult.Success = false;
                        objResult.Message = "Error del formato txt";
                    }
                    else
                    {
                        objResult.Success = true;
                        objResult.Message = "Se genero el txt correctamente";
                        Session[_session_listClienteBanco_Txt] = cadena;
                    }
                }


            }
            catch (Exception ex)
            {
                objResult.Success = false;
                objResult.Message = "No hay filas para exportar";
            }

            var JSON = JsonConvert.SerializeObject(objResult);

            return Json(JSON, JsonRequestBehavior.AllowGet);
        }

        public string get_Txt_Listar_Cliente_Banco_str(List<Ent_Listar_Cliente_Banco> Listar_Cliente_Banco)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                var Lista = Listar_Cliente_Banco.ToList();
                foreach (var item in Lista)
                {                   
                    sb.Append( item.Campo + "\r\n");
                }                
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return sb.ToString();
        }


        public ActionResult listClienteBancoTxt()
        {
            string NombreArchivo = "Lista_Cliente_Banco";
            try
            {
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "text/plain";
                Response.AddHeader("Content-Disposition", "attachment;filename=" + NombreArchivo + ".txt");
                Response.Charset = "UTF-8";
                Response.ContentEncoding = Encoding.Default;
                Response.Write(Session[_session_listClienteBanco_Txt].ToString());
                Response.End();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return Json(new { estado = 0, mensaje = 1 });
        }
        #endregion

        #region <LISTA DE PAGOS>
        /// <summary>
        /// lista los clientes y sus pagos
        /// </summary>
        /// <returns> View </returns>
        public ActionResult ListarPagos()
        {
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
                ViewBag.usu_tip_id = _usuario.usu_tip_id;
                Ent_Pedido_Maestro maestros = datPedido.Listar_Maestros_Pedido(_usuario.usu_id, _usuario.usu_postPago, "");
                ViewBag.listPromotor = maestros.combo_ListPromotor;
                return View();
            }
        }
        /// <summary>
        /// Controlador que sìrve para la paginación
        /// </summary>
        /// <param name="param"></param>
        /// <param name="NroConsignacion"></param>
        /// <param name="FechaInicio"></param>
        /// <param name="FechaFin"></param>
        /// <param name="IdCliente"></param>
        /// <param name="isOkUpdate"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult getListaPagosAjax(Ent_jQueryDataTableParams param, string NroConsignacion, string FechaInicio, string FechaFin, int IdCliente, bool isOkUpdate)
        {
            Ent_Listar_Cliente_Pagos Ent_ListarClientePagos = new Ent_Listar_Cliente_Pagos();
            if (isOkUpdate)
            {
                Ent_ListarClientePagos.FechaInicio = FechaInicio;
                Ent_ListarClientePagos.FechaFin = FechaFin;
                Ent_ListarClientePagos.IdCliente = IdCliente;
                Ent_ListarClientePagos.NumeroConsignacion = NroConsignacion;
                Session[_session_ListarClientePagos] = datFinanciera.Listar_Cliente_Pagos(Ent_ListarClientePagos).ToList();
            }

            /*verificar si esta null*/
            if (Session[_session_ListarClientePagos] == null)
            {
                List<Ent_Listar_Cliente_Pagos> ListarClientePagos = new List<Ent_Listar_Cliente_Pagos>();
                Session[_session_ListarClientePagos] = ListarClientePagos;
            }

            IQueryable<Ent_Listar_Cliente_Pagos> entCliPagos = ((List<Ent_Listar_Cliente_Pagos>)(Session[_session_ListarClientePagos])).AsQueryable();

            //Manejador de filtros
            int totalCount = entCliPagos.Count();
            IEnumerable<Ent_Listar_Cliente_Pagos> filteredMembers = entCliPagos;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = entCliPagos
                    .Where(m =>
                        m.Documento.ToUpper().Contains(param.sSearch.ToUpper()) ||
                        m.NombreCompleto.ToUpper().Contains(param.sSearch.ToUpper()) ||
                        m.PrimerNombre.ToUpper().Contains(param.sSearch.ToUpper()) ||
                        m.SegundoNombre.ToUpper().Contains(param.sSearch.ToUpper()) ||
                        m.PrimeroApellido.ToUpper().Contains(param.sSearch.ToUpper()) ||
                        m.SegundoApellido.ToUpper().Contains(param.sSearch.ToUpper()) ||
                        m.Correo.ToUpper().Contains(param.sSearch.ToUpper()) ||
                        m.NumeroConsignacion.ToUpper().Contains(param.sSearch.ToUpper()) ||
                        m.FechaConsignacion.ToUpper().Contains(param.sSearch.ToUpper()) ||
                        m.Estado.ToUpper().Contains(param.sSearch.ToUpper()) ||
                        m.EstadoNombre.ToUpper().Contains(param.sSearch.ToUpper())
                        );
            }

            //Manejador de orden
            var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);

            if (param.iSortingCols > 0)
            {
                if (Request["sSortDir_0"].ToString() == "asc")
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderBy(o => o.Documento); break;
                        case 1: filteredMembers = filteredMembers.OrderBy(o => o.NombreCompleto); break;
                        case 2: filteredMembers = filteredMembers.OrderBy(o => o.PrimerNombre); break;
                        case 3: filteredMembers = filteredMembers.OrderBy(o => o.SegundoNombre); break;
                        case 4: filteredMembers = filteredMembers.OrderBy(o => o.PrimeroApellido); break;
                        case 5: filteredMembers = filteredMembers.OrderBy(o => o.SegundoApellido); break;
                        case 6: filteredMembers = filteredMembers.OrderBy(o => o.Correo); break;
                        case 7: filteredMembers = filteredMembers.OrderBy(o => o.NumeroConsignacion); break;
                        case 8: filteredMembers = filteredMembers.OrderBy(o => o.FechaConsignacion); break;
                        case 9: filteredMembers = filteredMembers.OrderBy(o => o.Estado); break;
                        case 10: filteredMembers = filteredMembers.OrderBy(o => o.EstadoNombre); break;                        
                    }
                }
                else
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderByDescending(o => o.Documento); break;
                        case 1: filteredMembers = filteredMembers.OrderByDescending(o => o.NombreCompleto); break;
                        case 2: filteredMembers = filteredMembers.OrderByDescending(o => o.PrimerNombre); break;
                        case 3: filteredMembers = filteredMembers.OrderByDescending(o => o.SegundoNombre); break;
                        case 4: filteredMembers = filteredMembers.OrderByDescending(o => o.PrimeroApellido); break;
                        case 5: filteredMembers = filteredMembers.OrderByDescending(o => o.SegundoApellido); break;
                        case 6: filteredMembers = filteredMembers.OrderByDescending(o => o.Correo); break;
                        case 7: filteredMembers = filteredMembers.OrderByDescending(o => o.NumeroConsignacion); break;
                        case 8: filteredMembers = filteredMembers.OrderByDescending(o => o.FechaConsignacion); break;
                        case 9: filteredMembers = filteredMembers.OrderByDescending(o => o.Estado); break;                        
                        case 11: filteredMembers = filteredMembers.OrderByDescending(o => o.EstadoNombre); break;
                    }
                }
            }

            var Result = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);

            //Se devuelven los resultados por json
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalCount,
                iTotalDisplayRecords = filteredMembers.Count(),
                aaData = Result
            }, JsonRequestBehavior.AllowGet);

        }

        #region<Registrar pago nuevo>
        /// <summary>
        /// //controlador que inicia con el pago nuevo
        /// </summary>
        /// <returns>View pago</returns>
        public ActionResult NuevoPago()
        {
            Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
            Ent_Pago Pago = new Ent_Pago();
            string actionName = this.ControllerContext.RouteData.GetRequiredString("action");
            string controllerName = this.ControllerContext.RouteData.GetRequiredString("controller");
            string return_view = actionName + "|" + controllerName;

            if (_usuario == null)
            {
                return RedirectToAction("Login", "Control", new { returnUrl = return_view });
            }
            else
            {
                Ent_Pedido_Maestro maestros = datPedido.Listar_Maestros_Pedido(_usuario.usu_id, _usuario.usu_postPago, "");
                var ListarBancos = datBanco.Listar_Bancos().Where(x => x.Codigo == "1" || x.Codigo == "4").ToList();
                var ListarConceptos = datConcepto.Listar_Conceptos();
                ViewBag.ListarPromotor = maestros.combo_ListPromotor;
                ViewBag.ListarBancos = ListarBancos;
                ViewBag.ListarConceptos = ListarConceptos;
                
                ViewBag.Pago = Pago;
                return View("Pagos");
            }

        }
        /// <summary>
        /// controlador que valida la operacion que ya se encuentra registrado
        /// </summary>
        /// <param name="_Ent"></param>
        /// <returns>Success=true/false</returns>
        [HttpPost]
        public ActionResult getValOperacion(Ent_Pago _Ent)
        {            
            bool Result = false;
            JsonResponse objResult = new JsonResponse();           
            try
            {
                Result = datPago.ValOperacion(_Ent);
                if (Result)
                {
                    objResult.Success = true;
                }
                else
                {
                    objResult.Success = false;                    
                }
            }
            catch (Exception ex)
            {
                objResult.Success = false;
                objResult.Message = "Error";
            }

            var JSON = JsonConvert.SerializeObject(objResult);
            return Json(JSON, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        ///Controlador que registra el pago nuevo
        /// </summary>
        /// <param name="_Ent"></param>
        /// <returns>Success/Message</returns>
        public ActionResult getRegistrarPago(Ent_Pago _Ent)
        {
            bool Result = false;
            Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
            JsonResponse objResult = new JsonResponse();
            try
            {
                _Ent.Pag_Usu_Creacion = Convert.ToDouble(_usuario.usu_id);
                Result = datPago.GrabarPagos(_Ent);
                if (Result)
                {
                    objResult.Success = true;
                    objResult.Message = "El pago se registro correctamente.";
                }
                else
                {
                    objResult.Success = false;
                    objResult.Message = "El pago no se pudo resgitrar.";
                }
            }
            catch (Exception ex)
            {
                objResult.Success = false;
                objResult.Message = "Error al registrar";
            }

            var JSON = JsonConvert.SerializeObject(objResult);

            return Json(JSON, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// controlador que valida si el pago ya tiene transacciones 
        /// </summary>
        /// <param name="PagoId"></param>
        /// <returns>Success/Message</returns>
        public ActionResult getValPagoTransaccion(string PagoId)
        {
            int Result = 0;
            JsonResponse objResult = new JsonResponse();
            Ent_Pago _Ent = new Ent_Pago();
            try
            {
                _Ent.Pag_Id = PagoId;
                _Ent = datPago.ValPagoTransaccionInt(_Ent);
                if (_Ent.Existe== 0)
                {                    
                    objResult.Success = true;
                    objResult.Message = "El pago se elimino correctamente.";
                }
                else if (_Ent.Existe == 1)
                {
                    objResult.Success = false;
                    objResult.Message = "El pago no se puede eliminar porque se encuentra en una transacción( Nro : " + _Ent.RetVal + " ).";
                }
                else if (_Ent.Existe == 2)
                {
                    objResult.Success = false;
                    objResult.Message = "El pago no se puede eliminar porque se encuentre en una transacción.";
                }
            }
            catch (Exception ex)
            {
                objResult.Success = false;
                objResult.Message = "Error al validar pago";
            }

            var JSON = JsonConvert.SerializeObject(objResult);

            return Json(JSON, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// controlador que elimina el pago
        /// </summary>
        /// <param name="PagoId"></param>
        /// <returns>Success/Message</returns>
        public ActionResult getEliminarPago(string PagoId)
        {
            bool Result = false;
            JsonResponse objResult = new JsonResponse();
            Ent_Pago _Ent = new Ent_Pago();
            _Ent.Pag_Id = PagoId;
            try
            {
                Result = datPago.EliminarPago(_Ent);
                if (Result)
                {
                    objResult.Success = true;
                    objResult.Message = "El pago se elimino correctamente.";
                }
                else
                {
                    objResult.Success = false;
                    objResult.Message = "El pago no se pudo resgitrar.";
                }
            }
            catch (Exception ex)
            {
                objResult.Success = false;
                objResult.Message = "Error al eliminar";
            }

            var JSON = JsonConvert.SerializeObject(objResult);

            return Json(JSON, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #endregion

        #region<Confirmación de pagos y consignaciones>
        /// <summary>
        /// Vista donde se realiza la verificación de pagos y consignaciones por veficar.
        /// </summary>
        /// <returns></returns>
        public ActionResult VerificarPago()
        {
            Ent_Estado _EntEst = new Ent_Estado();
            Ent_Pago _EntPago = new Ent_Pago();
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
                ViewBag._EntPago = _EntPago;

                _EntEst.Est_Mod_Id = 4;
                var ListarEstados = datEstado.Listar_Estados(_EntEst);
                ViewBag.ListarEstados = ListarEstados;

                return View();
            }
        }
        /// <summary>
        /// Realice la verificación de pagos y consignaciones por veficar
        /// </summary>
        /// <param name="param"></param>
        /// <param name="NroConsignacion"></param>
        /// <param name="isOkUpdate"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult getListarVerificarPagosAjax(Ent_jQueryDataTableParams param, string Est_Id, string Are_Id, bool isOkUpdate)
        {
            Ent_Listar_Verificar_Pagos EntListarVerificarPagos = new Ent_Listar_Verificar_Pagos();
            if (isOkUpdate)
            {
                EntListarVerificarPagos.Est_Id = Est_Id;
                EntListarVerificarPagos.Are_Id = Are_Id;
                Session[_session_ListarVerificarPagos] = datPago.Listar_Verificar_Pagos(EntListarVerificarPagos).ToList();
            }

            /*verificar si esta null*/
            if (Session[_session_ListarVerificarPagos] == null)
            {
                List<Ent_Listar_Verificar_Pagos> ListarVerificarPagos = new List<Ent_Listar_Verificar_Pagos>();
                Session[_session_ListarVerificarPagos] = ListarVerificarPagos;
            }

            IQueryable<Ent_Listar_Verificar_Pagos> entVerificarPagos = ((List<Ent_Listar_Verificar_Pagos>)(Session[_session_ListarVerificarPagos])).AsQueryable();

            //Manejador de filtros
            int totalCount = entVerificarPagos.Count();
            IEnumerable<Ent_Listar_Verificar_Pagos> filteredMembers = entVerificarPagos;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = entVerificarPagos
                    .Where(m =>                            
                            m.Bas_Documento.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Pag_Num_Consignacion.ToUpper().Contains(param.sSearch.ToUpper()) 
                        );
            }

            //Manejador de orden
            var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);

            if (param.iSortingCols > 0)
            {
                if (Request["sSortDir_0"].ToString() == "asc")
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderBy(o => o.Pag_Id); break;
                        case 1: filteredMembers = filteredMembers.OrderBy(o => o.Lider); break;
                        case 2: filteredMembers = filteredMembers.OrderBy(o => o.Bas_Documento); break;
                        case 3: filteredMembers = filteredMembers.OrderBy(o => o.Promotor); break;
                        case 4: filteredMembers = filteredMembers.OrderBy(o => o.Ban_Descripcion); break;
                        case 5: filteredMembers = filteredMembers.OrderBy(o => o.Pag_Num_Consignacion); break;
                        case 6: filteredMembers = filteredMembers.OrderBy(o => o.Con_Descripcion); break;
                        case 7: filteredMembers = filteredMembers.OrderBy(o => o.Pag_Num_ConsFecha); break;
                        case 8: filteredMembers = filteredMembers.OrderBy(o => o.Pag_Monto); break;
                        case 9: filteredMembers = filteredMembers.OrderBy(o => o.Est_Id); break;
                        case 10: filteredMembers = filteredMembers.OrderBy(o => o.Con_Id); break;
                    }
                }
                else
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderByDescending(o => o.Pag_Id); break;
                        case 1: filteredMembers = filteredMembers.OrderByDescending(o => o.Lider); break;
                        case 2: filteredMembers = filteredMembers.OrderByDescending(o => o.Bas_Documento); break;
                        case 3: filteredMembers = filteredMembers.OrderByDescending(o => o.Promotor); break;
                        case 4: filteredMembers = filteredMembers.OrderByDescending(o => o.Ban_Descripcion); break;
                        case 5: filteredMembers = filteredMembers.OrderByDescending(o => o.Pag_Num_Consignacion); break;
                        case 6: filteredMembers = filteredMembers.OrderByDescending(o => o.Con_Descripcion); break;
                        case 7: filteredMembers = filteredMembers.OrderByDescending(o => o.Pag_Num_ConsFecha); break;
                        case 8: filteredMembers = filteredMembers.OrderByDescending(o => o.Pag_Monto); break;
                        case 9: filteredMembers = filteredMembers.OrderByDescending(o => o.Est_Id); break;
                        case 10: filteredMembers = filteredMembers.OrderByDescending(o => o.Con_Id); break;
                    }
                }
            }

            var Result = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);

            //Se devuelven los resultados por json
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalCount,
                iTotalDisplayRecords = filteredMembers.Count(),
                aaData = Result
            }, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// Controlado que actualiza el estado del pagpo
        /// </summary>
        /// <param name="PagoId"></param>
        /// <returns>Success/Message</returns>
        public ActionResult getActEstPago(Ent_Pago _Ent)
        {
            bool Result = false;
            JsonResponse objResult = new JsonResponse();
            try
            {
                Result = datPago.Actualizar_Estado_Pago(_Ent);
                if (Result)
                {
                    objResult.Success = true;
                    objResult.Message = "El estado del pago se actualizo correctamente.";
                }
                else
                {
                    objResult.Success = false;
                    objResult.Message = "El estado del pago no se pudo actualizo.";
                }
            }
            catch (Exception ex)
            {
                objResult.Success = false;
                objResult.Message = "Error al actualizar";
            }

            var JSON = JsonConvert.SerializeObject(objResult);

            return Json(JSON, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region <REPORTE SEMANAL>

        /// <summary>
        /// Vista que muestra el listado del reporte semanal
        /// </summary>
        /// <returns></returns>
        public ActionResult ReporteSemanal()
        {
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
                Ent_Venta_Semanal EntVentaSemanal = new Ent_Venta_Semanal();
                ViewBag.EntVentaSemanal = EntVentaSemanal;
                return View();
            }
        }
        /// <summary>
        ///  Listado del reporte semanal
        /// </summary>
        /// <param name="param"></param>
        /// <param name="FechaInicio"></param>
        /// <param name="FechaFin"></param>
        /// <param name="isOkUpdate"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult getListaVentaSemanalAjax(Ent_jQueryDataTableParams param, string FechaInicio, string FechaFin, bool isOkUpdate)
        {
            Ent_Venta_Semanal Ent_Lista_Venta_Semanal = new Ent_Venta_Semanal();
            JsonResponse objResult = new JsonResponse();
            DateTime time = new DateTime();
            if (isOkUpdate)
            {
                Ent_Lista_Venta_Semanal.FechaInicio = DateTime.Parse(FechaInicio);
                Ent_Lista_Venta_Semanal.FechaFin = DateTime.Parse(FechaFin);
                
                DataTable dtVentaSemanal = new DataTable();
                dtVentaSemanal.Columns.Add("Dtv_Clear", typeof(string));
                dtVentaSemanal.Columns.Add("Promotor", typeof(string));
                dtVentaSemanal.Columns.Add("DniRuc", typeof(string));
                dtVentaSemanal.Columns.Add("Ped", typeof(string));
                dtVentaSemanal.Columns.Add("BolFact", typeof(string));
                dtVentaSemanal.Columns.Add("FechaDoc", typeof(string));
                dtVentaSemanal.Columns.Add("MontoFac", typeof(Decimal));
                dtVentaSemanal.Columns.Add("NroVouBcp", typeof(string));
                dtVentaSemanal.Columns.Add("FechavouBcp", typeof(string));
                dtVentaSemanal.Columns.Add("MontoVouBcp", typeof(Decimal));
                dtVentaSemanal.Columns.Add("NroVisa", typeof(string));
                dtVentaSemanal.Columns.Add("FechaVisa", typeof(string));
                dtVentaSemanal.Columns.Add("MontoVisa", typeof(Decimal));
                dtVentaSemanal.Columns.Add("Nronc", typeof(string));
                dtVentaSemanal.Columns.Add("Fechanc", typeof(string));
                dtVentaSemanal.Columns.Add("Montonc", typeof(Decimal));
                dtVentaSemanal.Columns.Add("FechaSaldoant", typeof(string));
                dtVentaSemanal.Columns.Add("MontoSaldoant", typeof(Decimal));
                dtVentaSemanal.Columns.Add("TotalPagos", typeof(Decimal));
                dtVentaSemanal.Columns.Add("SaldoFavor", typeof(Decimal));
                dtVentaSemanal.Columns.Add("Id", typeof(int));

                List<Ent_Venta_Semanal> _listar_Venta_Semanal = datDocumento_Transaccion.Listar_Venta_Semanal(Ent_Lista_Venta_Semanal);
                //Filtra y agrupa por Dtv_Clear no null
                var ListarVentas = _listar_Venta_Semanal.Where(w => w.Dtv_Clear != string.Empty).GroupBy(x => new { x.Dtv_Clear }).Select(y => new { Cabecera = y.Key, Detalle = y.Select(m => new { Dtv_Clear = m.Dtv_Clear, Promotor = m.Promotor, DniRuc = m.DniRuc, Ped = m.Ped, BolFact = m.BolFact, FechaDoc = m.FechaDoc, MontoFac = m.MontoFac, TotalPagos = m.TotalPagos, SaldoFavor = m.SaldoFavor, NroVouBcp = m.NroVouBcp, FechavouBcp = m.FechavouBcp, MontoVouBcp = m.MontoVouBcp, NroVisa = "", FechaVisa = "", MontoVisa = "", Nronc = m.Nronc, Fechanc = m.Fechanc, Montonc = m.Montonc, FechaSaldoant = m.FechaSaldoant, MontoSaldoant = m.MontoSaldoant }) }).ToList();

                var Fila = 0; //Inicia la fila
                var vc = 0; //Inicia Vouches
                var nc = 0; //Inicia Nota de credito
                var sl = 0; //Inicia Saldo
                var VM = 0; //Inicia el valor mayor, configura la cantidad de filas por grupo.
                string Promotor="";
                string DniRuc = "";
                foreach (var c in ListarVentas)
                {
                    VM = 0;
                    var BalFac = c.Detalle.Where(w => w.Promotor != string.Empty && w.Dtv_Clear != string.Empty).Select(s => new { Dtv_Clear = s.Dtv_Clear, Promotor = s.Promotor, DniRuc = s.DniRuc, Ped = s.Ped, BolFact = s.BolFact, FechaDoc = s.FechaDoc, MontoFac = s.MontoFac, TotalPagos = s.TotalPagos, SaldoFavor = s.SaldoFavor }).Distinct().ToList();
                    VM = (VM > BalFac.Count() ? VM : BalFac.Count());
                    var Vouch = c.Detalle.Where(w => w.NroVouBcp != string.Empty).Select(s => new { Dtv_Clear = s.Dtv_Clear, NroVouBcp = s.NroVouBcp, FechavouBcp = s.FechavouBcp, MontoVouBcp = s.MontoVouBcp }).OrderBy(x => x.NroVouBcp).ToList();
                    VM = (VM > Vouch.Count() ? VM : Vouch.Count());
                    var NC = c.Detalle.Where(w => w.Nronc != string.Empty).Select(s => new { Dtv_Clear = s.Dtv_Clear, Nronc = s.Nronc, Fechanc = s.Fechanc, Montonc = s.Montonc, }).OrderBy(x => x.Nronc).ToList();
                    VM = (VM > NC.Count() ? VM : NC.Count());
                    var Saldo = c.Detalle.Where(w => w.FechaSaldoant != string.Empty).Select(s => new { Dtv_Clear = s.Dtv_Clear, FechaSaldoant = s.FechaSaldoant, MontoSaldoant = s.MontoSaldoant }).ToList();
                    VM = (VM > Saldo.Count() ? VM : Saldo.Count());

                    //Arma la cantidad de filas por grupos
                    for (int i = 0; i < VM; i++)
                    {
                        dtVentaSemanal.Rows.Add();
                    }

                    foreach (var d in BalFac)
                    {
                        Promotor = d.Promotor;
                        DniRuc = d.DniRuc;
                        dtVentaSemanal.Rows[Fila]["Id"] = 1;
                        dtVentaSemanal.Rows[Fila]["Dtv_Clear"] = d.Dtv_Clear;
                        dtVentaSemanal.Rows[Fila]["Promotor"] = d.Promotor;
                        dtVentaSemanal.Rows[Fila]["DniRuc"] = d.DniRuc;
                        dtVentaSemanal.Rows[Fila]["Ped"] = d.Ped;
                        dtVentaSemanal.Rows[Fila]["BolFact"] = d.BolFact;
                        dtVentaSemanal.Rows[Fila]["FechaDoc"] = d.FechaDoc;
                        dtVentaSemanal.Rows[Fila]["MontoFac"] = d.MontoFac;
                        dtVentaSemanal.Rows[Fila]["TotalPagos"] = d.TotalPagos;
                        dtVentaSemanal.Rows[Fila]["SaldoFavor"] = d.SaldoFavor;
                    }
                    vc = 0;
                    foreach (var d in Vouch)
                    {
                        dtVentaSemanal.Rows[Fila + vc]["Dtv_Clear"] = d.Dtv_Clear;
                        dtVentaSemanal.Rows[Fila + vc]["Promotor"] = Promotor;
                        dtVentaSemanal.Rows[Fila + vc]["DniRuc"] = DniRuc;
                        dtVentaSemanal.Rows[Fila + vc]["NroVouBcp"] = d.NroVouBcp;
                        dtVentaSemanal.Rows[Fila + vc]["FechavouBcp"] = d.FechavouBcp;
                        dtVentaSemanal.Rows[Fila + vc]["MontoVouBcp"] = d.MontoVouBcp;
                        vc++;
                    }
                    nc = 0;
                    foreach (var d in NC)
                    {
                        dtVentaSemanal.Rows[Fila + nc]["Dtv_Clear"] = d.Dtv_Clear;
                        dtVentaSemanal.Rows[Fila + nc]["Promotor"] = Promotor;
                        dtVentaSemanal.Rows[Fila + nc]["DniRuc"] = DniRuc;
                        dtVentaSemanal.Rows[Fila + nc]["Nronc"] = d.Nronc;
                        dtVentaSemanal.Rows[Fila + nc]["Fechanc"] = d.Fechanc;
                        dtVentaSemanal.Rows[Fila + nc]["Montonc"] = d.Montonc;
                        nc++;
                    }
                    sl = 0;
                    foreach (var d in Saldo)
                    {
                        dtVentaSemanal.Rows[Fila + sl]["Dtv_Clear"] = d.Dtv_Clear;
                        dtVentaSemanal.Rows[Fila + sl]["Promotor"] = Promotor;
                        dtVentaSemanal.Rows[Fila + sl]["DniRuc"] = DniRuc;
                        dtVentaSemanal.Rows[Fila + sl]["FechaSaldoant"] = d.FechaSaldoant;
                        dtVentaSemanal.Rows[Fila + sl]["MontoSaldoant"] = d.MontoSaldoant;
                        sl++;
                    }
                    DniRuc = "";
                    Promotor = "";
                    Fila += (VM);
                }

                IList<Ent_Venta_Semanal> _Listar_Venta_Semanal = dtVentaSemanal.AsEnumerable().Select(row =>
                    new Ent_Venta_Semanal
                    {
                        Id = row.Field<int?>("Id"),
                        Dtv_Clear = row.Field<string>("Dtv_Clear"),
                        Promotor = row.Field<string>("Promotor"),
                        DniRuc = row.Field<string>("DniRuc"),
                        Ped = row.Field<string>("Ped"),
                        BolFact = row.Field<string>("BolFact"),
                        FechaDoc = row.Field<string>("FechaDoc"),
                        MontoFac = row.Field<Decimal?>("MontoFac"),
                        NroVouBcp = row.Field<string>("NroVouBcp"),
                        FechavouBcp = row.Field<string>("FechavouBcp"),
                        MontoVouBcp = row.Field<Decimal?>("MontoVouBcp"),
                        NroVisa = row.Field<string>("NroVisa"),
                        FechaVisa = row.Field<string>("FechaVisa"),
                        MontoVisa = row.Field<Decimal?>("MontoVisa"),
                        Nronc = row.Field<string>("Nronc"),
                        Fechanc = row.Field<string>("Fechanc"),
                        Montonc = row.Field<Decimal?>("Montonc"),
                        FechaSaldoant = row.Field<string>("FechaSaldoant"),
                        MontoSaldoant = row.Field<Decimal?>("MontoSaldoant"),
                        TotalPagos = row.Field<Decimal?>("TotalPagos"),
                        SaldoFavor = row.Field<Decimal?>("SaldoFavor")
                    }).ToList();

                 Session[_session_ListarVentaSemanal] = _Listar_Venta_Semanal;
            }

            /*verificar si esta null*/
            if (Session[_session_ListarVentaSemanal] == null)
            {
                List<Ent_Venta_Semanal> Listar_Venta_Semanal = new List<Ent_Venta_Semanal>();
                Session[_session_ListarVentaSemanal] = Listar_Venta_Semanal;
            }

            IQueryable<Ent_Venta_Semanal> entDocTrans = ((List<Ent_Venta_Semanal>)(Session[_session_ListarVentaSemanal])).AsQueryable();

            //Manejador de filtros
            int totalCount = entDocTrans.Count();
            IEnumerable<Ent_Venta_Semanal> filteredMembers = entDocTrans;
            if (!string.IsNullOrEmpty(param.sSearch))
            {                
                filteredMembers = entDocTrans.Where(
                        m =>
                        (!string.IsNullOrEmpty(m.Promotor) && m.Promotor.ToUpper().Contains(param.sSearch.ToUpper())) ||
                        (!string.IsNullOrEmpty(m.DniRuc) && m.DniRuc.ToUpper().Contains(param.sSearch.ToUpper())) ||
                        (!string.IsNullOrEmpty(m.Ped) && m.Ped.ToUpper().Contains(param.sSearch.ToUpper())) ||
                        (!string.IsNullOrEmpty(m.BolFact) && m.BolFact.ToUpper().Contains(param.sSearch.ToUpper())) ||
                        (!string.IsNullOrEmpty(m.FechaDoc) && m.FechaDoc.ToUpper().Contains(param.sSearch.ToUpper())) 
                );
            }
            //Manejador de orden
            var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);

            if (param.iSortingCols > 0)
            {
                if (Request["sSortDir_0"].ToString() == "asc")
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderBy(o => o.Dtv_Clear); break;
                        case 1: filteredMembers = filteredMembers.OrderBy(o => o.Promotor); break;
                        case 2: filteredMembers = filteredMembers.OrderBy(o => o.DniRuc); break;
                        case 3: filteredMembers = filteredMembers.OrderBy(o => o.Ped); break;
                        case 4: filteredMembers = filteredMembers.OrderBy(o => o.BolFact); break;
                        case 5: filteredMembers = filteredMembers.OrderBy(o => o.FechaDoc); break;
                        case 6: filteredMembers = filteredMembers.OrderBy(o => Convert.ToDecimal(o.MontoFac)); break;
                        case 7: filteredMembers = filteredMembers.OrderBy(o => o.NroVouBcp); break;
                        case 8: filteredMembers = filteredMembers.OrderBy(o => o.FechavouBcp); break;
                        case 9: filteredMembers = filteredMembers.OrderBy(o => Convert.ToDecimal(o.MontoVouBcp)); break;
                        case 10: filteredMembers = filteredMembers.OrderBy(o => o.NroVisa); break;
                        case 11: filteredMembers = filteredMembers.OrderBy(o => o.FechaVisa); break;
                        case 12: filteredMembers = filteredMembers.OrderBy(o => Convert.ToDecimal(o.MontoVisa)); break;
                        case 13: filteredMembers = filteredMembers.OrderBy(o => o.Nronc); break;
                        case 14: filteredMembers = filteredMembers.OrderBy(o => o.Fechanc); break;
                        case 15: filteredMembers = filteredMembers.OrderBy(o => Convert.ToDecimal(o.Montonc)); break;
                        case 16: filteredMembers = filteredMembers.OrderBy(o => o.FechaSaldoant); break;
                        case 17: filteredMembers = filteredMembers.OrderBy(o => Convert.ToDecimal(o.MontoSaldoant)); break;
                        case 18: filteredMembers = filteredMembers.OrderBy(o => Convert.ToDecimal(o.TotalPagos)); break;
                        case 19: filteredMembers = filteredMembers.OrderBy(o => Convert.ToDecimal(o.SaldoFavor)); break;
                    }
                }
                else
                {
                    switch (sortIdx)
                    {                        
                        case 0: filteredMembers = filteredMembers.OrderByDescending(o => o.Dtv_Clear); break;
                        case 1: filteredMembers = filteredMembers.OrderByDescending(o => o.Promotor); break;
                        case 2: filteredMembers = filteredMembers.OrderByDescending(o => o.DniRuc); break;
                        case 3: filteredMembers = filteredMembers.OrderByDescending(o => o.Ped); break;
                        case 4: filteredMembers = filteredMembers.OrderByDescending(o => o.BolFact); break;
                        case 5: filteredMembers = filteredMembers.OrderByDescending(o => o.FechaDoc); break;
                        case 6: filteredMembers = filteredMembers.OrderByDescending(o => Convert.ToDecimal(o.MontoFac)); break;
                        case 7: filteredMembers = filteredMembers.OrderByDescending(o => o.NroVouBcp); break;
                        case 8: filteredMembers = filteredMembers.OrderByDescending(o => o.FechavouBcp); break;
                        case 9: filteredMembers = filteredMembers.OrderByDescending(o => Convert.ToDecimal(o.MontoVouBcp)); break;
                        case 10: filteredMembers = filteredMembers.OrderByDescending(o => o.NroVisa); break;
                        case 11: filteredMembers = filteredMembers.OrderByDescending(o => o.FechaVisa); break;
                        case 12: filteredMembers = filteredMembers.OrderByDescending(o => Convert.ToDecimal(o.MontoVisa)); break;
                        case 13: filteredMembers = filteredMembers.OrderByDescending(o => o.Nronc); break;
                        case 14: filteredMembers = filteredMembers.OrderByDescending(o => o.Fechanc); break;
                        case 15: filteredMembers = filteredMembers.OrderByDescending(o => Convert.ToDecimal(o.Montonc)); break;
                        case 16: filteredMembers = filteredMembers.OrderByDescending(o => o.FechaSaldoant); break;
                        case 17: filteredMembers = filteredMembers.OrderByDescending(o => Convert.ToDecimal(o.MontoSaldoant)); break;
                        case 18: filteredMembers = filteredMembers.OrderByDescending(o => Convert.ToDecimal(o.TotalPagos)); break;
                        case 19: filteredMembers = filteredMembers.OrderByDescending(o => Convert.ToDecimal(o.SaldoFavor)); break;
                    }
                }
            }

            var displayMembers = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);

            var Result = from a in displayMembers
                         select new
                         {
                            a.Id,
                            a.Dtv_Clear,
                            a.Promotor,
                            a.DniRuc,
                            a.Ped,
                            a.BolFact,
                            a.FechaDoc,
                            a.MontoFac,
                            a.NroVouBcp,
                            a.FechavouBcp,
                            a.MontoVouBcp,
                            a.NroVisa,
                            a.FechaVisa,
                            a.MontoVisa,
                            a.Nronc,
                            a.Fechanc,
                            a.Montonc,
                            a.FechaSaldoant,
                            a.MontoSaldoant,
                            a.TotalPagos,
                            a.SaldoFavor
                         };

            //Se devuelven los resultados por json
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalCount,
                iTotalDisplayRecords = filteredMembers.Count(),
                aaData = Result
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Valida la cantidad de data y crea el excel
        /// </summary>
        /// <param name="_Ent"></param>
        /// <returns></returns>
        public ActionResult get_exporta_ListaVentaSemanal_excel(Ent_Venta_Semanal _Ent)
        {
            JsonResponse objResult = new JsonResponse();
            try
            {
                Session[_session_ListarVentaSemanal_Excel] = null;
                string cadena = "";
                if (Session[_session_ListarVentaSemanal] != null)
                {

                    List<Ent_Venta_Semanal> Listar_Venta_Semanal = (List<Ent_Venta_Semanal>)Session[_session_ListarVentaSemanal];
                    if (Listar_Venta_Semanal.Count == 0)
                    {
                        objResult.Success = false;
                        objResult.Message = "No hay filas para exportar";
                    }
                    else
                    {
                        cadena = get_html_ListaVentaSemanal_str((List<Ent_Venta_Semanal>)Session[_session_ListarVentaSemanal], _Ent);
                        if (cadena.Length == 0)
                        {
                            objResult.Success = false;
                            objResult.Message = "Error del formato html";
                        }
                        else
                        {
                            objResult.Success = true;
                            objResult.Message = "Se genero el excel correctamente";
                            Session[_session_ListarVentaSemanal_Excel] = cadena;
                        }
                    }
                }
                else
                {
                    objResult.Success = false;
                    objResult.Message = "No hay filas para exportar";
                }

            }
            catch (Exception ex)
            {
                objResult.Success = false;
                objResult.Message = "No hay filas para exportar";
            }

            var JSON = JsonConvert.SerializeObject(objResult);

            return Json(JSON, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Genera el excel del ReporteSemanal
        /// </summary>
        /// <param name="ListaVentaSemanal"></param>
        /// <param name="ent"></param>
        /// <returns></returns>
        public string get_html_ListaVentaSemanal_str(List<Ent_Venta_Semanal> ListaVentaSemanal, Ent_Venta_Semanal ent)
        {
            StringBuilder sb = new StringBuilder();
            int intcounT = 0;
            int intCell = 1;
            try
            {
                //Lista por grupos
                var Lista = ListaVentaSemanal
               .GroupBy(x => new
               {
                   x.Dtv_Clear
               }).Select(y => new
               {
                   Padre = y.Key,
                   Hijos = y.Select(m => new
                   {
                       Id = m.Id,
                       Dtv_Clear = m.Dtv_Clear,
                       Promotor = m.Promotor,
                       DniRuc = m.DniRuc,
                       Ped = m.Ped,
                       BolFact = m.BolFact,
                       FechaDoc = m.FechaDoc,
                       MontoFac = m.MontoFac,

                       TotalPagos = m.TotalPagos,
                       SaldoFavor = m.SaldoFavor,

                       NroVouBcp = m.NroVouBcp,
                       FechavouBcp = m.FechavouBcp,
                       MontoVouBcp = m.MontoVouBcp,
                       NroVisa = m.NroVisa,
                       FechaVisa = m.FechaVisa,
                       MontoVisa = m.MontoVisa,

                       Nronc = m.Nronc,
                       Fechanc = m.Fechanc,
                       Montonc = m.Montonc,
                       FechaSaldoant = m.FechaSaldoant,
                       MontoSaldoant = m.MontoSaldoant

                   })
               }).ToList();

                sb.Append("<div><table cellspacing='0' rules='all' border='0' style='border-collapse:collapse;'><tr><td Colspan='19'></td></tr><tr><td Colspan='19' valign='middle' align='center' style='vertical-align: middle;font-size: 16.0pt;font-weight: bold;color:#285A8F'>REPORTE DE VENTA SEMANAL</td></tr><tr><td Colspan='19'  valign='middle' align='center' style='font-size: 10.0pt;font-weight: bold;vertical-align: middle'>DESDE EL " + String.Format("{0:dd/MM/yyyy}", ent.FechaInicio) + " HASTA EL " + String.Format("{0:dd/MM/yyyy}", ent.FechaFin) + "</td></tr></table>");
                sb.Append("<table  border='1' bgColor='#ffffff' borderColor='#FFFFFF' cellSpacing='5' cellPadding='5' style='font-size:10.0pt; font-family:Calibri; background:white;'><tr  bgColor='#5799bf'>\n");
                sb.Append("<tr bgColor='#5799bf'>\n");
                sb.Append("<th colspan='3'></th>\n");
                sb.Append("<th colspan='3' style='text-align: center;'><font color='#FFFFFF'>Datos del Documento Boleta o Factura</font></th>\n");
                sb.Append("<th colspan='3' style='text-align: center;'><font color='#FFFFFF'>Deposito del Banco de Credito</font></th>\n");
                sb.Append("<th colspan='3' style='text-align: center;'><font color='#FFFFFF'>Deposito de Visa Unica</font></th>\n");
                sb.Append("<th colspan='3' style='text-align: center;'><font color='#FFFFFF'>Datos de la Nota de Credito</font></th>\n");
                sb.Append("<th colspan='2' style='text-align: center;'><font color='#FFFFFF'>Saldo Anterior</font></th>\n");
                sb.Append("<th colspan='2'></th>\n");
                sb.Append("</tr>\n");
                sb.Append("<tr bgColor='#5799bf'>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Promotor</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Dni/Ruc</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Ped</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Número</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Fecha</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Monto</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Número</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Fecha</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Monto</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Número</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Fecha</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Monto</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Número</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Fecha</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Monto</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Fecha</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Monto</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Tot. Pagar</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Sal. Favor</font></th>\n");
                sb.Append("</tr>\n");

                string tdSColor = "";
                string tdBorder = "";
                Decimal? Stotal = 0;
                foreach (var itemP in Lista)
                {
                    foreach (var itemH in itemP.Hijos)
                    {
                        if (itemH.MontoFac != null) Stotal += itemH.MontoFac;
                        sb.Append("<tr>\n");
                       if (itemH.Id == 1) tdBorder = "border-top:solid 1px;";

                        if (intcounT == 0)
                        {
                            intcounT = itemP.Hijos.Count();
                            sb.Append("<td bgcolor='#7FCCF4' style='vertical-align: middle; "  + tdBorder + "' rowspan='" + intcounT + "'>" + ((itemH.Promotor == null) ? string.Empty : itemH.Promotor) + "</td>\n");
                            sb.Append("<td bgcolor='#7FCCF4' style='text-align: center; vertical-align: middle; " + tdBorder + "' rowspan='" + intcounT + "'>" + ((itemH.DniRuc == null) ? string.Empty : itemH.DniRuc) + "</td>\n");
                        }
                                      
                        sb.Append("<td bgcolor='#7FCCF4' align='center' style='" + tdBorder + "'>" + ((itemH.Ped == null) ? string.Empty : itemH.Ped) + "</td>\n");
                        sb.Append("<td bgcolor='#B1DEF6' align='center' style='" + tdBorder + "'>" + ((itemH.BolFact == null) ? string.Empty : itemH.BolFact) + "</td>\n");
                        sb.Append("<td bgcolor='#B1DEF6' align='center' style='" + tdBorder + "'>" + ((itemH.FechaDoc == null) ? string.Empty : itemH.FechaDoc) + "</td>\n");
                        sb.Append("<td bgcolor='#B1DEF6' align='right' style='" + tdBorder + "'>" + ((itemH.MontoFac == null) ? (Decimal?)null : Convert.ToDecimal(string.Format("{0:F2}", itemH.MontoFac))) + "</td>\n");
                        sb.Append("<td bgcolor='#B1DEF6' align='center' style='" + tdBorder + "'>" + ((itemH.NroVouBcp == null) ? string.Empty : itemH.NroVouBcp) + "</td>\n");
                        sb.Append("<td bgcolor='#B1DEF6' align='center'style='" + tdBorder + "'>" + ((itemH.FechavouBcp == null) ? string.Empty : itemH.FechavouBcp) + "</td>\n");
                        sb.Append("<td bgcolor='#B1DEF6' align='right' style='" + tdBorder + "'>" + ((itemH.MontoVouBcp == null) ? (Decimal?)null : Convert.ToDecimal(string.Format("{0:F2}", itemH.MontoVouBcp))) + "</td>\n");
                        sb.Append("<td align='center' style='" + tdBorder + "'>"  + ((itemH.NroVisa == null) ? string.Empty : itemH.NroVisa) + "</td>\n");
                        sb.Append("<td align='center' style='" + tdBorder + "'>" + ((itemH.FechaVisa == null) ? string.Empty : itemH.FechaVisa) + "</td>\n");
                        sb.Append("<td align='right' style='" + tdBorder + "'>" + ((itemH.MontoVisa == null) ? (Decimal?)null : Convert.ToDecimal(string.Format("{0:F2}", itemH.MontoVisa))) + "</td>\n");
                        sb.Append("<td align='center' style='" + tdBorder + "'>" + ((itemH.Nronc == null) ? string.Empty : itemH.Nronc) + "</td>\n");
                        sb.Append("<td align='center' style='" + tdBorder + "'>" + ((itemH.Fechanc == null) ? string.Empty : itemH.Fechanc) + "</td>\n");
                        sb.Append("<td align='right' style='" + tdBorder + "'>" + ((itemH.Montonc == null) ? (Decimal?)null : Convert.ToDecimal(string.Format("{0:F2}", itemH.Montonc))) + "</td>\n");
                        sb.Append("<td align='center' style='" + tdBorder + "'>" + ((itemH.FechaSaldoant == null) ? string.Empty : itemH.FechaSaldoant) + "</td>\n");
                        sb.Append("<td align='right' style='" + tdBorder + "'>" + ((itemH.MontoSaldoant == null) ? (Decimal?)null : Convert.ToDecimal(string.Format("{0:F2}", itemH.MontoSaldoant))) + "</td>\n");
                        sb.Append("<td bgcolor='#0397E7' align='right' style='font-weight: bold; " + tdBorder + "'><font color='#FFFFFF'>" + ((itemH.TotalPagos == null) ? (Decimal?)null : Convert.ToDecimal(string.Format("{0:F2}", itemH.TotalPagos))) + "</font></td>\n");
                        sb.Append("<td bgcolor='#0397E7' align='right' style='font-weight: bold; " + tdBorder + "''><font color='#FFFFFF'>" + ((itemH.SaldoFavor == null) ? (Decimal?)null : Convert.ToDecimal(string.Format("{0:F2}", itemH.SaldoFavor))) + "</font></td>\n");
                                               
                        sb.Append("</tr>\n");
                        intCell++;
                        tdBorder = "";
                    }
                    intcounT = 0;
                   
                    intCell = 1;
                }
                
                sb.Append("<tfoot>\n");
                sb.Append("<tr bgcolor='#0397E7'>\n");
                sb.Append("<td colspan='4'></td>\n");
                sb.Append("<td style='text-align:center;font-weight: bold;font-size:11.0pt; '><font color='#FFFFFF'>TOTAL</font></td>\n");
                sb.Append("<td style='text-align:right;font-weight: bold;font-size:11.0pt; '><font color='#FFFFFF'>" + Convert.ToDecimal(string.Format("{0:F2}", Stotal)) + "</font></td>\n");
                sb.Append("<td colspan ='13'></td>\n");

                sb.Append("</tr>\n");
                sb.Append("</tfoot>\n");
                sb.Append("</table></div>");
            }
            catch
            {

            }
            return sb.ToString();
        }
        /// <summary>
        /// Exporta el excel
        /// </summary>
        /// <returns></returns>
        public ActionResult ListaVentaSemanalExcel()
        {
            string NombreArchivo = "VentaSemanal";
            String style = style = @"<style> .textmode { mso-number-format:\@; } </script> ";
            try
            {
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", "attachment;filename=" + NombreArchivo + ".xls");
                Response.Charset = "UTF-8";
                Response.ContentEncoding = Encoding.Default;
                Response.Write(style);
                Response.Write(Session[_session_ListarVentaSemanal_Excel].ToString());
                Response.End();
            }
            catch
            {

            }
            return Json(new { estado = 0, mensaje = 1 });
        }
        #endregion

        #region <GENERAR SALDO>
        /// <summary>
        /// Genera saldo de anticpos a la nota de credito
        /// </summary>
        /// <returns></returns>
        public ActionResult Generar_Saldos()
        {
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
                List<Ent_Saldos_Anticipos> LisSaldosAnticipos = new List<Ent_Saldos_Anticipos>();
                ViewBag.LisSaldos = LisSaldosAnticipos;

                Ent_Saldos_Anticipos entSaldosAnticipos = new Ent_Saldos_Anticipos();
                ViewBag.entSaldo = entSaldosAnticipos;

                ViewBag.ListarSaldosAnticipos = datDocumento_Transaccion.Listar_Saldos_Anticipos().ToList();
                return View();
            }
        }
        /// <summary>
        /// Listar los clientes para hacer anticipos en las notas de credito.
        /// </summary>
        /// <param name="param"></param>
        /// <param name="isOkUpdate"></param>
        /// <returns>JSON</returns>
        public JsonResult getListaSaldosAnticiposAjax(Ent_jQueryDataTableParams param,bool isOkUpdate, bool isOkSaldo,Decimal Saldo)
        {
            List<Ent_Saldos_Anticipos> ListarSaldosAnticipos;

            if (isOkUpdate)
            {
                ListarSaldosAnticipos = (Saldo == 0) ? datDocumento_Transaccion.Listar_Saldos_Anticipos().ToList() : datDocumento_Transaccion.Listar_Saldos_Anticipos().Where(x => x.Saldo <= Saldo).ToList();
                Session[_session_listSaldosAnticipos] = ListarSaldosAnticipos;
            }

            /*verificar si esta null*/
            if (Session[_session_listSaldosAnticipos] == null)
            {
                List<Ent_Saldos_Anticipos> Lista_Saldos_Anticipos = new List<Ent_Saldos_Anticipos>();
                Session[_session_listSaldosAnticipos] = Lista_Saldos_Anticipos;
            }

            IQueryable<Ent_Saldos_Anticipos> entDocTrans = ((List<Ent_Saldos_Anticipos>)(Session[_session_listSaldosAnticipos])).AsQueryable();

            //Manejador de filtros
            int totalCount = entDocTrans.Count();
            IEnumerable<Ent_Saldos_Anticipos> filteredMembers = entDocTrans;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = entDocTrans
                    .Where(m =>
                            m.Documento.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Cliente.ToUpper().Contains(param.sSearch.ToUpper()) 
                            );
            }

            //Manejador de ordene
            var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);

            if (param.iSortingCols > 0)
            {
                if (Request["sSortDir_0"].ToString() == "asc")
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderBy(o => o.Documento); break;
                        case 1: filteredMembers = filteredMembers.OrderBy(o => o.Cliente); break;
                        case 2: filteredMembers = filteredMembers.OrderBy(o => o.Saldo); break;
                        case 3: filteredMembers = filteredMembers.OrderBy(o => o.SerieFac); break;
                        case 4: filteredMembers = filteredMembers.OrderBy(o => o.NumeroFac); break;
                        case 5: filteredMembers = filteredMembers.OrderBy(o => o.Fec_Fac); break;
                        case 6: filteredMembers = filteredMembers.OrderBy(o => o.MontoFac); break;
                        case 7: filteredMembers = filteredMembers.OrderBy(o => o.SerieNc); break;
                        case 8: filteredMembers = filteredMembers.OrderBy(o => o.NumeroNc); break;
                        case 9: filteredMembers = filteredMembers.OrderBy(o => o.Fec_Nc); break;
                        case 10: filteredMembers = filteredMembers.OrderBy(o => o.MontoNc); break;
                        case 11: filteredMembers = filteredMembers.OrderBy(o => o.Monto_Util); break;
                        case 12: filteredMembers = filteredMembers.OrderBy(o => o.Percepcion); break;
                        case 13: filteredMembers = filteredMembers.OrderBy(o => o.Chk); break;
                        case 14: filteredMembers = filteredMembers.OrderBy(o => o.Bas_Id); break;
                    }
                }
                else
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderByDescending(o => o.Documento); break;
                        case 1: filteredMembers = filteredMembers.OrderByDescending(o => o.Cliente); break;
                        case 2: filteredMembers = filteredMembers.OrderByDescending(o => o.Saldo); break;
                        case 3: filteredMembers = filteredMembers.OrderByDescending(o => o.SerieFac); break;
                        case 4: filteredMembers = filteredMembers.OrderByDescending(o => o.NumeroFac); break;
                        case 5: filteredMembers = filteredMembers.OrderByDescending(o => o.Fec_Fac); break;
                        case 6: filteredMembers = filteredMembers.OrderByDescending(o => o.MontoFac); break;
                        case 7: filteredMembers = filteredMembers.OrderByDescending(o => o.SerieNc); break;
                        case 8: filteredMembers = filteredMembers.OrderByDescending(o => o.NumeroNc); break;
                        case 9: filteredMembers = filteredMembers.OrderByDescending(o => o.Fec_Nc); break;
                        case 10: filteredMembers = filteredMembers.OrderByDescending(o => o.MontoNc); break;
                        case 11: filteredMembers = filteredMembers.OrderByDescending(o => o.Monto_Util); break;
                        case 12: filteredMembers = filteredMembers.OrderByDescending(o => o.Percepcion); break;
                        case 13: filteredMembers = filteredMembers.OrderByDescending(o => o.Chk); break;
                        case 14: filteredMembers = filteredMembers.OrderByDescending(o => o.Bas_Id); break;
                    }
                }
            }

            var Result = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);

            //Se devuelven los resultados por json
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalCount,
                iTotalDisplayRecords = filteredMembers.Count(),
                aaData = Result,
                iDisplayStart = param.iDisplayStart
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Genera/Registrar los saldos anticipos
        /// </summary>
        /// <param name="_LisSaldo"></param>
        /// <returns></returns>
        public ActionResult getGenerarAnticipos(List<Ent_Saldos_Anticipos> _LisSaldo)
        {
            bool Result = false;
            JsonResponse objResult = new JsonResponse();
            try
            {
                Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
                Ent_Saldos_Anticipos Ent = new Ent_Saldos_Anticipos();
                Ent.usu_ingreso = _usuario.usu_id;
                DataTable dtSaldos = new DataTable();
                dtSaldos.Columns.Add("Bas_Id", typeof(int));
                dtSaldos.Columns.Add("Monto_Util", typeof(Decimal));
                dtSaldos.Columns.Add("NumeroFac", typeof(string));
                dtSaldos.Columns.Add("Fec_Fac", typeof(string));
                dtSaldos.Columns.Add("NumeroNc", typeof(string));
                dtSaldos.Columns.Add("Fec_Nc", typeof(string));
                var Fila = 0;
                foreach (var item in _LisSaldo)
                {
                    dtSaldos.Rows.Add();
                    dtSaldos.Rows[Fila]["Bas_Id"] = item.Bas_Id;
                    dtSaldos.Rows[Fila]["Monto_Util"] = item.Monto_Util;
                    dtSaldos.Rows[Fila]["NumeroFac"] = item.SerieFac.ToString() + item.NumeroFac.ToString();
                    dtSaldos.Rows[Fila]["Fec_Fac"] = item.Fec_Fac;
                    dtSaldos.Rows[Fila]["NumeroNc"] = item.SerieNc.ToString() + item.NumeroNc.ToString();
                    dtSaldos.Rows[Fila]["Fec_Nc"] = item.Fec_Nc;
                    Fila++;
                }

                Result = datDocumento_Transaccion.Genera_Provisiones(dtSaldos, Ent);
                if (Result)
                {
                    objResult.Success = true;
                    objResult.Message = "se género las provisiones de los saldos seleccionados.";
                }
                else
                {
                    objResult.Success = false;
                    objResult.Message = "¡Error! Al generar las provisiones de los saldos seleccionados.";
                }
            }
            catch (Exception ex)
            {
                objResult.Success = false;
                objResult.Message = "Error al registrar";
            }
            var JSON = JsonConvert.SerializeObject(objResult);
            return Json(JSON, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Anula los saldos anticipos
        /// </summary>
        /// <param name="_ent"></param>
        /// <returns></returns>
        public ActionResult getAnularSaldos(Ent_Saldos_Anticipos _ent)
        {
            Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
            bool result = false;
            JsonResponse objResult = new JsonResponse();
            _ent.usu_ingreso = _usuario.usu_id;
            try
            {
                result = datDocumento_Transaccion.AnularSaldos(_ent);
                if (result)
                {
                    objResult.Success = true;
                    objResult.Message = "Se anulo los items seleccionados.";
                }
                else
                {
                    objResult.Success = false;
                    objResult.Message = "Error al anular los items seleccionados.";
                }
            }
            catch (Exception exc)
            {
                objResult.Success = false;
                objResult.Message = "Error al anular";
            }

            var JSON = JsonConvert.SerializeObject(objResult);
            return Json(JSON, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region <VALIDACIONES DE PAGOS>
        public ActionResult validar_Pagos()
        {
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
                var ListarBancos = datBanco.Listar_Bancos().Where(x => x.Codigo == "1" || x.Codigo == "4").ToList();
                ViewBag.ListarBancos = ListarBancos;

                Ent_Validar_Pagos EntValidarPagos = new Ent_Validar_Pagos();
                ViewBag.EntValidarPagos = EntValidarPagos;

                return View();
            }
        }
        /// <summary>
        /// Validamos el archivo entrante.
        /// </summary>
        /// <returns></returns>
        public ActionResult getValidarArchivo()
        {
            Session[_session_ListarValidarPagos] = null;
            Ent_Validar_Pagos _Ent = new Ent_Validar_Pagos();
            var Archivo = Request.Files[0];
            int NumBanco = Convert.ToInt32(Request.Form[0]);
            int NumTipoArchivo = Convert.ToInt32(Request.Form[1]);
            JsonResponse objResult = new JsonResponse();
            var JSON="";
            bool rpta = false;
            try
            {
                string FileName = Path.GetFileName(Archivo.FileName);
                string Extension = Path.GetExtension(Archivo.FileName);
                string FolderPath = Path.GetTempPath();
                Random r = new Random();
                int varAleatorio = r.Next(0, 999);

                if (NumBanco != 4)
                {
                    string FilePath = Path.Combine(FolderPath + FileName + varAleatorio);
                    Archivo.SaveAs(FilePath);

                    rpta = validarArchivo(FilePath, Extension, NumTipoArchivo);
                    if (rpta)
                    {
                    
                        Import_To_Grid(FilePath, Extension, NumTipoArchivo);
                        objResult.Data = NumBanco; 
                        objResult.Success = true;
                        objResult.Message = "Archivo correcto";
                    }
                    else
                    {
                        objResult.Data = NumBanco;
                        objResult.Success = false;
                        objResult.Message= "Debe seleccionar un tipo de archivo correcto";
                    }

                     JSON = JsonConvert.SerializeObject(objResult);
                    return Json(JSON, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    FileName = FileName.Substring(0, FileName.Length - 4);
                    string FilePath = Path.Combine(FolderPath + FileName + varAleatorio.ToString() + Extension);
                    Archivo.SaveAs(FilePath);

                    string[] lineas = System.IO.File.ReadAllLines(FilePath);

                    DataTable dtinter = new DataTable();
                    dtinter.Columns.Add("DniRuc", typeof(string));
                    dtinter.Columns.Add("Pedido", typeof(string));
                    dtinter.Columns.Add("Op", typeof(string));
                    dtinter.Columns.Add("Fecha", typeof(string));
                    dtinter.Columns.Add("Monto", typeof(string));

                    foreach (string lin in lineas)
                    {
                        string dniruc = Convert.ToString(lin.Substring(9, 20).Trim());
                        string pedido = Convert.ToString(Convert.ToInt32(lin.Substring(37, 15).Trim()).ToString());
                        string op = Convert.ToString(lin.Substring(139, 8).Trim());
                        string op_fecha = Convert.ToString(DateTime.ParseExact(lin.Substring(82, 8).Trim(), "yyyyMMdd", null, DateTimeStyles.None));
                        string enter = Convert.ToString(Convert.ToInt32(lin.Substring(96, 11).Trim()).ToString());
                        string decim = Convert.ToString(lin.Substring(96 + 11, 2).Trim().ToString());
                        string op_monto = Convert.ToString(Convert.ToDecimal(enter + "." + decim));

                        dtinter.Rows.Add(dniruc, pedido, op, op_fecha, op_monto);
                    }

                    DataTable dtreturn = datDocumento_Transaccion.getvalida_inter(dtinter);
                    if (dtreturn.Rows.Count == 0)
                    {
                        objResult.Data = NumBanco;
                        objResult.Success = false;
                        objResult.Message = "No hay datos para cargar , debido a que no esta registrado en nuestra base o ya ha sio registrado:" + DateTime.Now;
                    }
                    else
                    {
                        objResult.Data = NumBanco;
                        objResult.Success = true;
                        objResult.Message = "Solo se carga los numero de OP PENDIENTES del banco interbank. Última actualización" + DateTime.Now;

                        IList<Ent_Validar_Pagos> _Listar_Validar_Pagos = dtinter.AsEnumerable().Select(row =>
                            new Ent_Validar_Pagos
                            {
                                NumDocuemnto = row.Field<string>("DniRuc"),
                                NumPedido = row.Field<string>("Pedido"),
                                FecOperacion = row.Field<string>("Fecha"),                           
                                MonOperacion = row.Field<string>("Monto"),
                                NumOperacion = row.Field<string>("Op")
                            }).ToList();

                        Session[_session_ListarValidarPagos] = _Listar_Validar_Pagos;
                    }

                    JSON = JsonConvert.SerializeObject(objResult);
                    return Json(JSON, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                objResult.Data = NumBanco;
                objResult.Success = false;
                objResult.Message = "Debe seleccionar un tipo de archivo correcto";
                JSON = JsonConvert.SerializeObject(objResult);
                return Json(JSON, JsonRequestBehavior.AllowGet);
            }
                       
        }
        /// <summary>
        /// Validamos version y Tipo de archivo(Dia o historial)
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="Extension"></param>
        /// <param name="val_dwTipArc"></param>
        /// <returns></returns>
        public bool validarArchivo(string FilePath, string Extension, int val_dwTipArc)
        {
            bool rpta = false;
            string conStr = "";
            switch (Extension)
            {
                case ".xls": //Excel 97-03
                    conStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}; Extended Properties='Excel 8.0;HDR={1}";
                    break;

                case ".xlsx": //Excel 07
                    conStr = "Provider = Microsoft.ACE.OLEDB.12.0; Data Source={0}; Extended Properties='Excel 8.0;HDR={1};'";
                    break;
            }

            conStr = String.Format(conStr, FilePath, "A4:F5", true);
            OleDbConnection connExcel = new OleDbConnection(conStr);
            OleDbCommand cmdExcel = new OleDbCommand();
            OleDbDataAdapter oda = new OleDbDataAdapter();
            DataTable dt = new DataTable();
            cmdExcel.Connection = connExcel;
            //Get the name of First Sheet
            connExcel.Open();
            DataTable dtExcelSchema;
            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            string SheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
            connExcel.Close();
            //Read Data from First Sheet
            connExcel.Open();
            if (val_dwTipArc == 1)
            {
                cmdExcel.CommandText = "SELECT count([F3]) AS operacion From [" + SheetName + "] where [F6] = 'Nº operación' ";
            }
            else
            {
                cmdExcel.CommandText = "SELECT count([F3]) AS operacion From [" + SheetName + "] where [F7] = 'Operación - Número' ";
            }

            oda.SelectCommand = cmdExcel;
            oda.Fill(dt);

            int a = 0;
            foreach (DataRow dr in dt.Rows) 
            {
                a = System.Convert.ToInt32(dr["operacion"]);
                if (val_dwTipArc == 1)
                {
                    if (a > 0)
                    {
                        rpta = true;
                        break;
                    }
                }
                else
                {
                    if (a > 0)
                    {
                        rpta = true;
                        break;
                    }
                }
            }
            connExcel.Close();
            return rpta;
        }
        /// <summary>
        /// Creamos los temporales de los listados
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="Extension"></param>
        /// <param name="val_dwTipArc"></param>
        private void Import_To_Grid(string FilePath, string Extension, int val_dwTipArc)
        {
            string conStr = "";
            switch (Extension)
            {
                case ".xls": //Excel 97-03
                    conStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}; Extended Properties='Excel 8.0;HDR={1}";
                    break;

                case ".xlsx": //Excel 07
                    conStr = "Provider = Microsoft.ACE.OLEDB.12.0; Data Source={0}; Extended Properties='Excel 8.0;HDR={1};'";
                    break;
            }

            conStr = String.Format(conStr, FilePath, "A4:F5", true);
            OleDbConnection connExcel = new OleDbConnection(conStr);
            OleDbCommand cmdExcel = new OleDbCommand();
            OleDbDataAdapter oda = new OleDbDataAdapter();
            DataTable dtValidarPagos = new DataTable();
            dtValidarPagos.Columns.Add("FecOperacion", typeof(string));
            dtValidarPagos.Columns.Add("DesOperacion", typeof(string));
            dtValidarPagos.Columns.Add("MonOperacion", typeof(string));
            dtValidarPagos.Columns.Add("NumOperacion", typeof(string));

            try
            {
                cmdExcel.Connection = connExcel;
                //Get the name of First Sheet
                connExcel.Open();

                DataTable dtExcelSchema;
                dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                string SheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                connExcel.Close();

                //Read Data from First Sheet
                connExcel.Open();

                if (val_dwTipArc == 1)
                {
                    cmdExcel.CommandText = "SELECT [Cuenta] as FecOperacion,[F3] as DesOperacion ,[F4] as MonOperacion ,[F6] as NumOperacion From [" + SheetName + "] where MID([F3], 1, 8) = 'EFECTIVO' ";
                }
                else
                {
                    cmdExcel.CommandText = "SELECT [Cuenta] as FecOperacion,[F3] as DesOperacion,[F4] as MonOperacion,[F7] as NumOperacion From [" + SheetName + "] where MID([F3], 1, 8) = 'EFECTIVO' ";
                }

                oda.SelectCommand = cmdExcel;
                oda.Fill(dtValidarPagos);
                for (Int32 i = 0; i < dtValidarPagos.Rows.Count; ++i)
                {
                    string _MONTOstr = dtValidarPagos.Rows[i]["MonOperacion"].ToString().Replace(',', '.');
                    dtValidarPagos.Rows[i]["MonOperacion"] = formato_numerico(_MONTOstr);
                    if (val_dwTipArc == 1)
                    {
                        dtValidarPagos.Rows[i]["NumOperacion"] = Convert.ToString(dtValidarPagos.Rows[i]["NumOperacion"].ToString().PadLeft(8, '0'));
                    }
                    else
                    {
                        dtValidarPagos.Rows[i]["NumOperacion"] = Convert.ToString(dtValidarPagos.Rows[i]["NumOperacion"].ToString().PadLeft(8, '0'));
                    }
                }
                connExcel.Close();

                IList<Ent_Validar_Pagos> _Listar_Validar_Pagos = dtValidarPagos.AsEnumerable().Select(row =>
                    new Ent_Validar_Pagos
                    {
                        FecOperacion = row.Field<string>("FecOperacion"),
                        DesOperacion = row.Field<string>("DesOperacion"),
                        MonOperacion = row.Field<string>("MonOperacion"),
                        NumOperacion = row.Field<string>("NumOperacion")
                    }).ToList();

                Session[_session_ListarValidarPagos] = _Listar_Validar_Pagos;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }          

        }

        private decimal formato_numerico(string _valor)
        {
            decimal monto = 0;
            long b = _valor.LongCount(letra => letra.ToString() == ".");

            if (b > 1)
            {
                string cad_decimal = _valor.Substring(_valor.Length - 3, 3);
                string cad_comas = _valor.Substring(0, _valor.Length - 3);
                cad_comas = cad_comas.Replace('.', ',');
                string _numero_str = cad_comas + cad_decimal;
                monto = Convert.ToDecimal(_numero_str);
            }
            else
            {
                _valor = _valor.Replace(',', '.');
                monto = Convert.ToDecimal(_valor);
            }

            return monto;
        }
        /// <summary>
        /// Listado de la informacion que trae los archivos
        /// </summary>
        /// <param name="param"></param>
        /// <param name="isOkUpdate"></param>
        /// <returns></returns>
        public JsonResult getListarValidarPagos(Ent_jQueryDataTableParams param, bool isOkUpdate)
        {
            if (isOkUpdate)
            {
                Session[_session_ListarValidarPagos] = null;
            }
            /*verificar si esta null*/
            if (Session[_session_ListarValidarPagos] == null)
            {
                List<Ent_Validar_Pagos> Lista_Listar_Validar_Pagos = new List<Ent_Validar_Pagos>();
                Session[_session_ListarValidarPagos] = Lista_Listar_Validar_Pagos;
            }

            IQueryable<Ent_Validar_Pagos> entDocTrans = ((List<Ent_Validar_Pagos>)(Session[_session_ListarValidarPagos])).AsQueryable();

            //Manejador de filtros
            int totalCount = entDocTrans.Count();
            IEnumerable<Ent_Validar_Pagos> filteredMembers = entDocTrans;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = entDocTrans
                    .Where(m =>
                            m.NumDocuemnto.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.NumPedido.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.FecOperacion.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.DesOperacion.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.NumOperacion.ToUpper().Contains(param.sSearch.ToUpper())                           
                            );
            }

            //Manejador de ordene
            var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);

            if (param.iSortingCols > 0)
            {
                if (Request["sSortDir_0"].ToString() == "asc")
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderBy(o => o.NumDocuemnto); break;
                        case 1: filteredMembers = filteredMembers.OrderBy(o => o.NumPedido); break;
                        case 2: filteredMembers = filteredMembers.OrderBy(o => o.FecOperacion); break;
                        case 3: filteredMembers = filteredMembers.OrderBy(o => o.DesOperacion); break;
                        case 4: filteredMembers = filteredMembers.OrderBy(o => o.NumOperacion); break;
                        case 5: filteredMembers = filteredMembers.OrderBy(o => o.MonOperacion); break;
                    }
                }
                else
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderByDescending(o => o.NumDocuemnto); break;
                        case 1: filteredMembers = filteredMembers.OrderByDescending(o => o.NumPedido); break;
                        case 2: filteredMembers = filteredMembers.OrderByDescending(o => o.FecOperacion); break;
                        case 3: filteredMembers = filteredMembers.OrderByDescending(o => o.DesOperacion); break;
                        case 4: filteredMembers = filteredMembers.OrderByDescending(o => o.NumOperacion); break;
                        case 5: filteredMembers = filteredMembers.OrderByDescending(o => o.MonOperacion); break;
                    }
                }
            }

            var Result = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);

            //Se devuelven los resultados por json
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalCount,
                iTotalDisplayRecords = filteredMembers.Count(),
                aaData = Result
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Registra los archivos 
        /// </summary>
        /// <param name="NumBanco"></param>
        /// <returns></returns>
        public ActionResult getRegistrarArchivo(int NumBanco)
        {
            JsonResponse objResult = new JsonResponse();
            Ent_Validar_Pagos _Ent = new Ent_Validar_Pagos();
            Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
            var JSON = "";
            bool Result = false;
            try
            {
                IQueryable<Ent_Validar_Pagos> _ListarValidarPagos = ((List<Ent_Validar_Pagos>)(Session[_session_ListarValidarPagos])).AsQueryable();

                if (_ListarValidarPagos.Count() != 0)
                {
                    if (NumBanco != 4)
                    {
                        DataTable dtvalida = new DataTable();
                        dtvalida.Columns.Add("Pago_Fecha", typeof(DateTime));
                        dtvalida.Columns.Add("Pago_Descripcion", typeof(string));
                        dtvalida.Columns.Add("Pago_Monto", typeof(Decimal));
                        dtvalida.Columns.Add("Pago_Operacion", typeof(string));
                        dtvalida.Columns.Add("Pag_BanId", typeof(string));

                        var Fila = 0;
                        foreach (var item in _ListarValidarPagos)
                        {
                            dtvalida.Rows.Add();
                            dtvalida.Rows[Fila]["Pago_Fecha"] = Convert.ToDateTime(item.FecOperacion);
                            dtvalida.Rows[Fila]["Pago_Descripcion"] = Convert.ToString(item.DesOperacion);
                            dtvalida.Rows[Fila]["Pago_Monto"] = Convert.ToDecimal(item.MonOperacion);
                            dtvalida.Rows[Fila]["Pago_Operacion"] = Convert.ToString(item.NumOperacion);
                            dtvalida.Rows[Fila]["Pag_BanId"] = Convert.ToString(NumBanco);
                            Fila++;
                        }
                        _Ent.NumBanco = NumBanco;
                        _Ent.Usu_Validar = _usuario.usu_id;
                        
                        Result = datDocumento_Transaccion.SaveValidateBank(dtvalida, _Ent);
                        if (Result)
                        {
                            objResult.Data = NumBanco;
                            objResult.Success = true;
                            objResult.Message = "Se guardo correctamente. El archivo del banco " + " Última actualización: " + DateTime.Now;
                        }
                        else
                        {
                            objResult.Data = NumBanco;
                            objResult.Success = false;
                            objResult.Message = "Lamentablemente no se ha guardado el archivo";
                        }

                        JSON = JsonConvert.SerializeObject(objResult);
                        return Json(JSON, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        DataTable dtvalida = new DataTable();
                        dtvalida.Columns.Add("dniruc", typeof(string));
                        dtvalida.Columns.Add("pedido", typeof(string));
                        dtvalida.Columns.Add("op", typeof(string));
                        dtvalida.Columns.Add("fecha", typeof(DateTime));
                        dtvalida.Columns.Add("monto", typeof(Decimal));

                        var Fila = 0;
                        foreach (var item in _ListarValidarPagos)
                        {
                            dtvalida.Rows.Add();
                            dtvalida.Rows[Fila]["dniruc"] = item.NumDocuemnto;
                            dtvalida.Rows[Fila]["pedido"] = item.NumPedido;
                            dtvalida.Rows[Fila]["op"] = item.NumPedido;
                            dtvalida.Rows[Fila]["fecha"] = Convert.ToDateTime(item.FecOperacion);
                            dtvalida.Rows[Fila]["monto"] = Convert.ToDecimal(item.MonOperacion);
                            Fila++;
                        }
                        _Ent.NumBanco = NumBanco;
                        _Ent.Usu_Validar = _usuario.usu_id;

                        Result = datDocumento_Transaccion.SaveValidaInter(dtvalida, _Ent);
                        if (Result)
                        {
                            objResult.Data = NumBanco;
                            objResult.Success = true;
                            objResult.Message = "Se guardo correctamente. El archivo del banco Interbank" + " Última actualización: " + DateTime.Now;
                        }
                        else
                        {
                            objResult.Data = NumBanco;
                            objResult.Success = false;
                            objResult.Message = "Lamentablemente no se ha guardado el archivo";
                        }
                        JSON = JsonConvert.SerializeObject(objResult);
                        return Json(JSON, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    objResult.Data = NumBanco;
                    objResult.Success = false;
                    objResult.Message = "Lamentablemente no se ha guardado el archivo";
                    JSON = JsonConvert.SerializeObject(objResult);
                    return Json(JSON, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                objResult.Data = NumBanco;
                objResult.Success = false;
                objResult.Message = "Se produjo un error al registrar el archivo.";
                JSON = JsonConvert.SerializeObject(objResult);
                return Json(JSON, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region <CONSULTA DE OPERACIONES GRATUITAS>
        /// <summary>
        /// Consulta de Operaciones Gratuitas
        /// </summary>
        /// <create>Juilliand R. Damian Gomez </create>
        /// <update></update>
        /// <returns></returns>
        public ActionResult OpeGratuitas()
        {
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
                Ent_Operacion_Gratuita EntOpeGratuita = new Ent_Operacion_Gratuita();
                ViewBag.EntOpeGratuita = EntOpeGratuita;
                ViewBag.Listar_ConceptoOG = datFinanciera.Listar_ConceptoOG();
                return View("OperacionesGratuitas");
            }
        }
        /// <summary>
        /// lista de Operaciones Gratuitas
        /// </summary>
        /// <create>Juilliand R. Damian Gomez </create>
        /// <param name="param"></param>
        /// <param name="isOkUpdate"></param>
        /// <param name="FechaInicio"></param>
        /// <param name="FechaFin"></param>
        /// <param name="Tipo"></param>
        /// <returns></returns>
        public JsonResult getListOpeGratuitastAjax(Ent_jQueryDataTableParams param, bool isOkUpdate, string FechaInicio, string FechaFin, string Tipo)
        {
            Ent_Operacion_Gratuita EntOpeGratuita = new Ent_Operacion_Gratuita();

            if (isOkUpdate)
            {
                EntOpeGratuita.FechaInicio = DateTime.Parse(FechaInicio);
                EntOpeGratuita.FechaFin = DateTime.Parse(FechaFin);
                EntOpeGratuita.Tipo = Tipo;
                Session[_session_ListarOpeGratuitas] = datFinanciera.Listar_Liquidacion_Gratuita(EntOpeGratuita).ToList();
            }

            /*verificar si esta null*/
            if (Session[_session_ListarOpeGratuitas] == null)
            {
                List<Ent_Operacion_Gratuita> _ListarOpeGratuitas = new List<Ent_Operacion_Gratuita>();
                Session[_session_ListarOpeGratuitas] = _ListarOpeGratuitas;
            }

            IQueryable<Ent_Operacion_Gratuita> entDocTrans = ((List<Ent_Operacion_Gratuita>)(Session[_session_ListarOpeGratuitas])).AsQueryable();
            //Manejador de filtros
            int totalCount = entDocTrans.Count();
            IEnumerable<Ent_Operacion_Gratuita> filteredMembers = entDocTrans;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = entDocTrans.Where(
                        m =>
                            m.Tipo.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Fecha.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.TipoDocumento.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.NroDocumento.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Doc_cliente.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Cliente.ToUpper().Contains(param.sSearch.ToUpper())
                );
            }
            var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);

            if (param.iSortingCols > 0)
            {
                if (Request["sSortDir_0"].ToString() == "asc")
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderBy(o => o.Tipo); break;
                        case 1: filteredMembers = filteredMembers.OrderBy(o => o.Fecha); break;
                        case 2: filteredMembers = filteredMembers.OrderBy(o => o.TipoDocumento); break;
                        case 3: filteredMembers = filteredMembers.OrderBy(o => o.NroDocumento); break;
                        case 4: filteredMembers = filteredMembers.OrderBy(o => o.Doc_cliente); break;
                        case 5: filteredMembers = filteredMembers.OrderBy(o => o.Cliente); break;
                        case 6: filteredMembers = filteredMembers.OrderBy(o => o.SubTotal); break;
                        case 7: filteredMembers = filteredMembers.OrderBy(o => o.IGV); break;
                        case 8: filteredMembers = filteredMembers.OrderBy(o => o.Total); break;
                    }
                }
                else
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderByDescending(o => o.Tipo); break;
                        case 1: filteredMembers = filteredMembers.OrderByDescending(o => o.Fecha); break;
                        case 2: filteredMembers = filteredMembers.OrderByDescending(o => o.TipoDocumento); break;
                        case 3: filteredMembers = filteredMembers.OrderByDescending(o => o.NroDocumento); break;
                        case 4: filteredMembers = filteredMembers.OrderByDescending(o => o.Doc_cliente); break;
                        case 5: filteredMembers = filteredMembers.OrderByDescending(o => o.Cliente); break;
                        case 6: filteredMembers = filteredMembers.OrderByDescending(o => o.SubTotal); break;
                        case 7: filteredMembers = filteredMembers.OrderByDescending(o => o.IGV); break;
                        case 8: filteredMembers = filteredMembers.OrderByDescending(o => o.Total); break;
                    }
                }
            }

            var Result = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);

            //Se devuelven los resultados por json
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalCount,
                iTotalDisplayRecords = filteredMembers.Count(),
                aaData = Result
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Crea el archivo en excel
        /// </summary>
        /// <create>Juilliand R. Damian Gomez </create>
        /// <param name="_Ent"></param>
        /// <returns></returns>
        public ActionResult get_exporta_ListarOpeGratuitas_excel(Ent_Operacion_Gratuita _Ent)
        {
            JsonResponse objResult = new JsonResponse();
            try
            {
                Session[_session_ListarOpeGratuitas_Excel] = null;
                string cadena = "";
                if (Session[_session_ListarOpeGratuitas] != null)
                {

                    List<Ent_Operacion_Gratuita> _ListarOpeGratuitas = (List<Ent_Operacion_Gratuita>)Session[_session_ListarOpeGratuitas];
                    if (_ListarOpeGratuitas.Count == 0)
                    {
                        objResult.Success = false;
                        objResult.Message = "No hay filas para exportar";

                    }
                    else
                    {
                        cadena = get_html_ListarOpeGratuitas_str((List<Ent_Operacion_Gratuita>)Session[_session_ListarOpeGratuitas], _Ent);
                        if (cadena.Length == 0)
                        {
                            objResult.Success = false;
                            objResult.Message = "Error del formato html";
                        }
                        else
                        {
                            objResult.Success = true;
                            objResult.Message = "Se genero el excel correctamente";
                            Session[_session_ListarOpeGratuitas_Excel] = cadena;
                        }
                    }
                }
                else
                {
                    objResult.Success = false;
                    objResult.Message = "No hay filas para exportar";
                }

            }
            catch (Exception ex)
            {
                objResult.Success = false;
                objResult.Message = "No hay filas para exportar";
            }

            var JSON = JsonConvert.SerializeObject(objResult);

            return Json(JSON, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Armamos el archivo excel
        /// </summary>
        /// <create>Juilliand R. Damian Gomez </create>
        /// <param name="_ListarOpeGratuitas"></param>
        /// <param name="_Ent"></param>
        /// <returns></returns>
        public string get_html_ListarOpeGratuitas_str(List<Ent_Operacion_Gratuita> _ListarOpeGratuitas, Ent_Operacion_Gratuita _Ent)
        {
            StringBuilder sb = new StringBuilder();
            var Lista = _ListarOpeGratuitas.ToList();
            //var ClienteTitulo = _ListarOpeGratuitas.Select(x => new { Cliente = x.Cliente, NroDNI = x.NroDNI }).Distinct();

            try
            {
                sb.Append("<div><table cellspacing='0' style='width: 1000px' rules='all' border='0' style='border-collapse:collapse;'><tr><td Colspan='9'></td></tr><tr><td Colspan='9' valign='middle' align='center' style='vertical-align: middle;font-size: 16.0pt;font-weight: bold;color:#285A8F'>CONSULTA DE OPERACIONES GRATUITAS " + (_Ent.Tipo=="-1"? "" : " ' " + _Ent.TipoNombre.ToUpper()+" ' ") + " </td></tr><tr><td Colspan='9' valign='middle' align='center' style='vertical-align: middle;font-size: 11.0pt;font-weight: bold;color:#000000'>Desde el " + String.Format("{0:dd/MM/yyyy}", _Ent.FechaInicio) + " hasta el " + String.Format("{0:dd/MM/yyyy}", _Ent.FechaFin) + "</td></tr></table>");
                sb.Append("<table  border='1' bgColor='#ffffff' borderColor='#FFFFFF' cellSpacing='2' cellPadding='2' style='font-size:10.0pt; font-family:Calibri; background:white;width: 1000px'><tr  bgColor='#5799bf'>\n");
                sb.Append("<tr bgColor='#1E77AB'>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Tipo</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Fecha</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Tipo Documento</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Nro Documento</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Doc. cliente</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Cliente</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Sub Total</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>IGV</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Total</font></th>\n");
                sb.Append("</tr>\n");

                foreach (var item in Lista)
                {
                    sb.Append("<tr>\n");
                    sb.Append("<td align='Center'>" + item.Tipo + "</td>\n");
                    sb.Append("<td align='Center'>" + item.Fecha + "</td>\n");
                    sb.Append("<td align=''>" + item.TipoDocumento + "</td>\n");
                    sb.Append("<td align='Center'>" + item.NroDocumento + "</td>\n");
                    sb.Append("<td align='Center'>" + item.Doc_cliente+ "</td>\n");
                    sb.Append("<td align=''>" + item.Cliente + "</td>\n");
                    sb.Append("<td align='Right'>" + "S/ " + string.Format("{0:F2}", item.SubTotal) + "</td>\n");
                    sb.Append("<td align='Right'>" + "S/ " + string.Format("{0:F2}", item.IGV) + "</td>\n");
                    sb.Append("<td align='Right'>" + "S/ " + string.Format("{0:F2}", item.Total) + "</td>\n");
                    sb.Append("</tr>\n");
                }
                sb.Append("</table></div>");
            }
            catch
            {

            }
            return sb.ToString();
        }

        /// <summary>
        /// Exportamos el excel
        /// </summary>
        /// <create>Juilliand R. Damian Gomez </create>
        /// <returns>xlx</returns>
        public ActionResult ListarOpeGratuitasExcel()
        {
            string NombreArchivo = "OPeracionesGratuitas";
            String style = style = @"<style> .textmode { mso-number-format:\@; } </script> ";
            try
            {
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", "attachment;filename=" + NombreArchivo + ".xls");
                Response.Charset = "UTF-8";
                Response.ContentEncoding = Encoding.Default;
                Response.Write(style);
                Response.Write(Session[_session_ListarOpeGratuitas_Excel].ToString());
                Response.End();
            }
            catch
            {

            }
            return Json(new { estado = 0, mensaje = 1 });
        }
        #endregion

        #region <CONSULTA DE SALDO DE CLIENTES>
        /// <summary>
        /// Consulte entre un rango de fechas, todas los saldos realizAQUARELLAs por los clientes.
        /// </summary>
        /// <create>Juilliand R. Damián Gómez<create>
        /// <update></update>
        /// <returns></returns>
        public ActionResult Saldo_Cliente()
        {
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
                Ent_Saldo_Cliente EnSaldCliente = new Ent_Saldo_Cliente();
                ViewBag.Listar_ConceptoSC = datFinanciera.Listar_Concepto_Saldo().Where(x => x.Codigo == "90" || x.Codigo == "98" || x.Codigo == "9F" || x.Codigo == "9INT");
                ViewBag.ListarCLiente = datFinanciera.Leer_Clientes_Saldo();
                ViewBag.EnSaldCliente = EnSaldCliente;
                return View();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <create>Juilliand R. Damian Gomez </create>
        /// <update></update> 
        /// <param name="param"></param>
        /// <param name="isOkUpdate"></param>
        /// <param name="FechaInicio"></param>
        /// <param name="FechaFin"></param>
        /// <param name="Cod_Id"></param>
        /// <param name="Bas_Id"></param>
        /// <returns></returns>
        public JsonResult getLisSaldoClientesAjax(Ent_jQueryDataTableParams param, bool isOkUpdate, string FechaInicio, string FechaFin, string Cod_Id, int Bas_Id)
        {
            Ent_Saldo_Cliente EntSaldoCliente = new Ent_Saldo_Cliente();

            if (isOkUpdate)
            {
                EntSaldoCliente.Bas_Id = Bas_Id;
                EntSaldoCliente.Cod_Id = Cod_Id;
                EntSaldoCliente.FechaInicio = DateTime.Parse(FechaInicio);
                EntSaldoCliente.FechaFin = DateTime.Parse(FechaFin);
                EntSaldoCliente.Usu_Tipo = string.Empty;
                Session[_session_ListarSaldoCliente] = datFinanciera.Leer_Saldos_Pendientes(EntSaldoCliente).ToList();
            }

            /*verificar si esta null*/
            if (Session[_session_ListarSaldoCliente] == null)
            {
                List<Ent_Saldo_Cliente> _ListarSaldoCliente = new List<Ent_Saldo_Cliente>();
                Session[_session_ListarSaldoCliente] = _ListarSaldoCliente;
            }

            IQueryable<Ent_Saldo_Cliente> entDocTrans = ((List<Ent_Saldo_Cliente>)(Session[_session_ListarSaldoCliente])).AsQueryable();
            //Manejador de filtros
            int totalCount = entDocTrans.Count();
            IEnumerable<Ent_Saldo_Cliente> filteredMembers = entDocTrans;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = entDocTrans.Where(
                        m =>
                            m.Asesor.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Dniruc.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Lider.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Cliente.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Concepto.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Documento.ToUpper().Contains(param.sSearch.ToUpper())
                );
            }
            var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);

            if (param.iSortingCols > 0)
            {
                if (Request["sSortDir_0"].ToString() == "asc")
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderBy(o => o.Asesor); break;
                        case 1: filteredMembers = filteredMembers.OrderBy(o => o.Dniruc); break;
                        case 2: filteredMembers = filteredMembers.OrderBy(o => o.Lider); break;
                        case 3: filteredMembers = filteredMembers.OrderBy(o => o.Cliente); break;
                        case 4: filteredMembers = filteredMembers.OrderBy(o => o.Concepto); break;
                        case 5: filteredMembers = filteredMembers.OrderBy(o => o.Documento); break;
                        case 6: filteredMembers = filteredMembers.OrderBy(o => o.Monto); break;
                    }
                }
                else
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderByDescending(o => o.Asesor); break;
                        case 1: filteredMembers = filteredMembers.OrderByDescending(o => o.Dniruc); break;
                        case 2: filteredMembers = filteredMembers.OrderByDescending(o => o.Lider); break;
                        case 3: filteredMembers = filteredMembers.OrderByDescending(o => o.Cliente); break;
                        case 4: filteredMembers = filteredMembers.OrderByDescending(o => o.Concepto); break;
                        case 5: filteredMembers = filteredMembers.OrderByDescending(o => o.Documento); break;
                        case 6: filteredMembers = filteredMembers.OrderByDescending(o => o.Monto); break;
                    }
                }
            }

            var Result = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);

            //Se devuelven los resultados por json
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalCount,
                iTotalDisplayRecords = filteredMembers.Count(),
                aaData = Result
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Crea el archivo en excel
        /// </summary>
        /// <create>Juilliand R. Damian Gomez </create>
        /// <update></update>       
        /// <param name="_Ent"></param>
        /// <returns></returns>
        public ActionResult get_exporta_ListarSaldoCliente_excel(Ent_Saldo_Cliente _Ent)
        {
            JsonResponse objResult = new JsonResponse();
            try
            {
                Session[_session_ListarSaldoCliente_Excel] = null;
                string cadena = "";
                if (Session[_session_ListarSaldoCliente] != null)
                {

                    List<Ent_Saldo_Cliente> _ListarSaldoCliente = (List<Ent_Saldo_Cliente>)Session[_session_ListarSaldoCliente];
                    if (_ListarSaldoCliente.Count == 0)
                    {
                        objResult.Success = false;
                        objResult.Message = "No hay filas para exportar";

                    }
                    else
                    {
                        cadena = get_html_ListarSaldoCliente_str((List<Ent_Saldo_Cliente>)Session[_session_ListarSaldoCliente], _Ent);
                        if (cadena.Length == 0)
                        {
                            objResult.Success = false;
                            objResult.Message = "Error del formato html";
                        }
                        else
                        {
                            objResult.Success = true;
                            objResult.Message = "Se genero el excel correctamente";
                            Session[_session_ListarSaldoCliente_Excel] = cadena;
                        }
                    }
                }
                else
                {
                    objResult.Success = false;
                    objResult.Message = "No hay filas para exportar";
                }

            }
            catch (Exception ex)
            {
                objResult.Success = false;
                objResult.Message = "No hay filas para exportar";
            }

            var JSON = JsonConvert.SerializeObject(objResult);

            return Json(JSON, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Armamos el archivo excel
        /// </summary>
        /// <create>Juilliand R. Damian Gomez </create>
        /// <update></update>
        /// <param name="_ListarOpeGratuitas"></param>
        /// <param name="_Ent"></param>
        /// <returns></returns>
        public string get_html_ListarSaldoCliente_str(List<Ent_Saldo_Cliente> _ListarSaldoCliente, Ent_Saldo_Cliente _Ent)
        {
            StringBuilder sb = new StringBuilder();
            var Lista = _ListarSaldoCliente.ToList();
            try
            {
                sb.Append("<div><table cellspacing='0' style='width: 1000px' rules='all' border='0' style='border-collapse:collapse;'><tr><td Colspan='9'></td></tr><tr><td Colspan='9' valign='middle' align='center' style='vertical-align: middle;font-size: 16.0pt;font-weight: bold;color:#285A8F'>REPORTE DE SALDO DE CLIENTES  </td></tr><tr><td Colspan='9' valign='middle' align='center' style='vertical-align: middle;font-size: 10.0pt;font-weight: bold;color:#000000'>Desde el " + String.Format("{0:dd/MM/yyyy}", _Ent.FechaInicio) + " hasta el " + String.Format("{0:dd/MM/yyyy}", _Ent.FechaFin) + "</td></tr></table>");
                sb.Append("<table  border='1' bgColor='#ffffff' borderColor='#FFFFFF' cellSpacing='2' cellPadding='2' style='font-size:10.0pt; font-family:Calibri; background:white;width: 1000px'><tr  bgColor='#5799bf'>\n");
                sb.Append("<tr bgColor='#1E77AB'>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Asesor</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Lider</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Dni/Ruc (Promotor)</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Promotor</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Concepto</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Documento</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Fec. Transacción</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Fec. Documento</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Monto</font></th>\n");
                sb.Append("</tr>\n");
                NumberFormatInfo customNumFormat;
                customNumFormat = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
                customNumFormat.NumberGroupSeparator = ".";
                customNumFormat.NumberDecimalSeparator = ",";
                foreach (var item in Lista)
                {
                    sb.Append("<tr>\n");
                    sb.Append("<td align=''>" + item.Asesor + "</td>\n");
                    sb.Append("<td align=''>" + item.Lider + "</td>\n");
                    sb.Append("<td align='Center'>" + item.Dniruc + "</td>\n");
                    sb.Append("<td align=''>" + item.Cliente + "</td>\n");
                    sb.Append("<td align=''>" + item.Concepto + "</td>\n");
                    sb.Append("<td align='Center'>" + item.Documento + "</td>\n");
                    sb.Append("<td align='Center'>" + String.Format("{0:dd/MM/yyyy}", item.Fecha_Transac) + "</td>\n");
                    sb.Append("<td align='Center'>" + String.Format("{0:dd/MM/yyyy}", item.Fecha_Doc) + "</td>\n");
                    sb.Append("<td class='textDecim' align='Right'>" + string.Format(customNumFormat, "{0:F2}", item.Monto) + "</td>\n");
                    sb.Append("</tr>\n");
                }
                sb.Append("</table></div>");
            }
            catch
            {

            }
            return sb.ToString();
        }

        /// <summary>
        /// Exportamos el excel
        /// </summary>
        /// <create>Juilliand R. Damian Gomez </create>
        /// <update></update>
        /// <returns>xlx</returns>
        public ActionResult ListarSaldoClienteExcel()
        {
            string NombreArchivo = "ComisionLider";
            String style = style = @"<style> .textmode { mso-number-format:\@; } .textDecim { mso-number-format:0.00} </style> ";
            try
            {
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", "attachment;filename=" + NombreArchivo + ".xls");
                Response.Charset = "UTF-8";
                Response.ContentEncoding = Encoding.Default;
                Response.Write(style);
                Response.Write(Session[_session_ListarSaldoCliente_Excel].ToString());
                Response.End();
            }
            catch
            {

            }
            return Json(new { estado = 0, mensaje = 1 });
        }
        #endregion
        #region <MOVIMIENTO DE VENTAS-PAGOS>
        /// <summary>
        /// 
        /// </summary>
        /// <create>Juilliand R. Damian Gomez </create>
        /// <update></update> 
        /// <returns></returns>
        public ActionResult Mov_Venta_Pagos()
        {
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
                Ent_Movimientos_Pagos EntMovimientosPagos = new Ent_Movimientos_Pagos();
                ViewBag.EntMovimientosPagos = EntMovimientosPagos;
                return View();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <create>Juilliand R. Damian Gomez </create>
        /// <update></update> 
        /// <param name="param"></param>
        /// <param name="isOkUpdate"></param>
        /// <param name="FechaInicio"></param>
        /// <param name="FechaFin"></param>
        /// <returns></returns>
        public JsonResult getLisMovimientosPagosAjax(Ent_jQueryDataTableParams param, bool isOkUpdate, string FechaInicio, string FechaFin)
        {
            Ent_Movimientos_Pagos EntMovimientosPagos = new Ent_Movimientos_Pagos();

            if (isOkUpdate)
            {
                EntMovimientosPagos.FechaInicio = DateTime.Parse(FechaInicio);
                EntMovimientosPagos.FechaFin = DateTime.Parse(FechaFin);                
                Session[_session_ListarMovimientosPagos] = datFinanciera.Listar_Movimientos_Pagos(EntMovimientosPagos).ToList();
            }

            /*verificar si esta null*/
            if (Session[_session_ListarMovimientosPagos] == null)
            {
                List<Ent_Movimientos_Pagos> _ListarMovimientosPagos = new List<Ent_Movimientos_Pagos>();
                Session[_session_ListarMovimientosPagos] = _ListarMovimientosPagos;
            }

            IQueryable<Ent_Movimientos_Pagos> entDocTrans = ((List<Ent_Movimientos_Pagos>)(Session[_session_ListarMovimientosPagos])).AsQueryable();
            //Manejador de filtros
            int totalCount = entDocTrans.Count();
            IEnumerable<Ent_Movimientos_Pagos> filteredMembers = entDocTrans;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = entDocTrans.Where(
                        m =>
                        m.Fecha_Op.ToUpper().Contains(param.sSearch.ToUpper()) ||
                        m.Des_Operacion.ToUpper().Contains(param.sSearch.ToUpper()) ||
                        m.Op_Monto.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                        m.Op_Numero.ToUpper().Contains(param.sSearch.ToUpper()) ||
                        m.Fecha_Op2.ToUpper().Contains(param.sSearch.ToUpper()) ||
                        m.Dni_Ruc.ToUpper().Contains(param.sSearch.ToUpper()) ||
                        m.Cliente.ToUpper().Contains(param.sSearch.ToUpper())
                );
            }
            var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);

            if (param.iSortingCols > 0)
            {
                if (Request["sSortDir_0"].ToString() == "asc")
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderBy(o => o.Fecha_Op); break;
                        case 1: filteredMembers = filteredMembers.OrderBy(o => o.Des_Operacion); break;
                        case 2: filteredMembers = filteredMembers.OrderBy(o => o.Op_Monto); break;
                        case 3: filteredMembers = filteredMembers.OrderBy(o => o.Op_Numero); break;
                        case 4: filteredMembers = filteredMembers.OrderBy(o => o.Fecha_Op2); break;
                        case 5: filteredMembers = filteredMembers.OrderBy(o => o.Dni_Ruc); break;
                        case 6: filteredMembers = filteredMembers.OrderBy(o => o.Cliente); break;
                        case 7: filteredMembers = filteredMembers.OrderBy(o => o.Fecha_Doc); break;
                        case 8: filteredMembers = filteredMembers.OrderBy(o => o.Num_Doc); break;
                        case 9: filteredMembers = filteredMembers.OrderBy(o => o.Importe_Doc); break;
                        case 10: filteredMembers = filteredMembers.OrderBy(o => o.Fecha_Ncredito); break;
                        case 11: filteredMembers = filteredMembers.OrderBy(o => o.Num_Ncredito); break;
                        case 12: filteredMembers = filteredMembers.OrderBy(o => o.Importe_Ncredito); break;
                        case 13: filteredMembers = filteredMembers.OrderBy(o => o.Fecha_Ncredito2); break;
                        case 14: filteredMembers = filteredMembers.OrderBy(o => o.Num_Ncredito2); break;
                        case 15: filteredMembers = filteredMembers.OrderBy(o => o.Importe_Ncredito2); break;
                        case 16: filteredMembers = filteredMembers.OrderBy(o => o.Fecha_Ncredito3); break;
                        case 17: filteredMembers = filteredMembers.OrderBy(o => o.Num_Ncredito3); break;
                        case 18: filteredMembers = filteredMembers.OrderBy(o => o.Importe_Ncredito3); break;
                        case 19: filteredMembers = filteredMembers.OrderBy(o => o.Fecha_Ncredito4); break;
                        case 20: filteredMembers = filteredMembers.OrderBy(o => o.Num_Ncredito4); break;
                        case 21: filteredMembers = filteredMembers.OrderBy(o => o.Importe_Ncredito4); break;
                        case 22: filteredMembers = filteredMembers.OrderBy(o => o.Fecha_Ncredito5); break;
                        case 23: filteredMembers = filteredMembers.OrderBy(o => o.Num_Ncredito5); break;
                        case 24: filteredMembers = filteredMembers.OrderBy(o => o.Importe_Ncredito5); break;
                        case 25: filteredMembers = filteredMembers.OrderBy(o => o.Fecha_Ncredito6); break;
                        case 26: filteredMembers = filteredMembers.OrderBy(o => o.Num_Ncredito6); break;
                        case 27: filteredMembers = filteredMembers.OrderBy(o => o.Importe_Ncredito6); break;
                        case 28: filteredMembers = filteredMembers.OrderBy(o => o.Fecha_Ncredito7); break;
                        case 29: filteredMembers = filteredMembers.OrderBy(o => o.Num_Ncredito7); break;
                        case 30: filteredMembers = filteredMembers.OrderBy(o => o.Importe_Ncredito7); break;
                        case 31: filteredMembers = filteredMembers.OrderBy(o => o.Base_Imponible); break;
                        case 32: filteredMembers = filteredMembers.OrderBy(o => o.Percepcion); break;
                        case 33: filteredMembers = filteredMembers.OrderBy(o => o.Total); break;
                        case 34: filteredMembers = filteredMembers.OrderBy(o => o.Fecha_Saldo_Ant); break;
                        case 35: filteredMembers = filteredMembers.OrderBy(o => o.Importe_Saldo_Ant); break;
                        case 36: filteredMembers = filteredMembers.OrderBy(o => o.Pagar); break;
                        case 37: filteredMembers = filteredMembers.OrderBy(o => o.Deposito); break;
                        case 38: filteredMembers = filteredMembers.OrderBy(o => o.Saldo_Favor); break;
                        case 39: filteredMembers = filteredMembers.OrderBy(o => o.Ajuste); break;
                    }
                }
                else
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderByDescending(o => o.Fecha_Op); break;
                        case 1: filteredMembers = filteredMembers.OrderByDescending(o => o.Des_Operacion); break;
                        case 2: filteredMembers = filteredMembers.OrderByDescending(o => o.Op_Monto); break;
                        case 3: filteredMembers = filteredMembers.OrderByDescending(o => o.Op_Numero); break;
                        case 4: filteredMembers = filteredMembers.OrderByDescending(o => o.Fecha_Op2); break;
                        case 5: filteredMembers = filteredMembers.OrderByDescending(o => o.Dni_Ruc); break;
                        case 6: filteredMembers = filteredMembers.OrderByDescending(o => o.Cliente); break;
                        case 7: filteredMembers = filteredMembers.OrderByDescending(o => o.Fecha_Doc); break;
                        case 8: filteredMembers = filteredMembers.OrderByDescending(o => o.Num_Doc); break;
                        case 9: filteredMembers = filteredMembers.OrderByDescending(o => o.Importe_Doc); break;
                        case 10: filteredMembers = filteredMembers.OrderByDescending(o => o.Fecha_Ncredito); break;
                        case 11: filteredMembers = filteredMembers.OrderByDescending(o => o.Num_Ncredito); break;
                        case 12: filteredMembers = filteredMembers.OrderByDescending(o => o.Importe_Ncredito); break;
                        case 13: filteredMembers = filteredMembers.OrderByDescending(o => o.Fecha_Ncredito2); break;
                        case 14: filteredMembers = filteredMembers.OrderByDescending(o => o.Num_Ncredito2); break;
                        case 15: filteredMembers = filteredMembers.OrderByDescending(o => o.Importe_Ncredito2); break;
                        case 16: filteredMembers = filteredMembers.OrderByDescending(o => o.Fecha_Ncredito3); break;
                        case 17: filteredMembers = filteredMembers.OrderByDescending(o => o.Num_Ncredito3); break;
                        case 18: filteredMembers = filteredMembers.OrderByDescending(o => o.Importe_Ncredito3); break;
                        case 19: filteredMembers = filteredMembers.OrderByDescending(o => o.Fecha_Ncredito4); break;
                        case 20: filteredMembers = filteredMembers.OrderByDescending(o => o.Num_Ncredito4); break;
                        case 21: filteredMembers = filteredMembers.OrderByDescending(o => o.Importe_Ncredito4); break;
                        case 22: filteredMembers = filteredMembers.OrderByDescending(o => o.Fecha_Ncredito5); break;
                        case 23: filteredMembers = filteredMembers.OrderByDescending(o => o.Num_Ncredito5); break;
                        case 24: filteredMembers = filteredMembers.OrderByDescending(o => o.Importe_Ncredito5); break;
                        case 25: filteredMembers = filteredMembers.OrderByDescending(o => o.Fecha_Ncredito6); break;
                        case 26: filteredMembers = filteredMembers.OrderByDescending(o => o.Num_Ncredito6); break;
                        case 27: filteredMembers = filteredMembers.OrderByDescending(o => o.Importe_Ncredito6); break;
                        case 28: filteredMembers = filteredMembers.OrderByDescending(o => o.Fecha_Ncredito7); break;
                        case 29: filteredMembers = filteredMembers.OrderByDescending(o => o.Num_Ncredito7); break;
                        case 30: filteredMembers = filteredMembers.OrderByDescending(o => o.Importe_Ncredito7); break;
                        case 31: filteredMembers = filteredMembers.OrderByDescending(o => o.Base_Imponible); break;
                        case 32: filteredMembers = filteredMembers.OrderByDescending(o => o.Percepcion); break;
                        case 33: filteredMembers = filteredMembers.OrderByDescending(o => o.Total); break;
                        case 34: filteredMembers = filteredMembers.OrderByDescending(o => o.Fecha_Saldo_Ant); break;
                        case 35: filteredMembers = filteredMembers.OrderByDescending(o => o.Importe_Saldo_Ant); break;
                        case 36: filteredMembers = filteredMembers.OrderByDescending(o => o.Pagar); break;
                        case 37: filteredMembers = filteredMembers.OrderByDescending(o => o.Deposito); break;
                        case 38: filteredMembers = filteredMembers.OrderByDescending(o => o.Saldo_Favor); break;
                        case 39: filteredMembers = filteredMembers.OrderByDescending(o => o.Ajuste); break;
                    }
                }
            }

            var Result = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);

            //Se devuelven los resultados por json
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalCount,
                iTotalDisplayRecords = filteredMembers.Count(),
                aaData = Result
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Crea el archivo en excel
        /// </summary>
        /// <create>Juilliand R. Damian Gomez </create>
        /// <update></update>       
        /// <param name="_Ent"></param>
        /// <returns></returns>
        public ActionResult get_exporta_ListarMovimientosPagos_excel(Ent_Movimientos_Pagos _Ent)
        {
            JsonResponse objResult = new JsonResponse();
            try
            {
                Session[_session_ListarMovimientosPagos_Excel] = null;
                string cadena = "";
                if (Session[_session_ListarMovimientosPagos] != null)
                {

                    List<Ent_Movimientos_Pagos> _ListarMovimientosPagos = (List<Ent_Movimientos_Pagos>)Session[_session_ListarMovimientosPagos];
                    if (_ListarMovimientosPagos.Count == 0)
                    {
                        objResult.Success = false;
                        objResult.Message = "No hay filas para exportar";

                    }
                    else
                    {
                        cadena = get_html_ListarMovimientosPagos_str((List<Ent_Movimientos_Pagos>)Session[_session_ListarMovimientosPagos], _Ent);
                        if (cadena.Length == 0)
                        {
                            objResult.Success = false;
                            objResult.Message = "Error del formato html";
                        }
                        else
                        {
                            objResult.Success = true;
                            objResult.Message = "Se genero el excel correctamente";
                            Session[_session_ListarMovimientosPagos_Excel] = cadena;
                        }
                    }
                }
                else
                {
                    objResult.Success = false;
                    objResult.Message = "No hay filas para exportar";
                }

            }
            catch (Exception ex)
            {
                objResult.Success = false;
                objResult.Message = "No hay filas para exportar";
            }

            var JSON = JsonConvert.SerializeObject(objResult);

            return Json(JSON, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Armamos el archivo excel
        /// </summary>
        /// <create>Juilliand R. Damian Gomez </create>
        /// <update></update>
        /// <param name="_ListarOpeGratuitas"></param>
        /// <param name="_Ent"></param>
        /// <returns></returns>
        public string get_html_ListarMovimientosPagos_str(List<Ent_Movimientos_Pagos> _ListarMovimientosPagos, Ent_Movimientos_Pagos _Ent)
        {
            StringBuilder sb = new StringBuilder();
            var Lista = _ListarMovimientosPagos.ToList();
            try
            {
                sb.Append("<div><table cellspacing='0' style='width: 1000px' rules='all' border='0' style='border-collapse:collapse;'><tr><td Colspan='9'></td></tr><tr><td Colspan='9' valign='middle' align='center' style='vertical-align: middle;font-size: 16.0pt;font-weight: bold;color:#285A8F'>REPORTE DE MOVIMIENTOS DE VENTAS Y PAGOS</td></tr><tr><td Colspan='9' valign='middle' align='center' style='vertical-align: middle;font-size: 10.0pt;font-weight: bold;color:#000000'>Desde el " + String.Format("{0:dd/MM/yyyy}", _Ent.FechaInicio) + " hasta el " + String.Format("{0:dd/MM/yyyy}", _Ent.FechaFin) + "</td></tr></table>");
                sb.Append("<table  border='1' bgColor='#ffffff' borderColor='#FFFFFF' cellSpacing='2' cellPadding='2' style='font-size:10.0pt; font-family:Calibri; background:white;width: 1000px'><tr  bgColor='#5799bf'>\n");
                sb.Append("<tr bgColor='#5799bf'>\n");
                sb.Append("<th colspan='7'></th>\n");
                sb.Append("<th colspan='3' style='text-align: center;'><font color='#FFFFFF'>Ticket</font></th>\n");
                sb.Append("<th colspan='3' style='text-align: center;'><font color='#FFFFFF'>Nota de Credito 1</font></th>\n");
                sb.Append("<th colspan='3' style='text-align: center;'><font color='#FFFFFF'>Nota de Credito 2</font></th>\n");
                sb.Append("<th colspan='3' style='text-align: center;'><font color='#FFFFFF'>Nota de Credito 3</font></th>\n");
                sb.Append("<th colspan='3' style='text-align: center;'><font color='#FFFFFF'>Nota de Credito 4</font></th>\n");
                sb.Append("<th colspan='3' style='text-align: center;'><font color='#FFFFFF'>Nota de Credito 5</font></th>\n");
                sb.Append("<th colspan='3' style='text-align: center;'><font color='#FFFFFF'>Nota de Credito 6</font></th>\n");
                sb.Append("<th colspan='3' style='text-align: center;'><font color='#FFFFFF'>Nota de Credito 7</font></th>\n");
                sb.Append("<th colspan='3'></th>\n");
                sb.Append("<th colspan='2' style='text-align: center;'><font color='#FFFFFF'>Saldo Anterior</font></th>\n");
                sb.Append("<th colspan='4'></th>\n");
                sb.Append("</tr>\n");
                sb.Append("<tr bgColor='#5799bf'>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Fecha</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Des. Operación</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Monto</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Num. Operación</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Fecha</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Dni/Ruc</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Nombre/Razon Social</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Fecha</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Num. Documento</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Importe</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Fecha</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Num. Documento</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Importe</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Fecha</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Num. Documento</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Importe</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Fecha</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Num. Documento</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Importe</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Fecha</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Num. Documento</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Importe</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Fecha</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Num. Documento</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Importe</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Fecha</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Num. Documento</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Importe</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Fecha</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Num. Documento</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Importe</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Base Imponible</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Percepción 2%</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Tot. Ticket</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Fecha</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Importe</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>A Pagar</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Deposito</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Saldo Favor</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color='#FFFFFF'>Ajuste</font></th>\n");
                sb.Append("</tr>\n");

                foreach (var item in Lista)
                {
                    sb.Append("<tr>\n");
                    sb.Append("<td align='Center'>" + item.Fecha_Op + "</td>\n");
                    sb.Append("<td align=''>" + item.Des_Operacion + "</td>\n");
                    sb.Append("<td align='right'>" + "S/ " + string.Format("{0:F2}", item.Op_Monto) + "</td>\n");
                    sb.Append("<td align='Center'>" + item.Op_Numero + "</td>\n");
                    sb.Append("<td align='Center'>" + item.Fecha_Op2 + "</td>\n");
                    sb.Append("<td align='Center'>" + item.Dni_Ruc + "</td>\n");
                    sb.Append("<td align=''>" + item.Cliente + "</td>\n");
                    sb.Append("<td align='Center'>" + item.Fecha_Doc + "</td>\n");
                    sb.Append("<td align=''>" + item.Num_Doc + "</td>\n");
                    sb.Append("<td align='right'>" + (item.Importe_Doc == null ? " " :  "S/ " + string.Format("{0:F2}", item.Importe_Doc)) + "</td>\n");
                    sb.Append("<td align='Center'>" + item.Fecha_Ncredito + "</td>\n");
                    sb.Append("<td align=''>" + item.Num_Ncredito + "</td>\n");
                    sb.Append("<td align='right'>" + (item.Importe_Ncredito == null ? " " : "S/ " + string.Format("{0:F2}", item.Importe_Ncredito)) + "</td>\n");
                    sb.Append("<td align='Center'>" + item.Fecha_Ncredito2 + "</td>\n");
                    sb.Append("<td align=''>" + item.Num_Ncredito2 + "</td>\n");
                    sb.Append("<td align='right'>" + (item.Importe_Ncredito2 == null ? " " : "S/ " + string.Format("{0:F2}", item.Importe_Ncredito2)) + "</td>\n");
                    sb.Append("<td align='Center'>" + item.Fecha_Ncredito3 + "</td>\n");
                    sb.Append("<td align=''>" + item.Num_Ncredito3 + "</td>\n");
                    sb.Append("<td align='right'>" + (item.Importe_Ncredito3 == null ? " " : "S/ " + string.Format("{0:F2}", item.Importe_Ncredito3)) + "</td>\n");
                    sb.Append("<td align='Center'>" + item.Fecha_Ncredito4 + "</td>\n");
                    sb.Append("<td align=''>" + item.Num_Ncredito4 + "</td>\n");
                    sb.Append("<td align='right'>" + (item.Importe_Ncredito4 == null ? " " : "S/ " + string.Format("{0:F2}", item.Importe_Ncredito4) )+ "</td>\n");
                    sb.Append("<td align='Center'>" + item.Fecha_Ncredito5 + "</td>\n");
                    sb.Append("<td align=''>" + item.Num_Ncredito5 + "</td>\n");
                    sb.Append("<td align='right'>" + (item.Importe_Ncredito5 == null ? " " : "S/ " + string.Format("{0:F2}", item.Importe_Ncredito5)) + "</td>\n");
                    sb.Append("<td align='Center'>" + item.Fecha_Ncredito6 + "</td>\n");
                    sb.Append("<td align=''>" + item.Num_Ncredito6 + "</td>\n");
                    sb.Append("<td align='right'>" + (item.Importe_Ncredito6 == null ? " " : "S/ " + string.Format("{0:F2}", item.Importe_Ncredito6)) + "</td>\n");
                    sb.Append("<td align='Center'>" + item.Fecha_Ncredito7 + "</td>\n");
                    sb.Append("<td align=''>" + item.Num_Ncredito7 + "</td>\n");
                    sb.Append("<td align='right'>" + (item.Importe_Ncredito7 == null ? " " :"S/ " + string.Format("{0:F2}", item.Importe_Ncredito7)) + "</td>\n");
                    sb.Append("<td align='right'>" + (item.Base_Imponible == null ? " " :"S/ " + string.Format("{0:F2}", item.Base_Imponible)) + "</td>\n");
                    sb.Append("<td align='right'>" + (item.Percepcion == null ? " " :"S/ " + string.Format("{0:F2}", item.Percepcion)) + "</td>\n");
                    sb.Append("<td align='right'>" + (item.Total == null ? " " : "S/ " + string.Format("{0:F2}", item.Total)) + "</td>\n");
                    sb.Append("<td align='Center'>" + item.Fecha_Saldo_Ant + "</td>\n");
                    sb.Append("<td align='right'>" + (item.Importe_Saldo_Ant == null ? " " :"S/ " + string.Format("{0:F2}", item.Importe_Saldo_Ant)) + "</td>\n");
                    sb.Append("<td align='right'>" + (item.Pagar == null ? " " :"S/ " + string.Format("{0:F2}",item.Pagar)) + "</td>\n");
                    sb.Append("<td align='right'>" + (item.Deposito == null ? " " :"S/ " + string.Format("{0:F2}",item.Deposito)) + "</td>\n");
                    sb.Append("<td align='right'>" + (item.Saldo_Favor == null ? " " :"S/ " + string.Format("{0:F2}",item.Saldo_Favor))+ "</td>\n");
                    sb.Append("<td align='right'>" + (item.Ajuste == null ? " " : "S/ " + string.Format("{0:F2}", item.Ajuste)) + "</td>\n");
                    sb.Append("</tr>\n");
                }
                sb.Append("</table></div>");
            }
            catch
            {

            }
            return sb.ToString();
        }

        /// <summary>
        /// Exportamos el excel
        /// </summary>
        /// <create>Juilliand R. Damian Gomez </create>
        /// <update></update>
        /// <returns>xlx</returns>
        public ActionResult ListarMovimientosPagosExcel()
        {
            string NombreArchivo = "MovimientosPagos";
            String style = style = @"<style> .textmode { mso-number-format:\@; } </script> ";
            try
            {
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", "attachment;filename=" + NombreArchivo + ".xls");
                Response.Charset = "UTF-8";
                Response.ContentEncoding = Encoding.Default;
                Response.Write(style);
                Response.Write(Session[_session_ListarMovimientosPagos_Excel].ToString());
                Response.End();
            }
            catch
            {

            }
            return Json(new { estado = 0, mensaje = 1 });
        }



        #endregion
    }
}