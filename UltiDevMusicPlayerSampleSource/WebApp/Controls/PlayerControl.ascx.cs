using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Reflection;
using System.IO;

public partial class Controls_PlayerControl : System.Web.UI.UserControl
{
    private const string cookieName = "UltiDevMp3PlayerControlSettings";

    private string selectedFolder = null;

    private string GetPlaylistUrl(string folderPath)
    {
        byte[] pathBytes = System.Text.ASCIIEncoding.Unicode.GetBytes(folderPath);

        string recursive = this.includeSubfoldersCheckBox.Checked ?
            "Recursive." : string.Empty;

        string shuffle = this.shuffleCheckBox.Checked ? "Shuffle" : string.Empty;

        //string url = string.Format("{0}/{1}/{2}{3}Folder.dynplist",
        //            this.Request.ApplicationPath.TrimEnd('/'),
        //            System.Convert.ToBase64String(pathBytes),
        //            recursive, shuffle);
        string url = string.Format("{0}/{1}{2}Folder.dynplist",
                    //this.Request.ApplicationPath.TrimEnd('/'),
                    System.Convert.ToBase64String(pathBytes),
                    recursive, shuffle);

        return url;
    }

    private static string GetMyMusicFolderPath()
    {
        return Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
    }

    private static string GetDefaultMusicFolderPath()
    {
        string folder = GetMyMusicFolderPath();

        if (folder == null || folder.Length == 0)
            folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"..");

        return new DirectoryInfo(folder).FullName;
    }

    private static void ParseBool(string val, ref bool output)
    {
        if(val == null || val.Length == 0)
            return;

        bool oldVal = output;
        if (!bool.TryParse(val, out output))
            output = oldVal;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.IsPostBack)
        {
            HttpCookie settingsCookies = this.Request.Cookies[cookieName];

            if (settingsCookies != null)
            {
                bool tempBool;

                tempBool = this.includeSubfoldersCheckBox.Checked;
                ParseBool(settingsCookies["IncludeSubfolders"], ref tempBool);
                this.includeSubfoldersCheckBox.Checked = tempBool;

                tempBool = this.shuffleCheckBox.Checked;
                ParseBool(settingsCookies["Shuffle"], ref tempBool);
                this.shuffleCheckBox.Checked = tempBool;

                tempBool = this.repeateCheckBox.Checked;
                ParseBool(settingsCookies["Repeat"], ref tempBool);
                this.repeateCheckBox.Checked = tempBool;

                this.selectedFolder = settingsCookies["Selected Folder"];
            }

            if (this.selectedFolder == null || this.selectedFolder.Length == 0 || !Directory.Exists(this.selectedFolder))
            {
                this.selectedFolder = GetDefaultMusicFolderPath();
                this.selectAnotherFolderButton_Click(null, null);
            }

            this.UpdateCurrentNodeInfo();
        }
    }

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (this.selectedFolder == null || this.selectedFolder.Length == 0)
            this.selectedFolder = this.GetCurrentFolderPath();

        this.flashWrap.Text = string.Format(this.flashWrap.Text,
            this.GetPlaylistUrl(this.selectedFolder),
            //playlistUrl, 
            this.repeateCheckBox.Checked.ToString().ToLower());

        HttpCookie settingsCookies = new HttpCookie(cookieName);
        settingsCookies.Expires = DateTime.Now.AddYears(10);

        settingsCookies["IncludeSubfolders"] = this.includeSubfoldersCheckBox.Checked.ToString();
        settingsCookies["Shuffle"] = this.shuffleCheckBox.Checked.ToString();
        settingsCookies["Repeat"] = this.repeateCheckBox.Checked.ToString();
        settingsCookies["Selected Folder"] = this.selectedFolder;

        this.Response.Cookies.Add(settingsCookies);
    }

    private static string GetSelectedPath(TreeView treeView)
    {
        return GetSelectedPath(treeView.SelectedNode);
    }

    private static string GetSelectedPath(TreeNode node)
    {
        string selectedFolderPath = null;
        if (node != null)
            selectedFolderPath = GetNodePath(node);

        if (selectedFolderPath == null || selectedFolderPath.Length == 0 || !Directory.Exists(selectedFolderPath))
            selectedFolderPath = GetDefaultMusicFolderPath();

        return selectedFolderPath;
    }

    private void PopulateFolderTreeRoot()
    {
        this.folderTreeView.Nodes.Clear();

        // Populate tree root with drives
        foreach (DriveInfo drive in DriveInfo.GetDrives())
        {
            string driveName = drive.Name;

            if (drive.DriveType != DriveType.Network && drive.DriveType != DriveType.Unknown && drive.DriveType != DriveType.NoRootDirectory)
            {
                TreeNode node = new TreeNode();

                node.Text = driveName.Trim('\\');
                //node.Value = driveName;
                node.ImageUrl = string.Format("{0}/Images/{1}",
                    this.Request.ApplicationPath.TrimEnd('/'),
                    drive.DriveType == DriveType.CDRom ? "CDRomIcon.PNG" : "DriveIcon.PNG"
                    );
                node.PopulateOnDemand = true;

                this.folderTreeView.Nodes.Add(node);
            }
        }

        // Populate tree with selected folder subtree
        if (this.selectedFolder == null || this.selectedFolder.Length == 0)
            this.selectedFolder = this.GetCurrentFolderPath();

        DirectoryInfo selectedDirInfo = new DirectoryInfo(this.selectedFolder);
        List<DirectoryInfo> dirChain = new List<DirectoryInfo>();
        for (DirectoryInfo dir = selectedDirInfo; dir != null; dir = dir.Parent)
            dirChain.Insert(0, dir);

        TreeNodeCollection nodes = this.folderTreeView.Nodes;
        for (int i = 0; i < dirChain.Count; i++ )
        {
            DirectoryInfo dir = dirChain[i];
            string dirName = dir.Name.ToLower();

            // find node matching the dir node
            foreach (TreeNode folderNode in nodes)
            {
                string nodeFolderName = folderNode.Text.ToLower();
                if (nodeFolderName.EndsWith(":"))
                    nodeFolderName += @"\";

                if (dirName == nodeFolderName)
                {
                    if (i == dirChain.Count - 1)
                    {
                        folderNode.Selected = true;
                        folderNode.PopulateOnDemand = true;
                    }
                    else
                    {
                        folderNode.PopulateOnDemand = false;
                        this.PopulateSubfodlers(folderNode);
                        folderNode.Expanded = true;
                    }
                    
                    nodes = folderNode.ChildNodes;

                    break;
                }
            }
        }
    }

    private static string GetNodePath(TreeNode node)
    {
        string path = node.ValuePath;
        if (path.EndsWith(":"))
            path += @"\";

        return path;
    }

    private void UpdateCurrentNodeInfo()
    {
        DirectoryInfo di = new DirectoryInfo(this.selectedFolder);
        this.currentFolderLabel.Text = di.Name;
        this.currentFolderLabel.ToolTip = di.FullName;
    }

    protected void folderTreeView_SelectedNodeChanged(object sender, EventArgs e)
    {
        this.selectedFolder = GetSelectedPath(this.folderTreeView);
        this.UpdateCurrentNodeInfo();
        this.cancelButton_Click(null, null);
    }

    private void PopulateSubfodlers(TreeNode node)
    {
        DirectoryInfo dirInfo = new DirectoryInfo(GetNodePath(node));
        if (!dirInfo.Exists)
        {   // Selected folder was removed
            this.folderTreeView.Nodes.Remove(node);
            return;
        }

        foreach (DirectoryInfo di in dirInfo.GetDirectories())
        {
            if ((di.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                continue;

            TreeNode subNode = new TreeNode();

            subNode.Text = di.Name;
            if (subNode.Text.ToLower().Contains("music"))
                subNode.ImageUrl = string.Format("{0}/Images/MusicFolderIcon.PNG",
                    this.Request.ApplicationPath.TrimEnd('/'));
            subNode.PopulateOnDemand = true;

            node.ChildNodes.Add(subNode);
        }
    }

    protected void folderTreeView_TreeNodePopulate(object sender, TreeNodeEventArgs e)
    {
        PopulateSubfodlers(e.Node);
    }

    private string GetCurrentFolderPath()
    {
        if (this.currentFolderLabel.ToolTip == null || this.currentFolderLabel.ToolTip.Length == 0)
            return GetDefaultMusicFolderPath();

        return this.currentFolderLabel.ToolTip;
    }

    protected void selectAnotherFolderButton_Click(object sender, EventArgs e)
    {
        this.selectedFolder = this.GetCurrentFolderPath();
        this.PopulateFolderTreeRoot();

        this.selectFolderPanel.Visible = true;
    }
    
    protected void cancelButton_Click(object sender, EventArgs e)
    {
        this.selectFolderPanel.Visible = false;
    }
}
