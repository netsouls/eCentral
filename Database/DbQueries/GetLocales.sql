SELECT * from LocaleStringResource where ResourceName like 
--'Account.ChangePassword.Fields.%'  --and resourcename not like 'Web.Admin%'
'Admin.Configuration.Countries%'
and ResourceName Not like 'Admin.Configuration.Countries.%.Hint'
--order by updatedon desc

/*
SELECT 'insert into LocaleStringResource( LanguageId, ResourceName, ResourceValue, IsJsonResource, CreatedOn, UpdatedOn) VALUES (''C668E5F7-D174-4F09-B06C-4E3735A1BB4E'',''' + Replace(ResourceName,'Admin.','') + ''',''' + ResourceValue + ''',0,dbo.fnGetGMTDateTime(GETDATE()), dbo.fnGetGMTDateTime(GETDATE()))' 
 from LocaleStringResource where ResourceName like 
'Admin.Configuration.Countries%'
and ResourceName Not like 'Admin.Configuration.Countries.%.Hint'
*/

/*insert into ecentral.dbo.LocaleStringResource( LanguageId, ResourceName, ResourceValue, IsJsonResource, CreatedOn, UpdatedOn)
SELECT 'C668E5F7-D174-4F09-B06C-4E3735A1BB4E', Replace(ResourceName,'Admin.',''), ResourceValue,
0, ecentral.dbo.fnGetGMTDateTime(GETDATE()), ecentral.dbo.fnGetGMTDateTime(GETDATE())
from LocaleStringResource where ResourceName like 
'Admin.Configuration.Countries%'
and ResourceName Not like 'Admin.Configuration.Countries.%.Hint'*/