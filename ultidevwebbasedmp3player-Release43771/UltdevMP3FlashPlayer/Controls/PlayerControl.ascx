<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PlayerControl.ascx.cs" Inherits="UltdevMP3FlashPlayer.Controls.PlayerControl" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<%@ Register Src="FolderTreeControl.ascx" TagName="FolderTreeControl" TagPrefix="uc1" %>
<link href="/Controls/PlayerControlDefaultStyle.css" rel="stylesheet" type="text/css"/>
<%--<object classid="d27cdb6e-ae6d-11cf-96b8-444553540000" codebase="https://fpdownload.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=10,0,0,0"
    width="400" height="168">
    <param name="allowScriptAccess" value="sameDomain" />
    <param name="movie" value="/Player/xspf_player.swf" />
    <param name="quality" value="high" />
    <param name="bgcolor" value="#E6E6E6" />
    <embed src="/Player/xspf_player.swf?playlist_url=/Handlers/Paylistlist.ashx?C:\Documents and Settings\n1vhrybok\My Documents\My Music"
        quality="high" bgcolor="#E6E6E6" name="xspf_player" allowscriptaccess="sameDomain"
        type="application/x-shockwave-flash" pluginspage="http://www.macromedia.com/go/getflashplayer"
        align="center" height="168" width="400"> </embed>
</object>
--%>
<table cellpadding="0" cellspacing="0">
<tr>
    <td valign="top">
        <asp:Image ID="panelToggleImage" runat="server" 
            ImageControlID="panelToggleImage" Style="cursor:pointer" ImageUrl="~/Images/ExpandExplorer.PNG" /><br />
    </td>
    <td style="border-right: silver 1px solid; border-top: silver 1px solid; border-left: silver 1px solid; border-bottom: silver 1px solid" valign="top">
<table border="0" cellspacing="1" class="UltiDevPlayerControl" cellpadding="0">
    <tr>
        <td colspan="2" nowrap="nowrap" valign="top">
            <table width="100%">
                <tr>
                    <td align="left">
                        Now Playing Folder:&nbsp;
                    <asp:Label ID="currentFolderLabel" runat="server" CssClass="CurrentFolderName" /></td>
                    <td align="right">
        <asp:HyperLink ID="accessHyperLink" runat="server"
            NavigateUrl="~/AccessRights.aspx" ToolTip="Click to designate folder with music that will be accessible to people you invite to listen to your music." Font-Bold="False" Font-Size="Larger" Target="_blank">Music for Guests</asp:HyperLink></td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td nowrap="nowrap" valign="top" align="left">
            <asp:Panel ID="selectFolderPanel" runat="server" CssClass="NewFolderTreePanel">
<%--                <div class="NewFolderTreePanel">--%>
                <uc1:FolderTreeControl ID="folderTreeControl" runat="server" />
<%--               </div>--%>
            </asp:Panel>
            <cc1:CollapsiblePanelExtender ID="fileExplorerCollapsiblePanelExtender" runat="server" 
                    TargetControlID="selectFolderPanel" 
                    Collapsed="true" 
                    ExpandDirection="Horizontal" 
                    ExpandedImage="../Images/CloseExplorerPanel.PNG" 
                    CollapsedImage="../Images/ExpandExplorer.PNG" 
                    Enabled="true" 
                    CollapsedText="Show file system explorer" 
                    ScrollContents="false" 
                    CollapseControlID="panelToggleImage" 
                    ExpandControlID="panelToggleImage" 
                    SuppressPostBack="true" 
                    ImageControlID="panelToggleImage" ExpandedText="Close file system explorer panel">
            </cc1:CollapsiblePanelExtender>
        </td>
        <td nowrap="nowrap" valign="top" align="center">
            <asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="true">
            <ContentTemplate>
            <table width="100%">
                <tr>
                    <td align="center">
                    <asp:CheckBox ID="includeSubfoldersCheckBox" runat="server" AutoPostBack="True" Text="Include Subfodlers" CssClass="ControlCheckbox" ToolTip="Check to include contents of all subfolders of the selected folder in the playlist" /><asp:CheckBox ID="shuffleCheckBox" runat="server" AutoPostBack="True" Text="Shuffle" CssClass="ControlCheckbox" ToolTip="Check to have songs in your playlist appear in random order" /><asp:CheckBox ID="repeateCheckBox" runat="server" AutoPostBack="True" Text="Repeat Playlist" Checked="True" CssClass="ControlCheckbox" ToolTip="Check to start playing the playlist after it was finished" /></td>
                    <td align="right">
                        SKIN:<asp:DropDownList ID="skinDropDownList" runat="server" 
                            AutoPostBack="True">
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
        <asp:Literal ID="flashWrap" runat="server" EnableViewState="false">
<object id="flashPlayer"
    codebase="https://fpdownload.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=10,0,0,0"
    {3} 
    align="middle" classid="clsid:d27cdb6e-ae6d-11cf-96b8-444553540000">
    &nbsp;&nbsp;&nbsp; <param name="allowScriptAccess" value="sameDomain" />
    &nbsp;&nbsp;&nbsp; <param name="movie" value="Jukebox/xspf_jukebox.swf?playlist_url={0}{2}{4}&xn_auth=no&autoload=true&autoplay=true&repeat_playlist={1}&crossFade=false&player_title=UltiDev%20Music%20Player&info_button_text=Click%20To%20Find%20Out%20More..." />
    
&nbsp;&nbsp;&nbsp; 
&nbsp;&nbsp;&nbsp; <%--    <param name="movie" value="Player/xspf_player.swf?playlist_url=/DebugLists/Bad.xspf&xn_auth=no&autoload=true" />--%>
    &nbsp;&nbsp;&nbsp; <%--<param name="movie" value="http://musicplayer.sourceforge.net/xspf_player.swf?playlist_url=http://cchits.ning.com/recent/xspf/?xn_auth=no&autoload=true" />--%>
    &nbsp;&nbsp;&nbsp; <param name="quality" value="high" />
    &nbsp;&nbsp;&nbsp; <param name="bgcolor" value="#e6e6e6" />
    &nbsp;&nbsp;&nbsp; <embed src="Jukebox/xspf_jukebox.swf?playlist_url={0}{2}{4}&xn_auth=no&autoload=true&autoplay=true&repeat_playlist={1}&crossFade=false&player_title=UltiDev%20Music%20Player&info_button_text=Click%20To%20Find%20Out%20More..."
        quality="high" bgcolor="#e6e6e6" 
        {3} 
        name="xspf_player"
        align="middle" allowscriptaccess="sameDomain" type="application/x-shockwave-flash"
        pluginspage="http://www.macromedia.com/go/getflashplayer" />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </object>
        </asp:Literal>
        </td>
            </ContentTemplate>
            </asp:UpdatePanel>
    </tr>
    <tr>
        <td align="center" colspan="2" nowrap="nowrap" valign="top">
            UltiDev MP3 Web Player Sample powered by <a href="http://ultidev.com/products/cassini/" target="_blank"> Cassini Web Server for ASP.NET 2.0 and 3.x</a><br />
            <a href="http://blog.lacymorrow.com/projects/xspf-jukebox/" target="_blank">XSPF Jukebox</a> by
            Lacy Morrow, <a href="http://musicplayer.sourceforge.net" target="_blank">XSPF Music Player</a>
            by Fabricio Zuardi<br/>
            &nbsp;<a href="http://www.UltraID3Lib.com" target="_blank">UltraID3Lib</a>, an MP3 ID3 Tag Editor and MPEG Info Reader Library
   Copyright 2002 - 2006 <a href="http://www.HundredMilesSoftware.com" target="_blank">Hundred Miles Software</a> (Mitchell S. Honnert)&nbsp;
        </td>
    </tr>
</table>
    </td>
</tr>
</table>
