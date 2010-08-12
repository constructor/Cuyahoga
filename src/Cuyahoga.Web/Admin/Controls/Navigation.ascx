<%@ Control Language="c#" AutoEventWireup="false" CodeBehind="Navigation.ascx.cs" Inherits="Cuyahoga.Web.Admin.Controls.Navigation"  %>
<div class="navsection">
<%--	<asp:image imageurl="../Images/PanelLeftEnd.gif" runat="server" imagealign="left" id="i1" alternatetext="Home"></asp:image>--%>
	<h3>Sites</h3>
    <asp:Label Runat="server" ID="lblShowAll" AssociatedControlID="chkShowAll" CssClass="showall">Load all nodes into site(s) tree:</asp:Label>&nbsp</nbsp><asp:CheckBox Runat="server" ID="chkShowAll" AutoPostBack="true" oncheckedchanged="chkShowAll_CheckedChanged" onprerender="chkShowAll_PreRender" />
    <br />
    <div id="collapseallcontrol"><label for="expandall">Nodes: </label><a id="expandall" class="navLink" href="?#">Expand All</a> | <a id="collapseall" class="navLink" href="?#">Collapse All</a></div>
    <hr />
	<asp:placeholder id="plhNodes" runat="server"></asp:placeholder>
	<asp:PlaceHolder ID="plhAdminOnlyOptionAddSite" runat="server"></asp:PlaceHolder>
	<asp:Panel runat="server" ID="pnlNewSite" CssClass="newsite">
    <asp:image imageurl="../Images/site.png" runat="server" imagealign="left" id="inew" alternatetext="New Site"></asp:image><asp:hyperlink id="hplNew" navigateurl="../SiteEdit.aspx?SiteId=-1" cssclass="navLink" runat="server">Add a new site</asp:hyperlink>
    <hr />
    </asp:Panel>
</div>
<asp:PlaceHolder ID="plhAdminOnlyOptions" runat="server">
    <div class="navsection">
	    <%--<asp:image imageurl="../Images/PanelLeftEnd.gif" runat="server" imagealign="left" id="i2" alternatetext="Sections"></asp:image>--%>
	    <h3>Management</h3>
	    <asp:hyperlink id="hplSections" CssClass="navLink" navigateurl="../Sections.aspx" runat="server">Standalone Sections</asp:hyperlink>
    </div>
    <div class="navsection">
<%--	    <asp:image imageurl="../Images/PanelLeftEnd.gif" runat="server" imagealign="left" id="i3" alternatetext="Modules"></asp:image>
	    <h3>Modules</h3>--%>
	    <asp:hyperlink id="hplModules" CssClass="navLink" navigateurl="../Modules.aspx" runat="server">Modules</asp:hyperlink>
    </div>
    <div class="navsection">
<%--	    <asp:image imageurl="../Images/PanelLeftEnd.gif" runat="server" imagealign="left" id="i4" alternatetext="Templates"></asp:image>
	    <h3>Templates</h3>--%>
	    <asp:hyperlink id="hplTemplates" CssClass="navLink" navigateurl="../Templates.aspx" runat="server">Templates</asp:hyperlink>
    </div>
    <div class="navsection">
<%--	    <asp:image imageurl="../Images/PanelLeftEnd.gif" runat="server" imagealign="left" id="i5" alternatetext="Users"></asp:image>
	    <h3>Users</h3>--%>
	    <asp:hyperlink id="hplUsers" CssClass="navLink" navigateurl="../Users.aspx" runat="server">Users</asp:hyperlink>
    </div>
    <div class="navsection">
<%--	    <asp:image imageurl="../Images/PanelLeftEnd.gif" runat="server" imagealign="left" id="i6" alternatetext="Roles"></asp:image>
	    <h3>Roles</h3>--%>
	    <asp:hyperlink id="hplRoles" CssClass="navLink" navigateurl="../Roles.aspx" runat="server">Roles</asp:hyperlink>
    </div>
</asp:PlaceHolder>
<div class="navsection">
<%--	<asp:image imageurl="../Images/PanelLeftEnd.gif" runat="server" imagealign="left" id="i7" alternatetext="FullText index"></asp:image>
	<h3>Search</h3>--%>
	<asp:hyperlink id="hplRebuild" CssClass="navLink" navigateurl="../RebuildIndex.aspx" runat="server">Search Database</asp:hyperlink>
</div>
<div class="navsection">
<%--<asp:image imageurl="../Images/PanelLeftEnd.gif" runat="server" imagealign="left" id="Image1" alternatetext="FullText index"></asp:image>
	<h3>Categories</h3>--%>
	<asp:HyperLink ID="hplCategories" CssClass="navLink" NavigateUrl="../Categories.aspx" runat="server">Categories</asp:HyperLink>
</div>