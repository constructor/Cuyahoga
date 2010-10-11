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

            $(document).ready(function() {

                //Arrow Rollovers
                $(".tbl a[id*='Up'] img, table input[id*='Up']").hover(
                    function() {
                        $(this).attr('src', 'Images/arrow-up-over32x.png');
                    },
                    function() {
                        $(this).attr('src', 'Images/arrow-up-up32x.png');
                    }
                 );

                 $(".tbl a[id*='Down'] img, table input[id*='Down']").hover(
                    function() {
                        $(this).attr('src', 'Images/arrow-down-over32x.png');
                    },
                    function() {
                        $(this).attr('src', 'Images/arrow-down-up32x.png');
                    }
                 );

                 $("table input[id*='Left']").hover(
                    function() {
                        $(this).attr('src', 'Images/arrow-left-over32x.png');
                    },
                    function() {
                    $(this).attr('src', 'Images/arrow-left-up32x.png');
                    }
                 );

                 $("table input[id*='Right']").hover(
                    function() {
                        $(this).attr('src', 'Images/arrow-right-over32x.png');
                    },
                    function() {
                        $(this).attr('src', 'Images/arrow-right-up32x.png');
                    }
                 );


                //Validator is fired by 'Save' button
                var validator = $("#aspnetForm").validate({
                    success: function(label) {
                        label.remove(); //So the errorTab can be found (otherwise finds hidden labels)
                    },
                    invalidHandler: function(form, validator) {
                        var errors = validator.numberOfInvalids();
                        if (errors) {

                            var message = errors == 1
                                        ? 'There is 1 incomplete or invalid entry. It has been highlighted for you to correct.'
                                        : 'There are ' + errors + ' incomplete or invalid entries. They have been highlighted for you to correct.';

                            showMessage(message);
                        } else {
                            showMessage("");
                        }
                    }

                });

                function moveToErrorTab() {
                    var errorTab = $("#nodetabs").children("div").has(".error").find("label[class*='error']").first().parents("div").attr("id");
                    $("#nodetabs").tabs('select', '#' + errorTab);
                }

                $("input[id*='Save']").click(function() {
                    validator.form({ focusCleanup: true });
                    moveToErrorTab();
                });


                //Custom requiredUrl that allows http://localhost
                $.validator.addMethod("localUrl", function(value, element) {
                    return this.optional(element) || /^(http:\/\/|https:\/\/)?((([\w-]+\.)+[\w-]+)|localhost)(\/[\w- .\/?%&=]*)?/i.test(value);
                }, "required url is needed");

                //Custom Regex for nospace link
                $.validator.addMethod(
                        "noSpace",
                        function(value, element) {
                            return this.optional(element) || /^\S+$/.test(value);
                        },
                        "No spaces are allowed in your input."
                );


                //Validation rules
                var Title = $("input[name*='Title']").rules("add", {
                    required: true,
                    messages: {
                        required: "A Title is required"
                    }
                });

                var ShortDescription = $("input[name*='ShortDescription']").rules("add", {
                    noSpace: true,
                    required: true,
                    messages: {
                        noSpace: "No spaces are allowed",
                        required: "A friendly url is required"
                    }
                });


                //NodeEdit Tabs
                var theurl = window.location.search;
                if ($.getUrlVar(theurl, 'NodeId') == "-1") {

                    $('#nodetabs').tabs(
                        { cookie: {
                            //expires: 30,
                            name: 'nodetabscreate'
                        }
                        });

                    $("#nodetabs").tabs('select', '#tabs-1');
                }
                else {

                    $('#nodetabs').tabs(
                        { cookie: {
                            //expires: 30,
                            name: 'nodetabs'
                        }
                        });
                }

            });
            
		// -->
        </script>
        
        <p>Manage all the properties, styles, contents and permissions below.</p>

		<div id="nodetabs">
			<ul>
				<li><a href="#tabs-1">General</a></li>
				<li><a href="#tabs-2">Template</a></li>
				<li><a href="#tabs-3">Sections</a></li>
				<li><a href="#tabs-4">Authorisation</a></li>
			</ul>
			
			<div id="tabs-1">
		
                <table id="nodegeneral">
                    <tr>
                        <td id="nodetitle">Page title</td>
                        <td>
                            <asp:TextBox ID="txtTitle" runat="server" Width="300px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvTitle" runat="server" ErrorMessage="Title is required" 
                            Display="Dynamic" CssClass="validator" ControlToValidate="txtTitle" 
                            EnableClientScript="false"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>Friendly url</td>
                        <td>
                            <asp:TextBox ID="txtShortDescription" runat="server" Width="300px" ToolTip="You can use this for 'nice' links ([shortdescription].aspx). Make sure it's unique!"></asp:TextBox>.aspx&nbsp;
                            <asp:RegularExpressionValidator
                                    ID="revFriendlyURL" runat="server" ErrorMessage="No spaces are allowed"
                                    Display="Dynamic" CssClass="validator" ControlToValidate="txtShortDescription"
                                    EnableClientScript="False" ValidationExpression="\S+"></asp:RegularExpressionValidator>
                                <asp:RequiredFieldValidator
                                    ID="rfvFriendlyURL" runat="server" ErrorMessage="A friendly url is required"
                                    Display="Dynamic" ControlToValidate="txtShortDescription" 
                                    EnableClientScript="False"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>SEO Node title</td>
                        <td>
                            <asp:TextBox ID="txtTitleSEO" runat="server" Width="300px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>CSS Class</td>
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
                            <asp:ImageButton ID="btnUp" OnClick="btnUp_Click" runat="server" ImageUrl="~/Admin/Images/arrow-up-up32x.png" CausesValidation="False" ToolTip="Move up" AlternateText="Move up"></asp:ImageButton>
                            <asp:ImageButton ID="btnDown" OnClick="btnDown_Click" runat="server" ImageUrl="~/Admin/Images/arrow-down-up32x.png" CausesValidation="False" ToolTip="Move down" AlternateText="Move down"></asp:ImageButton>
                            <asp:ImageButton ID="btnLeft" OnClick="btnLeft_Click" runat="server" ImageUrl="~/Admin/Images/arrow-left-up32x.png" CausesValidation="False" ToolTip="Move left" AlternateText="Move left"></asp:ImageButton>
                            <asp:ImageButton ID="btnRight" OnClick="btnRight_Click" runat="server" ImageUrl="~/Admin/Images/arrow-right-up32x.png" CausesValidation="False" ToolTip="Move right" AlternateText="Move right"></asp:ImageButton>
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
                        <td>Link to external url</td>
                        <td>
                            <asp:CheckBox ID="chkLink" runat="server" OnCheckedChanged="chkLink_CheckedChanged" AutoPostBack="True" ToolTip="Link to external url"> </asp:CheckBox>
                            <asp:Panel ID="pnlLink" runat="server" Visible="False">
                                <table>
                                    <tr>
                                        <td>Url</td>
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
    		
		    <div id="tabs-2">
                
                <p>The template that is used by the selected node.</p>
                <p>
                <asp:DropDownList ID="ddlTemplates" OnSelectedIndexChanged="ddlTemplates_SelectedIndexChanged" runat="server" AutoPostBack="True"></asp:DropDownList>
                <asp:Button ID="btnApplyToAll" runat="server" Text="Apply to all nodes" OnClick="btnApplyToAll_Click" />
                </p>
                <p>
                <asp:HyperLink ID="hplAddTemplate0" runat="server" Text="Add new template" ToolTip="For more templates or a custom design simply contact us!" NavigateUrl="~/Admin/TemplateEdit.aspx?TemplateId=-1"></asp:HyperLink>
                </p>
                
            </div>
    		
		    <div id="tabs-3">
		    
                <asp:panel id="pnlSections" runat="server">
                    <table id="sections" class="tbl">
                        <asp:Repeater ID="rptSections" OnItemDataBound="rptSections_ItemDataBound" OnItemCommand="rptSections_ItemCommand" runat="server">
                            <HeaderTemplate>
                                <tr>
                                    <th id="title">Section title</th>
                                    <th id="type">Module type</th>
                                    <th id="placeholder">Placeholder</th>
                                    <th id="cache">Cache duration</th>
                                    <th id="position">Position</th>
                                    <th id="actions">Actions</th>
                                </tr>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <%# DataBinder.Eval(Container.DataItem, "Title") %>
                                    </td>
                                    <td class="center">
                                        <%# DataBinder.Eval(Container.DataItem, "ModuleType.Name") %>
                                    </td>
                                    <td class="center">
                                        <%# DataBinder.Eval(Container.DataItem, "PlaceholderId") %>
                                        <asp:Label ID="lblNotFound" CssClass="validator" Visible="False" runat="server">(not found in template!)</asp:Label>
                                    </td>
                                    <td class="center">
                                        <%# DataBinder.Eval(Container.DataItem, "CacheDuration") %>
                                    </td>
                                    <td class="center">
                                        <asp:HyperLink ID="hplSectionUp" ImageUrl="~/Admin/Images/arrow-up-up32x.png" Visible="False" EnableViewState="False" ToolTip="Move section up" runat="server">Move up</asp:HyperLink>
                                        <asp:HyperLink ID="hplSectionDown" ImageUrl="~/Admin/Images/arrow-down-up32x.png" Visible="False" EnableViewState="False" ToolTip="Move section down" runat="server">Move down</asp:HyperLink>
                                    </td>
                                    <td class="center">
                                        <asp:hyperlink id="hplAdmin" runat="server">Admin</asp:hyperlink>
                                        <asp:HyperLink ID="hplEdit" runat="server">Edit</asp:HyperLink>
                                        <asp:LinkButton ID="lbtDetach" runat="server" CausesValidation="False" CommandName="Detach" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'>Detach</asp:LinkButton>
                                        <asp:LinkButton ID="lbtDelete" runat="server" CausesValidation="False" CommandName="Delete" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'>Delete</asp:LinkButton>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </table>
                </asp:panel>
        		<asp:panel id="pnlNoSections" runat="server">
	                <p>There are no sections to display.</p>
	            </asp:panel>
                <asp:HyperLink ID="hplNewSection" runat="server" Visible="False">Add new section</asp:HyperLink>
            
            </div>
            
            <div id="tabs-4">
            
                <table id="roles" class="tbl">
                    <asp:Repeater ID="rptRoles" runat="server">
                        <HeaderTemplate>
                            <tr>
                                <th id="role">Role</th>
                                <th id="viewallowed">View allowed</th>
                                <th id="editallowed">Edit allowed</th>
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
                <asp:Label id="lblPropagateToSections" AssociatedControlId="chkPropagateToSections" Text="Propagate security settings to sections" runat="server" />
                <asp:CheckBox ID="chkPropagateToSections" runat="server"></asp:CheckBox>&nbsp;<asp:Label id="lblPropagateToChildNodes" AssociatedControlId="chkPropagateToChildNodes" Text="Propagate security settings to child nodes" runat="server" />
                <asp:CheckBox ID="chkPropagateToChildNodes" runat="server"></asp:CheckBox>
                <br />
                
            </div>
        
		</div>

        <asp:Button ID="btnSave" OnClick="btnSave_Click" runat="server" Text="Save"/>
        <asp:Button ID="btnCancel" OnClick="btnCancel_Click" runat="server" Text="Cancel"/>
        <asp:Button ID="btnNew" OnClick="btnNew_Click" runat="server" Text="New"/>
        <asp:Button ID="btnDuplicate" OnClick="btnDuplicate_Click" runat="server" Text="Duplicate" />
        <asp:Button ID="btnDelete" OnClick="btnDelete_Click" runat="server" Text="Delete"/>
        
    </form>
</body>
</html>