<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SectionAttachNode.aspx.cs" Inherits="Cuyahoga.Web.Admin.SectionAttachNode" %><!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head>
		<title>SectionAttach</title>
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
			<div class="group">
				<h4>Section</h4>
				<table>
					<tr>
						<td style="width:100px">Section</td>
						<td>
							<asp:label id="lblSection" runat="server"></asp:label>
						</td>
					</tr>
					<tr>
						<td>Module type</td>
						<td>
							<asp:label id="lblModuleType" runat="server"></asp:label>
						</td>
					</tr>
				</table>
			</div>
			<div class="group">
				<h4>Attach to</h4>
				<table>
					<tr>
						<td style="width:100px">Site</td>
						<td><asp:dropdownlist id="ddlSites" runat="server" OnSelectedIndexChanged="DdlSitesSelectedIndexChanged" autopostback="true"></asp:dropdownlist></td>
					</tr>
					<tr>
						<td>Node</td>
						<td><asp:listbox id="lbxAvailableNodes" runat="server" OnSelectedIndexChanged="LbxAvailableNodesSelectedIndexChanged" height="256px" width="440px" autopostback="true"
								visible="False"></asp:listbox></td>
					</tr>
					<tr>
						<td>Placeholder</td>
						<td><asp:dropdownlist id="ddlPlaceholder" runat="server" visible="False"></asp:dropdownlist>
							<asp:hyperlink id="hplLookup" runat="server" visible="False">Lookup</asp:hyperlink></td>
					</tr>
				</table>
			</div>
			<div>
				<asp:button id="btnSave" runat="server" OnClick="BtnSaveClick" text="Save" enabled="False"></asp:button>
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
