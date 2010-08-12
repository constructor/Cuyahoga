<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TemplateSection.aspx.cs" Inherits="Cuyahoga.Web.Admin.TemplateSection" %><!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head>
		<title>TemplateSection</title>
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
			<div class="group">
				<h4>Template</h4>
				<table>
					<tr>
						<td style="width:130px">Template</td>
						<td>
							<asp:label id="lblTemplate" runat="server"></asp:label></td>
					</tr>
					<tr>
						<td>Placeholder</td>
						<td>
							<asp:label id="lblPlaceholder" runat="server"></asp:label></td>
					</tr>
				</table>
			</div>
			<br/>
			<div class="group">
				<h4>Section</h4>
				<table>
					<tr>
						<td style="width:130px">Available sections</td>
						<td>
							<asp:dropdownlist id="ddlSections" runat="server"></asp:dropdownlist></td>
					</tr>
				</table>
			</div>
			<br/>
			<asp:button id="btnAttach" runat="server" OnClick="BtnAttachClick" text="Attach selected section"></asp:button>
			<asp:button id="btnBack" runat="server" OnClick="BtnBackClick" text="Back"></asp:button>
		</form>
	</body>
</html>

