using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace UltdevMP3FlashPlayer
{
    public partial class _Default : System.Web.UI.Page
    {
        private bool IsUnderHttpVpn()
        {
            return !string.IsNullOrEmpty(this.Request.Headers["HttpVPNData"]);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            if (!this.IsPostBack)
            {
                if (!IsUnderHttpVpn())
                {
                    this.cassiniServerHyperLink.NavigateUrl =
                        string.Format("{0}{1}:7756/",
                        this.Request.Url.GetLeftPart(UriPartial.Scheme),
                        this.Request.Url.Host);
                }
            }
        }
    }
}
