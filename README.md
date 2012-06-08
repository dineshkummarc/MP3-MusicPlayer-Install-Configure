The source includes a Visual Studio 2005 Solution comprising of an ASP.NET application and a Setup Project.
Introduction
The source code for this article shows how to build a simple web-based media player for MP3 files, and demonstrates several aspects of ASP.NET programming, like caching, using of HTTP handlers, and making IIS-independent installers for redistributable ASP.NET applications.
In the end, you will build an installable application looking like this:
 
 
Using the code

![UltiDev Cassini Web Server for ASP.NET 2.0](http://www.codeproject.com/KB/aspnet/ASPNET_MP3_Player/UltiDevMP3Player-2-Thumbnail.png)
![UltiDev Cassini Web Server for ASP.NET 2.0](http://www.codeproject.com/KB/aspnet/ASPNET_MP3_Player/UltiDevMP3Player-thumbnail.png)

Important: Unzip the archive to "C:\". Some project path settings may need to be changed if the solution is opened on an x64 system.
Prerequisites
Visual Studio 2005
UltiDev Cassini Web Server for ASP.NET 2.0
UltiDev Cassini is packaged together with the application into the Setup.exe so that the final application would not depend on IIS being present on the target system.
Let's begin (Getting the ducks in a row)
Some time ago, I stumbled upon a great piece of free software: a Flash-based XSPF-compatible MP3 player. When embedded on a page, it can take a playlist over HTTP and play it. The second nice thing was that the XSPF play list format has XSD schema available. The .NET Framework xsd.exe utility allows easy conversion of XSD schemas into C# or VB.NET classes encapsulating the structure of the data defined by the XSD, as well as implementing XML serialization to and from XML files conforming to the schema. So, I had an XSPF-compatible MP3 player and a free code generating XSPF-compatible XML. That meant, I could easily create XSPF-compatible playlists on the fly. Only if I had a free ID3 tag (MP3 file metadata) access API...
Finding an ID3v2 library for .NET was harder than I expected. However, my search was ultimately successful. TheUltraID3Lib ended up being just what I needed. It's a nice library; may be, just a bit over the top object-oriented.
The final piece is the UltiDev Cassini Web Server for ASP.NET 2.0. It's necessary because first, it can be packaged and shipped along with any ASP.NET application, eliminating the requirement for IIS. Second, unlike IIS, the UltiDev Cassini service works under the "Local System" account, which enables access to any local file and folder on the server. One thing to note: while it is quite convenient to have a web server running under a powerful account, it may pose a risk if the application is exposed on the web. It's best to work with the application inside a protected local area network.
After you have downloaded the solution, unzip it to C:\. It will create a "C:\UltiDevMusicPlayerSample" folder. If you want to put it in some other folder - you can do that too - simply adjust your project debugging settings later to point to the correct application folder (see below).
Application flow
The application has a single page (Default.aspx) containing the player control and a file system browser (Controls/PlayerControl.ascx and Controls/PlayerControl.ascx.cs).
After the user selects a folder with MP3 files, the file system browser tree gets hidden and the player control is re-rendered to point to the dynamically-generated playlist representing the selected folder.
The player control requests a dynamic playlist, and a custom IHttpHandler (AppCode/PlaylistClass.cs andAppCode/xspf.cs) serves the XSPF-encoded playlist containing the songs in the selected folder. The playlist contains song information retrieved from the songs' ID3v2 and ID3v1 MP3 tags.
The player plays songs one by one: requesting each one from the custom IHttpHandler(Handlers/Song.ashx), serving songs from the local file system. After a song starts playing, the player also requests a song album artwork (cover art) from the custom IHttpHandler (Handlers/CoverArt.ashx) which serves the image extracted from the song's ID3v2 tag.
Debugging
I had trouble getting the Visual Studio 2005 internal web server to serve a Flash component. I switched to UltiDev Cassini for debugging, and that solved the problem. Debugging with UltiDev Cassini is probably a good idea anyway since the application is eventually going to run under UltiDev Cassini.
To switch to UltiDev Cassini, bring up the ASP.NET application's properties, select Start Options on the left, and check the "Start External Program" radio-button. Enter "C:\Program Files\UltiDev\Cassini Web Server for ASP.NET 2.0\UltiDevCassinWebServer2.exe" as the program to be used for debugging, and specify "/run c:\UltiDevMusicPlayerSample\WebApp Default.aspx 4125" (no quotes) as the command line arguments. If you have unzipped the solution to a folder other than "C:\", then you will need to modify thec:\UltiDevMusicPlayerSample\WebApp part of the command line arguments to point to the actual application location.
 
![UltiDev Cassini Web Server for ASP.NET 2.0 Property Pages](http://www.codeproject.com/KB/aspnet/ASPNET_MP3_Player/VS2005DebugSettingsForMP3App.png)

Setup project
Unlike a regular ASP.NET application, this application uses a regular (non-web) setup project for the installer implementation. The reason for that is the Visual Studio web setup project is actually an IIS setup project. Since we are using UltiDev Cassini instead of IIS, a regular setup project is required instead.
The setup project packs UltiDev Cassini into the Setup.exe bootstrapper, and ensures the application is registered with UltiDev Cassini during the installation process and gets unregistered during uninstallation.
Creating a setup project for an ASP.NET application bundled with UltiDev Cassini is not complex, but if you need astep-by-step guide, please refer to this walk-through.
Important: When installing the application, don't just click the .MSI file. You will need to run Setup.exe to ensure the UltiDev Cassini web server gets installed on the target system. This is especially true on Vista, where clicking a.MSI and running a Setup.exe are not nearly as functionally close as it used to be on Windows XP.
Build & Enjoy!
License
This article has no explicit license attached to it but may contain usage terms in the article text or the download files themselves. If in doubt please contact the author via the discussion board below.
