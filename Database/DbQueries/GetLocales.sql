SELECT * from LocaleStringResource where ResourceName like 
--'Account.ChangePassword.Fields.%'  --and resourcename not like 'Web.Admin%'
'Web.Administration.Configuration.EmailAccounts.Added%'
and ResourceName Not like 'Web.Administration.Configuration.EmailAccounts.Fields.%.Hint'
order by updatedon desc

/*insert into ecentral.dbo.LocaleStringResource( LanguageId, ResourceName, ResourceValue, IsJsonResource, CreatedOn, UpdatedOn)
SELECT 'C668E5F7-D174-4F09-B06C-4E3735A1BB4E', Replace(ResourceName,'Web.Administration.',''), ResourceValue,
0, dbo.fnGetGMTDateTime(GETDATE()), dbo.fnGetGMTDateTime(GETDATE())
from LocaleStringResource where ResourceName like 
--'Account.ChangePassword.Fields.%'  --and resourcename not like 'Web.Admin%'
'Web.Administration.Configuration.EmailAccounts.Fields%'
and ResourceName Not like 'Web.Administration.Configuration.EmailAccounts.Fields.%.Hint'*/