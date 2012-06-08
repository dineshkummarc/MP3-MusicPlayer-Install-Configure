<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PlayerControl.ascx.cs"
    Inherits="Controls_PlayerControl" %>
<link href="/Controls/PlayerControlDefaultStyle.css" rel="stylesheet" type="text/css"/>
<%--<object classid="d27cdb6e-ae6d-11cf-96b8-444553540000" codebase="http://fpdownload.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=7,0,0,0"
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
<table border="0" cellspacing="1" class="UltiDevPlayerControl" cellpadding="0">
    <tr>
        <td align="center" colspan="2" nowrap="nowrap">
            <asp:CheckBox ID="includeSubfoldersCheckBox" runat="server" AutoPostBack="True" Text="Include Subfodlers" CssClass="ControlCheckbox" ToolTip="Check to include contents of all subfolders of the selected folder in the playlist" />&nbsp;
            <asp:CheckBox ID="shuffleCheckBox" runat="server" AutoPostBack="True" Text="Shuffle" CssClass="ControlCheckbox" ToolTip="Check to have songs in your playlist appear in random order" />&nbsp;
            &nbsp;<asp:CheckBox ID="repeateCheckBox" runat="server" AutoPostBack="True" Text="Repeat" Checked="True" CssClass="ControlCheckbox" ToolTip="Check to start playing the playlist after it was finished" /></td>
    </tr>
    <tr>
        <td nowrap="nowrap" valign="top" align="left">
            <asp:Panel ID="selectFolderPanel" runat="server" Visible="False">
                <div class="NewFolderButtonPanel">
                    <asp:Button ID="cancelButton" runat="server" Text=" x " OnClick="cancelButton_Click" ToolTip="Click to cancel new folder selection" />
                </div>
                <div class="NewFolderTreePanel">
                    <asp:TreeView ID="folderTreeView" runat="server" ExpandDepth="0"
                    ImageSet="XPFileExplorer" NodeIndent="16" OnSelectedNodeChanged="folderTreeView_SelectedNodeChanged"
                    OnTreeNodePopulate="folderTreeView_TreeNodePopulate" PathSeparator="\">
                    <ParentNodeStyle Font-Bold="False" />
                    <HoverNodeStyle Font-Underline="True" ForeColor="#6666AA" />
                    <SelectedNodeStyle Font-Underline="False" HorizontalPadding="2px"
                        VerticalPadding="0px" ChildNodesPadding="0px" CssClass="SelectedTreeNode" />
                    <NodeStyle HorizontalPadding="2px"
                        NodeSpacing="0px" CssClass="TreeNodeClass" />
                    </asp:TreeView>
               </div>
                </asp:Panel>
        </td>
        <td nowrap="nowrap" valign="top" align="left">
            <table border="0" cellpadding="0" style="width: 100%">
                <tr>
                    <td nowrap="nowrap" align="left">
            <asp:LinkButton ID="sleectAnotherFolderLinkButton" runat="server" OnClick="selectAnotherFolderButton_Click" ToolTip="Click to select another folder with MP3 files">Selected Music Folder:</asp:LinkButton>&nbsp;
            <asp:Label ID="currentFolderLabel" runat="server" CssClass="CurrentFolderName"></asp:Label>
                    </td>
                </tr>
            </table>
        <asp:Literal ID="flashWrap" runat="server" EnableViewState="false">
<object id="flashPlayer"
    codebase="http://fpdownload.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=7,0,0,0"
    width="700" height="500" align="middle" classid="clsid:d27cdb6e-ae6d-11cf-96b8-444553540000">
    <param name="allowScriptAccess" value="sameDomain" />
    <param name="movie" value="/Player/xspf_player.swf?playlist_url={0}&xn_auth=no&autoload=true&autoplay=true&repeat_playlist={1}&player_title=UltiDev%20Music%20Player&info_button_text=Click%20To%20Find%20Out%20More..." />
<%--    <param name="movie" value="/Player/xspf_player.swf?playlist_url=/DebugLists/Bad.xspf&xn_auth=no&autoload=true" />--%>
    <%--<param name="movie" value="http://musicplayer.sourceforge.net/xspf_player.swf?playlist_url=http://cchits.ning.com/recent/xspf/?xn_auth=no&autoload=true" />--%>
    <param name="quality" value="high" />
    <param name="bgcolor" value="#e6e6e6" />
    <embed src="/Player/xspf_player.swf?playlist_url={0}&xn_auth=no&autoload=true&autoplay=true&repeat_playlist={1}&player_title=UltiDev%20Music%20Player&info_button_text=Click%20To%20Find%20Out%20More..."
        quality="high" bgcolor="#e6e6e6" width="700" height="500" name="xspf_player"
        align="middle" allowscriptaccess="sameDomain" type="application/x-shockwave-flash"
        pluginspage="http://www.macromedia.com/go/getflashplayer" />
</object>
        </asp:Literal>
        </td>
    </tr>
    <tr>
        <td align="center" colspan="2" nowrap="nowrap" valign="top">
            UltiDev MP3 Web Player Sample powered by<br/>
            <a href="http://ultidev.com/products/cassini/" target="_blank">UltiDev Cassini Web Server for ASP.NET 2.0</a><br />
            <a href="http://musicplayer.sourceforge.net" target="_blank">XSPF Music Player</a>, Copyright (c) 2005, Fabricio Zuardi<br/>
            &nbsp;<a href="http://www.UltraID3Lib.com" target="_blank">UltraID3Lib</a>, an MP3 ID3 Tag Editor and MPEG Info Reader Library
   Copyright 2002 - 2006 <a href="http://www.HundredMilesSoftware.com" target="_blank">Hundred Miles Software</a> (Mitchell S. Honnert)&nbsp;
        </td>
    </tr>
</table>
