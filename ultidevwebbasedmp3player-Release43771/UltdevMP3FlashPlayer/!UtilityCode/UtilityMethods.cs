using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Collections.Generic;
using System.Security.Principal;

/// <summary>
/// Summary description for UtilityMethods
/// </summary>
public sealed class UtilityMethods
{
    public const string cookieName = "UltiDevMp3PlayerControlSettings";
    
    private UtilityMethods() { }

    public static void ShuffleArray(Array array)
    {
        Random randomizer = new Random();
        int lastIndex = array.Length - 1;

        for (int i = 0; i < lastIndex ; i++)
        {
            int pullOutIndex = randomizer.Next(i, lastIndex);
            if(pullOutIndex == i)
                continue;

            object pluckedOut = array.GetValue(pullOutIndex);
            array.SetValue(array.GetValue(i), pullOutIndex);
            array.SetValue(pluckedOut, i);
        }
    }

    /// <summary>
    /// Returns 304 (use cached version) code to the client if
    /// file was not changed since last request.
    /// </summary>
    /// <param name="filePath">Path to the file</param>
    /// <returns>True if 304 was returned.</returns>
    public static bool Returned304(string filePath)
    {
        if (filePath == null || filePath.Length == 0)
            return false;

        return Returned304(new FileInfo(filePath));
    }

    /// <summary>
    /// Returns 304 (use cached version) code to the client if
    /// file was not changed since last request.
    /// </summary>
    /// <param name="fileInfo">File location information</param>
    /// <returns>True if 304 was returned.</returns>
    public static bool Returned304(FileInfo fileInfo)
    {
        if (!fileInfo.Exists)
            return false;

        return Returned304(fileInfo.LastWriteTime);
    }

    /// <summary>
    /// Returns 304 (use cached version) code to the client if
    /// file was not changed since last request.
    /// </summary>
    /// <param name="fileInfo">File location information</param>
    /// <returns>True if 304 was returned.</returns>
    public static bool Returned304(DirectoryInfo dirInfo)
    {
        if (!dirInfo.Exists)
            return false;

        return Returned304(dirInfo.LastWriteTime);
    }

    /// <summary>
    /// Returns 304 (use cached version) code to the client if
    /// file was not changed since last request.
    /// </summary>
    /// <param name="DateTime">File or folder last modification time</param>
    /// <returns>True if 304 was returned.</returns>
    public static bool Returned304(DateTime fileLastWriteTime)
    {
        HttpRequest request = HttpContext.Current.Request;
        HttpResponse response = HttpContext.Current.Response;

        string modifiedSinceHeader = request.Headers["If-Modified-Since"];
        if (modifiedSinceHeader == null || modifiedSinceHeader.Length == 0)
            return false;
        
        DateTime modifiedDateRequest;
        if (!DateTime.TryParse(modifiedSinceHeader, out modifiedDateRequest))
            return false;

        // If-Modified-Since value is not of up to the millisecond resolution.
        DateTime fileWriteTimeNoMilliseconds = new DateTime(
            fileLastWriteTime.Year, fileLastWriteTime.Month, fileLastWriteTime.Day,
            fileLastWriteTime.Hour, fileLastWriteTime.Minute, fileLastWriteTime.Second);

        if (modifiedDateRequest != fileWriteTimeNoMilliseconds)
            return false;

        response.StatusCode = 304;
        return true;
    }

    public static string GetFullAppUrlPrefix()
    {
        //return string.Empty;

        HttpRequest req = HttpContext.Current.Request;

        string appName = req.ApplicationPath;
        if (appName == "/")
            appName = string.Empty;

        return appName;
    }

    public static string CalcAndStoreFilePathHash(string path)
    {
        string hash = path.GetHashCode().ToString();

        StringDictionary map = UtilityMethods.GetHashToPathMap();
        map[hash] = path;

        return hash;
    }

    public static StringDictionary GetHashToPathMap()
    {
        StringDictionary map = HttpContext.Current.Session["HashToPathMap"] as StringDictionary;
        if (map == null)
        {
            map = new StringDictionary();
            HttpContext.Current.Session["HashToPathMap"] = map;
        }

        return map;
    }

    private static void ParseBool(string val, ref bool output)
    {
        if (val == null || val.Length == 0)
            return;

        bool oldVal = output;
        if (!bool.TryParse(val, out output))
            output = oldVal;
    }

    public static string GetInitialSettingsFromCookies(HttpRequest request,
        ref bool includeSubfolder, ref bool shuffle, ref bool repeat)
    {
        HttpCookie settingsCookies = request.Cookies[UtilityMethods.cookieName];

        if (settingsCookies == null)
            return null;

        ParseBool(settingsCookies["IncludeSubfolders"], ref includeSubfolder);
        ParseBool(settingsCookies["Shuffle"], ref shuffle);
        ParseBool(settingsCookies["Repeat"], ref repeat);

        return settingsCookies["Selected Folder"];
    }

    private const string fileSearchMask = "*.mp3";

    public static void FindMediaFiles(List<FileInfo> files, DirectoryInfo dir, bool recursive)
    {
        if (recursive && !HttpContext.Current.Response.IsClientConnected)
            // No point to continue since client got bored
            throw new Exception("Client disconnected.");

        try
        {
            files.AddRange(dir.GetFiles(fileSearchMask));
        }
        catch { }

        if (recursive)
            try
            {
                foreach (DirectoryInfo subDir in dir.GetDirectories())
                    FindMediaFiles(files, subDir, recursive);
            }
            catch { }
    }

    public static string GetPathByHash(string hash)
    {
        StringDictionary map = GetHashToPathMap();

        if (map.Count == 0)
        {   // Maybe session has expired? 
            // Let's try to regenerate the list of media files stored in session.

            bool withSubfolders = false, shuffle = false, repeat = false;
            string folder = GetInitialSettingsFromCookies(HttpContext.Current.Request,
                    ref withSubfolders, ref shuffle, ref repeat);

            if (!string.IsNullOrEmpty(folder))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(folder);
                if (dirInfo.Exists)
                {
                    CalcAndStoreFilePathHash(folder);

                    List<FileInfo> mediaFiles = new List<FileInfo>();
                    FindMediaFiles(mediaFiles, dirInfo, withSubfolders);

                    foreach (FileInfo fileInfo in mediaFiles)
                        CalcAndStoreFilePathHash(fileInfo.FullName);

                    map = GetHashToPathMap();
                }
            }
        }

        return map[hash];
    }

    public static IPrincipal GetCurrentFormsUser()
    {
        IPrincipal user = HttpContext.Current.User;
        if (user == null || user.Identity == null || !user.Identity.IsAuthenticated)
            return null;

        FormsIdentity fid = user.Identity as FormsIdentity;
        if (fid == null)
            return null;

        return user;
    }

    public static bool IsFormsAuthenticated()
    {
        return GetCurrentFormsUser() != null;
    }

    /// <summary>
    /// Returns True if browser runs on the same machine as the server.
    /// </summary>
    /// <returns></returns>
    public static bool IsLocalMachine()
    {
        HttpRequest request = HttpContext.Current.Request;
        return request.ServerVariables["LOCAL_ADDR"] == request.ServerVariables["REMOTE_ADDR"];
    }

    public static bool IsAuthorizedToDefineUsersAccessRights()
    {
        return
            IsFormsAuthenticated()
            || HttpVpnHeader.Current.IsHttpVpnAppOwner
            || (IsLocalMachine() && !HttpVpnHeader.Current.IsHttpVpn)
            ;
    }

    private static string GuestRootFilePath
    {
        get
        {
            string guestRootFilePath = HttpContext.Current.Request.MapPath("~/GuestRoot.config");
            return guestRootFilePath;
        }
    }

    public const string GuestMusic = "[Guest Music]";

    public static DirectoryInfo GuestMusicRoot
    {
        get
        {
            if (!File.Exists(GuestRootFilePath))
                return null;

            using (StreamReader reader = new StreamReader(GuestRootFilePath))
            {
                string guestRootFolder = reader.ReadLine();
                if (string.IsNullOrEmpty(guestRootFolder) || !Directory.Exists(guestRootFolder))
                    return null;

                return new DirectoryInfo(guestRootFolder);
            }
        }
        set
        {
            if (value == null)
            {
                File.Delete(GuestRootFilePath);
                return;
            }

            if (!Directory.Exists(value.FullName))
                throw new Exception(string.Format("Folder \"{0}\" does not exist", value.FullName));

            using (StreamWriter writer = new StreamWriter(GuestRootFilePath, false))
            {
                writer.WriteLine(value.FullName);
            }
        }
    }

    public static bool GuestMusicRootAssigend
    {
        get
        {
            return GuestMusicRoot != null;
        }
    }
}
