using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace UltdevMP3FlashPlayer.Controls
{
    public partial class FolderTreeControl : System.Web.UI.UserControl
    {
        public delegate void SelectedFolderChangedDelegate();
        public event SelectedFolderChangedDelegate OnSelectedFolderChanged = null;

        private DirectoryInfo selectedFolder = null;

        public DirectoryInfo SelectedFolder
        {
            get
            {
                if (this.selectedFolder == null)
                {
                    string selectedPath = GetSelectedPath(this.folderTreeView);
                    if (selectedPath != null) // Null = selected "Undefined" guest path
                        this.selectedFolder = new DirectoryInfo(selectedPath);
                }

                return this.selectedFolder;
            }
            set
            {
                this.selectedFolder = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this.Refresh();
            }
        }

        public void Refresh()
        {
            this.PopulateFolderTreeRoot(this.folderTreeView.Nodes);
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

        private static string GetSelectedPath(TreeView treeView)
        {
            return GetSelectedPath(treeView.SelectedNode);
        }

        private static string GetSelectedPath(TreeNode node)
        {
            string selectedFolderPath = GetNodePath(node);
            return selectedFolderPath;
        }

        private void PopulateDrives(TreeNodeCollection treeRoot)
        {
            // Populate tree root with drives
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                string driveName = drive.Name;

                if (drive.DriveType != DriveType.Network && drive.DriveType != DriveType.Unknown && drive.DriveType != DriveType.NoRootDirectory)
                {
                    TreeNode node = new TreeNode();

                    node.Text = driveName.Trim('\\');
                    node.Value = node.Text;
                    node.ImageUrl = string.Format("{0}/Images/{1}",
                        this.Request.ApplicationPath.TrimEnd('/'),
                        drive.DriveType == DriveType.CDRom ? "CDRomIcon.PNG" : "DriveIcon.PNG"
                        );
                    node.PopulateOnDemand = true;

                    treeRoot.Add(node);
                }
            }
        }

        private void PopulateGuestRoot(TreeNodeCollection treeRoot)
        {
            TreeNode node = new TreeNode();

            if (UtilityMethods.GuestMusicRoot == null)
            {
                node.Text = "Owner has not specified guest music location.";
                node.Value = "Undefined";
                treeRoot.Add(node);
            }
            else
            {
                node.Text = UtilityMethods.GuestMusic.Trim('[', ']');
                node.Value = UtilityMethods.GuestMusicRoot.FullName;
                node.ImageUrl = string.Format("{0}/Images/MusicFolderIcon.PNG",
                    this.Request.ApplicationPath.TrimEnd('/'));

                treeRoot.Add(node);

                this.PopulateSubfodlers(node);
                node.Expanded = true;
            }
        }

        private void PopulateFolderTreeRoot(TreeNodeCollection treeRoot)
        {
            treeRoot.Clear();

            if (this.GuestTree)
                this.PopulateGuestRoot(treeRoot);
            else
                this.PopulateDrives(treeRoot);

            // Populate tree with selected folder subtree
            if (this.SelectedFolder != null)
            {
                List<DirectoryInfo> dirChain = new List<DirectoryInfo>();
                for (DirectoryInfo dir = this.SelectedFolder; dir != null; dir = dir.Parent)
                    dirChain.Insert(0, dir);

                TreeNodeCollection nodes = this.folderTreeView.Nodes;
                for (int i = 0; i < dirChain.Count; i++)
                {
                    DirectoryInfo dir = dirChain[i];
                    string dirName = dir.FullName.ToLower();

                    // find node matching the dir node
                    foreach (TreeNode folderNode in nodes)
                    {
                        string nodeFolderName = folderNode.ValuePath.ToLower();
                        if (nodeFolderName.EndsWith(":"))
                            nodeFolderName += @"\";

                        if (dirName == nodeFolderName)
                        {
                            if (i == dirChain.Count - 1)
                            {
                                folderNode.Selected = true;
                                if (folderNode.ChildNodes.Count == 0)
                                    folderNode.PopulateOnDemand = true;
                            }
                            else
                            {
                                folderNode.PopulateOnDemand = false;
                                if (folderNode.ChildNodes.Count == 0)
                                    this.PopulateSubfodlers(folderNode);
                                folderNode.Expanded = true;
                            }

                            nodes = folderNode.ChildNodes;

                            break;
                        }
                    }
                }
            }
        }

        private static string GetNodePath(TreeNode node)
        {
            if (node == null)
                return null;

            string path = node.ValuePath;
            if (path == "Undefined")
                return null;

            if (path.EndsWith(":"))
                path += @"\";

            return path;
        }

        protected void folderTreeView_SelectedNodeChanged(object sender, EventArgs e)
        {
            if (this.OnSelectedFolderChanged != null)
                this.OnSelectedFolderChanged();
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
                subNode.Value = di.Name;
                if (subNode.Text.ToLower().Contains("music"))
                    subNode.ImageUrl = string.Format("{0}/Images/MusicFolderIcon.PNG",
                        this.Request.ApplicationPath.TrimEnd('/'));
                subNode.PopulateOnDemand = true;

                node.ChildNodes.Add(subNode);
            }
        }

        protected void folderTreeView_TreeNodePopulate(object sender, TreeNodeEventArgs e)
        {
            if (e.Node.ChildNodes.Count == 0)
                PopulateSubfodlers(e.Node);
        }

        private bool guestTree = false;

        public bool GuestTree
        {
            get
            {
                return this.guestTree;
            }
            set
            {
                this.guestTree = value;
            }
        }

        public string GuestPath
        {
            get
            {
                if (!this.GuestTree)
                    throw new Exception("Invalid operation for non-guest tree.");

                if (UtilityMethods.GuestMusicRoot == null)
                    return null;

                string guestPath = UtilityMethods.GuestMusicRoot.FullName;
                string selectedPath = GetSelectedPath(this.folderTreeView);
                if (selectedPath == null || selectedPath == "Undefined")
                    return null;

                string prefix = UtilityMethods.GuestMusic;

                if (guestPath.Length > selectedPath.Length)
                    return prefix;

                string safePart = selectedPath.Substring(guestPath.Length);
                return prefix + safePart;
            }
        }

        public string ClientSelectedPath
        {
            get
            {
                if (this.GuestTree)
                {
                    string path = this.GuestPath;
                    if (path == null)
                        return string.Empty;
                    return path;
                }

                if (this.SelectedFolder == null)
                    return string.Empty;

                return this.SelectedFolder.FullName;
            }
        }
    }
}