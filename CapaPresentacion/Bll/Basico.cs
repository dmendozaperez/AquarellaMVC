using CapaEntidad.Menu;
using CapaEntidad.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CapaPresentacion.Bll
{
    public class Basico:Controller
    {
        public Boolean acc()
        {

            Boolean valor = true; 
            return valor;
        }
        public Boolean AccesoMenu(List<Ent_Menu_Items> menu, Controller cont)
        {
            Boolean valida = false;
            try
            {
                string actionName = cont.ControllerContext.RouteData.GetRequiredString("action");
                string controllerName = cont.ControllerContext.RouteData.GetRequiredString("controller");

                var existe = menu.Where(t => t.action == actionName && t.controller == controllerName).ToList();

                if (existe.Count > 0) valida = true;

            }
            catch
            {

                valida = false;
            }
            return valida;
        }

        #region<METODO PARA GENERAR GENERAR DE PDF>       
        private static DataTable dtbarra()
        {
            String sqlquery = "USP_GetBarraGenera";
            DataTable dt = null;
            SqlConnection cn = null;
            SqlCommand cmd = null;
            SqlDataAdapter da = null;
            try
            {
                cn = new SqlConnection(Ent_Conexion.conexion);
                cmd = new SqlCommand(sqlquery, cn);
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.StoredProcedure;
                da = new SqlDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);
            }
            catch
            {
                dt = null;
            }
            return dt;
        }
        public static void ejecuta_pdf(string file_html, string path_destination, ref Int32 _contar)
        {
            DataTable dt = null;
            try
            {
                dt = dtbarra();

                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {

                        string readContentsprincipal;
                        using (StreamReader streamReader = new StreamReader(@file_html, Encoding.UTF8))
                        {
                            readContentsprincipal = streamReader.ReadToEnd();
                        }

                        for (Int32 i = 0; i < dt.Rows.Count; ++i)
                        {
                            _contar += 1;
                            string _barra = dt.Rows[i]["Barra"].ToString();
                            string _montovale = dt.Rows[i]["MontoVale"].ToString();
                            string _montoletra = dt.Rows[i]["MontLetras"].ToString();
                            string _interno = dt.Rows[i]["Correlativo"].ToString();
                            string _readcontentsecundario = readContentsprincipal;
                            _readcontentsecundario = _readcontentsecundario.Replace("xxxxxxxxxxxxxxx", _barra);
                            _readcontentsecundario = _readcontentsecundario.Replace("MONTOS", _montovale);
                            _readcontentsecundario = _readcontentsecundario.Replace("MONTOL", _montoletra);
                            _readcontentsecundario = _readcontentsecundario.Replace("INTERNO", _interno);

                            string _montovale_int = Convert.ToInt32(dt.Rows[i]["MontoVale"]).ToString();

                            string _file_pdf = "BATA_" + _barra.ToString() + "_" + _montovale_int + ".pdf";
                            string _file_path_pdf = path_destination + "\\" + _file_pdf.ToString();

                            GeneraPDF(_readcontentsecundario, _file_path_pdf);
                        }
                    }
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
                //File.WriteAllBytes(@_file_pdf_destino, pdfBytes);
                _valida = true;
            }
            catch
            {
                _valida = false;
            }
            return _valida;
        }
        #endregion
    }


}