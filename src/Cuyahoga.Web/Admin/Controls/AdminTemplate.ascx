<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminTemplate.ascx.cs" Inherits="Cuyahoga.Web.Admin.Controls.AdminTemplate" %>
<%@ Register TagPrefix="uc1" TagName="Header" Src="Header.ascx" %>
<%@ Register TagPrefix="uc1" TagName="NavigationBar" Src="NavigationBar.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Navigation" Src="Navigation.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Cuyahoga Admin Panel - <asp:Literal ID="PageTitle" runat="server"></asp:Literal></title>
    <link id="CssStyleSheet" rel="stylesheet" type="text/css" runat="server" />
    <!--[if IE 6]><link href="Css/AdminIE6.css" type="text/css" rel="stylesheet"><![endif]-->
    <link type="text/css" href="/js/jquery-treeview/jquery.treeview.css" rel="stylesheet" />
    <link type="text/css" href="/js/jquery-ui-1.8.4/css/cuyahoga-green/jquery-ui-1.8.4.custom.css" rel="stylesheet" />
    
    <!-- The 'if statement' enables jQuery intellisense by provideing correct path to the files for VS2008. 'CTRL-SHIFT J' also refreshes the intellisense for jQuery -->
    <%if (false){ %><script src="~/js/jquery-1.4.1.min.js" type="text/javascript"></script><%} %>

    <script src="/js/jquery-1.4.2.min.js" type="text/javascript"></script>
    <script src="/js/jquery-ui-1.8.4/js/jquery-ui-1.8.4.custom.min.js" type="text/javascript"></script>
    <script src="/js/jquery.validate.min.js" type="text/javascript"></script>
    <script src="/js/jquery.cookie.js" type="text/javascript"></script>
    <script src="/js/jquery-urlEncode.js" type="text/javascript"></script>
    <script src="/js/jquery.urldecoder.min.js" type="text/javascript"></script>
    <script src="/js/jquery-treeview/jquery.treeview.min.js" type="text/javascript"></script>
    <script src="/js/jquery-contextmenu/jquery.contextMenu.js" type="text/javascript"></script>
    <script src="/js/jQuery-jTypeWriter.js" type="text/javascript"></script>

    <script type="text/javascript">
        //Session["ShowAll"]
        var showAll = <% = Session["ShowAll"].ToString().ToLower() %>;
        var appPath = '<% = Cuyahoga.Web.Util.UrlHelper.GetApplicationPath().ToString() %>';
    
        //START Context menu ajax functions
        function moveNode(sourceSiteId, sourceNodeId, direction) {
            $.ajax({
                type: "POST",
                url: appPath + "Admin/AdminService.asmx/MoveNode",
                data: "{sourceSiteId : " + sourceSiteId
                            + ",sourceNodeId : " + sourceNodeId
                            + ",direction : '" + direction + "'"
                            + ",showAll : " + showAll + "}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function(msg) {
                    //Put d (returnobject) into result
                    var returnObject = msg.d; //JSON Object

                    buffernode(0, 0, '');

                    $(".sitepanel").replaceWith(returnObject.SitesHTML);
                    showMessage(returnObject.Details);
                    siteTree();
                    contextMenu();

                    menuModalOff();
                    //refreshPage(sourceSiteId, returnObject.NodeId, returnObject.Details);
                },
                    error:  function(xhr) { 
                    //alert(xhr);
                    alert(xhr.statusText);
                    //alert(xhr.responseText);
                }
            })
        }

        function deleteNode(sourceSiteId, sourceNodeId) {
            $.ajax({
                type: "POST",
                url: appPath + "Admin/AdminService.asmx/DeleteNode",
                data: "{sourceSiteId: " + sourceSiteId
                        + ",sourceNodeId: " + sourceNodeId
                        + ",showAll : " + showAll + "}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function(msg) {
                    //Put d (returnobject) into result
                    var returnObject = msg.d; //JSON Object

                    buffernode(0, 0, '');

                    $(".sitepanel").replaceWith(returnObject.SitesHTML);
                    showMessage(returnObject.Details);
                    siteTree();
                    contextMenu();
                    
                    menuModalOff();
                    //refreshPage(sourceSiteId, returnObject.NodeId, returnObject.Details);
                },
                    error:  function(xhr) { 
                    //alert(xhr);
                    alert(xhr.statusText);
                    //alert(xhr.responseText);
                }
            })
        }

        function pasteNode(sourceSiteId, sourceNodeId, destinationSiteId, destinationNodeId) {
            $.ajax({
                type: "POST",
                url: appPath + "Admin/AdminService.asmx/PasteNode",
                data: "{sourceSiteId : " + sourceSiteId
                            + ",sourceNodeId : " + sourceNodeId
                            + ",destinationSiteId : " + destinationSiteId
                            + ",destinationNodeId : " + destinationNodeId
                            + ",showAll : " + showAll + "}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function(msg) {
                    //Put d (returnobject) into result
                    var returnObject = msg.d; //JSON Object

                    buffernode(0, 0, '');

                    $(".sitepanel").replaceWith(returnObject.SitesHTML);
                    showMessage(returnObject.Details);
                    siteTree();
                    contextMenu();
                    
                    menuModalOff();
                    //refreshPage(destinationSiteId, destinationNodeId, returnObject.Details);
                },
                    error:  function(xhr) { 
                    //alert(xhr);
                    alert(xhr.statusText);
                    //alert(xhr.responseText);
                }
            })
        }

        function pasteCopyNode(sourceSiteId, sourceNodeId, destinationSiteId, destinationNodeId) {

            if (sourceSiteId == destinationSiteId) {
                $.ajax({
                    type: "POST",
                    url: appPath + "Admin/AdminService.asmx/PasteCopyNode",
                    data: "{sourceSiteId : " + sourceSiteId
                            + ",sourceNodeId : " + sourceNodeId
                            + ",destinationSiteId : " + destinationSiteId
                            + ",destinationNodeId : " + destinationNodeId
                            + ",showAll : " + showAll + "}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function(msg) {
                        //Put d (returnobject) into result
                        var returnObject = msg.d; //JSON Object

                        buffernode(0, 0, '');

                        $(".sitepanel").replaceWith(returnObject.SitesHTML);
                        showMessage(returnObject.Details);
                        siteTree();
                        contextMenu();

                        menuModalOff();
                        //refreshPage(destinationSiteId, destinationNodeId, returnObject.Details);
                    },
                    error:  function(xhr) { 
                            //alert(xhr);
                            alert(xhr.statusText);
                            //alert(xhr.responseText);
                    }
                })
            }
            else {
                showMessage("Cross site node paste not allowed.");
                menuModalOff();
             }
        }
        //End context menu ajax functions

        //Context menu helper/utils functions
        
        //Exists function
        $.fn.exists = function(){return jQuery(this).length > 0;}
        
        function newNode(destinationSiteId, destinationNodeId) {
            var newnodeurl = 'http://' + location.hostname + '/Admin/NodeEdit.aspx';
            newnodeurl = addParamToURL(newnodeurl, 'NodeId', '-1');
            newnodeurl = addParamToURL(newnodeurl, 'ParentNodeId', destinationNodeId);
            newnodeurl = addParamToURL(newnodeurl, 'SiteID', destinationSiteId);
            window.location = newnodeurl;
        }

        function siteTree() {
            $(".sitepanel > ul").treeview({
                collapsed: false,
                animated: "fast",
                control: "#collapseallcontrol",
                persist: "cookie",
                cookieId: "cuyahoga"
            });
        }

        function contextMenu() {
            
            // Show menu when a list item is clicked
            $(".sitepanel ul li:not(.site) a, .sitepanel ul li:not(.site) span").contextMenu({ menu: 'admincontext' },
                        function(action, el, pos) {

                            //  var thesiteurl = $(el).parents().filter('.treeview').find('a:first').attr('href')
                            //  var thenodeurl = $(el).attr('href')
 
                            //  var thesiteid = $.getUrlVar(thesiteurl, 'SiteId');
                            //  var thenodeid = $.getUrlVar(thenodeurl, 'NodeId');
                            
                            var isRoot = $(el).parent('li').hasClass('rootnode');
                            var destinationSiteId = $(el).parent('li').attr('site');
                            var destinationNodeId = $(el).parent('li').attr('node');

                            switch (action) {
                                case "new":
                                    menuModalOn();
                                    newNode(destinationSiteId, destinationNodeId);
                                    break;
                                case "cut":
                                    if (!isRoot) {
                                        menuModalOn();
                                        buffernode(destinationSiteId, destinationNodeId, 'cut');
                                        menuModalOff();
                                        showMessage('Cut node \'' + $(el).text() + '\' of site \'' + $(el).parents().filter('.treeview').find('a:first').text() + '\'');
                                    }
                                    else
                                    {
                                        showMessage('Can not cut root node \'' + $(el).text() + '\' of site \'' + $(el).parents().filter('.treeview').find('a:first').text() + '\'');
                                    }
                                    break;
                                case "copy":
                                    menuModalOn();
                                    buffernode(destinationSiteId, destinationNodeId, 'copy');
                                    menuModalOff();
                                    showMessage('Copied node \'' + $(el).text() + '\' of site \'' + $(el).parents().filter('.treeview').find('a:first').text() + '\'');
                                    break;
                                case "delete":
                                    if (confirm('Confirm delete of node \'' + $(el).text() + '\'?')) {
                                        menuModalOn();
                                        deleteNode(destinationSiteId, destinationNodeId);
                                    }                                    
                                    break;
                                case "paste":
                                    menuModalOn();
                                    if (actionbuffer.action == 'cut') {
                                        pasteNode(actionbuffer.siteid, actionbuffer.nodeid, destinationSiteId, destinationNodeId);
                                    }
                                    else {
                                        pasteCopyNode(actionbuffer.siteid, actionbuffer.nodeid, destinationSiteId, destinationNodeId);
                                    }
                                    break;
                                case "moveup":
                                    if (!isRoot) {
                                        menuModalOn();
                                        moveNode(destinationSiteId, destinationNodeId, 'up');
                                    }
                                    else
                                    {
                                        showMessage('Can not move up root node \'' + $(el).text() + '\' of site \'' + $(el).parents().filter('.treeview').find('a:first').text() + '\'');
                                    }
                                    break;
                                case "movedown":
                                if (!isRoot) {
                                    menuModalOn();
                                    moveNode(destinationSiteId, destinationNodeId, 'down');
                                    }
                                    else
                                    {
                                        showMessage('Can not move down root node \'' + $(el).text() + '\' of site \'' + $(el).parents().filter('.treeview').find('a:first').text() + '\'');
                                    }
                                    break;
                                case "moveleft":
                                if (!isRoot) {
                                    menuModalOn();
                                    moveNode(destinationSiteId, destinationNodeId, 'left');
                                    }
                                    else
                                    {
                                        showMessage('Can not move left root node \'' + $(el).text() + '\' of site \'' + $(el).parents().filter('.treeview').find('a:first').text() + '\'');
                                    }
                                    break;
                                case "moveright":
                                if (!isRoot) {
                                    menuModalOn();
                                    moveNode(destinationSiteId, destinationNodeId, 'right');
                                    }
                                    else
                                    {
                                        showMessage('Can not move right root node \'' + $(el).text() + '\' of site \'' + $(el).parents().filter('.treeview').find('a:first').text() + '\'');
                                    }
                                    break;
                                default:
                                    //moveNode(thesiteid, thenodeid, thenodeid);
                            }

                        }
                );
    
        }

        function menuModalOn() {
            $('#menumodal').css('display', 'block');
        }
        function menuModalOff() {
            $('#menumodal').css('display', 'none');
        }

        function showMessage(message) {
            $('#messages').empty();
            if (message != '') {
                $('#messages').append('<p>Action complete: ' + message + '</p>');
                $('#messages p').jTypeWriter();
            }
        }

        function removeParamFromURL(URL, param) {
            URL = String(URL);
            var regex = new RegExp("\\?" + param + "=[^&]*&?", "gi");
            URL = URL.replace(regex, '?');
            regex = new RegExp("\\&" + param + "=[^&]*&?", "gi");
            URL = URL.replace(regex, '&');
            URL = URL.replace(/(\?|&)$/, '');
            regex = null;
            return URL;
        }

        function addParamToURL(URL, param, value) {
            URL = removeParamFromURL(URL, param);
            URL = URL + '&' + param + '=' + value
            if (!(/\?/.test(URL))) URL = URL.replace(/&/, '?');
            return URL;
        }

        function refreshPage(SiteId, NodeId, MsgTxt, SitesHTML) {
            var redirecturl = removeParamFromURL(window.location, 'message');
            redirecturl = addParamToURL(redirecturl, 'NodeId', NodeId);
            redirecturl = addParamToURL(redirecturl, 'SiteId', SiteId);
            redirecturl = addParamToURL(redirecturl, 'Msg', $.url.encode(MsgTxt));
            window.location = redirecturl;
        }

        function updateMessage() {
            var theurl = window.location.search;
            try {
                var theMessage = $.getUrlVar(theurl, 'Msg');
            }
            catch (e) {
                var theMessage = 'none';
            }
            if (theMessage != null) {
                theMessage = $.url.decode(theMessage);
                showMessage(theMessage);
            }
            else {
                showMessage('');
            }
        }
        //Context menu helper/utils functions

        //Cut and Copy Buffer
        var actionbuffer = { 'siteid': '0', 'nodeid': '0', 'action': '' };
        function buffernode(sid, nid, act) {
            actionbuffer = { 'siteid': sid, 'nodeid': nid, 'action': act };
            if (parseInt(actionbuffer.siteid) === 0 && parseInt(actionbuffer.nodeid) === 0) {
                $('#admincontext').disableContextMenuItems('#paste');
                $('.admincontext .paste').css('display', 'none');
                return false;
            }
            else {
                $('#admincontext').enableContextMenuItems('#paste');
                $('.admincontext .paste').css('display', 'block');
                return true;
            }
        }

        //End Url Parameter function (jQuery Plugin)
        $(function(theurl) {
            $.extend({
                getUrlVars: function(theurl) {
                    var vars = [], hash;
                    var hashes = theurl.slice(theurl.indexOf('?') + 1).split('&');

                    for (var i = 0; i < hashes.length; i++) {
                        hash = hashes[i].split('=');
                        vars.push(hash[0]);
                        vars[hash[0]] = hash[1];
                    }

                    return vars;
                },
                getUrlVar: function(theurl, name) {
                    return $.getUrlVars(theurl)[name];
                }
            });
        });
        //End Url Parameter function (jQuery Plugin)


        $(document).ready(function() {

            //UI Buttons
            $("input:submit, div[id='header'] a, div[id='navigationbar'] a, .tbl a:not([id*='Up'],[id*='Down']), a[id*='NewSection'], a[id*='AddTemplate']").button();
            $("input,textarea,select,.group,fieldset,legend,#moduleadminpane .AspNet-GridView a[id*='btnSelect'],#moduleadminpane .AspNet-GridView a[id*='btnEdit'],#moduleadminpane .AspNet-GridView a[id*='btnUpdate'],#moduleadminpane .AspNet-GridView a[id*='btnCancel'],#moduleadminpane .AspNet-GridView a[id*='btnDelete']").addClass("ui-corner-all");

            buffernode(actionbuffer.siteid, actionbuffer.nodeid);
            updateMessage();

            siteTree();
            contextMenu();

            $('.admincontext').mouseleave(function() {
                $('.admincontext').fadeOut('fast');
            });

        });

    </script>

</head>
<body>
    <form id="Frm" method="post" runat="server" enctype="multipart/form-data">
    <uc1:Header ID="Header" runat="server"></uc1:Header>
    <div id="messages" class="ui-widget-header"></div>
    <div id="navbar">
        <uc1:NavigationBar ID="NavigationBar" runat="server"></uc1:NavigationBar>
    </div>
    <div id="adminwrapper">
        <div id="contentpane">
            <div id="padding" class="cleanpad8">
            <div id="pad" class="min550">
                <h1><asp:Literal ID="PageTitleLabel" runat="server" /></h1>
                <div id="MessageBox" class="messagebox" runat="server" visible="false" enableviewstate="false">
                </div>
                <asp:PlaceHolder ID="PageContent" runat="server"></asp:PlaceHolder>
            </div>
            </div>
        </div>
        <div id="menupane">
            <div id="menumodal">
                <div id="progress">
                </div>
            </div>
            <div id="padding2" class="cleanpad8">
            <div id="pad" class="min550">
                <uc1:Navigation ID="Nav" runat="server"></uc1:Navigation>
                <asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
            </div>
            </div>
        </div>
    </div>
    <ul id="admincontext" class="admincontext">
        <optgroup label="Editing">
            <li class="new"><a href="#new">New</a></li>
            <li class="cut"><a href="#cut">Cut</a></li>
            <li class="copy"><a href="#copy">Copy</a></li>
            <li class="paste"><a href="#paste">Paste</a></li>
            <li class="delete"><a href="#delete">Delete</a></li>
        </optgroup>
        <optgroup label="Ordering">
            <li class="moveup"><a href="#moveup">Move Up</a></li>
            <li class="movedown"><a href="#movedown">Move Down</a></li>
            <li class="moveright"><a href="#moveright">Move In</a></li>
            <li class="moveleft"><a href="#moveleft">Move Out</a></li>
        </optgroup>
    </ul>
    </form>
</body>
</html>
