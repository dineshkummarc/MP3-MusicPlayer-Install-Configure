<%@ WebHandler Language="C#" Class="CoverArt" %>

using System;
using System.Web;
using System.IO;

using HundredMilesSoftware.UltraID3Lib;

public class CoverArt : IHttpHandler, System.Web.SessionState.IRequiresSessionState 
{
    private HttpRequest Request = null;
    private HttpResponse Response = null;

    public void ProcessRequest(HttpContext context)
    {
        this.Request = context.Request;
        this.Response = context.Response;

        string songFilePath = null;
        if(this.Request.QueryString.Count > 0)
            songFilePath = UtilityMethods.GetPathByHash(this.Request.QueryString[0]);
        
        if (songFilePath != null && songFilePath.Length > 0)
        {
            FileInfo songFileInfo = new FileInfo(songFilePath);
            if (songFileInfo.Exists)
            {
                if (UtilityMethods.Returned304(songFileInfo))
                    return;
                
                UltraID3 mp3Info = new UltraID3();
                mp3Info.Read(songFileInfo.FullName);
                if (mp3Info.ID3v23Tag.FoundFlag)
                {
                    mp3Info.ID3v23Tag.Frames.Count.ToString();
                    ID3FrameCollection pictures = mp3Info.ID3v23Tag.Frames.GetFrames(HundredMilesSoftware.UltraID3Lib.MultipleInstanceFrameTypes.Picture);
                    if (pictures != null && pictures.Count > 0)
                    {
                        ID3PictureFrame selectedPicFrame = null;
                        foreach (ID3PictureFrame picFrame in pictures)
                        {
                            if (picFrame.Type == PictureTypes.CoverFront)
                            {
                                selectedPicFrame = picFrame;
                                break;
                            }
                        }
                        if (selectedPicFrame == null)
                            selectedPicFrame = pictures[0] as ID3PictureFrame;

                        if (selectedPicFrame.Picture != null)
                        {
                            string mimeType = selectedPicFrame.MIMEType;
                            this.Response.ContentType = mimeType;

                            this.Response.Cache.SetCacheability(HttpCacheability.Public);
                            this.Response.Cache.SetLastModified(songFileInfo.LastWriteTime);
                            this.Response.Expires = 60 * 24 * 30; // 30 days expressed in minutes
                            this.Response.Cache.SetSlidingExpiration(true);

                            //this.Response.AddHeader("Content-Disposition", "filename=" + fi.Name);
                            selectedPicFrame.Picture.Save(this.Response.OutputStream, selectedPicFrame.Picture.RawFormat);
                            return;
                        }
                    }
                }
            }
        }

        this.Response.Redirect(string.Format("{0}/Images/NoCoverArt.jpg", 
            UtilityMethods.GetFullAppUrlPrefix()));
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}