<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AccessRights.aspx.cs" Inherits="AccessRights" %>

<%@ Register Src="Controls/FolderTreeControl.ascx" TagName="FolderTreeControl" TagPrefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Guest Music Folder Selection</title>
    <link href="Controls/PlayerControlDefaultStyle.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <div style="text-align: center">
            <table>
                <tr>
                    <td>
        <table class="UltiDevPlayerControl">
            <tr>
                <td style="white-space: nowrap;" colspan="2">
                    <asp:Panel ID="guestUserFolderPanel" runat="server">
                        Guest User Music Folder:
                        <asp:Label ID="guestMusicFolderLabel" runat="server" Font-Bold="True" Font-Size="Larger"></asp:Label>
                        <asp:Button ID="clearGuestLocationButton" runat="server" OnClick="clearGuestLocationButton_Click"
                            OnClientClick="return confirm('Are you sure you want to stop others from accessing your music?');"
                            Text="Clear" ToolTip="Click to prohibit guests accessing any folder" /></asp:Panel>
                </td>
            </tr>
            <tr>
                <td align="center" style="font-size: larger">
                    Select a sub-folder from which<br />
                    guest users will be able to select music to play.</td>
                <td align="center" style="font-size: larger" valign="bottom">
                    <asp:Label ID="guestPromptLabel" runat="server" Text="The tree guests will see:"></asp:Label></td>
            </tr>
            <tr>
                <td align="center" valign="top"><div>
                    <table>
                        <tr>
                            <td align="left">
        <uc1:FolderTreeControl ID="folderTreeControl" runat="server" />
                            </td>
                        </tr>
                    </table>
                    </div>
                    <br />
                    </td>
                <td align="center" valign="top">
                    <table>
                        <tr>
                            <td align="left" style="width: 100px">
                                <uc1:FolderTreeControl ID="guestFolderTreeControl" runat="server" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td align="center" colspan="2" valign="top">
                    <asp:HyperLink ID="doneHyperLink" runat="server" NavigateUrl="javascript:window.close();" Font-Size="Large">Done</asp:HyperLink></td>
            </tr>
        </table>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
