/*
 *  Table definition
 */
CREATE TABLE cm_article ( 
    contentitemid bigint NOT NULL, 
    content text NOT NULL,  
    PRIMARY KEY (contentitemid)
);
ALTER TABLE cm_article ADD CONSTRAINT FK_article_contentitem_contentitemid FOREIGN KEY (contentitemid) REFERENCES cuyahoga_contentitem(contentitemid) ON DELETE NO ACTION ON UPDATE NO ACTION;


/*
 *  Module type
 */
INSERT INTO cuyahoga_moduletype (name, assemblyname, classname, path, editpath) 
VALUES ('Articles', 'Cuyahoga.Modules.Articles', 'Cuyahoga.Modules.Articles.ArticleModule', 'Modules/Articles/Articles.ascx', 'Modules/Articles/AdminArticles.aspx');

SELECT @moduletypeid := last_insert_id();

/*
 *  Module settings 
 */
INSERT INTO cuyahoga_modulesetting (moduletypeid, name, friendlyname, settingdatatype, iscustomtype, isrequired) VALUES (@moduletypeid, 'ALLOW_COMMENTS', 'Allow comments', 'System.Boolean', 0, 1);
INSERT INTO cuyahoga_modulesetting (moduletypeid, name, friendlyname, settingdatatype, iscustomtype, isrequired) VALUES (@moduletypeid, 'NUMBER_OF_ARTICLES_IN_LIST', 'Number of articles to display', 'System.Int16', 0, 1);
INSERT INTO cuyahoga_modulesetting (moduletypeid, name, friendlyname, settingdatatype, iscustomtype, isrequired) VALUES (@moduletypeid, 'DISPLAY_TYPE', 'Display type', 'Cuyahoga.Modules.Articles.DisplayType', 1, 1);
INSERT INTO cuyahoga_modulesetting (moduletypeid, name, friendlyname, settingdatatype, iscustomtype, isrequired) VALUES (@moduletypeid, 'ALLOW_ANONYMOUS_COMMENTS', 'Allow anonymous comments', 'System.Boolean', 0, 1);
INSERT INTO cuyahoga_modulesetting (moduletypeid, name, friendlyname, settingdatatype, iscustomtype, isrequired) VALUES (@moduletypeid, 'ALLOW_SYNDICATION', 'Allow syndication', 'System.Boolean', 0, 1);
INSERT INTO cuyahoga_modulesetting (moduletypeid, name, friendlyname, settingdatatype, iscustomtype, isrequired) VALUES (@moduletypeid, 'SHOW_ARCHIVE', 'Show link to archived articles', 'System.Boolean', 0, 1);
INSERT INTO cuyahoga_modulesetting (moduletypeid, name, friendlyname, settingdatatype, iscustomtype, isrequired) VALUES (@moduletypeid, 'SHOW_CATEGORY', 'Show category', 'System.Boolean', 0, 1);
INSERT INTO cuyahoga_modulesetting (moduletypeid, name, friendlyname, settingdatatype, iscustomtype, isrequired) VALUES (@moduletypeid, 'SHOW_DATETIME', 'Show publish date and time', 'System.Boolean', 0, 1);
INSERT INTO cuyahoga_modulesetting (moduletypeid, name, friendlyname, settingdatatype, iscustomtype, isrequired) VALUES (@moduletypeid, 'SHOW_AUTHOR', 'Show author', 'System.Boolean', 0, 1);
INSERT INTO cuyahoga_modulesetting (moduletypeid, name, friendlyname, settingdatatype, iscustomtype, isrequired) VALUES (@moduletypeid, 'SORT_BY', 'Sort by', 'Cuyahoga.Modules.Articles.SortBy', 1, 1);
INSERT INTO cuyahoga_modulesetting (moduletypeid, name, friendlyname, settingdatatype, iscustomtype, isrequired) VALUES (@moduletypeid, 'SORT_DIRECTION', 'Sort direction', 'Cuyahoga.Modules.Articles.SortDirection', 1, 1);


/*
 *  Version data
 */
INSERT INTO cuyahoga_version (assembly, major, minor, patch) VALUES ('Cuyahoga.Modules.Articles', 2, 0, 0);
