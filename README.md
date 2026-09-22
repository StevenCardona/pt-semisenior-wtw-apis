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

### Requisitos

1. [.NET 10 SDK](https://dotnet.microsoft.com/download)
2. **SQL Server LocalDB** (viene con Visual Studio) o SQL Server Express / completo — solo si corres en local

Comprueba .NET:

```powershell
dotnet --version
```

Deberías ver una versión `10.x`.

### Arranque

```powershell
cd wtw-task-manager-apis
dotnet restore
dotnet run --project WebApis --launch-profile http
```

Al iniciar, la API crea la base si no existe, aplica las migrations y carga los usuarios de prueba.

Cuando veas `Now listening on: http://localhost:5065`, ya está lista.

### Paso 3 — Connection string

La API usa la clave **`Remoto`** (ver `Program.cs` → `GetConnectionString("Remoto")`).

Por defecto usa LocalDB y la base `WtwTaskManager` en `WebApis/appsettings.json`:

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

Si prefieres no depender de `MigrateAsync`, ejecuta el script:

[`scripts/WtwTaskManager.sql`](scripts/WtwTaskManager.sql)

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

## Decisiones técnicas

- **Capas desacopladas.** Se separó lo que es la API (controllers), el muelle del negocio (services), el ORM (repository) y los modelos. Así es más fácil modificar o extender sin que todo quede amarrado a EF o a HTTP.
- **Capa de servicios.** Ahí vive la lógica de negocio, las validaciones y el manejo de errores de dominio. Es la pieza más importante del backend.
- **Migraciones.** El esquema se versiona con EF migrations para que todos tengan la misma base y no haya sorpresas entre entornos.
- **Seed de 2 usuarios.** Al arrancar (o con el script SQL) quedan `wtw` (admin) y `steven` (user) para probar endpoints de una.
- **Respuestas y errores uniformes.** Todo sale envuelto en `ApiResponse` y un middleware convierte excepciones de aplicación a JSON con el status correcto.
- **CORS** abierto al frontend Angular en `http://localhost:4200`.
- **Estados de tarea.** Flujo permitido: `pending` → `inProgress` → `done` (no se salta de pendiente a hecha).
- **AdditionalInfo como JSON nativo.** Se guarda en `NVARCHAR(MAX)` con `CHECK (ISJSON(...)=1)`. La API expone un objeto tipado; SQL Server usa `JSON_VALUE` / `JSON_MODIFY` para filtrar y actualizar. No se usa una tabla de tags aparte: el enunciado pide demostrar JSON en SQL Server.

---

## Endpoints

Base local: `http://localhost:5065`  
Base producción: `https://wtw-task-manager-apis.runasp.net`

### Usuarios

| Método | Ruta | Qué hace |
|--------|------|----------|
| `GET` | `/api/users` | Lista usuarios |
| `POST` | `/api/users` | Crea usuario |

### Tareas

| Método | Ruta | Qué hace |
|--------|------|----------|
| `POST` | `/api/tasks` | Crea tarea (estado inicial `pending`; acepta `additionalInfo`) |
| `GET` | `/api/tasks?orderBy=&priority=` | Lista todas (filtro opcional por prioridad JSON) |
| `GET` | `/api/tasks/user/{userId}?status=&orderBy=&priority=` | Lista por usuario |
| `PUT` | `/api/tasks/{id}/status` | Cambia estado |
| `PATCH` | `/api/tasks/{id}/additional-info` | Actualiza `$.priority` con `JSON_MODIFY` |

Las respuestas de tarea traen el asignado en `assignedTo` (`id`, `name`, `mail`, `rol`) y `additionalInfo` (objeto o `null`).

#### Crear tarea con JSON adicional

```http
POST https://wtw-task-manager-apis.runasp.net/api/tasks
Content-Type: application/json

{
  "name": "Revisar reporte",
  "description": "Revision mensual",
  "userId": 2,
  "createdBy": 1,
  "additionalInfo": {
    "priority": "high",
    "dueDate": "2026-10-15",
    "tags": ["sql", "api"],
    "metadata": { "source": "ui" }
  }
}
```

`priority` admite `low`, `medium` o `high`. Filtrar:

```http
GET https://wtw-task-manager-apis.runasp.net/api/tasks?priority=high
```

Actualizar solo la prioridad (usa `JSON_MODIFY` en SQL Server):

```http
PATCH https://wtw-task-manager-apis.runasp.net/api/tasks/1/additional-info
Content-Type: application/json

{
  "priority": "medium",
  "updatedBy": 1
}
```

### Usuarios seed

| Id | Name | Mail | Rol |
|----|------|------|-----|
| 1 | wtw | wtw@wtw.com | admin |
| 2 | steven | steven@wtw.com | user |

Úsalos en `userId` / `createdBy` / `updatedBy` al probar.

---

## Qué quedó pendiente

La API permite peticiones desde:

- `http://localhost:4200` (desarrollo local)
- `https://pt-semisenior-wtw-web.vercel.app` (frontend publicado)

Si cambias el dominio del front, actualiza `WithOrigins` en `WebApis/Program.cs` y **vuelve a publicar la API**.

---

## Apuntes previos al desarrollo
<img width="1000" height="1300" alt="wtw" src="https://github.com/user-attachments/assets/80364ce4-24ad-44f0-976d-85a766e33430" />

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
| AdditionalInfo | nvarchar(max) | JSON opcional; `CHECK ISJSON` |
| CreatedBy / CreatedDate | auditoría | |
| UpdatedBy / UpdatedDate | auditoría | nullable |

### AdditionalInfo (JSON)

```json
{
  "priority": "medium",
  "dueDate": "2026-10-15",
  "tags": ["sql", "api"],
  "metadata": { "source": "ui" }
}
```

Ejemplos de consultas con funciones nativas (también en [`scripts/WtwTaskManager.sql`](scripts/WtwTaskManager.sql)):

```sql
-- ISJSON
SELECT Id, Name, ISJSON(AdditionalInfo) AS IsValidJson
FROM Tasks WHERE AdditionalInfo IS NOT NULL;

-- JSON_VALUE (filtro por prioridad — igual que GET /api/tasks?priority=high)
SELECT Id, Name, JSON_VALUE(AdditionalInfo, '$.priority') AS Priority
FROM Tasks
WHERE JSON_VALUE(AdditionalInfo, '$.priority') = N'high';

-- JSON_QUERY
SELECT Id, JSON_QUERY(AdditionalInfo, '$.tags') AS TagsJson
FROM Tasks WHERE AdditionalInfo IS NOT NULL;

-- OPENJSON (filtrar por etiqueta)
SELECT t.Id, t.Name, tag.value AS Tag
FROM Tasks AS t
CROSS APPLY OPENJSON(t.AdditionalInfo, '$.tags') AS tag
WHERE tag.value = N'sql';

-- JSON_MODIFY (opcional)
UPDATE Tasks
SET AdditionalInfo = JSON_MODIFY(COALESCE(AdditionalInfo, N'{}'), '$.priority', N'medium')
WHERE Id = 1;
```

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
