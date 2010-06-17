<%@ Control Language="c#" AutoEventWireup="True" Codebehind="Register.ascx.cs" Inherits="Cuyahoga.Modules.User.Register"  %>
<h1><%= GetText("REGISTERTITLE") %></h1>
<asp:panel id="pnlRegister" runat="server">
    <p>
    <asp:label id="lblError" runat="server" visible="False" cssclass="error" enableviewstate="False"></asp:label>
    </p>
	<fieldset>
	    <legend>Register</legend>
	    <div class="padding">
	        <p><%= GetText("REGISTERINFO") %></p>
            <asp:Label ID="lblUsername" AssociatedControlID="txtUsername" runat="server"><%= GetText("USERNAME") %></asp:Label>
	        <asp:textbox id="txtUsername" runat="server" maxlength="50" width="200px"></asp:textbox>
	        <asp:requiredfieldvalidator id="rfvUsername" runat="server" ErrorMessage='<%# GetText("USERNAMEREQUIRED") %>' display="Dynamic" cssclass="error" controltovalidate="txtUsername" enableclientscript="False"></asp:requiredfieldvalidator>
		    <br />
            <asp:Label ID="lblEmail" AssociatedControlID="txtEmail" runat="server"><%= GetText("EMAIL") %></asp:Label>
		    <asp:textbox id="txtEmail" runat="server" maxlength="100" width="200px"></asp:textbox>
		    <asp:requiredfieldvalidator id="rfvEmail" runat="server" ErrorMessage='<%# GetText("EMAILREQUIRED") %>' display="Dynamic" cssclass="error" controltovalidate="txtEmail" enableclientscript="False"></asp:requiredfieldvalidator>
		    <asp:regularexpressionvalidator id="revEmail" runat="server" ErrorMessage='<%# GetText("EMAILINVALID") %>' display="Dynamic" cssclass="error" controltovalidate="txtEmail" enableclientscript="False" validationexpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:regularexpressionvalidator>
		    <br />
		    <asp:button id="btnRegister" runat="server" Text='<%# GetText("REGISTER") %>' onclick="btnRegister_Click"></asp:button></td></tr></table>
        </div>
    </fieldset>
</asp:panel>
<asp:panel id="pnlConfirmation" runat="server" visible="False">
	<fieldset>
	    <legend>Register</legend>
            <div class="padding">
	            <asp:label id="lblConfirmation" runat="server"></asp:label>
	        </div>
    </fieldset>
</asp:panel>