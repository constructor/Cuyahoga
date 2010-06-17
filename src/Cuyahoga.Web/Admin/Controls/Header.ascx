<%@ Control Language="c#" AutoEventWireup="false" Codebehind="Header.ascx.cs" Inherits="Cuyahoga.Web.Admin.Controls.Header" %>
<div id="header">
	<div id="headertitle">
	    <span>Website Administration</span>
	</div>
</div>
<div id="subheader">
	<asp:hyperlink id="hplSite" runat="server"> View the current site </asp:hyperlink>
	&nbsp;&nbsp;&nbsp;&nbsp; 	
	<asp:linkbutton id="lbtLogout" runat="server"> Log out </asp:linkbutton>
	&nbsp;&nbsp;&nbsp;&nbsp;
</div>