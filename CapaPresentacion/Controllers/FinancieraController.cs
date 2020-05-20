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
        private string _sessin_customer = "_sessin_customer";

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
        public ActionResult GuardarCruce()
        {
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
            }
            if (countLiqSel > 1)
            {
                _mensaje = "No se puede realizar cruce de pagos con 2 o más Pedidos, por favor seleccione solo 1 pedido.";
            }
            if (countLiqSel == 0)
            {
                _mensaje = "no ha seleccionado ningun pedido para cruzar el pago";
            }
            if (countPagSel > 1)
            {
                decimal _sum_pag = 0;
                decimal _liq_val = list.Where(w => w.dtv_concept_id == "LIQUIDACIONES").Select(s => s.val).FirstOrDefault();
                
                foreach (Ent_Pag_Liq item in list.OrderByDescending(o=> o.val))
                {
                    if ( item.dtv_concept_id == "PAGOS")
                    {                        
                        _sum_pag += item.val;
                        if (_sum_pag > _liq_val)
                        {
                            _mensaje = "por favor solo seleccione el pago necesario para pagar su pedido";
                            break;
                        }
                    }
                }                    
             }
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
                    }
                    foreach (Ent_Pag_Liq item in list.Where(w=>w.dtv_concept_id == "PAGOS"))
                    {
                        dtpagos.Rows.Add(item.dtv_transdoc_id);
                    }
                    
                    string strIdPromotor = cust.Bas_id;
                    string clear = datFinanciera.setPreClear(listLiq, dtpagos);

                    if (!String.IsNullOrEmpty(clear))
                    {
                        string[] prems = clear.Split('|');
                        string strpremio = prems[1].ToString();
                        string strpremio2 = prems[2].ToString();
                        string strmensaje = "";
                        string strmensajePremio = "";

                        if (strpremio != "N" && strpremio != "0")
                        {
                            string strIdLiquidacion = datFinanciera.setCrearLiquidacionPremio(Convert.ToInt32(strIdPromotor), Convert.ToInt32(strpremio), "C");
                            strmensajePremio = "Premio generado en el pedido:" + strIdLiquidacion + " ";
                        }

                        if (strpremio2 != "N" && strpremio2 != "0")
                        {
                            string strIdLiquidacion = datFinanciera.setCrearLiquidacionPremio(Convert.ToInt32(strIdPromotor), Convert.ToInt32(strpremio2), "P");
                            string cadena = "";
                            if (strmensajePremio != "") { cadena = "y"; }

                            strmensajePremio = strmensajePremio + " " + cadena + " en el pedido:" + strIdLiquidacion + " ";
                        }

                        if (strmensajePremio != "") { strmensajePremio = " ( " + strmensajePremio + " ) "; }

                        _mensaje = "El cruce de información fue grabado correctamente " + strmensajePremio + ", su pedido sera enviado  marcación y posterior facturación; número del cruce: " + prems[0].ToString() + strmensaje;
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
    }    
}