<%@ Control Language="c#" AutoEventWireup="false" Inherits="Cuyahoga.Web.UI.BaseTemplate" %><!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"><html xmlns="http://www.w3.org/1999/xhtml">
    <head>
        <title><asp:Literal ID="PageTitle" runat="server"></asp:Literal></title>
        <meta http-equiv="content-type" content="text/html; charset=utf-8" />
        <asp:Literal ID="MetaTags" runat="server" />
        <asp:Literal ID="Stylesheets" runat="server" />
        <asp:Literal ID="JavaScripts" runat="server" />
    </head>
    <body>
        <form id="t" method="post" runat="server">
            <div id="header"><div id="banner"></div></div>
            <div id="menu">
                <asp:PlaceHolder ID="Navigation_Main" runat="server"></asp:PlaceHolder>
            </div>
            <div id="contentwrapper">
                <div id="content">
                    <div id="posts">
                        <div class="post">
                            <asp:PlaceHolder ID="Content_Main" runat="server"></asp:PlaceHolder>
                        </div>
                    </div>
                    <div id="links">
                        <ul>
                            <li><asp:PlaceHolder ID="Navigation_Sub" runat="server"></asp:PlaceHolder></li>	                
                            <li><asp:PlaceHolder ID="Login_Main" runat="server"></asp:PlaceHolder></li>
                            <li><asp:PlaceHolder ID="Content_Side_Right" runat="server"></asp:PlaceHolder></li>
                        </ul>
                    </div>
                </div>
            </div>
            <div id="footer">
                <p>Copyright &copy; 2010 <a href="#">Company Name</a></p>
            </div>
        </form>
    </body>
</html>