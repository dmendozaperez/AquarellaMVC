using CapaDato.ValeCompra;
using CapaEntidad.ValeCompra;
using CapaEntidad.Control;
using CapaEntidad.Menu;
using CapaEntidad.Util;
using CapaPresentacion.Bll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

using System.IO;
using System.IO.Compression;
using System.Text;

namespace CapaPresentacion.Controllers
{
    public class ValeCompraController : Controller
    {
        // GET: ValeCompra
        private Dat_ValeCompra datvalecompra = new Dat_ValeCompra();
        private Dat_Cliente datCliente = new Dat_Cliente();
        private string _session_listvalcompra_private = "session_listvalcompra_private";

      
        [Authorize]
        public ActionResult Index()
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
                    ViewBag.cliente = datCliente.get_lista();

                    return View(lista());
                }
                else
                {
                    return RedirectToAction("Login", "Control", new { returnUrl = return_view });
                }
            }

        }

        public PartialViewResult ListaValeCompra()
        {
            return PartialView(lista());
        }

        public List<Ent_ValeCompra> lista()
        {
            List<Ent_ValeCompra> listvaleCompra = datvalecompra.get_lista();
            Session[_session_listvalcompra_private] = listvaleCompra;
            return listvaleCompra;
        }

        public ActionResult Nuevo()
        {

            ViewBag.cliente = datCliente.get_lista();
            return View();

        }
        

        public ActionResult ProcesarPdf(string IdValeCompra)
        {
            Boolean _valida_nuevo = false;

            Dat_ValeCompra datCompra = new Dat_ValeCompra();
            Ent_ValeCompra entCompra = new Ent_ValeCompra();

            entCompra = datCompra.listarCupones(IdValeCompra);
            ejecuta_pdf(entCompra);
            _valida_nuevo = true;

            return Json(new { estado = (_valida_nuevo) ? "1" : "-1", desmsg = (_valida_nuevo) ? "Se actualizo satisfactoriamente." : "Hubo un error al actualizar." });
        }


        public ActionResult Consulta(string Id)
        {
            Dat_ValeCompra datCompra = new Dat_ValeCompra();

            Ent_ValeCompra _valeCompra = new Ent_ValeCompra();

            
                _valeCompra = datCompra.ConsultarVales(Id);
                return View(_valeCompra);

         

        }

        //public JsonResult GuardarVale(Ent_ValeCompra _valeCompra)
        //{
        //    var oJRespuesta = new JsonResponse();
        //    Ent_Usuario _usuario = (Ent_Usuario)Session[Ent_Constantes.NameSessionUser];

        //    Dat_ValeCompra datCompra = new Dat_ValeCompra();
        //    Ent_ValeCompra _compra = new Ent_ValeCompra();

        //    _valeCompra.valCompra_usuCreacion = _usuario.usu_login;
        //    _valeCompra.valCompra_ipCreacion = _usuario.usu_ip;

        //    datCompra.valeCompra = _valeCompra;
          
        //    _compra = datCompra.InsertarGeneracionValeCompraEntidad();
            

        //    ejecuta_pdf(_compra);

        //    oJRespuesta.Message = _compra.valCompra_id.ToString();

        //    return Json(oJRespuesta, JsonRequestBehavior.AllowGet);
        //}

        public FileResult Download(string strIdLote)
        {
            string directorio = System.Web.HttpContext.Current.Server.MapPath(Ent_Conexion.strDirectorio);
            byte[] fileBytes = System.IO.File.ReadAllBytes(directorio + strIdLote + ".zip");
            string fileName = "Bata_" + strIdLote + ".zip";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        private static bool ActualizarValCompra(Ent_ValeCompra _valeCompra)
        {
           
           Dat_ValeCompra datCompra = new Dat_ValeCompra();
            Boolean actualizado = false;

            datCompra.valeCompra = _valeCompra;
            actualizado = datCompra.ActualizarValeCompra();

            return actualizado;
         }

        public static void ejecuta_pdf(Ent_ValeCompra _valeCompra)
        {
           
            try
            {
          
                if (_valeCompra != null)
                {
                    string file_html = System.Web.HttpContext.Current.Server.MapPath(Ent_Conexion.plantilla);
                    
                    string strDirectorio = System.Web.HttpContext.Current.Server.MapPath(Ent_Conexion.strDirectorio);
                    string _codigoCliente = _valeCompra.cli_codigo;
                    string _razon = _valeCompra.valCompra_razon;
                    string _ruc = _valeCompra.valCompra_ruc;
                    string _direccion = _valeCompra.cli_Direccion;
                    string _fechaIni = _valeCompra.valCompra_fecVigenIni;
                    string _fechaFin = _valeCompra.valCompra_fecVigenFin;
                    string _carpeta = (_valeCompra.valCompra_id).ToString();
                    string path_destination = "";
                    string path = strDirectorio + _carpeta;
                    path_destination = path;

                    DirectoryInfo di = Directory.CreateDirectory(path);
                    //Console.WriteLine("The directory was created successfully at {0}.", Directory.GetCreationTime(path));

                    string readContentsprincipal;
                        using (StreamReader streamReader = new StreamReader(@file_html, Encoding.UTF8))
                        {
                            readContentsprincipal = streamReader.ReadToEnd();
                        }

                        foreach(Ent_ValeCompraDetalle item in _valeCompra.valCompra_ListDetalle)
                        {

                            string _barra = item.valCom_codeBarra.ToString();
                            string _montovale = item.valComDet_monto.ToString("0.00");
                            string _montoletra = item.valComDet_montoLetras.ToString();
                            string _interno = item.valComDet_correlativo.ToString();
                            
                        string _readcontentsecundario = readContentsprincipal;

                            _readcontentsecundario = _readcontentsecundario.Replace("xxxxxxxxxxxxxxx", _barra);
                            _readcontentsecundario = _readcontentsecundario.Replace("MONTOS", _montovale);
                            _readcontentsecundario = _readcontentsecundario.Replace("MONTOL", _montoletra);
                            _readcontentsecundario = _readcontentsecundario.Replace("INTERNO", _interno);

                            _readcontentsecundario = _readcontentsecundario.Replace("CodigoCliente", _codigoCliente);
                            _readcontentsecundario = _readcontentsecundario.Replace("RucCliente", _ruc);
                            _readcontentsecundario = _readcontentsecundario.Replace("RazonCliente", _razon);
                            _readcontentsecundario = _readcontentsecundario.Replace("DireccionCliente", _direccion);
                            _readcontentsecundario = _readcontentsecundario.Replace("fechaIni", _fechaIni);
                            _readcontentsecundario = _readcontentsecundario.Replace("fechaFin", _fechaFin);

                        string _file_pdf = "BATA_" + _interno +"_"+ _montovale + ".pdf";
                        string _file_path_pdf = path_destination + "\\" + _file_pdf.ToString();

                            GeneraPDF(_readcontentsecundario, _file_path_pdf);
                        }

                    string startPath = strDirectorio + _carpeta;
                    string zipPath = strDirectorio + _carpeta+".zip";

                    if (System.IO.File.Exists(zipPath))
                    {
                        System.IO.File.Delete(zipPath);
                    }

                    ZipFile.CreateFromDirectory(startPath, zipPath);
                    _valeCompra.valCompra_generado = "S";
                    ActualizarValCompra(_valeCompra);

                    System.IO.Directory.Delete(startPath, true);

                    //di.Delete();

                }

            }
            catch
            {

            }
        }
        private static bool GeneraPDF(string readContents, string _file_pdf_destino)
        {
            Boolean _valida = false;
            try
            {
                var htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter();
                htmlToPdf.PageHeight = 242;
                htmlToPdf.PageWidth = 170;
                var margins = new NReco.PdfGenerator.PageMargins();
                margins.Bottom = 2;
                margins.Top = 1;
                margins.Left = 2;
                margins.Right = 5;
                htmlToPdf.Margins = margins;
                htmlToPdf.Orientation = NReco.PdfGenerator.PageOrientation.Portrait;
                htmlToPdf.Zoom = 1F;
                htmlToPdf.Size = NReco.PdfGenerator.PageSize.A4;
                var pdfBytes = htmlToPdf.GeneratePdf(readContents);
                System.IO.File.WriteAllBytes(@_file_pdf_destino, pdfBytes);
                _valida = true;
            }
            catch
            {
                _valida = false;
            }
            return _valida;
        }


    }
}