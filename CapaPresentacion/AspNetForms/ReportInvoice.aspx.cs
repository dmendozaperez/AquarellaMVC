using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CapaPresentacion.AspNetForms
{
    public partial class ReportInvoice : System.Web.UI.Page
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

                rd.OpenSubreport("VentaPNC").SetDataSource(rptSource1);
                rd.OpenSubreport("formapago").SetDataSource(rptSource2);

                //var download = System.Web.HttpContext.Current.Session["rptDownload"];

                crv.ReportSource = rd;
                crv.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None;
                crv.HasToggleGroupTreeButton = false;
                crv.Zoom(93);
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
    }
}