using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

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
