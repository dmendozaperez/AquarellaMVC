using OfficeOpenXml;
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
            //Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
            Ent_Usuario _usuario = new Ent_Usuario();
            _usuario.usu_id = 1;
            _usuario.usu_postPago = "1";
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
                //valida_rol = valida_controller.AccesoMenu(menu, this);
                valida_rol = true;
                #endregion
                if (valida_rol)
                {
                    Session[_sessionPagsLiqs] = null;

                    Ent_Pedido_Maestro maestros = datPedido.Listar_Maestros_Pedido(_usuario.usu_id, _usuario.usu_postPago, "");
                    ViewBag.listPromotor = maestros.combo_ListPromotor;

                    return View();
                }
                else
                {
                    return RedirectToAction("Login", "Control", new { returnUrl = return_view });
                }
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

        public FileContentResult ListaCuentasExcel()
        {
            string excelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            var entDocTrans = (List<Ent_Lista_Cuenta_Contables>)Session[_session_listCuentasContables];

            var Lista = entDocTrans.GroupBy(x => x.Clear_id).Select(y => new
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

            //// var ListCount = Lista.Count();

            int row = 7;
            int rowCount = 0;
            int varMerge = 0;
            List<int> newId = new List<int>();
            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Cuentas");
            ////Titulo
            //Sheet.Cells["C2:F2"].Merge = true;
            //Sheet.Cells["C2"].Value = "LISTA DE ARTICULOS - CATALOGO - BATA";
            //Sheet.Cells["C2"].Style.Font.Size = 24;
            //Sheet.Cells["C2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            //Cabecera
            Sheet.Cells["B6"].Value = "Clear Id";
            Sheet.Cells["C6"].Value = "Cuenta Contable";
            Sheet.Cells["D6"].Value = "Descripción Cuenta";
            Sheet.Cells["E6"].Value = "Tipo de Entidad";
            Sheet.Cells["F6"].Value = "Codigo entidad";
            Sheet.Cells["G6"].Value = "Descripción Entidad";
            Sheet.Cells["H6"].Value = "Tipo";
            Sheet.Cells["I6"].Value = "Serie";
            Sheet.Cells["J6"].Value = "Número";
            Sheet.Cells["K6"].Value = "Fecha";
            Sheet.Cells["L6"].Value = "Debe";
            Sheet.Cells["M6"].Value = "Haber";

            //Formato de cabecera
            Sheet.Cells["B1:M6"].Style.Font.Bold = true;
            Sheet.Cells["B5:M6"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            Sheet.Cells["B5:M6"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DodgerBlue);
            Sheet.Cells["B5:M6"].Style.Font.Color.SetColor(System.Drawing.Color.White);
            Sheet.Cells["B5:M6"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            Sheet.Cells["B5:M6"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

            Sheet.Cells["H5:K5"].Merge = true;
            Sheet.Cells["H5"].Value = "Documentos";
            
            //Estilo al cuerpo del excel
            //Carga datos      
            var recuperar = 0;
            foreach (var itemP in Lista)
            {
                foreach (var itemH in itemP.Hijos)
                {

                    Sheet.Cells[string.Format("B{0}", row)].Value = itemH.Clear_id;
                    Sheet.Cells[string.Format("B{0}", row)].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    Sheet.Cells[string.Format("B{0}", row)].Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                    Sheet.Cells[string.Format("C{0}", row)].Value = itemH.Cuenta;
                    Sheet.Cells[string.Format("C{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    Sheet.Cells[string.Format("D{0}", row)].Value = itemH.CuentaDes;

                    Sheet.Cells[string.Format("E{0}", row)].Value = itemH.TipoEntidad;
                    Sheet.Cells[string.Format("E{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    Sheet.Cells[string.Format("F{0}", row)].Value = itemH.CodigoEntidad;
                    Sheet.Cells[string.Format("F{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    Sheet.Cells[string.Format("G{0}", row)].Value = itemH.DesEntidad;

                    Sheet.Cells[string.Format("H{0}", row)].Value = itemH.Tipo;
                    Sheet.Cells[string.Format("H{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    Sheet.Cells[string.Format("I{0}", row)].Value = itemH.Serie;
                    Sheet.Cells[string.Format("I{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    Sheet.Cells[string.Format("J{0}", row)].Value = itemH.Numero;
                    Sheet.Cells[string.Format("J{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    
                    Sheet.Cells[string.Format("K{0}", row)].Style.Numberformat.Format = "dd/mm/yyyy";
                    Sheet.Cells[string.Format("K{0}", row)].Value = (itemH.Fecha == null) ? (DateTime?)null : Convert.ToDateTime(itemH.Fecha);
                    Sheet.Cells[string.Format("K{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    Sheet.Cells[string.Format("L{0}", row)].Value = (itemH.Debe == null) ? (double?)null : Convert.ToDouble(string.Format("{0:F2}", itemH.Debe));
                    Sheet.Cells[string.Format("L{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                    Sheet.Cells[string.Format("M{0}", row)].Value = (itemH.Haber == null) ? (double?)null : Convert.ToDouble(string.Format("{0:F2}", itemH.Haber));
                    Sheet.Cells[string.Format("M{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    Sheet.Cells[string.Format("M{0}", row)].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    Sheet.Cells[string.Format("M{0}", row)].Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                    row++;
                    rowCount++;
                }
                newId.Add(row);
                varMerge = row - rowCount;
                Sheet.Cells[string.Format("B" + varMerge + ":B" + (row - 1))].Merge = true;
                Sheet.Cells[string.Format("B" + varMerge + ":B" + (row - 1))].Style.Font.Bold = true;
                Sheet.Cells[string.Format("B" + varMerge + ":B" + (row - 1))].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells[string.Format("B" + varMerge + ":B" + (row - 1))].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.CornflowerBlue);
                Sheet.Cells[string.Format("B" + varMerge + ":B" + (row - 1))].Style.Font.Color.SetColor(System.Drawing.Color.Black);
                Sheet.Cells[string.Format("B" + varMerge + ":B" + (row - 1))].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                Sheet.Cells[string.Format("B" + varMerge + ":B" + (row - 1))].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                varMerge = 0;
                rowCount = 0;
            }

            int[] arr_newId = newId.ToArray();

            foreach (var item in arr_newId)
            {
                Sheet.Cells["B" + (item - 1) + ":M" + (item - 1)].Style.Font.Bold = true;
                Sheet.Cells["B" + (item - 1) + ":M" + (item - 1)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["B" + (item - 1) + ":M" + (item - 1)].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.SkyBlue);
                Sheet.Cells["B" + (item - 1) + ":M" + (item - 1)].Style.Font.Color.SetColor(System.Drawing.Color.Black);

                Sheet.Cells["B" + (item - 1) + ":M" + (item - 1)].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                Sheet.Cells["B" + (item - 1) + ":M" + (item - 1)].Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
            }

            Sheet.Cells["A:AZ"].AutoFitColumns();
            var stream = new MemoryStream(Ep.GetAsByteArray());
            return File(stream.ToArray(), excelContentType, "CuentaContables.xlsx");
        }

        #endregion
    }    
}