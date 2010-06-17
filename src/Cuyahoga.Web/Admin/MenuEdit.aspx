<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MenuEdit.aspx.cs" Inherits="Cuyahoga.Web.Admin.MenuEdit" %><!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head>
		<title>MenuEdit</title>
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
			<div class="group">
				<h4>General</h4>
				<table>
					<tr>
						<td style="width: 200px">Name</td>
						<td><asp:textbox id="txtName" runat="server" width="200px"></asp:textbox><asp:requiredfieldvalidator id="rfvName" runat="server" enableviewstate="False" enableclientscript="False" controltovalidate="txtName"
								display="Dynamic" cssclass="validator" errormessage="Name is required"></asp:requiredfieldvalidator></td>
					</tr>
					<tr>
						<td>Placeholder</td>
						<td><asp:dropdownlist id="ddlPlaceholder" runat="server"></asp:dropdownlist></td>
					</tr>
				</table>
			</div>
			<div class="group">
				<h4>Nodes</h4>
				<table>
					<tr>
						<td>Available:<br/><asp:listbox id="lbxAvailableNodes" runat="server" width="150px" height="200px"></asp:listbox></td>
						<td><asp:button id="btnAdd" runat="server" OnClick="btnAdd_Click" width="50px" text=">"></asp:button><br/>
							<asp:button id="btnRemove" runat="server" OnClick="btnRemove_Click" width="50px" text="<"></asp:button></td>
						<td>Selected:<br/><asp:listbox id="lbxSelectedNodes" runat="server" width="150px" height="200px"></asp:listbox></td>
						<td>
							<asp:button id="btnUp" runat="server" OnClick="btnUp_Click" text="Up" width="50px"></asp:button><br/>
							<asp:button id="btnDown" runat="server" OnClick="btnDown_Click" text="Down" width="50px"></asp:button></td>
					</tr>
				</table>
			</div>
			<div><asp:button id="btnSave" runat="server" OnClick="btnSave_Click" text="Save"></asp:button><asp:button id="btnBack" runat="server" OnClick="BtnBackClick" text="Back" causesvalidation="False"></asp:button><asp:button id="btnDelete" runat="server" OnClick="btnDelete_Click" text="Delete" causesvalidation="False"></asp:button></div>
		</form>
	</body>
</html>
