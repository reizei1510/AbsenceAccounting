USE Absences
GO

CREATE TABLE dbo.employees (
    id INT PRIMARY KEY IDENTITY,
    last_name NVARCHAR(128) NOT NULL,
    first_name NVARCHAR(128) NOT NULL
)
GO

INSERT INTO dbo.employees VALUES ('Иванов', 'Иван')
INSERT INTO dbo.employees VALUES ('Петров', 'Петр')
INSERT INTO dbo.employees VALUES ('Васильев', 'Василий')
INSERT INTO dbo.employees VALUES ('Сергеев', 'Сергей')
INSERT INTO dbo.employees VALUES ('Андреев', 'Андрей')
GO

CREATE TABLE dbo.reasons (
    id INT PRIMARY KEY IDENTITY,
    reason NVARCHAR(50) NOT NULL
)
GO

INSERT INTO dbo.reasons VALUES ('Отпуск')
INSERT INTO dbo.reasons VALUES ('Больничный')
INSERT INTO dbo.reasons VALUES ('Прогул')
GO

CREATE TABLE dbo.timesheet (
    id INT PRIMARY KEY IDENTITY,
    employee_id INT NOT NULL,
    reason_id INT NOT NULL,
    date_start DATE NOT NULL,
    duration INT NOT NULL,
    taken BIT NOT NULL,
    comment NVARCHAR(1024) NOT NULL,
    CONSTRAINT FK_employee_id FOREIGN KEY (employee_id) REFERENCES employees(id),
    CONSTRAINT FK_reason_id FOREIGN KEY (reason_id) REFERENCES reasons(id)
)
GO