using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Configuration;
using System.Web.Security;

namespace UltdevMP3FlashPlayer
{
    public partial class Login : System.Web.UI.Page
    {
        private AuthenticationSection GetFormsAuthConfig()
        {
            Configuration dummy;
            return this.GetFormsAuthConfig(out dummy);
        }
        private AuthenticationSection GetFormsAuthConfig(out Configuration webConfig)
        {
            string root = this.Request.ApplicationPath;
            webConfig = WebConfigurationManager.OpenWebConfiguration(root);
            AuthenticationSection authenticationSection = (AuthenticationSection)webConfig.GetSection("system.web/authentication");

            return authenticationSection;
        }

        private bool IsDefaultFormsAuthPassword()
        {
            AuthenticationSection authenticationSection = this.GetFormsAuthConfig();

            if (authenticationSection.Forms.Credentials.PasswordFormat != FormsAuthPasswordFormat.Clear)
                return false;

            FormsAuthenticationUserCollection users = authenticationSection.Forms.Credentials.Users;
            if (users.Count != 1 || users[0].Name != "admin")
                return false;

            string password = users[0].Password;
            return password == "1234567";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                bool defaultPassword = this.IsDefaultFormsAuthPassword();
                if (defaultPassword)
                {
                    this.Login1.Visible = false;
                    this.changePassword.Focus();
                }
                else
                {
                    this.changePassword.Visible = false;

                    AuthenticationSection authenticationSection = this.GetFormsAuthConfig();
                    FormsAuthenticationUserCollection users = authenticationSection.Forms.Credentials.Users;
                    if (users.Count > 0)
                        this.Login1.UserName = users[0].Name;

                    this.Login1.Focus();
                }
            }
        }
        protected void Login1_Authenticate(object sender, AuthenticateEventArgs e)
        {
            //Debug.Write(string.Format("Password \"{0}\" hash is ", this.Login1.Password));
            //Debug.WriteLine(FormsAuthentication.HashPasswordForStoringInConfigFile(this.Login1.Password, "SHA1"));
            e.Authenticated = FormsAuthentication.Authenticate(this.Login1.UserName, this.Login1.Password);
        }
        protected void changePassword_ChangingPassword(object sender, LoginCancelEventArgs e)
        {
            e.Cancel = true;

            string newPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(this.changePassword.NewPassword, "SHA1");

            Configuration webConfig;
            AuthenticationSection authenticationSection = this.GetFormsAuthConfig(out webConfig);

            authenticationSection.Forms.Credentials.PasswordFormat = FormsAuthPasswordFormat.SHA1;
            authenticationSection.Forms.Credentials.Users[0].Password = newPassword;

            webConfig.Save();

            this.Response.Redirect(this.Request.Url.ToString());
        }
    }
}
