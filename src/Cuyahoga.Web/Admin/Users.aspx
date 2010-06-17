<%@ Register TagPrefix="csc" Namespace="Cuyahoga.ServerControls" Assembly="Cuyahoga.ServerControls" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Users.aspx.cs" Inherits="Cuyahoga.Web.Admin.Users" %><!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
  <head>
		<title>Users</title>
  </head>
	<body>
		<form id="Form1" method="post" runat="server">
			<div class="group">
			    <h4>List all users</h4>
			    By Website <asp:DropDownList ID="ddlWebsite" AppendDataBoundItems="true" 
                    DataTextField="Name" DataValueField="iD" runat="server" AutoPostBack="True" 
                    onselectedindexchanged="DdlWebsiteSelectedIndexChanged">
                                <asp:ListItem Value="0">All</asp:ListItem>
                            </asp:DropDownList>
                <p>Or...</p>
				By Username <asp:textbox id="txtUsername" runat="server"></asp:textbox><asp:button id="btnFind" runat="server" OnClick="BtnFindClick" text="Find"></asp:button>
			</div>
			<asp:panel id="pnlResults" runat="server" cssclass="group">
				<h4>Search results</h4>
					<table class="tbl">
						<asp:repeater id="rptUsers" runat="server" OnItemDataBound="RptUsersItemDataBound">
							<headertemplate>
								<tr>
									<th>Username</th>
									<th>Firstname</th>
									<th>Lastname</th>
									<th>Email</th>
									<th>Website</th>
									<th>Last login date</th>
									<th>Last login from</th>
									<th></th>
								</tr>
							</headertemplate>
							<itemtemplate>
								<tr>
									<td><%# DataBinder.Eval(Container.DataItem, "UserName") %></td>
									<td><%# DataBinder.Eval(Container.DataItem, "FirstName") %></td>
									<td><%# DataBinder.Eval(Container.DataItem, "LastName") %></td>
									<td><%# DataBinder.Eval(Container.DataItem, "Email") %></td>
									<td><%# DataBinder.Eval(Container.DataItem, "Website") %></td>
									<td><asp:label id="lblLastLogin" runat="server"></asp:label></td>
									<td style="text-align:right"><%# DataBinder.Eval(Container.DataItem, "LastIp") %></td>
									<td>
										<asp:hyperlink id="hplEdit" runat="server">Edit</asp:hyperlink>
									</td>
								</tr>
							</itemtemplate>
						</asp:repeater>
					</table>
				<div class=pager>
					<csc:pager id="pgrUsers" runat="server" OnPageChanged="PgrUsersPageChanged" OnCacheEmpty="PgrUsersCacheEmpty" controltopage="rptUsers" cachedatasource="True" pagesize="10" CacheDuration="10"></csc:pager>
				</div>
			</asp:panel>
			<div>
				<asp:button id="btnNew" runat="server" OnClick="BtnNewClick" text="Add new user"></asp:button>
			</div>
		</form>
	</body>
</html>
