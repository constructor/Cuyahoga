<%@ Control Language="c#" AutoEventWireup="false" Inherits="Cuyahoga.Web.UI.BaseTemplate" %><!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title><asp:Literal ID="PageTitle" runat="server"></asp:Literal></title>
    <meta http-equiv="content-type" content="text/html; charset=utf-8" />
    <script src="<% =String.Format("{0}Templates/Corporate/javascript/jquery-1.2.6.pack.js", Cuyahoga.Web.Util.UrlHelper.GetApplicationPath().ToString()) %>" type="text/javascript"></script>
    
    <asp:Literal ID="MetaTags" runat="server" />
    <asp:Literal ID="Stylesheets" runat="server" />
    <asp:Literal ID="JavaScripts" runat="server" />
</head>
<body>
<form id="t" method="post" runat="server">
    <div id="menu">
        <asp:PlaceHolder ID="MainNavigation" runat="server"></asp:PlaceHolder>
    </div>
    <div id="logo">
        <h1><a href="#">Corporate</a></h1><h2>Sub Heading Here</h2>
    </div>
    <div id="page">
        <div id="contentforum">
            <asp:PlaceHolder ID="MainContent" runat="server"></asp:PlaceHolder>
        </div>
    </div>
    <div id="footer">
        <p>Copyright &copy; 2009 Company Name. Designed by <a href="http://www.novodevelopments.com/">Novodevelopments</a></p>
    </div>
</form>
</body>
</html>