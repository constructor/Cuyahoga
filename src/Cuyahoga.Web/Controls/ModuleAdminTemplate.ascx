<%@ Control Language="c#" AutoEventWireup="false" Codebehind="ModuleAdminTemplate.ascx.cs" Inherits="Cuyahoga.Web.Controls.ModuleAdminTemplate"%>
<%@ Register TagPrefix="uc1" TagName="header" Src="../Controls/ModuleAdminHeader.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head>
		<title><asp:literal id="PageTitle" runat="server"></asp:literal></title>
		<link id="CssStyleSheet" rel="stylesheet" type="text/css" runat="server" />
		<link type="text/css" href="/js/jquery-ui-1.8.4/css/cuyahoga-green/jquery-ui-1.8.4.custom.css" rel="stylesheet" />	
	    <asp:PlaceHolder ID="AddedCssPlaceHolder" runat="server"></asp:PlaceHolder>
        <asp:PlaceHolder ID="AddedJavaScriptPlaceHolder" runat="server"></asp:PlaceHolder>
		<script type="text/javascript" src="<%= ResolveUrl("~/js/jquery-1.4.2.min.js") %>"></script>
		<script type="text/javascript" src="<%= ResolveUrl("~/js/jquery-ui-1.8.4/js/jquery-ui-1.8.4.custom.min.js") %>"></script>
	    <script type="text/javascript">
            $(document).ready(function() {
                //UI Buttons
                $("input:submit, input:button, div[id='header'] a, .tbl a, a[id*='NewSection']").button();
                $("input,textarea,select,.group,fieldset,legend,#moduleadminpane .AspNet-GridView a[id*='btnSelect'],#moduleadminpane .AspNet-GridView a[id*='btnEdit'],#moduleadminpane .AspNet-GridView a[id*='btnUpdate'],#moduleadminpane .AspNet-GridView a[id*='btnCancel'],#moduleadminpane .AspNet-GridView a[id*='btnDelete']").addClass("ui-corner-all");
            });
        </script>
	</head>
	<body>
		<form id="Frm" method="post" enctype="multipart/form-data" runat="server">
			<uc1:header id="header" runat="server"></uc1:header>
			<div id="adminwrap">
			    <div id="padding" class="cleanpad16">
			        <div id="MessageBox" class="messagebox" runat="server" visible="false" enableviewstate="false"></div>
			        <asp:placeholder id="PageContent" runat="server"></asp:placeholder>
			    </div>
			</div>
		</form>
	</body>
</html>