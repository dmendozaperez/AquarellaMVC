using CapaEntidad.General;
using CapaEntidad.Control;
using CapaEntidad.Util;
using CapaEntidad.RRHH;
using CapaDato.Util;
using CapaDato.RRHH;
using CapaDato.Facturacion;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using Newtonsoft.Json;
using System.Reflection;

namespace CapaPresentacion.Controllers
{
    public class RRHHController : Controller
    {
        private Dat_Util datUtil = new Dat_Util();
        private Dat_RRHH datRRHH = new Dat_RRHH();

        private string _session_ListarPromotor_Lider = "_session_ListarPromotor_Lider";
        private string _session_ListarPromotor_Lider_Excel = "_session_ListarPromotor_Lider_Excel";
        private string _session_ListarConsultaKPI_Detalle = "_session_ListarConsultaKPI_Detalle";
        private string _session_ListarConsultaKPI_Detalle_Excel = "_session_ListarConsultaKPI_Detalle_Excel";
        private string _session_ListarConsultaKPI = "_session_ListarConsultaKPI";
        private string _session_ListarConsultaKPI_Excel = "_session_ListarConsultaKPI_Excel";
        #region <Consulta de Promotor por lider>
        public ActionResult ConsultaPromotorXLider()
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
                Ent_Promotor_Lider EntPromotorLider = new Ent_Promotor_Lider();
                ViewBag.EntPromotorLider = EntPromotorLider;
                ListarCLiente.Add(new Ent_Combo() { codigo = "-1", descripcion = "-- Selecionar Todos--" });
                int Cant = datUtil.Listar_Clientes(_usuario).Count();
                ViewBag.ListarCLiente = (Cant == 1 ? datUtil.Listar_Clientes(_usuario) : ListarCLiente.Concat(datUtil.Listar_Clientes(_usuario)));

                return View();
            }
        }

        public JsonResult getLisPromotorXLiderAjax(Ent_jQueryDataTableParams param, bool isOkUpdate, string FechaInicio, string FechaFin,string IdCliente)
        {
            Ent_Promotor_Lider EntPromotor_Lider = new Ent_Promotor_Lider();
            Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
            if (isOkUpdate)
            {
                EntPromotor_Lider.Bas_Id = IdCliente;
                EntPromotor_Lider.FechaInicio = DateTime.Parse(FechaInicio);
                EntPromotor_Lider.FechaFin = DateTime.Parse(FechaFin);
                EntPromotor_Lider.Asesor = _usuario.usu_asesor;

                List<Ent_Promotor_Lider> _ListarPromotor_Lider = datRRHH.ListarPromotorLider(EntPromotor_Lider).ToList();
                Session[_session_ListarPromotor_Lider] = _ListarPromotor_Lider;
            }

            /*verificar si esta null*/
            if (Session[_session_ListarPromotor_Lider] == null)
            {
                List<Ent_Promotor_Lider> _ListarPromotor_Lider = new List<Ent_Promotor_Lider>();
                Session[_session_ListarPromotor_Lider] = _ListarPromotor_Lider;
            }

            IQueryable<Ent_Promotor_Lider> entDocTrans = ((List<Ent_Promotor_Lider>)(Session[_session_ListarPromotor_Lider])).AsQueryable();
            //Manejador de filtros
            int totalCount = entDocTrans.Count();
            IEnumerable<Ent_Promotor_Lider> filteredMembers = entDocTrans;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = entDocTrans.Where(
                        m =>
                            m.Asesor.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Lider.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Promotor.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Documento.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Departamento.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Provincia.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Distrito.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Direccion.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Telefono.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Correo.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Celular.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Fecing.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Fecactv.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Fec_Nac.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Zona.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Activo.ToUpper().Contains(param.sSearch.ToUpper())
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
                        case 4: filteredMembers = filteredMembers.OrderBy(o => o.Departamento); break;
                        case 5: filteredMembers = filteredMembers.OrderBy(o => o.Provincia); break;
                        case 6: filteredMembers = filteredMembers.OrderBy(o => o.Distrito); break;
                        case 7: filteredMembers = filteredMembers.OrderBy(o => o.Direccion); break;
                        case 8: filteredMembers = filteredMembers.OrderBy(o => o.Telefono); break;
                        case 9: filteredMembers = filteredMembers.OrderBy(o => o.Correo); break;
                        case 10: filteredMembers = filteredMembers.OrderBy(o => o.Celular); break;
                        case 11: filteredMembers = filteredMembers.OrderBy(o => o.Fecing); break;
                        case 12: filteredMembers = filteredMembers.OrderBy(o => o.Fecactv); break;
                        case 13: filteredMembers = filteredMembers.OrderBy(o => o.Fec_Nac); break;
                        case 14: filteredMembers = filteredMembers.OrderBy(o => o.Zona); break;
                        case 15: filteredMembers = filteredMembers.OrderBy(o => o.Activo); break;
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
                        case 4: filteredMembers = filteredMembers.OrderByDescending(o => o.Departamento); break;
                        case 5: filteredMembers = filteredMembers.OrderByDescending(o => o.Provincia); break;
                        case 6: filteredMembers = filteredMembers.OrderByDescending(o => o.Distrito); break;
                        case 7: filteredMembers = filteredMembers.OrderByDescending(o => o.Direccion); break;
                        case 8: filteredMembers = filteredMembers.OrderByDescending(o => o.Telefono); break;
                        case 9: filteredMembers = filteredMembers.OrderByDescending(o => o.Correo); break;
                        case 10: filteredMembers = filteredMembers.OrderByDescending(o => o.Celular); break;
                        case 11: filteredMembers = filteredMembers.OrderByDescending(o => o.Fecing); break;
                        case 12: filteredMembers = filteredMembers.OrderByDescending(o => o.Fecactv); break;
                        case 13: filteredMembers = filteredMembers.OrderByDescending(o => o.Fec_Nac); break;
                        case 14: filteredMembers = filteredMembers.OrderByDescending(o => o.Zona); break;
                        case 15: filteredMembers = filteredMembers.OrderByDescending(o => o.Activo); break;
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

        public ActionResult get_exporta_LisPromotorXLider_excel(Ent_Promotor_Lider _Ent)
        {
            JsonResponse objResult = new JsonResponse();
            try
            {
                Session[_session_ListarPromotor_Lider_Excel] = null;
                string cadena = "";
                if (Session[_session_ListarPromotor_Lider] != null)
                {

                    List<Ent_Promotor_Lider> _ListarPromotor_Lider = (List<Ent_Promotor_Lider>)Session[_session_ListarPromotor_Lider];
                    if (_ListarPromotor_Lider.Count == 0)
                    {
                        objResult.Success = false;
                        objResult.Message = "No hay filas para exportar";
                    }
                    else
                    {
                        cadena = get_html_ListarPromotor_Lider_str((List<Ent_Promotor_Lider>)Session[_session_ListarPromotor_Lider], _Ent);
                        if (cadena.Length == 0)
                        {
                            objResult.Success = false;
                            objResult.Message = "Error del formato html";
                        }
                        else
                        {
                            objResult.Success = true;
                            objResult.Message = "Se genero el excel correctamente";
                            Session[_session_ListarPromotor_Lider_Excel] = cadena;
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

        public string get_html_ListarPromotor_Lider_str(List<Ent_Promotor_Lider> _ListarPromotor_Lider, Ent_Promotor_Lider _Ent)
        {
            StringBuilder sb = new StringBuilder();
            var Lista = _ListarPromotor_Lider.ToList();
            try
            {
                sb.Append("<div>");
                sb.Append("<table cellspacing='0' style='width: 1000px' rules='all' border='0' style='border-collapse:collapse;'>");
                sb.Append("<tr><td Colspan='10'></td></tr>");
                sb.Append("<tr><td Colspan='10' valign='middle' align='center' style='vertical-align: middle;font-size: 18.0pt;font-weight: bold;color:#285A8F'>REPORTE DE PROMOTOR POR DIRECTORA</td></tr>");
                sb.Append("<tr><td Colspan='10' valign='middle' align='center' style='vertical-align: middle;font-size: 10.0pt;font-weight: bold;color:#000000'>Desde el " + String.Format("{0:dd/MM/yyyy}", _Ent.FechaInicio) + " hasta el " + String.Format("{0:dd/MM/yyyy}", _Ent.FechaFin) + "</td></tr>");//subtitulo
                sb.Append("<tr bgColor='#1E77AB'>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Asesor</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Directora</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Promotor</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Documento</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Fec. Nacimiento</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Departamento</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Provincia</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Distrito</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Direccion</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Telefono</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Correo</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Celular</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Fec. Ing</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Fec. Activación</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Activo</font></th>\n");
                sb.Append("</tr>\n");
                // {0:N2} Separacion miles , {0:F2} solo dos decimales
                foreach (var item in Lista)
                {
                    sb.Append("<tr>\n");
                    sb.Append("<td align=''>" + item.Asesor + "</td>\n");
                    sb.Append("<td align=''>" + item.Lider + "</td>\n");
                    sb.Append("<td align=''>" + item.Promotor + "</td>\n");
                    sb.Append("<td align='Center'>" + item.Documento + "</td>\n");
                    sb.Append("<td align='Center'>" + item.Fec_Nac + "</td>\n");
                    sb.Append("<td align=''>" + item.Departamento + "</td>\n");
                    sb.Append("<td align=''>" + item.Provincia + "</td>\n");
                    sb.Append("<td align=''>" + item.Distrito + "</td>\n");
                    sb.Append("<td align=''>" + item.Direccion + "</td>\n");
                    sb.Append("<td align='Center'>" + item.Telefono + "</td>\n");
                    sb.Append("<td align=''>" + item.Correo + "</td>\n");
                    sb.Append("<td align='Center'>" + item.Celular + "</td>\n");
                    sb.Append("<td align='Center'>" + item.Fecing + "</td>\n");
                    sb.Append("<td align='Center'>" + item.Fecactv + "</td>\n");
                    sb.Append("<td align='Center'>" + item.Activo + "</td>\n");
                    sb.Append("</tr>\n");
                }
                //sb.Append("<tfoot>\n");
                //sb.Append("<tr bgcolor='#085B8C'>\n");
                //sb.Append("</tr>\n");
                //sb.Append("</tfoot>\n");
                sb.Append("</table></div>");
            }
            catch
            {

            }
            return sb.ToString();
        }
        public ActionResult ListarPromotor_LiderExcel()
        {
            string NombreArchivo = "ListaPromotorXLider";
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
                Response.Write(Session[_session_ListarPromotor_Lider_Excel].ToString());
                Response.End();
            }
            catch
            {

            }
            return Json(new { estado = 0, mensaje = 1 });
        }
        #endregion

        #region <Consulta de Resultado KPI>
        public ActionResult Consulta_KPI()
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
                Session[_session_ListarConsultaKPI_Detalle] = null;
                Session[_session_ListarConsultaKPI] = null;
                Ent_KPI_Lider _EntL = new Ent_KPI_Lider();
                List<Ent_KPI_Asesor> ListarAsesor = new List<Ent_KPI_Asesor>{ new Ent_KPI_Asesor() {Codigo = "", Descripcion ="Seleccionar a todos"}};
                List<Ent_KPI_Lider> ListarLider = new List<Ent_KPI_Lider>{new Ent_KPI_Lider() {Codigo = "-1", Descripcion ="Seleccionar a todos"}};
                ViewBag.ListarAsesor = ListarAsesor.Concat(datRRHH.ListarAsesor());
                _EntL.IdAsesor = "";
                ViewBag.ListarLider = ListarLider.Concat(datRRHH.ListarLider(_EntL));
                Ent_ConsultaKPI_Detalle EntConsultaKPIDetalle = new Ent_ConsultaKPI_Detalle();
                Ent_ConsultaKPI EntConsultaKPI = new Ent_ConsultaKPI();
                ViewBag.EntConsultaKPIDetalle = EntConsultaKPIDetalle;
                ViewBag.EntConsultaKPI = EntConsultaKPI;
                return View();
            }
        }

        public JsonResult getLisConsultaKPI_DetalleAjax(Ent_jQueryDataTableParams param, bool isOkUpdate, string FechaInicio, string FechaFin, string IdAsesor, string IdLider, bool IsAseOrLider)
        {
            Ent_ConsultaKPI_Detalle EntConsultaKPIDetalle = new Ent_ConsultaKPI_Detalle();
       
            if (isOkUpdate)
            {
                EntConsultaKPIDetalle.IdAsesor = IdAsesor;
                EntConsultaKPIDetalle.IdLider = IdLider;
                EntConsultaKPIDetalle.FechaInicio = DateTime.Parse(FechaInicio);
                EntConsultaKPIDetalle.FechaFin = DateTime.Parse(FechaFin);
                EntConsultaKPIDetalle.IsAseOrLider = IsAseOrLider;

                List<Ent_ConsultaKPI_Detalle> _ListarConsultaKPI_Detalle = datRRHH.ListarConsultaKPI_Detalle(EntConsultaKPIDetalle).ToList();
                Session[_session_ListarConsultaKPI_Detalle] = _ListarConsultaKPI_Detalle;
            }

            /*verificar si esta null*/
            if (Session[_session_ListarConsultaKPI_Detalle] == null)
            {
                List<Ent_ConsultaKPI_Detalle> _ListarConsultaKPI_Detalle = new List<Ent_ConsultaKPI_Detalle>();
                Session[_session_ListarConsultaKPI_Detalle] = _ListarConsultaKPI_Detalle;
            }

            IQueryable<Ent_ConsultaKPI_Detalle> entDocTrans = ((List<Ent_ConsultaKPI_Detalle>)(Session[_session_ListarConsultaKPI_Detalle])).AsQueryable();
            //Manejador de filtros
            int totalCount = entDocTrans.Count();
            IEnumerable<Ent_ConsultaKPI_Detalle> filteredMembers = entDocTrans;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = entDocTrans.Where(
                        m =>
                            m.Lider.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Asesor.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Anio.ToString().Contains(param.sSearch.ToUpper()) ||
                            m.Mes.ToUpper().Contains(param.sSearch.ToUpper()) ||
                            m.Facturacion.ToString().Contains(param.sSearch.ToUpper()) ||
                            m.Margen.ToString().Contains(param.sSearch.ToUpper()) ||
                            m.Continuas.ToString().Contains(param.sSearch.ToUpper()) ||
                            m.Afiliadas.ToString().Contains(param.sSearch.ToUpper()) ||
                            m.Reactivadas.ToString().Contains(param.sSearch.ToUpper()) ||
                            m.Activasenmes.ToString().Contains(param.sSearch.ToUpper()) ||
                            m.Desactivadas.ToString().Contains(param.sSearch.ToUpper()) ||
                            m.PorDesact.ToString().Contains(param.sSearch.ToUpper()) ||
                            m.Reg_Mes.ToString().Contains(param.sSearch.ToUpper()) ||
                            m.TactRegMes.ToString().Contains(param.sSearch.ToUpper()) ||
                            m.PorAfiliadasMes.ToString().Contains(param.sSearch.ToUpper()) ||
                            m.ActivasOtroMes.ToString().Contains(param.sSearch.ToUpper()) ||
                            m.TotalActivas.ToString().Contains(param.sSearch.ToUpper()) ||
                            m.TicketProm.ToString().Contains(param.sSearch.ToUpper())
                );
            }
            var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);

            if (param.iSortingCols > 0)
            {
                if (Request["sSortDir_0"].ToString() == "asc")
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderBy(o => o.Lider); break;
                        case 1: filteredMembers = filteredMembers.OrderBy(o => o.Asesor); break;
                        case 2: filteredMembers = filteredMembers.OrderBy(o => o.Anio); break;
                        case 3: filteredMembers = filteredMembers.OrderBy(o => o.Mes); break;
                        case 4: filteredMembers = filteredMembers.OrderBy(o => o.Facturacion); break;
                        case 5: filteredMembers = filteredMembers.OrderBy(o => o.Margen); break;
                        case 6: filteredMembers = filteredMembers.OrderBy(o => o.Continuas); break;
                        case 7: filteredMembers = filteredMembers.OrderBy(o => o.Afiliadas); break;
                        case 8: filteredMembers = filteredMembers.OrderBy(o => o.Reactivadas); break;
                        case 9: filteredMembers = filteredMembers.OrderBy(o => o.Activasenmes); break;
                        case 10: filteredMembers = filteredMembers.OrderBy(o => o.Desactivadas); break;
                        case 11: filteredMembers = filteredMembers.OrderBy(o => o.PorDesact); break;
                        case 12: filteredMembers = filteredMembers.OrderBy(o => o.Reg_Mes); break;
                        case 13: filteredMembers = filteredMembers.OrderBy(o => o.TactRegMes); break;
                        case 14: filteredMembers = filteredMembers.OrderBy(o => o.PorAfiliadasMes); break;
                        case 15: filteredMembers = filteredMembers.OrderBy(o => o.ActivasOtroMes); break;
                        case 16: filteredMembers = filteredMembers.OrderBy(o => o.TotalActivas); break;
                        case 17: filteredMembers = filteredMembers.OrderBy(o => o.TicketProm); break;
                    }
                }
                else
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderByDescending(o => o.Lider); break;
                        case 1: filteredMembers = filteredMembers.OrderByDescending(o => o.Asesor); break;
                        case 2: filteredMembers = filteredMembers.OrderByDescending(o => o.Anio); break;
                        case 3: filteredMembers = filteredMembers.OrderByDescending(o => o.Mes); break;
                        case 4: filteredMembers = filteredMembers.OrderByDescending(o => o.Facturacion); break;
                        case 5: filteredMembers = filteredMembers.OrderBy(o => o.Margen); break;
                        case 6: filteredMembers = filteredMembers.OrderByDescending(o => o.Continuas); break;
                        case 7: filteredMembers = filteredMembers.OrderByDescending(o => o.Afiliadas); break;
                        case 8: filteredMembers = filteredMembers.OrderByDescending(o => o.Reactivadas); break;
                        case 9: filteredMembers = filteredMembers.OrderByDescending(o => o.Activasenmes); break;
                        case 10: filteredMembers = filteredMembers.OrderByDescending(o => o.Desactivadas); break;
                        case 11: filteredMembers = filteredMembers.OrderByDescending(o => o.PorDesact); break;
                        case 12: filteredMembers = filteredMembers.OrderByDescending(o => o.Reg_Mes); break;
                        case 13: filteredMembers = filteredMembers.OrderByDescending(o => o.TactRegMes); break;
                        case 14: filteredMembers = filteredMembers.OrderByDescending(o => o.PorAfiliadasMes); break;
                        case 15: filteredMembers = filteredMembers.OrderByDescending(o => o.ActivasOtroMes); break;
                        case 16: filteredMembers = filteredMembers.OrderByDescending(o => o.TotalActivas); break;
                        case 17: filteredMembers = filteredMembers.OrderByDescending(o => o.TicketProm); break;
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
                bIsAseOrLider = IsAseOrLider
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult get_exporta_LisConsultaKPI_Detalle_excel(Ent_ConsultaKPI_Detalle _Ent)
        {
            JsonResponse objResult = new JsonResponse();
            try
            {
                Session[_session_ListarConsultaKPI_Detalle_Excel] = null;
                string cadena = "";
                if (Session[_session_ListarConsultaKPI_Detalle] != null)
                {

                    List<Ent_ConsultaKPI_Detalle> _ListarConsultaKPI_Detalle = (List<Ent_ConsultaKPI_Detalle>)Session[_session_ListarConsultaKPI_Detalle];
                    if (_ListarConsultaKPI_Detalle.Count == 0)
                    {
                        objResult.Success = false;
                        objResult.Message = "No hay filas para exportar";
                    }
                    else
                    {
                        cadena = get_html_ListarConsultaKPI_Detalle_str((List<Ent_ConsultaKPI_Detalle>)Session[_session_ListarConsultaKPI_Detalle], _Ent);
                        if (cadena.Length == 0)
                        {
                            objResult.Success = false;
                            objResult.Message = "Error del formato html";
                        }
                        else
                        {
                            objResult.Success = true;
                            objResult.Message = "Se genero el excel correctamente";
                            Session[_session_ListarConsultaKPI_Detalle_Excel] = cadena;
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

        public string get_html_ListarConsultaKPI_Detalle_str(List<Ent_ConsultaKPI_Detalle> _ListarConsultaKPI_Detalle, Ent_ConsultaKPI_Detalle _Ent)
        {
            StringBuilder sb = new StringBuilder();
            var Lista = _ListarConsultaKPI_Detalle.ToList();
            try
            {
                sb.Append("<div>");
                sb.Append("<table cellspacing='0' style='width: 1000px' rules='all' border='0' style='border-collapse:collapse;'>");
                sb.Append("<tr><td Colspan='10'></td></tr>");
                sb.Append("<tr><td Colspan='10' valign='middle' align='center' style='vertical-align: middle;font-size: 18.0pt;font-weight: bold;color:#285A8F'>REPORTE DE RESULTADOS KPI</td></tr>");
                sb.Append("<tr><td Colspan='10' valign='middle' align='center' style='vertical-align: middle;font-size: 10.0pt;font-weight: bold;color:#000000'>Desde el " + String.Format("{0:dd/MM/yyyy}", _Ent.FechaInicio) + " hasta el " + String.Format("{0:dd/MM/yyyy}", _Ent.FechaFin) + "</td></tr>");//subtitulo
                sb.Append("<tr>\n");
                if (!_Ent.IsAseOrLider)
                {
                    sb.Append("<th bgColor='#1E77AB' style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Lider</font></th>\n");
                }               
                sb.Append("<th bgColor='#1E77AB' style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Asesor</font></th>\n");
                sb.Append("<th bgColor='#1E77AB' style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Anio</font></th>\n");
                sb.Append("<th bgColor='#1E77AB' style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Mes</font></th>\n");
                sb.Append("<th bgColor='#1E77AB' style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Facturacion</font></th>\n");
                sb.Append("<th bgColor='#1E77AB' style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Margen</font></th>\n");
                sb.Append("<th bgColor='#1E77AB' style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Continuas</font></th>\n");
                sb.Append("<th bgColor='#1E77AB' style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Afiliadas</font></th>\n");
                sb.Append("<th bgColor='#1E77AB' style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Reactivadas</font></th>\n");
                sb.Append("<th bgColor='#1E77AB' style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Activasenmes</font></th>\n");
                sb.Append("<th bgColor='#1E77AB' style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Desactivadas</font></th>\n");
                sb.Append("<th bgColor='#1E77AB' style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>% Desact</font></th>\n");
                sb.Append("<th bgColor='#1E77AB' style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Reg_Mes</font></th>\n");
                sb.Append("<th bgColor='#1E77AB' style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>TactRegMes</font></th>\n");
                sb.Append("<th bgColor='#1E77AB' style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>% Afiliadas Mes</font></th>\n");
                sb.Append("<th bgColor='#1E77AB' style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Activas De Otro Mes</font></th>\n");
                sb.Append("<th bgColor='#1E77AB' style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Total Activas</font></th>\n");
                sb.Append("<th bgColor='#1E77AB' style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Ticket Prom</font></th>\n"); ;
                sb.Append("</tr>\n");
                // {0:N2} Separacion miles , {0:F2} solo dos decimales
                foreach (var item in Lista)
                {
                    sb.Append("<tr>\n");
                    if (!_Ent.IsAseOrLider)
                    {
                        sb.Append("<td align=''>" + item.Lider + "</td>\n");
                    }                    
                    sb.Append("<td align=''>" + item.Asesor + "</td>\n");
                    sb.Append("<td align='Center'>" + item.Anio + "</td>\n");
                    sb.Append("<td align='Center'>" + item.Mes + "</td>\n");
                    sb.Append("<td align='Right'>" + item.Facturacion + "</td>\n");
                    sb.Append("<td align='Right'>" + item.Margen + "</td>\n");
                    sb.Append("<td align='Right'>" + item.Continuas + "</td>\n");
                    sb.Append("<td align='Right'>" + item.Afiliadas + "</td>\n");
                    sb.Append("<td align='Right'>" + item.Reactivadas + "</td>\n");
                    sb.Append("<td align='Right'>" + item.Activasenmes + "</td>\n");
                    sb.Append("<td align='Right'>" + item.Desactivadas + "</td>\n");
                    sb.Append("<td align='Right'>" + item.PorDesact + "</td>\n");
                    sb.Append("<td align='Right'>" + item.Reg_Mes + "</td>\n");
                    sb.Append("<td align='Right'>" + item.TactRegMes + "</td>\n");
                    sb.Append("<td align='Right'>" + item.PorAfiliadasMes + "</td>\n");
                    sb.Append("<td align='Right'>" + item.ActivasOtroMes + "</td>\n");
                    sb.Append("<td align='Right'>" + item.TotalActivas + "</td>\n");
                    sb.Append("<td align='Right'>" + item.TicketProm + "</td>\n");
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
        public ActionResult ListarConsultaKPI_DetalleExcel()
        {
            string NombreArchivo = "ResultadoKPI_Detalle";
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
                Response.Write(Session[_session_ListarConsultaKPI_Detalle_Excel].ToString());
                Response.End();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return Json(new { estado = 0, mensaje = 1 });
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

                        if (column.ColumnName == "concepto")
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
        public JsonResult getLisConsultaKPIAjax(Ent_jQueryDataTableParams param, bool isOkUpdate, string FechaInicio, string FechaFin, string IdAsesor, string IdLider)
        {
            Ent_ConsultaKPI EntConsultaKPIDetalle = new Ent_ConsultaKPI();
            Decimal Enero = 0, Febrero = 0, Marzo = 0, Abril = 0, Mayo = 0, Junio = 0, Julio = 0, Agosto = 0, Septiembre = 0, Octubre = 0, Noviembre = 0, Diciembre = 0;
            if (isOkUpdate)
            {
                EntConsultaKPIDetalle.IdAsesor = IdAsesor;
                EntConsultaKPIDetalle.IdLider = IdLider;
                EntConsultaKPIDetalle.FechaInicio = DateTime.Parse(FechaInicio);
                EntConsultaKPIDetalle.FechaFin = DateTime.Parse(FechaFin);

                DataTable dtListarConsultaKPI = datRRHH.ListarConsultaKPI(EntConsultaKPIDetalle);

                List<Ent_ConsultaKPI> _ListarConsultaKPI = new List<Ent_ConsultaKPI>();
                _ListarConsultaKPI = ConvertDataTable<Ent_ConsultaKPI>(dtListarConsultaKPI).ToList();
                Session[_session_ListarConsultaKPI] = _ListarConsultaKPI;
            }

            /*verificar si esta null*/
            if (Session[_session_ListarConsultaKPI] == null)
            {
                List<Ent_ConsultaKPI> _ListarConsultaKPI = new List<Ent_ConsultaKPI>();
                Session[_session_ListarConsultaKPI] = _ListarConsultaKPI;
            }

            IQueryable<Ent_ConsultaKPI> entDocTrans = ((List<Ent_ConsultaKPI>)(Session[_session_ListarConsultaKPI])).AsQueryable();
            //Manejador de filtros
            int totalCount = entDocTrans.Count();

            if (totalCount > 0)
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

            IEnumerable<Ent_ConsultaKPI> filteredMembers = entDocTrans;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = entDocTrans.Where(
                        m =>
                            m.concepto.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
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
                            m.Diciembre.ToString().ToUpper().Contains(param.sSearch.ToUpper())                           
                );
            }
            var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);

            if (sortIdx!=0)
            {
                if (param.iSortingCols > 0)
                {
                    if (Request["sSortDir_0"].ToString() == "asc")
                    {
                        switch (sortIdx)
                        {
                            case 0: filteredMembers = filteredMembers.OrderBy(o => o.concepto); break;
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
                        }
                    }
                    else
                    {
                        switch (sortIdx)
                        {
                            case 0: filteredMembers = filteredMembers.OrderByDescending(o => o.concepto); break;
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
                        }
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

        public ActionResult get_exporta_LisConsultaKPI_excel(Ent_ConsultaKPI _Ent)
        {
            JsonResponse objResult = new JsonResponse();
            try
            {
                Session[_session_ListarConsultaKPI_Excel] = null;
                string cadena = "";
                if (Session[_session_ListarConsultaKPI] != null)
                {

                    List<Ent_ConsultaKPI> _ListarConsultaKPI = (List<Ent_ConsultaKPI>)Session[_session_ListarConsultaKPI];
                    if (_ListarConsultaKPI.Count == 0)
                    {
                        objResult.Success = false;
                        objResult.Message = "No hay filas para exportar";
                    }
                    else
                    {
                        cadena = get_html_ListarConsultaKPI_str((List<Ent_ConsultaKPI>)Session[_session_ListarConsultaKPI], _Ent);
                        if (cadena.Length == 0)
                        {
                            objResult.Success = false;
                            objResult.Message = "Error del formato html";
                        }
                        else
                        {
                            objResult.Success = true;
                            objResult.Message = "Se genero el excel correctamente";
                            Session[_session_ListarConsultaKPI_Excel] = cadena;
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

        public string get_html_ListarConsultaKPI_str(List<Ent_ConsultaKPI> _ListarConsultaKPI, Ent_ConsultaKPI _Ent)
        {
            StringBuilder sb = new StringBuilder();
            Decimal Enero = 0, Febrero = 0, Marzo = 0, Abril = 0, Mayo = 0, Junio = 0, Julio = 0, Agosto = 0, Septiembre = 0, Octubre = 0, Noviembre = 0, Diciembre = 0;
            var TCS = 0; //Tamaño de Colspan
            var Lista = _ListarConsultaKPI.ToList();

            Enero = _ListarConsultaKPI.Sum(x => x.Enero);
            TCS += (Enero == 0 ? 0 : 1);
            Febrero = _ListarConsultaKPI.Sum(x => x.Febrero);
            TCS += (Febrero == 0 ? 0 : 1);
            Marzo = _ListarConsultaKPI.Sum(x => x.Marzo);
            TCS += (Marzo == 0 ? 0 : 1);
            Abril = _ListarConsultaKPI.Sum(x => x.Abril);
            TCS += (Abril == 0 ? 0 : 1);
            Mayo = _ListarConsultaKPI.Sum(x => x.Mayo);
            TCS += (Mayo == 0 ? 0 : 1);
            Junio = _ListarConsultaKPI.Sum(x => x.Junio);
            TCS += (Junio == 0 ? 0 : 1);
            Julio = _ListarConsultaKPI.Sum(x => x.Julio);
            TCS += (Julio == 0 ? 0 : 1);
            Agosto = _ListarConsultaKPI.Sum(x => x.Agosto);
            TCS += (Agosto == 0 ? 0 : 1);
            Septiembre = _ListarConsultaKPI.Sum(x => x.Septiembre);
            TCS += (Septiembre == 0 ? 0 : 1);
            Octubre = _ListarConsultaKPI.Sum(x => x.Octubre);
            TCS += (Octubre == 0 ? 0 : 1);
            Noviembre = _ListarConsultaKPI.Sum(x => x.Noviembre);
            TCS += (Noviembre == 0 ? 0 : 1);
            Diciembre = _ListarConsultaKPI.Sum(x => x.Diciembre);
            TCS += (Diciembre == 0 ? 0 : 1);
            try
            {
                sb.Append("<div>");
                sb.Append("<table cellspacing='0' style='width: 1000px' rules='all' border='0' style='border-collapse:collapse;'>");
                sb.Append("<tr><td Colspan=" + (TCS + 1) + "></td></tr>");
                sb.Append("<tr><td Colspan=" + (TCS + 1) + " valign='middle' align='center' style='vertical-align: middle;font-size: 18.0pt;font-weight: bold;color:#285A8F'>REPORTE DE RESULTADOS KPI</td></tr>");
                sb.Append("<tr><td Colspan=" + (TCS + 1) + " valign='middle' align='center' style='vertical-align: middle;font-size: 10.0pt;font-weight: bold;color:#000000'>Rango de fechas : del " + String.Format("{0:dd/MM/yyyy}", _Ent.FechaInicio) + " hasta " + String.Format("{0:dd/MM/yyyy}", _Ent.FechaFin) + "</td></tr>");//subtitulo
                sb.Append("</table>");
                sb.Append("<table  border='1' bgColor='#ffffff' borderColor='#FFFFFF' cellSpacing='2' cellPadding='2' style='font-size:10.0pt; font-family:Calibri; background:white;width: 1000px'><tr  bgColor='#5799bf'>\n");
                sb.Append("<tr bgColor='#1E77AB'>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Lider</font></th>\n");

                if (Enero > 0)
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
                
                sb.Append("</tr>\n");
                // {0:N2} Separacion miles , {0:F2} solo dos decimales
                foreach (var item in Lista)
                {
                    sb.Append("<tr>\n");
                    sb.Append("<td bgcolor='' align=''>" + item.concepto + "</td>\n");
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
                    sb.Append("</tr>\n");
                }
                sb.Append("<tfoot>\n");
                sb.Append("<tr bgcolor='#085B8C'>\n");
                sb.Append("</tr>\n");
                sb.Append("</tfoot>\n");
                sb.Append("</table></div>");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return sb.ToString();
        }
        public ActionResult ListarConsultaKPIExcel()
        {
            string NombreArchivo = "ResultadoKPI";
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
                Response.Write(Session[_session_ListarConsultaKPI_Excel].ToString());
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