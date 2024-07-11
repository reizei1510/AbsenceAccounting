CREATE SEQUENCE RecordIdSequence
    START WITH 1
GO

CREATE TRIGGER RecordInsert
ON dbo.timesheet
AFTER INSERT
AS
BEGIN
    DECLARE @lastId INT
    DECLARE @sql NVARCHAR(MAX)

    SELECT @lastId = ISNULL(MAX(id), 0) FROM dbo.timesheet
   
    SET @sql = 'ALTER SEQUENCE RecordIdSequence RESTART WITH ' + CAST((@lastId + 1) AS NVARCHAR(10))
    EXEC sp_executesql @sql
END
GO

CREATE PROCEDURE GetNextRecordId
AS
BEGIN
    SELECT current_value FROM sys.sequences WHERE name = 'RecordIdSequence'
END
GO