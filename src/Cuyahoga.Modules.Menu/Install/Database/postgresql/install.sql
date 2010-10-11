-- first set sequence values, otherwise the inserts will fail due to violation of the pk_constraint
SELECT setval('cuyahoga_moduletype_moduletypeid_seq', (SELECT max(moduletypeid) FROM cuyahoga_moduletype));
SELECT setval('cuyahoga_modulesetting_modulesettingid_seq', (SELECT max(modulesettingid) FROM cuyahoga_modulesetting));



INSERT INTO cuyahoga_moduletype (name, assemblyname, classname, path, editpath, inserttimestamp, updatetimestamp)
VALUES 
( 'Menu', 'Cuyahoga.Modules.Menu', 'Cuyahoga.Modules.Menu.MenuModule', 'Modules/Menu/MenuControl.ascx', '', '2008-02-22', '2008-02-22');




INSERT INTO cuyahoga_modulesetting 
( moduletypeid,name,friendlyname,settingdatatype,iscustomtype,isrequired) 
VALUES 
( currval('cuyahoga_moduletype_moduletypeid_seq'), 'FIRST_LEVEL', 'First Level [0 = Home, 1 = 1st level]', 'System.Int16', FALSE, TRUE );

INSERT INTO cuyahoga_modulesetting  
( moduletypeid,name,friendlyname,settingdatatype,iscustomtype,isrequired) 
VALUES 
( currval('cuyahoga_moduletype_moduletypeid_seq'), 'LAST_LEVEL', 'Last Level [-1 = render all]', 'System.Int16', FALSE, TRUE);

INSERT INTO cuyahoga_modulesetting 
( moduletypeid,name,friendlyname,settingdatatype,iscustomtype,isrequired)  
VALUES 
( currval('cuyahoga_moduletype_moduletypeid_seq'), 'TYPE_RENDER', 'Type of Render', 'Cuyahoga.Modules.Menu.TypeRender', TRUE, TRUE);

INSERT INTO cuyahoga_modulesetting 
( moduletypeid,name,friendlyname,settingdatatype,iscustomtype,isrequired) 
VALUES 
( currval('cuyahoga_moduletype_moduletypeid_seq'), 'CHILDREN_ACTNODE', 'Show Children of Active Only', 'System.Boolean', FALSE, TRUE);

INSERT INTO cuyahoga_modulesetting 
( moduletypeid,name,friendlyname,settingdatatype,iscustomtype,isrequired) 
VALUES 
( currval('cuyahoga_moduletype_moduletypeid_seq'), 'REQUIRES_JQUERY', 'Requires jQuery', 'System.Boolean', FALSE, TRUE);




INSERT INTO cuyahoga_version
( assembly,major,minor,patch) 
VALUES 
('Cuyahoga.Modules.Menu', 2, 0, 0);