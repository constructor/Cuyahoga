<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Modules.aspx.cs" Inherits="Cuyahoga.Web.Admin.Modules" %><!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
    <title>Modules</title>
</head>
<body>
    <form id="Form1" method="post" runat="server">
    
        <div class="group">
            <h4>Upload Module</h4>
            <p><asp:Literal ID="litMessages" runat="server" Text=""/></p>
            <asp:Label ID="lblUploadModule" AssociatedControlID="uplUploadModule" Text="Upload Module Pack(.zip)" runat="server"></asp:Label>
            <asp:FileUpload ID="uplUploadModule" runat="server" />
            <asp:Button ID="btnUpload" runat="server" Text="Upload Module Pack" 
                ValidationGroup="uploader" CausesValidation="true" onclick="btnUpload_Click" />
            <asp:RequiredFieldValidator ID="uplUploadModuleValidator" runat="server" ErrorMessage="Zip file is required" CssClass="validator" EnableClientScript="True" ValidationGroup="uploader" ControlToValidate="uplUploadModule"></asp:RequiredFieldValidator>
        </div>
        <hr />
    
        <p><em>NOTE: The Load on startup setting affects the activation on application startup. However,
            checking it will try to activate the module immediately immediately</em></p>
        <table class="tbl">
            <asp:Repeater ID="rptModules" runat="server" OnItemDataBound="rptModules_ItemDataBound" OnItemCommand="rptModules_ItemCommand">
                <HeaderTemplate>
                    <tr>
                        <th>Module name</th>
                        <th>Assembly</th>
                        <th>Load on startup</th>
                        <th>Activation Status</th>
                        <th>Installation Status</th>
                        <th>Actions</th>
                    </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td><%# DataBinder.Eval(Container.DataItem, "Name") %></td>
                        <td><%# DataBinder.Eval(Container.DataItem, "AssemblyName") %></td>
                        <td><asp:CheckBox ID="chkBoxActivation" runat="server" AutoPostBack="true" OnCheckedChanged="chkBoxActivation_CheckedChanged" /></td>
                        <td><asp:Literal ID="litActivationStatus" runat="server" /></td>
                        <td><asp:Literal ID="litStatus" runat="server"></asp:Literal></td>
                        <td><asp:LinkButton ID="lbtInstall" runat="server" Visible="False" CommandName="Install" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Name") + ":" + DataBinder.Eval(Container.DataItem, "AssemblyName") %>'>Install</asp:LinkButton>
                            <asp:LinkButton ID="lbtUpgrade" runat="server" Visible="False" CommandName="Upgrade" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Name") + ":" + DataBinder.Eval(Container.DataItem, "AssemblyName") %>'>Upgrade</asp:LinkButton>
                            <asp:LinkButton ID="lbtUninstall" runat="server" Visible="False" CommandName="Uninstall" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Name") + ":" + DataBinder.Eval(Container.DataItem, "AssemblyName") %>'>Uninstall</asp:LinkButton>
                            <asp:LinkButton ID="lbtDelete" runat="server" Visible="False" CommandName="Delete" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Name") + ":" + DataBinder.Eval(Container.DataItem, "AssemblyName") %>'>Delete</asp:LinkButton></td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>
    </form>
</body>
</html>