<%@ Page language="c#" Codebehind="EditArticle.aspx.cs" AutoEventWireup="True" Inherits="Cuyahoga.Modules.Articles.Web.EditArticle" ValidateRequest="false" %><%@ Register TagPrefix="cc1" Namespace="Cuyahoga.ServerControls" Assembly="Cuyahoga.ServerControls" %><%@ Register TagPrefix="fckeditorv2" Namespace="FredCK.FCKeditorV2" Assembly="FredCK.FCKeditorV2" %><!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title>Edit Article</title>
</head>
	<body>
		<form id="Form1" method="post" runat="server">
			<div id="moduleadminpane">
				<h1>Edit Article</h1>
				<div class="group">
					<h4>Article</h4>
                    <asp:Label ID="TitleLabel" AssociatedControlID="txtTitle" runat="server" Text="Title"></asp:Label>
                    <asp:textbox id="txtTitle" runat="server" width="70%"></asp:textbox>
                    <asp:requiredfieldvalidator id="rfvTitle" runat="server" cssclass="validator" display="Dynamic" errormessage="The title is required" enableclientscript="False" controltovalidate="txtTitle"></asp:requiredfieldvalidator>
                    <hr />
                    <asp:Label ID="SummaryLabel" AssociatedControlID="txtSummary" runat="server" Text="Summary"></asp:Label>
					<asp:textbox id="txtSummary" runat="server" width="70%" Rows="2" textmode="MultiLine"></asp:textbox> <asp:RegularExpressionValidator ID="revSummary" runat="server" ControlToValidate="txtSummary" Display="Dynamic" ErrorMessage="250 character limit"
                    ValidationExpression="[\s\S]{1,250}"></asp:RegularExpressionValidator>
                    <hr />
                    <fckeditorv2:fckeditor id="fckContent" runat="server" height="320px" width="100%"></fckeditorv2:fckeditor></td>
					<hr />
                    <asp:Label ID="CategoryLabel" AssociatedControlID="ddlCategory" runat="server" Text="Category"></asp:Label>	
					<asp:dropdownlist id="ddlCategory" runat="server" width="200px"></asp:dropdownlist>&nbsp;or enter a new category: <asp:textbox id="txtCategory" runat="server" width="200px"></asp:textbox>
				</div>
				<div class="group">
					<h4>Publishing</h4>
                    <p>Syndicate: <asp:checkbox id="chkSyndicate" runat="server" checked="True"></asp:checkbox></p>
                    <p>Date online: <cc1:calendar id="calDateOnline" runat="server" displaytime="True"></cc1:calendar></p>
                    <p>Date offline: <cc1:calendar id="calDateOffline" runat="server" displaytime="True"></cc1:calendar></p>
				</div>
				<p><asp:button id="btnSave" runat="server" text="Save" onclick="btnSave_Click"></asp:button><asp:button id="btnDelete" runat="server" text="Delete" visible="False" onclick="btnDelete_Click"></asp:button><input id="btnCancel" type="button" value="Cancel" runat="server"></p>
			</div>
			
			<script type="text/javascript">
			    /*Limit chars in multiline textarea*/
			    $(document).ready(function() {
			        $('input[id*=Title],textarea[id*=Summary]').keyup(function() {
			            var max = 250;
			            if ($(this).val().length > max) {
			                $(this).val($(this).val().substr(0, max));
			            }

			            /*$(this).parent().find('.charsRemaining').html('You have ' + (max - $(this).val().length) + ' characters remaining');*/
			        });
			    });
            </script>
            
		</form>
	</body>
</html>