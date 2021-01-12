using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalDecisions.Shared;

namespace CapaPresentacion.AspNetForms
{
    public partial class reportPicking : System.Web.UI.Page
    {
        ReportDocument _pickObjReport = null;
        
        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {
                string strReportName = System.Web.HttpContext.Current.Session["ReportName"].ToString();
                string Liq_Id = System.Web.HttpContext.Current.Session["Liq_Id"].ToString();
                string typeFile = System.Web.HttpContext.Current.Session["typeFile"].ToString();
                var rptSource = System.Web.HttpContext.Current.Session["rptSource"];
                // Ubicacion del reporte crystal
                string strRptPath = Server.MapPath("~/") + "RptsCrystal//" + strReportName;
                string nombreArchivo = "Picking_" + Liq_Id;
                // Instanciar el objeto de reporte de crystal
                _pickObjReport = new ReportDocument();
                // Enlazar el archivo del reporte y el objeto instanciado
                _pickObjReport.Load(strRptPath);
                // Establecer el dataSource dirigido al reporte crystal
                _pickObjReport.SetDataSource(rptSource);
                if (typeFile == "Excel")
                {
                    _pickObjReport.ExportToHttpResponse(ExportFormatType.Excel, Response, true, nombreArchivo);
                }
                else
                {
                    _pickObjReport.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, true, nombreArchivo);
                }
                // Objeto crystal reports presente en la pagina aspx
                crvPicking.ReportSource = _pickObjReport;
            }
            catch (Exception exc)
            {
            }
        }

        protected void Page_Unload(object sender, EventArgs e)
        {
            /// Reporte generalizado
            if ((_pickObjReport != null) && _pickObjReport.IsLoaded)
            {
                _pickObjReport.Close();
                _pickObjReport.Dispose();
            }
        }
    }
}