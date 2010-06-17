DELETE FROM cuyahoga_version WHERE assembly = 'Cuyahoga.Modules.Menu'
go

DELETE cuyahoga_modulesetting 
FROM cuyahoga_modulesetting ms
	INNER JOIN cuyahoga_moduletype mt ON mt.moduletypeid = ms.moduletypeid AND mt.assemblyname = 'Cuyahoga.Modules.Menu'
go

DELETE cuyahoga_moduleservice
FROM cuyahoga_moduleservice ms
	INNER JOIN cuyahoga_moduletype mt ON mt.moduletypeid = ms.moduletypeid AND mt.assemblyname = 'Cuyahoga.Modules.Menu'

DELETE FROM cuyahoga_moduletype
WHERE assemblyname = 'Cuyahoga.Modules.Menu'
go