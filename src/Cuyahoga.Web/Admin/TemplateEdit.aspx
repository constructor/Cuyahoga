<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TemplateEdit.aspx.cs" Inherits="Cuyahoga.Web.Admin.TemplateEdit" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>TemplateEdit</title>
</head>
<body>
    <form id="Form1" method="post" runat="server">
        <script type="text/javascript"> 
        <!--
            $(document).ready(function() {

                //Validator is fired by 'Save' button
                var validator = $("#aspnetForm").validate({

                    onsubmit: false,
                    success: function(label) {
                        label.remove(); //So the errorTab can be found (otherwise finds hidden labels)
                    },
                    invalidHandler: function(form, validator) {
                        showErrors();
                    }

                });

                function showErrors() {
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

                function moveToErrorTab() {
                    var errorTab = $("#templatetabs").children("div").has(".error").find("label[class*='error']").first().parents("div").attr("id");
                    $("#templatetabs").tabs('select', '#' + errorTab);
                }


                //Custom Regex for nospace link
                $.validator.addMethod(
                        "noSpace",
                        function(value, element) {
                            return this.optional(element) || /^\S+$/.test(value);
                        },
                        "No spaces are allowed in your input."
                );

                //Custom file type validator
                $.validator.addMethod("allowedFiles", function(val, element) {
                    var ext = $(element).val().split('.').pop().toLowerCase();
                    var allow = new Array('zip');
                    if (jQuery.inArray(ext, allow) == -1) {
                        return false
                    } else {
                        return true
                    }
                }, "File type error");


                //Validation rules
                var vName = $("input[name*='Name']");
                vName.rules("add", {
                    required: true,
                    messages: {
                        required: "A template name is required"
                    }
                });

                var vbasePath = $("input[name*='BasePath']");
                vbasePath.rules("add", {
                    required: true,
                    messages: {
                        required: "A verified template path is required"
                    }
                });

                var vTemplateControls = $("select[name*='TemplateControls']");
                vTemplateControls.rules("add", {
                    required: true,
                    messages: {
                        required: "A template Control must be selected"
                    }
                });

                var vCss = $("select[name*='Css']");
                vCss.rules("add", {
                    required: true,
                    messages: {
                        required: "A template default CSS must be selected"
                    }
                });

                var vUploadTemplate = $("input[id*='UploadTemplate']");
                vUploadTemplate.rules("add", {
                    required: true,
                    allowedFiles: true,
                    messages: {
                        required: "Template pack required",
                        allowedFiles: "Only a .zip file is allowed"
                    }
                });


                //Create template button
                $("input[id*='Save']").click(function() {
                    validator.resetForm();
                    $('.error').remove();
                    isValid = validator.element(vName); //if validations are all done with && (together) stops on first invalid 
                    isValid = validator.element(vbasePath);
                    isValid = validator.element(vTemplateControls);
                    isValid = validator.element(vCss);
                    moveToErrorTab();
                    showErrors();
                    return (isValid); //Return after moving the tab focus
                });

                //Upload button
                $("input[id*='btnUpload']").click(function() {
                    validator.resetForm();
                    $('.error').remove();
                    isValid = validator.element(vUploadTemplate);
                    moveToErrorTab();
                    showErrors();
                    return (isValid); //Return after moving the tab focus
                });


                //NodeEdit Tabs
                var theurl = window.location.search;
                if ($.getUrlVar(theurl, 'TemplateId') == "-1") {

                    $('#templatetabs').tabs(
                        { cookie: {
                            //expires: 30,
                            name: 'templatetabscreate'
                        }
                        });

                    $("#templatetabs").tabs('select', '#tabs-1');
                }
                else {

                    $('#templatetabs').tabs(
                        { cookie: {
                            //expires: 30,
                            name: 'templatetabs'
                        }
                        });
                }

            });
            
	    // -->
        </script>
        
        <p>Manage existing and upload new templates.</p>
        
	    <div id="templatetabs">
	        <ul>
		        <li><a href="#tabs-1">Template</a></li>
		        <li><a href="#tabs-2">Placeholder Sections</a></li>
		        <li><a href="#tabs-3">Upload Template Pack</a></li>
	        </ul>
    	
	        <div id="tabs-1">
            
                <table id="templates">
                    <tr>
                        <td id="name">Name</td>
                        <td>
                            <asp:TextBox ID="txtName" runat="server" Width="200px"></asp:TextBox>
                            <%--<asp:RequiredFieldValidator ID="rfvName" runat="server" ErrorMessage="Name is required" CssClass="validator" Display="Dynamic" EnableClientScript="False" ControlToValidate="txtName"></asp:RequiredFieldValidator>--%>
                        </td>
                    </tr>
                    <tr>
                        <td>Site (the template belongs to)</td>
                        <td>
                            <asp:DropDownList ID="ddlSites" runat="server" AppendDataBoundItems="true" DataValueField="Id"
                                DataTextField="Name" AutoPostBack="True" onselectedindexchanged="DdlSitesSelectedIndexChanged">
                                <asp:ListItem>None</asp:ListItem>
                            </asp:DropDownList>
                            <asp:Button ID="btnCopyToSite" runat="server" Text="Copy to site" CssClass="cancel" OnClick="CopyTemplateToSite"></asp:Button>
                        </td>
                    </tr>
                    <tr>
                        <td>Template path (from root) /</td>
                        <td>
                            <asp:TextBox ID="txtBasePath" runat="server" Width="200px"></asp:TextBox>
                            <asp:Button ID="btnVerifyBasePath" runat="server" Text="Verify" CssClass="cancel" OnClick="BtnVerifyBasePathClick"></asp:Button>
                            <%--<asp:RequiredFieldValidator ID="rfvBasePath" runat="server" ErrorMessage="Base path is required" CssClass="validator" EnableClientScript="True" ControlToValidate="txtBasePath" ValidationGroup="basepath"></asp:RequiredFieldValidator>--%>
                        </td>
                    </tr>
                    <tr>
                        <td>Template control</td>
                        <td>
                            <asp:DropDownList ID="ddlTemplateControls" runat="server"></asp:DropDownList>
                            <asp:Label ID="lblTemplateControlWarning" runat="server" CssClass="validator" Visible="False" EnableViewState="False"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>Css</td>
                        <td>
                            <asp:DropDownList ID="ddlCss" runat="server"></asp:DropDownList>
                            <asp:Label ID="lblCssWarning" runat="server" CssClass="validator" Visible="False" EnableViewState="False"></asp:Label>
                        </td>
                    </tr>
                </table>
            
            </div>
            
            <div id="tabs-2">
            
                <asp:Panel ID="pnlPlaceholders" runat="server" Visible="False">
                     <table id="placeholders" class="tbl">
                        <thead>
                            <tr>
                                <th id="placeholder">Placeholder</th>
                                <th id="attached">Attached section</th>
                                <th id="attach">Attach/Detach</th>
                            </tr>
                        </thead>
                        <tbody>
                        <asp:Repeater ID="rptPlaceholders" runat="server" OnItemDataBound="RptPlaceholdersItemDataBound" OnItemCommand="RptPlaceholdersItemCommand">
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblPlaceholder" runat="server"></asp:Label>
                                    </td>
                                    <td class="center">
                                        <asp:HyperLink ID="hplSection" runat="server" Visible="False"></asp:HyperLink>
                                    </td>
                                    <td class="center">
                                        <asp:HyperLink ID="hplAttachSection" runat="server" Visible="false">Attach section</asp:HyperLink>
                                        <asp:LinkButton ID="lbtDetachSection" runat="server" Visible="false" CommandName="detach">Detach section</asp:LinkButton>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                        </tbody>
                    </table>
                    
                </asp:Panel>
                
                <asp:Panel ID="pnlNoPlaceholders" runat="server" Visible="false">
                    <p>There are no sections to display.</p>
                </asp:Panel>
            
            </div>
            
            <div id="tabs-3">
            
                <p><em>Please ensure that the .zip file contains the correct files required to be processed as a valid template pack.</em></p>
        
                <p><asp:Literal ID="litMessages" runat="server" Text=""/></p>
                <asp:Label ID="lblUploadTemplate" AssociatedControlID="uplUploadTemplate" Text="Upload Template Pack(.zip)" runat="server"></asp:Label>
                <asp:FileUpload ID="uplUploadTemplate" runat="server" />
                <asp:Button ID="btnUpload" runat="server" Text="Upload Template Pack" CssClass="cancel" onclick="BtnUploadClick"/>
                <%--<asp:RequiredFieldValidator ID="uplUploadTemplateValidator" runat="server" ErrorMessage="Zip file is required" CssClass="validator" EnableClientScript="True" ValidationGroup="uploader" ControlToValidate="uplUploadTemplate"></asp:RequiredFieldValidator>--%>
        
            </div>
        </div>

        <asp:Button ID="btnSave" runat="server" OnClick="BtnSaveClick" Text="Save"></asp:Button>
        <asp:Button ID="btnBack" runat="server" OnClick="BtnCancelClick" Text="Back" CssClass="cancel"></asp:Button>
        <asp:Button ID="btnDelete" runat="server" OnClick="BtnDeleteClick" Text="Delete"  CssClass="cancel"></asp:Button>

    </form>
</body>
</html>
