<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NavigationBar.ascx.cs" Inherits="Cuyahoga.Web.Admin.Controls.NavigationBar" %>
<div id="navigationbar" class="shadow">
    <asp:PlaceHolder ID="plhAdminOnlyOptions" runat="server">
        <asp:hyperlink id="hplTemplates" navigateurl="../Templates.aspx" runat="server">Templates</asp:hyperlink>
        <asp:hyperlink id="hplModules" navigateurl="../Modules.aspx" runat="server">Modules</asp:hyperlink>
        <asp:hyperlink id="hplSections" navigateurl="../Sections.aspx" runat="server">Standalone Sections</asp:hyperlink>
        <asp:hyperlink id="hplUsers" navigateurl="../Users.aspx" runat="server">Users</asp:hyperlink>
        <asp:hyperlink id="hplRoles" navigateurl="../Roles.aspx" runat="server">Roles</asp:hyperlink>
    </asp:PlaceHolder>
    <asp:HyperLink ID="hplCategories" NavigateUrl="../Categories.aspx" runat="server">Categories</asp:HyperLink>
    <asp:hyperlink id="hplRebuild" navigateurl="../RebuildIndex.aspx" runat="server">Search Database</asp:hyperlink>
</div>