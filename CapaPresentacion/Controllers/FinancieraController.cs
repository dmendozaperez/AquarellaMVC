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

        private string _sessionPagsLiqs = "_SessionPagsLiqs";
        private string _sessin_customer = "_sessin_customer";
        private string _session_listCuentasContables = "_session_listCuentasContables";
        private string _session_ListCuentasContables_Excel = "_session_listCuentasContables_Excel";
        
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

        #region Lista de cuenta contables

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
                Session[_sessionPagsLiqs] = null;

                Ent_Pedido_Maestro maestros = datPedido.Listar_Maestros_Pedido(_usuario.usu_id, _usuario.usu_postPago, "");
                ViewBag.listPromotor = maestros.combo_ListPromotor;

                return View();               
            }
        }

        [HttpGet]
        public JsonResult CuentaContable(string FechaInicio, string FechaFin, int IdCliente)
        {
            JsonResponse objResult = new JsonResponse();
            Ent_Lista_Cuenta_Contables ent = new Ent_Lista_Cuenta_Contables();
            DateTime time = new DateTime();

            ent.FechaInicio = DateTime.Parse(FechaInicio);
            ent.FechaFin = DateTime.Parse(FechaFin);
            ent.IdCliente = IdCliente;

            var entDocTrans = datDocumento_Transaccion.Listar_Asientos_Adonis(ent).ToList();

            Session[_session_listCuentasContables] = entDocTrans;
            objResult.Data = entDocTrans.GroupBy(x => x.Clear_id).Select(y => new
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
                    devito = m.devito,
                    Amount = m.Amount,
                    Concepto = m.Concepto,
                    Ad_Co = m.Ad_Co,
                    Pad_Pay_Date = m.Pad_Pay_Date,
                    Contador = m.Contador
                })
            }).ToList();

            if (entDocTrans.Count > 0)
            {
                objResult.Success = true;
            }

            var JSON = JsonConvert.SerializeObject(objResult);

            return Json(JSON, JsonRequestBehavior.AllowGet);
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

                sb.Append("<div><table cellspacing='0' rules='all' border='1' style='border-collapse:collapse;'><td Colspan='12' valign='middle' align='center' style='font-size: 18px;font-weight: bold;color:#285A8F'>CUENTAS CONTABLE - CATALOGO - BATA</td></table>");
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
    }
}