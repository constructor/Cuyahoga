<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TemplateEdit.aspx.cs" Inherits="Cuyahoga.Web.Admin.TemplateEdit" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>TemplateEdit</title>
</head>
<body>
    <form id="Form1" method="post" runat="server">
    <p>
        <em>Make sure you have placed at least one template control (.ascx) in the directory
            specified as the Base path and at least one css file in Base path/Css directory.</em>
    </p>
    <div class="group">
        <h4>Upload Template</h4>
        <p><asp:Literal ID="litMessages" runat="server" Text=""/></p>
        <asp:Label ID="lblUploadTemaple" AssociatedControlID="uplUploadTemplate" Text="Upload Template Pack (.zip)" runat="server"></asp:Label>
        <asp:FileUpload ID="uplUploadTemplate" runat="server" />
        <asp:Button ID="btnUpload" runat="server" Text="Upload Template Pack" onclick="BtnUploadClick" ValidationGroup="uploader" CausesValidation="true" />
        <asp:RequiredFieldValidator ID="uplUploadTemplateValidator" runat="server" ErrorMessage="Zip file is required" CssClass="validator" EnableClientScript="True" ValidationGroup="uploader" ControlToValidate="uplUploadTemplate"></asp:RequiredFieldValidator>
    </div>
    <hr />
    <div class="group">
        <h4>General</h4>
        <table>
            <tr>
                <td style="width: 200px">
                    Name
                </td>
                <td>
                    <asp:TextBox ID="txtName" runat="server" Width="200px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvName" runat="server" ErrorMessage="Name is required" CssClass="validator" Display="Dynamic" EnableClientScript="False" ControlToValidate="txtName"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td style="width: 200px">
                    Site (the template belongs to)
                </td>
                <td>
                    <asp:DropDownList ID="ddlSites" runat="server" AppendDataBoundItems="true" DataValueField="Id"
                        DataTextField="Name" AutoPostBack="True" onselectedindexchanged="DdlSitesSelectedIndexChanged">
                        <asp:ListItem>None</asp:ListItem>
                    </asp:DropDownList>
                    <asp:Button ID="btnCopyToSite" runat="server" Text="Copy to site" CausesValidation="False" OnClick="CopyTemplateToSite"></asp:Button>
                </td>
            </tr>
            <tr>
                <td>
                    Template path (from root) /</td>
                <td>
                    <asp:TextBox ID="txtBasePath" runat="server" Width="200px"></asp:TextBox>
                    <asp:Button ID="btnVerifyBasePath" runat="server" Text="Verify" CausesValidation="True" ValidationGroup="basepath" OnClick="BtnVerifyBasePathClick"></asp:Button>
                    <asp:RequiredFieldValidator ID="rfvBasePath" runat="server" ErrorMessage="Base path is required" CssClass="validator" EnableClientScript="True" ControlToValidate="txtBasePath" ValidationGroup="basepath"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td>
                    Template control
                </td>
                <td>
                    <asp:DropDownList ID="ddlTemplateControls" runat="server"></asp:DropDownList>
                    <asp:Label ID="lblTemplateControlWarning" runat="server" CssClass="validator" Visible="False" EnableViewState="False"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    Css
                </td>
                <td>
                    <asp:DropDownList ID="ddlCss" runat="server"></asp:DropDownList>
                    <asp:Label ID="lblCssWarning" runat="server" CssClass="validator" Visible="False" EnableViewState="False"></asp:Label>
                </td>
            </tr>
        </table>
    </div>
    <br />
    <asp:Panel ID="pnlPlaceholders" runat="server" Visible="False" CssClass="group">
        <h4>Placeholders</h4>
        <table class="tbl">
            <tr>
                <th>
                    Placeholder
                </th>
                <th>
                    Attached section
                </th>
                <th>
                </th>
            </tr>
            <asp:Repeater ID="rptPlaceholders" runat="server" OnItemDataBound="RptPlaceholdersItemDataBound" OnItemCommand="RptPlaceholdersItemCommand">
                <ItemTemplate>
                    <tr>
                        <td>
                            <asp:Label ID="lblPlaceholder" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:HyperLink ID="hplSection" runat="server" Visible="False"></asp:HyperLink>
                        </td>
                        <td>
                            <asp:HyperLink ID="hplAttachSection" runat="server" Visible="false">Attach section</asp:HyperLink>
                            <asp:LinkButton ID="lbtDetachSection" runat="server" Visible="false" CommandName="detach">Detach section</asp:LinkButton>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>
    </asp:Panel>
    <br />
    <asp:Button ID="btnSave" runat="server" OnClick="BtnSaveClick" Text="Save"></asp:Button>
    <asp:Button ID="btnBack" runat="server" OnClick="BtnCancelClick" Text="Back" CausesValidation="false"></asp:Button>
    <asp:Button ID="btnDelete" runat="server" OnClick="BtnDeleteClick" Text="Delete" CausesValidation="false"></asp:Button>
    </form>
</body>
</html>
