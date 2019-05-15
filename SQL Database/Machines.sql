CREATE TABLE [dbo].[Machines]
(
	[Id] int Identity(1,1) PRIMARY KEY,
	[MachineType] varchar(50) not null DEFAULT('Smasher'),
	[Broken] bit NOT NULL DEFAULT(0),
	[Variation] int NOT NULL DEFAULT(5),
	[Active] bit NOT NULL DEFAULT(1),
	[CreateDate] DateTime not null DEFAULT(GETDATE())
)
