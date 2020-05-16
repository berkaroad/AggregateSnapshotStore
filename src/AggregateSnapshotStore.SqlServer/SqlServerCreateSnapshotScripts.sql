CREATE TABLE [dbo].[AggregateSnapshot] (
    [Sequence]              BIGINT IDENTITY (1, 1) NOT NULL,
    [AggregateRootTypeName] NVARCHAR (256)         NOT NULL,
    [AggregateRootId]       NVARCHAR (36)          NOT NULL,
    [Version]               INT                    NOT NULL,
    [Data]                  VARBINARY(MAX)		   NOT NULL,
    CONSTRAINT [PK_AggregateSnapshot] PRIMARY KEY CLUSTERED ([Sequence] ASC)
)
GO

CREATE UNIQUE INDEX [ix_AggregateRootId] ON [dbo].[AggregateSnapshot] ([AggregateRootId] ASC)
GO
