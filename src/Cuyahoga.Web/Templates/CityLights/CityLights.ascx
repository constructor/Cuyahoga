<%@ Control Language="c#" AutoEventWireup="false" Inherits="Cuyahoga.Web.UI.BaseTemplate" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
    <head>
        <title><asp:Literal ID="PageTitle" runat="server"></asp:Literal></title>
        <meta http-equiv="content-type" content="text/html; charset=utf-8" />
        <asp:Literal ID="MetaTags" runat="server" />
        <asp:Literal ID="Stylesheets" runat="server" />
        <asp:Literal ID="JavaScripts" runat="server" />
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