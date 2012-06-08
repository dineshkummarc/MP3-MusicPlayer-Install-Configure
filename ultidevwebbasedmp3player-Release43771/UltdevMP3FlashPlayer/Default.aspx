<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="UltdevMP3FlashPlayer._Default" %>
<%@ Register Src="Controls/PlayerControl.ascx" TagName="PlayerControl" TagPrefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=EmulateIE7" />
    <title>UltiDev MP3 Player Application Sample</title>
    <link href="Controls/PlayerControlDefaultStyle.css" rel="stylesheet" type="text/css" />
    <meta http-equiv="Page-Enter" content="progid:DXImageTransform.Microsoft.Fade(Duration=0)">
    <link href="/Controls/PlayerControlDefaultStyle.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div align="center">
        <asp:ScriptManager ID="ajaxScriptManager" runat="server"></asp:ScriptManager>
        <table border="0" cellspacing="1" cellpadding="5" class="UltiDevPlayerControl">
            <tr>
                <td align="center">
        <h3><a href="http://ultidev.com/products/httpvpn/" target="_blank">UltiDev HttpVPN™</a> and 
            <asp:HyperLink ID="cassiniServerHyperLink" runat="server" Target="_blank">UltiDev Cassini Web Server</asp:HyperLink>
            for ASP.NET 2.0 &amp; 3.x&nbsp;</h3>
            
            <h4>making your web applications accessible on the Internet right after the installation.</h4>
                </td>
            </tr>
        </table>
        <uc1:PlayerControl id="PlayerControl1" runat="server">
        </uc1:PlayerControl>
        </div>
    </form>
</body>
</html>
