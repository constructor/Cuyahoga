SET DATEFORMAT ymd

DECLARE @moduletypeid int

INSERT INTO cuyahoga_moduletype (name, assemblyname, classname, path, editpath, inserttimestamp, updatetimestamp) 	VALUES ('Menu', 'Cuyahoga.Modules.Menu', 'Cuyahoga.Modules.Menu.MenuModule', 'Modules/Menu/MenuControl.ascx', null, '2008-02-22', '2008-02-22')
SELECT @moduletypeid = Scope_Identity()

INSERT INTO cuyahoga_modulesetting (moduletypeid, name, friendlyname, settingdatatype, iscustomtype, isrequired) 	VALUES (@moduletypeid, 'FIRST_LEVEL', 'First Level [0 = Home, 1 = 1st level]', 'System.Int16', 0, 1)
INSERT INTO cuyahoga_modulesetting (moduletypeid, name, friendlyname, settingdatatype, iscustomtype, isrequired) 	VALUES (@moduletypeid, 'LAST_LEVEL', 'Last Level [-1 = render all]', 'System.Int16', 0, 1)
INSERT INTO cuyahoga_modulesetting (moduletypeid, name, friendlyname, settingdatatype, iscustomtype, isrequired) 	VALUES (@moduletypeid, 'TYPE_RENDER', 'Type of Render', 'Cuyahoga.Modules.Menu.TypeRender', 1, 1)
INSERT INTO cuyahoga_modulesetting (moduletypeid, name, friendlyname, settingdatatype, iscustomtype, isrequired) 	VALUES (@moduletypeid, 'CHILDREN_ACTNODE', 'Show Children of Active Only', 'System.Boolean', 0, 1)
INSERT INTO cuyahoga_modulesetting (moduletypeid, name, friendlyname, settingdatatype, iscustomtype, isrequired) 	VALUES (@moduletypeid, 'REQUIRES_JQUERY', 'Requires jQuery', 'System.Boolean', 0, 1)

INSERT INTO cuyahoga_version (assembly, major, minor, patch) VALUES ('Cuyahoga.Modules.Menu', 2, 0, 0)


--Add Section Info
--SET IDENTITY_INSERT [dbo].[cuyahoga_section] ON
--GO
--INSERT INTO [cuyahoga_section] ([sectionid], [nodeid], [moduletypeid], [title], [showtitle], [placeholder], [position], [cacheduration], [inserttimestamp], [updatetimestamp]) VALUES (6, NULL, 13, N'Menu_Sub', 0, N'', 0, 0, '20090207 23:24:19.813', '20090207 23:24:19.790')
--GO
--SET IDENTITY_INSERT [dbo].[cuyahoga_section] OFF
--GO

--SET IDENTITY_INSERT [cuyahoga_sectionsetting] ON
--GO
--INSERT INTO [cuyahoga_sectionsetting] ([sectionsettingid], [sectionid], [name], [value]) VALUES (11, 6, N'LAST_LEVEL', N'-1')
--INSERT INTO [cuyahoga_sectionsetting] ([sectionsettingid], [sectionid], [name], [value]) VALUES (12, 6, N'TYPE_RENDER', N'NavigationTree')
--INSERT INTO [cuyahoga_sectionsetting] ([sectionsettingid], [sectionid], [name], [value]) VALUES (13, 6, N'FIRST_LEVEL', N'1')
--GO
--SET IDENTITY_INSERT [cuyahoga_sectionsetting] OFF
--GO

--SET IDENTITY_INSERT [dbo].[cuyahoga_sectionrole] ON
--GO
--INSERT INTO [cuyahoga_sectionrole] ([sectionroleid], [sectionid], [roleid], [viewallowed], [editallowed]) VALUES (21, 6, 4, 1, 0)
--INSERT INTO [cuyahoga_sectionrole] ([sectionroleid], [sectionid], [roleid], [viewallowed], [editallowed]) VALUES (22, 6, 3, 1, 0)
--INSERT INTO [cuyahoga_sectionrole] ([sectionroleid], [sectionid], [roleid], [viewallowed], [editallowed]) VALUES (23, 6, 2, 1, 0)
--INSERT INTO [cuyahoga_sectionrole] ([sectionroleid], [sectionid], [roleid], [viewallowed], [editallowed]) VALUES (24, 6, 1, 1, 1)
--GO
--SET IDENTITY_INSERT [cuyahoga_sectionrole] OFF
--GO