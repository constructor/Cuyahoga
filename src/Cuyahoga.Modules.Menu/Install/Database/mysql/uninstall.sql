/*
 *  Remove module definitions
 */
DELETE FROM cuyahoga_version WHERE assembly = 'Cuyahoga.Modules.Menu';

/*
 *  Remove module services
 */ 
DELETE FROM cuyahoga_modulesetting
WHERE moduletypeid IN
	(SELECT mt.moduletypeid FROM cuyahoga_moduletype mt WHERE mt.assemblyname = 'Cuyahoga.Modules.Menu');
	
DELETE FROM cuyahoga_moduleservice
WHERE moduletypeid IN
	(SELECT mt.moduletypeid FROM cuyahoga_moduletype mt WHERE mt.assemblyname = 'Cuyahoga.Modules.Menu');

/*
 *  Remove module type
 */ 
DELETE FROM cuyahoga_moduletype
WHERE assemblyname = 'Cuyahoga.Modules.Menu';
