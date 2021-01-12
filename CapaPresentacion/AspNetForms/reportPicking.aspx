<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="reportPicking.aspx.cs"  Inherits="CapaPresentacion.AspNetForms.reportPicking" %>
<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304"  Namespace="CrystalDecisions.Web" TagPrefix="CR" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <CR:CrystalReportViewer ID="crvPicking" runat="server" HasCrystalLogo="False"  HasRefreshButton="True" />
    </div>
    </form>
</body>
</html>
