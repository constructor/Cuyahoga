<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Header.ascx.cs" Inherits="Cuyahoga.Web.Admin.Controls.Header" %>
<div id="header">
    <h1>Website Administration</h1>
    <div id="viewbuttons">
	    <asp:hyperlink id="hplSite" runat="server"> View the current site </asp:hyperlink>
        &nbsp;&nbsp;&nbsp;&nbsp; 	
        <asp:linkbutton id="lbtLogout" runat="server" onclick="lbtLogout_Click"> Log out </asp:linkbutton>
        &nbsp;&nbsp;&nbsp;&nbsp;
    </div>
</div>
