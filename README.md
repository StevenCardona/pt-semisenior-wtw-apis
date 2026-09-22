# WTW Task Manager — API

Backend de la prueba técnica: API REST para gestionar usuarios y tareas.

Stack: **.NET 10**, **Entity Framework Core** y **SQL Server**.

---

## URLs publicadas

| Entorno | URL |
|---------|-----|
| **API (producción)** | https://wtw-task-manager-apis.runasp.net |
| **Frontend (Vercel)** | https://pt-semisenior-wtw-web.vercel.app |

Ejemplo rápido:

```http
GET https://wtw-task-manager-apis.runasp.net/api/users
```

---

## 1. Qué necesitas instalar

Antes de empezar, ten instalado:

1. [.NET 10 SDK](https://dotnet.microsoft.com/download)
2. **SQL Server LocalDB** (viene con Visual Studio) o SQL Server Express / completo — solo si corres en local

Para comprobar que .NET está bien:

```powershell
dotnet --version
```

Deberías ver una versión `10.x`.

---

## 2. Cómo ponerlo a funcionar (paso a paso)

### Paso 1 — Abrir la carpeta del proyecto

```powershell
cd wtw-task-manager-apis
```

### Paso 2 — Restaurar paquetes NuGet

```powershell
dotnet restore
```

### Paso 3 — Connection string

La API usa la clave **`Remoto`** (ver `Program.cs` → `GetConnectionString("Remoto")`).

Archivo: `WebApis/appsettings.json`

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=WtwTaskManager;Trusted_Connection=True;TrustServerCertificate=True",
  "Remoto": "Data Source=...;Initial Catalog=...;User ID=...;Password=...;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
}
```

- **Producción / host actual:** usa `Remoto` (SQL Server remoto).
- **Desarrollo local opcional:** puedes apuntar `Remoto` a LocalDB, o cambiar el código para leer `DefaultConnection`.

No subas contraseñas reales a repositorios públicos; en el host configura la connection string por variables de entorno o panel del proveedor.

### Paso 4 — Arrancar la API (local)

```powershell
dotnet run --project WebApis --launch-profile http
```

Al iniciar, la API:

- Crea la base de datos si no existe
- Aplica las migrations
- Carga los usuarios de prueba

Cuando veas algo como `Now listening on: http://localhost:5065`, ya está lista.

**URL base local:** `http://localhost:5065`

---

## 3. Cómo comprobar que funciona

### Local

```http
GET http://localhost:5065/api/users
```

### Producción

```http
GET https://wtw-task-manager-apis.runasp.net/api/users
```

Deberías ver al menos a `wtw` y `steven`.

### Crear una tarea

```http
POST https://wtw-task-manager-apis.runasp.net/api/tasks
Content-Type: application/json

{
  "name": "Revisar reporte",
  "description": "Revision mensual",
  "userId": 2,
  "createdBy": 1
}
```

Respuesta esperada: **201** con la tarea en estado `pending`.

### Cambiar estado (flujo válido)

```http
PUT https://wtw-task-manager-apis.runasp.net/api/tasks/1/status
Content-Type: application/json

{
  "status": "inProgress",
  "updatedBy": 1
}
```

Luego puedes pasar a `done`. Intentar `pending` → `done` directo debe devolver **400**.

> **Nota sobre estados:** en el JSON de la API se usa camelCase del enum (`pending`, `inProgress`, `done`). En SQL Server se persisten como `pending`, `in_progress`, `done`.

---

## 4. Arquitectura (resumen)

El backend está separado en capas:

| Proyecto | Para qué sirve |
|----------|----------------|
| `WebApis` | Controllers, CORS, connection string, arranque de la API |
| `Services` | Lógica de negocio (crear tarea, cambiar estado, etc.) |
| `Repository` | Acceso a datos con EF Core y migrations |
| `Models` | Entidades `User` y `TaskItem` |
| `Common` | Respuestas estándar y excepciones |

Flujo típico: **Controller → Service → Repository → Base de datos**

---

## 5. Endpoints

Base local: `http://localhost:5065`  
Base producción: `https://wtw-task-manager-apis.runasp.net`

### Usuarios

| Método | Ruta | Qué hace |
|--------|------|----------|
| `GET` | `/api/users` | Lista todos los usuarios |
| `POST` | `/api/users` | Crea un usuario |

### Tareas

| Método | Ruta | Qué hace |
|--------|------|----------|
| `POST` | `/api/tasks` | Crea una tarea (estado inicial: `pending`) |
| `GET` | `/api/tasks?orderBy=createdDate` o `status` | Lista todas las tareas |
| `PUT` | `/api/tasks/{id}/status` | Cambia el estado de una tarea |
| `GET` | `/api/tasks/user/{userId}?status=&orderBy=` | Tareas de un usuario (filtro opcional por estado) |

---

## 6. Reglas de negocio

- El **título** de la tarea es obligatorio (no puede quedar vacío después de quitar espacios).
- Toda tarea debe tener un **usuario asignado** (`userId`).
- Estados permitidos (API): `pending` → `inProgress` → `done`.
- **No** se puede pasar de `pending` a `done` directamente.

---

## 7. Usuarios de prueba (seed)

Se crean solos al arrancar la API:

| Id | Name | Mail | Rol |
|----|------|------|-----|
| 1 | wtw | wtw@wtw.com | admin |
| 2 | steven | steven@wtw.com | user |

Úsalos en `userId` / `createdBy` al probar los endpoints.

---

## 8. CORS (frontend Angular)

La API permite peticiones desde:

- `http://localhost:4200` (desarrollo local)
- `https://pt-semisenior-wtw-web.vercel.app` (frontend publicado)

Si cambias el dominio del front, actualiza `WithOrigins` en `WebApis/Program.cs` y **vuelve a publicar la API**.

---

## 9. Modelo de datos (referencia)

### Users

| Columna | Tipo | Notas |
|--------|------|--------|
| Id | int | PK |
| Name | nvarchar(200) | requerido |
| Mail | nvarchar(256) | único |
| Rol | nvarchar(20) | `admin` o `user` |
| CreatedBy / CreatedDate | auditoría | |
| UpdatedBy / UpdatedDate | auditoría | nullable |

### Tasks

| Columna | Tipo | Notas |
|--------|------|--------|
| Id | int | PK |
| Name | nvarchar(200) | título, requerido |
| Description | nvarchar(2000) | opcional |
| Status | nvarchar(20) | en DB: `pending`, `in_progress`, `done` |
| UserId | int | FK → Users (asignado) |
| CreatedBy / CreatedDate | auditoría | |
| UpdatedBy / UpdatedDate | auditoría | nullable |

---

## 10. Publicar la API

1. Publica el proyecto `WebApis` al host (Web Deploy / panel del proveedor).
2. Asegúrate de que la connection string `Remoto` apunte a la base remota.
3. Confirma CORS con el dominio Vercel del front.
4. Prueba: `GET https://wtw-task-manager-apis.runasp.net/api/users`

**Orden recomendado:** publica primero la API (CORS), después el frontend en Vercel.

---

## 11. Opción alternativa: script SQL

Si prefieres crear la base a mano (sin esperar a que la API haga `MigrateAsync`), ejecuta el script de abajo en SSMS o Azure Data Studio.

**Si arrancas la API con migrations, no necesitas este script.**

```sql
------------------------------------------------------------
-- 1) Base de datos
------------------------------------------------------------
IF DB_ID(N'WtwTaskManager') IS NULL
BEGIN
    CREATE DATABASE [WtwTaskManager];
END
GO

USE [WtwTaskManager];
GO

------------------------------------------------------------
-- 2) Historial de migrations (EF)
------------------------------------------------------------
IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

------------------------------------------------------------
-- 3) Tablas e índices (InitialCreate)
------------------------------------------------------------
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

------------------------------------------------------------
-- 4) Usuarios seed (SeedDefaultUsers)
------------------------------------------------------------
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
```
