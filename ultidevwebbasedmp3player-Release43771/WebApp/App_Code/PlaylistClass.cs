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
using System.Collections.Generic;
using System.Xml.Serialization;

using HundredMilesSoftware.UltraID3Lib;
using XSPF;

/// <summary>
/// Generates XSPF playlist out the given folder's contents.
/// </summary>
public class PlaylistClass : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{
	public PlaylistClass()
	{
	}

    private HttpRequest Request = null;
    private HttpResponse Response = null;

    public void ProcessRequest(HttpContext context)
    {
        this.Request = context.Request;
        this.Response = context.Response;

        string segmentZ = this.Request.Url.Segments[this.Request.Url.Segments.Length - 1].ToLower();
        bool recursive = segmentZ.StartsWith("recursive");

        bool shuffle = segmentZ.Contains("shuffle");

        // Playlist URL looks like 'http://host/<folder hash>/[Recursive.]Folder.dynplist'
        // For example, "/-385020135/Recursive.Folder.dynplist".
        string pathHash = this.Request.Url.Segments[this.Request.Url.Segments.Length - 2].TrimEnd('/');
        string folder = UtilityMethods.GetPathByHash(pathHash);

        if (folder == null || folder.Length == 0)
        {
            folder = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);

            if (folder == null || folder.Length == 0)
                folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"..");
        }

        DirectoryInfo dir = new DirectoryInfo(folder);
        if (!dir.Exists)
        {
            this.Response.StatusCode = 404;
            this.Response.ContentType = "text/html";
            this.Response.Write(string.Format("Folder \"{0}\" was not found.", dir.FullName));
            this.Response.End();
            return;
        }

        //if (UtilityMethods.Returned304(dir))
        //    return;

        this.GeneratePlaylist(dir, recursive, shuffle);
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

    private void GeneratePlaylist(DirectoryInfo dir, bool recursive, bool shuffle)
    {
        this.Response.Buffer = false;
        this.Response.ContentType = "application/xspf+xml";

        // Allow caching of the non-shuffled playlists.
        // Shuffled ones should be cached because they change every time.
        // Note: since v. 2.0 we have stopped generating shuffled lists
        // and instead generated regular lists and tell the player to do
        // shuffling on its own.
        this.Response.Cache.SetCacheability(shuffle ? HttpCacheability.NoCache : HttpCacheability.Private);
        //if (!shuffle)
        //    this.Response.Cache.SetLastModified(dir.LastAccessTime);

        PlaylistType playlist = new PlaylistType();
        this.RenderPlaylistHeader(dir, playlist);

        List<FileInfo> mediaFiles = new List<FileInfo>();
        UtilityMethods.FindMediaFiles(mediaFiles, dir, recursive);
        this.RenderPlaylistTracks(mediaFiles, playlist, shuffle);

        this.Response.ContentEncoding = System.Text.ASCIIEncoding.UTF8;
        XmlSerializer ser = new XmlSerializer(playlist.GetType());
        ser.Serialize(this.Response.Output, playlist);
    }

    private void RenderPlaylistHeader(DirectoryInfo dir, PlaylistType playlist)
    {
        playlist.title = dir.Name;
        playlist.creator = "UltiDev playlist generator";
        playlist.annotation = "Playlist generated on the fly from local media files.";
        playlist.info = new Uri(dir.FullName).ToString();
        playlist.location = HttpContext.Current.Server.UrlEncode(this.Request.Url.ToString());

        string folderImageFilePath = Path.Combine(dir.FullName, "folder.jpg");
        if (File.Exists(folderImageFilePath))
            playlist.image = string.Format("{0}/Handlers/CoverArt.ashx?FolderImage={1}",
                UtilityMethods.GetFullAppUrlPrefix(), folderImageFilePath);

        playlist.date = DateTime.UtcNow;
    }

    private void RenderPlaylistTracks(List<FileInfo> mediaFiles, PlaylistType playlist, bool shuffle)
    {
        List<TrackType> trackList = new List<TrackType>();

        string songPathPrefix = string.Format("{0}/Handlers/Song.ashx?", UtilityMethods.GetFullAppUrlPrefix());
        string imagePathPrefix = string.Format("{0}/Handlers/CoverArt.ashx?", UtilityMethods.GetFullAppUrlPrefix());
        string noImage = string.Format("{0}/Images/NoCoverArt.jpg", UtilityMethods.GetFullAppUrlPrefix());

        foreach (FileInfo mediaFileInfo in mediaFiles)
        {
            if (!mediaFileInfo.Exists)
                continue;

            TrackType trackInfo = new TrackType();

            try
            {
                UltraID3 mp3Info = new UltraID3();
                mp3Info.Read(mediaFileInfo.FullName);

                if (!mp3Info.ID3v23Tag.FoundFlag && !mp3Info.ID3v23Tag.FoundFlag)
                {
                    // No tags
                    trackInfo.title = mediaFileInfo.Name.Substring(0, mediaFileInfo.Name.Length - mediaFileInfo.Extension.Length);
                }
                else
                {
                    // Image
                    if (mp3Info.ID3v23Tag.FoundFlag)
                    {
                        ID3FrameCollection pictureFrames = mp3Info.ID3v23Tag.Frames.GetFrames(MultipleInstanceFrameTypes.Picture);
                        if (pictureFrames.Count > 0) // File has picture
                            trackInfo.image = imagePathPrefix + UtilityMethods.CalcAndStoreFilePathHash(mediaFileInfo.FullName);
                    }

                    // Title
                    if (mp3Info.Title == null || mp3Info.Title.Length == 0)
                        trackInfo.title = mediaFileInfo.Name.Substring(0, mediaFileInfo.Name.Length - mediaFileInfo.Extension.Length - 1);
                    else
                        trackInfo.title = mp3Info.Title;

                    trackInfo.meta = new MetaType[1];
                    trackInfo.meta[0] = new MetaType();
                    trackInfo.meta[0].rel = "length";
                    trackInfo.meta[0].Value = mediaFileInfo.Length.ToString();

                    trackInfo.album = mp3Info.Album;
                    trackInfo.creator = mp3Info.Artist;
                    trackInfo.annotation = mp3Info.Comments;
                    if (mp3Info.TrackNum.HasValue)
                        trackInfo.trackNum = mp3Info.TrackNum.Value.ToString();
                }
            }
            catch
            {
                trackInfo.title = mediaFileInfo.Name.Substring(0, mediaFileInfo.Name.Length - mediaFileInfo.Extension.Length);
            }

            trackInfo.location = new string[1];
            trackInfo.location[0] = songPathPrefix + UtilityMethods.CalcAndStoreFilePathHash(mediaFileInfo.FullName);

            if (trackInfo.title != null && trackInfo.title.Length > 0)
                trackList.Add(trackInfo);

            if (trackInfo.image == null || trackInfo.image.Length == 0)
                trackInfo.image = noImage;

            //System.GC.Collect();
        }

        playlist.trackList = trackList.ToArray();

        if (shuffle)
            UtilityMethods.ShuffleArray(playlist.trackList);
    }
}
