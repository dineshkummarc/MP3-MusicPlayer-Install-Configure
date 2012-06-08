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

    private HttpVpnHeader httpVpnHelper;

    private string GetPlaylistUrl(string folderPath)
    {
        if (string.IsNullOrEmpty(folderPath))
            return string.Empty;

        string recursive = this.includeSubfoldersCheckBox.Checked ?
            "Recursive." : string.Empty;

        //string shuffle = this.shuffleCheckBox.Checked ? "Shuffle" : string.Empty;
        string shuffle = string.Empty; // Shuffling now implemented by directing player to shuffle the list on its own.

        string appName = this.Request.ApplicationPath;
        if (appName == "/")
            appName = string.Empty;

        string url = string.Format("./{1}/{2}{3}Folder.dynplist",
                    UtilityMethods.GetFullAppUrlPrefix(),
                    UtilityMethods.CalcAndStoreFilePathHash(folderPath),
                    recursive, shuffle);

        return url;
    }

    private static void ParseBool(string val, ref bool output)
    {
        if (val == null || val.Length == 0)
            return;

        bool oldVal = output;
        if (!bool.TryParse(val, out output))
            output = oldVal;
    }

    private bool ShouldShowAccessTab()
    {
        return
            UtilityMethods.IsAuthorizedToDefineUsersAccessRights()
                || !this.httpVpnHelper.IsHttpVpn;
    }

    private void InitSelectedFolder(string selectedFolder)
    {
        if (string.IsNullOrEmpty(selectedFolder))
        {
            DirectoryInfo guestMusicBase = UtilityMethods.GuestMusicRoot;
            if (guestMusicBase != null)
                selectedFolder = UtilityMethods.GuestMusic;
        }

        if (!string.IsNullOrEmpty(selectedFolder))
        {
            if (selectedFolder.StartsWith(UtilityMethods.GuestMusic))
            {
                DirectoryInfo guestMusicBase = UtilityMethods.GuestMusicRoot;
                if (guestMusicBase == null)
                    selectedFolder = null;
                else
                {
                    string guestSubFolder = selectedFolder.Substring(UtilityMethods.GuestMusic.Length).TrimStart('\\');
                    selectedFolder = Path.Combine(guestMusicBase.FullName, guestSubFolder);
                }
            }
            else if (this.folderTreeControl.GuestTree)
            {
                selectedFolder = null;

                DirectoryInfo guestMusicBase = UtilityMethods.GuestMusicRoot;
                if (guestMusicBase == null)
                    selectedFolder = null;
                else
                    selectedFolder = guestMusicBase.FullName;
            }
        }

        if (!string.IsNullOrEmpty(selectedFolder))
        {
            DirectoryInfo musicFolderInfo = new DirectoryInfo(selectedFolder);
            if (musicFolderInfo.Exists)
                this.folderTreeControl.SelectedFolder = musicFolderInfo;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        this.httpVpnHelper = new HttpVpnHeader(this.Request);

        this.accessHyperLink.Visible = this.ShouldShowAccessTab();

        this.folderTreeControl.GuestTree = !UtilityMethods.IsAuthorizedToDefineUsersAccessRights();
        this.folderTreeControl.OnSelectedFolderChanged += new Controls_FolderTreeControl.SelectedFolderChangedDelegate(folderTreeControl_OnSelectedFolderChanged);

        if (!this.IsPostBack)
        {
            HttpCookie settingsCookies = this.Request.Cookies[cookieName];

            string skin = string.Empty;
            string selectedFolder = null;

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

                skin = settingsCookies["Skin"];
                selectedFolder = settingsCookies["Selected Folder"];
            }

            this.InitSelectedFolder(selectedFolder);

            this.PopulateSkinList(skin);
        }
    }

    private void PopulateSkinList(string selectedSkinName)
    {
        this.skinDropDownList.Items.Clear();

        int selectedIndex = 0;
        this.skinDropDownList.Items.Add(new ListItem("Default", string.Empty));

        if (string.IsNullOrEmpty(selectedSkinName))
            selectedSkinName = "iTunes";

        string skinFolder = this.Request.MapPath("~/Jukebox/Skins");
        DirectoryInfo skinFolderInfo = new DirectoryInfo(skinFolder);

        int index = 1;
        foreach (DirectoryInfo skinDir in skinFolderInfo.GetDirectories())
        {
            ListItem item = new ListItem(skinDir.Name);
            this.skinDropDownList.Items.Add(item);
            if (selectedSkinName == skinDir.Name)
                selectedIndex = index;
            index++;
        }

        this.skinDropDownList.SelectedIndex = selectedIndex;
    }

    private string GetSkinSizeAttribs(string skinName)
    {
        int width = 700, height = 500;

        if(!string.IsNullOrEmpty(skinName))
        {
            string skinFolder = this.Request.MapPath(string.Format("~/Jukebox/Skins/{0}/images", skinName));
            DirectoryInfo di = new DirectoryInfo(skinFolder);
            if (di.Exists)
            {
                int square = 0;
                foreach (FileInfo imageFileInfo in di.GetFiles())
                {
                    string ext = imageFileInfo.Extension.Trim('.').ToLower();
                    if (ext == "jpg" || ext == "png" || ext == "gif" || ext == "bmp")
                    {
                        System.Drawing.Bitmap image = new System.Drawing.Bitmap(imageFileInfo.FullName);
                        int thisSquare = image.Width * image.Height;
                        if (thisSquare > square)
                        {
                            square = thisSquare;
                            width = image.Width;
                            height = image.Height;
                        }
                    }
                }
            }
        }

        return string.Format("width=\"{0}\" height=\"{1}\"", width, height);
    }

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        this.UpdateCurrentNodeInfo();

        string selectedFolder;
        if (this.folderTreeControl.SelectedFolder == null)
            selectedFolder = string.Empty;
        else
            selectedFolder = this.folderTreeControl.SelectedFolder.FullName;

        // &skin_url=./Jukebox/Skins/iTunes
        string skinUrl;
        string skin = this.skinDropDownList.SelectedItem.Value;
        if(string.IsNullOrEmpty(skin))
            skinUrl = string.Empty;
        else
            skinUrl = "&skin_url=./Jukebox/Skins/" + skin;

        string shuffle;
        if (this.shuffleCheckBox.Checked)
            shuffle = "&shuffle=true";
        else
            shuffle = string.Empty;

        this.flashWrap.Text = string.Format(this.flashWrap.Text,
            this.GetPlaylistUrl(selectedFolder),
            //playlistUrl, 
            this.repeateCheckBox.Checked.ToString().ToLower(),
            skinUrl, GetSkinSizeAttribs(skin), shuffle
            );

        HttpCookie settingsCookies = new HttpCookie(cookieName);
        settingsCookies.Expires = DateTime.Now.AddYears(10);

        settingsCookies["IncludeSubfolders"] = this.includeSubfoldersCheckBox.Checked.ToString();
        settingsCookies["Shuffle"] = this.shuffleCheckBox.Checked.ToString();
        settingsCookies["Repeat"] = this.repeateCheckBox.Checked.ToString();
        settingsCookies["Selected Folder"] = this.folderTreeControl.ClientSelectedPath;
        settingsCookies["Skin"] = skin;

        this.Response.Cookies.Add(settingsCookies);
    }

    private void UpdateCurrentNodeInfo()
    {
        if (this.folderTreeControl.SelectedFolder == null)
        {
            this.currentFolderLabel.Text = string.Empty;
            this.currentFolderLabel.ToolTip = string.Empty;
        }
        else
        {
            if (this.folderTreeControl.ClientSelectedPath == UtilityMethods.GuestMusic)
            {
                this.currentFolderLabel.Text = UtilityMethods.GuestMusic.Trim('[', ']');
                this.currentFolderLabel.ToolTip = this.currentFolderLabel.Text;
            }
            else
            {
                this.currentFolderLabel.Text = this.folderTreeControl.SelectedFolder.Name;
                this.currentFolderLabel.ToolTip = this.folderTreeControl.ClientSelectedPath;
            }
        }
    }

    void folderTreeControl_OnSelectedFolderChanged()
    {
        this.fileExplorerCollapsiblePanelExtender.Collapsed = true;
    }
    
    protected void skinDropDownList_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
}
