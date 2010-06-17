<%@ Register TagPrefix="cc1" Namespace="Cuyahoga.ServerControls" Assembly="Cuyahoga.ServerControls" %><%@ Page language="c#" Codebehind="AdminArticles.aspx.cs" AutoEventWireup="True" Inherits="Cuyahoga.Modules.Articles.Web.AdminArticles" %><!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title>Articles</title>
</head>
	<body>
		<form id="Form1" method="post" runat="server">
			<div id="moduleadminpane">
				<h1>Article management</h1>
                    <div id="catagoryfilers" class="group">
                    <h4>Article Filtering</h4>
                    
                    </div>
				    <div id="categories" class="group">
				        <h4>Articles</h4>
					    <table class="tbl">
						    <asp:repeater id="rptArticles" runat="server">
							    <headertemplate>
								    <tr>
									    <th>Title</th>
									    <th>Category</th>
									    <th>Date online</th>
									    <th>Date offline</th>
									    <th>Created by</th>
									    <th>Modified by</th>
									    <th> </th>
								    </tr>
							    </headertemplate>
							    <itemtemplate>
								    <tr>
									    <td><%# DataBinder.Eval(Container.DataItem, "Title") %></td>
									    <td><%# GetArtacleCatagories(DataBinder.Eval(Container.DataItem, "Id").ToString()) %></td>
									    <td>
										    <asp:literal id="litDateOnline" runat="server"></asp:literal></td>
									    <td>
										    <asp:literal id="litDateOffline" runat="server"></asp:literal></td>
									    <td><%# DataBinder.Eval(Container.DataItem, "CreatedBy.Username") %></td>
									    <td><%# DataBinder.Eval(Container.DataItem, "ModifiedBy.Username") %></td>
									    <td>
										    <asp:hyperlink id="hplEdit" runat="server">Edit</asp:hyperlink>
										    <asp:hyperlink id="hplComments" runat="server">Comments</asp:hyperlink>
									    </td>
								    </tr>
							    </itemtemplate>
						    </asp:repeater>
					    </table>
					</div>
                <div id="paging" class="group">
				    <div class="pager">
					    <cc1:pager id="pgrArticles" runat="server" controltopage="rptArticles" cachedatasource="True" pagesize="10" cacheduration="30" cachevarybyparams="SectionId" oncacheempty="pgrArticles_CacheEmpty"></cc1:pager>
				    </div>
				</div>
				<br/>
				<input id="btnNew" type="button" value="New Article" runat="server">
			</div>
		</form>
	</body>
</html>