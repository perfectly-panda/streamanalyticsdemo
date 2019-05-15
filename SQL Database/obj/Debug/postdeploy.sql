/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]		
			   */
SET IDENTITY_INSERT dbo.Settings ON;  

INSERT INTO dbo.Settings
	(Id
	,SettingName
	,SettingValue)
SELECT
	seed.Id
	,seed.SettingName
	,seed.SettingValue
FROM (
	SELECT 1 AS Id, 'MaxMachines' AS SettingName, 20 AS SettingValue
	UNION SELECT 2 AS Id, 'MaxOrderSize' AS SettingName, 2000 AS SettingValue
	UNION SELECT 3 AS Id, 'MaxActiveOrders' AS SettingName, 10 AS SettingValue
	UNION SELECT 4 AS Id, 'AverageProductionTime' AS SettingName, 25 AS SettingValue
	UNION SELECT 5 AS Id, 'ProductionTimeVariability' AS SettingName, 10 AS SettingValue
	UNION SELECT 6 AS Id, 'AverageFailureRate' AS SettingName, .01 AS SettingValue
	UNION SELECT 7 AS Id, 'FailureRateVariability' AS SettingName, 2 AS SettingValue
	UNION SELECT 8 AS Id, 'MachineFailureRate' AS SettingName, .01 AS SettingValue
	UNION SELECT 9 AS Id, 'OrderRate' AS SettingName, .2 AS SettingValue
	UNION SELECT 10 AS Id, 'BaseMachineRate' AS SettingName, 2 AS SettingValue
) seed
LEFT JOIN dbo.Settings s
	on seed.Id = s.Id
 WHERE s.Id IS NULL

 SET IDENTITY_INSERT dbo.Settings OFF; 
GO
