<%@ Register TagPrefix="cc1" Namespace="Cuyahoga.ServerControls" Assembly="Cuyahoga.ServerControls" %><%@ Page language="c#" Codebehind="EditFile.aspx.cs" AutoEventWireup="false" Inherits="Cuyahoga.Modules.Downloads.Web.EditFile" %><!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head>
		<title>File Download Management</title>
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
			<div id="moduleadminpane">
				<h1>Edit File</h1>
				<div class="group">
					<h3>File properties</h3>
					<table>
						<tr>
							<td style="width: 100px">File</td>
							<td><asp:panel id="pnlFileName" runat="server" visible="True">
									<asp:textbox id="txtFile" runat="server" width="300px" enabled="False"></asp:textbox>
									<asp:requiredfieldvalidator id="rfvFile" runat="server" errormessage="File is required" display="Dynamic" cssclass="validator" controltovalidate="txtFile" enableclientscript="True"></asp:requiredfieldvalidator>
								</asp:panel><input id="filUpload" style="width: 300px" type="file" runat="server">
								<asp:button id="btnUpload" runat="server" causesvalidation="False" text="Upload"></asp:button>
							</td>
						</tr>
						<tr>
							<td style="width: 100px">Title (optional)</td>
							<td><asp:textbox id="txtTitle" runat="server" width="300px"></asp:textbox></td>
						</tr>
						<tr>
							<td style="width: 100px">Date published</td>
							<td><cc1:calendar id="calDatePublished" runat="server" displaytime="True"></cc1:calendar>
								<asp:requiredfieldvalidator id="rfvDatePublished" runat="server" errormessage="Date published is required" display="Dynamic"
									cssclass="validator" controltovalidate="calDatePublished" enableclientscript="False"></asp:requiredfieldvalidator>
						    </td>
						</tr>
					</table>
				</div>
				<div class="group">
					<h3>Allowed roles for download</h3>
					<table class="tbl">
						<asp:repeater id="rptRoles" runat="server">
							<headertemplate>
								<tr>
									<th>
										Role</th>
									<th>
									</th>
								</tr>
							</headertemplate>
							<itemtemplate>
								<tr>
									<td><%# DataBinder.Eval(Container.DataItem, "Role.Name") %></td>
									<td style="text-align:center">
										<asp:checkbox id="chkRole" runat="server"></asp:checkbox></td>
								</tr>
							</itemtemplate>
						</asp:repeater>
                    </table>
				</div>
				<p><asp:button id="btnSave" runat="server" text="Save"></asp:button><asp:button id="btnDelete" runat="server" visible="False" causesvalidation="False" text="Delete"></asp:button><asp:button id="btnCancel" runat="server" causesvalidation="False" text="Cancel"></asp:button></p>
			</div>
		</form>
	</body>
</html>
