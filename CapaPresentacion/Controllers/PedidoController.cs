using CapaDato.Util;
using CapaDato.Persona;
using CapaDato.Pedido;
using CapaEntidad.Control;
using CapaEntidad.Menu;
using CapaEntidad.Util;
using CapaEntidad.Pedido;
using CapaPresentacion.Bll;
using System;
using System.Collections.Generic;

using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Data;
using CapaEntidad.General;
using System.Linq;

namespace CapaPresentacion.Controllers
{
    public class PedidoController : Controller
    {
        // GET: Funcion
        private Dat_Pedido datPedido = new Dat_Pedido();
        private string _session_listPedido_private = "_session_listPedido_private";

        [Authorize]
        public ActionResult Nuevo()
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
                    Ent_Pedido_Maestro maestros = datPedido.Listar_Maestros_Pedido(_usuario.usu_id, _usuario.usu_postPago);
                                

                    ViewBag.listPromotor = maestros.combo_ListPromotor;
                    ViewBag.listFormaPago = maestros.combo_ListFormaPago;                 


                    return View();
                }
                else
                {
                    return RedirectToAction("Login", "Control", new { returnUrl = return_view });
                }
            }

        }

        public ActionResult PersonaPedido(int basId) {
            
            Ent_Pedido_Persona Persona = new Ent_Pedido_Persona();
            Persona = datPedido.BuscarPersonaPedido(basId);

            return Json(Persona, JsonRequestBehavior.AllowGet);
        }

        public string listarStr_ArticuloTalla(string codArticulo)
        {

            string strJson = "";
                       
            Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
            
            JsonResult jRespuesta = null;
            strJson = datPedido.listarStr_ArticuloTalla(codArticulo, _usuario.usu_id);
            var serializer = new JavaScriptSerializer();
            jRespuesta = Json(serializer.Deserialize<List<Ent_Articulo_pedido>>(strJson), JsonRequestBehavior.AllowGet);           

            return strJson;
        }

        public JsonResult GenerarCombo(int Numsp, string Params)
        {
            string strJson = "";
            JsonResult jRespuesta = null;
            var serializer = new JavaScriptSerializer();


            //switch (Numsp)
            //{
            //    case 1:
            //        strJson = datUtil.listarStr_Provincia(Params);
            //        jRespuesta = Json(serializer.Deserialize<List<Ent_Combo>>(strJson), JsonRequestBehavior.AllowGet);
            //        break;
            //    case 2:
            //        String[] substrings = Params.Split('|');
            //        strJson = datUtil.listarStr_Distrito(Params);
            //        jRespuesta = Json(serializer.Deserialize<List<Ent_Combo>>(strJson), JsonRequestBehavior.AllowGet);
            //        break;
            //    default:
            //        Console.WriteLine("Default case");
            //        break;
            //}
            return jRespuesta;
        }

        private static string[] splitString(string _textString, char _character)
        {
            string[] split = null;
            if (!string.IsNullOrEmpty(_textString))
            {
                split = _textString.Split(new Char[] { _character });
            }
            return split;
        }

        //public ActionResult LiquidarPedido(decimal _usu, decimal _idCust, string _reference, decimal _discCommPctg,
        //                                       decimal _discCommValue, string _shipTo, string _specialInstr, List<Ent_Order_Dtl> _itemsDetail,
        //                                       decimal _varpercepcion, Int32 _estado, string _ped_id = "", string _liq = "", Int32 _liq_dir = 0,
        //                                       Int32 _PagPos = 0, string _PagoPostarjeta = "", string _PagoNumConsignacion = "", decimal _PagoTotal = 0,
        //                                       DataTable dtpago = null, Boolean _pago_credito = false, Decimal _porc_percepcion = 0, List<Order_Dtl_Temp>
        //                                        order_dtl_temp = null, string strTipoPago = "N")
        //{

        //    var oJRespuesta = new JsonResponse();


        //    return Json(oJRespuesta, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult LiquidarPedido(decimal _usu = 0, decimal _idCust = 0, string _reference = "", decimal _discCommPctg = 0,
                                       decimal _discCommValue = 0, string _shipTo = "", string _specialInstr = "", List<Ent_Order_Dtl> _itemsDetail = null,
                                       decimal _varpercepcion = 0, Int32 _estado = 0, string _ped_id = "", string _liq = "", Int32 _liq_dir = 0,
                                       Int32 _PagPos = 0, string _PagoPostarjeta = "", string _PagoNumConsignacion = "", decimal _PagoTotal = 0,
                                       DataTable dtpago = null, Boolean _pago_credito = false, Decimal _porc_percepcion = 0, List<Order_Dtl_Temp>
                                        order_dtl_temp = null, string strTipoPago = "N")
        {

            string[] noOrder;
            Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];

            noOrder = datPedido.Gua_Mod_Liquidacion(_usuario.usu_id, _idCust, string.Empty, _discCommPctg, 0, string.Empty, string.Empty, _itemsDetail, _varpercepcion,
                                                    _estado, _ped_id, _liq, _liq_dir, _PagPos, _PagoPostarjeta, _PagoNumConsignacion, _PagoTotal, dtpago, _pago_credito,
                                                    _porc_percepcion, order_dtl_temp, strTipoPago);

            var oJRespuesta = new JsonResponse();


            return Json(oJRespuesta, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ListarPedido()
        {
            Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
            string actionName = this.ControllerContext.RouteData.GetRequiredString("action");
            string controllerName = this.ControllerContext.RouteData.GetRequiredString("controller");
            string return_view = actionName + "|" + controllerName;

            if (_usuario == null)
            {
                return RedirectToAction("Login", "Control", new { returnUrl = return_view });
            }

            Ent_Pedido_Maestro maestros = datPedido.Listar_Maestros_Pedido(_usuario.usu_id, _usuario.usu_postPago);
            ViewBag.listPromotor = maestros.combo_ListPromotor;
            //Ent_Promotor_Maestros maestros = datUtil.ListarEnt_Maestros_Promotor(_usuario.usu_id);
            //ViewBag.listLider = maestros.combo_ListLider;

            return View();
        }


        public List<Ent_Pedido> lista(decimal IdPromotor)
        {       

            List<Ent_Pedido> listPedido = datPedido.ListarPedidos(IdPromotor);
            Session[_session_listPedido_private] = listPedido;
            return listPedido;
        }

        public ActionResult getListPedido(Ent_jQueryDataTableParams param)
        {

            /*verificar si esta null*/
            if (Session[_session_listPedido_private] == null)
            {
                List<Ent_Pedido> listPed = new List<Ent_Pedido>();
                Session[_session_listPedido_private] = listPed;
            }

            //Traer registros
            IQueryable<Ent_Pedido> membercol = ((List<Ent_Pedido>)(Session[_session_listPedido_private])).AsQueryable();  //lista().AsQueryable();

            //Manejador de filtros
            int totalCount = membercol.Count();
            IEnumerable<Ent_Pedido> filteredMembers = membercol;

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = membercol
                    .Where(m => m.liq_PedId.ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.liq_PedId.ToUpper().Contains(param.sSearch.ToUpper()));
            }
            //Manejador de orden
            var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);
            Func<Ent_Pedido, string> orderingFunction =
            (
            m => sortIdx == 0 ? m.liq_PedId :
             m.liq_PedId
            );
            var sortDirection = Request["sSortDir_0"];
            //if (sortDirection == "asc")
            //    filteredMembers = filteredMembers.OrderBy(orderingFunction);
            //else
            //    filteredMembers = filteredMembers.OrderByDescending(orderingFunction);
            var displayMembers = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);
            var result = from a in displayMembers
                         select new
                         {
                             a.liq_PedId,
                             a.liq_Fecha,
                             a.Pares,
                             a.Estado,
                             a.Ganancia,
                             a.Subtotal,
                             a.N_C,
                             a.Total,
                             a.Percepcion,
                             a.TotalPagar,
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