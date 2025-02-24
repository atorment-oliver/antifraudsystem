CREATE DATABASE AntiFraudSystem;
USE [AntiFraudSystem];
GO

-- Crear la tabla Transactions
CREATE TABLE [dbo].[Transactions] (
    [TransactionExternalId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, -- ID único de la transacción
    [SourceAccountId]       UNIQUEIDENTIFIER NOT NULL,            -- ID de la cuenta de origen
    [TargetAccountId]       UNIQUEIDENTIFIER NOT NULL,            -- ID de la cuenta de destino
    [TransferTypeId]        INT              NOT NULL,            -- Tipo de transferencia
    [Value]                 DECIMAL(18, 2)   NOT NULL,            -- Valor de la transacción
    [Status]                NVARCHAR(50)     NOT NULL,            -- Estado de la transacción (pending, approved, rejected)
    [CreatedAt]             DATETIME2        NOT NULL DEFAULT GETDATE() -- Fecha de creación
);
GO

-- Crear un índice para mejorar las consultas por estado
CREATE INDEX [IX_Transactions_Status] ON [dbo].[Transactions] ([Status]);
GO

-- Crear un índice para mejorar las consultas por fecha de creación
CREATE INDEX [IX_Transactions_CreatedAt] ON [dbo].[Transactions] ([CreatedAt]);
GO