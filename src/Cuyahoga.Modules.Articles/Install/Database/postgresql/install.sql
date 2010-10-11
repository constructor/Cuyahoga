/*
 *  Table definition
 */
CREATE TABLE cm_article ( 
    contentitemid bigint NOT NULL, 
    content text NOT NULL,  
    PRIMARY KEY (contentitemid)
);
ALTER TABLE cm_article ADD CONSTRAINT FK_article_contentitem_contentitemid FOREIGN KEY (contentitemid) REFERENCES cuyahoga_contentitem(contentitemid) ON DELETE NO ACTION ON UPDATE NO ACTION;


-- first set sequence values, otherwise the inserts will fail due to violation of the pk_constraint
SELECT setval('cuyahoga_moduletype_moduletypeid_seq', (SELECT max(moduletypeid) FROM cuyahoga_moduletype));
SELECT setval('cuyahoga_modulesetting_modulesettingid_seq', (SELECT max(modulesettingid) FROM cuyahoga_modulesetting));


/*
 *  Module type
 */
INSERT INTO cuyahoga_moduletype (name, assemblyname, classname, path, editpath) 
VALUES ('Articles', 'Cuyahoga.Modules.Articles', 'Cuyahoga.Modules.Articles.ArticleModule', 'Modules/Articles/Articles.ascx', 'Modules/Articles/AdminArticles.aspx');

/*
 *  Module settings 
 */
INSERT INTO cuyahoga_modulesetting (moduletypeid, name, friendlyname, settingdatatype, iscustomtype, isrequired) VALUES (currval('cuyahoga_moduletype_moduletypeid_seq'), 'ALLOW_COMMENTS', 'Allow comments', 'System.Boolean', FALSE, TRUE);
INSERT INTO cuyahoga_modulesetting (moduletypeid, name, friendlyname, settingdatatype, iscustomtype, isrequired) VALUES (currval('cuyahoga_moduletype_moduletypeid_seq'), 'NUMBER_OF_ARTICLES_IN_LIST', 'Number of articles to display', 'System.Int16', FALSE, TRUE);
INSERT INTO cuyahoga_modulesetting (moduletypeid, name, friendlyname, settingdatatype, iscustomtype, isrequired) VALUES (currval('cuyahoga_moduletype_moduletypeid_seq'), 'DISPLAY_TYPE', 'Display type', 'Cuyahoga.Modules.Articles.DisplayType', TRUE, TRUE);
INSERT INTO cuyahoga_modulesetting (moduletypeid, name, friendlyname, settingdatatype, iscustomtype, isrequired) VALUES (currval('cuyahoga_moduletype_moduletypeid_seq'), 'ALLOW_ANONYMOUS_COMMENTS', 'Allow anonymous comments', 'System.Boolean', FALSE, TRUE);
INSERT INTO cuyahoga_modulesetting (moduletypeid, name, friendlyname, settingdatatype, iscustomtype, isrequired) VALUES (currval('cuyahoga_moduletype_moduletypeid_seq'), 'ALLOW_SYNDICATION', 'Allow syndication', 'System.Boolean', FALSE, TRUE);
INSERT INTO cuyahoga_modulesetting (moduletypeid, name, friendlyname, settingdatatype, iscustomtype, isrequired) VALUES (currval('cuyahoga_moduletype_moduletypeid_seq'), 'SHOW_ARCHIVE', 'Show link to archived articles', 'System.Boolean', FALSE, TRUE);
INSERT INTO cuyahoga_modulesetting (moduletypeid, name, friendlyname, settingdatatype, iscustomtype, isrequired) VALUES (currval('cuyahoga_moduletype_moduletypeid_seq'), 'SHOW_CATEGORY', 'Show category', 'System.Boolean', FALSE, TRUE);
INSERT INTO cuyahoga_modulesetting (moduletypeid, name, friendlyname, settingdatatype, iscustomtype, isrequired) VALUES (currval('cuyahoga_moduletype_moduletypeid_seq'), 'SHOW_DATETIME', 'Show publish date and time', 'System.Boolean', FALSE, TRUE);
INSERT INTO cuyahoga_modulesetting (moduletypeid, name, friendlyname, settingdatatype, iscustomtype, isrequired) VALUES (currval('cuyahoga_moduletype_moduletypeid_seq'), 'SHOW_AUTHOR', 'Show author', 'System.Boolean', FALSE, TRUE);
INSERT INTO cuyahoga_modulesetting (moduletypeid, name, friendlyname, settingdatatype, iscustomtype, isrequired) VALUES (currval('cuyahoga_moduletype_moduletypeid_seq'), 'SORT_BY', 'Sort by', 'Cuyahoga.Modules.Articles.SortBy', TRUE, TRUE);
INSERT INTO cuyahoga_modulesetting (moduletypeid, name, friendlyname, settingdatatype, iscustomtype, isrequired) VALUES (currval('cuyahoga_moduletype_moduletypeid_seq'), 'SORT_DIRECTION', 'Sort direction', 'Cuyahoga.Modules.Articles.SortDirection', TRUE, TRUE);


/*
 *  Version data
 */
INSERT INTO cuyahoga_version (assembly, major, minor, patch) VALUES ('Cuyahoga.Modules.Articles', 2, 0, 0);
