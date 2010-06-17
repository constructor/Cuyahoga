<%@ Page language="c#" Codebehind="SiteAliasEdit.aspx.cs" AutoEventWireup="false" Inherits="Cuyahoga.Web.Admin.SiteAliasEdit" %><!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
  <head>
    <title>SiteAliasEdit</title>
  </head>
  <body>
	
    <form id="Form1" method="post" runat="server">
			<div class="group">
				<h4>General</h4>
				<table>
					<tr>
						<td style="width: 100px">Url (incl. http://)</td>
						<td>
						    <asp:textbox id="txtUrl" runat="server" width="300px" maxlength="100"></asp:textbox>
						    <asp:requiredfieldvalidator id="rfvName" runat="server" enableclientscript="False" controltovalidate="txtUrl"
								    cssclass="validator" display="Dynamic">Url is required</asp:requiredfieldvalidator>
						</td>
					</tr>
					<tr>
						<td>Entry node</td>
						<td>
						    <asp:DropDownList id=ddlEntryNodes runat="server"></asp:DropDownList>
						</td>
					</tr>
				</table>
			</div>
			<div>
				<asp:button id="btnSave" runat="server" text="Save" onclick="BtnSave_Click"></asp:button>
				<asp:button id="btnCancel" runat="server" text="Cancel" causesvalidation="False" onclick="BtnCancel_Click"></asp:button>
				<asp:button id="btnDelete" runat="server" text="Delete" causesvalidation="False" onclick="BtnDelete_Click"></asp:button>
			</div>
     </form>
	
  </body>
</html>
