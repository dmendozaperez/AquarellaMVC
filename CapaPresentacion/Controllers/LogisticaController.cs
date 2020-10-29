using CapaDato.Logistica;
using CapaEntidad.Control;
using CapaEntidad.General;
using CapaEntidad.Logistica;
using CapaEntidad.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CapaPresentacion.Controllers
{
    public class LogisticaController : Controller
    {       
        private Dat_Despacho  dat_despacho = new Dat_Despacho();

        #region<REGION DE CONSULTA DE DESPACHO>
        private string _session_listDespacho_private = "_session_listDespacho_private";
        // GET: Logistica
        public ActionResult ListaDespacho()
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
                List<Ent_Tipo_Despacho> lista_tipo = new List<Ent_Tipo_Despacho>();
                Ent_Tipo_Despacho tip = new Ent_Tipo_Despacho();
                //tip.tip_des_cod = "L";
                //tip.tip_des_nom = "Lima-Callao";
                //lista_tipo.Add(tip);

                //tip = new Ent_Tipo_Despacho();
                tip.tip_des_cod = "P";
                tip.tip_des_nom = "Provincia";
                lista_tipo.Add(tip);

                ViewBag.Tipo = lista_tipo;

                return View();
            }
          
        }      
        public List<Ent_Lista_Despacho> lista(string fecha_ini,string fecha_fin,string tipo_des)
        {            
            List<Ent_Lista_Despacho> listdespacho = dat_despacho.get_lista_despacho(Convert.ToDateTime(fecha_ini), Convert.ToDateTime(fecha_fin), tipo_des);            
            Session[_session_listDespacho_private] = listdespacho;
            return listdespacho;
        }
        public ActionResult getListDespachoAjax(Ent_jQueryDataTableParams param, string actualizar,string fecha_ini,string fecha_fin,string tipo_des)
        {

            List<Ent_Lista_Despacho> listdespacho = new List<Ent_Lista_Despacho>();

            if (!String.IsNullOrEmpty(actualizar))
            {
                listdespacho = lista(fecha_ini, fecha_fin, tipo_des);
                //listAtributos = datOE.get_lista_atributos();
                Session[_session_listDespacho_private] = listdespacho;
            }

                /*verificar si esta null*/
            if (Session[_session_listDespacho_private] == null)
            {
             
                listdespacho = new List<Ent_Lista_Despacho>();
             
                Session[_session_listDespacho_private] = listdespacho;
            }


            //Traer registros
            IQueryable<Ent_Lista_Despacho> membercol = ((List<Ent_Lista_Despacho>)(Session[_session_listDespacho_private])).AsQueryable();  //lista().AsQueryable();

            //Manejador de filtros
            int totalCount = membercol.Count();
            IEnumerable<Ent_Lista_Despacho> filteredMembers = membercol;

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = membercol
                    .Where(m => m.desp_nrodoc.ToUpper().Contains(param.sSearch.ToUpper()));
            }
            //Manejador de orden
            var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);

            if (param.iSortingCols > 0)
            {
                if (Request["sSortDir_0"].ToString() == "asc")
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderBy(o => o.desp_nrodoc); break;
                        case 1: filteredMembers = filteredMembers.OrderBy(o => o.desp_descripcion); break;
                        case 2: filteredMembers = filteredMembers.OrderBy(o => Convert.ToDateTime(o.desp_fechacre)); break;
                        case 3: filteredMembers = filteredMembers.OrderBy(o => o.totalparesenviado); break;
                        case 4: filteredMembers = filteredMembers.OrderBy(o => o.estado); break;
                        
                    }
                }
                else
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderByDescending(o => o.desp_nrodoc); break;
                        case 1: filteredMembers = filteredMembers.OrderByDescending(o => o.desp_descripcion); break;
                        case 2: filteredMembers = filteredMembers.OrderByDescending(o => Convert.ToDateTime(o.desp_fechacre)); break;
                        case 3: filteredMembers = filteredMembers.OrderByDescending(o => o.totalparesenviado); break;
                        case 4: filteredMembers = filteredMembers.OrderByDescending(o => o.estado); break;                        
                    }
                }
            }
            var displayMembers = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);


            var result = from a in displayMembers
                         select new
                         {
                             a.desp_nrodoc,
                             a.desp_descripcion,
                             a.desp_fechacre,
                             a.totalparesenviado,
                             a.estado,  
                             a.desp_id,                           
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
        #endregion
        #region<REGION DE DESPACHO DE ALMACEN ACCION>
        private string _session_listDespacho_almacen_cab_private = "_session_listDespacho_almacen_cab_private";
        private string _session_listDespacho_almacen_liq_private = "_session_listDespacho_almacen_liq_private";
        public ActionResult DespachoAlmacen(Int32 estado=0,Int32 estado_edicion=0/*si es 1 entonces no se puede editar*/ ,decimal desp_id=0)
        {

            if (estado==0)
            {
                return RedirectToAction("ListaDespacho", "Logistica");
            }

            ViewBag.TipoDes = "Provincia";
            Session[_session_listDespacho_almacen_cab_private] = null;
            Session[_session_listDespacho_almacen_liq_private] = null;

            List<Ent_Lista_Rotulo> lista_inicio_rotulo= new List<Ent_Lista_Rotulo>();
            Ent_Lista_Rotulo obj_rot = new Ent_Lista_Rotulo();        
            lista_inicio_rotulo.Add(obj_rot);

            ViewBag.Lista = lista_inicio_rotulo;

            List<Ent_Despacho_Almacen_Update> desp_upd_lista = new List<Ent_Despacho_Almacen_Update>();

            Ent_Despacho_Almacen_Update desp_upd = new Ent_Despacho_Almacen_Update();

            ViewBag.ListaDespachoUpd = desp_upd_lista;
            ViewBag.DespachoUpd = desp_upd;
            ViewBag.estado = estado;
            ViewBag.desp_id = desp_id;
            ViewBag.estado_edicion = estado_edicion;

            return View();
        }
        public Ent_Despacho_Almacen lista_despacho_almacen(string tipo_des, string fecha_ini, string fecha_fin)
        {
            Ent_Despacho_Almacen listdespacho = dat_despacho.get_despacho_almacen(tipo_des, Convert.ToDateTime(fecha_ini), Convert.ToDateTime(fecha_fin));
            Session[_session_listDespacho_almacen_cab_private] = listdespacho.despacho_cab;
            Session[_session_listDespacho_almacen_liq_private] = listdespacho.despacho_liq;
            return listdespacho;
        }

        public ActionResult getListDespachoAlmacenAjax(Ent_jQueryDataTableParams param, string actualizar, string fecha_ini, string fecha_fin, string tipo_des)
        {

            //List<Ent_Lista_Despacho> listdespacho = new List<Ent_Lista_Despacho>();

            Ent_Despacho_Almacen listdespacho = new Ent_Despacho_Almacen();

            if (!String.IsNullOrEmpty(actualizar))
            {
                listdespacho = lista_despacho_almacen(tipo_des, fecha_ini, fecha_fin);
                //listAtributos = datOE.get_lista_atributos();
                Session[_session_listDespacho_almacen_cab_private] = listdespacho.despacho_cab;
                Session[_session_listDespacho_almacen_liq_private] = listdespacho.despacho_liq;
            }

            /*verificar si esta null*/
            if (Session[_session_listDespacho_almacen_cab_private] == null)
            {

                listdespacho = new Ent_Despacho_Almacen();

                List<Ent_Despacho_Almacen_Cab> lista1 = new List<Ent_Despacho_Almacen_Cab>();
                List<Ent_Despacho_Almacen_Liquidacion> lista2 = new List<Ent_Despacho_Almacen_Liquidacion>();
                listdespacho.despacho_cab = lista1;
                listdespacho.despacho_liq = lista2;

                Session[_session_listDespacho_almacen_cab_private] = listdespacho.despacho_cab;
                Session[_session_listDespacho_almacen_liq_private] = listdespacho.despacho_liq;
            }


            //Traer registros
            IQueryable<Ent_Despacho_Almacen_Cab> membercol = ((List<Ent_Despacho_Almacen_Cab>)(Session[_session_listDespacho_almacen_cab_private])).AsQueryable();  //lista().AsQueryable();

            //Manejador de filtros
            int totalCount = membercol.Count();
            IEnumerable<Ent_Despacho_Almacen_Cab> filteredMembers = membercol;

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = membercol
                    .Where(m => m.Asesor.ToUpper().Contains(param.sSearch.ToUpper()) ||
                        m.Asesor.ToUpper().Contains(param.sSearch.ToUpper()) ||
                        m.NombreLider.ToUpper().Contains(param.sSearch.ToUpper()) ||
                        m.Promotor.ToUpper().Contains(param.sSearch.ToUpper()) 
                    );
            }
            //Manejador de orden
            //var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);

            //if (param.iSortingCols > 0)
            //{
            //    if (Request["sSortDir_0"].ToString() == "asc")
            //    {
            //        switch (sortIdx)
            //        {
            //            case 0: filteredMembers = filteredMembers.OrderBy(o => o.desp_nrodoc); break;
            //            case 1: filteredMembers = filteredMembers.OrderBy(o => o.desp_descripcion); break;
            //            case 2: filteredMembers = filteredMembers.OrderBy(o => Convert.ToDateTime(o.desp_fechacre)); break;
            //            case 3: filteredMembers = filteredMembers.OrderBy(o => o.totalparesenviado); break;
            //            case 4: filteredMembers = filteredMembers.OrderBy(o => o.estado); break;

            //        }
            //    }
            //    else
            //    {
            //        switch (sortIdx)
            //        {
            //            case 0: filteredMembers = filteredMembers.OrderByDescending(o => o.desp_nrodoc); break;
            //            case 1: filteredMembers = filteredMembers.OrderByDescending(o => o.desp_descripcion); break;
            //            case 2: filteredMembers = filteredMembers.OrderByDescending(o => Convert.ToDateTime(o.desp_fechacre)); break;
            //            case 3: filteredMembers = filteredMembers.OrderByDescending(o => o.totalparesenviado); break;
            //            case 4: filteredMembers = filteredMembers.OrderByDescending(o => o.estado); break;
            //        }
            //    }
            //}
            var displayMembers = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);


            var result = from a in displayMembers
                         select new
                         {
                             a.Asesor,
                             a.area_id,
                             a.NombreLider,
                             a.Promotor,
                             a.Rotulo,
                             a.rotulo_courier,
                             a.Agencia,
                             a.Destino,
                             a.Pedido,
                             a.TotalPares,
                             a.TotalCatalogo,
                             a.TotalPremio,
                             a.TotalCantidad,
                             a.TotalVenta,
                             a.Igv,
                             a.McaFlete,
                             a.Flete,
                             a.Lid_Prom, 
                             a.observacion,
                             a.detalle                            
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

        public ActionResult getRotulo_promotor(string idlider)
        {
            string mensaje = "";
            string estado = "0";
            List<Ent_Lista_Rotulo> lista_rotulo = null;
            try
            {
                lista_rotulo= dat_despacho.listar_rotulo_x_lider(idlider);
                if (lista_rotulo.Count==0)
                {
                    estado = "1";
                    mensaje = "No hay filas para mostrar";
                }
                else
                {
                   
                    estado = "0";
                }
                
            }
            catch (Exception exc)
            {
                estado = "1";
                mensaje = exc.Message;
                lista_rotulo = new List<Ent_Lista_Rotulo>();
            }
            return Json(new { estado = estado, mensaje = mensaje, info = lista_rotulo });
        }
        public ActionResult UpdateRotulo(string lid_prom,string rotulo,Int32 estado_accion/*Esta de accion 1 o 2 1=nuevo,2=edicion*/,
                                        List<Ent_Despacho_Almacen_Update> desp_lista_upd /*string agencia,string destino,string observacion,string detalle,string flete*/)
        {
            string mensaje = "";
            string estado = "0";
            try
            {

                if (estado_accion==1 || estado_accion == 3)
                {
                    List<Ent_Despacho_Almacen_Cab> despacho_edit_lista = (List<Ent_Despacho_Almacen_Cab>)Session[_session_listDespacho_almacen_cab_private];

                    var index = despacho_edit_lista.FindIndex(item => item.Lid_Prom == lid_prom);

                    var obj = despacho_edit_lista.Where(e => e.Lid_Prom == lid_prom).ToList();

                    obj[0].Rotulo = rotulo;
                   

                    Ent_Despacho_Almacen_Cab despacho_edit = new Ent_Despacho_Almacen_Cab();

                    foreach (Ent_Despacho_Almacen_Cab item in obj)
                    {
                        despacho_edit = item;
                    }

                    despacho_edit_lista[index] = despacho_edit;

                    List<Ent_Despacho_Almacen_Cab> lista_despa = new List<Ent_Despacho_Almacen_Cab>();

                    foreach (var it in despacho_edit_lista)
                    {
                        lista_despa.Add(it);
                    }
                    
                    foreach (var fila in lista_despa)
                    {
                        var index_upd = despacho_edit_lista.FindIndex(item => item.Lid_Prom == fila.Lid_Prom);
                        var obj_upd = despacho_edit_lista.Where(e => e.Lid_Prom == fila.Lid_Prom).ToList();

                        var str = desp_lista_upd.Where(u => u.strLid_Prom == fila.Lid_Prom).ToList();

                        obj_upd[0].Agencia = str[0].strAgencia;
                        obj_upd[0].Destino = str[0].strDestino;
                        obj_upd[0].observacion = str[0].strObs;
                        obj_upd[0].detalle = str[0].strDetalle;
                        obj_upd[0].Flete = str[0].strMcaFlete;

                   
                        despacho_edit_lista[index_upd] = obj_upd[0];// despacho_edit;

                    }

                    Session[_session_listDespacho_almacen_cab_private] = despacho_edit_lista;

                    estado = "0";
                }
                else
                {
                    List<Ent_Despacho_Almacen_Cab_Update> despacho_edit_lista = (List<Ent_Despacho_Almacen_Cab_Update>)Session[_session_listDespacho_almacen_cab_editar_private];

                    var index = despacho_edit_lista.FindIndex(item => item.Lid_Prom == lid_prom);

                    var obj = despacho_edit_lista.Where(e => e.Lid_Prom == lid_prom).ToList();

                    obj[0].Rotulo = rotulo;
                    
                    Ent_Despacho_Almacen_Cab_Update despacho_edit = new Ent_Despacho_Almacen_Cab_Update();

                    foreach (Ent_Despacho_Almacen_Cab_Update item in obj)
                    {
                        despacho_edit = item;
                    }

                    despacho_edit_lista[index] = despacho_edit;

                    List<Ent_Despacho_Almacen_Cab_Update> lista_despa = new List<Ent_Despacho_Almacen_Cab_Update>();

                    foreach (var it in despacho_edit_lista)
                    {
                        lista_despa.Add(it);
                    }

                    foreach (var fila in lista_despa)
                    {
                        var index_upd = despacho_edit_lista.FindIndex(item => item.Lid_Prom == fila.Lid_Prom);
                        var obj_upd = despacho_edit_lista.Where(e => e.Lid_Prom == fila.Lid_Prom).ToList();

                        var str = desp_lista_upd.Where(u => u.strLid_Prom == fila.Lid_Prom).ToList();

                        obj_upd[0].Agencia = str[0].strAgencia;
                        obj_upd[0].Destino = str[0].strDestino;
                        obj_upd[0].Observacion = str[0].strObs;
                        obj_upd[0].Detalle = str[0].strDetalle;
                        obj_upd[0].CobroFlete = str[0].strMcaFlete;


                        despacho_edit_lista[index_upd] = obj_upd[0];// despacho_edit;

                    }


                    Session[_session_listDespacho_almacen_cab_editar_private] = despacho_edit_lista;

                    estado = "0";
                }

               
            }
            catch (Exception exc)
            {

                estado = "1";
                mensaje = exc.Message;
            }
            return Json(new { estado = estado, mensaje = mensaje});
        }

        private string devolverIdliquidacion(string strIdLider, string strLid_Prom, string pedido)
        {
            string StrlistLiquidacion = "";
            List<Ent_Despacho_Almacen_Liquidacion> list_liqui =(List<Ent_Despacho_Almacen_Liquidacion>) Session[_session_listDespacho_almacen_liq_private];
            try
            {
                foreach(Ent_Despacho_Almacen_Liquidacion item in list_liqui)
                {
                    if (strIdLider ==item.area_id && strLid_Prom ==item.lid_prom && pedido.Length > 0)
                    {

                        string strLiq_Id =item.liq_id.ToString();
                        StrlistLiquidacion += "<row  ";
                        StrlistLiquidacion += " IdLider=¿" + strIdLider + "¿ ";
                        StrlistLiquidacion += " IdDespacho=¿xxyy¿ ";
                        StrlistLiquidacion += " IdLiqui=¿" + strLiq_Id + "¿ ";
                        StrlistLiquidacion += " LidProm=¿" + strLid_Prom + "¿ ";
                        StrlistLiquidacion += "/>";
                    }
                    if (strIdLider ==item.area_id && pedido.Length == 0 && item.bas_tip_des == "02")
                    {
                        string strLiq_Id = item.liq_id.ToString();
                        StrlistLiquidacion += "<row  ";
                        StrlistLiquidacion += " IdLider=¿" + strIdLider + "¿ ";
                        StrlistLiquidacion += " IdDespacho=¿xxyy¿ ";
                        StrlistLiquidacion += " IdLiqui=¿" + strLiq_Id + "¿ ";
                        StrlistLiquidacion += " LidProm=¿" + strLid_Prom + "¿ ";
                        StrlistLiquidacion += "/>";
                    }
                }

            }
            catch (Exception exc)
            {

                throw;
            }

                       
            return StrlistLiquidacion;

        }

        public ActionResult GenerarDespacho(Int32 estado_accion,Int32 iddespacho_upd,List<Ent_Despacho_Almacen_Update> desp_lista_upd)
        {
            string mensaje = "";
            string estado = "0";
            Int32 iddespacho = 0;
            try
            {
                string strDataDetalle = "";
                string strLiqLiderDespacho = "";
                Int32 id_despacho = iddespacho_upd;
                foreach (Ent_Despacho_Almacen_Update obj in desp_lista_upd)
                {
                    if (obj.strPromotor == null) obj.strPromotor = "";
                    if (obj.strPedidos == null) obj.strPedidos = "";
                    if (obj.strLid_Prom == null) obj.strLid_Prom = "";

                    strDataDetalle += "<row  ";
                    strDataDetalle += " IdLider=¿" + obj.strIdLider + "¿ ";
                    strDataDetalle += " IdDetalle=¿" + obj.strIdDetalle + "¿ ";
                    strDataDetalle += " Lider=¿" + obj.strLider + "¿ ";
                    strDataDetalle += " Rotulo=¿" + obj.strRotulo + "¿ ";
                    strDataDetalle += " RotuloCourier=¿" + obj.strRotuloCourier + "¿ ";
                    strDataDetalle += " McaCourier=¿" + obj.strMcaCourier + "¿ ";
                    strDataDetalle += " Pares=¿" + obj.strPares + "¿ ";
                    strDataDetalle += " Catalogo=¿" + obj.strCatalogo + "¿ ";
                    strDataDetalle += " Premio=¿" + obj.strPremio + "¿ ";
                    strDataDetalle += " Destino=¿" + obj.strDestino + "¿ ";
                    strDataDetalle += " Agencia=¿" + obj.strAgencia + "¿ ";
                    strDataDetalle += " Monto=¿" + obj.strMonto + "¿ ";
                    strDataDetalle += " Obs=¿" + obj.strObs + "¿ ";
                    strDataDetalle += " Det=¿" + obj.strDetalle + "¿ ";
                    strDataDetalle += " McaFlete=¿" + obj.strMcaFlete + "¿ ";



                    strDataDetalle += " Promotor=¿" + obj.strPromotor + "¿ ";
                    strDataDetalle += " Pedidos=¿" + obj.strPedidos + "¿ ";
                    strDataDetalle += " LidProm=¿" + obj.strLid_Prom + "¿ ";

                    strDataDetalle += "/>";

                    strLiqLiderDespacho += devolverIdliquidacion(obj.strIdLider, obj.strLid_Prom, obj.strPedidos);

                }

                Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
                string generar = "";
                if (estado_accion == 1 || estado_accion == 3)
                {
                    generar = dat_despacho.insertar_despacho(_usuario.usu_id, ref id_despacho, strDataDetalle, strLiqLiderDespacho, "");
                }
                else
                {
                    generar = dat_despacho.update_despacho(_usuario.usu_id, iddespacho_upd, strDataDetalle, "");
                }
                    

                if (generar.Length==0)
                {

                    switch(estado_accion)
                    {
                        case 1:
                            iddespacho = id_despacho;
                            estado = "0";
                            mensaje = "Se genero con exito el despacho numero " + iddespacho.ToString();
                            break;
                        case 2:
                            iddespacho = iddespacho_upd;
                            estado = "0";
                            mensaje = "Se actualizo con exito el despacho numero " + iddespacho.ToString();
                            break;
                        case 3:
                            iddespacho = iddespacho_upd;
                            estado = "0";
                            mensaje = "Se Agregaron Items con exito el despacho numero " + iddespacho.ToString();
                            break;
                    }

                    //if (estado_accion == 1)
                    //{
                    //    iddespacho = id_despacho;
                    //    estado = "0";
                    //    mensaje = "Se genero con exito el despacho numero " + iddespacho.ToString();
                    //}
                    //else
                    //{
                    //    iddespacho = iddespacho_upd;
                    //    estado = "0";
                    //    mensaje = "Se actualizo con exito el despacho numero " + iddespacho.ToString();
                    //}
                        


                }else
                {
                    estado = "1";
                    mensaje = "Hubo un problema en el despacho...";
                }

                
            }
            catch (Exception exc)
            {
                estado = "1";
                mensaje = exc.Message;
            }
            return Json(new { estado = estado, mensaje = mensaje, iddespacho = iddespacho });
        }

        #region <REGION DE EDITAR DESPACHO>
        private string _session_listDespacho_almacen_cab_editar_private = "_session_listDespacho_almacen_cab_editar_private";
        private string _session_listDespacho_almacen_det_editar_private = "_session_listDespacho_almacen_det_editar_private";
        private string _session_listDespacho_almacen_det_get_private = "_session_listDespacho_almacen_det_get_private";
        public ActionResult ConsultaDespachoEdit(string iddespacho)
        {
            string mensaje = "";
            string estado = "0";
            Ent_Despacho_Almacen_Editar get_despacho = null;
            try
            {
                get_despacho = get_consulta_despacho(iddespacho);

                Session[_session_listDespacho_almacen_cab_editar_private] = get_despacho.Almacen_Cab_Update;
                Session[_session_listDespacho_almacen_det_editar_private] = get_despacho.Almacen_Det_Update;
                Session[_session_listDespacho_almacen_det_get_private] = get_despacho;



            }
            catch (Exception exc)
            {
                estado = "1";
                mensaje = exc.Message;
            }
            return Json(new { estado = estado, mensaje = mensaje,infocab= get_despacho.Almacen_Cab_Update,infodet= get_despacho.Almacen_Det_Update });
        }
        public Ent_Despacho_Almacen_Editar get_consulta_despacho(string iddespacho)
        {
            Ent_Despacho_Almacen_Editar obj = null;
            try
            {
                obj = dat_despacho.get_despacho_almacen_editar(Convert.ToInt32(iddespacho));
            }
            catch (Exception exc)
            {

                throw exc;
            }
            return obj;
        }
        public ActionResult EliminarItemDespachoEdit(string iddespacho,string lid_prom)
        {
            string mensaje = "";
            string estado = "0";           
            try
            {

                List<Ent_Despacho_Almacen_Cab_Update> despacho_cons =(List<Ent_Despacho_Almacen_Cab_Update>) Session[_session_listDespacho_almacen_cab_editar_private];

                if (despacho_cons.Count==1)
                {
                    estado = "1";
                    mensaje = "Accion rechazada: El despacho debe tener al menos un detalle.";
                }
                else
                {
                    Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
                    string eliminar_item = dat_despacho.eliminar_despacho_item(_usuario.usu_id,Convert.ToInt32(iddespacho),Convert.ToInt32(lid_prom));

                    if (eliminar_item.Length == 0)
                    {
                        estado = "0";
                        mensaje = "Item elininado con exito.";
                    }
                    else
                    {
                        estado = "1";
                        mensaje = eliminar_item;
                    }
                  
                }
               

            }
            catch (Exception exc)
            {
                estado = "1";
                mensaje = exc.Message;
            }
            return Json(new { estado = estado, mensaje = mensaje });
        }

        public ActionResult getListDespachoAlmacenEditarAjax(Ent_jQueryDataTableParams param/*, string actualizar, string fecha_ini, string fecha_fin, string tipo_des*/)
        {

            //List<Ent_Lista_Despacho> listdespacho = new List<Ent_Lista_Despacho>();

            List<Ent_Despacho_Almacen_Cab_Update> listdespacho = new List<Ent_Despacho_Almacen_Cab_Update>();

            //if (!String.IsNullOrEmpty(actualizar))
            //{
            //    listdespacho = lista_despacho_almacen(tipo_des, fecha_ini, fecha_fin);
            //    //listAtributos = datOE.get_lista_atributos();
            //    Session[_session_listDespacho_almacen_cab_private] = listdespacho.despacho_cab;
            //    Session[_session_listDespacho_almacen_liq_private] = listdespacho.despacho_liq;
            //}

            /*verificar si esta null*/
            if (Session[_session_listDespacho_almacen_cab_editar_private] == null)
            {

                listdespacho = new List<Ent_Despacho_Almacen_Cab_Update>();

                Session[_session_listDespacho_almacen_cab_editar_private] = listdespacho;
                //Session[_session_listDespacho_almacen_liq_private] = listdespacho.despacho_liq;
            }


            //Traer registros
            IQueryable<Ent_Despacho_Almacen_Cab_Update> membercol = ((List<Ent_Despacho_Almacen_Cab_Update>)(Session[_session_listDespacho_almacen_cab_editar_private])).AsQueryable();  //lista().AsQueryable();

            //Manejador de filtros
            int totalCount = membercol.Count();
            IEnumerable<Ent_Despacho_Almacen_Cab_Update> filteredMembers = membercol;

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = membercol
                    .Where(m => m.asesor.ToUpper().Contains(param.sSearch.ToUpper()) ||
                        m.NombreLider.ToUpper().Contains(param.sSearch.ToUpper()) ||
                        m.Promotor.ToUpper().Contains(param.sSearch.ToUpper())
                    );
            }
            //Manejador de orden
            //var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);

            //if (param.iSortingCols > 0)
            //{
            //    if (Request["sSortDir_0"].ToString() == "asc")
            //    {
            //        switch (sortIdx)
            //        {
            //            case 0: filteredMembers = filteredMembers.OrderBy(o => o.desp_nrodoc); break;
            //            case 1: filteredMembers = filteredMembers.OrderBy(o => o.desp_descripcion); break;
            //            case 2: filteredMembers = filteredMembers.OrderBy(o => Convert.ToDateTime(o.desp_fechacre)); break;
            //            case 3: filteredMembers = filteredMembers.OrderBy(o => o.totalparesenviado); break;
            //            case 4: filteredMembers = filteredMembers.OrderBy(o => o.estado); break;

            //        }
            //    }
            //    else
            //    {
            //        switch (sortIdx)
            //        {
            //            case 0: filteredMembers = filteredMembers.OrderByDescending(o => o.desp_nrodoc); break;
            //            case 1: filteredMembers = filteredMembers.OrderByDescending(o => o.desp_descripcion); break;
            //            case 2: filteredMembers = filteredMembers.OrderByDescending(o => Convert.ToDateTime(o.desp_fechacre)); break;
            //            case 3: filteredMembers = filteredMembers.OrderByDescending(o => o.totalparesenviado); break;
            //            case 4: filteredMembers = filteredMembers.OrderByDescending(o => o.estado); break;
            //        }
            //    }
            //}
            var displayMembers = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);


            var result = from a in displayMembers
                         select new
                         {
                             a.asesor,
                             a.IdLider,
                             a.NombreLider,
                             a.Promotor,
                             a.Rotulo,
                             a.Rotulo_Courier,
                             a.Agencia,
                             a.Destino,
                             a.Pedido,
                             a.TotalPares,
                             a.TotalCatalogo,
                             a.TotalPremio,
                             a.Total_Cantidad,
                             a.TotalVenta,
                             //a.Igv,
                             a.McaFlete,
                             a.CobroFlete,
                             a.Lid_Prom,
                             a.Observacion,
                             a.Detalle,
                             a.Total_Cantidad_Envio,
                             a.Desp_IdDetalle,
                             a.Desp_id
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
        private void ExportarExcel(Ent_Despacho_Almacen_Editar desp, string ColumnasOcultas, string ColumnasTexto, string NombreArchivo)
        {

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            String style = style = @"<style> .textmode { mso-number-format:\@; } </script> ";
            Page page = new Page();
            try
            {
                List<Ent_Despacho_Almacen_Cab_Update> list_cab = desp.Almacen_Cab_Update;
                List<Ent_Despacho_Almacen_Det_Update> list_det = desp.Almacen_Det_Update;

                String inicio;
                ColumnasOcultas = ',' + ColumnasOcultas + ",";
                ColumnasTexto = ',' + ColumnasTexto + ",";

                Style stylePrueba = new Style();
                stylePrueba.Width = Unit.Pixel(200);
                string strRows = "";
                string strRowsHead = "";
                strRowsHead = strRowsHead + "<tr height=38 >";
               
                var PropertyInfos = list_cab.First().GetType().GetProperties();
                foreach (var col in PropertyInfos)
                {               
               
                    switch(col.Name.ToUpper())
                    {
                        case "ASESOR":
                        case "NOMBRELIDER":
                        case "PROMOTOR":
                        case "ROTULO":
                        case "AGENCIA":
                        case "DESTINO":
                        case "PEDIDO":
                        case "TOTAL_CANTIDAD":
                        case "TOTAL_CANTIDAD_ENVIO":
                        case "TOTALVENTA":
                        case "COBROFLETE":
                        case "OBSERVACION":
                        case "DETALLE":
                            strRowsHead = strRowsHead + "<td height=38  bgcolor='#969696' width='38'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + col.Name + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</ td > ";
                            break;
                    }                 
                }

                strRowsHead = strRowsHead + "</tr>";

                foreach (var Item in list_cab)
                {
                    strRows = strRows + "<tr height='38' >";                    
                    string strClass = "";

                    strRows = strRows + "<td width='400' " + strClass + " >" + Item.asesor + "</ td > ";
                    strRows = strRows + "<td width='400' " + strClass + " >" + Item.NombreLider + "</ td > ";
                    strRows = strRows + "<td width='400' " + strClass + " >" + Item.Promotor + "</ td > ";
                    strRows = strRows + "<td width='400' " + strClass + " >" + Item.Rotulo + "</ td > ";
                    strRows = strRows + "<td width='400' " + strClass + " >" + Item.Agencia + "</ td > ";
                    strRows = strRows + "<td width='400' " + strClass + " >" + Item.Destino + "</ td > ";
                    strRows = strRows + "<td width='400' " + strClass + " >" + Item.Pedido + "</ td > ";
                    strRows = strRows + "<td width='400' " + strClass + " >" + Item.Total_Cantidad.ToString() + "</ td > ";
                    strRows = strRows + "<td width='400' " + strClass + " >" + Item.Total_Cantidad_Envio.ToString() + "</ td > ";
                    strRows = strRows + "<td width='400' " + strClass + " >" + Item.TotalVenta.ToString() + "</ td > ";
                    strRows = strRows + "<td width='400' " + strClass + " >" + Item.CobroFlete + "</ td > ";
                    strRows = strRows + "<td width='400' " + strClass + " >" + Item.Observacion + "</ td > ";
                    strRows = strRows + "<td width='400' " + strClass + " >" + Item.Detalle + "</ td > ";
                    
                    strRows = strRows + "</tr>";
                }


                string desc = "";
                string nrodoc = list_cab[0].Desp_NroDoc.ToString();
                string est = list_cab[0].estado.ToString();
                string fec = list_cab[0].Desp_FechaCre;
                string strTotalPedido = list_det[0].NroPedidos.ToString();
                string strTotalEnviado = list_det[0].NroEnviados.ToString();
                string strTotalCataPedido = list_det[0].CatalogPedidos.ToString();
                string strTotalCataEnviado = list_det[0].CatalogEnviados.ToString(); 
                string strTotalPremioPedido = list_det[0].NroPremio.ToString();
                string strTotalPremioEnviado = list_det[0].PremioEnviados.ToString();
                string strTotalMonto = list_det[0].MontoTotal.ToString();

                string strTable = "<table <Table border='1' bgColor='#ffffff' " +
            "borderColor='#000000' cellSpacing='2' cellPadding='2' " +
            "style='font-size:10.0pt; font-family:Calibri; background:white;'>";
                strTable += "<tr height=38 ><td height=38  bgcolor='#969696' width='38'>Nro. Documento </ td ><td width='400' align='left' >" + nrodoc + "</ td > ";
                strTable += "<td height=38  bgcolor='#969696' width='38'>Fec. Creación. </ td ><td width='400' align='left' colspan='2' >" + fec + "</ td > </tr>";
                strTable += "<tr height=38 ><td height=38  bgcolor='#969696' width='38'>Total Monto. </ td ><td width='400' align='left' >" + strTotalMonto + "</ td > ";
                strTable += "<td height=38  bgcolor='#969696' width='38'>Estado </ td ><td width='400' align='left' colspan='2' >" + est + "</ td ></tr>";
                strTable += "<tr height=38 ><td height=38  bgcolor='#969696' width='38'>Pares Pedido. </ td ><td width='400' align='left' >" + strTotalPedido + "</ td > ";
                strTable += "<td height=38  bgcolor='#969696' width='38'>Pares Enviado </ td ><td width='400' align='left' colspan='2' >" + strTotalEnviado + "</ td ></tr>";
                strTable += "<tr height=38 ><td height=38  bgcolor='#969696' width='38'>Catalogo Facturado </ td ><td width='400' align='left' >" + strTotalCataPedido + "</ td > ";
                strTable += "<td height=38  bgcolor='#969696' width='38'>Catalogo Enviado </ td ><td width='400' align='left' colspan='2' >" + strTotalCataEnviado + "</ td ></tr>";

                strTable += "<tr height=38 ><td height=38  bgcolor='#969696' width='38'>Premio Pedido </ td ><td width='400' align='left' >" + strTotalPremioPedido + "</ td > ";
                strTable += "<td height=38  bgcolor='#969696' width='38'>Premio Enviado </ td ><td width='400' align='left' colspan='2' >" + strTotalPremioEnviado + "</ td ></tr>";

                //strTable += "<tr height=38 ><td height=38  bgcolor='#969696' width='38'>Descripción </ td ><td colspan='4' align='left' >" + desc + "</ td > ";
                strTable += "</tr>";

                strTable += "</table>";

                inicio = "<div> " + strTable +
                "<table <Table border='1' bgColor='#ffffff' " +
                "borderColor='#000000' cellSpacing='2' cellPadding='2' " +
                "style='font-size:10.0pt; font-family:Calibri; background:white;'>" +
                strRowsHead +
                strRows +
                "</table>" +
                "</div>";

                sb.Append(inicio);

                Session[_session_despacho_almacen_excel] = sb.ToString();

            }
            catch (Exception exc)
            {

                throw exc;
            }

           
        }

        private string _session_despacho_almacen_excel = "_session_stock_articulo_categoria_excel";
        public ActionResult ListaDespachoExcel()
        {
            string NombreArchivo = "Orden_Despacho";
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
                Response.Write(Session[_session_despacho_almacen_excel].ToString());
                Response.End();

            }
            catch 
            {
                
            }
                      
            return Json(new { estado = 0, mensaje = 1 });
        }
        public ActionResult get_exporta_excel(string id="0")
        {
            string mensaje = "";
            string estado = "0";
            try
            {
                Ent_Despacho_Almacen_Editar despacho_lista = get_consulta_despacho(id);
                ExportarExcel(despacho_lista, "", "2", "Orden_Despacho");

                mensaje = "Se genero el excel correctamente";
                estado = "1";
               
            }
            catch (Exception exc)
            {
                estado = "0";
                mensaje = exc.Message;
            }

            return Json(new { estado = estado, mensaje = mensaje });
        }

        #endregion
        #endregion
    }
}