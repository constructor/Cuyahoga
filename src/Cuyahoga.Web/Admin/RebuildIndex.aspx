<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RebuildIndex.aspx.cs" Inherits="Cuyahoga.Web.Admin.RebuildIndex" %><!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head>
		<titleRebuildIndex</title>
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
			<p>
				<asp:label id="lblMessage" runat="server" visible="False">The index was successfully rebuilt.</asp:label>
			</p>
			<p>
				<asp:button id="btnRebuild" runat="server" text="Rebuild index" 
                    onclick="btnRebuild_Click"></asp:button>
			</p>
			<div id="pleasewait" style="DISPLAY: none">
				Please wait while the index is being rebuild...
			ild...
			</div>
		</form>
	</body>
</html>
