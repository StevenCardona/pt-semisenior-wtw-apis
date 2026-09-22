# WTW Task Manager — API

Backend de la prueba técnica: API REST para gestionar usuarios y tareas.

Stack: **.NET 10**, **Entity Framework Core** y **SQL Server** (LocalDB por defecto).

---

## Pasos para ejecutar el proyecto

### Requisitos

1. [.NET 10 SDK](https://dotnet.microsoft.com/download)
2. **SQL Server LocalDB** (viene con Visual Studio) o SQL Server Express / completo

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

**URL base:** `http://localhost:5065`

### Connection string

Por defecto usa LocalDB y la base `WtwTaskManager` en `WebApis/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=WtwTaskManager;Trusted_Connection=True;TrustServerCertificate=True"
}
```

Cámbiala solo si usas otra instancia de SQL Server.

### Opción: crear la base a mano

Si prefieres no depender de `MigrateAsync`, ejecuta el script:

[`scripts/WtwTaskManager.sql`](scripts/WtwTaskManager.sql)

Ábrelo en SSMS o Azure Data Studio, cópialo y ejecútalo. Crea la base, tablas, índices y los 2 usuarios seed. Si luego arrancas la API con migrations, el historial EF ya queda alineado.

---

## Cómo está compuesto

```text
WebApis/      Controllers, middleware, Program, CORS, connection string
Services/     Lógica de negocio y validaciones (un servicio por caso de uso)
Repository/   Acceso a datos con EF Core + migrations
Models/       Entidades de dominio (User, TaskItem, estados, roles)
Common/       ApiResponse y excepciones de aplicación
```

Flujo típico: **Controller → Service → Repository → base de datos**.

---

## Decisiones técnicas

- **Capas desacopladas.** Se separó lo que es la API (controllers), el muelle del negocio (services), el ORM (repository) y los modelos. Así es más fácil modificar o extender sin que todo quede amarrado a EF o a HTTP.
- **Capa de servicios.** Ahí vive la lógica de negocio, las validaciones y el manejo de errores de dominio. Es la pieza más importante del backend.
- **Migraciones.** El esquema se versiona con EF migrations para que todos tengan la misma base y no haya sorpresas entre entornos.
- **Seed de 2 usuarios.** Al arrancar (o con el script SQL) quedan `wtw` (admin) y `steven` (user) para probar endpoints de una.
- **Respuestas y errores uniformes.** Todo sale envuelto en `ApiResponse` y un middleware convierte excepciones de aplicación a JSON con el status correcto.
- **CORS** abierto al frontend Angular en `http://localhost:4200`.
- **Estados de tarea.** Flujo permitido: `pending` → `inProgress` → `done` (no se salta de pendiente a hecha).

---

## Endpoints

Base: `http://localhost:5065`

### Usuarios

| Método | Ruta | Qué hace |
|--------|------|----------|
| `GET` | `/api/users` | Lista usuarios |
| `POST` | `/api/users` | Crea usuario |

### Tareas

| Método | Ruta | Qué hace |
|--------|------|----------|
| `POST` | `/api/tasks` | Crea tarea (estado inicial `pending`) |
| `GET` | `/api/tasks?orderBy=` | Lista todas |
| `GET` | `/api/tasks/user/{userId}?status=&orderBy=` | Lista por usuario |
| `PUT` | `/api/tasks/{id}/status` | Cambia estado |

Las respuestas de tarea traen el asignado en `assignedTo` (`id`, `name`, `mail`, `rol`).

### Usuarios seed

| Id | Name | Mail | Rol |
|----|------|------|-----|
| 1 | wtw | wtw@wtw.com | admin |
| 2 | steven | steven@wtw.com | user |

Úsalos en `userId` / `createdBy` / `updatedBy` al probar.

---

## Qué quedó pendiente

- Editar y eliminar usuarios
- Editar y eliminar tareas
- Búsqueda (usuarios / tareas por nombre o descripción)
- Filtros, orden y paginación más completos en listados
- Autenticación real (JWT o MFA): hoy no hay login; los ids de auditoría los manda el cliente
- Autorización por rol (que un usuario solo vea lo asignado)
