<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Cuyahoga.Web.Admin.Default" %><!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
  <head>
    <title>Overview</title>
    <link href="Css/Admin.css" type="text/css" rel="stylesheet"/>
  </head>
  <body>
    <form id="Form1" method="post" runat="server">
        <div>
            <img id="cuyahoga1-7" src="Images/logo-admin-splash.gif" />
            <p>This version was released on {date} and some of the features include...</p>
            <ul id="feature-list">
                <li>New core</li>
                <li>New UI</li>
                <li>Multiple sites</li>
                <li>New modules</li>
                <li>More...</li>
            </ul>
            <p>Links to release notes...</p>
            <h3>Cuyahoga Modules</h3>
            <p>Links to module downloads / module developers...</p>
            <h3>Extend Cuyahoga</h3>
            <p>Developer documentation...</p>
        </div>
    </form>
  </body>
</html>