<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SiteEdit.aspx.cs" Inherits="Cuyahoga.Web.Admin.SiteEdit" %><!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
    <head>
		<title>SiteEdit</title>
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
		
		<fieldset>
		<legend>General</legend>
            <label>Name</label>
            <asp:textbox id="txtName" runat="server" width="300px" maxlength="100"></asp:textbox>
            <asp:requiredfieldvalidator id="rfvName" runat="server" enableclientscript="False" controltovalidate="txtName" cssclass="validator" display="Dynamic">Name is required</asp:requiredfieldvalidator><hr />
            
            <label>Site url (incl. http://)</label>
            <asp:textbox id="txtSiteUrl" runat="server" width="300px" maxlength="100"></asp:textbox>
            <asp:requiredfieldvalidator id="rfvSiteUrl" runat="server" enableclientscript="False" controltovalidate="txtSiteUrl" cssclass="validator" display="Dynamic">Site url is required</asp:requiredfieldvalidator><hr />
            
            <label>Webmaster email</label>
            <asp:textbox id="txtWebmasterEmail" runat="server" maxlength="100" width="300px"></asp:textbox>
            <asp:requiredfieldvalidator id="rfvWebmasterEmail" runat="server" errormessage="Webmaster email is required" cssclass="validator" display="Dynamic" controltovalidate="txtWebmasterEmail" enableclientscript="False"></asp:requiredfieldvalidator><hr />
            
            <label>Use friendly url's</label>
            <asp:checkbox id="chkUseFriendlyUrls" runat="server"></asp:checkbox>
		</fieldset>
		
		<fieldset>
		<legend>Defaults</legend>
			<label>Template</label>
            <asp:DropDownList ID="ddlTemplates" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DdlTemplatesSelectedIndexChanged"></asp:DropDownList>
            <asp:HyperLink ID="hlAddTemplate" NavigateUrl="~/Admin/TemplateEdit.aspx?TemplateId=-1" runat="server">Add Template</asp:HyperLink><hr />
            
			<label>Placeholder</label>
			<asp:dropdownlist id="ddlPlaceholders" runat="server"></asp:dropdownlist><em>(this is the placeholder where the content of general pages is inserted)</em><hr />

			<label>Culture</label>
			<asp:dropdownlist id="ddlCultures" runat="server"></asp:dropdownlist><hr />

			<label>Role for registered users</label>
			<asp:dropdownlist id="ddlRoles" runat="server"></asp:dropdownlist><hr />

			<label>Meta description</label>
            <asp:TextBox ID="txtMetaDescription" runat="server" MaxLength="500" TextMode="MultiLine" Width="400px" height="70px"></asp:TextBox><hr />

			<label>Meta keywords</label>
			<asp:TextBox ID="txtMetaKeywords" runat="server" MaxLength="500" TextMode="MultiLine" Width="400px" height="70px"></asp:TextBox><hr />
        </fieldset>
			
			<asp:panel id="pnlAliases" runat="server" cssclass="group">
				<h4>Aliases</h4>
				<table class="tbl">
					<asp:repeater id="rptAliases" runat="server" 
                        onitemdatabound="RptAliasesItemDataBound">
						<headertemplate>
							<tr>
								<th>Alias url</th>
								<th>Entry Node</th>
								<th>&nbsp;</th>
							</tr>
						</headertemplate>
						<itemtemplate>
							<tr>
								<td><%# DataBinder.Eval(Container.DataItem, "Url") %></td>
								<td><asp:label id="lblEntryNode" runat="server"></asp:label></td>
								<td><asp:hyperlink id="hplEdit" runat="server">Edit</asp:hyperlink></td>
							</tr>
						</itemtemplate>
					</asp:repeater></table>
				<asp:hyperlink id="hplNewAlias" runat="server">Add alias</asp:hyperlink>
			</asp:panel>
			
			<div>
				<asp:button id="btnSave" runat="server" text="Save" onclick="BtnSaveClick"></asp:button>
				<asp:button id="btnCancel" runat="server" text="Cancel" causesvalidation="False" onclick="BtnCancelClick"></asp:button>
				<asp:button id="btnDelete" runat="server" text="Delete" causesvalidation="False" onclick="BtnDeleteClick"></asp:button>
			</div>
			
		</form>
	</body>
</html>
