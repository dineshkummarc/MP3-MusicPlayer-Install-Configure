<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FolderTreeControl.ascx.cs" Inherits="Controls_FolderTreeControl" %>
<asp:TreeView ID="folderTreeView" runat="server" ExpandDepth="0" ImageSet="XPFileExplorer"
    NodeIndent="16" OnSelectedNodeChanged="folderTreeView_SelectedNodeChanged" OnTreeNodePopulate="folderTreeView_TreeNodePopulate"
    PathSeparator="\">
    <ParentNodeStyle Font-Bold="False" />
    <HoverNodeStyle Font-Underline="True" ForeColor="#6666AA" />
    <SelectedNodeStyle ChildNodesPadding="0px" CssClass="SelectedTreeNode" Font-Underline="False"
        HorizontalPadding="2px" VerticalPadding="0px" />
    <NodeStyle CssClass="TreeNodeClass" HorizontalPadding="2px" NodeSpacing="0px" />
</asp:TreeView>
