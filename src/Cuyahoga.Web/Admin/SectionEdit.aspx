<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SectionEdit.aspx.cs" Inherits="Cuyahoga.Web.Admin.SectionEdit" %><!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head>
		<title>SectionEdit</title>
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
			<div class="group">
				<h4>General</h4>
				<table>
					<tr>
						<td style="width:100px">Section title</td>
						<td><asp:textbox id="txtTitle" runat="server" width="300px"></asp:textbox>
							<asp:requiredfieldvalidator id="rfvTitle" runat="server" display="Dynamic" cssclass="validator" controltovalidate="txtTitle"
								enableclientscript="False">Title is required</asp:requiredfieldvalidator></td>
					</tr>
					<tr>
						<td style="width:100px">CSS Class</td>
						<td><asp:textbox id="txtCSSClass" runat="server" width="300px"></asp:textbox></td>
					</tr>
					<tr>
						<td>Show section title</td>
						<td><asp:checkbox id="chkShowTitle" runat="server"></asp:checkbox></td>
					</tr>
					<tr>
						<td>Module</td>
						<td>
							<asp:dropdownlist id="ddlModule" runat="server" autopostback="True" visible="False"></asp:dropdownlist>
							<asp:label id="lblModule" runat="server" visible="False"></asp:label>
						</td>
					</tr>
					<tr>
						<td>Placeholder</td>
						<td>
							<asp:dropdownlist id="ddlPlaceholder" runat="server"></asp:dropdownlist>
							&nbsp;<asp:hyperlink id="hplLookup" runat="server">Lookup</asp:hyperlink>
						</td>
					</tr>
					<tr>
						<td>Cache duration</td>
						<td><asp:textbox id="txtCacheDuration" runat="server" width="30px"></asp:textbox><asp:requiredfieldvalidator id="rfvCache" runat="server" display="Dynamic" cssclass="validator" controltovalidate="txtCacheDuration"
								enableclientscript="False">Cache duration is required (0 for no cache)</asp:requiredfieldvalidator><asp:comparevalidator id="cpvCache" runat="server" display="Dynamic" cssclass="validator" controltovalidate="txtCacheDuration"
								enableclientscript="False" errormessage="Only positive integers allowed" operator="GreaterThanEqual" valuetocompare="0" type="Integer"></asp:comparevalidator></td>
					</tr>
				</table>
			</div>
			<asp:panel id="pnlCustomSettings" cssclass="group" runat="server" enableviewstate="false">
				<h4>Custom settings</h4>
				<table>
					<asp:placeholder id="plcCustomSettings" runat="server" />
				</table>
			</asp:panel>
			<asp:panel id="pnlConnections" cssclass="group" runat="server" visible="False">
				<h4>Connections</h4>
				<table class="tbl">
					<asp:repeater id="rptConnections" OnItemCommand="RptConnectionsItemCommand" runat="server">
						<headertemplate>
							<tr>
								<th>To section</th>
								<th>Action</th>
								<th></th>
							</tr>
						</headertemplate>
						<itemtemplate>
							<tr>
								<td><%# DataBinder.Eval(Container.DataItem, "Value.FullName") %></td>
								<td><%# DataBinder.Eval(Container.DataItem, "Key") %></td>
								<td><asp:linkbutton id="lbtDelete" runat="server" causesvalidation="False" commandname="DeleteConnection" commandargument='<%# DataBinder.Eval(Container.DataItem, "Key") %>'>Delete</asp:linkbutton></td>
							</tr>
						</itemtemplate>
					</asp:repeater>
				</table>
				<asp:hyperlink id="hplNewConnection" runat="server">Add connection</asp:hyperlink>
			</asp:panel>
			<div class="group">
				<h4>Authorization</h4>
				<table class="tbl">
					<asp:repeater id="rptRoles" runat="server">
						<headertemplate>
							<tr>
								<th>Role</th>
								<th>View allowed</th>
								<th>Edit allowed</th>
							</tr>
						</headertemplate>
						<itemtemplate>
							<tr>
								<td><%# DataBinder.Eval(Container.DataItem, "Name") %></td>
								<td style="text-align:center">
									<asp:checkbox id="chkViewAllowed" runat="server"></asp:checkbox></td>
								<td style="text-align:center">
									<asp:checkbox id="chkEditAllowed" runat="server"></asp:checkbox></td>
							</tr>
						</itemtemplate>
					</asp:repeater></table>
			</div>
			<div>
				<asp:button id="btnSave" runat="server" OnClick="BtnSaveClick" text="Save"></asp:button>
				<asp:button id="btnBack" runat="server" OnClick="BtnBackClick" text="Back" causesvalidation="False"></asp:button>
			</div>
			<script type="text/javascript"> <!--
			function setPlaceholderValue(ddlist, val)
			{
				var placeholdersList = document.getElementById(ddlist);
				if (placeholdersList != null)
				{
					for (i = 0; i < placeholdersList.options.length; i++)
					{
						if (placeholdersList.options[i].value == val)
						{
							placeholdersList.selectedIndex = i;
						}
					}				
				}
			}
			// -->
			</script>
		</form>
	</body>
</html>