use ecentral
SELECT * from LocaleStringResource where ResourceName like 
--'Account.ChangePassword.Fields.%'  --and resourcename not like 'Web.Admin%'
'Configuration.Countries.%'
order by updatedon desc

/*UPDATE LocaleStringResource SET ResourceName = 'Clients.Form.Heading'
where ResourceName = 'Clients.Form.Add.Heading'
--'Account.ChangePassword.Fields.%'  --and resourcename not like 'Web.Admin%'
'Web.Administration.ContentManagement.ContentPosts%'
*/
--delete from localestringresource where rowid IN ( '49F48A14-3F1F-4C32-90B0-20F5132DB1FF','272E85EB-BF29-4F5D-AD75-61FA1C4EBBBD','6476FBCD-6A85-48BB-B898-6965FA401442','4C4319CB-9CCA-4ED0-AD22-6B8D47FA9805','2B8B84B8-1F0B-4B28-92D9-74495EE4D6AE','0C12829D-8816-4EF6-8112-80BA4B040E42','354972EA-48C1-4D53-AA52-95A11F5709B7','F8B02ACC-17D3-483A-BFD3-A62C9F6404DC')
--SELECT 'insert into LocaleStringResource (RowId, LanguagaeId, ResourceName, ResourceValue, IsJsonResource) VALUES(NewId(), ''CF1CD9A2-47CE-4C57-9364-2E2F7DFD09FF'', ''' + ResourceName + ''', ''' + CAST(ResourceValue AS VARCHAR(8000)) + ''', ' + CAST(IsJsonResource AS VARCHAR(1)) + ')' from LocaleStringResource
-- update localestringresource set ResourceName = 'BranchOffice.Updated' where Rowid = '9A4387ED-83F4-49DC-BC3E-602109ACA671', UpdatedOn = dbo.fnGetGMTDateTime(GETDATE()) where RowId = '0B484BC2-75A7-4836-895B-5A1D8A56485F'

/*
insert into LocaleStringResource( LanguageId, ResourceName, ResourceValue, IsJsonResource, CreatedOn, UpdatedOn)
VALUES ( 'C668E5F7-D174-4F09-B06C-4E3735A1BB4E',
'System.Log.Cleared',
'The log has been cleared successfully.', --.
0, dbo.fnGetGMTDateTime(GETDATE()), dbo.fnGetGMTDateTime(GETDATE()))
	
*/


--SELECT * FROM LocaleStringResource where ResourceValue like 'Please browse for your custom images to use as your security image.'