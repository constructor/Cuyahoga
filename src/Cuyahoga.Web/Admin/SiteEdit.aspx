<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SiteEdit.aspx.cs" Inherits="Cuyahoga.Web.Admin.SiteEdit" %><!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
    <head>
		<title>SiteEdit</title>
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
		
	        <script type="text/javascript"> 
                <!--
	            $(document).ready(function() {

	                //Validator is fired by 'Save' button
	                var validator = $("#aspnetForm").validate({
                        success: function(label){
                                label.remove();//So the errorTab can be found (otherwise finds hidden labels)
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
	                    var errorTab = $("#sitetabs").children("div").has(".error").find("label[class*='error']").first().parents("div").attr("id");
	                    $("#sitetabs").tabs('select', '#' + errorTab);
	                }

	                $("input[id*='Save']").click(function() {
	                    validator.form({focusCleanup: true});
	                    moveToErrorTab();
	                });

	                //Custom requiredUrl that allows http://localhost
	                $.validator.addMethod("localUrl", function(value, element) {
	                    return this.optional(element) || /^(http:\/\/|https:\/\/)?((([\w-]+\.)+[\w-]+)|localhost)(\/[\w- .\/?%&=]*)?/i.test(value);
	                }, "required url is needed");



	                //Validation rules
	                var vtxtName = $("input[name*='txtName']").rules("add", {
	                    required: true,
	                    messages: {
	                        required: "A site name is required"
	                    }
	                });

	                var vtxtSiteUrl = $("input[name*='txtSiteUrl']").rules("add", {
	                    required: true,
	                    localUrl: true,
	                    messages: {
	                        required: "A valid site url is required",
	                        localUrl: "A valid site url is required"
	                    }
	                });

	                var vtxtWebmasterEmail = $("input[name*='txtWebmasterEmail']").rules("add", {
	                    required: true,
	                    email: true,
	                    messages: {
	                        required: "A valid email is required",
	                        email: "A valid email is required"
	                    }
	                });

	                var vddlTemplates = $("select[id*='ddlTemplates']").rules("add", {
	                    required: true,
	                    messages: {
	                        required: "A site template must be selected"
	                    }
	                });

	                //SiteEdit Tabs
	                var theurl = window.location.search;
	                if ($.getUrlVar(theurl, 'SiteId') == "-1") {
	                    $('#sitetabs').tabs(
	                            {
	                                cookie: {
	                                    //expires: 30,
	                                    name: 'sitetabscreate'
	                                }
	                            });
	                } else {
	                    $('#sitetabs').tabs(
                            {
                                cookie: {
                                    //expires: 30,
                                    name: 'sitetabs'
                                }
                            });
	                } //End Tabs

	            }); //End $(document).ready(function() {

	            // -->
            </script>
		
		    <p>Edit the settings, defaults and aliases of the selected site.</p>
	        
			<div id="sitetabs">
			    <ul>
				    <li><a href="#tabs-1">General Settings</a></li>
				    <li><a href="#tabs-2">Site Defaults</a></li>
				    <li><a href="#tabs-3">Site Aliases</a></li>
			    </ul>
			
			    <div id="tabs-1">
    			
                        <label>Name</label>
                        <asp:textbox id="txtName" runat="server" width="300px" maxlength="100"></asp:textbox>
                        <asp:requiredfieldvalidator id="rfvName" runat="server" enableclientscript="False" 
                            controltovalidate="txtName" cssclass="validator" display="Dynamic" 
                            ValidationGroup="Site">Name is required</asp:requiredfieldvalidator>
                        <hr />
                        
                        <label>Site url (incl. http://)</label>
                        <asp:textbox id="txtSiteUrl" runat="server" width="300px" maxlength="100"></asp:textbox>
                        <asp:requiredfieldvalidator id="rfvSiteUrl" runat="server" enableclientscript="False" 
                            controltovalidate="txtSiteUrl" cssclass="validator" display="Dynamic" 
                            ValidationGroup="Site">Site url is required</asp:requiredfieldvalidator>
                        <hr />
                        
                        <label>Webmaster email</label>
                        <asp:textbox id="txtWebmasterEmail" runat="server" maxlength="100" width="300px"></asp:textbox>
                        <asp:requiredfieldvalidator id="rfvWebmasterEmail" runat="server" 
                            errormessage="Webmaster email is required" cssclass="validator" display="Dynamic" 
                            controltovalidate="txtWebmasterEmail" enableclientscript="False" ValidationGroup="Site"></asp:requiredfieldvalidator>
                        <asp:RegularExpressionValidator ID="rexvWebmasterEmail" runat="server" cssclass="validator" 
                            display="Dynamic" controltovalidate="txtWebmasterEmail" 
                            ValidationExpression="^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$" 
                            ErrorMessage="A valid email address is required" EnableClientScript="False" 
                            ValidationGroup="Site"></asp:RegularExpressionValidator>
                        <hr />
                        
                        <label>Use friendly url's</label>
                        <asp:checkbox id="chkUseFriendlyUrls" runat="server"></asp:checkbox>

        		    </div>
            		
        		    <div id="tabs-2">

			            <label>Template</label>
                        <asp:DropDownList ID="ddlTemplates" runat="server" AutoPostBack="true" AppendDataBoundItems="true" OnSelectedIndexChanged="DdlTemplatesSelectedIndexChanged">
                            <asp:ListItem Value="">Select Template</asp:ListItem>
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvddlTemplates" runat="server" ControlToValidate="ddlTemplates" 
                            ErrorMessage="Please select a default template" EnableClientScript="False" Display="Dynamic" 
                            ValidationGroup="Site"></asp:RequiredFieldValidator>
                        <p><asp:HyperLink ID="hplAddTemplate" runat="server">Add Template</asp:HyperLink><hr /></p>
                        
			            <label>Placeholder</label>
			            <asp:dropdownlist id="ddlPlaceholders" runat="server"></asp:dropdownlist><em>(this is the placeholder where the content of general pages is inserted)</em><hr />

			            <label>Culture</label>
			            <asp:dropdownlist id="ddlCultures" runat="server"></asp:dropdownlist><hr />

			            <label>Role for registered users</label>
			            <asp:dropdownlist id="ddlRoles" runat="server"></asp:dropdownlist><hr />

			            <label>Meta description</label>
                        <asp:TextBox ID="txtMetaDescription" runat="server" MaxLength="500" TextMode="MultiLine" Width="400px" height="70px"></asp:TextBox><hr />

			            <label>Meta keywords</label>
			            <asp:TextBox ID="txtMetaKeywords" runat="server" MaxLength="500" TextMode="MultiLine" Width="400px" height="70px"></asp:TextBox><hr />
            			
        		    </div>
            		
        		    <div id="tabs-3">
        		    
        		        <asp:panel id="pnlAliases" runat="server">
		                    <table id="aliases" class="tbl">
			                    <asp:repeater id="rptAliases" runat="server" onitemdatabound="RptAliasesItemDataBound">
				                    <headertemplate>
					                    <tr>
						                    <th id="aliasurl">Alias url</th>
						                    <th id="entrynode">Entry Node</th>
						                    <th>&nbsp;</th>
					                    </tr>
				                    </headertemplate>
				                    <itemtemplate>
					                    <tr>
						                    <td><%# DataBinder.Eval(Container.DataItem, "Url") %></td>
						                    <td><asp:label id="lblEntryNode" runat="server"></asp:label></td>
						                    <td><asp:hyperlink id="hplEdit" runat="server">Edit</asp:hyperlink></td>
					                    </tr>
				                    </itemtemplate>
			                    </asp:repeater>
			                </table>
			            </asp:panel>
			            <asp:panel id="pnlNoAliases" runat="server">
			                <p>There are no aliases to display for this site.</p>
			                <asp:Literal ID="litAliases" runat="server" Text="<p>You can not create an alias until the site has been created.</p>"></asp:Literal>
			            </asp:panel>
		                <asp:hyperlink id="hplNewAlias" runat="server">Add Site Alias</asp:hyperlink>
    		        
        		    </div>
        		
        		</div>
        		        			
		        <asp:button id="btnSave" runat="server" text="Save" onclick="BtnSaveClick" CausesValidation="true" ValidationGroup="Site"></asp:button>
		        <asp:button id="btnCancel" runat="server" text="Cancel" CssClass="cancel" causesvalidation="False" onclick="BtnCancelClick"></asp:button>
		        <asp:button id="btnDelete" runat="server" text="Delete" CssClass="cancel" causesvalidation="False" onclick="BtnDeleteClick"></asp:button>
		        
		</form>
	</body>
</html>
