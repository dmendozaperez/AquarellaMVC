﻿using CapaEntidad.General;
using CapaEntidad.Util;
using CapaEntidad.Control;
using CapaDato.Facturacion;
using CapaEntidad.Facturacion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Data;
using Newtonsoft.Json;
using System.Reflection;

namespace CapaPresentacion.Controllers
{
    public class FacturacionController : Controller
    {
        private Dat_Facturacion datFacturacion = new Dat_Facturacion();
        private string _session_ListarMovimientosVentas = "_session_ListarMovimientosVentas";
        private string _session_ListarMovimientosVentasChart = "_session_ListarMovimientosVentasChart";
        private string _session_ListarMovimientosVentas_Excel = "_session_ListarMovimientosVentas_Excel";
        private string _session_ListarComisiones = "_session_ListarComisiones";
        private string _session_ListarComisiones_Excel = "_session_ListarComisiones_Excel";
        private string _session_ListarVentasResumido = "_session_ListarVentasResumido";
        private string _session_ListarVentasResumido_Excel = "_session_ListarVentasResumido_Excel";
        private string _session_ListarLiderVentas = "_session_ListarLiderVentas";
        private string _session_ListarLiderVentas_Excel = "_session_ListarLiderVentas_Excel";
        private string _session_ListarVentasTallas = "_session_ListarVentasTallas";
        private string _session_ListarVentasTallas_Excel = "_session_ListarVentasTallas_Excel";

        #region <CONSULTA DE VENTAS POR CATEGORIA>
        public ActionResult Ventas_Categoria()
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
                Ent_Movimientos_Ventas EntMovimientosVentas = new Ent_Movimientos_Ventas();
                ViewBag.EntMovimientosVentas = EntMovimientosVentas;
                ViewBag.ListarTipoArticulo = datFacturacion.ListarTipoArticulo();
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
        /// <param name="isOkSemanal"></param>
        /// <param name="FechaInicio"></param>
        /// <param name="FechaFin"></param>
        /// <param name="TipoArticulo"></param>
        /// <returns></returns>
        public JsonResult getLisMovimientosVentaAjax(Ent_jQueryDataTableParams param, bool isOkUpdate, bool isOkSemanal,string FechaInicio, string FechaFin, string TipoArticulo)
        {
            Ent_Movimientos_Ventas EntMovimientosVentas = new Ent_Movimientos_Ventas();
            Decimal? Ventas = 0, Podv = 0, Pventas = 0, Pventasneto = 0, Pmargen = 0, Pmargenpor = 0;
            JsonResponse objResult = new JsonResponse();
            
            if (isOkUpdate)
            {                
                EntMovimientosVentas.FechaInicio = DateTime.Parse(FechaInicio);
                EntMovimientosVentas.FechaFin = DateTime.Parse(FechaFin);
                EntMovimientosVentas.IdTipoArticulo = TipoArticulo;
                List<Ent_Movimientos_Ventas> _ListarMovimientosVentas = datFacturacion.ListarVenPorCategoria(EntMovimientosVentas).ToList();
                Session[_session_ListarMovimientosVentas] = _ListarMovimientosVentas;
                Session[_session_ListarMovimientosVentasChart] = _ListarMovimientosVentas;

            }

            /*verificar si esta null*/
            if (Session[_session_ListarMovimientosVentas] == null)
            {
                List<Ent_Movimientos_Ventas> _ListarMovimientosVentas = new List<Ent_Movimientos_Ventas>();
                Session[_session_ListarMovimientosVentas] = _ListarMovimientosVentas;
            }

            if (isOkSemanal)
            {
                List<Ent_Movimientos_Ventas> _ListarData = (List<Ent_Movimientos_Ventas>)Session[_session_ListarMovimientosVentas];
                List<Ent_Movimientos_Ventas> _ListarReturn = new List<Ent_Movimientos_Ventas>();
                 _ListarReturn = (from x in _ListarData
                                  group x by x.Mcv_Description into y
                                     select new Ent_Movimientos_Ventas
                                     {
                                         Anno = 0,
                                         Can_Week_No = 0,
                                         Mcv_Description = y.Key,
                                         Ventas = y.Sum(x => x.Ventas),
                                         Podv = y.Sum(x => x.Podv),
                                         Pventas = y.Sum(x => x.Pventas),
                                         Pventasneto = y.Sum(x => x.Pventasneto),
                                         Pmargen = y.Sum(x => x.Pmargen),
                                         Pmargenpor = y.Sum(x => x.Pmargenpor)
                                     }).Distinct().ToList();
                Session[_session_ListarMovimientosVentas] = _ListarReturn;
            }

            /*Ini Crear chart*/
            List<Ent_Movimientos_Ventas> _ListarChartData = (List<Ent_Movimientos_Ventas>)Session[_session_ListarMovimientosVentasChart];
            List<Ent_Movimientos_Ventas> _ListarChartReturn = new List<Ent_Movimientos_Ventas>();
            _ListarChartReturn = (from x in _ListarChartData
                                  group x by x.Mcv_Description into y
                             select new Ent_Movimientos_Ventas
                             {
                                 Mcv_Description = y.Key,
                                 Pventasneto = y.Sum(x => x.Pventasneto)
                             }).Distinct().ToList();

            Ent_Movimientos_Ventas_Chart_Data EntMovVentasChartData = new Ent_Movimientos_Ventas_Chart_Data();
            EntMovVentasChartData.datasets = new List<Ent_Movimientos_Ventas_Chart>()
            {
                (new Ent_Movimientos_Ventas_Chart()
                {
                    label = "",
                    backgroundColor = new string[] { "#3c8dbc", "#00c0ef", "#00a65a", "#f39c12", "#f56954", "#d2d6de", "#001F3F", "#39CCCC", "#605ca8", "#ca195a", "#009473"},//Enumerable.Repeat("#0EA5FF",_ListarChartReturn.Count).ToArray(),
                    borderWidth = "1",
                    data = _ListarChartReturn.Select(s => s.Pventasneto).ToArray()
                })
            };
            EntMovVentasChartData.labels = _ListarChartReturn.Select(s => s.Mcv_Description).ToArray();
            objResult.Data = EntMovVentasChartData;
            objResult.Success = (_ListarChartData.Count == 0 ? false : true);
            var JSONChart = JsonConvert.SerializeObject(objResult);
            /*fin Crear chart*/

            IQueryable<Ent_Movimientos_Ventas> entDocTrans = ((List<Ent_Movimientos_Ventas>)(Session[_session_ListarMovimientosVentas])).AsQueryable();
            //Manejador de filtros
            int totalCount = entDocTrans.Count();
            IEnumerable<Ent_Movimientos_Ventas> filteredMembers = entDocTrans;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = entDocTrans.Where(
                        m =>
                            m.Mcv_Description.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Anno.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Can_Week_No.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Ventas.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Podv.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Pventas.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Pventasneto.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Pmargen.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Pmargenpor.ToString().ToUpper().Contains(param.sSearch.ToUpper())

                );
            }
            var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);

            if (param.iSortingCols > 0)
            {
                if (Request["sSortDir_0"].ToString() == "asc")
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderBy(o => o.Mcv_Description); break;
                        case 1: filteredMembers = filteredMembers.OrderBy(o => o.Anno); break;
                        case 2: filteredMembers = filteredMembers.OrderBy(o => o.Can_Week_No); break;
                        case 3: filteredMembers = filteredMembers.OrderBy(o => o.Ventas); break;
                        case 4: filteredMembers = filteredMembers.OrderBy(o => o.Podv); break;
                        case 5: filteredMembers = filteredMembers.OrderBy(o => o.Pventas); break;
                        case 6: filteredMembers = filteredMembers.OrderBy(o => o.Pventasneto); break;
                        case 7: filteredMembers = filteredMembers.OrderBy(o => o.Pmargen); break;
                        case 8: filteredMembers = filteredMembers.OrderBy(o => o.Pmargenpor); break;
                    }
                }
                else
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderByDescending(o => o.Mcv_Description); break;
                        case 1: filteredMembers = filteredMembers.OrderByDescending(o => o.Anno); break;
                        case 2: filteredMembers = filteredMembers.OrderByDescending(o => o.Can_Week_No); break;
                        case 3: filteredMembers = filteredMembers.OrderByDescending(o => o.Ventas); break;
                        case 4: filteredMembers = filteredMembers.OrderByDescending(o => o.Podv); break;
                        case 5: filteredMembers = filteredMembers.OrderByDescending(o => o.Pventas); break;
                        case 6: filteredMembers = filteredMembers.OrderByDescending(o => o.Pventasneto); break;
                        case 7: filteredMembers = filteredMembers.OrderByDescending(o => o.Pmargen); break;
                        case 8: filteredMembers = filteredMembers.OrderByDescending(o => o.Pmargenpor); break;
                    }
                }
            }

            if (totalCount > 0)
            {
                Ventas = filteredMembers.Sum(v => v.Ventas);
                Podv = filteredMembers.Sum(v => v.Podv);
                Pventas = filteredMembers.Sum(v => v.Pventas);
                Pventasneto = filteredMembers.Sum(v => v.Pventasneto);
                Pmargen = filteredMembers.Sum(v => v.Pmargen);
                Pmargenpor = ((Pventasneto - Podv) / Pventasneto);
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
                dVentas = Ventas,
                dPodv = Podv,
                dPventas = Pventas,
                dPventasneto = Pventasneto,
                dPmargen = Pmargen,
                dPmargenpor = Pmargenpor,
                jsonChart = JSONChart
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Crea el archivo en excel
        /// </summary>
        /// <create>Juilliand R. Damian Gomez </create>
        /// <update></update>       
        /// <param name="_Ent"></param>
        /// <returns></returns>
        public ActionResult get_exporta_ListarMovimientosVentas_excel(Ent_Movimientos_Ventas _Ent)
        {
            JsonResponse objResult = new JsonResponse();
            try
            {
                Session[_session_ListarMovimientosVentas_Excel] = null;
                string cadena = "";
                if (Session[_session_ListarMovimientosVentas] != null)
                {

                    List<Ent_Movimientos_Ventas> _ListarMovimientosVentas = (List<Ent_Movimientos_Ventas>)Session[_session_ListarMovimientosVentas];
                    if (_ListarMovimientosVentas.Count == 0)
                    {
                        objResult.Success = false;
                        objResult.Message = "No hay filas para exportar";
                    }
                    else
                    {
                        cadena = get_html_ListarMovimientosVentas_str((List<Ent_Movimientos_Ventas>)Session[_session_ListarMovimientosVentas], _Ent);
                        if (cadena.Length == 0)
                        {
                            objResult.Success = false;
                            objResult.Message = "Error del formato html";
                        }
                        else
                        {
                            objResult.Success = true;
                            objResult.Message = "Se genero el excel correctamente";
                            Session[_session_ListarMovimientosVentas_Excel] = cadena;
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
        public string get_html_ListarMovimientosVentas_str(List<Ent_Movimientos_Ventas> _ListarMovimientosVentas, Ent_Movimientos_Ventas _Ent)
        {
            Decimal? Ventas = 0, Podv = 0, Pventas = 0, Pventasneto = 0, Pmargen = 0, Pmargenpor = 0;

            StringBuilder sb = new StringBuilder();
            var Lista = _ListarMovimientosVentas.ToList();
            Ventas = Lista.Sum(v => v.Ventas);
            Podv = Lista.Sum(v => v.Podv);
            Pventas = Lista.Sum(v => v.Pventas);
            Pventasneto = Lista.Sum(v => v.Pventasneto);
            Pmargen = Lista.Sum(v => v.Pmargen);
            Pmargenpor = ((Pventasneto - Podv) / Pventasneto);
            try
            {
                sb.Append("<div><table cellspacing='0' style='width: 1000px' rules='all' border='0' style='border-collapse:collapse;'><tr><td Colspan='9'></td></tr><tr><td Colspan='9' valign='middle' align='center' style='vertical-align: middle;font-size: 16.0pt;font-weight: bold;color:#285A8F'>REPORTE DE " + _Ent.Descripcion + "</td></tr><tr><td Colspan='9' valign='middle' align='center' style='vertical-align: middle;font-size: 10.0pt;font-weight: bold;color:#000000'>Desde el " + String.Format("{0:dd/MM/yyyy}", _Ent.FechaInicio) + " hasta el " + String.Format("{0:dd/MM/yyyy}", _Ent.FechaFin) + "</td></tr></table>");
                sb.Append("<table  border='1' bgColor='#ffffff' borderColor='#FFFFFF' cellSpacing='2' cellPadding='2' style='font-size:10.0pt; font-family:Calibri; background:white;width: 1000px'><tr  bgColor='#5799bf'>\n");
                sb.Append("<tr bgColor='#1E77AB'>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Año</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Semana</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Categoría</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Unidades</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Costo Std</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Ventas Bruto</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Ventas Neta</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Margen</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Margen %</font></th>\n");
                sb.Append("</tr>\n");

                foreach (var item in Lista)
                {
                    sb.Append("<tr>\n");                    
                    sb.Append("<td align='Center'>" + (item.Anno == 0 ? "" : ""  + item.Anno ) + "</td>\n");
                    sb.Append("<td align='Center'>" + (item.Can_Week_No == 0 ?  "" : "" + item.Can_Week_No) + "</td>\n");
                    sb.Append("<td align=''>" + item.Mcv_Description + "</td>\n");
                    sb.Append("<td align='Center'>" + item.Ventas + "</td>\n");
                    sb.Append("<td align='right'>" + "S/ " + string.Format("{0:F2}", item.Podv) + "</td>\n");
                    sb.Append("<td align='right'>" + "S/ " + string.Format("{0:F2}", item.Pventas) + "</td>\n");
                    sb.Append("<td align='right'>" + "S/ " + string.Format("{0:F2}", item.Pventasneto) + "</td>\n");
                    sb.Append("<td align='right'>" + "S/ " + string.Format("{0:F2}", item.Pmargen) + "</td>\n");
                    sb.Append("<td align='right'>" + string.Format("{0:F2}", item.Pmargenpor) + "%" + "</td>\n");
                    sb.Append("</tr>\n");
                }
                sb.Append("<tfoot>\n");
                sb.Append("<tr bgcolor='#085B8C'>\n");
                sb.Append("<th></th>\n");
                sb.Append("<th></th>\n");
                sb.Append("<th style='text-align:left;font-weight:bold;font-size:11.0pt; '><font color='#FFFFFF'>Totales</font></td>\n");
                sb.Append("<th style='text-align:right;font-weight: bold;font-size:11.0pt; '><font color='#FFFFFF'>" +   Ventas + "</font></th>\n");
                sb.Append("<th style='text-align:right;font-weight: bold;font-size:11.0pt; '><font color='#FFFFFF'>" + "S/ " + String.Format("{0:N2}", Podv) + "</font></th>\n");
                sb.Append("<th style='text-align:right;font-weight: bold;font-size:11.0pt; '><font color='#FFFFFF'>" + "S/ " + String.Format("{0:N2}", Pventas) + "</font></th>\n");
                sb.Append("<th style='text-align:right;font-weight: bold;font-size:11.0pt; '><font color='#FFFFFF'>" + "S/ " + String.Format("{0:N2}", Pventasneto) + "</font></th>\n");
                sb.Append("<th style='text-align:right;font-weight: bold;font-size:11.0pt; '><font color='#FFFFFF'>" + "S/ " + String.Format("{0:N2}", Pmargen) + "</font></th>\n");
                sb.Append("<th style='text-align:right;font-weight: bold;font-size:11.0pt; '><font color='#FFFFFF'>" +  String.Format("{0:N2}", (Pmargenpor*100)) + "%"+ "</font></th>\n");
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
        /// Exportamos el excel
        /// </summary>
        /// <create>Juilliand R. Damian Gomez </create>
        /// <update></update>
        /// <returns>xlx</returns>
        public ActionResult ListarMovimientosVentasExcel()
        {
            string NombreArchivo = "VentaxCategoriaxSemana";
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
                Response.Write(Session[_session_ListarMovimientosVentas_Excel].ToString());
                Response.End();
            }
            catch
            {

            }
            return Json(new { estado = 0, mensaje = 1 });
        }
        #endregion

        #region <CONSULTA DE COMISIONES>
        public ActionResult Consulta_Comisiones()
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
                Ent_Comisiones EntComisiones = new Ent_Comisiones();
                ViewBag.EntComisiones = EntComisiones;
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
        /// <param name="isOkSemanal"></param>
        /// <param name="FechaInicio"></param>
        /// <param name="FechaFin"></param>
        /// <param name="TipoArticulo"></param>
        /// <returns></returns>
        public JsonResult getLisComicionesAjax(Ent_jQueryDataTableParams param, bool isOkUpdate,string FechaInicio, string FechaFin)
        {
            Ent_Comisiones EntComisiones = new Ent_Comisiones();
            if (isOkUpdate)
            {
                EntComisiones.FechaInicio = DateTime.Parse(FechaInicio);
                EntComisiones.FechaFin = DateTime.Parse(FechaFin);
                
                List<Ent_Comisiones> _ListarComisiones = datFacturacion.ListarComisiones(EntComisiones).ToList();
                Session[_session_ListarComisiones] = _ListarComisiones;
            }

            /*verificar si esta null*/
            if (Session[_session_ListarComisiones] == null)
            {
                List<Ent_Comisiones> _ListarComisiones = new List<Ent_Comisiones>();
                Session[_session_ListarComisiones] = _ListarComisiones;
            }

            IQueryable<Ent_Comisiones> entDocTrans = ((List<Ent_Comisiones>)(Session[_session_ListarComisiones])).AsQueryable();
            //Manejador de filtros
            int totalCount = entDocTrans.Count();
            IEnumerable<Ent_Comisiones> filteredMembers = entDocTrans;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = entDocTrans.Where(
                        m =>
                            m.Asesor.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Lider.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.LiderDni.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.TotPares.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.TotVenta.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.PorComision.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Comision.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Bonosnuevas.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.SubTotalSinIGV.ToString().ToUpper().Contains(param.sSearch.ToUpper())
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
                        case 1: filteredMembers = filteredMembers.OrderBy(o => o.Lider); break;
                        case 2: filteredMembers = filteredMembers.OrderBy(o => o.LiderDni); break;
                        case 3: filteredMembers = filteredMembers.OrderBy(o => o.TotPares); break;
                        case 4: filteredMembers = filteredMembers.OrderBy(o => o.TotVenta); break;
                        case 5: filteredMembers = filteredMembers.OrderBy(o => o.PorComision); break;
                        case 6: filteredMembers = filteredMembers.OrderBy(o => o.Comision); break;
                        case 7: filteredMembers = filteredMembers.OrderBy(o => o.Bonosnuevas); break;
                        case 8: filteredMembers = filteredMembers.OrderBy(o => o.SubTotalSinIGV); break;

                    }
                }
                else
                {
                    switch (sortIdx)
                    {                         
                        case 0: filteredMembers = filteredMembers.OrderByDescending(o => o.Asesor); break;
                        case 1: filteredMembers = filteredMembers.OrderByDescending(o => o.Lider); break;
                        case 2: filteredMembers = filteredMembers.OrderByDescending(o => o.LiderDni); break;
                        case 3: filteredMembers = filteredMembers.OrderByDescending(o => o.TotPares); break;
                        case 4: filteredMembers = filteredMembers.OrderByDescending(o => o.TotVenta); break;
                        case 5: filteredMembers = filteredMembers.OrderByDescending(o => o.PorComision); break;
                        case 6: filteredMembers = filteredMembers.OrderByDescending(o => o.Comision); break;
                        case 7: filteredMembers = filteredMembers.OrderByDescending(o => o.Bonosnuevas); break;
                        case 8: filteredMembers = filteredMembers.OrderByDescending(o => o.SubTotalSinIGV); break;
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
        public ActionResult get_exporta_ListarComisines_excel(Ent_Comisiones _Ent)
        {
            JsonResponse objResult = new JsonResponse();
            try
            {
                Session[_session_ListarComisiones_Excel] = null;
                string cadena = "";
                if (Session[_session_ListarComisiones] != null)
                {

                    List<Ent_Comisiones> _ListarComisiones = (List<Ent_Comisiones>)Session[_session_ListarComisiones];
                    if (_ListarComisiones.Count == 0)
                    {
                        objResult.Success = false;
                        objResult.Message = "No hay filas para exportar";
                    }
                    else
                    {
                        cadena = get_html_ListarComisiones_str((List<Ent_Comisiones>)Session[_session_ListarComisiones], _Ent);
                        if (cadena.Length == 0)
                        {
                            objResult.Success = false;
                            objResult.Message = "Error del formato html";
                        }
                        else
                        {
                            objResult.Success = true;
                            objResult.Message = "Se genero el excel correctamente";
                            Session[_session_ListarComisiones_Excel] = cadena;
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
        public string get_html_ListarComisiones_str(List<Ent_Comisiones> _ListarComisiones, Ent_Comisiones _Ent)
        {
            StringBuilder sb = new StringBuilder();
            var Lista = _ListarComisiones.OrderByDescending(x => x.TotVenta).ToList(); ;
            try
            {
                sb.Append("<div><table cellspacing='0' style='width: 1000px' rules='all' border='0' style='border-collapse:collapse;'><tr><td Colspan='9'></td></tr><tr><td Colspan='9' valign='middle' align='center' style='vertical-align: middle;font-size: 16.0pt;font-weight: bold;color:#285A8F'>REPORTE DE COMISIONES</td></tr><tr><td Colspan='9' valign='middle' align='center' style='vertical-align: middle;font-size: 10.0pt;font-weight: bold;color:#000000'>Desde el " + String.Format("{0:dd/MM/yyyy}", _Ent.FechaInicio) + " hasta el " + String.Format("{0:dd/MM/yyyy}", _Ent.FechaFin) + "</td></tr></table>");
                sb.Append("<table  border='1' bgColor='#ffffff' borderColor='#FFFFFF' cellSpacing='2' cellPadding='2' style='font-size:10.0pt; font-family:Calibri; background:white;width: 1000px'><tr  bgColor='#5799bf'>\n");
                sb.Append("<tr bgColor='#1E77AB'>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Asesor</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Lider</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>DNI - Lider</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Total Pares</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Total Venta Neta</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>% de Comision</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Comision Lider</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Bonos nuevas</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>SubTotal Sin IGV</font></th>\n");
                sb.Append("</tr>\n");
                // {0:N2} Separacion miles , {0:F2} solo dos decimales
                foreach (var item in Lista)
                {
                    sb.Append("<tr>\n");
                    sb.Append("<td align=''>" + item.Asesor + "</td>\n");
                    sb.Append("<td align=''>" + item.Lider + "</td>\n");
                    sb.Append("<td align='Center'>" + item.LiderDni+ "</td>\n");
                    sb.Append("<td align='Center'>" + item.TotPares + "</td>\n");
                    sb.Append("<td align='right'>" + (item.TotVenta == null ? " " : "S/ " + string.Format("{0:N2}", item.TotVenta)) + "</td>\n");
                    sb.Append("<td align='right'>" + string.Format("{0:n0}", item.PorComision) + "</td>\n");
                    sb.Append("<td align='right'>" + (item.Comision == null ? " " : "S/ " + string.Format("{0:N2}", item.Comision)) + "</td>\n");
                    sb.Append("<td align='right'>" + (item.Bonosnuevas == null ? " " : "S/ " + string.Format("{0:N2}", item.Bonosnuevas)) + "</td>\n");
                    sb.Append("<td align='right'>" + (item.SubTotalSinIGV == null ? " " : "S/ " + string.Format("{0:N2}", item.SubTotalSinIGV)) + "</td>\n");
                    sb.Append("</tr>\n");
                }
                sb.Append("<tfoot>\n");
                sb.Append("<tr bgcolor='#085B8C'>\n");
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
        /// Exportamos el excel
        /// </summary>
        /// <create>Juilliand R. Damian Gomez </create>
        /// <update></update>
        /// <returns>xlx</returns>
        public ActionResult ListarComisionesExcel()
        {
            string NombreArchivo = "comisione_bono_xlider";
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
                Response.Write(Session[_session_ListarComisiones_Excel].ToString());
                Response.End();
            }
            catch
            {

            }
            return Json(new { estado = 0, mensaje = 1 });
        }
        #endregion

        #region <RESUMEN DE VENTAS POR SEMANA>
        /// <summary>
        /// RESUMEN DE VENTAS POR SEMANA
        /// </summary>
        /// <create>Juilliand R. Damian Gomez </create>
        /// <update></update> 
        /// <returns></returns>
        public ActionResult Resumen_Ventas()
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
                Ent_Resumen_Ventas EntResumenVentas = new Ent_Resumen_Ventas();
                ViewBag.EntResumenVentas = EntResumenVentas;
                ViewBag.ListarAnno = datFacturacion.ListarAnno();
                return View();
            }

        }
        /// <summary>
        /// LISTADO PRINCIPAL
        /// </summary>
        /// <create>Juilliand R. Damian Gomez </create>
        /// <update></update>         
        /// <param name="param"></param>
        /// <param name="isOkUpdate"></param>
        /// <param name="Anno"></param>
        /// <returns></returns>
        public JsonResult getResumenVentasAjax(Ent_jQueryDataTableParams param, bool isOkUpdate, int Anno)
        {
            Ent_Resumen_Ventas EntResumenVentas = new Ent_Resumen_Ventas();
            if (isOkUpdate)
            {
                EntResumenVentas.Anno = Anno;


                List<Ent_Resumen_Ventas> _ListarResumenVenta = datFacturacion.ListarResumenVenta(EntResumenVentas).ToList();
                Session[_session_ListarVentasResumido] = _ListarResumenVenta;
            }

            /*verificar si esta null*/
            if (Session[_session_ListarVentasResumido] == null)
            {
                List<Ent_Resumen_Ventas> _ListarResumenVenta = new List<Ent_Resumen_Ventas>();
                Session[_session_ListarVentasResumido] = _ListarResumenVenta;
            }

            IQueryable<Ent_Resumen_Ventas> entDocTrans = ((List<Ent_Resumen_Ventas>)(Session[_session_ListarVentasResumido])).AsQueryable();
            //Manejador de filtros
            int totalCount = entDocTrans.Count();
            IEnumerable<Ent_Resumen_Ventas> filteredMembers = entDocTrans;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = entDocTrans.Where(
                        m =>
                            m.Anno.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Semana.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.TotalTickets.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Pares.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.TotalIgv.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.PrecioPromedio.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.NParesTicket.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Anno1.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Semana1.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.TotalTickets1.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Pares1.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.TotalIgv1.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.PrecioPromedio1.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.NParesTicket1.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                );
            }
            var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);

            if (param.iSortingCols > 0)
            {
                if (Request["sSortDir_0"].ToString() == "asc")
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderBy(o => o.Anno); break;
                        case 1: filteredMembers = filteredMembers.OrderBy(o => o.Semana); break;
                        case 2: filteredMembers = filteredMembers.OrderBy(o => o.TotalTickets); break;
                        case 3: filteredMembers = filteredMembers.OrderBy(o => o.Pares); break;
                        case 4: filteredMembers = filteredMembers.OrderBy(o => o.TotalIgv); break;
                        case 5: filteredMembers = filteredMembers.OrderBy(o => o.PrecioPromedio); break;
                        case 6: filteredMembers = filteredMembers.OrderBy(o => o.NParesTicket); break;
                        case 7: filteredMembers = filteredMembers.OrderBy(o => o.Anno1); break;
                        case 8: filteredMembers = filteredMembers.OrderBy(o => o.Semana1); break;
                        case 9: filteredMembers = filteredMembers.OrderBy(o => o.TotalTickets1); break;
                        case 10: filteredMembers = filteredMembers.OrderBy(o => o.Pares1); break;
                        case 11: filteredMembers = filteredMembers.OrderBy(o => o.TotalIgv1); break;
                        case 12: filteredMembers = filteredMembers.OrderBy(o => o.PrecioPromedio1); break;
                        case 13: filteredMembers = filteredMembers.OrderBy(o => o.NParesTicket1); break;
                    }
                }
                else
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderByDescending(o => o.Anno); break;
                        case 1: filteredMembers = filteredMembers.OrderByDescending(o => o.Semana); break;
                        case 2: filteredMembers = filteredMembers.OrderByDescending(o => o.TotalTickets); break;
                        case 3: filteredMembers = filteredMembers.OrderByDescending(o => o.Pares); break;
                        case 4: filteredMembers = filteredMembers.OrderByDescending(o => o.TotalIgv); break;
                        case 5: filteredMembers = filteredMembers.OrderByDescending(o => o.PrecioPromedio); break;
                        case 6: filteredMembers = filteredMembers.OrderByDescending(o => o.NParesTicket); break;
                        case 7: filteredMembers = filteredMembers.OrderByDescending(o => o.Anno1); break;
                        case 8: filteredMembers = filteredMembers.OrderByDescending(o => o.Semana1); break;
                        case 9: filteredMembers = filteredMembers.OrderByDescending(o => o.TotalTickets1); break;
                        case 10: filteredMembers = filteredMembers.OrderByDescending(o => o.Pares1); break;
                        case 11: filteredMembers = filteredMembers.OrderByDescending(o => o.TotalIgv1); break;
                        case 12: filteredMembers = filteredMembers.OrderByDescending(o => o.PrecioPromedio1); break;
                        case 13: filteredMembers = filteredMembers.OrderByDescending(o => o.NParesTicket1); break;
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
        /// VALIDACIONES PARA EXPORTAR EN EXCEL
        /// </summary>
        /// <create>Juilliand R. Damian Gomez </create>
        /// <update></update>        
        /// <param name="_Ent"></param>
        /// <returns></returns>
        public ActionResult get_exporta_ListarResumenVenta_excel(Ent_Resumen_Ventas _Ent)
        {
            JsonResponse objResult = new JsonResponse();
            try
            {
                Session[_session_ListarVentasResumido_Excel] = null;
                string cadena = "";
                if (Session[_session_ListarVentasResumido] != null)
                {

                    List<Ent_Resumen_Ventas> _ListarVentasResumido = (List<Ent_Resumen_Ventas>)Session[_session_ListarVentasResumido];
                    if (_ListarVentasResumido.Count == 0)
                    {
                        objResult.Success = false;
                        objResult.Message = "No hay filas para exportar";
                    }
                    else
                    {
                        cadena = get_html_ListarVentasResumido_str((List<Ent_Resumen_Ventas>)Session[_session_ListarVentasResumido], _Ent);
                        if (cadena.Length == 0)
                        {
                            objResult.Success = false;
                            objResult.Message = "Error del formato html";
                        }
                        else
                        {
                            objResult.Success = true;
                            objResult.Message = "Se genero el excel correctamente";
                            Session[_session_ListarVentasResumido_Excel] = cadena;
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
        /// CREAMOS EL CUERPO DEL EXCEL
        /// </summary>
        /// <create>Juilliand R. Damian Gomez </create>
        /// <update></update>  
        /// <param name="_ListarVentasResumido"></param>
        /// <param name="_Ent"></param>
        /// <returns></returns>
        public string get_html_ListarVentasResumido_str(List<Ent_Resumen_Ventas> _ListarVentasResumido, Ent_Resumen_Ventas _Ent)
        {
            StringBuilder sb = new StringBuilder();
            var Lista = _ListarVentasResumido.ToList(); ;
            try
            {
                sb.Append("<div>");
                sb.Append("<table cellspacing='0' style='width: 1000px' rules='all' border='0' style='border-collapse:collapse;'>");
                sb.Append("<tr><td Colspan='14'></td></tr>");
                sb.Append("<tr><td Colspan='14' valign='middle' align='center' style='vertical-align: middle;font-size: 18.0pt;font-weight: bold;color:#285A8F'>REPORTE DE VENTAS POR SEMANA</td></tr>");
                //sb.Append("<tr><td Colspan='14' valign='middle' align='center' style='vertical-align: middle;font-size: 10.0pt;font-weight: bold;color:#000000'>Ventas de semana con el año anterior al " + _Ent.Anno+ "</td></tr>");//subtitulo
                sb.Append("</table>");
                sb.Append("<table  border='1' bgColor='#ffffff' borderColor='#FFFFFF' cellSpacing='2' cellPadding='2' style='font-size:10.0pt; font-family:Calibri; background:white;width: 1000px'><tr  bgColor='#5799bf'>\n");
                sb.Append("<tr bgColor='#1E77AB'>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Año</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Semana</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Total Tickets</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Pares</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Total + Igv</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Precio Promedio</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>N Pares por Ticket</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Año</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Semana</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Total Tickets</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Pares</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Total + Igv</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Precio Promedio</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>N Pares por Ticket</font></th>\n");
                sb.Append("</tr>\n");
                // {0:N2} Separacion miles , {0:F2} solo dos decimales
                foreach (var item in Lista)
                {
                    sb.Append("<tr>\n");
                    sb.Append("<td bgcolor='#B1DEF6' align='Center'>" + item.Anno + "</td>\n");
                    sb.Append("<td bgcolor='#B1DEF6' align='Center'>" + item.Semana + "</td>\n");
                    sb.Append("<td bgcolor='#B1DEF6' align='Center'>" + item.TotalTickets + "</td>\n");
                    sb.Append("<td bgcolor='#B1DEF6' align='Center'>" + item.Pares + "</td>\n");
                    sb.Append("<td bgcolor='#B1DEF6' align='right'>" + (item.TotalIgv == null ? " " : "S/ " + string.Format("{0:N2}", item.TotalIgv)) + "</td>\n");
                    sb.Append("<td bgcolor='#B1DEF6' align='right'>" + (item.PrecioPromedio == null ? " " : "S/ " + string.Format("{0:N2}", item.PrecioPromedio)) + "</td>\n");
                    sb.Append("<td bgcolor='#B1DEF6' align='right'>" + (item.NParesTicket == null ? " " :  string.Format("{0:F2}", item.NParesTicket)) + "</td>\n");
                    sb.Append("<td align='Center'>" + item.Anno1 + "</td>\n");
                    sb.Append("<td align='Center'>" + item.Semana1 + "</td>\n");
                    sb.Append("<td align='Center'>" + item.TotalTickets1 + "</td>\n");
                    sb.Append("<td align='Center'>" + item.Pares1 + "</td>\n");
                    sb.Append("<td align='right'>" + (item.TotalIgv1 == null ? " " : "S/ " + string.Format("{0:N2}", item.TotalIgv1)) + "</td>\n");
                    sb.Append("<td align='right'>" + (item.PrecioPromedio1 == null ? " " : "S/ " + string.Format("{0:N2}", item.PrecioPromedio1)) + "</td>\n");
                    sb.Append("<td align='right'>" + (item.NParesTicket1 == null ? " " : string.Format("{0:F2}", item.NParesTicket1)) + "</td>\n");
                    sb.Append("</tr>\n");
                }
                sb.Append("<tfoot>\n");
                sb.Append("<tr bgcolor='#085B8C'>\n");
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
        /// Exportamos el excel
        /// </summary>
        /// <create>Juilliand R. Damian Gomez </create>
        /// <update></update>
        /// <returns>xlx</returns>
        public ActionResult ListarVentasResumidoExcel()
        {
            string NombreArchivo = "ventaResum";
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
                Response.Write(Session[_session_ListarVentasResumido_Excel].ToString());
                Response.End();
            }
            catch
            {

            }
            return Json(new { estado = 0, mensaje = 1 });
        }
        #endregion

        #region <NUEVO LIDER - VENTA>
        /// <summary>
        /// Nuevos Lideres - Ventas x Rango de Fecha
        /// </summary>
        /// <create>Juilliand R. Damian Gomez </create>
        /// <update></update> 
        /// <returns></returns>
        public ActionResult LideresVenta()
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
                Ent_Lider_Ventas EntLiderVentas = new Ent_Lider_Ventas();
                ViewBag.EntLiderVentas = EntLiderVentas;
                return View();
            }
        }
        private static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                    {

                        if (column.ColumnName == "lider")
                        {
                            pro.SetValue(obj, dr[column.ColumnName], null);
                        }
                        else
                        {
                            pro.SetValue(obj, (dr[column.ColumnName] is DBNull) ? (Decimal?)null : Convert.ToDecimal(dr[column.ColumnName]), null);
                        }                        
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            return obj;
        }
        /// <summary>
        /// LISTADO PRINCIPAL
        /// </summary>
        /// <create>Juilliand R. Damian Gomez </create>
        /// <update></update> 
        /// <param name="param"></param>
        /// <param name="isOkUpdate"></param>
        /// <param name="FechaInicio"></param>
        /// <param name="FechaFin"></param>
        /// <returns></returns>
        public JsonResult getLiderVentasAjax(Ent_jQueryDataTableParams param, bool isOkUpdate, string FechaInicio, string FechaFin)
        {
            Ent_Lider_Ventas EntLiderVentas = new Ent_Lider_Ventas();
            Decimal Enero = 0, Febrero = 0, Marzo = 0, Abril = 0, Mayo = 0, Junio = 0, Julio = 0, Agosto = 0, Septiembre = 0, Octubre = 0, Noviembre = 0, Diciembre = 0;
            if (isOkUpdate)
            {
                EntLiderVentas.FechaInicio = DateTime.Parse(FechaInicio);
                EntLiderVentas.FechaFin = DateTime.Parse(FechaFin);

                DataTable dtLiderVentas = datFacturacion.Consulta_Lider_N(EntLiderVentas);
                List<Ent_Lider_Ventas> _ListarLiderVentas = new List<Ent_Lider_Ventas>();
                _ListarLiderVentas = ConvertDataTable<Ent_Lider_Ventas>(dtLiderVentas).ToList();

                Session[_session_ListarLiderVentas] = _ListarLiderVentas;
            }

            /*verificar si esta null*/
            if (Session[_session_ListarLiderVentas] == null)
            {
                List<Ent_Lider_Ventas> _ListarLiderVentas = new List<Ent_Lider_Ventas>();
                Session[_session_ListarLiderVentas] = _ListarLiderVentas;
            }

            IQueryable<Ent_Lider_Ventas> entDocTrans = ((List<Ent_Lider_Ventas>)(Session[_session_ListarLiderVentas])).AsQueryable();
            //Manejador de filtros
            int totalCount = entDocTrans.Count();

            if (totalCount>0)
            {
                Enero = entDocTrans.Sum(x => x.Enero);
                Febrero = entDocTrans.Sum(x => x.Febrero);
                Marzo = entDocTrans.Sum(x => x.Marzo);
                Abril = entDocTrans.Sum(x => x.Abril);
                Mayo = entDocTrans.Sum(x => x.Mayo);
                Junio = entDocTrans.Sum(x => x.Junio);
                Julio = entDocTrans.Sum(x => x.Julio);
                Agosto = entDocTrans.Sum(x => x.Agosto);
                Septiembre = entDocTrans.Sum(x => x.Septiembre);
                Octubre = entDocTrans.Sum(x => x.Octubre);
                Noviembre = entDocTrans.Sum(x => x.Noviembre);
                Diciembre = entDocTrans.Sum(x => x.Diciembre);
            }
            IEnumerable<Ent_Lider_Ventas> filteredMembers = entDocTrans;


            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = entDocTrans.Where(
                        m =>
                            m.lider.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Enero.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Febrero.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Marzo.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Abril.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Mayo.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Junio.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Julio.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Agosto.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Septiembre.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Octubre.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Noviembre.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Diciembre.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Grand_Total.ToString().ToUpper().Contains(param.sSearch.ToUpper())

                );
            }
            var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);

            if (param.iSortingCols > 0)
            {
                if (Request["sSortDir_0"].ToString() == "asc")
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderBy(o => o.lider); break;
                        case 1: filteredMembers = filteredMembers.OrderBy(o => o.Enero); break;
                        case 2: filteredMembers = filteredMembers.OrderBy(o => o.Febrero); break;
                        case 3: filteredMembers = filteredMembers.OrderBy(o => o.Marzo); break;
                        case 4: filteredMembers = filteredMembers.OrderBy(o => o.Abril); break;
                        case 5: filteredMembers = filteredMembers.OrderBy(o => o.Mayo); break;
                        case 6: filteredMembers = filteredMembers.OrderBy(o => o.Junio); break;
                        case 7: filteredMembers = filteredMembers.OrderBy(o => o.Julio); break;
                        case 8: filteredMembers = filteredMembers.OrderBy(o => o.Agosto); break;
                        case 9: filteredMembers = filteredMembers.OrderBy(o => o.Septiembre); break;
                        case 10: filteredMembers = filteredMembers.OrderBy(o => o.Octubre); break;
                        case 11: filteredMembers = filteredMembers.OrderBy(o => o.Noviembre); break;
                        case 12: filteredMembers = filteredMembers.OrderBy(o => o.Diciembre); break;
                        case 13: filteredMembers = filteredMembers.OrderBy(o => o.Grand_Total); break;
                    }
                }
                else
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderByDescending(o => o.lider); break;
                        case 1: filteredMembers = filteredMembers.OrderByDescending(o => o.Enero); break;
                        case 2: filteredMembers = filteredMembers.OrderByDescending(o => o.Febrero); break;
                        case 3: filteredMembers = filteredMembers.OrderByDescending(o => o.Marzo); break;
                        case 4: filteredMembers = filteredMembers.OrderByDescending(o => o.Abril); break;
                        case 5: filteredMembers = filteredMembers.OrderByDescending(o => o.Mayo); break;
                        case 6: filteredMembers = filteredMembers.OrderByDescending(o => o.Junio); break;
                        case 7: filteredMembers = filteredMembers.OrderByDescending(o => o.Julio); break;
                        case 8: filteredMembers = filteredMembers.OrderByDescending(o => o.Agosto); break;
                        case 9: filteredMembers = filteredMembers.OrderByDescending(o => o.Septiembre); break;
                        case 10: filteredMembers = filteredMembers.OrderByDescending(o => o.Octubre); break;
                        case 11: filteredMembers = filteredMembers.OrderByDescending(o => o.Noviembre); break;
                        case 12: filteredMembers = filteredMembers.OrderByDescending(o => o.Diciembre); break;
                        case 13: filteredMembers = filteredMembers.OrderByDescending(o => o.Grand_Total); break;
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
                dEnero = Enero,
                dFebrero = Febrero,
                dMarzo = Marzo,
                dAbril = Abril,
                dMayo = Mayo,
                dJunio = Junio,
                dJulio = Julio,
                dAgosto = Agosto,
                dSeptiembre = Septiembre,
                dOctubre = Octubre,
                dNoviembre = Noviembre,
                dDiciembre = Diciembre
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// VALIDACIONES PARA EXPORTAR EN EXCEL
        /// </summary>
        /// <create>Juilliand R. Damian Gomez </create>
        /// <update></update>        
        /// <param name="_Ent"></param>
        /// <returns></returns>
        public ActionResult get_exporta_ListarLiderVentas_excel(Ent_Lider_Ventas _Ent)
        {
            JsonResponse objResult = new JsonResponse();
            try
            {
                Session[_session_ListarLiderVentas_Excel] = null;
                string cadena = "";
                if (Session[_session_ListarLiderVentas] != null)
                {

                    List<Ent_Lider_Ventas> _ListarLiderVentas = (List<Ent_Lider_Ventas>)Session[_session_ListarLiderVentas];
                    if (_ListarLiderVentas.Count == 0)
                    {
                        objResult.Success = false;
                        objResult.Message = "No hay filas para exportar";
                    }
                    else
                    {
                        cadena = get_html_ListarLiderVentas_str((List<Ent_Lider_Ventas>)Session[_session_ListarLiderVentas], _Ent);
                        if (cadena.Length == 0)
                        {
                            objResult.Success = false;
                            objResult.Message = "Error del formato html";
                        }
                        else
                        {
                            objResult.Success = true;
                            objResult.Message = "Se genero el excel correctamente";
                            Session[_session_ListarLiderVentas_Excel] = cadena;
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
        /// CREAMOS EL CUERPO DEL EXCEL
        /// </summary>
        /// <create>Juilliand R. Damian Gomez </create>
        /// <update></update>  
        /// <param name="_ListarLiderVentas"></param>
        /// <param name="_Ent"></param>
        /// <returns></returns>
        public string get_html_ListarLiderVentas_str(List<Ent_Lider_Ventas> _ListarLiderVentas, Ent_Lider_Ventas _Ent)
        {
            StringBuilder sb = new StringBuilder();
            Decimal Enero = 0, Febrero = 0, Marzo = 0, Abril = 0, Mayo = 0, Junio = 0, Julio = 0, Agosto = 0, Septiembre = 0, Octubre = 0, Noviembre = 0, Diciembre = 0;
            var TCS = 0; //Tamaño de Colspan
            var Lista = _ListarLiderVentas.ToList();

            Enero = _ListarLiderVentas.Sum(x => x.Enero);
            TCS += (Enero == 0 ? 0 : 1);
            Febrero = _ListarLiderVentas.Sum(x => x.Febrero);
            TCS += (Febrero == 0 ? 0 : 1);
            Marzo = _ListarLiderVentas.Sum(x => x.Marzo);
            TCS += (Marzo == 0 ? 0 : 1);
            Abril = _ListarLiderVentas.Sum(x => x.Abril);
            TCS += (Abril == 0 ? 0 : 1);
            Mayo = _ListarLiderVentas.Sum(x => x.Mayo);
            TCS += (Mayo == 0 ? 0 : 1);
            Junio = _ListarLiderVentas.Sum(x => x.Junio);
            TCS += (Junio == 0 ? 0 : 1);
            Julio = _ListarLiderVentas.Sum(x => x.Julio);
            TCS += (Julio == 0 ? 0 : 1);
            Agosto = _ListarLiderVentas.Sum(x => x.Agosto);
            TCS += (Agosto == 0 ? 0 : 1);
            Septiembre = _ListarLiderVentas.Sum(x => x.Septiembre);
            TCS += (Septiembre == 0 ? 0 : 1);
            Octubre = _ListarLiderVentas.Sum(x => x.Octubre);
            TCS += (Octubre == 0 ? 0 : 1);
            Noviembre = _ListarLiderVentas.Sum(x => x.Noviembre);
            TCS += (Noviembre == 0 ? 0 : 1);
            Diciembre = _ListarLiderVentas.Sum(x => x.Diciembre);
            TCS += (Diciembre == 0 ? 0 : 1);
            try
            {
                sb.Append("<div>");
                sb.Append("<table cellspacing='0' style='width: 1000px' rules='all' border='0' style='border-collapse:collapse;'>");
                sb.Append("<tr><td Colspan=" + (TCS + 2) + "></td></tr>");
                sb.Append("<tr><td Colspan=" + (TCS + 2) + " valign='middle' align='center' style='vertical-align: middle;font-size: 18.0pt;font-weight: bold;color:#285A8F'>REPORTE DE VENTAS - NUEVOS LIDERES</td></tr>");
                sb.Append("<tr><td Colspan=" + (TCS + 2) + " valign='middle' align='center' style='vertical-align: middle;font-size: 10.0pt;font-weight: bold;color:#000000'>Desde el " + String.Format("{0:dd/MM/yyyy}", _Ent.FechaInicio) + " hasta el " + String.Format("{0:dd/MM/yyyy}", _Ent.FechaFin) + "</td></tr>");//subtitulo
                sb.Append("</table>");
                sb.Append("<table  border='1' bgColor='#ffffff' borderColor='#FFFFFF' cellSpacing='2' cellPadding='2' style='font-size:10.0pt; font-family:Calibri; background:white;width: 1000px'><tr  bgColor='#5799bf'>\n");
                sb.Append("<tr bgColor='#1E77AB'>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Lider</font></th>\n");

                if (Enero>0)
                {
                    sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Enero </font></th>\n");
                }
                if (Febrero > 0)
                {
                    sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Febrero</font></th>\n");
                }
                if (Marzo > 0)
                {
                    sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Marzo</font></th>\n");
                }
                if (Abril > 0)
                {
                    sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Abril</font></th>\n");
                }
                if (Mayo > 0)
                {
                    sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Mayo</font></th>\n");
                }
                if (Junio > 0)
                {
                    sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Junio</font></th>\n");
                }

                if (Julio > 0)
                {
                    sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Julio</font></th>\n");
                }
                if (Agosto > 0)
                {
                    sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Agosto</font></th>\n");
                }
                if (Septiembre > 0)
                {
                    sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Septiembre</font></th>\n");
                }
                if (Octubre > 0)
                {
                    sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Octubre</font></th>\n");
                }
                if (Noviembre > 0)
                {
                    sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Noviembre</font></th>\n");
                }
                if (Diciembre > 0)
                {
                    sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Diciembre</font></th>\n");
                }


                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Total</font></th>\n");
                sb.Append("</tr>\n");
                // {0:N2} Separacion miles , {0:F2} solo dos decimales
                foreach (var item in Lista)
                {
                    sb.Append("<tr>\n");
                    sb.Append("<td bgcolor='' align=''>" + item.lider + "</td>\n");
                    if (Enero > 0)
                    {
                        sb.Append("<td align='right'>" + (item.Enero == null || item.Enero == 0 ? " " : "S/ " + string.Format("{0:N2}", item.Enero)) + "</td>\n");
                    }
                    if (Febrero > 0)
                    {
                        sb.Append("<td align='right'>" + (item.Febrero == null || item.Febrero == 0 ? " " : "S/ " + string.Format("{0:N2}", item.Febrero)) + "</td>\n");
                    }
                    if (Marzo > 0)
                    {
                        sb.Append("<td align='right'>" + (item.Marzo == null || item.Marzo == 0 ? " " : "S/ " + string.Format("{0:N2}", item.Marzo)) + "</td>\n");
                    }
                    if (Abril > 0)
                    {
                        sb.Append("<td align='right'>" + (item.Abril == null || item.Abril == 0 ? " " : "S/ " + string.Format("{0:N2}", item.Abril)) + "</td>\n");
                    }
                    if (Mayo > 0)
                    {
                        sb.Append("<td align='right'>" + (item.Mayo == null || item.Mayo == 0 ? " " : "S/ " + string.Format("{0:N2}", item.Mayo)) + "</td>\n");
                    }
                    if (Junio > 0)
                    {
                        sb.Append("<td align='right'>" + (item.Junio == null || item.Junio == 0 ? " " : "S/ " + string.Format("{0:N2}", item.Junio)) + "</td>\n");
                    }
                    if (Julio > 0)
                    {
                        sb.Append("<td align='right'>" + (item.Julio == null || item.Julio == 0 ? " " : "S/ " + string.Format("{0:N2}", item.Julio)) + "</td>\n");
                    }
                    if (Agosto > 0)
                    {
                        sb.Append("<td align='right'>" + (item.Agosto == null || item.Agosto == 0 ? " " : "S/ " + string.Format("{0:N2}", item.Agosto)) + "</td>\n");
                    }
                    if (Septiembre > 0)
                    {
                        sb.Append("<td align='right'>" + (item.Septiembre == null || item.Septiembre == 0 ? " " : "S/ " + string.Format("{0:N2}", item.Septiembre)) + "</td>\n");
                    }
                    if (Octubre > 0)
                    {
                        sb.Append("<td align='right'>" + (item.Octubre == null || item.Octubre == 0 ? " " : "S/ " + string.Format("{0:N2}", item.Octubre)) + "</td>\n");
                    }
                    if (Noviembre > 0)
                    {
                        sb.Append("<td align='right'>" + (item.Noviembre == null || item.Noviembre == 0 ? " " : "S/ " + string.Format("{0:N2}", item.Noviembre)) + "</td>\n");
                    }
                    if (Diciembre > 0)
                    {
                        sb.Append("<td align='right'>" + (item.Diciembre == null || item.Diciembre == 0 ? " " : "S/ " + string.Format("{0:N2}", item.Diciembre)) + "</td>\n");

                    }                    
                    sb.Append("<td align='right'>" + (item.Grand_Total == null || item.Grand_Total == 0 ? " " : "S/ " + string.Format("{0:N2}", item.Grand_Total)) + "</td>\n");
                    sb.Append("</tr>\n");
                }
                sb.Append("<tfoot>\n");
                sb.Append("<tr bgcolor='#085B8C'>\n");
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
        /// Exportamos el excel
        /// </summary>
        /// <create>Juilliand R. Damian Gomez </create>
        /// <update></update>
        /// <returns>xlx</returns>
        public ActionResult ListarLiderVentasExcel()
        {
            string NombreArchivo = "VentaLiderNuevos";
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
                Response.Write(Session[_session_ListarLiderVentas_Excel].ToString());
                Response.End();
            }
            catch
            {

            }
            return Json(new { estado = 0, mensaje = 1 });
        }
        #endregion

        #region <REPORTE DE VENTA Y STOCK POR TALA>
        /// <summary>
        /// RESUMEN DE VENTAS  Y STOCK POR TALA
        /// </summary>
        /// <create>Juilliand R. Damian Gomez </create>
        /// <update></update> 
        /// <returns></returns>
        public ActionResult Ventas_Tallas()
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
                Ent_Ventas_Tallas EntVentasTallas = new Ent_Ventas_Tallas();
                ViewBag.EntVentasTallas = EntVentasTallas;
                return View();
            }

        }
        /// <summary>
        /// LISTADO PRINCIPAL
        /// </summary>
        /// <create>Juilliand R. Damian Gomez </create>
        /// <update></update> 
        /// <param name="param"></param>
        /// <param name="isOkUpdate"></param>
        /// <param name="FechaInicio"></param>
        /// <param name="FechaFin"></param>
        /// <param name="Articulo"></param>
        /// <returns></returns>
        public JsonResult getVentasTallaAjax(Ent_jQueryDataTableParams param, bool isOkUpdate, string FechaInicio, string FechaFin, string Articulo)
        {
            Ent_Ventas_Tallas EntVentasTallas = new Ent_Ventas_Tallas();
            if (isOkUpdate)
            {
                EntVentasTallas.FechaInicio = FechaInicio;
                EntVentasTallas.FechaFin = FechaFin;
                EntVentasTallas.Articulo = Articulo;

                List<Ent_Ventas_Tallas> _ListarVentasTallas = datFacturacion.ListarVentaTalla(EntVentasTallas).ToList();
                Session[_session_ListarVentasTallas] = _ListarVentasTallas;
            }

            /*verificar si esta null*/
            if (Session[_session_ListarVentasTallas] == null)
            {
                List<Ent_Ventas_Tallas> _ListarVentasTallas = new List<Ent_Ventas_Tallas>();
                Session[_session_ListarVentasTallas] = _ListarVentasTallas;
            }

            IQueryable<Ent_Ventas_Tallas> entDocTrans = ((List<Ent_Ventas_Tallas>)(Session[_session_ListarVentasTallas])).AsQueryable();
            //Manejador de filtros
            int totalCount = entDocTrans.Count();
            IEnumerable<Ent_Ventas_Tallas> filteredMembers = entDocTrans;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = entDocTrans.Where(
                        m =>
                           m.Articulo.ToUpper().Contains(param.sSearch.ToUpper()) ||
                           m.Pares_Venta.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                );
            }
            var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);

            if (param.iSortingCols > 0)
            {
                if (Request["sSortDir_0"].ToString() == "asc")
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderBy(o => o.Articulo); break;
                        case 1: filteredMembers = filteredMembers.OrderBy(o => o.Pares_Venta); break;
                    }
                }
                else
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderByDescending(o => o.Articulo); break;
                        case 1: filteredMembers = filteredMembers.OrderByDescending(o => o.Pares_Venta); break;
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
        /// VALIDACIONES PARA EXPORTAR EN EXCEL
        /// </summary>
        /// <create>Juilliand R. Damian Gomez </create>
        /// <update></update>        
        /// <param name="_Ent"></param>
        /// <returns></returns>
        public ActionResult get_exporta_ListarVentaTalla_excel(Ent_Ventas_Tallas _Ent)
        {
            JsonResponse objResult = new JsonResponse();
            try
            {
                Session[_session_ListarVentasTallas_Excel] = null;
                string cadena = "";
                if (Session[_session_ListarVentasTallas] != null)
                {

                    List<Ent_Ventas_Tallas> _ListarVentasTallas = (List<Ent_Ventas_Tallas>)Session[_session_ListarVentasTallas];
                    if (_ListarVentasTallas.Count == 0)
                    {
                        objResult.Success = false;
                        objResult.Message = "No hay filas para exportar";
                    }
                    else
                    {
                        cadena = get_html_ListarVentasTallas_str((List<Ent_Ventas_Tallas>)Session[_session_ListarVentasTallas], _Ent);
                        if (cadena.Length == 0)
                        {
                            objResult.Success = false;
                            objResult.Message = "Error del formato html";
                        }
                        else
                        {
                            objResult.Success = true;
                            objResult.Message = "Se genero el excel correctamente";
                            Session[_session_ListarVentasTallas_Excel] = cadena;
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
        /// CREAMOS EL CUERPO DEL EXCEL
        /// </summary>
        /// <create>Juilliand R. Damian Gomez </create>
        /// <update></update>  
        /// <param name="_ListarVentasResumido"></param>
        /// <param name="_Ent"></param>
        /// <returns></returns>
        public string get_html_ListarVentasTallas_str(List<Ent_Ventas_Tallas> _ListarVentasTallas, Ent_Ventas_Tallas _Ent)
        {
            StringBuilder sb = new StringBuilder();
            var Lista = _ListarVentasTallas.ToList();
            var tdCabecera = "";
            var tdCantidad = "";
            try
            {
                sb.Append("<div>");
                sb.Append("<table cellspacing='0' style='width: 1000px' rules='all' border='0' style='border-collapse:collapse;'>");
                sb.Append("<tr><td Colspan='13'></td></tr>");
                sb.Append("<tr><td Colspan='13' valign='middle' align='center' style='vertical-align: middle;font-size: 16.0pt;font-weight: bold;color:#285A8F'>REPORTE DE VENTA Y STOCK FINAL POR TALLA</td></tr>");
                sb.Append("<tr><td Colspan='13' valign='middle' align='center' style='vertical-align: middle;font-size: 10.0pt;font-weight: bold;color:#000000'>Desde el " + String.Format("{0:dd/MM/yyyy}", _Ent.FechaInicio) + " hasta el " + String.Format("{0:dd/MM/yyyy}", _Ent.FechaFin) + "</td></tr>");//subtitulo
                sb.Append("</table>");
                sb.Append("<table  border='1' bgColor='#ffffff' borderColor='#FFFFFF' cellSpacing='2' cellPadding='2' style='font-size:10.0pt; font-family:Calibri; background:white;width: 1000px'>");
                sb.Append("<tr bgColor='#1E77AB'>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Articulo</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Pares</font></th>\n");
                sb.Append("<th Colspan='3' style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Talla / Cantidad</font></th>\n");

                sb.Append("</tr>\n");
                // {0:N2} Separacion miles , {0:F2} solo dos decimales
                foreach (var item in Lista)
                {
                    foreach (var itemR in item._ListarDetalle)
                    {
                        tdCabecera += "<td style='width:10%;background-color: #5393C8; border-color: #FFFFFF; font-weight:bold;font-size:11.0pt;' align='Center'><font color='#FFFFFF'>" + itemR.Talla + "</font></td>";
                        tdCantidad += "<td style='width:10%;background-color: #5bc0de; border-color: #FFFFFF; font-weight:bold;font-size:11.0pt;' align='Center'><font color='#FFFFFF'>" + itemR.Pares_Stock + "</font></td>";
                    }
                    //d2d6de
                    sb.Append("<tr>");
                    sb.Append("<td align='Center' style='text-align: center; vertical-align: middle;' rowspan='2'>" + item.Articulo + "</td>");
                    sb.Append("<td align='Center' style='text-align: center; vertical-align: middle;' rowspan='2'>" + item.Pares_Venta + "</td>");
                    sb.Append("<td style='width:10%;background-color: #d2d6de;border-color: #d2d6de; font-weight:bold;font-size:11.0pt;' align='Center'><font color='#000000'>T:</font></td>");
                    sb.Append("<td style='width:10%;background-color: #008448;border-color: #FFFFFF; font-weight:bold;font-size:11.0pt;' align='Center'><font color='#FFFFFF'>Total</font></td>");
                    sb.Append("<td bgcolor='' align='Center'>" +
                              "<table bgColor='#ffffff' borderColor='#FFFFFF' cellSpacing='2' cellPadding='2'>" +
                                "<tr>" +
                                    tdCabecera + 
                                "</tr>" +
                              "</table>" +
                            "</td>\n");
                    sb.Append("</tr>");

                    sb.Append("<tr>");
                    sb.Append("<td style='width:10%;background-color: #d2d6de;border-color: #d2d6de; font-weight:bold;font-size:11.0pt;' align='Center'><font color='#000000'>C:</font></td>");
                    sb.Append("<td style='width:10%;background-color: #00a65a;border-color: #FFFFFF; font-weight:bold;font-size:11.0pt;' align='Center'><font color='#FFFFFF'>" + item.TotalParesStock + "</font></td>");
                    sb.Append("<td bgcolor='' align='Center'>" +
                              "<table>" +
                                "<tr>" +
                                    tdCantidad +
                                "</tr>" +
                              "</table>" +
                            "</td>\n");
                    sb.Append("</tr>");

                    tdCabecera = "";
                    tdCantidad = "";
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
        public ActionResult ListarVentasTallaExcel()
        {
            string NombreArchivo = "ventatalla_stock";
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
                Response.Write(Session[_session_ListarVentasTallas_Excel].ToString());
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