<%@ Control Language="c#" AutoEventWireup="false" Codebehind="ModuleAdminHeader.ascx.cs" Inherits="Cuyahoga.Web.Controls.ModuleAdminHeader" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<div id="header">
    <h1>Cuyahoga Module Administration</h1>
    <div id="viewbuttons">
        <h3>Current Node: <asp:label id="lblNode" runat="server"></asp:label></h3> |
        <h3>Section Name: <asp:label id="lblSection" runat="server"></asp:label></h3> 
        <asp:hyperlink id="hplBack" runat="server">Return to site</asp:hyperlink>
    </div>
</div>