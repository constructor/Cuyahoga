<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NodeEdit.aspx.cs" Inherits="Cuyahoga.Web.Admin.NodeEdit" MaintainScrollPositionOnPostback="true" %><!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
    <title>Cuyahoga Site Administration</title>
</head>
<body>
    <form id="Form1" method="post" runat="server">
        <script type="text/javascript"> 
        <!--
            function confirmDeleteNode() {
                return confirm("Are you sure you want to delete this node?");
            }
            function confirmSiteApplyTemplate() {
                return confirm("Apply this template to all pages in the current site?");
            }
				    // -->
        </script>
        <p>Manage the properties of the node (page). Use the buttons on the bottom of the page
            to save or delete the page or to add a new child node underneath this node.</p>
        <div class="group">
		    <asp:Button id="btnSave2" runat="server" text="Save" OnClick="btnSave_Click"></asp:Button>
		    <asp:Button id="btnCancel2" runat="server" causesvalidation="False" text="Cancel" OnClick="btnCancel_Click"></asp:Button>
		    <asp:Button id="btnNew2" runat="server" text="Add new child" OnClick="btnNew_Click"></asp:Button>
		    <asp:Button ID="btnDuplicate2" runat="server" Text="Duplicate" OnClick="btnDuplicate_Click"></asp:Button>
		    <asp:Button id="btnDelete2" runat="server" causesvalidation="False" text="Delete" OnClick="btnDelete_Click"></asp:Button>			    
		</div>
        <div class="group">
            <h4>General</h4>
            <table>
                <tr>
                    <td style="width: 100px">Node title</td>
                    <td>
                        <asp:TextBox ID="txtTitle" runat="server" Width="300px"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvTitle" runat="server" ErrorMessage="Title is required" Display="Dynamic" CssClass="validator" ControlToValidate="txtTitle" EnableClientScript="False"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td>Friendly url</td>
                    <td>
                        <asp:TextBox ID="txtShortDescription" runat="server" Width="300px" ToolTip="You can use this for 'nice' links ([shortdescription].aspx). Make sure it's unique!"></asp:TextBox>.aspx&nbsp;<asp:RegularExpressionValidator
                            ID="revShortDescription" runat="server" ErrorMessage="No spaces are allowed"
                            Display="Dynamic" CssClass="validator" ControlToValidate="txtShortDescription"
                            EnableClientScript="False" ValidationExpression="\S+"></asp:RegularExpressionValidator><asp:RequiredFieldValidator
                                ID="rfvShortDescription" runat="server" ErrorMessage="Short description is required"
                                Display="Dynamic" ControlToValidate="txtShortDescription" EnableClientScript="False"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px">SEO Node title</td>
                    <td>
                        <asp:TextBox ID="txtTitleSEO" runat="server" Width="300px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px">CSS Class</td>
                    <td>
                        <asp:TextBox ID="txtCSSClass" runat="server" Width="300px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>Parent node</td>
                    <td>
                        <asp:Label ID="lblParentNode" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>Position</td>
                    <td>
                        <asp:ImageButton ID="btnUp" OnClick="btnUp_Click" runat="server" ImageUrl="~/Admin/Images/ArrowUp-Over.gif" CausesValidation="False" AlternateText="Move up"></asp:ImageButton>
                        <asp:ImageButton ID="btnDown" OnClick="btnDown_Click" runat="server" ImageUrl="~/Admin/Images/ArrowDown-Over.gif" CausesValidation="False" AlternateText="Move down"></asp:ImageButton>
                        <asp:ImageButton ID="btnLeft" OnClick="btnLeft_Click" runat="server" ImageUrl="~/Admin/Images/ArrowLeft-Over.gif" CausesValidation="False" AlternateText="Move left"></asp:ImageButton>
                        <asp:ImageButton ID="btnRight" OnClick="btnRight_Click" runat="server" ImageUrl="~/Admin/Images/ArrowRight-Over.gif" CausesValidation="False" AlternateText="Move right"></asp:ImageButton>
                    </td>
                </tr>
                <tr>
                    <td>Culture</td>
                    <td>
                        <asp:DropDownList ID="ddlCultures" runat="server"></asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>Show in navigation</td>
                    <td>
                        <asp:CheckBox ID="chkShowInNavigation" runat="server"></asp:CheckBox>
                    </td>
                </tr>
                <tr>
                    <td>Meta description</td>
                    <td>
                        <asp:TextBox ID="txtMetaDescription" runat="server" MaxLength="500" TextMode="MultiLine" Width="400px" Height="35px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>Meta keywords</td>
                    <td>
                        <asp:TextBox ID="txtMetaKeywords" runat="server" MaxLength="500" TextMode="MultiLine" Width="400px" Height="35px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td>
                        <asp:CheckBox ID="chkLink" runat="server" OnCheckedChanged="chkLink_CheckedChanged" AutoPostBack="True" Text="The node is a link to an external url"> </asp:CheckBox>
                        <asp:Panel ID="pnlLink" runat="server" Visible="False">
                            <table>
                                <tr>
                                    <td style="width: 60px">Url</td>
                                    <td>
                                        <asp:TextBox ID="txtLinkUrl" runat="server" Width="400px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Target</td>
                                    <td>
                                        <asp:DropDownList ID="ddlLinkTarget" runat="server">
                                            <asp:ListItem Value="Self">Same window</asp:ListItem>
                                            <asp:ListItem Value="New">New window</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </td>
                </tr>
            </table>
        </div>
        <asp:Panel ID="pnlTemplate" runat="server" CssClass="group">
            <h4>Template</h4>
            <table>
                <tr>
                    <td style="width: 100px">
                    <asp:HyperLink ID="hplAddTemplate0" ToolTip="For more templates or a custom design simply contact us!" runat="server" NavigateUrl="~/Admin/TemplateEdit.aspx?TemplateId=-1">Add Template</asp:HyperLink>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlTemplates" OnSelectedIndexChanged="ddlTemplates_SelectedIndexChanged" runat="server" AutoPostBack="True"></asp:DropDownList>
                        <asp:Button ID="btnApplyToAll" runat="server" Text="Apply to all nodes" OnClick="btnApplyToAll_Click" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <asp:Panel ID="pnlMenus" runat="server" CssClass="group" Visible="False">
            <h4>Menus</h4>
            <em>You're editing a root node, so you can also attach on or more custom menu's.</em>
            <table class="tbl">
                <asp:Repeater ID="rptMenus" OnItemDataBound="rptMenus_ItemDataBound" runat="server">
                    <HeaderTemplate>
                        <tr>
                            <th>Menu</th>
                            <th>Placeholder</th>
                            <th>Actions</th>
                        </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td>
                                <%# DataBinder.Eval(Container.DataItem, "Name") %>
                            </td>
                            <td>
                                <%# DataBinder.Eval(Container.DataItem, "Placeholder") %>
                            </td>
                            <td>
                                <asp:HyperLink ID="hplEditMenu" runat="server">Edit</asp:HyperLink>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
            <asp:HyperLink ID="hplNewMenu" runat="server">Add menu</asp:HyperLink>
        </asp:Panel>
        <asp:Panel ID="pnlSections" runat="server" CssClass="group">
            <h4>Sections</h4>
            <table class="tbl">
                <asp:Repeater ID="rptSections" OnItemDataBound="rptSections_ItemDataBound" OnItemCommand="rptSections_ItemCommand" runat="server">
                    <HeaderTemplate>
                        <tr>
                            <th>Section title</th>
                            <th>Module type</th>
                            <th>Placeholder</th>
                            <th>Cache duration</th>
                            <th>Position</th>
                            <th>Actions</th>
                        </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td>
                                <%# DataBinder.Eval(Container.DataItem, "Title") %>
                            </td>
                            <td>
                                <%# DataBinder.Eval(Container.DataItem, "ModuleType.Name") %>
                            </td>
                            <td>
                                <%# DataBinder.Eval(Container.DataItem, "PlaceholderId") %>
                                <asp:Label ID="lblNotFound" CssClass="validator" Visible="False" runat="server">(not found in template!)</asp:Label>
                            </td>
                            <td style="text-align: right">
                                <%# DataBinder.Eval(Container.DataItem, "CacheDuration") %>
                            </td>
                            <td>
                                <asp:HyperLink ID="hplSectionUp" ImageUrl="~/Admin/Images/ArrowUp-Over.gif" Visible="False" EnableViewState="False" runat="server">Move up</asp:HyperLink>
                                <asp:HyperLink ID="hplSectionDown" ImageUrl="~/Admin/Images/ArrowDown-Over.gif" Visible="False" EnableViewState="False" runat="server">Move down</asp:HyperLink>
                            </td>
                            <td>
                                <asp:hyperlink id="hplAdmin" runat="server">Admin</asp:hyperlink>
                                <asp:HyperLink ID="hplEdit" runat="server">Edit</asp:HyperLink>
                                <asp:LinkButton ID="lbtDetach" runat="server" CausesValidation="False" CommandName="Detach" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'>Detach</asp:LinkButton>
                                <asp:LinkButton ID="lbtDelete" runat="server" CausesValidation="False" CommandName="Delete" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'>Delete</asp:LinkButton>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
            <asp:HyperLink ID="hplNewSection" runat="server" Visible="False">Add section</asp:HyperLink>
        </asp:Panel>
        <div class="group">
            <h4>Authorization</h4>
                <table class="tbl">
                    <asp:Repeater ID="rptRoles" runat="server">
                        <HeaderTemplate>
                            <tr>
                                <th>Role</th>
                                <th>View allowed</th>
                                <th>Edit allowed</th>
                            </tr>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td>
                                    <%# DataBinder.Eval(Container.DataItem, "Name") %>
                                </td>
                                <td style="text-align: center">
                                    <asp:CheckBox ID="chkViewAllowed" runat="server"></asp:CheckBox>
                                </td>
                                <td style="text-align: center">
                                    <asp:CheckBox ID="chkEditAllowed" runat="server"></asp:CheckBox>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </table>
                <br />
                <asp:CheckBox ID="chkPropagateToSections" runat="server" Text="Propagate security settings to sections"></asp:CheckBox>
                <asp:CheckBox ID="chkPropagateToChildNodes" runat="server" Text="Propagate security settings to child nodes"></asp:CheckBox>
                <br />
        </div>
        <div class="group">
            <asp:Button ID="btnSave" OnClick="btnSave_Click" runat="server" Text="Save"></asp:Button>
            <asp:Button ID="btnCancel" OnClick="btnCancel_Click" runat="server" CausesValidation="False" Text="Cancel"></asp:Button>
            <asp:Button ID="btnNew" OnClick="btnNew_Click" runat="server" Text="Add new child"></asp:Button>
            <asp:Button ID="btnDuplicate" OnClick="btnDuplicate_Click" runat="server" Text="Duplicate" /></asp:Button>
            <asp:Button ID="btnDelete" OnClick="btnDelete_Click" runat="server" CausesValidation="False" Text="Delete"></asp:Button>
        </div>
    </form>
</body>
</html>