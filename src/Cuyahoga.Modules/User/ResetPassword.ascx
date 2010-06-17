<%@ Control Language="c#" AutoEventWireup="True" Codebehind="ResetPassword.ascx.cs" Inherits="Cuyahoga.Modules.User.ResetPassword"  %>
<h1><%= GetText("RESETTITLE") %></h1>
<asp:panel id="pnlReset" runat="server">
    <p>
    <asp:label id="lblError" runat="server" cssclass="error" enableviewstate="False" visible="False"></asp:label>
    </p>
	<fieldset>
	    <legend>Reset Password</legend>
	    <div class="padding">
	        <p><%= GetText("RESETINFO") %></p>
            <asp:Label ID="lblUsername" AssociatedControlID="txtUsername" runat="server"><%= GetText("USERNAME") %></asp:Label>
	        <asp:textbox id="txtUsername" runat="server" maxlength="50" width="200px"></asp:textbox>
	        <asp:requiredfieldvalidator id=rfvUsername runat="server" cssclass="error" enableclientscript="False" controltovalidate="txtUsername" display="Dynamic" errormessage='<%# GetText("USERNAMEREQUIRED") %>'></asp:requiredfieldvalidator>
		    <br />
            <asp:Label ID="lblEmail" AssociatedControlID="txtEmail" runat="server"><%= GetText("EMAIL") %></asp:Label>
		    <asp:textbox id="txtEmail" runat="server" maxlength="100" width="200px"></asp:textbox><asp:requiredfieldvalidator id=rfvEmail runat="server" cssclass="error" enableclientscript="False" controltovalidate="txtEmail" display="Dynamic" errormessage='<%# GetText("EMAILREQUIRED") %>'></asp:requiredfieldvalidator>
		    <asp:regularexpressionvalidator id="revEmail" runat="server" cssclass="error" enableclientscript="False" controltovalidate="txtEmail" display="Dynamic" errormessage='<%# GetText("EMAILINVALID") %>' validationexpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:regularexpressionvalidator>
		    <br />
		    <asp:button id=btnReset runat="server" text='<%# GetText("RESET") %>' onclick="BtnReset_Click"></asp:button>
        </div>
    </fieldset>
</asp:panel>
<asp:panel id="pnlConfirmation" runat="server" visible="False">
    <asp:label id="lblConfirmation" runat="server"></asp:label>
</asp:panel>
