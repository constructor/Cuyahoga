<%@ Control Language="c#" AutoEventWireup="True" Codebehind="EditProfile.ascx.cs" Inherits="Cuyahoga.Modules.User.EditProfile"  %>
<h1><%= GetText("EDITPROFILETITLE") %></h1>
<asp:panel id="pnlEdit" runat="server">
	<p>
	<asp:label id="lblError" runat="server" visible="False" cssclass="error" enableviewstate="False"></asp:label>
	<p/>
	<fieldset>
	    <legend>Edit Profile Information</legend>
	    <div class="padding">
	        <p><%= GetText("EDITPROFILEINFO") %></p>
		    <asp:label id="lbllblUsername" AssociatedControlID="lblUsername" runat="server"><%= GetText("USERNAME") %></asp:label>
		    <asp:label id="lblUsername" CssClass="labeltext" runat="server"></asp:label>
            <br /><br />
            <asp:Label ID="lblFirstname" AssociatedControlID="txtFirstname" runat="server"><%= GetText("FIRSTNAME") %></asp:Label>
		    <asp:TextBox id="txtFirstname" runat="server" maxlength="100"></asp:TextBox>
            <br />
            <asp:Label ID="lblLastname" AssociatedControlID="txtLastname" runat="server"><%= GetText("LASTNAME") %></asp:Label>
		    <asp:TextBox id="txtLastname" runat="server" maxlength="100"></asp:TextBox>
            <br />
            <asp:Label ID="lblEmail" AssociatedControlID="txtEmail" runat="server"><%= GetText("EMAIL") %></asp:Label>
		    <asp:TextBox id="txtEmail" runat="server" maxlength="100"></asp:TextBox>
		    <asp:RequiredFieldValidator id="rfvEmail" runat="server" cssclass="error" errormessage='<%# GetText("EMAILREQUIRED") %>' display="Dynamic" controltovalidate="txtEmail" enableclientscript="False"></asp:RequiredFieldValidator>
		    <asp:RegularExpressionValidator id="revEmail" runat="server" cssclass="error" errormessage='<%# GetText("EMAILINVALID") %>' display="Dynamic" controltovalidate="txtEmail" enableclientscript="False" validationexpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>

            <asp:Label ID="lblWebsite" Visible="false" AssociatedControlID="txtWebsite" runat="server"><%= GetText("WEBSITE") %></asp:Label>
		    <asp:textbox id="txtWebsite" Visible="false" runat="server" maxlength="100" width="200px"></asp:textbox>
            
            <hr />
            <asp:Label ID="lblddlTimeZone" AssociatedControlID="ddlTimeZone" runat="server"><%= GetText("TIMEZONE") %></asp:Label>
		    <asp:dropdownlist id="ddlTimeZone" CssClass="selectlarge" runat="server"></asp:dropdownlist>
            <hr />
		    <asp:Button id="btnSave" runat="server" text='<%# GetText("SAVEPROFILE") %>' onclick="btnSave_Click"></asp:Button>
	        <br/>
        </div>
    </fieldset>
	<fieldset>
        <legend>Change Password</legend>
        <div class="padding">
            <p><%= GetText("EDITPROFILEINFO") %></p>
            <asp:Label ID="lblCurrentPassword" AssociatedControlID="txtCurrentPassword" runat="server"><%= GetText("CURRENTPASSWORD") %></asp:Label>
		    <asp:TextBox id="txtCurrentPassword" runat="server" maxlength="100" textmode="Password"></asp:TextBox>
            <br />
            <asp:Label ID="lblNewPassword" AssociatedControlID="txtNewPassword" runat="server"><%= GetText("NEWPASSWORD") %></asp:Label> 
		    <asp:TextBox id="txtNewPassword" runat="server" maxlength="100" textmode="Password"></asp:TextBox>
            <br />
            <asp:Label ID="lblNewPasswordConfirmation" AssociatedControlID="txtNewPasswordConfirmation" runat="server"><%= GetText("NEWPASSWORDCONFIRMATION") %></asp:Label>
		    <asp:TextBox id="txtNewPasswordConfirmation" runat="server" maxlength="100" textmode="Password"></asp:TextBox>
            <br />
		    <asp:Button id="btnSavePassword" runat="server" text='<%# GetText("SAVEPASSWORD") %>' onclick="btnSavePassword_Click"></asp:Button>
        </div>
    </fieldset>
</asp:panel>
<asp:panel id="pnlInfo" runat="server" visible="False">
	<asp:label id="lblInfo" runat="server"></asp:label>
</asp:panel>