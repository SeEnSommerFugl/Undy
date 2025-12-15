# SQL Scripts – Undy

This folder contains the SQL scripts used to set up and maintain the database.

The scripts are separated by responsibility to make development, testing, and maintenance easier and to avoid errors when re-running SQL during development.

---

## Overview

The SQL setup is divided into two main files:

- `01_CreateTables.sql`
- `02_ViewsAndProcedures.sql`

Each file has a specific purpose and should be executed in the correct context.

---

## 01_CreateTables.sql

### Purpose
This script creates all database tables used by the application.

It includes:
- Product
- Customers
- SalesOrder
- ProductSalesOrder
- WholesaleOrder
- ProductWholesaleOrder
- ReturnOrder

### When to run
- Run **only once** when creating a new or empty database.
- Run when the database has been deleted and needs to be recreated from scratch.

### Important
This file should **not** be run multiple times on the same database, as it uses `CREATE TABLE` statements and will fail if the tables already exist.

---

## 02_ViewsAndProcedures.sql

### Purpose
This script contains:
- All `CREATE OR ALTER VIEW` statements
- All `CREATE OR ALTER PROCEDURE` statements

Including, but not limited to:
- `vw_SalesOrderHeaders`
- `vw_SalesOrderLines`
- `vw_Products`
- `usp_Insert_SalesOrder`
- `usp_Update_SalesOrder`

### When to run
- Run every time a view or stored procedure is added or modified.
- Run when SQL logic is changed, such as calculations, joins, or output columns.

This file can safely be executed multiple times because it uses `CREATE OR ALTER`.

---

## When to run which script

| Situation | Script to run |
|---------|---------------|
| New database | `01_CreateTables.sql` then `02_ViewsAndProcedures.sql` |
| Change in views or procedures | `02_ViewsAndProcedures.sql` |
| Only C# or XAML changes | No SQL scripts |

---

## Notes

- The application does **not** read or execute these SQL files at runtime.
- The program communicates directly with the database using stored procedures and views.
- These scripts exist solely for database setup, maintenance, and documentation.

---

## Summary

- Tables are created once.
- Views and stored procedures are updated multiple times.
- SQL scripts are kept separate from application code for clarity and maintainability.

