# Inventory Management

Aplicación Full Stack para gestionar el inventario de productos en múltiples almacenes. Desarrollada durante mi pasantía en Star Gas C.A. (enero – abril 2026).

## Funcionalidades

- **Catálogos:** CRUD de productos, categorías y almacenes, con reglas de integridad (no se puede eliminar una categoría con productos ni un producto o almacén con movimientos asociados).
- **Movimientos de inventario:** registro de entradas, salidas y transferencias entre almacenes. Cada operación se ejecuta dentro de una transacción.
- **Validación de stock en el servidor:** una salida se rechaza si el almacén de origen no tiene existencias suficientes.
- **Dashboard:** stock actual por producto y almacén, con indicador de *Stock bajo* según el stock mínimo definido.
- **Reporte en Excel:** exportación del detalle por producto (ClosedXML).
- **Documentación de la API** con Swagger en entorno de desarrollo.

## Tecnologías

| Capa | Tecnologías |
|---|---|
| Frontend | Angular, TypeScript, Bootstrap 5, componentes standalone |
| Backend | C#, ASP.NET Core (.NET 10), API REST |
| Datos | Entity Framework Core, Microsoft SQL Server (LocalDB) |
| Otros | Swagger, ClosedXML, Git |

## Estructura

```
InventoryManagement/
├── Inventory.API/     # Web API (controllers, services, models, DbContext)
├── Inventory-App/     # Cliente Angular (dashboard, movimientos, catálogos)
└── InventoryManagementSystem.slnx
```

## Cómo ejecutarlo

**Requisitos:** .NET SDK 10, Node.js y npm, SQL Server LocalDB (incluido con Visual Studio).

1. **Backend**
   ```bash
   cd Inventory.API
   dotnet run
   ```
   La API queda en `https://localhost:7084`. La base de datos `InventoryManagementDB` se crea automáticamente en LocalDB al iniciar. Swagger: `https://localhost:7084/swagger`.

2. **Frontend**
   ```bash
   cd Inventory-App
   npm install
   npm start
   ```
   Abre `http://localhost:4200`.

## Endpoints principales

| Método | Ruta | Descripción |
|---|---|---|
| GET / POST / PUT / DELETE | `/api/Products` | Gestión de productos |
| GET / POST / PUT / DELETE | `/api/Categories` | Gestión de categorías |
| GET / POST / PUT / DELETE | `/api/Warehouses` | Gestión de almacenes |
| GET | `/api/InventoryMovements/stock-summary` | Resumen de stock |
| POST | `/api/InventoryMovements/movement` | Registrar entrada, salida o transferencia |
| GET | `/api/InventoryMovements/report/excel` | Reporte en Excel por producto |

## Capturas

<!-- Agrega aquí 2 o 3 capturas: dashboard, registro de movimiento y catálogos -->

## Qué aprendí

- Diseñar un modelo relacional y mantener la integridad referencial con EF Core.
- Separar la lógica de negocio en una capa de servicios y validar las reglas en el servidor, no solo en el cliente.
- Manejar transacciones para que una transferencia entre almacenes no quede a medias.
- Integrar un frontend Angular con una API en C# (CORS y proxy de desarrollo).

## Mejoras pendientes

- Autenticación y roles de usuario.
- Migraciones de EF Core en lugar de creación automática de la base de datos.
- Pruebas unitarias para el servicio de inventario.

## Autor

**Andrés Nava** · [GitHub](https://github.com/AndresANF) · andresnava0406@gmail.com
