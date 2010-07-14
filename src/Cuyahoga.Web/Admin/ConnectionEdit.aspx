<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConnectionEdit.aspx.cs" Inherits="Cuyahoga.Web.Admin.ConnectionEdit" %><!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
    <head>
		<title>ConnectionEdit</title>
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
			<div class="group">
				<h4>From section</h4>
				<table>
					<tr>
						<td>Section</td>
						<td><asp:label id="lblSectionFrom" runat="server"></asp:label></td>
					</tr>
					<tr>
						<td>Module type</td>
						<td><asp:label id="lblModuleType" runat="server"></asp:label></td>
					</tr>
					<tr>
					    <td>Action</td>
						<td><asp:dropdownlist id="ddlAction" runat="server" OnSelectedIndexChanged="DdlAction_SelectedIndexChanged" autopostback="True"></asp:dropdownlist></td>
					</tr>
				</table>
			</div>
			<asp:panel id="pnlTo" cssclass="group" runat="server" visible="False">
				<h4>To section</h4>
				<table>
					<tr>
						<td style="width: 100px">Section</td>
						<td><asp:dropdownlist id="ddlSectionTo" runat="server"></asp:dropdownlist></td>
					</tr>
				</table>
			</asp:panel>
			<div>
				<asp:button id="btnSave" runat="server" text="Save" OnClick="btnSave_Click" enabled="False"></asp:button>
				<asp:button id="btnBack" runat="server" text="Back" OnClick="btnBack_Click" causesvalidation="False"></asp:button>
			</div>
		</form>
	</body>
</html>
