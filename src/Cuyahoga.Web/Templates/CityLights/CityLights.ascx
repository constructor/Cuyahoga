<%@ Control Language="c#" AutoEventWireup="false" Inherits="Cuyahoga.Web.UI.BaseTemplate" %><%@ Register TagPrefix="uc1" TagName="navigation" Src="~/Controls/Navigation/HierarchicalMenu.ascx" %><!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
&nbsp;<html xmlns="http://www.w3.org/1999/xhtml"><head><title><asp:literal id="PageTitle" runat="server"></asp:literal></title><meta http-equiv="content-type" content="text/html; charset=utf-8" /><script src="<% =String.Format("{0}Templates/CityLights/js/jquery-1.4.1.min.js", Cuyahoga.Web.Util.UrlHelper.GetApplicationPath().ToString()) %>" type="text/javascript"></script><script src="<% =String.Format("{0}Templates/CityLights/js/jquery.droppy.js", Cuyahoga.Web.Util.UrlHelper.GetApplicationPath().ToString()) %>" type="text/javascript"></script><script src="<% =String.Format("{0}Templates/CityLights/js/page.js", Cuyahoga.Web.Util.UrlHelper.GetApplicationPath().ToString()) %>" type="text/javascript"></script><asp:literal id="MetaTags" runat="server" />
    <asp:literal id="Stylesheets" runat="server" />
    <asp:literal id="JavaScripts" runat="server" />
</head>
<body>
<form id="t" method="post" runat="server">
    <div id="header">
	    <div id="logo">
		    <h1><a href="#">Cuyahoga</a></h1>
		    <p><a href="http://www.cuyahoga-project.org">Cuyahoga</a></p>
	    </div>
	    <div id="menu" style="margin-bottom: 0px">
	        <asp:PlaceHolder ID="Navigation_Main" runat="server"></asp:PlaceHolder>
	    </div>
    </div>
    <div id="banner">&nbsp;</div>
    <div id="page">
	    <div id="sidebar">
		    <ul>	
 			    <li><asp:PlaceHolder ID="Navigation_Sub" runat="server"></asp:PlaceHolder></li>
			    <li><asp:PlaceHolder ID="Login_Main" runat="server"></asp:PlaceHolder></li>
			    <li><asp:PlaceHolder ID="Content_Side_Right" runat="server"></asp:PlaceHolder></li>
		    </ul>
	    </div>
	    <div id="content">
                <asp:PlaceHolder ID="Content_Main" runat="server"></asp:PlaceHolder>
		    <br style="clear: both;" />
	    </div>
	    <br style="clear: both;" />
    </div>
    <div id="footer">
        <p>Copyright &copy; 2010 <a href="#">Company Name</a></p>
    </div>
</form>
</body>
</html>