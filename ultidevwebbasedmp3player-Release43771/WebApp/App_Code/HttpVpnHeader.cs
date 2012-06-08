using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;

/// <summary>
/// HttpVpnHeader class parses out values in the
/// custom http header supplied by HttpVPN Portal.
/// </summary>
/// <remarks>
/// HttpVPN™ (http://httpvpn.com) is a proxy component
/// making browser applications accessible on the web
/// right out of the box.
/// 
/// When users access local web application on the web
/// via HttpVPN Portal, requests received by the applicaiton
/// have additional information from HttpVPN Portal, coded
/// as a custom HTTP header named 'HttpVPNData'.
/// 
/// HttpVPN header contains httpVPN user identity (user ID 
/// and user group/role membership), as well as application
/// ownership information, like whether applicaiton is used
/// on a trial basis or is paid for or free.
/// 
/// IMPORTANT: All public properties and methods return valid
/// value only if IsHttpVpn property returns True.
/// </remarks>
public class HttpVpnHeader
{
    // Each application has its own release ID. Don't use this ID for your app!!!
    static readonly Guid httpVpnAppReleaseID = new Guid("90ae5e0b-a143-45f4-b429-6a59866e7051");

    /// <summary>
    /// (For ordered/purchased software only.)
    /// Tells whether this application ordered via HttpVPN Portal
    /// was paid for, used as a trial version, or is an illegal copy.
    /// </summary>
    public enum AppOnwership
    {
        PaidForOrFree, TrialUnderway, TrialExpired, IllegalCopy
    }

    #region Fields
    /// <summary>
    /// Collection of parsed HttpVPN name-value pairs.
    /// </summary>
    private Dictionary<string, string> httpVpnHeaders = null;
    #endregion Fields

    #region Constructors
    /// <summary>
    /// Constructor. Parses data received as 'HttpVPNData' custom HTTP header;
    /// </summary>
    /// <param name="request"></param>
    public HttpVpnHeader(HttpRequest request)
    {
        string headerTextValue = request.Headers["HttpVPNData"];
        if (string.IsNullOrEmpty(headerTextValue))
            return;

        // Name=value pairs are semicolon-separeated.
        string[] pairs = headerTextValue.Split(';');

        if (pairs.Length == 0)
            return;

        this.httpVpnHeaders = new Dictionary<string, string>(pairs.Length);

        foreach (string pair in pairs)
        {
            string[] namevalue = pair.Split('=');
            this.httpVpnHeaders.Add(namevalue[0], namevalue[1]);
        }
    }
    #endregion Constructors

    /// <summary>
    /// Instantiates HttpVpnHeader instance based on current http requests.
    /// </summary>
    /// <remarks>
    /// Must be called only within HTTP requests processing context.
    /// </remarks>
    public static HttpVpnHeader Current
    {
        get
        {
            return new HttpVpnHeader(HttpContext.Current.Request);
        }
    }

    #region Properties
    /// <summary>
    /// Returns True if aplication is being accessed on the web
    /// via HttpVPN.
    /// </summary>
    /// <remarks>
    /// IMPORTANT: All public properties and methods return valid
    /// value only if this property returns True.
    /// </remarks>
    public bool IsHttpVpn
    {
        get
        {
            return this.httpVpnHeaders != null && this.httpVpnHeaders.Count > 0;
        }
    }

    /// <summary>
    /// HttpVPN user ID. 
    /// </summary>
    /// <remarks>
    /// If your application is public, user
    /// may or may not be authenticated. If he's not, all-zero
    /// GUID (Guid.Empty) will be returned. If user is authenticated,
    /// then non-empty ID will be provided even if applciation
    /// is public.
    /// </remarks>
    public Guid UserID
    {
        get
        {
            string userIdString = this.GetHttpVpnSubHeader("UserID");
            if (string.IsNullOrEmpty(userIdString))
                return Guid.Empty;

            return new Guid(userIdString);
        }
    }

    /// <summary>
    /// Returns True if client is an authenticated HttpVPN user and is
    /// one of the Managers/Owners of the local network where application
    /// is hosted.
    /// </summary>
    public bool IsHttpVpnAppOwner
    {
        get
        {
            return this.IsUserInHttpVpnRoles("Owner");
        }
    }

    /// <summary>
    /// Returs True if software title was legally ordered from HttpVPN.
    /// </summary>
    /// <remarks>
    /// This property tells whether a software title was obtained legally
    /// at HttpVPN. Since any web application can be made accessbile on the
    /// Web via HttpVPN manually, this property tells whether this 
    /// installation of the software is associated with an order placed
    /// via HttpVPN.
    /// 
    /// If this property returns false, you can drive user to the 
    /// HttpVPN Portal page wheter this software is sold.
    /// </remarks>
    public bool IsPurchaseRegistered
    {
        get
        {
            string releaseIdText = this.GetHttpVpnSubHeader("PurchasedSoftwareReleaseID");

            if (string.IsNullOrEmpty(releaseIdText))
                return false;

            try
            {
                Guid releaseID = new Guid(releaseIdText);
                return releaseID == httpVpnAppReleaseID;
            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>
    /// When the application is accessed via HttpVPN, this property
    /// tells whether the software title is a Trial, Free or Paid for,
    /// or is an illegal copy altogether.
    /// </summary>
    /// <remarks>
    /// When IllegalCopy is returned, it's equivalent of IsPurchaseRegistered
    /// proeprty returning false;
    /// When TrialUnderway is returned, GetTrialDaysLeft() method can be used
    /// to determine how many days of the trial period is remaining.
    /// When TrialExpired is returned, it means the software was legally ordered
    /// as a trial version, but trial period has already expired. You can reduce
    /// funcionality and/or drive user to the HttpVPN payment page.
    /// When PaidForOrFree is returned, it means tha the software title is 
    /// either paid for, or was free at the time of pruchase. Please note that
    /// even free redistributable applications need to be ordered and taken
    /// through the checkout process (with no payment).
    /// </remarks>
    public AppOnwership AppOwnershipType
    {
        get
        {
            if (!this.IsPurchaseRegistered)
                return AppOnwership.IllegalCopy;

            string purchaseOwnershipValue = this.GetHttpVpnSubHeader("PurchaseOwnership");
            if (string.IsNullOrEmpty(purchaseOwnershipValue))
                return AppOnwership.IllegalCopy;

            switch (purchaseOwnershipValue)
            {
                case "PaidForOrFree":
                    return AppOnwership.PaidForOrFree;
                case "Trial":
                    {
                        if (this.GetTrialDaysLeft() < 0)
                            return AppOnwership.TrialExpired;
                        else
                            return AppOnwership.TrialUnderway;
                    }
                default:
                    throw new Exception(string.Format("Purchase ownership type \"{0}\" is invalid.",
                                purchaseOwnershipValue));
            }
        }
    }
    #endregion Properties

    #region Methods
    private string GetHttpVpnSubHeader(string subHeaderName)
    {
        if (!this.IsHttpVpn || !this.httpVpnHeaders.ContainsKey(subHeaderName))
            return null;

        return this.httpVpnHeaders[subHeaderName];
    }

    /// <summary>
    /// Returns True if user is a member of the given HttpVPN user group.
    /// </summary>
    /// <remarks>
    /// Allows leveraging HttpVPN identity management.
    /// </remarks>
    public bool IsUserInHttpVpnRoles(params string[] roleNames)
    {
        string httpVpnUserRolesString = this.GetHttpVpnSubHeader("UserRoles");
        if (string.IsNullOrEmpty(httpVpnUserRolesString))
            return false;

        string[] httpVpnUserRoles = httpVpnUserRolesString.Split(',');
        foreach (string checkingRole in roleNames)
            if (checkingRole.Length > 0)
            {
                foreach (string userHttpVpnRole in httpVpnUserRoles)
                    if (userHttpVpnRole == checkingRole)
                        return true;
            }

        return false;
    }

    /// <summary>
    /// Returns number of days left in the trial period for
    /// a software title which has AppOwnershipType of TrialUnderway.
    /// </summary>
    /// <remarks>
    /// Call only when AppOwnershipType property has TrialUnderway value.
    /// </remarks>
    public int GetTrialDaysLeft()
    {
        string trialDaysLeftValue = this.GetHttpVpnSubHeader("PurchaseTrialDaysLeft");
        return int.Parse(trialDaysLeftValue);
    }

    /// <summary>
    /// Returns True if the software title is Beta.
    /// </summary>
    public bool IsPurchaseBeta()
    {
        if (!this.IsPurchaseRegistered)
            throw new Exception("Purchase information is not received by the application. This is likely to be an illegal copy.");

        string betaValue = this.GetHttpVpnSubHeader("PurchaseIsBeta");
        if (string.IsNullOrEmpty(betaValue))
            throw new Exception("PurchaseIsBeta sub-header of the HttpVpnData header is missing.");

        return bool.Parse(betaValue);
    }
    #endregion Methods
}
