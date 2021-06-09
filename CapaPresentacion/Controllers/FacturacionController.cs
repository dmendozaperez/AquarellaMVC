using CapaEntidad.General;
using CapaEntidad.Util;
using CapaPresentacion.Util;
using CapaEntidad.Control;
using CapaEntidad.Facturacion;
using CapaEntidad.Logistica;
using CapaEntidad.RRHH;
using CapaDato.Logistica;
using CapaDato.Facturacion;
using CapaDato.Util;
using CapaDato.RRHH;
using CapaDato.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Data;
using Newtonsoft.Json;
using System.Reflection;
using System.Web.Script.Serialization;

namespace CapaPresentacion.Controllers
{
    public class FacturacionController : Controller
    {
        private Dat_Facturacion datFacturacion = new Dat_Facturacion();
        private Dat_Despacho dat_despacho = new Dat_Despacho();
        private Dat_Util datUtil = new Dat_Util();
        private Dat_RRHH datRRHH = new Dat_RRHH();
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
        private string _session_ListarSalidaDespacho = "_session_ListarSalidaDespacho";
        private string _session_ListarSalidaDespacho_Excel = "_session_ListarSalidaDespacho_Excel";
        private string _session_ListarSalidaDespacho_Cabecera= "_session_ListarSalidaDespacho_Cabecera";
        private string _session_ListarSalidaDespacho_Detalle = "_session_ListarSalidaDespacho_Detalle";
        private string _session_ListarVentasSemanales = "_session_ListarVentasSemanales";
        private string _session_ListarVentasSemanales_Excel = "_session_ListarVentasSemanales_Excel";
        private string _session_ListarVentasLider = "_session_ListarVentasLider";
        private string _session_ListarVentasLider_Excel = "_session_ListarVentasLider_Excel";
        private string _session_Listar_Servicio = "_session_Listar_Servicio";
        private string _session_ListarConsulta_Premios = "_session_ListarConsulta_Premios";
        private string _session_ListarConsulta_Premios_Excel = "_session_ListarConsulta_Premios_Excel";

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

        #region <SALIDA DE DESPACHO>
        /// <summary>
        /// SALIDA DE DESPACHO
        /// </summary>
        /// <create>Juilliand R. Damian Gomez </create>
        /// <update></update> 
        /// <returns></returns>
        public ActionResult Salida_Almacen()
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

                List<Ent_Salida_Almacen> ListarTipo = new List<Ent_Salida_Almacen>() {
                    new Ent_Salida_Almacen {Codigo = "L",Descripcion = "Lima-Callao" },
                    new Ent_Salida_Almacen {Codigo = "P",Descripcion = "Provincia" }
                };

                ViewBag.ListarTipo = ListarTipo;
                Ent_Salida_Almacen EntSalidaDespacho = new Ent_Salida_Almacen();
                ViewBag.EntSalidaDespacho = EntSalidaDespacho;
                Session[_session_Listar_Servicio] = dat_despacho.Listar_Servicio().ToList();
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
        /// <param name="Tipo"></param>
        /// <returns></returns>
        public JsonResult getSalidaDespachoAjax(Ent_jQueryDataTableParams param, bool isOkUpdate, string FechaInicio, string FechaFin, string Tipo)
        {
            Ent_Salida_Almacen EntSalidaAlmacen = new Ent_Salida_Almacen();
            if (isOkUpdate)
            {
                EntSalidaAlmacen.FechaInicio = DateTime.Parse(FechaInicio);
                EntSalidaAlmacen.FechaFin = DateTime.Parse(FechaFin);
                EntSalidaAlmacen.Tipo = Tipo;

                List<Ent_Salida_Almacen> _ListarSalidaAlmacen = datFacturacion.ListarSalidaDespacho(EntSalidaAlmacen).ToList();
                Session[_session_ListarSalidaDespacho] = _ListarSalidaAlmacen;
            }

            /*verificar si esta null*/
            if (Session[_session_ListarSalidaDespacho] == null)
            {
                List<Ent_Salida_Almacen> _ListarSalidaDespacho = new List<Ent_Salida_Almacen>();
                Session[_session_ListarSalidaDespacho] = _ListarSalidaDespacho;
            }

            IQueryable<Ent_Salida_Almacen> entDocTrans = ((List<Ent_Salida_Almacen>)(Session[_session_ListarSalidaDespacho])).AsQueryable();
            //Manejador de filtros
            int totalCount = entDocTrans.Count();
            IEnumerable<Ent_Salida_Almacen> filteredMembers = entDocTrans;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = entDocTrans.Where(
                        m =>
                            m.IdDespacho.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Desp_Nrodoc.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Desp_Descripcion.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Desp_Tipo_Descripcion.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Desp_Tipo.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.TotalParesEnviado.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Estado.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Desp_FechaCre.ToUpper().Contains(param.sSearch.ToUpper())
                );
            }
            var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);

            if (param.iSortingCols > 0)
            {
                if (Request["sSortDir_0"].ToString() == "asc")
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderBy(o => o.IdDespacho); break;
                        case 1: filteredMembers = filteredMembers.OrderBy(o => o.Desp_Nrodoc); break;
                        case 2: filteredMembers = filteredMembers.OrderBy(o => o.Desp_Descripcion); break;
                        case 3: filteredMembers = filteredMembers.OrderBy(o => o.Desp_Tipo_Descripcion); break;
                        case 4: filteredMembers = filteredMembers.OrderBy(o => o.Desp_Tipo); break;
                        case 5: filteredMembers = filteredMembers.OrderBy(o => o.TotalParesEnviado); break;
                        case 6: filteredMembers = filteredMembers.OrderBy(o => o.Estado); break;
                        case 7: filteredMembers = filteredMembers.OrderBy(o => o.Desp_FechaCre); break;
                    }
                }
                else
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderByDescending(o => o.IdDespacho); break;
                        case 1: filteredMembers = filteredMembers.OrderByDescending(o => o.Desp_Nrodoc); break;
                        case 2: filteredMembers = filteredMembers.OrderByDescending(o => o.Desp_Descripcion); break;
                        case 3: filteredMembers = filteredMembers.OrderByDescending(o => o.Desp_Tipo_Descripcion); break;
                        case 4: filteredMembers = filteredMembers.OrderByDescending(o => o.Desp_Tipo); break;
                        case 5: filteredMembers = filteredMembers.OrderByDescending(o => o.TotalParesEnviado); break;
                        case 6: filteredMembers = filteredMembers.OrderByDescending(o => o.Estado); break;
                        case 7: filteredMembers = filteredMembers.OrderByDescending(o => o.Desp_FechaCre); break;
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

        public ActionResult EditSalida_Almacen(int Id)
        {
            Session[_session_ListarSalidaDespacho_Cabecera] = null;
            Session[_session_ListarSalidaDespacho_Detalle] = null;
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
                Ent_Salida_Almacen _EntData = new Ent_Salida_Almacen();
                _EntData.IdDespacho = Id;
                _EntData = datFacturacion.ListarDespacho(_EntData);
                //Cabecera
                Session[_session_ListarSalidaDespacho_Cabecera] = _EntData._Cabecera;
                //Detalle
                Session[_session_ListarSalidaDespacho_Detalle] = _EntData._Detalle;

                ViewBag._Ent = _EntData;
                ViewBag._Detalle = _EntData._Detalle;
                ViewBag.Servicio = dat_despacho.Listar_Servicio();
                Session[_session_Listar_Servicio] = dat_despacho.Listar_Servicio().ToList();
                return View(_EntData._Cabecera);
            }
        }
        public JsonResult getSalidaDespachoEditAjax(Ent_jQueryDataTableParams param)
        {

            /*verificar si esta null*/
            if (Session[_session_ListarSalidaDespacho_Detalle] == null)
            {
                List<Ent_Edit_Salida_Almacen_Detalle> _ListarSalidaDespacho = new List<Ent_Edit_Salida_Almacen_Detalle>();
                Session[_session_ListarSalidaDespacho_Detalle] = _ListarSalidaDespacho;
            }

            IQueryable<Ent_Edit_Salida_Almacen_Detalle> entDocTrans = ((List<Ent_Edit_Salida_Almacen_Detalle>)(Session[_session_ListarSalidaDespacho_Detalle])).AsQueryable();
            //Manejador de filtros
            int totalCount = entDocTrans.Count();
            IEnumerable<Ent_Edit_Salida_Almacen_Detalle> filteredMembers = entDocTrans;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = entDocTrans.Where(
                        m =>
                            m.Asesor.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.NombreLider.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Promotor.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Rotulo.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.TotalCatalogo.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.TotalCatalogEnviadoEdit.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.TotalPremio.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.TotalPremioEnviadoEdit.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.TotalPares.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.TotalParesEnviadoEdit.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Agencia.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Destino.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Pedido.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.TotalVenta.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.CobroFlete.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Observacion.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Detalle.ToUpper().Contains(param.sSearch.ToUpper())
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
                        case 1: filteredMembers = filteredMembers.OrderBy(o => o.NombreLider); break;
                        case 2: filteredMembers = filteredMembers.OrderBy(o => o.Promotor); break;
                        case 3: filteredMembers = filteredMembers.OrderBy(o => o.Rotulo); break;
                        case 4: filteredMembers = filteredMembers.OrderBy(o => o.TotalCatalogo); break;
                        case 5: filteredMembers = filteredMembers.OrderBy(o => o.TotalCatalogEnviadoEdit); break;
                        case 6: filteredMembers = filteredMembers.OrderBy(o => o.TotalPremio); break;
                        case 7: filteredMembers = filteredMembers.OrderBy(o => o.TotalPremioEnviadoEdit); break;
                        case 8: filteredMembers = filteredMembers.OrderBy(o => o.TotalPares); break;
                        case 9: filteredMembers = filteredMembers.OrderBy(o => o.TotalParesEnviadoEdit); break;
                        case 10: filteredMembers = filteredMembers.OrderBy(o => o.Agencia); break;
                        case 11: filteredMembers = filteredMembers.OrderBy(o => o.Destino); break;
                        case 12: filteredMembers = filteredMembers.OrderBy(o => o.Pedido); break;
                        case 13: filteredMembers = filteredMembers.OrderBy(o => o.TotalVenta); break;
                        case 14: filteredMembers = filteredMembers.OrderBy(o => o.CobroFlete); break;
                        case 15: filteredMembers = filteredMembers.OrderBy(o => o.Observacion); break;
                        case 16: filteredMembers = filteredMembers.OrderBy(o => o.Detalle); break;
                    }
                }
                else
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderByDescending(o => o.Asesor); break;
                        case 1: filteredMembers = filteredMembers.OrderByDescending(o => o.NombreLider); break;
                        case 2: filteredMembers = filteredMembers.OrderByDescending(o => o.Promotor); break;
                        case 3: filteredMembers = filteredMembers.OrderByDescending(o => o.Rotulo); break;
                        case 4: filteredMembers = filteredMembers.OrderByDescending(o => o.TotalCatalogo); break;
                        case 5: filteredMembers = filteredMembers.OrderByDescending(o => o.TotalCatalogEnviadoEdit); break;
                        case 6: filteredMembers = filteredMembers.OrderByDescending(o => o.TotalPremio); break;
                        case 7: filteredMembers = filteredMembers.OrderByDescending(o => o.TotalPremioEnviadoEdit); break;
                        case 8: filteredMembers = filteredMembers.OrderByDescending(o => o.TotalPares); break;
                        case 9: filteredMembers = filteredMembers.OrderByDescending(o => o.TotalParesEnviadoEdit); break;
                        case 10: filteredMembers = filteredMembers.OrderByDescending(o => o.Agencia); break;
                        case 11: filteredMembers = filteredMembers.OrderByDescending(o => o.Destino); break;
                        case 12: filteredMembers = filteredMembers.OrderByDescending(o => o.Pedido); break;
                        case 13: filteredMembers = filteredMembers.OrderByDescending(o => o.TotalVenta); break;
                        case 14: filteredMembers = filteredMembers.OrderByDescending(o => o.CobroFlete); break;
                        case 15: filteredMembers = filteredMembers.OrderByDescending(o => o.Observacion); break;
                        case 16: filteredMembers = filteredMembers.OrderByDescending(o => o.Detalle); break;
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
        /// Genera/Registrar los saldos anticipos
        /// </summary>
        /// <param name="_LisSaldo"></param>
        /// <returns></returns>
        public ActionResult getRegistrarDespacho(List<Ent_Edit_Salida_Almacen_Detalle> _Listado, Ent_Edit_Salida_Almacen_Cabecera _Ent)
        {
            JsonResponse objResult = new JsonResponse();

            Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
            bool Result = false;

            try
            {
                string strDataDetalle = "";
                _Ent.Atendido = "N";
                _Ent.Estado = "R";

                if (_Ent.chkAtender)
                    _Ent.Atendido = "S";

                if (_Ent.chkEstSalida)
                    _Ent.Estado = "S";

                foreach (var item in _Listado)
                {
                    strDataDetalle += "<row  ";
                    strDataDetalle += " IdDetalle=¿" + item.Desp_IdDetalle.ToString() + "¿ ";
                    strDataDetalle += " ParesSalida=¿" + item.TotalParesEnviadoEdit.ToString() + "¿ ";
                    strDataDetalle += " CatalogSalida=¿" + item.TotalCatalogEnviadoEdit.ToString() + "¿ ";
                    strDataDetalle += " PremioSalida=¿" + item.TotalPremioEnviadoEdit.ToString() + "¿ ";
                    strDataDetalle += "/>";
                }

                _Ent.strDataDetalle = strDataDetalle;
                _Ent.UsuarioCrea = 1; //_usuario.usu_id;
                
                Result = datFacturacion.ActualizarSalidaDespacho(_Ent);
                if (Result)
                {
                    objResult.IdPrincipal = Convert.ToInt32(_Ent.Desp_id);
                    objResult.Success = true;
                    objResult.Message = "Se actualizo correctamente el despacho.";
                }
                else
                {
                    objResult.Success = false;
                    objResult.Message = "¡Error! Al actualizar el despacho.";
                }
            }
            catch (Exception ex)
            {
                objResult.Success = false;
                objResult.Message = "¡Error! Al actualizar el despacho.";
            }
            var JSON = JsonConvert.SerializeObject(objResult);
            return Json(JSON, JsonRequestBehavior.AllowGet);
        }

        public ActionResult get_exporta_ListarSalidaDespacho_excel(int Id, bool isOkPrincipal)
        {
            JsonResponse objResult = new JsonResponse();
            try
            {
                Session[_session_ListarSalidaDespacho_Excel] = null;
                string cadena = "";

                if (isOkPrincipal)
                {
                    Ent_Salida_Almacen _EntData = new Ent_Salida_Almacen();
                    _EntData.IdDespacho = Id;
                    _EntData = datFacturacion.ListarDespacho(_EntData);
                    cadena = get_html_ListarSalidaDespacho_str(_EntData._Detalle, _EntData._Cabecera);
                    if (cadena.Length == 0)
                    {
                        objResult.Success = false;
                        objResult.Message = "Error del formato html";
                    }
                    else
                    {
                        objResult.Success = true;
                        objResult.Message = "Se genero el excel correctamente";
                        Session[_session_ListarSalidaDespacho_Excel] = cadena;
                    }
                }
                else
                {
                    if (Session[_session_ListarSalidaDespacho_Cabecera] != null && Session[_session_ListarSalidaDespacho_Detalle] != null)
                    {

                        Ent_Edit_Salida_Almacen_Cabecera _ListarSalidaDespacho = (Ent_Edit_Salida_Almacen_Cabecera)Session[_session_ListarSalidaDespacho_Cabecera];
                        if (_ListarSalidaDespacho == null)
                        {
                            objResult.Success = false;
                            objResult.Message = "No hay filas para exportar";
                        }
                        else
                        {
                            cadena = get_html_ListarSalidaDespacho_str((List<Ent_Edit_Salida_Almacen_Detalle>)Session[_session_ListarSalidaDespacho_Detalle], (Ent_Edit_Salida_Almacen_Cabecera)Session[_session_ListarSalidaDespacho_Cabecera]);
                            if (cadena.Length == 0)
                            {
                                objResult.Success = false;
                                objResult.Message = "Error del formato html";
                            }
                            else
                            {
                                objResult.Success = true;
                                objResult.Message = "Se genero el excel correctamente";
                                Session[_session_ListarSalidaDespacho_Excel] = cadena;
                            }
                        }
                    }
                    else
                    {
                        objResult.Success = false;
                        objResult.Message = "No hay filas para exportar";
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


        public string get_html_ListarSalidaDespacho_str(List<Ent_Edit_Salida_Almacen_Detalle> _SalidaDespachoDetalle, Ent_Edit_Salida_Almacen_Cabecera _Cabecera)
        {
            StringBuilder sb = new StringBuilder();
            List<Ent_Despacho_Delivery> _ListarServico = (List<Ent_Despacho_Delivery>)Session[_session_Listar_Servicio];
            var _Detalle = _SalidaDespachoDetalle.ToList();
            try
            {
                sb.Append("<div>");
                sb.Append("<table cellspacing='0' style='width: 1000px' rules='all' border='0' style='border-collapse:collapse;'>");
                sb.Append("<tr><td Colspan='14'></td></tr>");
                //sb.Append("<tr><td Colspan='14' valign='middle' align='center' style='vertical-align: middle;font-size: 18.0pt;font-weight: bold;color:#285A8F'>REPORTE DE VENTAS POR SEMANA</td></tr>");
                //sb.Append("<tr><td Colspan='14' valign='middle' align='center' style='vertical-align: middle;font-size: 10.0pt;font-weight: bold;color:#000000'>Ventas de semana con el año anterior al " + _Ent.Anno+ "</td></tr>");//subtitulo
                sb.Append("</table>");
                sb.Append("<Table border='1' bgColor='#ffffff' borderColor='#000000' cellSpacing='2' cellPadding='2' style='font-size:10.0pt; font-family:Calibri; background:white;'>");
                sb.Append("<tr height=38 ><td height=38  bgcolor='#969696' width='38'>Tipo Despacho </ td ><td width='400' align='left' >" + _Cabecera.Desp_Tipo_Des + "</ td > ");
                sb.Append("<tr height=38 ><td height=38  bgcolor='#969696' width='38'>Nro. Documento </ td ><td width='400' align='left' >" + _Cabecera.Desp_NroDoc + "</ td > ");
                sb.Append("<td height=38  bgcolor='#969696' width='38'>Fec. Creación. </ td ><td width='400' align='left' colspan='2' >" + _Cabecera.Desp_FechaCre + "</ td > </tr>");
                sb.Append("<tr height=38 ><td height=38  bgcolor='#969696' width='38'>Total Monto. </ td ><td width='400' align='left' >" + _Cabecera.MontoTotal + "</ td > ");
                sb.Append("<td height=38  bgcolor='#969696' width='38'>Estado </ td ><td width='400' align='left' colspan='2' >" + _Cabecera.Estado + "</ td ></tr>");
                sb.Append("<tr height=38 ><td height=38  bgcolor='#969696' width='38'>Pares Pedido </ td ><td width='400' align='left' >" +_Cabecera.NroPedidos + "</ td > ");
                sb.Append("<td height=38  bgcolor='#969696' width='38'>Pares Enviado </ td ><td width='400' align='left' colspan='2' >" + _Cabecera.NroEnviados + "</ td ></tr>");
                sb.Append("<tr height=38 ><td height=38  bgcolor='#969696' width='38'>Catalogo Facturado </ td ><td width='400' align='left' >" + _Cabecera.CatalogPedidos + "</ td > ");
                sb.Append("<td height=38  bgcolor='#969696' width='38'>Catalogo Enviado </ td ><td width='400' align='left' colspan='2' >" + _Cabecera.CatalogEnviados+ "</ td ></tr>");
                sb.Append("<tr height=38 ><td height=38  bgcolor='#969696' width='38'>Premio Pedido </ td ><td width='400' align='left' >" + _Cabecera.NroPremio+ "</ td > ");
                sb.Append("<td height=38  bgcolor='#969696' width='38'>Premio Enviado </ td ><td width='400' align='left' colspan='2' >" + _Cabecera.PremioEnviados+ "</ td ></tr>");
                sb.Append("<tr height=38 ><td height=38  bgcolor='#969696' width='38'>Descripción </ td ><td colspan='4' align='left' >" + _Cabecera.Desp_Descripcion + "</ td > ");
                sb.Append("</tr>");
                sb.Append("</table>");

                sb.Append("<table>");
                sb.Append("<tr></tr>");
                sb.Append("</table>");

                sb.Append("</table>");
                sb.Append("<Table border='1' bgColor='#ffffff' borderColor='#000000' cellSpacing='2' cellPadding='2' style='font-size:10.0pt; font-family:Calibri; background:white;'>");
                sb.Append("<tr bgColor='#969696'>\n");
                sb.Append("<th style='text-align: center;'><font color=''>Asesor</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color=''>Lider</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color=''>Promotor</font></th>\n");
                if (_Cabecera.Desp_Tipo == "L")
                {
                    sb.Append("<th style='text-align: center; '><font color=''>Dni Promotor</font></th>\n");
                }
                sb.Append("<th style='text-align: center;'><font color=''>Rotulo</font></th>\n");
                if (_Cabecera.Desp_Tipo == "P")
                {
                    sb.Append("<th style='text-align: center; '><font color=''>Agencia</font></th>\n");
                    sb.Append("<th style='text-align: center; '><font color=''>Destino</font></th>\n");
                }
                sb.Append("<th style='text-align: center;'><font color=''>Pedido</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color=''>TotalPares</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color=''>Flete</font></th>\n");
                sb.Append("<th style='text-align: center;'><font color=''>Observacion</font></th>\n");
                if (_Cabecera.Desp_Tipo == "L")
                {
                    sb.Append("<th style='text-align: center;'><font color=''>Delivery</font></th>\n");
                }
                sb.Append("</tr>\n");

                foreach (var item in _Detalle)
                {
                    sb.Append("<tr>\n");
                    sb.Append("<td align=''>" + item.Asesor+ "</td>\n");
                    sb.Append("<td align=''>" + item.NombreLider + "</td>\n");
                    sb.Append("<td align=''>" + item.Promotor + "</td>\n");
                    if (_Cabecera.Desp_Tipo == "L")
                    {
                        sb.Append("<td align='' class='xlxTexto'>" + item.Dni_Promotor + "</td>\n");
                    }
                    sb.Append("<td align=''>" + item.Rotulo + "</td>\n");
                   
                    if (_Cabecera.Desp_Tipo == "P")
                    {
                        sb.Append("<td align=''>" + item.Agencia + "</td>\n");
                        sb.Append("<td align=''>" + item.Destino + "</td>\n");                        
                    }
                    sb.Append("<td align=''>" + item.Pedido + "</td>\n");
                    sb.Append("<td align=''>" + item.TotalPares + "</td>\n");
                    sb.Append("<td align=''>" + item.CobroFlete + "</td>\n");
                    sb.Append("<td align=''>" + item.Observacion + "</td>\n");
                    if (_Cabecera.Desp_Tipo == "L")
                    {
                        sb.Append("<td align=''>" + (item.Delivery == "" ? "" : _ListarServico.Where(x => x.Codigo == item.Delivery).Select(y => new { Descripcion = y.Descripcion }).ElementAt(0).Descripcion) + "</td>\n");
                    }
                    sb.Append("</tr>\n");
                }
                sb.Append("</table></div>");
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return sb.ToString();
        }

        public ActionResult ListarSalidaDespachoExcel()
        {
            string NombreArchivo = "Orden_Despacho";
            String style = style = @"<style> .textmode { mso-number-format:\@; } 
                                .xlxTexto
	                            {padding-top:1px;
	                            padding-right:1px;
	                            padding-left:1px;
	                            mso-ignore:padding;
	                            color:black;
	                            font-size:10.0pt;
	                            font-weight:400;
	                            font-style:normal;
	                            text-decoration:none;
	                            font-family:Calibri, sans-serif;
	                            mso-font-charset:0;
	                            mso-number-format:\@;
	                            text-align:general;
	                            vertical-align:bottom;
	                            border:.5pt solid black;
	                            background:white;
	                            mso-pattern:black none;
	                            white-space:normal;}
                            </style> ";
            try
            {
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", "attachment;filename=" + NombreArchivo + ".xls");
                Response.Charset = "UTF-8";
                Response.ContentEncoding = Encoding.Default;
                Response.Write(style);
                Response.Write(Session[_session_ListarSalidaDespacho_Excel].ToString());
                Response.End();
            }
            catch
            {

            }
            return Json(new { estado = 0, mensaje = 1 });
        }
        #endregion

        #region <VENTAS SEMANALES>
        /// <summary>
        /// Ventas semanales
        /// </summary>
        /// <returns></returns>
        public ActionResult Ventas_Semanales()
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
                Ent_Ventas_Semanales EntVentasSemanales = new Ent_Ventas_Semanales();
                ViewBag.EntVentasSemanales = EntVentasSemanales;
                return View();
            }
        }
        /// <summary>
        /// Listado principal
        /// </summary>
        /// <param name="_Ent"></param>
        /// <returns></returns>
        public JsonResult getVentasSemanalesAjax(Ent_Ventas_Semanales _Ent)
        {
            Session[_session_ListarVentasSemanales] = null;

            JsonResponse objResult = new JsonResponse();
            //_Ent.FechaInicio = DateTime.Parse("01/10/2020");
            //_Ent.FechaFin = DateTime.Parse("19/02/2021");
            int Fila = -1;
            DataTable dtInicio = datFacturacion.ListarVentaSemanal(_Ent);

            if (dtInicio.Rows.Count >0){
                DataTable dtFin = new DataTable();

                string[] Dias = dtInicio.AsEnumerable().Select(x => x.Field<string>("dia")+"/"+ x.Field<string>("Mes")+"/"+ x.Field<int>("Anio")).ToArray();

                dtFin.Columns.Add("AQ", typeof(string));

                for (int i = 0; i < 3; i++){
                    dtFin.Rows.Add();
                    Fila++;
                }

                foreach (string dia in Dias){
                    dtFin.Columns.Add(dia, typeof(string));
                }

                var Anio = dtInicio.AsEnumerable()
                            .GroupBy(x => x.Field<int>("Anio"))
                            .Select(x => new {
                                Anio = x.Key,
                                Cant = x.Select(y => y.Field<string>("mes")).Count()
                            });

                var Mes = dtInicio.AsEnumerable()
                            .GroupBy(x => new {
                                Anio = x.Field<int>("Anio"),
                                Mes = x.Field<string>("mes")
                            })
                            .Select(x => new {
                                Anio = x.Key.Anio,
                                Mes = x.Key.Mes,
                                Cant = x.Select(y => y.Field<string>("dia")).Count()
                            });

                var StrAnio = "";
                foreach (var itemA in Anio){
                    StrAnio += itemA.Anio + "/" + itemA.Cant + "|";
                }
                StrAnio = StrAnio.Remove(StrAnio.Trim().Length - 1);
                var StrMes = "";
                foreach (var itemM in Mes){
                    StrMes += itemM.Anio + "/" + itemM.Mes + "/" + itemM.Cant + "|";
                }
                StrMes = StrMes.Remove(StrMes.Trim().Length - 1);
                var groups = dtInicio.AsEnumerable().GroupBy(x => x.Field<string>("AQ"));

                foreach (var group in groups){
                    Fila++;
                    dtFin.Rows.Add();
                    dtFin.Rows[Fila - 3]["AQ"] = StrAnio;
                    dtFin.Rows[Fila - 2]["AQ"] = StrMes;
                    dtFin.Rows[Fila - 1]["AQ"] = "AQ";
                    dtFin.Rows[Fila]["AQ"] = group.Key;
                    foreach (var row in group)
                    {
                        var BtF = dtInicio.AsEnumerable().Where(r => r.Field<string>("AQ") == group.Key && r.Field<string>("dia") == row.Field<string>("dia") && r.Field<string>("mes") == row.Field<string>("Mes") && r.Field<int>("anio") == row.Field<int>("anio")).Select(r => new { Total = r.Field<Decimal>("total") }).ToList();
                        if (BtF.Count > 0)
                        {
                            if (Fila == 3)
                            {
                                dtFin.Rows[Fila - 1][row.Field<string>("dia") + "/" + row.Field<string>("Mes") + "/" + row.Field<int>("Anio")] = row.Field<string>("dia");
                            }
                            dtFin.Rows[Fila][row.Field<string>("dia") + "/" + row.Field<string>("Mes") + "/" + row.Field<int>("Anio")] = BtF.ElementAt(0).Total;
                        }
                        else
                        {
                            dtFin.Rows[Fila][row.Field<string>("dia") + "/" + row.Field<string>("Mes") + "/" + row.Field<int>("Anio")] = "";
                        }
                    }
                }
                //Convierte el a lista de objeto
                List<string[]> ListTabla = new List<string[]>();
                foreach (DataRow dr in dtFin.Rows)
                {
                    int columnCount = 0;
                    string[] myTableRow = new string[dtFin.Columns.Count];
                    foreach (DataColumn dc in dtFin.Columns)
                    {
                        myTableRow[columnCount] = dr[dc.ColumnName].ToString();
                        columnCount++;
                    }
                    ListTabla.Add(myTableRow);
                }

                var arrListTabla = ListTabla.ToArray();

                objResult.Data = arrListTabla;
                objResult.Success = true;
                
                Session[_session_ListarVentasSemanales] = ListTabla;
            }
            else
            {
                objResult.Success = false;
            }
            var JSON = JsonConvert.SerializeObject(objResult);
            return Json(JSON, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Exportar excel
        /// </summary>
        /// <param name="_Ent"></param>
        /// <returns></returns>
        public ActionResult get_exporta_ListarVentaSemanal_excel(Ent_Ventas_Semanales _Ent)
        {
            JsonResponse objResult = new JsonResponse();
            try
            {
                Session[_session_ListarVentasSemanales_Excel] = null;
                string cadena = "";
                if (Session[_session_ListarVentasSemanales] != null)
                {

                    List<string[]> _ListarVentasSemanales = (List<string[]>)Session[_session_ListarVentasSemanales];
                    if (_ListarVentasSemanales.Count == 0)
                    {
                        objResult.Success = false;
                        objResult.Message = "No hay filas para exportar";
                    }
                    else
                    {
                        cadena = get_html_ListarVentasSemanal_str((List<string[]>)Session[_session_ListarVentasSemanales], _Ent);
                        if (cadena.Length == 0)
                        {
                            objResult.Success = false;
                            objResult.Message = "Error del formato html";
                        }
                        else
                        {
                            objResult.Success = true;
                            objResult.Message = "Se genero el excel correctamente";
                            Session[_session_ListarVentasSemanales_Excel] = cadena;
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
        /// Armando el excel
        /// </summary>
        /// <param name="ListarVentasSemanal"></param>
        /// <param name="_Ent"></param>
        /// <returns></returns>
        public string get_html_ListarVentasSemanal_str(List<string[]> ListarVentasSemanal, Ent_Ventas_Semanales _Ent)
        {
            StringBuilder sb = new StringBuilder();
            var Lista = ListarVentasSemanal.ToArray(); 
            try
            {
                string[] StrAnio = Lista[0][0].Split('|');
                string result = "<tr bgColor='#1E77AB'><th></th>";
                foreach (var item in StrAnio)
                {
                    result += "<th colspan = " + item.Substring((item.IndexOf("/") + 1)) + " style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>" + item.Substring(0, item.IndexOf("/")) + "</font></th>";
                }
                result += "</tr>";

                string[] StrMes = Lista[1][0].Split('|');
                result += "<tr bgColor='#1E77AB'><th></th>";
                foreach (var item in StrMes)
                {
                    result += "<th colspan = " + item.Substring((item.LastIndexOf("/") + 1)) + " style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>" + item.Substring((item.IndexOf("/") + 1), (item.LastIndexOf("/") - (item.IndexOf("/") + 1))) + "</font></th>";
                }
                result += "</tr>";

                for (var i = 2; i < 3; i++)
                {
                    result += "<tr bgColor='#1E77AB'>";
                    for (var j = 0; j < Lista[i].Length; j++)
                    {
                        result += "<th  style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>" + Lista[i][j] + "</font></th>";
                    }
                    result += "</tr>";
                }

                for (var i = 3; i < Lista.Length; i++)
                {
                    result += "<tr>";
                    for (var j = 0; j < Lista[i].Length; j++)
                    {
                        result += "<td >" + Lista[i][j] + "</td>";
                    }
                    result += "</tr>";
                }

                sb.Append("<div>");
                sb.Append("<table cellspacing='0' style='width: 1000px' rules='all' border='0' style='border-collapse:collapse;'>");
                sb.Append("<tr><td Colspan='14'></td></tr>");
                sb.Append("<tr><td Colspan='14' valign='middle' align='center' style='vertical-align: middle;font-size: 18.0pt;font-weight: bold;color:#285A8F'>REPORTE DE VENTAS POR SEMANA</td></tr>");
                sb.Append("<tr><td Colspan='13' valign='middle' align='center' style='vertical-align: middle;font-size: 10.0pt;font-weight: bold;color:#000000'>Desde el " + String.Format("{0:dd/MM/yyyy}", _Ent.FechaInicio) + " hasta el " + String.Format("{0:dd/MM/yyyy}", _Ent.FechaFin) + "</td></tr>");//subtitulo
                sb.Append("</table>");
                sb.Append("<table  border='1' bgColor='#ffffff' borderColor='#FFFFFF' cellSpacing='2' cellPadding='2' style='font-size:10.0pt; font-family:Calibri; background:white;width: 1000px'><tr  bgColor='#5799bf'>\n");

                sb.Append(result);

                sb.Append("<tfoot>\n");
                sb.Append("<tr bgcolor='#085B8C'>\n");
                sb.Append("</tr>\n");
                sb.Append("</tfoot>\n");
                sb.Append("</table></div>");
            }
            catch(Exception ex)
            {

            }
            return sb.ToString();
        }
        /// <summary>
        /// Exportar excel
        /// </summary>
        /// <returns></returns>
        public ActionResult ListarVentasSemanalExcel()
        {
            string NombreArchivo = "Ventas_Semanales";
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
                Response.Write(Session[_session_ListarVentasSemanales_Excel].ToString());
                Response.End();
            }
            catch
            {

            }
            return Json(new { estado = 0, mensaje = 1 });
        }
        #endregion

        #region <VENTAS POR LIDER>
        /// <summary>
        /// Ventas semanales
        /// </summary>
        /// <create>Juilliand R. Damian Gomez </create>
        /// <update></update> 
        /// <returns></returns>
        public ActionResult Ventas_Lider()
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
                List<Ent_Combo> ListarCLiente = new List<Ent_Combo>();
                ListarCLiente.Add(new Ent_Combo() { codigo = "-1", descripcion = "-- Selecionar Todos--" });

                int Cant = datUtil.Listar_Clientes(_usuario).Count();

                ViewBag.ListarCLiente = (Cant==1 ? datUtil.Listar_Clientes(_usuario) : ListarCLiente.Concat(datUtil.Listar_Clientes(_usuario)));

                Ent_Ventas_Lider EntVentasLider = new Ent_Ventas_Lider();
                ViewBag.EntVentasLider = EntVentasLider;
                return View();
            }
        }
        /// <summary>
        /// Listado principal
        /// </summary>
        /// <create>Juilliand R. Damian Gomez </create>
        /// <update></update> 
        /// <param name="_Ent"></param>
        /// <returns></returns>
        private string _ListAno = "_listAno";
        private string _ListMes = "_listMes";
        private string _ListSem = "_listSem";
        private string _ListContenido = "_ListContenido";
        private string _ListNombre = "_ListNombre";
        public JsonResult getVentasLiderCol(Ent_Ventas_Lider _Ent)
        {
            Session[_session_ListarVentasLider] = null;
            Session[_ListAno] = null;
            Session[_ListMes] = null;
            Session[_ListSem] = null;
            Session[_ListNombre] = null;            
            Session[_ListContenido] = null;
            Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
            JsonResponse objResult = new JsonResponse();
            //_Ent.Bas_Id ="-1" ;
            //_Ent.FechaInicio = DateTime.Parse("01/01/2021");
            //_Ent.FechaFin = DateTime.Parse("04/03/2021");
            _Ent.Asesor = _usuario.usu_asesor;
            DataTable dtInicio = datFacturacion.ListarVentaLider(_Ent);

            if (dtInicio.Rows.Count > 0)
            {
                List<Ent_Ventas_Lider> ListaMes = new List<Ent_Ventas_Lider>()
                {
                    new Ent_Ventas_Lider {IdMes= 1,Mes="Enero" },
                    new Ent_Ventas_Lider {IdMes= 2,Mes="Febrero" },
                    new Ent_Ventas_Lider {IdMes= 3,Mes="Marzo" },
                    new Ent_Ventas_Lider {IdMes= 4,Mes="Abril" },
                    new Ent_Ventas_Lider {IdMes= 5,Mes="Mayo" },
                    new Ent_Ventas_Lider {IdMes= 6,Mes="Junio" },
                    new Ent_Ventas_Lider {IdMes= 7,Mes="Julio" },
                    new Ent_Ventas_Lider {IdMes= 8,Mes="Agosto" },
                    new Ent_Ventas_Lider {IdMes= 9,Mes="Septiembre" },
                    new Ent_Ventas_Lider {IdMes= 10,Mes="Octubre" },
                    new Ent_Ventas_Lider {IdMes= 11,Mes="Noviembre" },
                    new Ent_Ventas_Lider {IdMes= 12,Mes="Diciembre" },
                };

                Pivot pvt = new Pivot(dtInicio);
                string[] filagru = { "Asesor", "Lider", "Cliente", "Dni", "Celular", "Correo", "Zona", "Actividad" };
                string[] col = { "Ano", "Mes", "Semana" };
                
                DataTable dtPivot = pvt.PivotData("Venta Total", AggregateFunction.Sum, filagru, col, ListaMes);

                var grpAnio = dtInicio.AsEnumerable()
                             .GroupBy(x => new {
                                 Anio = x.Field<int>("Ano")
                             })
                             .Select(x => new {
                                 Anio = x.Key.Anio,
                                 Mes = x.GroupBy(y => new {
                                     Mes = y.Field<string>("Mes"),
                                     IdMes = ListaMes.Where(t => t.Mes == y.Field<string>("Mes")).Select(m => new { IdMes = m.IdMes }).ToList().First().IdMes
                                 }).Select(y => new {
                                     Mes = y.Key.Mes,
                                     IdMes = y.Key.IdMes,
                                     Semana = y.GroupBy(gr => new {
                                         Semana = gr.Field<string>("Semana")
                                     }).Select(gr => new { Semana = gr.Key.Semana }).OrderBy(o => o.Semana).ToList()
                                 }).OrderBy(o => o.IdMes).ToList()
                             })
                             .OrderBy(o => o.Anio)
                             .ToList();

                var Mes = dtInicio.AsEnumerable()
                            .GroupBy(x => new {
                                Anio = x.Field<int>("Ano"),
                                Mes = x.Field<string>("Mes"),
                                IdMes = ListaMes.Where(t => t.Mes == x.Field<string>("Mes")).Select(m => new { IdMes = m.IdMes }).ToList().First().IdMes,
                            })
                            .Select(x => new {
                                Anio = x.Key.Anio,
                                Mes = x.Key.Mes,
                                IdMes = x.Key.IdMes,
                                Cant = (x.GroupBy(y => y.Field<string>("Semana")).Count() + 2),
                            })
                            .OrderBy(o => o.Anio)
                            .ThenBy(o => o.IdMes)
                            .ToList();

                var Anio = Mes.GroupBy(x => new { Anio = x.Anio }).Select(s => new { Anio = s.Key.Anio, Cant = s.Sum(x => x.Cant)}).ToList();


                Ent_Ventas_Lider Ent = new Ent_Ventas_Lider();
                List<string> listSem = new List<string>() {
                    "Asesor","Directora","Promotora","DNI","Celular","Correo","Zona","Actividad"
                };

                List<Ent_Ventas_Lider_Col> _ListHead = new List<Ent_Ventas_Lider_Col>(){
                    new Ent_Ventas_Lider_Col {sName= "Asesor", mData="Asesor" ,sClass="Asesor",fName = "Asesor",cssColor = "#1E77AB"},
                    new Ent_Ventas_Lider_Col {sName= "Lider", mData="Lider" ,sClass="Lider",fName = "Directora",cssColor = "#1E77AB"},
                    new Ent_Ventas_Lider_Col {sName= "Cliente", mData="Cliente" ,sClass="Cliente",fName = "Promotora",cssColor = "#1E77AB"},
                    new Ent_Ventas_Lider_Col {sName= "Dni", mData="Dni" ,sClass="Dni",fName = "Dni",cssColor = "#1E77AB"},
                    new Ent_Ventas_Lider_Col {sName= "Celular", mData="Celular" ,sClass="",fName = "Celular",cssColor = "#1E77AB"},
                    new Ent_Ventas_Lider_Col {sName= "Correo", mData="Correo" ,sClass="Correo",fName = "Correo",cssColor = "#1E77AB"},
                    new Ent_Ventas_Lider_Col {sName= "Zona", mData="Zona" ,sClass="Zona",fName = "Zona",cssColor = "#1E77AB"},
                    new Ent_Ventas_Lider_Col {sName= "Actividad", mData="Actividad" ,sClass="Actividad",fName = "Actividad",cssColor = "#1E77AB"},
                    };

                Ent_Ventas_Lider_Col _EntVL = null;
                List<string> listAno = new List<string>();


                foreach (var item in grpAnio)
                {
                    foreach (var itemM in item.Mes)
                    {
                        foreach (var itemS in itemM.Semana)
                        {
                            _EntVL = new Ent_Ventas_Lider_Col();
                            _EntVL.sName = item.Anio + "/" + itemM.Mes + "/" + itemS.Semana;
                            _EntVL.mData = item.Anio + "/" + itemM.Mes + "/" + itemS.Semana;
                            _EntVL.cssColor = "#A7C5EB";
                            _EntVL.fName = itemS.Semana;
                            _ListHead.Add(_EntVL);
                            listSem.Add(itemS.Semana);
                        }
                        _EntVL = new Ent_Ventas_Lider_Col();
                        _EntVL.sName = item.Anio + "/" + itemM.Mes + "/Venta Total";
                        _EntVL.mData = item.Anio + "/" + itemM.Mes + "/Venta Total";
                        _EntVL.cssColor = "#709EB0";
                        _EntVL.sClass = "Venta_Total";
                        _EntVL.fName = "Venta <br> Neta <br>" + itemM.Mes;
                        _ListHead.Add(_EntVL);
                        _EntVL = new Ent_Ventas_Lider_Col();
                        _EntVL.sName = item.Anio + "/" + itemM.Mes + "/Venta Neta";
                        _EntVL.mData = item.Anio + "/" + itemM.Mes + "/Venta Neta";
                        _EntVL.cssColor = "#1E77AB";
                        _EntVL.sClass = "Venta_Neta";
                        _EntVL.fName = "Venta <br> Bruta <br>" + itemM.Mes;
                        _ListHead.Add(_EntVL);
                        listSem.Add("Venta <br> Neta <br>" + itemM.Mes);
                        listSem.Add("Venta <br> Bruta <br>" + itemM.Mes);
                    }
                    _EntVL = new Ent_Ventas_Lider_Col();
                    _EntVL.sName = item.Anio + "/Venta Total/";
                    _EntVL.mData = item.Anio + "/Venta Total/";
                    _EntVL.cssColor = "#709EB0";
                    _EntVL.sClass = "Total";
                    _EntVL.fName = "NETO";
                    _ListHead.Add(_EntVL);
                    _EntVL = new Ent_Ventas_Lider_Col();
                    _EntVL.sName = item.Anio + "/Venta Neta/";
                    _EntVL.mData = item.Anio + "/Venta Neta/";
                    _EntVL.cssColor = "#1E77AB";
                    _EntVL.sClass = "Total";
                    _EntVL.fName = "BRUTO";
                    _ListHead.Add(_EntVL);
                    listSem.Add("NETO");
                    listSem.Add("BRUTO");
                }


                foreach (var item in Anio)
                {
                    listAno.Add(item.Anio + "/"+ (item.Cant + 2));
                }

                List<string> listMes = new List<string>();

                foreach (var item in grpAnio)
                {
                    foreach (var itemM in item.Mes)
                    {
                        listMes.Add(item.Anio + "/" + itemM.Mes + "/" + (itemM.Semana.Count()+2));
                    }
                    listMes.Add(item.Anio + "/Total/" + "1");
                    listMes.Add(item.Anio + "/Total/" + "1");
                }

                List<string[]> ListTabla = new List<string[]>();
                foreach (DataRow dr in dtPivot.Rows)
                {
                    int columnCount = 0;
                    string[] myTableRow = new string[dtPivot.Columns.Count];
                    foreach (DataColumn dc in dtPivot.Columns)
                    {
                        myTableRow[columnCount] = dr[dc.ColumnName].ToString();
                        columnCount++;
                    }
                    ListTabla.Add(myTableRow);
                }

                Ent._List_Ent_Ventas_Lider_Col = _ListHead;
                Ent.RorwsTh1 = listAno.ToArray();
                Ent.RorwsTh2 = listMes.ToArray();
                Ent.RorwsTh3 = listSem.ToArray();
                objResult.Data = Ent;
                objResult.Success = true;

                Session[_session_ListarVentasLider] = dtPivot;
                Session[_ListAno] = listAno.ToArray();
                Session[_ListMes] = listMes.ToArray();
                Session[_ListSem] = listSem.ToArray();
                Session[_ListNombre] = _ListHead;
                Session[_ListContenido] = ListTabla;
                var JSON = JsonConvert.SerializeObject(objResult);
                return Json(JSON, JsonRequestBehavior.AllowGet);
            }
            else
            {
                objResult.Success = false;
                var JSON = JsonConvert.SerializeObject(objResult);
                return Json(JSON, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Listado principal
        /// </summary>
        /// <create>Juilliand R. Damian Gomez </create>
        /// <update></update> 
        /// <param name="_Ent"></param>
        /// <returns></returns>
        public JsonResult getVentasLiderListarAjax(Ent_jQueryDataTableParams param)
        {
            JsonResponse objResult = new JsonResponse();
    
            DataTable dtreturnf = (DataTable)Session["_session_ListarVentasLider"];
            /*verificar si esta null*/

            int totalCount = dtreturnf.Rows.Count;

            var filteredMembers = dtreturnf.AsEnumerable().CopyToDataTable();

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = dtreturnf.AsEnumerable().Where(x => 
                    x.Field<string>("Asesor").ToUpper().Contains(param.sSearch.ToUpper()) ||
                    x.Field<string>("Lider").ToUpper().Contains(param.sSearch.ToUpper()) ||
                    x.Field<string>("Cliente").ToUpper().Contains(param.sSearch.ToUpper()) ||
                    x.Field<string>("Dni").ToUpper().Contains(param.sSearch.ToUpper()) ||
                    x.Field<string>("Celular").ToUpper().Contains(param.sSearch.ToUpper()) ||
                    x.Field<string>("Correo").ToUpper().Contains(param.sSearch.ToUpper()) ||
                    x.Field<string>("Zona").ToUpper().Contains(param.sSearch.ToUpper())).CopyToDataTable();
            }

            var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);

            if (param.iSortingCols > 0)
            {
                if (Request["sSortDir_0"].ToString() == "asc")
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.AsEnumerable().OrderBy(o => o.Field<string>("Asesor")).CopyToDataTable(); break;
                        case 1: filteredMembers = filteredMembers.AsEnumerable().OrderBy(o => o.Field<string>("Lider")).CopyToDataTable(); break;
                        case 2: filteredMembers = filteredMembers.AsEnumerable().OrderBy(o => o.Field<string>("Cliente")).CopyToDataTable(); break;
                        case 3: filteredMembers = filteredMembers.AsEnumerable().OrderBy(o => o.Field<string>("Dni")).CopyToDataTable(); break;
                        case 4: filteredMembers = filteredMembers.AsEnumerable().OrderBy(o => o.Field<string>("Celular")).CopyToDataTable(); break;
                        case 5: filteredMembers = filteredMembers.AsEnumerable().OrderBy(o => o.Field<string>("Correo")).CopyToDataTable(); break;
                        case 6: filteredMembers = filteredMembers.AsEnumerable().OrderBy(o => o.Field<string>("Asesor")).CopyToDataTable(); break;
                    }
                }
                else
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.AsEnumerable().OrderByDescending(o => o.Field<string>("Asesor")).CopyToDataTable(); break;
                        case 1: filteredMembers = filteredMembers.AsEnumerable().OrderByDescending(o => o.Field<string>("Lider")).CopyToDataTable(); break;
                        case 2: filteredMembers = filteredMembers.AsEnumerable().OrderByDescending(o => o.Field<string>("Cliente")).CopyToDataTable(); break;
                        case 3: filteredMembers = filteredMembers.AsEnumerable().OrderByDescending(o => o.Field<string>("Dni")).CopyToDataTable(); break;
                        case 4: filteredMembers = filteredMembers.AsEnumerable().OrderByDescending(o => o.Field<string>("Celular")).CopyToDataTable(); break;
                        case 5: filteredMembers = filteredMembers.AsEnumerable().OrderByDescending(o => o.Field<string>("Correo")).CopyToDataTable(); break;
                        case 6: filteredMembers = filteredMembers.AsEnumerable().OrderByDescending(o => o.Field<string>("Zona")).CopyToDataTable(); break;
                    }
                }
            }

            var dtreturn = filteredMembers.AsEnumerable()
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength)
            .CopyToDataTable();

            JavaScriptSerializer serializer = new JavaScriptSerializer();

            var lst = dtreturn.AsEnumerable()
                        .Select(r => r.Table.Columns.Cast<DataColumn>()
                                .Select(c => new KeyValuePair<string, object>(c.ColumnName, r[c.Ordinal])
                               ).ToDictionary(z => z.Key, z => z.Value)
                        ).ToList();

            objResult.Data = serializer.Serialize(lst);
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalCount,
                iTotalDisplayRecords = filteredMembers.Rows.Count,
                aaData = objResult
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Validando exportancion
        /// </summary>
        /// <create>Juilliand R. Damian Gomez </create>
        /// <update></update> 
        /// <param name="_Ent"></param>
        /// <returns></returns>
        public ActionResult get_exporta_LiderListar_excel(Ent_Ventas_Lider _Ent)
        {
            JsonResponse objResult = new JsonResponse();
            try
            {
                Session[_session_ListarVentasLider_Excel] = null;
                string cadena = "";
                if (Session[_session_ListarVentasLider] != null)
                {

                    DataTable _ListarVentasLider = (DataTable)Session["_session_ListarVentasLider"];
                    if (_ListarVentasLider.Rows.Count == 0)
                    {
                        objResult.Success = false;
                        objResult.Message = "No hay filas para exportar";
                    }
                    else
                    {
                        cadena = get_html_ListarVentasLider_str((List<string[]>)Session[_ListContenido], _Ent);
                        if (cadena.Length == 0)
                        {
                            objResult.Success = false;
                            objResult.Message = "Error del formato html";
                        }
                        else
                        {
                            objResult.Success = true;
                            objResult.Message = "Se genero el excel correctamente";
                            Session[_session_ListarVentasLider_Excel] = cadena;
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
        /// Forma del Excel
        /// </summary>
        /// <create>Juilliand R. Damian Gomez </create>
        /// <update></update> 
        /// <param name="_ListContenido"></param>
        /// <param name="_Ent"></param>
        /// <returns></returns>
        public string get_html_ListarVentasLider_str(List<string[]> _ListContenido, Ent_Ventas_Lider _Ent)
        {
            StringBuilder sb = new StringBuilder();
            var Listar = _ListContenido.ToArray();
            string[] listAno = (string[])Session[_ListAno];
            string[] listMes = (string[])Session[_ListMes];
            //string[] listSem = (string[])Session[_ListSem];
            List<Ent_Ventas_Lider_Col> _ListNombres = (List<Ent_Ventas_Lider_Col>)Session[_ListNombre];
            try
            {
                var result = "<thead>";
                result += "<tr bgColor='#1E77AB'><th colspan=8></th>";

                foreach (var key in listAno)
                {                  
                    result += "<th  style='text-align: center;' colspan=" + key.Substring((key.IndexOf("/") + 1)) + " style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>" + key.Substring(0, key.IndexOf("/")) + "</font></th>";
                }
                result += "</tr>";
                result += "<tr bgColor='#1E77AB'><th colspan=8></th>";
                foreach (var key in listMes)
                {
                    result += "<th  style='text-align: center;' colspan=" + key.Substring((key.LastIndexOf("/") + 1)) + " style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>" + key.Substring((key.IndexOf("/") + 1), (key.LastIndexOf("/") - (key.IndexOf("/") + 1))) + "</font></th>";
                }
                result += "</tr>";
                result += "<tr>";

                foreach (var item in _ListNombres)
                {
                    result += "<th  bgColor='" + item.cssColor + "' style='text-align: center;' style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>" + item.fName + "</font></th>";
                }
                result += "</tr>";
                result += "</thead>";

                sb.Append("<div>");
                sb.Append("<table cellspacing='0' style='width: 1000px' rules='all' border='0' style='border-collapse:collapse;'>");
                sb.Append("<tr><td Colspan='14'></td></tr>");
                sb.Append("<tr><td Colspan='14' valign='middle' align='center' style='vertical-align: middle;font-size: 18.0pt;font-weight: bold;color:#285A8F'>REPORTE DE VENTAS POR DIRECTORA</td></tr>");
                sb.Append("<tr><td Colspan='14' valign='middle' align='center' style='vertical-align: middle;font-size: 10.0pt;font-weight: bold;color:#000000'>Desde el " + String.Format("{0:dd/MM/yyyy}", _Ent.FechaInicio) + " hasta el " + String.Format("{0:dd/MM/yyyy}", _Ent.FechaFin) + "</td></tr>");//subtitulo
                sb.Append("</table>");
                sb.Append("<table  border='1' bgColor='#ffffff' borderColor='#FFFFFF' cellSpacing='2' cellPadding='2' style='font-size:10.0pt; font-family:Calibri; background:white;width: 1000px'><tr  bgColor='#5799bf'>\n");

                sb.Append(result);
                sb.Append("<tbody>");
                foreach (var item in Listar)
                {
                    sb.Append("<tr>");
                    foreach (var j in item)
                    {
                        sb.Append("<td>" + j + "</td>");
                    }
                    sb.Append("</tr>");
                }

                sb.Append("</tbody>");
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
        /// Exportar Excel
        /// </summary>
        /// <create>Juilliand R. Damian Gomez </create>
        /// <update></update> 
        /// <returns></returns>
        public ActionResult ListarVentasLiderExcel()
        {
            string NombreArchivo = "VentasXLider";
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
                Response.Write(Session[_session_ListarVentasLider_Excel].ToString());
                Response.End();
            }
            catch
            {

            }
            return Json(new { estado = 0, mensaje = 1 });
        }
        #endregion

        #region <CONSULTA DE PREMIOS>	
        public ActionResult Consulta_Premios()
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
                Session[_session_ListarConsulta_Premios] = null;

                List<Ent_Combo> ListarLider = new List<Ent_Combo>();
                List<Ent_Combo> ListarAsesor = new List<Ent_Combo>();
                var ListarAsesorLider = datUtil.Lista_Asesor_Lider().ToList();

                if (_usuario.usu_tip_id =="09")
                {
                    ListarLider = new List<Ent_Combo>() { new Ent_Combo() { bas_id = -1, nombres = "Seleccionar a todos" } };
                    ListarAsesor = ListarAsesorLider.Where(x => x.bas_usu_tipid == "09" && x.bas_aco_id == _usuario.usu_asesor).ToList();
                    ListarLider = ListarLider.Concat(ListarAsesorLider.Where(x => x.bas_usu_tipid != "09" && x.bas_aco_id == _usuario.usu_asesor)).ToList();
                }

                if (_usuario.usu_tip_id == "01")
                {                   
                    ListarAsesor = ListarAsesorLider.Where(x => x.bas_usu_tipid == "09" && x.bas_aco_id == _usuario.usu_asesor).ToList();
                    ListarLider = ListarAsesorLider.Where(x => x.bas_usu_tipid != "09" && x.bas_aco_id == _usuario.usu_asesor && x.bas_id == _usuario.usu_id).ToList();
                }

                if (_usuario.usu_tip_id == "04" || _usuario.usu_tip_id == "07")
                {
                    ListarAsesor = new List<Ent_Combo>(){ new Ent_Combo() { bas_aco_id ="", nombres ="Seleccionar a todos" }};
                    ListarLider = new List<Ent_Combo>() { new Ent_Combo() { bas_id = -1, nombres = "Seleccionar a todos" } };
                    ListarAsesor = ListarAsesor.Concat(ListarAsesorLider.Where(x => x.bas_usu_tipid == "09")).ToList();
                    ListarLider = ListarLider.Concat(ListarAsesorLider.Where(x => x.bas_usu_tipid != "09" && x.bas_aco_id != "")).ToList();
                }

                ViewBag.ListarAsesor = ListarAsesor.ToList();
                ViewBag.ListarLider = ListarLider.ToList();

                DateTime date = DateTime.Now;
                ViewBag.ListarCamFecha = datFacturacion.ListarCampaniaFecha().Where(x => x.Anio == date.Year);
                Ent_Consulta_Premios EntConsultaPremios = new Ent_Consulta_Premios();
                ViewBag.EntConsultaPremios = EntConsultaPremios;
                return View();
            }
        }
        public JsonResult getLisConsulta_PremiosAjax(Ent_jQueryDataTableParams param, bool isOkUpdate, string FechaInicio,string FechaFin, string Bas_Id, string Bas_Aco_Id)
        {
            Ent_Consulta_Premios EntConsultaPremios = new Ent_Consulta_Premios();

            if (isOkUpdate)
            {
                EntConsultaPremios.FechaIni = DateTime.Parse(FechaInicio);
                EntConsultaPremios.FechaFin = DateTime.Parse(FechaFin);
                EntConsultaPremios.Valida =false;
                EntConsultaPremios.Bas_Id = Bas_Id;
                EntConsultaPremios.Bas_Aco_Id = (Bas_Id == "-1" ) ? Bas_Aco_Id : Bas_Aco_Id = "";

                List<Ent_Consulta_Premios> _ListarConsulta_Premios = datFacturacion.List_ConsultaPremio(EntConsultaPremios).ToList();
                Session[_session_ListarConsulta_Premios] = _ListarConsulta_Premios;
            }

            /*verificar si esta null*/
            if (Session[_session_ListarConsulta_Premios] == null)
            {
                List<Ent_Consulta_Premios> _ListarConsulta_Premios = new List<Ent_Consulta_Premios>();
                Session[_session_ListarConsulta_Premios] = _ListarConsulta_Premios;
            }

            IQueryable<Ent_Consulta_Premios> entDocTrans = ((List<Ent_Consulta_Premios>)(Session[_session_ListarConsulta_Premios])).AsQueryable();
            //Manejador de filtros
            int totalCount = entDocTrans.Count();
            IEnumerable<Ent_Consulta_Premios> filteredMembers = entDocTrans;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = entDocTrans.Where(
                        m =>
                            m.Asesor.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Lider.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Promotor.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Documento.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Total.ToString().Contains(param.sSearch.ToUpper()) ||
                            m.Limite.ToString().Contains(param.sSearch.ToUpper()) ||
                            m.Saldo.ToString().Contains(param.sSearch.ToUpper()) ||
                            m.Descripcion.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Liqprem.ToString().Contains(param.sSearch.ToUpper()) ||
                            m.Liqpremiori.ToString().Contains(param.sSearch.ToUpper()) ||
                            m.Xentrega.ToUpper().Contains(param.sSearch.ToUpper())
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
                        case 2: filteredMembers = filteredMembers.OrderBy(o => o.Promotor); break;
                        case 3: filteredMembers = filteredMembers.OrderBy(o => o.Documento); break;
                        case 4: filteredMembers = filteredMembers.OrderBy(o => o.Total); break;
                        case 5: filteredMembers = filteredMembers.OrderBy(o => o.Limite); break;
                        case 6: filteredMembers = filteredMembers.OrderBy(o => o.Saldo); break;
                        case 7: filteredMembers = filteredMembers.OrderBy(o => o.Descripcion); break;
                        case 8: filteredMembers = filteredMembers.OrderBy(o => o.Liqprem); break;
                        case 9: filteredMembers = filteredMembers.OrderBy(o => o.Liqpremiori); break;
                        case 10: filteredMembers = filteredMembers.OrderBy(o => o.Xentrega); break;
                    }
                }
                else
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderByDescending(o => o.Asesor); break;
                        case 1: filteredMembers = filteredMembers.OrderByDescending(o => o.Lider); break;
                        case 2: filteredMembers = filteredMembers.OrderByDescending(o => o.Promotor); break;
                        case 3: filteredMembers = filteredMembers.OrderByDescending(o => o.Documento); break;
                        case 4: filteredMembers = filteredMembers.OrderByDescending(o => o.Total); break;
                        case 5: filteredMembers = filteredMembers.OrderByDescending(o => o.Limite); break;
                        case 6: filteredMembers = filteredMembers.OrderByDescending(o => o.Saldo); break;
                        case 7: filteredMembers = filteredMembers.OrderByDescending(o => o.Descripcion); break;
                        case 8: filteredMembers = filteredMembers.OrderByDescending(o => o.Liqprem); break;
                        case 9: filteredMembers = filteredMembers.OrderByDescending(o => o.Liqpremiori); break;
                        case 10: filteredMembers = filteredMembers.OrderByDescending(o => o.Xentrega); break;
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

        public ActionResult get_exporta_LisConsulta_Premios_excel(Ent_Consulta_Premios _Ent)
        {
            JsonResponse objResult = new JsonResponse();
            try
            {
                Session[_session_ListarConsulta_Premios_Excel] = null;
                string cadena = "";
                if (Session[_session_ListarConsulta_Premios] != null)
                {

                    List<Ent_Consulta_Premios> _ListarConsulta_Premios = (List<Ent_Consulta_Premios>)Session[_session_ListarConsulta_Premios];
                    if (_ListarConsulta_Premios.Count == 0)
                    {
                        objResult.Success = false;
                        objResult.Message = "No hay filas para exportar";
                    }
                    else
                    {
                        cadena = get_html_ListarConsulta_Premios_str((List<Ent_Consulta_Premios>)Session[_session_ListarConsulta_Premios], _Ent);
                        if (cadena.Length == 0)
                        {
                            objResult.Success = false;
                            objResult.Message = "Error del formato html";
                        }
                        else
                        {
                            objResult.Success = true;
                            objResult.Message = "Se genero el excel correctamente";
                            Session[_session_ListarConsulta_Premios_Excel] = cadena;
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

        public string get_html_ListarConsulta_Premios_str(List<Ent_Consulta_Premios> _ListarConsulta_Premios, Ent_Consulta_Premios _Ent)
        {
            StringBuilder sb = new StringBuilder();
            var Lista = _ListarConsulta_Premios.ToList();
            try
            {
                sb.Append("<div>");
                sb.Append("<table cellspacing='0' style='width: 1000px' rules='all' border='0' style='border-collapse:collapse;'>");
                sb.Append("<tr><td Colspan='10'></td></tr>");
                sb.Append("<tr><td Colspan='10' valign='middle' align='center' style='vertical-align: middle;font-size: 18.0pt;font-weight: bold;color:#285A8F'>REPORTE DE PREMIOS</td></tr>");
                sb.Append("<tr><td Colspan='10' valign='middle' align='center' style='vertical-align: middle;font-size: 10.0pt;font-weight: bold;color:#000000'>Rango de : " + String.Format("{0:dd/MM/yyyy}", _Ent.FechaIni) + " hasta " + String.Format("{0:dd/MM/yyyy}", _Ent.FechaFin) + "</td></tr>");//subtitulo
                sb.Append("<tr>\n");
                sb.Append("<th bgColor='#1E77AB' style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Asesor</font></th>\n");
                sb.Append("<th bgColor='#1E77AB' style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Directora</font></th>\n");
                sb.Append("<th bgColor='#1E77AB' style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Promotor</font></th>\n");
                sb.Append("<th bgColor='#1E77AB' style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Documento</font></th>\n");
                sb.Append("<th bgColor='#1E77AB' style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Venta Bruta</font></th>\n");
                sb.Append("<th bgColor='#1E77AB' style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Monto Minima</font></th>\n");
                sb.Append("<th bgColor='#1E77AB' style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Falta premio</font></th>\n");
                sb.Append("<th bgColor='#1E77AB' style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Descripcion</font></th>\n");
                sb.Append("<th bgColor='#1E77AB' style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Nro Regalo</font></th>\n");
                sb.Append("<th bgColor='#1E77AB' style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Nro Pedido</font></th>\n");
                sb.Append("</tr>\n");
                // {0:N2} Separacion miles , {0:F2} solo dos decimales
                foreach (var item in Lista)
                {
                    sb.Append("<tr>\n");
                    sb.Append("<td align=''>" + item.Asesor + "</td>\n");
                    sb.Append("<td align=''>" + item.Lider + "</td>\n");
                    sb.Append("<td align=''>" + item.Promotor + "</td>\n");
                    sb.Append("<td align=''>" + item.Documento + "</td>\n");
                    sb.Append("<td align=''>" + item.Total + "</td>\n");
                    sb.Append("<td align='Right'>" + item.Limite + "</td>\n");
                    sb.Append("<td align='Right'>" + item.Saldo + "</td>\n");
                    sb.Append("<td align='Right'>" + item.Descripcion + "</td>\n");
                    sb.Append("<td align='Center'>" + item.Liqprem + "</td>\n");
                    sb.Append("<td align='Center'>" + item.Liqpremiori + "</td>\n");
                    sb.Append("</tr>\n");
                }
                //sb.Append("<tfoot>\n");
                //sb.Append("<tr bgcolor='#085B8C'>\n");
                //sb.Append("</tr>\n");
                //sb.Append("</tfoot>\n");
                sb.Append("</table></div>");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return sb.ToString();
        }

        public ActionResult ListarConsulta_PremiosExcel()
        {
            string NombreArchivo = "Consulta_Premios";
            String style = style = @"<style> .textmode { mso-number-format:\@; }</style>";
            try
            {
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", "attachment;filename=" + NombreArchivo + ".xls");
                Response.Charset = "UTF-8";
                Response.ContentEncoding = Encoding.Default;
                Response.Write(style);
                Response.Write(Session[_session_ListarConsulta_Premios_Excel].ToString());
                Response.End();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return Json(new { estado = 0, mensaje = 1 });
        }
        #endregion
    }
}