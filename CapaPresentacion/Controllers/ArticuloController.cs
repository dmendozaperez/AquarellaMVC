using OfficeOpenXml;
using CapaDato.Articulo;
using CapaDato.Pedido;
using CapaEntidad.Articulo;
using CapaEntidad.Control;
using CapaEntidad.General;
using CapaEntidad.Pedido;
using CapaEntidad.Util;
using CapaPresentacion.Bll;
using CapaPresentacion.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.IO;
using Newtonsoft.Json;
namespace CapaPresentacion.Controllers
{
    public class ArticuloController : Controller
    {
        // GET: Articulo
        private string _session_stock_x_articulo = "_session_stock_x_articulo";
        private string _session_stock_x_articulo_filtro = "_session_stock_x_articulo_filtro";
        private string _session_pedido_sin_stock = "_session_pedido_sin_stock";
        private string _session_listaArticuloPrecios = "_session_listaArticuloPrecios";

        Dat_Articulo Dat_Articulo = new Dat_Articulo();
        public ActionResult Index()
        {
            return View();
        }
        #region<Stock de un Articulo>
        public FileContentResult ListaArticuloSinStockExcel()
        {

            List<Ent_Articulo_Sin_Stock> lista = (List<Ent_Articulo_Sin_Stock>)Session[_session_pedido_sin_stock];

            if (lista == null) {
                lista = new List<Ent_Articulo_Sin_Stock>();
            } 

            string[] columns = { "pedido", "dni", "nombres", "articulo", "talla", "estado" };

            byte[] filecontent = ExcelExportHelper.ExportExcel(lista, "LISTA DE ARTICULOS SIN STOCK - CATALOGO - BATA", false, columns);
            string nom_excel = "ArtsinStk_Catalogo";
            return File(filecontent, ExcelExportHelper.ExcelContentType, nom_excel + ".xlsx");
        }
        public ActionResult get_articulo_sin_stock()
        {
            string mensaje = "";
            string estado = "0";
            List<Ent_Articulo_Sin_Stock> art_sin_stk = null;
            Dat_Articulo_Stock stk_articulo = new Dat_Articulo_Stock();
            try
            {
                art_sin_stk = stk_articulo.listar_articulo_sinstock();
                if (art_sin_stk.Count == 0)
                {
                    estado = "0";
                    mensaje = "No hay pedidos para mostrar";
                }
                else
                {
                    estado = "1";
                }

            }
            catch
            {
                
            }
            Session[_session_pedido_sin_stock] = art_sin_stk;
            return Json(new { estado = estado, mensaje = mensaje });
        }
        public ActionResult getTableArticuloSinStockAjax(Ent_jQueryDataTableParams param)
        {
            /*verificar si esta null*/
            if (Session[_session_pedido_sin_stock] == null)
            {
                List<Ent_Articulo_Sin_Stock> liststock = new List<Ent_Articulo_Sin_Stock>();
                Session[_session_pedido_sin_stock] = liststock;
            }

            //}
            //Traer registros
            IQueryable<Ent_Articulo_Sin_Stock> membercol = ((List<Ent_Articulo_Sin_Stock>)(Session[_session_pedido_sin_stock])).AsQueryable();

            //Manejador de filtros
            int totalCount = membercol.Count();

            IEnumerable<Ent_Articulo_Sin_Stock> filteredMembers = membercol;


            //Manejador de orden
            var displayMembers = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);

            var result = from a in displayMembers
                         select new
                         {
                             a.pedido,
                             a.dni,
                             a.nombres,
                             a.articulo,
                             a.talla,
                             a.estado,
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

        public ActionResult get_articulo_almacen(string alm)
        {
            string mensaje = "";
            string estado = "0";
            try
            {
                List<Ent_Articulo_Stock> listar_articulo = (List<Ent_Articulo_Stock>)Session[_session_stock_x_articulo];

                if (listar_articulo == null)
                {
                    estado = "0";
                    mensaje = "Ingrese el codigo de articulo";
                }
                else
                {
                    List<Ent_Articulo_Stock> filtrar_stk = null;
                    if (alm == "AQ")
                    {
                        filtrar_stk = listar_articulo.Where(s => s.id_almacen == alm).ToList();
                    }
                    else
                    {
                        filtrar_stk = listar_articulo.Where(s => s.id_almacen.Length>0).ToList();
                    }
                    estado = "1";
                    Session[_session_stock_x_articulo_filtro] = filtrar_stk;
                }


            }
            catch (Exception)
            {

             
            }

          
            return Json(new { estado = estado, mensaje = mensaje});
        }
        public ActionResult StockArticulo()
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
                Session[_session_stock_x_articulo] = null;
                Session[_session_stock_x_articulo_filtro] = null;
                Session[_session_pedido_sin_stock] = null;
                /*04 y 06 usuarios de acceso a paneles de stock y stock de */

                ViewBag.tipo_User = _usuario.usu_tip_id;

                return View();
            }

            
        }
        public ActionResult get_articulo(string articulo)
        {
            Dat_Articulo_Stock stk_articulo = new Dat_Articulo_Stock();
            List<Ent_Articulo_Stock> listar_articulo = null;
            Ent_Articulo_Info info_articulo = null;
            string mensaje = "";
            string estado = "0";
            decimal total_precio = 0;
            decimal total_costo = 0;
            try
            {
                info_articulo = new Ent_Articulo_Info();
                listar_articulo =stk_articulo.listar_stock(articulo, ref info_articulo);

                if (listar_articulo.Count == 0)
                {
                    estado = "0";
                    mensaje = "No hay stock del articulo ingresado";
                }
                else
                {

                    total_precio = listar_articulo.Where(c => c.id_almacen == "AQ").Sum(t => t.total) * info_articulo.precio;
                    total_costo = listar_articulo.Where(c => c.id_almacen == "AQ").Sum(t => t.total) * info_articulo.costo;


                    estado = "1";
                    mensaje = "";
                }

                Session[_session_stock_x_articulo] = listar_articulo;
                Session[_session_stock_x_articulo_filtro] = listar_articulo.Where(s=>s.id_almacen=="AQ").ToList();
            }
            catch 
            {

                
            }

            return Json(new { estado = estado, mensaje = mensaje,info= info_articulo,total_precio=total_precio,total_costo=total_costo });
        }

        public ActionResult getTableStockAjax(Ent_jQueryDataTableParams param)
        {
            /*verificar si esta null*/
            if (Session[_session_stock_x_articulo_filtro] == null)
            {
                List<Ent_Articulo_Stock> liststock = new List<Ent_Articulo_Stock>();
                Session[_session_stock_x_articulo_filtro] = liststock;
            }
         
            //}
            //Traer registros
            IQueryable<Ent_Articulo_Stock> membercol = ((List<Ent_Articulo_Stock>)(Session[_session_stock_x_articulo_filtro])).AsQueryable();

            //Manejador de filtros
            int totalCount = membercol.Count();

            IEnumerable<Ent_Articulo_Stock> filteredMembers = membercol;

        
            //Manejador de orden
            var displayMembers = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);

            var result = from a in displayMembers
                         select new
                         {
                             a.id_almacen,
                             a.almacen,
                             a.total,
                             a.articulo,
                             a.list_talla,  
                             a.list_pedido_sep,                           
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
        #region<Stock por Categpria>
        public ActionResult StockCategoria()
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
            Dat_Articulo_Stock stk_articulo = new Dat_Articulo_Stock();
            ViewBag.Categoria=stk_articulo.listar_categoria_principal();
            ViewBag.Temporada=stk_articulo.listar_temporada();

            Session[_session_stock_articulo_categoria] = null;
            Session[_session_stock_articulo_categoria_excel] = null;

            return View();
            }
        }
        private string _session_stock_articulo_categoria = "_session_stock_articulo_categoria";
        private string _session_stock_articulo_categoria_excel = "_session_stock_articulo_categoria_excel";
        public ActionResult get_articulo_categoria(string categoria,string tempo)
        {
            Dat_Articulo_Stock stk_articulo = new Dat_Articulo_Stock();
            List<Ent_Articulo_Categoria_Stock> listar_stock = null;
           
            string mensaje = "";
            string estado = "0";           
            try
            {

                listar_stock = stk_articulo.listar_stock_categoria(categoria, tempo);

                if (listar_stock.Count == 0)
                {
                    estado = "0";
                    mensaje = "No hay stock disponible";
                }
                else
                {                  
                    estado = "1";
                    mensaje = "";
                }

                Session[_session_stock_articulo_categoria] = listar_stock;                
            }
            catch
            {


            }

            return Json(new { estado = estado, mensaje = mensaje });
        }
        public ActionResult getTableStockCategoriaAjax(Ent_jQueryDataTableParams param)
        {
            /*verificar si esta null*/
            if (Session[_session_stock_articulo_categoria] == null)
            {
                List<Ent_Articulo_Categoria_Stock> liststock = new List<Ent_Articulo_Categoria_Stock>();
                Session[_session_stock_articulo_categoria] = liststock;
            }

            //}
            //Traer registros
            IQueryable<Ent_Articulo_Categoria_Stock> membercol = ((List<Ent_Articulo_Categoria_Stock>)(Session[_session_stock_articulo_categoria])).AsQueryable();

            //Manejador de filtros
            int totalCount = membercol.Count();



            IEnumerable<Ent_Articulo_Categoria_Stock> filteredMembers = membercol;

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = membercol
                    .Where(m => m.categoria.ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.codigo.ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.descripcion.ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.tempo.ToUpper().Contains(param.sSearch.ToUpper())
                     );
            }


            //Manejador de orden
            var displayMembers = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);

            var result = from a in displayMembers
                         select new
                         {
                             a.categoria,
                             a.foto,
                             a.codigo,
                             a.descripcion,
                             a.tempo,
                             a.precio,   
                             a.total,                         
                             a.list_talla
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

        public string get_html_str(List<Ent_Articulo_Categoria_Stock> listar_stock)
        {
            string html_str = "";
            try
            {

                string cadena = "<div>" +
                         "<table cellspacing='0' rules='all' border='1' style='border-collapse:collapse;'>" +
                            "<td valign='middle' align='center' style='font-size: 18px;font-weight: bold;color:#285A8F'>Stock de Articulo por Categoria</td>" +
                         "</table>" +
                         "<table cellspacing='0' rules='all' border='1' style='border-collapse:collapse;'>" +
                             "<tr height = 40 bgcolor='#B5B5B5'>" +
                                 "<th bgcolor='' scope='col'>CATEGORIA</th>" +
                                 "<th scope='col'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Foto&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</th>" +
                                 "<th scope='col'>CODIGO</th>" +
                                 "<th scope='col'>DESCRIPCION</th>" +
                                 "<th scope='col'>TEMPO</th>" +
                                 "<th scope='col'>PRECIO</th>" +
                                 "<th scope='col'>TALLA/CANTIDAD</th>" +
                             "</tr>";
                             
                foreach(Ent_Articulo_Categoria_Stock obj in listar_stock)
                {
                    cadena += "<tr height = 40 valign='middle'>" +
                                "<td valign='middle' height = 40 align='center'>" + obj.categoria + "</td>" +
                                "<td height = 40><img WIDTH='34' HEIGHT='34' alt='Logo_FR' src=" + obj.foto +" style = 'margin: 50 50 50 100; vertical-align:middle; padding-left:20' /></td>" +
                                "<td height = 40>" + obj.codigo + "</td>" +
                                "<td height = 40>" + obj.descripcion +"</td>" +
                                "<td height = 40>" + obj.tempo +"</td>" +
                                "<td height = 40>"+ obj.precio.ToString() +"</td>" +
                                "<td>";
                    string botones_head = "";
                    string botones_cant = "";
                    botones_head = "<div class='col-sm-1' style='width:100%;margin-top:2px;margin-bottom:2px;text-align:center;'>" +
                             "<table border='1'> " +
                                 "<tbody> " +
                                 "<tr height = 40>" +
                                     "<td bgcolor='#E5E4E4'>" +
                                         "<div style='margin-top:2px;margin-right:2px;'>" +
                                             "<button  class='' title='Tallas' style='width:100%;color:black;'>T:</button>" +
                                         "</div>" +
                                      "</td>" +
                                      "<td bgcolor='#00a65a' style='color: white;'>" +
                                         "<div style='margin-top:2px;margin-right:2px;'>" +
                                             "<button class='btn btn-success btn-xs' style='width:100%;background: #00a65a;color: white'>Total</button>" +
                                          "</div>" +
                                       "</td>";

                    botones_cant = "<tr height = 40>" +
                                                  "<td bgcolor='#E5E4E4'>" +
                                                     "<div style='margin-top:2px;margin-right:2px;'> " +
                                                         "<button title='Cantidad' class='' style='width:100%;color:black;'>C:</button> " +
                                                     "</div>" +
                                              "</td>";
                    botones_cant += "<td bgcolor='#00a65a' style='color: white;'> " +
                                                             "<div style='margin-top:2px;margin-right:2px;'> " +
                                                                 "<button class='btn btn-success btn-xs' style='width:100%;background: #00a65a;color: white'>" + obj.total.ToString() +"</button> " +
                                                             "</div>" +
                                                         "</td>";

                    foreach(Ent_Articulo_Talla obj_talla in obj.list_talla)
                    {

                        botones_head += "<td style='background: #3c8dbc;color: white'> " +
                           "<div style='margin-top:2px;margin-right:2px;'> " +
                              "<button class='btn btn-primary btn-xs' style='width:100%;background: #3c8dbc;color: white' title='Talla' data-id='codTienda' data-tda='desTiendaS' data-modal='codTalla' data-art='codArticulo'>" + obj_talla.talla.ToString() +"</button> " +
                           "</div>" +
                        "</td>";

                        botones_cant += "<td bgcolor='#E5E4E4'>" +
                                                 "<div style='margin-top:2px;margin-right:2px;'> " +
                                                     "<button class='btn btn-info btn-xs' style='width:100%;text-align:center;background: '>"+ obj_talla.cantidad.ToString() +"</button> " +
                                                 "</div>" +
                                             "</td>";
                    }
                    botones_head += botones_cant + "</td></tr>" +
                                                   "</tbody>" +
                                                   "</table>" +
                                                   "</div>";

                    cadena += botones_head;
                }


                cadena += "</tr>" +
                            "</table>" +
                             "</div>";

                html_str = cadena;
              

            }
            catch 
            {
                html_str = "";

            }
            return html_str;
        }

        public ActionResult get_exporta_excel()
        {
            string mensaje = "";
            string estado = "0";
            try
            {
                Session[_session_stock_articulo_categoria_excel] = null;
                string cadena = "";
                if (Session[_session_stock_articulo_categoria]!=null)
                {
                  
                    List<Ent_Articulo_Categoria_Stock> lista_excel = (List<Ent_Articulo_Categoria_Stock>)Session[_session_stock_articulo_categoria]; 
                    if (lista_excel.Count == 0)
                    {
                        estado = "0";
                        mensaje = "No hay filas para exportar";
                    }
                    else
                    {
                        cadena = get_html_str((List<Ent_Articulo_Categoria_Stock>)Session[_session_stock_articulo_categoria]);
                        if (cadena.Length == 0)
                        {
                            estado = "0";
                            mensaje = "Error del formato html";
                        }
                        else
                        {
                         
                            estado = "1";
                            mensaje = "Se genero el excel correctamente";
                            Session[_session_stock_articulo_categoria_excel] = cadena;
                         
                        }

                    }
                 
                }
                else
                {
                    estado = "0";
                    mensaje = "No hay filas para exportar";
                }
                 
            }
            catch (Exception)
            {

               
            }

            return Json(new { estado = estado, mensaje = mensaje });
        }

        public ActionResult ListaStockCategExcel()
        {
            string attachment = "attachment; filename=stockdetallado.xls";
            Response.ClearContent();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/ms-excel";
            Response.Write(Session[_session_stock_articulo_categoria_excel].ToString());
            Response.End();
            return Json(new { estado = 0, mensaje = 1 });          
        }

        public ActionResult JsonExcelArticulos(string articulos)
        {
            List<Ent_Articulo_Categoria_Stock> listArtExcel = null;
            Dat_Articulo_Stock stk_articulo = new Dat_Articulo_Stock();
            try
            {
                listArtExcel = new List<Ent_Articulo_Categoria_Stock>();
                listArtExcel = JsonConvert.DeserializeObject<List<Ent_Articulo_Categoria_Stock>>(articulos.ToUpper());
                if (listArtExcel.Where(w => String.IsNullOrEmpty(w.ARTICULO)).ToList().Count > 0)
                {
                    Session[_session_stock_articulo_categoria] = new List<Ent_Articulo_Categoria_Stock>();
                    return Json(new { estado = 0, resultados = "El Archivo no tiene el formato correcto ó hay campos vacios.\nVerifique el archivo." });
                }
                else
                {
               

                    string str_articulo_listar = "";
                    decimal filas = 0;
                    foreach (var item in listArtExcel)
                    {
                        filas += 1;
                        str_articulo_listar +=(item.ARTICULO) + ((filas== listArtExcel.Count)?"":",");
                    }


                    Session[_session_stock_articulo_categoria] = stk_articulo.listar_stock_categoria("","", str_articulo_listar);
                    return Json(new { estado = 1, resultados = "ok" });
                  
                }               
            }
            catch (Exception ex)
            {
                return Json(new { estado = 0, resultados = ex.Message });
            }
            
        }



        #endregion

        #region<Actualizar Precios>

        Dat_Articulo_Precio dat_precio = new Dat_Articulo_Precio();
        private string _session_lista_articulo_precio = "_session_lista_articulo_precio";

        public ActionResult JsonExcelArticulos_Precio(string articulos)
        {
            List<Ent_Articulo_Precio> listArtExcel = null;
            //Dat_Articulo_Stock stk_articulo = new Dat_Articulo_Stock();
            try
            {
                listArtExcel = new List<Ent_Articulo_Precio>();
                listArtExcel = JsonConvert.DeserializeObject<List<Ent_Articulo_Precio>>(articulos.ToUpper());
                if (listArtExcel.Where(w => String.IsNullOrEmpty(w.articulo) || w.precio == 0).ToList().Count > 0)
                {
                    Session[_session_lista_articulo_precio] = new List<Ent_Articulo_Precio>();
                    return Json(new { estado = 0, resultados = "El Archivo no tiene el formato correcto ó hay campos vacios.\nVerifique el archivo." });
                }
                else
                {


                 
                    Session[_session_lista_articulo_precio] = dat_precio.lista_articulo_precio(listArtExcel);
                    return Json(new { estado = 1, resultados = "ok" });

                }
            }
            catch (Exception ex)
            {
                return Json(new { estado = 0, resultados = ex.Message });
            }

        }

        public ActionResult UpdateListaArticulo()
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

                Session[_session_lista_articulo_precio] = null;
                ViewBag.Tipo = dat_precio.tipo_precio();

                List<Ent_Articulo_Precio> lista_precio = new List<Ent_Articulo_Precio>();

                Ent_Articulo_Precio articulo_precio = new Ent_Articulo_Precio();

                ViewBag.ListaPrecio = lista_precio;
                ViewBag.ArticuloPrecio = articulo_precio;

                return View();
            }
        }


        public ActionResult guardar_articulo_precio(List<Ent_Articulo_Precio> listarticulo_precio)
        {


            string mensaje = "";
            string estado = "0";
            try
            {
                Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];

                string grabar = dat_precio.update_lista_precio(listarticulo_precio, _usuario.usu_id,_usuario.usu_nom_ape);

                if (grabar.Length==0)
                {
                    estado = "1";
                    mensaje = "Lista actualizado correctamente..";
                    Session[_session_lista_articulo_precio] = null;

                }
                else
                {
                    estado = "0";
                    mensaje = grabar;
                }


            }
            catch (Exception exc)
            {
                estado = "0";
                mensaje = exc.Message;
            }

            return Json(new { estado = estado, mensaje = mensaje });
        }

        public ActionResult del_articulo_precio(string tipo, string articulo)
        {


            string mensaje = "";
            string estado = "0";
            try
            {

                List<Ent_Articulo_Precio> listarticulo_precio = (List<Ent_Articulo_Precio>)Session[_session_lista_articulo_precio];


                var existe_data = listarticulo_precio.Where(b => b.tipo == tipo && b.articulo == articulo).ToList();

                if (existe_data.Count>0)
                {
                    listarticulo_precio.Remove(existe_data[0]);
                    Session[_session_lista_articulo_precio] = listarticulo_precio;
                    estado = "1";
                    mensaje = "Registro eliminado..";
                }

               

            }
            catch (Exception exc)
            {
                estado = "0";
                mensaje = exc.Message;
            }

            return Json(new { estado = estado, mensaje = mensaje });
        }

        public ActionResult add_articulo_precio(string tipo, string articulo)
        {
          

            string mensaje = "";
            string estado = "0";
            try
            {               

                List<Ent_Articulo_Precio> listar_buscar = dat_precio.buscar_lista(articulo, tipo);             

                if (listar_buscar.Count == 0)
                {
                    estado = "0";
                    mensaje = "El Articulo Ingresado no existe..";
                }
                else
                {
                    List<Ent_Articulo_Precio> listarticulo_precio = (List<Ent_Articulo_Precio>)Session[_session_lista_articulo_precio];


                    var existe_lista = listarticulo_precio.Where(b => b.tipo == tipo && b.articulo == articulo).ToList();

                    if (existe_lista.Count>0)
                    {
                        estado = "0";
                        mensaje = "El Tipo y Codigo de articulo existe en la lista..";
                    }
                    else
                    {
                        Ent_Articulo_Precio obj = new Ent_Articulo_Precio();
                        obj.tipo = listar_buscar[0].tipo;
                        obj.tipodes = listar_buscar[0].tipodes;
                        obj.articulo = listar_buscar[0].articulo;
                        obj.descripcion = listar_buscar[0].descripcion;
                        obj.precioigv = listar_buscar[0].precioigv;
                        obj.precion = listar_buscar[0].precion;
                        obj.Art_Temporada = listar_buscar[0].Art_Temporada;
                        listarticulo_precio.Add(obj);
                        Session[_session_lista_articulo_precio] = listarticulo_precio;
                        estado = "1";
                        mensaje = "El codigo articulo se agrego..";
                    }
                    
                }
                
            }
            catch(Exception exc)
            {
                estado = "0";
                mensaje = exc.Message;
            }

            return Json(new { estado = estado, mensaje = mensaje });
        }

        public ActionResult getTableArticuloPrecioAjax(Ent_jQueryDataTableParams param)
        {
            /*verificar si esta null*/
            if (Session[_session_lista_articulo_precio] == null)
            {
                List<Ent_Articulo_Precio> listarticulo_precio = new List<Ent_Articulo_Precio>();
                Session[_session_lista_articulo_precio] = listarticulo_precio;
            }

            //}
            //Traer registros
            IQueryable<Ent_Articulo_Precio> membercol = ((List<Ent_Articulo_Precio>)(Session[_session_lista_articulo_precio])).AsQueryable();

            //Manejador de filtros
            int totalCount = membercol.Count();



            IEnumerable<Ent_Articulo_Precio> filteredMembers = membercol;

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = membercol
                    .Where(m => m.articulo.ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.Art_Temporada.ToUpper().Contains(param.sSearch.ToUpper()) 
                     );
            }


            //Manejador de orden
            var displayMembers = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);

            var result = from a in displayMembers
                         select new
                         {
                             a.tipo,
                             a.tipodes,
                             a.articulo,
                             a.descripcion,
                             a.precioigv,
                             a.precion,
                             a.Art_Temporada,
                             
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

        #region<Vista de Articulo - Foto - Popup>

        public ActionResult Articulo_View(string codArticulo)
        {
            Dat_Pedido datPedido = new Dat_Pedido();
            Ent_Articulo_pedido articulo = new Ent_Articulo_pedido();
            List<Ent_Articulo_Tallas> tallas = new List<Ent_Articulo_Tallas>();

            //string codArticulo = "7016678";

            datPedido.listarStr_ArticuloTalla(codArticulo, 0, ref articulo, ref tallas);

            ViewBag.DataArticulo = articulo;

            return View();
        }

        #endregion

        #region<REGION DE PREMIOS DE ARTICULOS>

        private string _session_lista_premios = "_session_lista_premios";
        private string _session_lista_premios_articulos = "_session_lista_premios_articulos";
        private string _session_lista_premios_busqueda_articulos = "_session_lista_premios_busqueda_articulos";
        private string _session_lista_premios_lista_articulos = "_session_lista_premios_lista_articulos";

        private Dat_Premios premio = new Dat_Premios();
        public ActionResult Premios_Articulos()
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
                Session[_session_lista_premios] = null;
                return View();
            }
        }

        public ActionResult getTablePremioArticuloAjax(Ent_jQueryDataTableParams param, string actualizar,string IdPremio)
        {

            List<Ent_Premios_Articulo> liststock = new List<Ent_Premios_Articulo>();

            if (!String.IsNullOrEmpty(actualizar))
            {
                liststock = premio.lista_premios_articulo(Convert.ToInt32(IdPremio));
                //listAtributos = datOE.get_lista_atributos();
                Session[_session_lista_premios_articulos] = liststock;
               
            }

            /*verificar si esta null*/
            if (Session[_session_lista_premios_articulos] == null)
            {
                 liststock = new List<Ent_Premios_Articulo>();
                 Session[_session_lista_premios_articulos] = liststock;
            }

            //}
            //Traer registros
            IQueryable<Ent_Premios_Articulo> membercol = ((List<Ent_Premios_Articulo>)(Session[_session_lista_premios_articulos])).AsQueryable();

            //Manejador de filtros
            int totalCount = membercol.Count();

            IEnumerable<Ent_Premios_Articulo> filteredMembers = membercol;


            //Manejador de orden
            var displayMembers = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);

            var result = from a in displayMembers
                         select new
                         {
                           
                             a.total,
                             a.articulo,
                             a.list_talla,                           
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

        public ActionResult getTablePremiosListaAjax(Ent_jQueryDataTableParams param)
        {
            /*verificar si esta null*/
            if (Session[_session_lista_premios] == null)
            {
                List<Ent_Premios> listpremios = new List<Ent_Premios>();
                Session[_session_lista_premios] = premio.lista_premios();
            }

            //}
            //Traer registros
            IQueryable<Ent_Premios> membercol = ((List<Ent_Premios>)(Session[_session_lista_premios])).AsQueryable();

            //Manejador de filtros
            int totalCount = membercol.Count();



            IEnumerable<Ent_Premios> filteredMembers = membercol;

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = membercol
                    .Where(m => m.descripcion.ToUpper().Contains(param.sSearch.ToUpper())
                     );
            }


            //Manejador de orden
            var displayMembers = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);

            var result = from a in displayMembers
                         select new
                         {
                             a.id,
                             a.descripcion,
                             a.monto,
                             a.stock,
                             a.stockingresado,                             
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

        public ActionResult borrar_articulo_premio(string id)
        {
            string mensaje = "";
            string estado = "0";
         
            try
            {

                Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];
                mensaje = premio.eliminar_articulo_premio(Convert.ToInt32(id), _usuario.usu_id);
                if (mensaje.Length != 0)
                {
                    estado = "0";                   
                }
                else
                {
                    estado = "1";
                    mensaje = "Se elimino con exito el articulo";
                }

            }
            catch(Exception exc)
            {
                estado = "0";
                mensaje = exc.Message;
            }       
            return Json(new { estado = estado, mensaje = mensaje });
        }
        public ActionResult agregar_articulo_premio(string idpremio,List<Ent_Premios_Articulo_Stock> lista_seleccion_articulos)
        {
            string mensaje = "";
            string estado = "0";

            try
            {
                string strDataDetalle = "";
                foreach (var obj in lista_seleccion_articulos)
                {
                    strDataDetalle += "<row  ";
                    strDataDetalle += " Codigo=¿" + obj.articulo + "¿ ";
                    strDataDetalle += " Talla=¿" + obj.talla + "¿ ";
                    strDataDetalle += " Cantidad=¿" + obj.cantidad + "¿ ";
                    strDataDetalle += " Precio=¿" + 0 + "¿ ";
                    strDataDetalle += "/>";
                }


                Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];

                mensaje = premio.insertar_articulo_premio(Convert.ToInt32(idpremio),strDataDetalle, _usuario.usu_id);
                if (mensaje.Length != 0)
                {
                    estado = "0";
                }
                else
                {
                    estado = "1";
                    mensaje = "Articulo agregado con exito..";
                    Session[_session_lista_premios_busqueda_articulos] = null;
                }

            }
            catch (Exception exc)
            {
                estado = "0";
                mensaje = exc.Message;
            }
            return Json(new { estado = estado, mensaje = mensaje });
        }

        public ActionResult Premios_Articulos_Lista(string idpremio,string des_premio)
        {

            if (idpremio == null)
            {
                return RedirectToAction("Premios_Articulos", "Articulo");
            }
            else
            {
                List<Ent_Premios_Articulo_Stock> lista_seleccion_articulos = new List<Ent_Premios_Articulo_Stock>();

                Ent_Premios_Articulo_Stock obj_articulo = new Ent_Premios_Articulo_Stock();

                ViewBag.ListaArticulos = lista_seleccion_articulos;
                ViewBag.ArticuloObj = obj_articulo;

                ViewBag.idpremio = idpremio;

                ViewBag.des_premio = des_premio;

                Session[_session_lista_premios_busqueda_articulos] = null;
                Session[_session_lista_premios_lista_articulos] = null;

                return View();
            }

            
        }

        public ActionResult getTablePremioArticuloBusquedaAjax(Ent_jQueryDataTableParams param, string actualizar, string articulo)
        {            

            List<Ent_Premios_Articulo_Stock> liststock = new List<Ent_Premios_Articulo_Stock>();

            if (!String.IsNullOrEmpty(actualizar))
            {
                liststock = premio.lista_premios_articulo_stock(articulo);
                //listAtributos = datOE.get_lista_atributos();
                Session[_session_lista_premios_busqueda_articulos] = liststock;

            }

            /*verificar si esta null*/
            if (Session[_session_lista_premios_busqueda_articulos] == null)
            {
                liststock = new List<Ent_Premios_Articulo_Stock>();
                Session[_session_lista_premios_busqueda_articulos] = liststock;
            }

            //}
            //Traer registros
            IQueryable<Ent_Premios_Articulo_Stock> membercol = ((List<Ent_Premios_Articulo_Stock>)(Session[_session_lista_premios_busqueda_articulos])).AsQueryable();

            //Manejador de filtros
            int totalCount = membercol.Count();

            IEnumerable<Ent_Premios_Articulo_Stock> filteredMembers = membercol;


            //Manejador de orden
            var displayMembers = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);

            var result = from a in displayMembers
                         select new
                         {

                             a.articulo,
                             a.talla,
                             a.stock,
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

        public ActionResult getTablePremioArticuloListaAjax(Ent_jQueryDataTableParams param, string actualizar, string idpremio)
        {

            List<Ent_Lista_PremiosXArticulos> liststock = new List<Ent_Lista_PremiosXArticulos>();

            if (!String.IsNullOrEmpty(actualizar))
            {
                liststock = premio.lista_premiosXarticulo(Convert.ToInt32(idpremio));
                //listAtributos = datOE.get_lista_atributos();
                Session[_session_lista_premios_lista_articulos] = liststock;

            }

            /*verificar si esta null*/
            if (Session[_session_lista_premios_lista_articulos] == null)
            {
                liststock = new List<Ent_Lista_PremiosXArticulos>();
                Session[_session_lista_premios_lista_articulos] = liststock;
            }

            //}
            //Traer registros
            IQueryable<Ent_Lista_PremiosXArticulos> membercol = ((List<Ent_Lista_PremiosXArticulos>)(Session[_session_lista_premios_lista_articulos])).AsQueryable();

            //Manejador de filtros
            int totalCount = membercol.Count();

            IEnumerable<Ent_Lista_PremiosXArticulos> filteredMembers = membercol;


            //Manejador de orden
            var displayMembers = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);

            var result = from a in displayMembers
                         select new
                         {
                             a.id,
                             a.articulo,
                             a.talla,
                             a.cantidad,
                             a.entregado,
                             a.stock
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

        #region<Listar Precios de los articulos>
        public ActionResult ListarArticulo()
        {
             //Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];

             //string actionName = this.ControllerContext.RouteData.GetRequiredString("action");
             //string controllerName = this.ControllerContext.RouteData.GetRequiredString("controller");
             //string return_view = actionName + "|" + controllerName;

             //if (_usuario == null)
             //{
             //    return RedirectToAction("Login", "Control", new { returnUrl = return_view });
             //}
             //else
             //{
             //    Session[_session_listaArticuloPrecios] = Dat_Articulo.ListaPrecios();
             //    return View();
             //}
             
            Session[_session_listaArticuloPrecios] = Dat_Articulo.ListaPrecios();
            return View();
        }

        /****/

        public ActionResult getListArticulosAjax(Ent_jQueryDataTableParams param)
        {

            // List<Ent_Articulo> listArticulo = new List<Ent_Articulo>();
            /*verificar si esta null*/
            /*f (Session[_session_listCliente_private] == null)
             {
                 listArticulo = new List<Ent_Articulo>();
                 listArticulo = lista(); //datOE.get_lista_atributos();
                 if (listcliente == null)
                 {
                     listArticulo = new List<Ent_Articulo>();
                 }
                 Session[_session_listaArticuloPrecios] = listArticulo;
             }*/

            //Traer registros
            var membercol = ((List<Ent_Articulo>)(Session[_session_listaArticuloPrecios])).Select(x => new ArticuloViewModel
            {
                IdArticulo = x.Art_Id,
                Cat_Principal = x.Ent_CategoriaPrincipal.Cat_Pri_Descripcion,
                SubCategoria = x.Ent_SubCategoria.Sca_Descripcion,
                Marca = x.Ent_Marca.Mar_Descripcion,
                Descripcion = x.Art_Descripcion,
                PrecioIgv = x.precioigv,
                PrecioSinIgv = x.preciosinigv,
                Costo = x.costo
            }).ToList();

            //Manejador de filtros
            int totalCount = membercol.Count();
            IEnumerable<ArticuloViewModel> filteredMembers = membercol;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = membercol
                    .Where(m => m.IdArticulo.ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.Cat_Principal.ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.SubCategoria.ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.Marca.ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.Descripcion.ToUpper().Contains(param.sSearch.ToUpper())
                     );
            }

            //Manejador de orden
            var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);

            if (param.iSortingCols > 0)
            {
                if (Request["sSortDir_0"].ToString() == "asc")
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderBy(o => o.IdArticulo); break;
                        case 1: filteredMembers = filteredMembers.OrderBy(o => o.Cat_Principal); break;
                        case 2: filteredMembers = filteredMembers.OrderBy(o => o.SubCategoria); break;
                        case 3: filteredMembers = filteredMembers.OrderBy(o => o.Marca); break;
                        case 4: filteredMembers = filteredMembers.OrderBy(o => o.Descripcion); break;
                    }
                }
                else
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderByDescending(o => o.IdArticulo); break;
                        case 1: filteredMembers = filteredMembers.OrderByDescending(o => o.Cat_Principal); break;
                        case 2: filteredMembers = filteredMembers.OrderByDescending(o => o.SubCategoria); break;
                        case 3: filteredMembers = filteredMembers.OrderByDescending(o => o.Marca); break;
                        case 4: filteredMembers = filteredMembers.OrderByDescending(o => o.Descripcion); break;
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

        public FileContentResult ListaArticulosPrecioExcel()
        {
            string excelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var Lista = ((List<Ent_Articulo>)(Session[_session_listaArticuloPrecios])).Select(x => new ArticuloViewModel
            {
                IdArticulo = x.Art_Id,
                Cat_Principal = x.Ent_CategoriaPrincipal.Cat_Pri_Descripcion,
                SubCategoria = x.Ent_SubCategoria.Sca_Descripcion,
                Marca = x.Ent_Marca.Mar_Descripcion,
                Descripcion = x.Art_Descripcion,
                PrecioIgv = x.precioigv,
                PrecioSinIgv = x.preciosinigv,
                Costo = x.costo
            }).ToList();

            var ListCount = Lista.Count();
            int row = 5;

            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Articulos");
            //Titulo
            Sheet.Cells["C2:F2"].Merge = true;
            Sheet.Cells["C2"].Value = "LISTA DE ARTICULOS - CATALOGO - BATA";
            Sheet.Cells["C2"].Style.Font.Size = 24;
            Sheet.Cells["C2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            //Cabecera
            Sheet.Cells["B4"].Value = "Articulo";
            Sheet.Cells["C4"].Value = "Categoría";
            Sheet.Cells["D4"].Value = "Sub Categoría";
            Sheet.Cells["E4"].Value = "Marca";
            Sheet.Cells["F4"].Value = "Descripción";
            Sheet.Cells["G4"].Value = "Precio Inc(Igv)";
            Sheet.Cells["H4"].Value = "Precio Sin(Igv)";
            Sheet.Cells["I4"].Value = "Precio Costo";
            //Formato de cabecera
            Sheet.Cells["B1:I4"].Style.Font.Bold = true;
            Sheet.Cells["B4:I4"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            Sheet.Cells["B4:I4"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightSkyBlue);
            Sheet.Cells["B4:I4"].Style.Font.Color.SetColor(System.Drawing.Color.White);
            //Estilo al cuerpo del excel
            using (var range = Sheet.Cells[4, 2, ListCount + 4, 9])
            {
                range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            }
            //Carga datos      
            foreach (var item in Lista)
            {
                Sheet.Cells[string.Format("B{0}", row)].Value = item.IdArticulo;
                Sheet.Cells[string.Format("B{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                Sheet.Cells[string.Format("C{0}", row)].Value = item.Cat_Principal;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.SubCategoria;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.Marca;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.Descripcion;

                Sheet.Cells[string.Format("G{0}", row)].Value = "S/ " + Convert.ToDecimal(string.Format("{0:F2}", item.PrecioIgv));
                Sheet.Cells[string.Format("G{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                Sheet.Cells[string.Format("H{0}", row)].Value = "S/ " + Convert.ToDecimal(string.Format("{0:F2}", item.PrecioSinIgv));
                Sheet.Cells[string.Format("H{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                Sheet.Cells[string.Format("I{0}", row)].Value = "S/ " + Convert.ToDecimal(string.Format("{0:F2}", item.Costo));
                Sheet.Cells[string.Format("I{0}", row)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            var stream = new MemoryStream(Ep.GetAsByteArray());
            return File(stream.ToArray(), excelContentType, "Lista de Articulos.xlsx");
        }
        #endregion
    }
}