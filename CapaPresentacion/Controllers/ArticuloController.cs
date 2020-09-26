using CapaDato.Articulo;
using CapaEntidad.Articulo;
using CapaEntidad.Control;
using CapaEntidad.General;
using CapaEntidad.Util;
using CapaPresentacion.Bll;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace CapaPresentacion.Controllers
{
    public class ArticuloController : Controller
    {
        // GET: Articulo
        private string _session_stock_x_articulo = "_session_stock_x_articulo";
        private string _session_stock_x_articulo_filtro = "_session_stock_x_articulo_filtro";

        private string _session_pedido_sin_stock = "_session_pedido_sin_stock";
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

            Dat_Articulo_Stock stk_articulo = new Dat_Articulo_Stock();
            ViewBag.Categoria=stk_articulo.listar_categoria_principal();
            ViewBag.Temporada=stk_articulo.listar_temporada();

            Session[_session_stock_articulo_categoria] = null;
            Session[_session_stock_articulo_categoria_excel] = null;

            return View();
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

        #endregion
    }
}