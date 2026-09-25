IF DB_ID(N'Processador') IS NULL
BEGIN
    CREATE DATABASE Processador;
END
GO

USE Processador;
GO

IF OBJECT_ID(N'dbo.Aprovacoes', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Aprovacoes
    (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        Projeto NVARCHAR(100) NOT NULL,
        ComentariosAdicionais NVARCHAR(MAX) NULL,
        DataAprovacao DATETIME2 NOT NULL
    );
END
GO
