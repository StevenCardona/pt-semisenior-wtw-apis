-- WtW Task Manager — script listo para pegar en SSMS / Azure Data Studio
-- Si arrancas la API con migrations (MigrateAsync), no necesitas este archivo.

-- 1) Base de datos
IF DB_ID(N'WtwTaskManager') IS NULL
BEGIN
    CREATE DATABASE [WtwTaskManager];
END
GO

USE [WtwTaskManager];
GO

-- 2) Historial de migrations (EF)
IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

-- 3) Tablas e índices
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921182157_InitialCreate'
)
BEGIN
    CREATE TABLE [Users] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(200) NOT NULL,
        [Mail] nvarchar(256) NOT NULL,
        [Rol] nvarchar(20) NOT NULL,
        [CreatedBy] int NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [UpdatedBy] int NULL,
        [UpdatedDate] datetime2 NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
    );

    CREATE TABLE [Tasks] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(200) NOT NULL,
        [Description] nvarchar(2000) NULL,
        [Status] nvarchar(20) NOT NULL,
        [UserId] int NOT NULL,
        [CreatedBy] int NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [UpdatedBy] int NULL,
        [UpdatedDate] datetime2 NULL,
        CONSTRAINT [PK_Tasks] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Tasks_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );

    CREATE UNIQUE INDEX [IX_Users_Mail] ON [Users] ([Mail]);
    CREATE INDEX [IX_Tasks_UserId] ON [Tasks] ([UserId]);
    CREATE INDEX [IX_Tasks_UserId_Status] ON [Tasks] ([UserId], [Status]);

    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260921182157_InitialCreate', N'10.0.12');
END;
GO

-- 4) Usuarios de prueba
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260921204411_SeedDefaultUsers'
)
BEGIN
    SET IDENTITY_INSERT [Users] ON;

    IF NOT EXISTS (SELECT 1 FROM [Users] WHERE [Mail] = N'wtw@wtw.com')
    BEGIN
        INSERT INTO [Users] ([Id], [Name], [Mail], [Rol], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate])
        VALUES (1, N'wtw', N'wtw@wtw.com', N'admin', 1, '2026-01-01T00:00:00.0000000Z', NULL, NULL);
    END;

    IF NOT EXISTS (SELECT 1 FROM [Users] WHERE [Mail] = N'steven@wtw.com')
    BEGIN
        INSERT INTO [Users] ([Id], [Name], [Mail], [Rol], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate])
        VALUES (2, N'steven', N'steven@wtw.com', N'user', 1, '2026-01-01T00:00:00.0000000Z', NULL, NULL);
    END;

    SET IDENTITY_INSERT [Users] OFF;

    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260921204411_SeedDefaultUsers', N'10.0.12');
END;
GO
