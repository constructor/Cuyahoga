<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Sections.aspx.cs" Inherits="Cuyahoga.Web.Admin.Sections" %><!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head>
		<title>Sections</title>
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
			<p>Manage sections that are not related to a node. This can be detached sections or 
				sections that are connected to&nbsp;one or more&nbsp;templates.</p>
			<div class="group">
			    <table class="tbl">
				    <asp:repeater id="rptSections" OnItemDataBound="rptSections_ItemDataBound" OnItemCommand="rptSections_ItemCommand" runat="server">
					    <headertemplate>
						    <tr>
							    <th>Section name</th>
							    <th>Module type</th>
							    <th>Attached to template(s)</th>
							    <th>Actions</th>
						    </tr>
					    </headertemplate>
					    <itemtemplate>
						    <tr>
							    <td><%# DataBinder.Eval(Container.DataItem, "Title") %></td>
							    <td><%# DataBinder.Eval(Container.DataItem, "ModuleType.Name") %></td>
							    <td>
								    <asp:literal id="litTemplates" runat="server" />
							    </td>
							    <td style="white-space:nowrap">
								    <asp:hyperlink id="hplEdit" runat="server">Edit</asp:hyperlink>
								    <asp:linkbutton id="lbtDelete" runat="server" causesvalidation="False" commandname="Delete" commandargument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'>Delete</asp:linkbutton>
								    <asp:hyperlink id="hplAttachTemplate" runat="server">Attach to template</asp:hyperlink>
								    <asp:hyperlink id="hplAttachNode" runat="server">Attach to node</asp:hyperlink>
							    </td>
						    </tr>
					    </itemtemplate>
				    </asp:repeater>
			    </table>
			</div>
			<div><asp:button id="btnNew" runat="server" OnClick="btnNew_Click" text="Add new section"></asp:button></div>
		</form>
	</body>
</html>
