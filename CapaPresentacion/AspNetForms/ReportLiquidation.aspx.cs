using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CapaPresentacion.AspNetForms
{
    public partial class ReportLiquidation : System.Web.UI.Page
    {
        ReportDocument rd = null;
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {
                string strReportName = System.Web.HttpContext.Current.Session["ReportName"].ToString();
                var rptSource = System.Web.HttpContext.Current.Session["rptSource"];
                var rptSource1 = System.Web.HttpContext.Current.Session["rptSource1"];
                var rptSource2 = System.Web.HttpContext.Current.Session["rptSource2"];

                rd = new ReportDocument();

                string strRptPath = Server.MapPath("~/") + "RptsCrystal//" + strReportName;

                rd.Load(strRptPath);

                // Setting report data source
                if (rptSource != null && rptSource.GetType().ToString() != "System.String")
                    rd.SetDataSource(rptSource);

                rd.OpenSubreport("pagonc").SetDataSource(rptSource1);
                rd.OpenSubreport("pagoforma").SetDataSource(rptSource2);

                var download = System.Web.HttpContext.Current.Session["rptDownload"];

                if ((bool)download)
                {
                    DownloadPDF();
                }
                else
                {
                    crv.ReportSource = rd;
                    crv.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None;
                    crv.HasToggleGroupTreeButton = false;
                    crv.Zoom(93);
                }

                
            }
            catch (Exception exc)
            {
            }
        }
        protected void Page_Unload(object sender, EventArgs e)
        {
            /// Reporte generalizado
            if ((rd != null) && rd.IsLoaded)
            {
                rd.Close();
                rd.Dispose();
            }
        }
        protected void Print(object sender, EventArgs e)
        {

            rd.Refresh();
            rd.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;
            // Set Paper Size.
            rd.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;

            rd.PrintOptions.PrinterName = GetDefaultPrinter();
            rd.PrintToPrinter(1, true, 0, 0);
        }
        private string GetDefaultPrinter()
        {
            PrinterSettings settings = new PrinterSettings();
            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
                settings.PrinterName = printer;
                if (settings.IsDefaultPrinter)
                {
                    return printer;
                }
            }
            return string.Empty;
        }

        protected void Unnamed1_Click(object sender, EventArgs e)
        {
            DownloadPDF();
        }
        private void DownloadPDF()
        {
            try
            {
                ExportFormatType formatType = ExportFormatType.NoFormat;
                formatType = ExportFormatType.PortableDocFormat;
                rd.ExportToHttpResponse(formatType, Response, true, "liquidation");
                Response.End();
            }
            catch (Exception)
            {
            }
        }
    }
}