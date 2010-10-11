<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserEdit.aspx.cs" Inherits="Cuyahoga.Web.Admin.UserEdit" %><!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head>
		<title>UserEdit</title>
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
			<script type="text/javascript"> <!--
				function confirmDeleteUser(userId)
				{
					if (confirm("Are you sure you want to delete this user?"))
						document.location.href = "UserEdit.aspx?UserId=" + userId + "&Action=Delete";
                }

                $(document).ready(function() {
                    // Tabs
                    $("#adminTabsNode").tabs({
                        cookie: {
                            // store cookie for a day, without, it would be a session cookie
                            expires: 1,
                            name: 'useredittabs'
                        }
                    });
                });
				// -->
			</script>
			
			<p>Edit the details and roles of the selected user.</p>
			
			<div id="adminTabsNode">
		        <ul>
			        <li><a href="#tabs-1">User Details</a></li>
			        <li><a href="#tabs-2">User Roles</a></li>
		        </ul>
		
		        <div id="tabs-1">
    			
				    <table id="userdetails">
					    <tr>
					        <td>User assigned site(s)</td>
						    <td>
                                <asp:CheckBoxList ID="cblUserSitesList" DataTextField="SiteUrl" DataValueField="Id" runat="server" CssClass="usersites">
                                </asp:CheckBoxList>
                            </td>
					    </tr>
					    <tr>
						    <td>&nbsp;</td>
						    <td>&nbsp;</td>
					    </tr>
					    <tr>
						    <td>Username</td>
						    <td><asp:textbox id="txtUsername" runat="server" width="200px"></asp:textbox><asp:label id="lblUsername" runat="server" visible="False"></asp:label><asp:requiredfieldvalidator id="rfvUsername" runat="server" errormessage="Username is required" cssclass="validator"
								    display="Dynamic" enableclientscript="False" controltovalidate="txtUsername"></asp:requiredfieldvalidator></td>
					    </tr>
					    <tr>
						    <td>Firstname</td>
						    <td><asp:textbox id="txtFirstname" runat="server" width="200px"></asp:textbox></td>
					    </tr>
					    <tr>
						    <td>Lastname</td>
						    <td><asp:textbox id="txtLastname" runat="server" width="200px"></asp:textbox></td>
					    </tr>
					    <tr>
						    <td>Email</td>
						    <td><asp:textbox id="txtEmail" runat="server" width="200px"></asp:textbox><asp:requiredfieldvalidator id="rfvEmail" runat="server" controltovalidate="txtEmail" enableclientscript="False" display="Dynamic" cssclass="validator" errormessage="Email is required"></asp:requiredfieldvalidator><asp:RegularExpressionValidator ID="rexvWebmasterEmail" runat="server" cssclass="validator" display="Dynamic" controltovalidate="txtEmail" ValidationExpression="^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$" ErrorMessage="A valid email address is required" EnableClientScript="true"></asp:RegularExpressionValidator></td>
					    </tr>
                        <tr>
						    <td>Website</td>
						    <td><asp:textbox id="txtWebsite" runat="server" width="200px"></asp:textbox></td>
					    </tr>
					    <tr>
						    <td>Active</td>
						    <td><asp:checkbox id="chkActive" runat="server"></asp:checkbox></td>
					    </tr>
					    <tr>
						    <td>Timezone</td>
						    <td><asp:dropdownlist id="ddlTimeZone" runat="server"></asp:dropdownlist></td>
					    </tr>
					    <tr>
						    <td>Password</td>
						    <td><asp:textbox id="txtPassword1" runat="server" width="200px" textmode="Password"></asp:textbox></td>
					    </tr>
					    <tr>
						    <td>Confirm Password</td>
						    <td><asp:textbox id="txtPassword2" runat="server" width="200px" textmode="Password"></asp:textbox><asp:comparevalidator id="covPassword" runat="server" controltovalidate="txtPassword1" enableclientscript="False"
								    display="Dynamic" cssclass="validator" errormessage="Both passwords must be the same" controltocompare="txtPassword2"></asp:comparevalidator></td>
					    </tr>
				    </table>
    			
			    </div>
			
			    <div id="tabs-2">

			        <table id="roles" class="tbl">
				        <asp:repeater id="rptRoles" runat="server">
					        <headertemplate>
						        <tr>
							        <th>Role</th>
							        <th>
							        </th>
						        </tr>
					        </headertemplate>
					        <itemtemplate>
						        <tr>
							        <td><%# DataBinder.Eval(Container.DataItem, "Name") %></td>
							        <td style="text-align:center">
								        <asp:checkbox id="chkRole" runat="server"></asp:checkbox>
							        </td>
						        </tr>
					        </itemtemplate>
				        </asp:repeater>
		            </table>
    			
			    </div>
			
			</div>
			
			
			<div>
			<asp:button id="btnSave" runat="server" OnClick="BtnSaveClick" text="Save"></asp:button>
			<asp:button id="btnCancel" runat="server" OnClick="BtnCancelClick" text="Cancel" causesvalidation="False"></asp:button>
			<asp:button id="btnDelete" runat="server" OnClick="BtnDeleteClick" text="Delete"></asp:button>
			<asp:button id="btnBack" runat="server" OnClick="BtnBackClick" text="Back" causesvalidation="False"></asp:button>
			</div>
		</form>
	</body>
</html>
