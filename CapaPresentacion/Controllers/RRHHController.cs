using CapaEntidad.General;
using CapaEntidad.Control;
using CapaEntidad.Util;
using CapaEntidad.RRHH;
using CapaDato.Util;
using CapaDato.RRHH;
using CapaDato.Facturacion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using Newtonsoft.Json;

namespace CapaPresentacion.Controllers
{
    public class RRHHController : Controller
    {
        private Dat_Util datUtil = new Dat_Util();
        private Dat_RRHH datRRHH = new Dat_RRHH();

        private string _session_ListarPromotor_Lider = "_session_ListarPromotor_Lider";
        private string _session_ListarPromotor_Lider_Excel = "_session_ListarPromotor_Lider_Excel";

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
                sb.Append("<tr><td Colspan='10' valign='middle' align='center' style='vertical-align: middle;font-size: 18.0pt;font-weight: bold;color:#285A8F'>REPORTE DE VENTAS POR LIDER</td></tr>");
                sb.Append("<tr><td Colspan='10' valign='middle' align='center' style='vertical-align: middle;font-size: 10.0pt;font-weight: bold;color:#000000'>Desde el " + String.Format("{0:dd/MM/yyyy}", _Ent.FechaInicio) + " hasta el " + String.Format("{0:dd/MM/yyyy}", _Ent.FechaFin) + "</td></tr>");//subtitulo
                sb.Append("<tr bgColor='#1E77AB'>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Asesor</font></th>\n");
                sb.Append("<th style='text-align: center; font-weight:bold;font-size:11.0pt;'><font color='#FFFFFF'>Lider</font></th>\n");
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
    }
}