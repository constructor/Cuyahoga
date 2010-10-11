INSERT INTO cuyahoga_moduletype (name, assemblyname, classname, path, editpath, inserttimestamp, updatetimestamp)
VALUES 
( 'Menu', 'Cuyahoga.Modules.Menu', 'Cuyahoga.Modules.Menu.MenuModule', 'Modules/Menu/MenuControl.ascx', '', '2008-02-22', '2008-02-22');

SELECT @moduletypeid := last_insert_id();


INSERT INTO cuyahoga_modulesetting 
( moduletypeid,name,friendlyname,settingdatatype,iscustomtype,isrequired) 
VALUES 
( @moduletypeid, 'FIRST_LEVEL', 'First Level [0 = Home, 1 = 1st level]', 'System.Int16', 0, 1 );

INSERT INTO cuyahoga_modulesetting  
( moduletypeid,name,friendlyname,settingdatatype,iscustomtype,isrequired) 
VALUES 
(@moduletypeid, 'LAST_LEVEL', 'Last Level [-1 = render all]', 'System.Int16', 0, 1);

INSERT INTO cuyahoga_modulesetting 
( moduletypeid,name,friendlyname,settingdatatype,iscustomtype,isrequired)  
VALUES 
(@moduletypeid, 'TYPE_RENDER', 'Type of Render', 'Cuyahoga.Modules.Menu.TypeRender', 1, 1);

INSERT INTO cuyahoga_modulesetting 
( moduletypeid,name,friendlyname,settingdatatype,iscustomtype,isrequired) 
VALUES 
(@moduletypeid, 'CHILDREN_ACTNODE', 'Show Children of Active Only', 'System.Boolean', 0, 1);

INSERT INTO cuyahoga_modulesetting 
( moduletypeid,name,friendlyname,settingdatatype,iscustomtype,isrequired) 
VALUES 
(@moduletypeid, 'REQUIRES_JQUERY', 'Requires jQuery', 'System.Boolean', 0, 1);


INSERT INTO cuyahoga_version
( assembly,major,minor,patch) 
VALUES 
('Cuyahoga.Modules.Menu', 2, 0, 0);