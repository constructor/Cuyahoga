<%@ Control Language="C#" EnableViewState="false" AutoEventWireup="false" Inherits="FredCK.FCKeditorV2.FileBrowser.Config" %>
<%--
 * FCKeditor - The text editor for Internet - http://www.fckeditor.net
 * Copyright (C) 2003-2007 Frederico Caldeira Knabben
 *
 * == BEGIN LICENSE ==
 *
 * Licensed under the terms of any of the following licenses at your
 * choice:
 *
 *  - GNU General Public License Version 2 or later (the "GPL")
 *    http://www.gnu.org/licenses/gpl.html
 *
 *  - GNU Lesser General Public License Version 2.1 or later (the "LGPL")
 *    http://www.gnu.org/licenses/lgpl.html
 *
 *  - Mozilla Public License Version 1.1 or later (the "MPL")
 *    http://www.mozilla.org/MPL/MPL-1.1.html
 *
 * == END LICENSE ==
 *
 * Configuration file for the File Browser Connector for ASP.NET.
--%>
<script runat="server">
    
    private Cuyahoga.Core.Domain.User cuyahogaUser;
	/**
	 * This function must check the user session to be sure that he/she is
	 * authorized to upload and access files in the File Browser.
	 */
    
	private bool CheckAuthentication()
	{
		cuyahogaUser = this.Page.User.Identity as Cuyahoga.Core.Domain.User;
		if (cuyahogaUser == null)
		{
			return false;
		}
		else
		{
			return (cuyahogaUser.IsInRole("Administrator") || cuyahogaUser.IsInRole("Editor"));
		}
	}
 
	public override void SetConfig()
	{
		// SECURITY: You must explicitly enable this "connector". (Set it to "true").
		Enabled = CheckAuthentication();

		// URL path to user files.
        UserFilesPath = Cuyahoga.Web.Util.UrlHelper.GetApplicationPath() + "SiteData/" + Session["SiteId"].ToString();

		// The connector tries to resolve the above UserFilesPath automatically.
		// Use the following setting it you prefer to explicitely specify the
		// absolute path. Examples: 'C:\\MySite\\userfiles\\' or '/root/mysite/userfiles/'.
		// Attention: The above 'UserFilesPath' URL must point to the same directory.
        UserFilesAbsolutePath = Server.MapPath("~/SiteData/") + Session["SiteId"].ToString();

		// Due to security issues with Apache modules, it is recommended to leave the
		// following setting enabled.
		ForceSingleExtension = true;

		// Allowed Resource Types
		AllowedTypes = new string[] { "File", "Image", "Flash", "Media" };

		// For security, HTML is allowed in the first Kb of data for files having the
		// following extensions only.
		HtmlExtensions = new string[] { "html", "htm", "xml", "xsd", "txt", "js" };
        
        TypeConfig["File"].AllowedExtensions = new string[] { "7z", "aiff", "asf", "avi", "bmp", "csv", "doc", "fla", "flv", "gif", "gz", "gzip", "jpeg", "jpg", "mid", "mov", "mp3", "mp4", "mpc", "mpeg", "mpg", "ods", "odt", "pdf", "png", "ppt", "pxd", "qt", "ram", "rar", "rm", "rmi", "rmvb", "rtf", "sdc", "sitd", "swf", "sxc", "sxw", "tar", "tgz", "tif", "tiff", "txt", "vsd", "wav", "wma", "wmv", "xls", "xml", "zip" };
        TypeConfig["File"].DeniedExtensions = new string[] { };
        TypeConfig["File"].FilesPath = "%UserFilesPath%file/";
        TypeConfig["File"].FilesAbsolutePath = (UserFilesAbsolutePath == "" ? "" : "%UserFilesAbsolutePath%file/");
        TypeConfig["File"].QuickUploadPath = "%UserFilesPath%file/";
        TypeConfig["File"].QuickUploadAbsolutePath = (UserFilesAbsolutePath == "" ? "" : "%UserFilesAbsolutePath%file/");
        //TypeConfig["File"].QuickUploadPath = "%UserFilesPath%";
        //TypeConfig["File"].QuickUploadAbsolutePath = (UserFilesAbsolutePath == "" ? "" : "%UserFilesAbsolutePath%");
        // (Custom) Above original changed to allow QuickUpload (not browsing upload) to be inside folders not at root
        
        TypeConfig["Image"].AllowedExtensions = new string[] { "bmp", "gif", "jpeg", "jpg", "png" };
        TypeConfig["Image"].DeniedExtensions = new string[] { };
        TypeConfig["Image"].FilesPath = "%UserFilesPath%image/";
        TypeConfig["Image"].FilesAbsolutePath = (UserFilesAbsolutePath == "" ? "" : "%UserFilesAbsolutePath%image/");
        TypeConfig["Image"].QuickUploadPath = "%UserFilesPath%image/";
        TypeConfig["Image"].QuickUploadAbsolutePath = (UserFilesAbsolutePath == "" ? "" : "%UserFilesAbsolutePath%image/");
        //TypeConfig[ "Image" ].QuickUploadPath			= "%UserFilesPath%";
        //TypeConfig[ "Image" ].QuickUploadAbsolutePath	= ( UserFilesAbsolutePath == "" ? "" : "%UserFilesAbsolutePath%" );
        // (Custom) Above original changed to allow QuickUpload (not browsing upload) to be inside folders not at root
        
        TypeConfig["Flash"].AllowedExtensions = new string[] { "swf", "flv" };
        TypeConfig["Flash"].DeniedExtensions = new string[] { };
        TypeConfig["Flash"].FilesPath = "%UserFilesPath%flash/";
        TypeConfig["Flash"].FilesAbsolutePath = (UserFilesAbsolutePath == "" ? "" : "%UserFilesAbsolutePath%flash/");
        TypeConfig["Flash"].QuickUploadPath = "%UserFilesPath%flash/";
        TypeConfig["Flash"].QuickUploadAbsolutePath = (UserFilesAbsolutePath == "" ? "" : "%UserFilesAbsolutePath%flash/");
        //TypeConfig["Flash"].QuickUploadPath = "%UserFilesPath%";
        //TypeConfig["Flash"].QuickUploadAbsolutePath = (UserFilesAbsolutePath == "" ? "" : "%UserFilesAbsolutePath%");
        // (Custom) Above original changed to allow QuickUpload (not browsing upload) to be inside folders not at root
        
        TypeConfig["Media"].AllowedExtensions = new string[] { "aiff", "asf", "avi", "bmp", "fla", "flv", "gif", "jpeg", "jpg", "mid", "mov", "mp3", "mp4", "mpc", "mpeg", "mpg", "png", "qt", "ram", "rm", "rmi", "rmvb", "swf", "tif", "tiff", "wav", "wma", "wmv" };
        TypeConfig["Media"].DeniedExtensions = new string[] { };
        TypeConfig["Media"].FilesPath = "%UserFilesPath%media/";
        TypeConfig["Media"].FilesAbsolutePath = (UserFilesAbsolutePath == "" ? "" : "%UserFilesAbsolutePath%media/");
        TypeConfig["Media"].QuickUploadPath = "%UserFilesPath%media/";
        TypeConfig["Media"].QuickUploadAbsolutePath = (UserFilesAbsolutePath == "" ? "" : "%UserFilesAbsolutePath%media/");
        //TypeConfig["Media"].QuickUploadPath = "%UserFilesPath%";
        //TypeConfig["Media"].QuickUploadAbsolutePath = (UserFilesAbsolutePath == "" ? "" : "%UserFilesAbsolutePath%");
        // (Custom) Above original changed to allow QuickUpload (not browsing upload) to be inside folders not at root
    }
	
</script>