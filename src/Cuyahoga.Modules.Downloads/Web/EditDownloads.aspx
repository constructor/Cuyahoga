<%@ Page language="c#" Codebehind="EditDownloads.aspx.cs" AutoEventWireup="false" Inherits="Cuyahoga.Modules.Downloads.Web.EditDownloads" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head>
		<title>File Download Management</title>
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
			<div id="moduleadminpane">
				<h1>Downloads management</h1>

				<table class="tbl">
					<asp:repeater id="rptFiles" runat="server">
						<headertemplate>
							<tr>
								<th>Title</th>
								<th>Filename</th>
								<th>Size (bytes)</th>
								<th>Published by</th>
								<th>Number of downloads</th>
								<th>Date published</th>
								<th> </th>
							</tr>
						</headertemplate>
						<itemtemplate>
							<tr>
								<td><%# DataBinder.Eval(Container.DataItem, "Title") %></td>
								<td><%# DataBinder.Eval(Container.DataItem, "PhysicalFilePath")%></td>
								<td><%# DataBinder.Eval(Container.DataItem, "Length")%></td>
								<td><%# DataBinder.Eval(Container.DataItem, "PublishedBy.FullName") %></td>
								<td><%# DataBinder.Eval(Container.DataItem, "DownloadCount")%></td>
								<td><asp:literal id="litDateModified" runat="server"></asp:literal></td>
								<td><asp:hyperlink id="hplEdit" runat="server">Edit</asp:hyperlink></td>
							</tr>
						</itemtemplate>
					</asp:repeater>
				</table>

				<br/>
				<input id="btnNew" type="button" value="Add File" runat="server" name="btnNew"/>
			</div>
		</form>
	</body>
</html>
