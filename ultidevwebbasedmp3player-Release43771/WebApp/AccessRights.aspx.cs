using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class AccessRights : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!UtilityMethods.IsAuthorizedToDefineUsersAccessRights())
            FormsAuthentication.RedirectToLoginPage();

        this.guestFolderTreeControl.GuestTree = true;

        if (!this.IsPostBack)
        {
            this.folderTreeControl.SelectedFolder = UtilityMethods.GuestMusicRoot;

            bool guestUserInfoControlsVisible = this.folderTreeControl.SelectedFolder != null;
            this.UpdateGuestInfoControlsVisibility(guestUserInfoControlsVisible);
        }

        this.folderTreeControl.OnSelectedFolderChanged += new Controls_FolderTreeControl.SelectedFolderChangedDelegate(folderTreeControl_OnSelectedFolderChanged);
        this.guestFolderTreeControl.OnSelectedFolderChanged += new Controls_FolderTreeControl.SelectedFolderChangedDelegate(guestFolderTreeControl_OnSelectedFolderChanged);
    }

    void guestFolderTreeControl_OnSelectedFolderChanged()
    {
        //this.Label1.Text = string.Format("Folder path: {0}<br>Guest Path: {1}", 
        //    this.guestFolderTreeControl.SelectedFolder.FullName,
        //    this.guestFolderTreeControl.GuestPath);
    }

    private void UpdateGuestInfoControlsVisibility(bool visible)
    {
        this.guestUserFolderPanel.Visible = visible;

        if (visible)
            this.guestMusicFolderLabel.Text = this.folderTreeControl.SelectedFolder.FullName;
    }

    void folderTreeControl_OnSelectedFolderChanged()
    {
        UtilityMethods.GuestMusicRoot = this.folderTreeControl.SelectedFolder;
        
        this.UpdateGuestInfoControlsVisibility(true);
        this.guestFolderTreeControl.Refresh();
        //this.Label1.Text = string.Empty;
    }

    protected void clearGuestLocationButton_Click(object sender, EventArgs e)
    {
        UtilityMethods.GuestMusicRoot = null;

        this.UpdateGuestInfoControlsVisibility(false);

        this.folderTreeControl.Refresh();
        this.guestFolderTreeControl.Refresh();
        //this.Label1.Text = string.Empty;
    }
}
