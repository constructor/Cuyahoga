CREATE TABLE cuyahoga_contentitem ( 
    contentitemid bigserial NOT NULL, 
    globalid varchar(255) NOT NULL, 
    workflowstatus integer NOT NULL, 
    title varchar(255) NOT NULL, 
    description varchar(255) NULL, 
    version integer NOT NULL, 
    locale varchar(5) NULL, 
    syndicate boolean NOT NULL DEFAULT true, 
    createdat timestamp DEFAULT current_timestamp NOT NULL, 
    modifiedat timestamp DEFAULT current_timestamp NOT NULL, 
    publishedat timestamp NULL, 
    publisheduntil timestamp NULL, 
    createdby integer NOT NULL, 
    modifiedby integer NOT NULL, 
    publishedby integer NULL, 
    sectionid integer NOT NULL, 
    CONSTRAINT PK_contentitem PRIMARY KEY (contentitemid)
);

CREATE TABLE cuyahoga_contentitemrole ( 
    contentitemroleid serial NOT NULL, 
    contentitemid integer NOT NULL, 
    roleid integer NOT NULL, 
    viewallowed boolean NOT NULL, 
    editallowed boolean NOT NULL, 
    CONSTRAINT PK_contentitemrole PRIMARY KEY (contentitemroleid)
);

CREATE UNIQUE INDEX IX_contentitemrole_roleid_contentitemid ON cuyahoga_contentitemrole (roleid,contentitemid);

CREATE TABLE cuyahoga_category ( 
    categoryid serial NOT NULL, 
    siteid integer NOT NULL, 
    parentcategoryid integer NULL, 
    path varchar(80) NOT NULL, 
    categoryname varchar(100) NOT NULL, 
    description varchar(255) NULL, 
    position integer NOT NULL, 
    CONSTRAINT PK_category PRIMARY KEY (categoryid)
);

CREATE UNIQUE INDEX IX_category_categoryname_siteid ON cuyahoga_category (categoryname,siteid);

CREATE UNIQUE INDEX IX_category_path_siteid ON cuyahoga_category (path,siteid);

CREATE TABLE cuyahoga_categorycontentitem ( 
    categorycontentitemid serial NOT NULL, 
    categoryid integer NOT NULL, 
    contentitemid bigint NOT NULL, 
    CONSTRAINT PK_categorycontentitem PRIMARY KEY (categorycontentitemid)
);

CREATE TABLE cuyahoga_comment ( 
    commentid serial NOT NULL, 
    contentitemid bigint NOT NULL, 
    userid integer NULL, 
    commentdatetime timestamp DEFAULT current_timestamp NOT NULL, 
    name varchar(100) NULL, 
    website varchar(100) NULL, 
    commenttext varchar(2000) NOT NULL, 
    userip varchar(15) NULL, 
    CONSTRAINT PK_comment PRIMARY KEY (commentid)
);

CREATE TABLE cuyahoga_fileresource ( 
    fileresourceid bigint NOT NULL, 
    filename varchar(255) NOT NULL, 
    physicalfilepath varchar(1000) NOT NULL, 
    length bigint NULL, 
    mimetype varchar(255) NULL, 
    downloadcount integer NULL, 
    CONSTRAINT PK_fileresource PRIMARY KEY (fileresourceid)
);

CREATE TABLE cuyahoga_user ( 
    userid serial NOT NULL, 
    siteid integer NULL, 
    username varchar(50) NOT NULL, 
    password varchar(100) NOT NULL, 
    firstname varchar(100) NULL, 
    lastname varchar(100) NULL, 
    email varchar(100) NOT NULL, 
    website varchar(100) NULL, 
    timezone integer NOT NULL DEFAULT 0, 
    isactive boolean NULL, 
    lastlogin timestamp NULL, 
    lastip varchar(40) NULL, 
    inserttimestamp timestamp DEFAULT current_timestamp NOT NULL, 
    updatetimestamp timestamp DEFAULT current_timestamp NOT NULL, 
    CONSTRAINT PK_user PRIMARY KEY (userid)
);

ALTER TABLE cuyahoga_user ADD CONSTRAINT UC_user_username UNIQUE (username);

CREATE TABLE cuyahoga_siteuser ( 
    siteid integer NOT NULL, 
    userid integer NOT NULL, 
    CONSTRAINT PK_cuyahoga_siteuser PRIMARY KEY (siteid, userid)
);

CREATE TABLE cuyahoga_role ( 
    roleid serial NOT NULL, 
    name varchar(50) NOT NULL, 
    isglobal boolean NOT NULL DEFAULT true, 
    inserttimestamp timestamp DEFAULT current_timestamp NOT NULL, 
    updatetimestamp timestamp DEFAULT current_timestamp NOT NULL, 
    CONSTRAINT PK_role PRIMARY KEY (roleid)
);
ALTER TABLE cuyahoga_role ADD CONSTRAINT UC_role_name UNIQUE (name);

CREATE TABLE cuyahoga_userrole ( 
    userroleid serial NOT NULL, 
    userid integer NOT NULL, 
    roleid integer NOT NULL, 
    CONSTRAINT PK_userrole PRIMARY KEY (userroleid)
);


CREATE TABLE cuyahoga_right ( 
    rightid serial NOT NULL, 
    name varchar(50) NOT NULL, 
    description varchar(255) NULL, 
    inserttimestamp timestamp DEFAULT current_timestamp NOT NULL, 
    CONSTRAINT PK_right PRIMARY KEY (rightid)
);

CREATE TABLE cuyahoga_roleright ( 
    roleid integer NOT NULL, 
    rightid integer NOT NULL, 
    CONSTRAINT PK_roleright PRIMARY KEY (roleid, rightid)
);

CREATE TABLE cuyahoga_template ( 
    templateid serial NOT NULL, 
    siteid integer NULL, 
    name varchar(100) NOT NULL, 
    basepath varchar(100) NOT NULL, 
    templatecontrol varchar(50) NOT NULL, 
    css varchar(100) NOT NULL, 
    inserttimestamp timestamp DEFAULT current_timestamp NOT NULL, 
    updatetimestamp timestamp DEFAULT current_timestamp NOT NULL, 
    CONSTRAINT PK_template PRIMARY KEY (templateid)
);

CREATE TABLE cuyahoga_moduletype ( 
    moduletypeid serial NOT NULL, 
    name varchar(100) NOT NULL, 
    assemblyname varchar(100) NULL, 
    classname varchar(255) NOT NULL, 
    path varchar(255) NOT NULL, 
    editpath varchar(255) NULL, 
    autoactivate boolean NOT NULL DEFAULT true, 
    inserttimestamp timestamp DEFAULT current_timestamp NOT NULL, 
    updatetimestamp timestamp DEFAULT current_timestamp NOT NULL, 
    CONSTRAINT PK_moduletype PRIMARY KEY (moduletypeid)
);

ALTER TABLE cuyahoga_moduletype ADD CONSTRAINT UC_moduletype_classname UNIQUE (classname);

CREATE TABLE cuyahoga_modulesetting ( 
    modulesettingid serial NOT NULL, 
    moduletypeid integer NOT NULL, 
    name varchar(100) NOT NULL, 
    friendlyname varchar(255) NOT NULL, 
    settingdatatype varchar(100) NOT NULL, 
    iscustomtype boolean NOT NULL, 
    isrequired boolean NOT NULL, 
    CONSTRAINT PK_modulesetting PRIMARY KEY (modulesettingid)
);

CREATE UNIQUE INDEX IX_modulesetting_moduletypeid_name ON cuyahoga_modulesetting (moduletypeid,name);

CREATE TABLE cuyahoga_moduleservice ( 
    moduleserviceid serial NOT NULL, 
    moduletypeid integer NOT NULL, 
    servicekey varchar(50) NOT NULL, 
    servicetype varchar(255) NOT NULL, 
    classtype varchar(255) NOT NULL, 
    lifestyle varchar(10) NULL, 
    CONSTRAINT PK_moduleservice PRIMARY KEY (moduleserviceid)
);

CREATE UNIQUE INDEX IX_moduleservice_moduletypeid_servicekey ON cuyahoga_moduleservice (moduletypeid,servicekey);

CREATE TABLE cuyahoga_site ( 
    siteid serial NOT NULL, 
    templateid integer NULL, 
    roleid integer NOT NULL, 
    name varchar(100) NOT NULL, 
    homeurl varchar(100) NOT NULL, 
    defaultculture varchar(8) NOT NULL, 
    defaultplaceholder varchar(100) NULL, 
    webmasteremail varchar(100) NOT NULL, 
    usefriendlyurls boolean NULL, 
    metakeywords varchar(500) NULL, 
    metadescription varchar(500) NULL, 
    inserttimestamp timestamp DEFAULT current_timestamp NOT NULL, 
    updatetimestamp timestamp DEFAULT current_timestamp NOT NULL, 
    CONSTRAINT PK_site PRIMARY KEY (siteid)
);

ALTER TABLE cuyahoga_site ADD CONSTRAINT UC_site_name UNIQUE (name);

CREATE TABLE cuyahoga_siterole ( 
    siteid integer NOT NULL, 
    roleid integer NOT NULL, 
    CONSTRAINT PK_siterole PRIMARY KEY (siteid, roleid)
);

CREATE TABLE cuyahoga_node ( 
    nodeid serial NOT NULL, 
    parentnodeid integer NULL, 
    templateid integer NULL, 
    siteid integer NOT NULL, 
    title varchar(255) NOT NULL, 
    titleseo varchar(255) NULL, 
    shortdescription varchar(255) NOT NULL, 
    position integer NOT NULL DEFAULT 0, 
    culture varchar(8) NOT NULL, 
    showinnavigation boolean NOT NULL, 
    linkurl varchar(255) NULL, 
    linktarget integer NULL, 
    metakeywords varchar(500) NULL, 
    metadescription varchar(500) NULL, 
    cssclass varchar(128) NULL, 
    inserttimestamp timestamp DEFAULT current_timestamp NOT NULL, 
    updatetimestamp timestamp DEFAULT current_timestamp NOT NULL, 
    CONSTRAINT PK_node PRIMARY KEY (nodeid)
);

CREATE UNIQUE INDEX IX_node_shortdescription_siteid ON cuyahoga_node (shortdescription,siteid);

CREATE TABLE cuyahoga_sitealias ( 
    sitealiasid serial NOT NULL, 
    siteid integer NOT NULL, 
    nodeid integer NULL, 
    url varchar(100) NOT NULL, 
    inserttimestamp timestamp DEFAULT current_timestamp NOT NULL, 
    updatetimestamp timestamp DEFAULT current_timestamp NOT NULL, 
    CONSTRAINT PK_sitealias PRIMARY KEY (sitealiasid)
);

CREATE TABLE cuyahoga_section ( 
    sectionid serial NOT NULL, 
    siteid integer NOT NULL, 
    nodeid integer NULL, 
    moduletypeid integer NOT NULL, 
    title varchar(100) NOT NULL, 
    cssclass varchar(100) NULL, 
    showtitle boolean NOT NULL DEFAULT true, 
    placeholder varchar(100) NULL, 
    position integer NOT NULL DEFAULT 0, 
    cacheduration integer NULL, 
    inserttimestamp timestamp DEFAULT current_timestamp NOT NULL, 
    updatetimestamp timestamp DEFAULT current_timestamp NOT NULL, 
    CONSTRAINT PK_section PRIMARY KEY (sectionid)
);

CREATE TABLE cuyahoga_sectionsetting ( 
    sectionsettingid serial NOT NULL, 
    sectionid integer NOT NULL, 
    name varchar(100) NOT NULL, 
    value varchar(100) NULL, 
    CONSTRAINT PK_sectionsetting PRIMARY KEY (sectionsettingid)
);

CREATE UNIQUE INDEX IX_sectionsetting_sectionid_name ON cuyahoga_sectionsetting (sectionid,name);

CREATE TABLE cuyahoga_sectionconnection ( 
    sectionconnectionid serial NOT NULL, 
    sectionidfrom integer NOT NULL, 
    sectionidto integer NOT NULL, 
    actionname varchar(50) NOT NULL, 
    CONSTRAINT PK_sectionconnection PRIMARY KEY (sectionconnectionid)
);

CREATE UNIQUE INDEX IX_sectionconnection_sectionidfrom_actionname ON cuyahoga_sectionconnection (sectionidfrom,actionname);

CREATE TABLE cuyahoga_templatesection ( 
    templatesectionid serial NOT NULL, 
    templateid integer NOT NULL, 
    sectionid integer NOT NULL, 
    placeholder varchar(100) NOT NULL, 
    CONSTRAINT PK_templatesection PRIMARY KEY (templatesectionid)
);

CREATE UNIQUE INDEX IX_templatesection_templateidid_placeholder ON cuyahoga_templatesection (templateid,placeholder);

CREATE TABLE cuyahoga_noderole ( 
    noderoleid serial NOT NULL, 
    nodeid integer NOT NULL, 
    roleid integer NOT NULL, 
    viewallowed boolean NOT NULL, 
    editallowed boolean NOT NULL, 
    CONSTRAINT PK_noderole PRIMARY KEY (noderoleid)
);

CREATE UNIQUE INDEX IX_noderole_nodeid_roleid ON cuyahoga_noderole (nodeid,roleid);

CREATE TABLE cuyahoga_sectionrole ( 
    sectionroleid serial NOT NULL, 
    sectionid integer NOT NULL, 
    roleid integer NOT NULL, 
    viewallowed boolean NOT NULL, 
    editallowed boolean NOT NULL, 
    CONSTRAINT PK_sectionrole PRIMARY KEY (sectionroleid)
);

CREATE UNIQUE INDEX IX_sectionrole_roleid_sectionid ON cuyahoga_sectionrole (roleid,sectionid);

CREATE TABLE cuyahoga_version ( 
    versionid serial NOT NULL, 
    assembly varchar(255) NOT NULL, 
    major integer NOT NULL, 
    minor integer NOT NULL, 
    patch integer NOT NULL, 
    CONSTRAINT PK_version PRIMARY KEY (versionid)
);





ALTER TABLE cuyahoga_contentitem
ADD CONSTRAINT FK_contentitem_user_createdby 
FOREIGN KEY (createdby) REFERENCES cuyahoga_user (userid) ON DELETE NO ACTION ON UPDATE NO ACTION;


ALTER TABLE cuyahoga_contentitem
ADD CONSTRAINT FK_contentitem_user_modifiedby 
FOREIGN KEY (modifiedby) REFERENCES cuyahoga_user (userid) ON DELETE NO ACTION ON UPDATE NO ACTION;


ALTER TABLE cuyahoga_contentitem
ADD CONSTRAINT FK_contentitem_user_publishedby 
FOREIGN KEY (publishedby) REFERENCES cuyahoga_user (userid) ON DELETE NO ACTION ON UPDATE NO ACTION;


ALTER TABLE cuyahoga_contentitem
ADD CONSTRAINT FK_contentitem_section_sectionid 
FOREIGN KEY (sectionid) REFERENCES cuyahoga_section (sectionid) ON DELETE NO ACTION ON UPDATE NO ACTION;


ALTER TABLE cuyahoga_category
ADD CONSTRAINT FK_category_category_parentcategoryid 
FOREIGN KEY (parentcategoryid) REFERENCES cuyahoga_category (categoryid) ON DELETE NO ACTION ON UPDATE NO ACTION;


ALTER TABLE cuyahoga_category
ADD CONSTRAINT FK_category_site_siteid 
FOREIGN KEY (siteid) REFERENCES cuyahoga_site (siteid) ON DELETE NO ACTION ON UPDATE NO ACTION;


ALTER TABLE cuyahoga_categorycontentitem
ADD CONSTRAINT FK_categorycontentitem_contentitem_contentitemid
FOREIGN KEY (contentitemid) REFERENCES cuyahoga_contentitem (contentitemid) ON DELETE NO ACTION ON UPDATE NO ACTION;


ALTER TABLE cuyahoga_categorycontentitem
ADD CONSTRAINT FK_categorycontentitem_category_categoryid
FOREIGN KEY (categoryid) REFERENCES cuyahoga_category (categoryid) ON DELETE NO ACTION ON UPDATE NO ACTION;


ALTER TABLE cuyahoga_comment
ADD CONSTRAINT FK_comment_contentitem_contentitemid
FOREIGN KEY (contentitemid) REFERENCES cuyahoga_contentitem (contentitemid) ON DELETE NO ACTION ON UPDATE NO ACTION;


ALTER TABLE cuyahoga_comment
ADD CONSTRAINT FK_comment_user_userid
FOREIGN KEY (userid) REFERENCES cuyahoga_user (userid) ON DELETE NO ACTION ON UPDATE NO ACTION;


ALTER TABLE cuyahoga_fileresource
ADD CONSTRAINT FK_fileresource_contentitem_fileresourceid 
FOREIGN KEY (fileresourceid) REFERENCES cuyahoga_contentitem (contentitemid) ON DELETE NO ACTION ON UPDATE NO ACTION;


ALTER TABLE cuyahoga_userrole
ADD CONSTRAINT FK_userrole_role_roleid
FOREIGN KEY (roleid) REFERENCES cuyahoga_role (roleid) ON DELETE NO ACTION ON UPDATE NO ACTION;


ALTER TABLE cuyahoga_userrole
ADD CONSTRAINT FK_user_userid
FOREIGN KEY (userid) REFERENCES cuyahoga_user (userid) ON DELETE NO ACTION ON UPDATE NO ACTION;


ALTER TABLE cuyahoga_roleright
ADD CONSTRAINT FK_roleright_role_roleid 
FOREIGN KEY (roleid) REFERENCES cuyahoga_role (roleid) ON DELETE NO ACTION ON UPDATE NO ACTION;


ALTER TABLE cuyahoga_roleright
ADD CONSTRAINT FK_roleright_right_rightid 
FOREIGN KEY (rightid) REFERENCES cuyahoga_right (rightid) ON DELETE NO ACTION ON UPDATE NO ACTION;


ALTER TABLE cuyahoga_modulesetting
ADD CONSTRAINT FK_modulesetting_moduletype_moduletypeid
FOREIGN KEY (moduletypeid) REFERENCES cuyahoga_moduletype (moduletypeid) ON DELETE NO ACTION ON UPDATE NO ACTION;


ALTER TABLE cuyahoga_moduleservice
ADD CONSTRAINT FK_moduleservice_moduletype_moduletypeid
FOREIGN KEY (moduletypeid) REFERENCES cuyahoga_moduletype (moduletypeid) ON DELETE NO ACTION ON UPDATE NO ACTION;


ALTER TABLE cuyahoga_site
ADD CONSTRAINT FK_site_role_roleid
FOREIGN KEY (roleid) REFERENCES cuyahoga_role (roleid) ON DELETE NO ACTION ON UPDATE NO ACTION;


ALTER TABLE cuyahoga_site
ADD CONSTRAINT FK_site_template_templateid
FOREIGN KEY (templateid) REFERENCES cuyahoga_template (templateid) ON DELETE NO ACTION ON UPDATE NO ACTION;


ALTER TABLE cuyahoga_siterole
ADD CONSTRAINT FK_siterole_site_siteid 
FOREIGN KEY (siteid) REFERENCES cuyahoga_site (siteid) ON DELETE NO ACTION ON UPDATE NO ACTION;


ALTER TABLE cuyahoga_siterole
ADD CONSTRAINT FK_siterole_role_roleid 
FOREIGN KEY (roleid) REFERENCES cuyahoga_role (roleid) ON DELETE NO ACTION ON UPDATE NO ACTION;


ALTER TABLE cuyahoga_node
ADD CONSTRAINT FK_node_node_parentnodeid 
FOREIGN KEY (parentnodeid) REFERENCES cuyahoga_node (nodeid) ON DELETE NO ACTION ON UPDATE NO ACTION;


ALTER TABLE cuyahoga_node
ADD CONSTRAINT FK_node_site_siteid
FOREIGN KEY (siteid) REFERENCES cuyahoga_site (siteid) ON DELETE NO ACTION ON UPDATE NO ACTION;


ALTER TABLE cuyahoga_node
ADD CONSTRAINT FK_node_template_templateid
FOREIGN KEY (templateid) REFERENCES cuyahoga_template (templateid) ON DELETE NO ACTION ON UPDATE NO ACTION;


ALTER TABLE cuyahoga_sitealias
ADD CONSTRAINT FK_sitealias_node_nodeid
FOREIGN KEY (nodeid) REFERENCES cuyahoga_node (nodeid) ON DELETE NO ACTION ON UPDATE NO ACTION;


ALTER TABLE cuyahoga_sitealias
ADD CONSTRAINT FK_sitealias_site_siteid
FOREIGN KEY (siteid) REFERENCES cuyahoga_site (siteid) ON DELETE NO ACTION ON UPDATE NO ACTION;


ALTER TABLE cuyahoga_section
ADD CONSTRAINT FK_section_site_siteid
FOREIGN KEY (siteid) REFERENCES cuyahoga_site (siteid) ON DELETE NO ACTION ON UPDATE NO ACTION;


ALTER TABLE cuyahoga_section
ADD CONSTRAINT FK_section_moduletype_moduletypeid
FOREIGN KEY (moduletypeid) REFERENCES cuyahoga_moduletype (moduletypeid) ON DELETE NO ACTION ON UPDATE NO ACTION;


ALTER TABLE cuyahoga_section
ADD CONSTRAINT FK_section_node_nodeid
FOREIGN KEY (nodeid) REFERENCES cuyahoga_node (nodeid) ON DELETE NO ACTION ON UPDATE NO ACTION;



ALTER TABLE cuyahoga_sectionsetting
ADD CONSTRAINT FK_sectionsetting_section_sectionid
FOREIGN KEY (sectionid) REFERENCES cuyahoga_section (sectionid) ON DELETE NO ACTION ON UPDATE NO ACTION;


ALTER TABLE cuyahoga_sectionconnection
ADD CONSTRAINT FK_sectionconnection_section_sectionidfrom
FOREIGN KEY (sectionidfrom) REFERENCES cuyahoga_section (sectionid) ON DELETE NO ACTION ON UPDATE NO ACTION;


ALTER TABLE cuyahoga_sectionconnection
ADD CONSTRAINT FK_sectionconnection_section_sectionidto
FOREIGN KEY (sectionidto) REFERENCES cuyahoga_section (sectionid) ON DELETE NO ACTION ON UPDATE NO ACTION;


ALTER TABLE cuyahoga_template
ADD CONSTRAINT FK_template_site_siteid
FOREIGN KEY(siteid) REFERENCES cuyahoga_site(siteid) ON DELETE NO ACTION ON UPDATE NO ACTION;


ALTER TABLE cuyahoga_templatesection
ADD CONSTRAINT FK_templatesection_template_templateid
FOREIGN KEY (templateid) REFERENCES cuyahoga_template (templateid) ON DELETE NO ACTION ON UPDATE NO ACTION;


ALTER TABLE cuyahoga_templatesection
ADD CONSTRAINT FK_templatesection_section_sectionid
FOREIGN KEY (sectionid) REFERENCES cuyahoga_section (sectionid) ON DELETE NO ACTION ON UPDATE NO ACTION;


ALTER TABLE cuyahoga_noderole
ADD CONSTRAINT FK_noderole_node_nodeid
FOREIGN KEY (nodeid) REFERENCES cuyahoga_node (nodeid) ON DELETE NO ACTION ON UPDATE NO ACTION;


ALTER TABLE cuyahoga_noderole
ADD CONSTRAINT FK_noderole_role_roleid
FOREIGN KEY (roleid) REFERENCES cuyahoga_role (roleid) ON DELETE NO ACTION ON UPDATE NO ACTION;


ALTER TABLE cuyahoga_sectionrole
ADD CONSTRAINT FK_sectionrole_role_roleid
FOREIGN KEY (roleid) REFERENCES cuyahoga_role (roleid) ON DELETE NO ACTION ON UPDATE NO ACTION;


ALTER TABLE cuyahoga_sectionrole
ADD CONSTRAINT FK_sectionrole_section_sectionid
FOREIGN KEY (sectionid) REFERENCES cuyahoga_section (sectionid) ON DELETE NO ACTION ON UPDATE NO ACTION;













INSERT INTO cuyahoga_role (roleid, name, isglobal, inserttimestamp, updatetimestamp) 
VALUES (1,'Administrator',true,'2004-01-04 16:33:42','2004-09-19 17:08:47');

INSERT INTO cuyahoga_role (roleid, name, isglobal, inserttimestamp, updatetimestamp) 
VALUES (2,'Site Administrator',true,'2004-01-04 16:33:42','2004-09-19 17:08:47');
 
INSERT INTO cuyahoga_role (roleid, name, isglobal, inserttimestamp, updatetimestamp) 
VALUES (3,'Editor',true,'2004-01-04 16:34:26','2004-06-25 00:59:08');
 
INSERT INTO cuyahoga_role (roleid, name, isglobal, inserttimestamp, updatetimestamp) 
VALUES (4,'Authenticated User',true,'2004-01-04 16:34:50','2004-06-25 00:59:03');
 
INSERT INTO cuyahoga_role (roleid, name, isglobal, inserttimestamp, updatetimestamp) 
VALUES (5,'Anonymous User',true,'2004-01-04 16:35:11','2004-07-16 21:18:09');






INSERT INTO cuyahoga_right (rightid,name,description,inserttimestamp) VALUES 
 (1,'Access Admin','Access site administration','2010-08-17 14:49:00'),
 (2,'Manage Server','Manage server properties','2010-08-17 14:49:00'),
 (3,'Create Site','Create a new site','2010-08-17 14:49:00'),
 (4,'Manage Site','Manage site properties','2010-08-17 14:49:00'),
 (5,'Manage Templates','Manage templates','2010-08-17 14:49:00'),
 (6,'Manage Users','Manage users and roles','2010-08-17 14:49:00'),
 (7,'Manage Modules','Can install and uninstall site modules','2010-08-17 14:49:00'),
 (8,'Global Permissions','Manage permissions that are shared across sites','2010-08-17 14:49:00'),
 (9,'Manage Pages','Create, edit, move and delete pages','2010-08-17 14:49:00'),
 (10,'Manage Sections','Create, edit, move and delete sections within pages','2010-08-17 14:49:00'),
 (11,'Edit Sections','Can content manage sections','2010-08-17 14:49:00'),
 (12,'Manage Files','Manage files','2010-08-17 14:49:00'),
 (13,'Access Root Data Folder','Access root data folder','2010-08-17 14:49:00'),
 (14,'Create Directory','Create a new directory','2010-08-17 14:49:00'),
 (15,'Manage Directories','Move, Rename and Delete Directories','2010-08-17 14:49:00'),
 (16,'Copy Files','Copy files','2010-08-17 14:49:00'),
 (17,'Move Files','Move files','2010-08-17 14:49:00'),
 (18,'Delete Files','Delete files','2010-08-17 14:49:00');
ALTER TABLE cuyahoga_right ADD CONSTRAINT UC_right_name UNIQUE (name);

INSERT INTO cuyahoga_roleright (roleid,rightid) VALUES 
 (1,1),
 (2,1),
 (1,2),
 (2,2),
 (1,3),
 (2,3),
 (1,4),
 (2,4),
 (1,5),
 (2,5),
 (1,6),
 (2,6),
 (1,7),
 (2,7),
 (1,8),
 (2,8),
 (1,9),
 (2,9),
 (3,9),
 (1,10),
 (2,10),
 (3,10),
 (1,11),
 (2,11),
 (3,11),
 (1,12),
 (2,12),
 (3,12),
 (1,13),
 (2,13),
 (1,14),
 (2,14),
 (3,14),
 (1,15),
 (2,15),
 (3,15),
 (1,16),
 (2,16),
 (3,16),
 (1,17),
 (2,17),
 (3,17),
 (1,18),
 (2,18),
 (3,18);
 
 

-- Do not set the index values explicitly. Use the tables own sequence otherwise there will be a discrepancy
-- between the next value in the sequence and the id. This causes a NHibernate.NonUniqueObjectException
-- due to NHibernate trying to enter values with an id that already exists.
-- I used (SELECT nextval('cuyahoga_template_templateid_seq') to get the next id value.

INSERT INTO cuyahoga_template (templateid,siteid,name,basepath,templatecontrol,css,inserttimestamp,updatetimestamp) VALUES 
 ((SELECT nextval('cuyahoga_template_templateid_seq')),NULL,'Cuyahoga','Templates/Cuyahoga','Cuyahoga.ascx','default.css','2010-06-17 17:59:50','2010-08-17 14:49:10'),
 ((SELECT nextval('cuyahoga_template_templateid_seq')),NULL,'Corporate','Templates/Corporate','Corporate.ascx','style.css','2010-01-26 21:52:52','2010-08-17 14:49:10'),
 ((SELECT nextval('cuyahoga_template_templateid_seq')),NULL,'Impact(Droppy)','Templates/Impact(Droppy)','Impact(Droppy).ascx','default.css','2010-01-26 21:52:52','2010-08-17 14:49:10'),
 ((SELECT nextval('cuyahoga_template_templateid_seq')),NULL,'CityLights','Templates/CityLights','CityLights.ascx','default.css','2010-01-26 21:52:52','2010-08-17 14:49:10');

INSERT INTO cuyahoga_version (assembly, major, minor, patch) VALUES ('Cuyahoga.Core', 2, 0, 0);


