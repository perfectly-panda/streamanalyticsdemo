CREATE TABLE [dbo].[Orders]
(
	[Id] INT IDENTITY(1,1) PRIMARY KEY,
	[WidgetCount] int NOT NULL DEFAULT(50),
	[CreateDate] DateTime NOT NULL DEFAULT(GETDATE()),
	[PendingCount] INT NOT NULL DEFAULT(0),
	[SmashedCount] INT NOT NULL DEFAULT(0),
	[SlashedCount] INT NOT NULL DEFAULT(0),
	[CompletedCount] INT NOT NULL DEFAULT(0),
	[CompleteDate] DateTime null,
	[Completed] bit not null DEFAULT(0)
)
