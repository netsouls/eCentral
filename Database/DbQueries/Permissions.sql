SELECT * FROM Permissions
 WHERE SystemName IN ('ManageSystemLog', 'ManageMessageQueue','ManageMaintenance')
--INSERT INTO Permissions ( Name, SystemName, Category) VALUES ( 'Manage Maintenance', 'ManageMaintenance', 'Configuration')

SELECT * FROM PermissionsInRoles 
 WHERE RoleId = 'CDA6FF15-9293-4DE7-A26E-A04EC233BAA4'
SELECT * FROM UserRoles

SELECT * FROM UsersInRoles
--DELETE FROM UserRoles where RowId= '3B844316-7C9D-4365-B8B0-9F8395E1623A'

/* INSERT INTO PermissionsInRoles SELECT RowId, 'CDA6FF15-9293-4DE7-A26E-A04EC233BAA4'
  FROM Permissions WHERE SystemName IN ('ManageSystemLog', 'ManageMessageQueue','ManageMaintenance')
*/