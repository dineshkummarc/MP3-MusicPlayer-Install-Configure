<%@ Page Language="C#" AutoEventWireup="true" aspcompat="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<%@ Register Src="Controls/PlayerControl.ascx" TagName="PlayerControl" TagPrefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>UltiDev MP3 Player Application Sample</title>
</head>
<body>
    <form id="form1" runat="server">
    <div align="center">
        <table border="0" cellspacing="1" cellpadding="5" class="UltiDevPlayerControl">
            <tr>
                <td align="center">
        <h2>Showcasing 
            <asp:HyperLink ID="cassiniServerHyperLink" runat="server" Target="_blank">UltiDev Cassini Web Server</asp:HyperLink>
            for ASP.NET 2.0</h2>
        Click the link above to learn about how to package your ASP.NET application
        with<br />the <a href="http://ultidev.com/download/" target="_blank">free</a> and <a href="http://ultidev.com/Products/Cassini/CassiniDevGuide.htm" target="_blank">redistributable</a>
                    UltiDev Cassini Web Server.<br /><br />
                    <h5>Windows Media Center users, consider <a href="http://www.asciiexpress.com/webguide/" target="_blank">WebGuide4</a> for web access<br />
                    to your music, recorded TV, pictres and videos.</h5>
                </td>
            </tr>
        </table>
        <br />
        <uc1:PlayerControl id="PlayerControl1" runat="server">
        </uc1:PlayerControl></div>
    </form>
</body>
</html>
