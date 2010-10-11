<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SectionEdit.aspx.cs" Inherits="Cuyahoga.Web.Admin.SectionEdit" %><!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head>
		<title>SectionEdit</title>
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
		    
		    <script type="text/javascript"> 
            <!--

                $(document).ready(function() {

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
                        var errorTab = $("#sectiontabs").children("div").has(".error").find("label[class*='error']").first().parents("div").attr("id");
                        $("#sectiontabs").tabs('select', '#' + errorTab);
                    }

                    $("input[id*='Save']").click(function() {
                        validator.form({ focusCleanup: true });
                        moveToErrorTab();
                    });

                    //Custom Regex for nospace link
                    $.validator.addMethod(
                            "positiveInt",
                            function(value, element) {
                            return this.optional(element) || /^\d+$/.test(value);
                            },
                            "No spaces are allowed in your input."
                    );

                    //Validation rules
                    var vTitle = $("input[name*='Title']").rules("add", {
                        required: true,
                        messages: {
                            required: "A Title is required"
                        }
                    });
                    
                    var vCacheDuration = $("input[name*='CacheDuration']").rules("add", {
                        required: true,
                        positiveInt: true,
                        messages: {
                            required: "Cache duration is required (0 for no cache)",
                            positiveInt : "Only positive integers allowed"
                        }
                    });
                
                
                    // Tabs
                    $("#sectiontabs").tabs({
                        cookie: {
	                        // store cookie for a day, without, it would be a session cookie
	                        // expires: 30,
	                        name: 'sectiontabs'
                        }
                    });

                });
                
		    // -->
            </script>
            
            <p>Manage the sections that are in the selected page.</p>
            
            <div id="sectiontabs">
			    <ul>
				    <li><a href="#tabs-1">General</a></li>
				    <li><a href="#tabs-2">Custom Settings</a></li>
				    <li><a href="#tabs-3">Connections</a></li>
				    <li><a href="#tabs-4">Authorisation</a></li>
			    </ul>
    			
			    <div id="tabs-1">
    			
				    <table id="sections">
					    <tr>
						    <td id="title">Section title</td>
						    <td><asp:textbox id="txtTitle" runat="server" width="300px"></asp:textbox>
							    <asp:requiredfieldvalidator id="rfvTitle" runat="server" display="Dynamic" cssclass="validator" controltovalidate="txtTitle"
								    enableclientscript="False">Title is required</asp:requiredfieldvalidator></td>
					    </tr>
					    <tr>
						    <td>CSS Class</td>
						    <td><asp:textbox id="txtCSSClass" runat="server" width="300px"></asp:textbox></td>
					    </tr>
					    <tr>
						    <td>Show section title</td>
						    <td><asp:checkbox id="chkShowTitle" runat="server"></asp:checkbox></td>
					    </tr>
					    <tr>
						    <td>Module</td>
						    <td>
							    <asp:dropdownlist id="ddlModule" runat="server" autopostback="True" visible="False"></asp:dropdownlist>
							    <asp:label id="lblModule" runat="server" visible="False"></asp:label>
						    </td>
					    </tr>
					    <tr>
						    <td>Placeholder</td>
						    <td>
							    <asp:dropdownlist id="ddlPlaceholder" runat="server"></asp:dropdownlist>
							    &nbsp;<asp:hyperlink id="hplLookup" runat="server">Lookup</asp:hyperlink>
						    </td>
					    </tr>
					    <tr>
						    <td>Cache duration</td>
						    <td><asp:textbox id="txtCacheDuration" runat="server" width="30px"></asp:textbox><asp:requiredfieldvalidator id="rfvCache" runat="server" display="Dynamic" cssclass="validator" controltovalidate="txtCacheDuration"
								    enableclientscript="False">Cache duration is required (0 for no cache)</asp:requiredfieldvalidator><asp:comparevalidator id="cpvCache" runat="server" display="Dynamic" cssclass="validator" controltovalidate="txtCacheDuration"
								    enableclientscript="False" errormessage="Only positive integers allowed" operator="GreaterThanEqual" valuetocompare="0" type="Integer"></asp:comparevalidator></td>
					    </tr>
				    </table>
    			
			    </div>
    			
			    <div id="tabs-2">
    			
			        <asp:panel id="pnlCustomSettings" runat="server" enableviewstate="false">
				        <table id="customsettings" class="tbl">
 				            <tr>
                                <th id="settingname">Section Setting Name</th>
                                <th id="action">Section Setting Value</th>
                            </tr>
					        <asp:placeholder id="plcCustomSettings" runat="server" />
				        </table>
			        </asp:panel>
    			    <asp:panel id="pnlNoCustomSettings" runat="server" visible="true">
			            <p>There are no settings to display for this section.</p>
			        </asp:panel>
			    </div>
    			
			    <div id="tabs-3">
    			
			        <asp:panel id="pnlConnections" runat="server" visible="False">
				        <table id="connections" class="tbl">
					        <asp:repeater id="rptConnections" OnItemCommand="RptConnectionsItemCommand" runat="server">
						        <headertemplate>
							        <tr>
								        <th id="tosection">To section</th>
								        <th id="action">Action</th>
								        <th id="connectiondelete">Delete</th>
							        </tr>
						        </headertemplate>
						        <itemtemplate>
							        <tr>
								        <td><%# DataBinder.Eval(Container.DataItem, "Value.FullName") %></td>
								        <td class="center"><%# DataBinder.Eval(Container.DataItem, "Key") %></td>
								        <td class="center"><asp:linkbutton id="lbtDelete" runat="server" causesvalidation="False" commandname="DeleteConnection" commandargument='<%# DataBinder.Eval(Container.DataItem, "Key") %>'>Delete</asp:linkbutton></td>
							        </tr>
						        </itemtemplate>
					        </asp:repeater>
				        </table>
				        <asp:hyperlink id="hplNewConnection" runat="server">Add connection</asp:hyperlink>
			        </asp:panel>
			        <asp:panel id="pnlNoConnections" runat="server" visible="true">
			            <p>There are no connections to display for this section.</p>
			        </asp:panel>
    			
			    </div>
    			
			    <div id="tabs-4">
    			
				    <table id="authorisation" class="tbl">
					    <asp:repeater id="rptRoles" runat="server">
						    <headertemplate>
							    <tr>
								    <th id="role">Role</th>
								    <th id="viewallowed">View allowed</th>
								    <th id="editallowed">Edit allowed</th>
							    </tr>
						    </headertemplate>
						    <itemtemplate>
							    <tr>
								    <td><%# DataBinder.Eval(Container.DataItem, "Name") %></td>
								    <td style="text-align:center">
									    <asp:checkbox id="chkViewAllowed" runat="server"></asp:checkbox></td>
								    <td style="text-align:center">
									    <asp:checkbox id="chkEditAllowed" runat="server"></asp:checkbox></td>
							    </tr>
						    </itemtemplate>
					    </asp:repeater>
				    </table>
    			
			    </div>
			</div>
			
			<div>
				<asp:button id="btnSave" runat="server" OnClick="BtnSaveClick" text="Save"></asp:button>
				<asp:button id="btnBack" runat="server" OnClick="BtnBackClick" text="Back" causesvalidation="False"></asp:button>
			</div>
			
			<script type="text/javascript"> <!--
			function setPlaceholderValue(ddlist, val)
			{
				var placeholdersList = document.getElementById(ddlist);
				if (placeholdersList != null)
				{
					for (i = 0; i < placeholdersList.options.length; i++)
					{
						if (placeholdersList.options[i].value == val)
						{
							placeholdersList.selectedIndex = i;
						}
					}				
				}
			}
			// -->
			</script>
			
		</form>
	</body>
</html>