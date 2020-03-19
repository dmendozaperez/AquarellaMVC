using CapaDato.Financiera;
using CapaDato.Pedido;
using CapaDato.Persona;
using CapaEntidad.Control;
using CapaEntidad.Financiera;
using CapaEntidad.General;
using CapaEntidad.Pedido;
using CapaEntidad.Persona;
using CapaEntidad.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CapaPresentacion.Controllers
{
    public class FinancieraController : Controller
    {
        private Dat_Pedido datPedido = new Dat_Pedido();
        private Dat_Persona datPersona = new Dat_Persona();
        private Dat_Financiera datFinanciera = new Dat_Financiera();

        private string _sessionPagsLiqs = "_SessionPagsLiqs";

        // GET: Financiera
        public ActionResult Index()
        {
            return View();
        }
        #region Cruces

        public ActionResult CrucePago()
        {
            Session[_sessionPagsLiqs] = null;
            Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
            Ent_Pedido_Maestro maestros = datPedido.Listar_Maestros_Pedido(_usuario.usu_id, _usuario.usu_postPago, "");
            ViewBag.listPromotor = maestros.combo_ListPromotor;
            return View();
        }
        public ActionResult GET_INFO_PERSONA(string codigo)
        {
            try
            {
                Ent_Persona info = datPersona.GET_INFO_PERSONA(codigo);
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
                             document_date_desc = a.document_date_desc.ToString("dd/MM/yyyy hh:mm"),
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
        public ActionResult GuardarCruce()
        {
            string _mensaje = "";
            if (Session[_sessionPagsLiqs] == null)
            {
                List<Ent_Pag_Liq> listPed = new List<Ent_Pag_Liq>();
                Session[_sessionPagsLiqs] = listPed;
            }
            List<Ent_Pag_Liq> list = (List<Ent_Pag_Liq>)Session[_sessionPagsLiqs];
            list = list.Where(w => w.checks).ToList();

            int countLiqSel = list.Where(w => w.dtv_concept_id == "LIQUIDACIONES").Count();

            if (list.Count == 0)
            {
                _mensaje = "No ha seleccionado ningun item.";
            }
            if (countLiqSel > 1)
            {
                _mensaje = "No se puede realizar cruce de pagos con 2 o más Pedidos, por favor seleccione solo 1 pedido.";
            }
            if (countLiqSel == 0)
            {
                _mensaje = "no ha seleccionado ningun pedido para cruzar el pago";
            }
            
           

        }
        #endregion
    }    
}