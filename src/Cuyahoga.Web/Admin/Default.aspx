<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Cuyahoga.Web.Admin.Default" %><!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
  <head>
    <title>Cuyahoga Site Administration</title>
    <link href="Css/Admin.css" type="text/css" rel="stylesheet"/>
  </head>
  <body>
    <form id="Form1" method="post" runat="server">
        <div>
            <p>This is where you manage the pages, modules and content of your website. The five steps to creating and managing a website are:</p>
            <ol>
                <li>Create your site structure</li>
                <li>Select and apply a site style template</li>
                <li>Add modules to your pages</li>
                <li>Edit the content of the modules</li>
                <li>Your are done!</li>
            </ol>
            <p>That's all there is to it. Of course, you can do a lot more if required. Take a look at the menu to the left and examine the options.</p>
            <h3>Sites</h3>
            <p>Your website navigation/page structure is here. Select your site and then a page to edit its settings.</p>
            <h3>Sections</h3>
            <p>Create a library of modules complete with content and presets.</p>
            <h3>Modules</h3>
            <p>Here you can manage the modules available to your site. You can install new modules or remove unwanted (and unused) modules.</p>
            <h3>Templates</h3>
            <p>Here is where you manage the templates that control the visual aesthetic of your website.</p>
            <h3>Users</h3>
            <p>Give people access to your website.</p>
            <h3>Roles</h3>
            <p>Here is where you manage the templates that control the visual aesthetic of your website.</p>
            <h3>Search</h3>
            <p>The search uses an 'index' of all the searchable content in your website. After editing content you should use this function to re-index your website. This will make all your new content available to the search.</p>
        </div>
    </form>
  </body>
</html>