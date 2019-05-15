CREATE TABLE [dbo].[MachineRunResults]
(
	[Id] int IDENTITY(1,1) NOT NULL PRIMARY KEY, 
    [MachineRuns] VARCHAR(MAX) NOT NULL, 
    [RunTime] DATETIME NOT NULL DEFAULT GETDATE(), 
    [CountBrokenMachines] INT NOT NULL DEFAULT 0, 
    [SmashedCount] INT NOT NULL DEFAULT 0, 
    [SlashedCount] INT NOT NULL DEFAULT 0, 
    [TrashedCount] INT NOT NULL DEFAULT 0, 
    [WidgetsDestroyed] INT NOT NULL DEFAULT 0
)
