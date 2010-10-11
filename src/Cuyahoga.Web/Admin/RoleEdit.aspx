<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RoleEdit.aspx.cs" Inherits="Cuyahoga.Web.Admin.RoleEdit" %><!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
  <head>
		<title>RoleEdit</title>
  </head>
	<body>

		<form id="Form1" method="post" runat="server">
		<p>Edit the rights of the selected role.</p>
			<div class="group">
				<table id="roles">
					<tr>
						<td id="rightname">Name</td>
						<td><asp:textbox id="txtName" runat="server" width="200px"></asp:textbox>
							<asp:requiredfieldvalidator id="rfvName" runat="server" errormessage="Name is required" cssclass="validator" display="Dynamic" enableclientscript="False" controltovalidate="txtName"></asp:requiredfieldvalidator>
						</td>
					</tr>
					<tr>
						<td>Rights</td>
						<td><asp:checkboxlist id="cblRights" runat="server" repeatlayout="Flow"></asp:checkboxlist>

						</td>
					</tr>
				</table>
			</div>
			<br/>
			<asp:button id="btnSave" runat="server" OnClick="BtnSaveClick" text="Save"></asp:button>
			<asp:Button id="btnCancel" runat="server" OnClick="BtnCancelClick" Text="Cancel" causesvalidation="false"></asp:Button>
			<asp:Button id="btnDelete" runat="server" OnClick="BtnDeleteClick" Text="Delete" causesvalidation="false"></asp:Button>
		</form>

	</body>
</html>
