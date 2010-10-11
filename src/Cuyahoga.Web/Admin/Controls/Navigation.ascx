<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Navigation.ascx.cs" Inherits="Cuyahoga.Web.Admin.Controls.Navigation" %>
<div class="navsection">

	<h3>Sites</h3>
    <asp:Label Runat="server" ID="lblShowAll" AssociatedControlID="chkShowAll" CssClass="showall" EnableViewState="false">Load all nodes into site(s) tree:</asp:Label>&nbsp&nbsp<asp:CheckBox Runat="server" ID="chkShowAll" CssClass="chkshowall" AutoPostBack="true" oncheckedchanged="chkShowAll_CheckedChanged" onprerender="chkShowAll_PreRender" />
    <br />
    <div id="collapseallcontrol"><a id="expandall" class="navLink" href="?#">Expand All</a> | <a id="collapseall" class="navLink" href="?#">Collapse All</a></div>
    <hr />
	<asp:placeholder id="plhNodes" runat="server"></asp:placeholder>
	<asp:PlaceHolder ID="plhAdminOnlyOptionAddSite" runat="server"></asp:PlaceHolder>
	<asp:Panel runat="server" ID="pnlNewSite" CssClass="newsite">
    <asp:image imageurl="../Images/site.png" runat="server" imagealign="left" id="inew" alternatetext="New Site"></asp:image><asp:hyperlink id="hplNew" navigateurl="../SiteEdit.aspx?SiteId=-1" cssclass="nodeLink" runat="server">Add a new site</asp:hyperlink>
    <hr />
    </asp:Panel>
    
</div>