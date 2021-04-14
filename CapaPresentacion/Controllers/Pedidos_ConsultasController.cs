using System;
using CapaDato.Pedido;
using CapaEntidad.Util;
using CapaEntidad.Control;
using CapaEntidad.General;
using CapaEntidad.Pedido;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using Newtonsoft.Json;

namespace CapaPresentacion.Controllers
{
    public class Pedidos_ConsultasController : Controller
    {
        #region <DECLARACION DE VARIABLES>
        private Dat_Pedido datPedido = new Dat_Pedido();
        private string _session_ListarPedido_Pagados = "_session_ListarPedido_Pagados";
        private string _session_ListarPedido_Pagados_Excel = "_session_ListarPedido_Pagados_Excel";
        #endregion

        #region <PEDIDOS PAGADOS>
        public ActionResult Pedido_Pagados()
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
                return View();
            }
        }
        public JsonResult getListarPedido_PagadosAjax(Ent_jQueryDataTableParams param, bool isOkUpdate, bool isOkEstado, string ddlEstado)
        {
            Ent_Pedido_Pagados Ent_Pedido_Pagados = new Ent_Pedido_Pagados();
            Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
            int totalpares = 0, Paq_Cantidad = 0;
            Decimal liq_value = 0;
            Session[_session_ListarPedido_Pagados_Excel] = null;
            object ListaEstado = "";
            if (isOkUpdate)
            {
                Ent_Pedido_Pagados.Usu_Id = _usuario.usu_id;
                Session[_session_ListarPedido_Pagados] = datPedido.ListarPedido_Pagados(Ent_Pedido_Pagados).ToList();
            }

            /*verificar si esta null*/
            if (Session[_session_ListarPedido_Pagados] == null)
            {
                List<Ent_Pedido_Pagados> _ListarPedido_Pagados = new List<Ent_Pedido_Pagados>();
                Session[_session_ListarPedido_Pagados] = _ListarPedido_Pagados;
            }

            IQueryable<Ent_Pedido_Pagados> entDocTransEs = ((List<Ent_Pedido_Pagados>)(Session[_session_ListarPedido_Pagados])).AsQueryable();

            var entDocTrans = (isOkEstado == true ? entDocTransEs.Where(x => x.Tipo_Estado == ddlEstado).ToList() : entDocTransEs.ToList());
            Session[_session_ListarPedido_Pagados_Excel] = entDocTrans;
            //Manejador de filtros
            int totalCount = entDocTrans.Count();
            IEnumerable<Ent_Pedido_Pagados> filteredMembers = entDocTrans;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = entDocTrans.Where(
                        m =>
                            m.Asesor.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Lider.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Promotor.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Dni.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Ubicacion.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Pedido.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Tipo_Estado.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Fecha_Cruce.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Estado_Pedido.ToUpper().Contains(param.sSearch.ToUpper())
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
                        case 3: filteredMembers = filteredMembers.OrderBy(o => o.Dni); break;
                        case 4: filteredMembers = filteredMembers.OrderBy(o => o.Ubicacion); break;
                        case 5: filteredMembers = filteredMembers.OrderBy(o => o.Pedido); break;
                        case 6: filteredMembers = filteredMembers.OrderBy(o => o.Tipo_Estado); break;
                        case 7: filteredMembers = filteredMembers.OrderBy(o => o.Fecha_Cruce); break;
                        case 8: filteredMembers = filteredMembers.OrderBy(o => o.Estado_Pedido); break;
                    }
                }
                else
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderByDescending(o => o.Asesor); break;
                        case 1: filteredMembers = filteredMembers.OrderByDescending(o => o.Lider); break;
                        case 2: filteredMembers = filteredMembers.OrderByDescending(o => o.Promotor); break;
                        case 3: filteredMembers = filteredMembers.OrderByDescending(o => o.Dni); break;
                        case 4: filteredMembers = filteredMembers.OrderByDescending(o => o.Ubicacion); break;
                        case 5: filteredMembers = filteredMembers.OrderByDescending(o => o.Pedido); break;
                        case 6: filteredMembers = filteredMembers.OrderByDescending(o => o.Tipo_Estado); break;
                        case 7: filteredMembers = filteredMembers.OrderByDescending(o => o.Fecha_Cruce); break;
                        case 8: filteredMembers = filteredMembers.OrderByDescending(o => o.Estado_Pedido); break;
                    }
                }
            }
            if (entDocTrans.Count() > 0)
            {
                ListaEstado = entDocTransEs.Select(x => new { Codigo = x.Tipo_Estado, Descripcion = x.Tipo_Estado }).Distinct().ToList();
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
                lListaEstado = ListaEstado
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Se arma el reporte en excel
        /// </summary>
        /// <param name="_Ent"></param>
        /// <returns></returns>
        public ActionResult get_exporta_Pedido_Pagados_excel(Ent_Pedido_Pagados _Ent)
        {
            JsonResponse objResult = new JsonResponse();
            try
            {
                //Session[_session_ListarPedidoFactura_Excel] = null;
                string cadena = "";
                if (Session[_session_ListarPedido_Pagados_Excel] != null)
                {

                    List<Ent_Pedido_Pagados> _ListarPedido_Pagados = (List<Ent_Pedido_Pagados>)Session[_session_ListarPedido_Pagados_Excel];
                    if (_ListarPedido_Pagados.Count == 0)
                    {
                        objResult.Success = false;
                        objResult.Message = "No hay filas para exportar";

                    }
                    else
                    {
                        cadena = get_html_ListarPedido_Pagados_str((List<Ent_Pedido_Pagados>)Session[_session_ListarPedido_Pagados_Excel], _Ent);
                        if (cadena.Length == 0)
                        {
                            objResult.Success = false;
                            objResult.Message = "Error del formato html";
                        }
                        else
                        {
                            objResult.Success = true;
                            objResult.Message = "Se genero el excel correctamente";
                            Session[_session_ListarPedido_Pagados_Excel] = cadena;
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
        /// Formato excel
        /// </summary>
        /// <param name="_ListarPedidoDespacho"></param>
        /// <param name="_Ent"></param>
        /// <returns></returns>
        public string get_html_ListarPedido_Pagados_str(List<Ent_Pedido_Pagados> _ListarPedidoDespacho, Ent_Pedido_Pagados _Ent)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                var Lista = _ListarPedidoDespacho.ToList();
                sb.Append("<div><table cellspacing='0' style='width: 1000px' rules='all' border='0' style='border-collapse:collapse;'><tr><td Colspan='13'></td></tr><tr><td Colspan='13' valign='middle' align='center' style='vertical-align: middle;font-size: 16.0pt;font-weight: bold;color:#285A8F'>REPORTE DE PEDIDOS PAGADOS</td></tr></table>");
                sb.Append("<table  border='1' bgColor='#ffffff' borderColor='#FFFFFF' cellSpacing='2' cellPadding='2' style='font-size:10.0pt; font-family:Calibri; background:white;width: 1000px'><tr  bgColor='#5799bf'>\n");
                sb.Append("<tr bgColor='#1E77AB'>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Asesor</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Lider</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Promotor</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Documento</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Ubicación</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Nro. Pedido</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Tipo Estado</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Fecha</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Estado Pedido</font></th>\n");
                sb.Append("</tr>\n");

                foreach (var item in Lista)
                {
                    sb.Append("<tr>\n");
                    sb.Append("<td>" + item.Asesor + "</td>\n");
                    sb.Append("<td>" + item.Lider + "</td>\n");
                    sb.Append("<td>" + item.Promotor + "</td>\n");
                    sb.Append("<td align='center'>" + item.Dni + "</td>\n");
                    sb.Append("<td>" + item.Ubicacion + "</td>\n");
                    sb.Append("<td align='Center'>" + item.Pedido + "</td>\n");
                    sb.Append("<td>" + item.Tipo_Estado + "</td>\n");
                    sb.Append("<td>" + item.Fecha_Cruce + "</td>\n");
                    sb.Append("<td>" + item.Estado_Pedido + "</td>\n");
                    sb.Append("</tr>\n");
                }
                sb.Append("</table></div>");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Exportamos el excel
        /// </summary>
        /// <returns></returns>
        public ActionResult ListaPedido_Pagados_Excel()
        {
            string NombreArchivo = "Info_Liq_" + DateTime.Today.ToShortDateString();
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
                Response.Write(Session[_session_ListarPedido_Pagados_Excel].ToString());
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