<%@ Register TagPrefix="fckeditorv2" Namespace="FredCK.FCKeditorV2" Assembly="FredCK.FCKeditorV2" %>
<%@ Page language="c#" Codebehind="EditHtml.aspx.cs" AutoEventWireup="True" Inherits="Cuyahoga.Modules.StaticHtml.EditHtml" ValidateRequest="false" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
	<head>
		<title>EditHtml</title>
	</head>
	<body >
		<form id="Form1" method="post" runat="server">
			<div id="moduleadminpane">
			    <h1>Edit Static HTML Content</h1>
			    <hr />
                <asp:Label ID="lblTitle" runat="server" AssociatedControlID="tbTitle" Text="Title: "></asp:Label><asp:TextBox ID="tbTitle" runat="server"></asp:TextBox>&nbsp;(for search results page)
			    <hr />
			    <fckeditorv2:fckeditor id="fckEditor" runat="server" height="440px" width="100%"></fckeditorv2:fckeditor>
			    <br/>
			    <br/>
			    <asp:button id="btnSave" runat="server" text="Save" onclick="btnSave_Click"></asp:button>
			</div>
		</form>
	</body>
</html>