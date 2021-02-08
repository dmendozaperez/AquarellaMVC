using CapaEntidad.General;
using CapaEntidad.Util;
using CapaDato.Facturacion;
using CapaEntidad.Facturacion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using Newtonsoft.Json;
namespace CapaPresentacion.Controllers
{
    public class FacturacionController : Controller
    {
        private Dat_Facturacion datFacturacion = new Dat_Facturacion();
        private string _session_ListarMovimientosVentas = "_session_ListarMovimientosVentas";
        private string _session_ListarMovimientosVentasChart = "_session_ListarMovimientosVentasChart";
        private string _session_ListarMovimientosVentas_Excel = "_session_ListarMovimientosVentas_Excel";

        #region <CONSULTA DE VENTAS POR CATEGORIA>
        public ActionResult Ventas_Categoria()
        {
            ViewBag.Data = "Value,Value1,Value2,Value3";
            ViewBag.ObjectName = "Test,Test1,Test2,Test3";
            Ent_Movimientos_Ventas EntMovimientosVentas = new Ent_Movimientos_Ventas();
            ViewBag.EntMovimientosVentas = EntMovimientosVentas;
            ViewBag.ListarTipoArticulo = datFacturacion.ListarTipoArticulo();
            return View();
        }
        /// <summary>
        /// ListarVenPorCategoria
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
        public JsonResult getLisMovimientosVentaAjax(Ent_jQueryDataTableParams param, bool isOkUpdate, bool isOkSemanal,bool isOkChart,string FechaInicio, string FechaFin, string TipoArticulo)
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
    }
}