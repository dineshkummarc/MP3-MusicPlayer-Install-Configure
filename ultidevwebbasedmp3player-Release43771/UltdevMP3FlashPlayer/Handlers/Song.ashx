<%@ WebHandler Language="C#" Class="Song" %>

using System;
using System.Web;
using System.IO;

public class Song : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{
    private HttpRequest Request = null;
    private HttpResponse Response = null;
    
    public void ProcessRequest (HttpContext context) 
    {
        this.Request = context.Request;
        this.Response = context.Response;

        string songFilePath = UtilityMethods.GetPathByHash(this.Request.QueryString[0]);
        if (songFilePath == null || songFilePath.Length == 0)
        {
            this.Response.StatusCode = 500;
            this.Response.ContentType = "text/html";
            this.Response.Write("Song file is not specified.");
            return;
        }

        FileInfo songFileInfo = new FileInfo(songFilePath);
        if (!songFileInfo.Exists)
        {
            this.Response.StatusCode = 404;
            this.Response.ContentType = "text/html";
            this.Response.Write(string.Format("Song file \"{0}\" was not found.", songFileInfo.FullName));
            return;
        }

        if (UtilityMethods.Returned304(songFileInfo))
            return;
        
        // Serve the file:
        // 1) Set headers
        this.Response.Buffer = false;
        this.Response.ContentType = "audio/mpeg3"; // audio/wav, audio/x-ms-wma
        this.Response.AddHeader("Content-Disposition",
                string.Format("attachment; filename={0}; size={1};", songFileInfo.Name, songFileInfo.Length));

        this.Response.Cache.SetCacheability(HttpCacheability.Public);
        this.Response.Cache.SetLastModified(songFileInfo.LastWriteTime);
        this.Response.Expires = 60 * 24 * 30; // 30 days expressed in minutes
        this.Response.Cache.SetSlidingExpiration(true);
                
        // 2) Send the bits.
        byte[] buffer = new byte[8 * 1024];
        using (FileStream songInStream = songFileInfo.OpenRead())
        {
            int read;
            while((read = songInStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                this.Response.OutputStream.Write(buffer, 0, read);
            }
        }
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }
}