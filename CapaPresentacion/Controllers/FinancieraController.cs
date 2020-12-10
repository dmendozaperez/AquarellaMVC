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
using Newtonsoft.Json;

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
    }
}