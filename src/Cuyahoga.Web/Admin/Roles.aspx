<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Roles.aspx.cs" Inherits="Cuyahoga.Web.Admin.Roles" %><!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head>
		<title>Roles</title>
	</head>
	<body>
		<form id="Form1" method="post" runat="server">&nbsp;
			<table class="tbl">
				<asp:repeater id="rptRoles" runat="server" OnItemDataBound="RptRolesItemDataBound">
					<headertemplate>
						<tr>
						    <th></th>
							<th>Rolename</th>
							<th>Right(s)</th>
							<th>Last update</th>
							<th></th>
						</tr>
					</headertemplate>
					<itemtemplate>
						<tr>
						    <td><asp:Image Width="14" Height="12" ImageAlign="Middle" runat="server" ID="imgRole" /></td>
							<td><%# DataBinder.Eval(Container.DataItem, "Name") %></td>
							<td><asp:label id="lblRights" runat="server"></asp:label></td>
							<td><asp:label id="lblLastUpdate" runat="server"></asp:label></td>
							<td>
								<asp:hyperlink id="hplEdit" runat="server">Edit</asp:hyperlink>
							</td>
						</tr>
					</itemtemplate>
				</asp:repeater>
			</table>
			<br/>
			<div>
				<asp:button id="btnNew" runat="server" OnClick="BtnNewClick" text="Add new role"></asp:button>
			</div>
		</form>
	</body>
</html>
