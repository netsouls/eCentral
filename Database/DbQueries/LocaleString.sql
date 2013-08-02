SELECT * from LocaleStringResource where ResourceName like 
--'Account.ChangePassword.Fields.%'  --and resourcename not like 'Web.Admin%'
'%PageTitle.Clients.Edit%'
order by updatedon desc

/*UPDATE LocaleStringResource SET ResourceName = 'Clients.Form.Heading'
where ResourceName = 'Clients.Form.Add.Heading'
--'Account.ChangePassword.Fields.%'  --and resourcename not like 'Web.Admin%'
'Web.Administration.ContentManagement.ContentPosts%'
*/
--delete from localestringresource where rowid = '5638D6B3-0707-4B55-BFDA-79D7B2257C8E'
--SELECT 'insert into LocaleStringResource (RowId, LanguagaeId, ResourceName, ResourceValue, IsJsonResource) VALUES(NewId(), ''CF1CD9A2-47CE-4C57-9364-2E2F7DFD09FF'', ''' + ResourceName + ''', ''' + CAST(ResourceValue AS VARCHAR(8000)) + ''', ' + CAST(IsJsonResource AS VARCHAR(1)) + ')' from LocaleStringResource
-- update localestringresource set ResourceName = 'BranchOffice.Updated' where Rowid = '9A4387ED-83F4-49DC-BC3E-602109ACA671', UpdatedOn = dbo.fnGetGMTDateTime(GETDATE()) where RowId = '0B484BC2-75A7-4836-895B-5A1D8A56485F'

/*
insert into LocaleStringResource( LanguageId, ResourceName, ResourceValue, IsJsonResource, CreatedOn, UpdatedOn)
VALUES ( 'C668E5F7-D174-4F09-B06C-4E3735A1BB4E',
'Settings.Advanced.Fields.Value',
'Value', --.
0, dbo.fnGetGMTDateTime(GETDATE()), dbo.fnGetGMTDateTime(GETDATE()))
	
*/


--SELECT * FROM LocaleStringResource where ResourceValue like 'Please browse for your custom images to use as your security image.'