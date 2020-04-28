using CapaDato.Devolucion;
using CapaDato.Pedido;
using CapaDato.Persona;
using CapaEntidad.Devolucion;
using CapaEntidad.General;
using CapaEntidad.Pedido;
using CapaEntidad.Persona;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CapaPresentacion.Controllers
{
    public class DevolucionController : Controller
    {
        // GET: Devolucion
        private Dat_Pedido datPedido = new Dat_Pedido();
        private Dat_Documentos_Dev dat_dev = new Dat_Documentos_Dev();
        private Dat_Persona datPersona = new Dat_Persona();
        private string _session_listDocDev_private = "_session_listDocDev_private";
        private string _session_listDocDev_det_new_private = "_session_listDocDev_det_new_private";
        public ActionResult Index()
        {

            Session[_session_listDocDev_private] = null;

            Ent_Pedido_Maestro maestros = datPedido.Listar_Maestros_Pedido(1,"", "");
            ViewBag.listPromotor = maestros.combo_ListPromotor;

            return View();
        }
        public ActionResult GET_INFO_PERSONA_DEVOLUCION(string codigo)
        {
            try
            {
                Ent_Persona info = datPersona.GET_INFO_PERSONA(codigo);
                string _mensaje = "";
                Ent_Info_Devolucion infoGeneralDevolucion = dat_dev.ListarDevolucion(Convert.ToDecimal(codigo), ref _mensaje);
                Session[_session_listDocDev_private] = infoGeneralDevolucion.documentosDev;
                //Session[_session_list_NotaCredito] = infoGeneralPedidos.notaCredito;
                //Session[_session_list_consignaciones] = infoGeneralPedidos.consignaciones;
                //Session[_session_list_saldos] = infoGeneralPedidos.saldos;

                return Json(new { estado = 0, info = info, mensaje = _mensaje });
            }
            catch (Exception ex)
            {
                return Json(new { estado = 2, mensaje = ex.Message });
            }
        }
        /** Lista Documentos **/
        public ActionResult getListDocumentosDev(Ent_jQueryDataTableParams param)
        {
            /*verificar si esta null*/
            if (Session[_session_listDocDev_private] == null)
            {
                List<Ent_Documentos_Dev> listPed = new List<Ent_Documentos_Dev>();
                Session[_session_listDocDev_private] = listPed;
            }

            //Traer registros
            IQueryable<Ent_Documentos_Dev> membercol = ((List<Ent_Documentos_Dev>)(Session[_session_listDocDev_private])).AsQueryable();  //lista().AsQueryable();

            //Manejador de filtros
            int totalCount = membercol.Count();
            IEnumerable<Ent_Documentos_Dev> filteredMembers = membercol;

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = membercol
                    .Where(m => m.TIPODOC.ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.NDOC.ToUpper().Contains(param.sSearch.ToUpper()));
            }
            //Manejador de orden
            var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);

            if (param.iSortingCols > 0)
            {
                if (Request["sSortDir_0"].ToString() == "asc")
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderBy(o =>o.TIPODOC); break;
                        case 1: filteredMembers = filteredMembers.OrderBy(o => o.NDOC); break;
                        case 2: filteredMembers = filteredMembers.OrderBy(o => Convert.ToDateTime(o.FDOC)).ThenBy(to => Convert.ToDecimal(to.FDOC)); break;
                        //case 3: filteredMembers = filteredMembers.OrderBy(o => o.Pares); break;
                        //case 4: filteredMembers = filteredMembers.OrderBy(o => o.Estado); break;
                        //case 5: filteredMembers = filteredMembers.OrderBy(o => o.Ganancia); break;
                        //case 6: filteredMembers = filteredMembers.OrderBy(o => o.Subtotal); break;
                        //case 7: filteredMembers = filteredMembers.OrderBy(o => o.N_C); break;
                        //case 8: filteredMembers = filteredMembers.OrderBy(o => o.Total); break;
                        //case 9: filteredMembers = filteredMembers.OrderBy(o => o.Percepcion); break;
                        //case 10: filteredMembers = filteredMembers.OrderBy(o => o.TotalPagar); break;
                    }
                }
                else
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderByDescending(o => o.TIPODOC); break;
                        case 1: filteredMembers = filteredMembers.OrderByDescending(o => o.NDOC); break;
                        case 2: filteredMembers = filteredMembers.OrderByDescending(o => Convert.ToDateTime(o.FDOC)).ThenBy(to => Convert.ToDecimal(to.FDOC)); break;
                        //case 3: filteredMembers = filteredMembers.OrderByDescending(o => o.Pares); break;
                        //case 4: filteredMembers = filteredMembers.OrderByDescending(o => o.Estado); break;
                        //case 5: filteredMembers = filteredMembers.OrderByDescending(o => o.Ganancia); break;
                        //case 6: filteredMembers = filteredMembers.OrderByDescending(o => o.Subtotal); break;
                        //case 7: filteredMembers = filteredMembers.OrderByDescending(o => o.N_C); break;
                        //case 8: filteredMembers = filteredMembers.OrderByDescending(o => o.Total); break;
                        //case 9: filteredMembers = filteredMembers.OrderByDescending(o => o.Percepcion); break;
                        //case 10: filteredMembers = filteredMembers.OrderByDescending(o => o.TotalPagar); break;
                    }
                }
            }
            var displayMembers = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);
            var result = from a in displayMembers
                         select new
                         {
                             a.TIPODOC,
                             a.NDOC,
                             a.FDOC,
                             a.SUBTOTAL,
                             a.IGV,
                             a.TOTAL,                             
                         };
            //Se devuelven los resultados por json
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalCount,
                iTotalDisplayRecords = filteredMembers.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult genera_nuevo_dev(string nro_doc)
        {
            string _mensaje = "";
            List<Ent_Documentos_Dev_Det_New> listdoc_det = dat_dev.get_dev_det_new(nro_doc,ref _mensaje);
            
            Session[_session_listDocDev_det_new_private] = listdoc_det;

            return PartialView(listdoc_det);
        }
        public ActionResult getTableconsdocDetNewAjax(Ent_jQueryDataTableParams param)
        {
            /*verificar si esta null*/
            if (Session[_session_listDocDev_det_new_private] == null)
            {
                List<Ent_Documentos_Dev_Det_New> listdoc_det = new List<Ent_Documentos_Dev_Det_New>();
                Session[_session_listDocDev_det_new_private] = listdoc_det;
            }
            //if (!String.IsNullOrEmpty(dniEliminar))
            //{
            //    List<Ent_BataClub_Cupones> listAct = (List<Ent_BataClub_Cupones>)(Session[_session_lista_clientes_cupon]);
            //    listAct.Remove(listAct.Where(w => w.dniCliente == dniEliminar).FirstOrDefault());
            //    Session[_session_lista_clientes_cupon] = listAct;
            //}
            //Traer registros
            IQueryable<Ent_Documentos_Dev_Det_New> membercol = ((List<Ent_Documentos_Dev_Det_New>)(Session[_session_listDocDev_det_new_private])).AsQueryable();

            //Manejador de filtros
            int totalCount = membercol.Count();

            IEnumerable<Ent_Documentos_Dev_Det_New> filteredMembers = membercol;

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = membercol
                    .Where(m =>
                    m.ARTICULO.ToUpper().Contains(param.sSearch.ToUpper())                                         
                    );
            }

            //Manejador de orden
            var displayMembers = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);

            var result = from a in displayMembers
                         select new
                         {
                             a.ARTICULO,
                             a.TALLA,
                             a.CANTIDAD                             
                             //a.talla,
                             //a.cantidad,
                         };
            //Se devuelven los resultados por json
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalCount,
                iTotalDisplayRecords = filteredMembers.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
    }
}