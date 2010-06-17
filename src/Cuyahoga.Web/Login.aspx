<%@ Page language="c#" Codebehind="Login.aspx.cs" AutoEventWireup="false" Inherits="Cuyahoga.Web.Admin.Login" %><!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head>
		<title>Login</title>
		<link rel="stylesheet" type="text/css" href="Admin/Css/Admin.css"/>
		<!--[if IE 6]>
        <link href="Admin/Css/AdminIE6.css" type="text/css" rel="stylesheet">
        <![endif]-->
	</head>
	<body>
		<form id="Form1" method="post" runat="server" class="centre">
		    <div id="loginarea">
		        <div id="login">
		            <div id="cuyahoga"></div>
			        <h3 class="hidden">Site Administration login</h3>
                    <asp:Label ID="lblUsername" runat="server" AssociatedControlID="txtUsername" Text="Username"></asp:Label>
			        <asp:TextBox id="txtUsername" runat="server" MaxLength="18"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredtxtUsername" ControlToValidate="txtUsername" runat="server" ErrorMessage="*"></asp:RequiredFieldValidator>
			        <br />
                    <asp:Label ID="lblPassword" runat="server" AssociatedControlID="txtPassword" Text="Password"></asp:Label>
			        <asp:TextBox id="txtPassword" runat="server" MaxLength="18" textmode="Password"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredtxtPassword" ControlToValidate="txtPassword" runat="server" ErrorMessage="*"></asp:RequiredFieldValidator>
			        <br />
			        <asp:Label id="lblError" runat="server" enableviewstate="False" visible="False" CssClass="loginerror"></asp:Label>
		            <div id="btnLoginwrap"><asp:Button id="btnLogin" CssClass="btnLogin" runat="server" Text="Login"></asp:Button></div>
		        </div>
		    </div>
		</form>
	</body>
</html>