using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;

/// <summary>
/// Summary description for UtilityMethods
/// </summary>
public sealed class UtilityMethods
{
	private UtilityMethods(){}

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
}
