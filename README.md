Canteen/
│
├── Presentation/
│   ├── Views/
│   └── ViewModels/
│
├── Business/
│   ├── Services/
│   │   ├── FoodService.cs
│   │   └── BillService.cs
│   │
│   └── Interfaces/
│
├── EntityDataAccess/
│   ├── Repositories/
│   │   ├── FoodRepository.cs
│   │   ├── CategoryRepository.cs
│   │   └── BillRepository.cs
│   │
│   └── CanteenContext.cs
│
└── Models/
    ├── Food.cs
    ├── Category.cs
    └── Bill.cs
	
# CanteenManager

A desktop canteen management application built with **C# / .NET** following the **MVVM (Model-View-ViewModel)** architectural pattern, supporting both **SQLite** (local/embedded storage) and **SQL Server** (centralized relational database) via **Entity Framework Core** and **ADO.NET**.

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![Architecture](https://img.shields.io/badge/Architecture-MVVM-0078D4?style=flat-square)]()
[![ORM](https://img.shields.io/badge/ORM-EF%20Core-512BD4?style=flat-square)]()
[![Database](https://img.shields.io/badge/Database-SQLite%20%7C%20SQL%20Server-336791?style=flat-square)]()
[![License](https://img.shields.io/badge/License-MIT-green?style=flat-square)]()

---

## Table of Contents

- [Overview](#overview)
- [Key Features](#key-features)
- [Architecture & Tech Stack](#architecture--tech-stack)
- [Project Structure](#project-structure)
- [Database Configuration](#database-configuration)
- [Getting Started](#getting-started)
- [Roadmap](#roadmap)
- [Contributing](#contributing)
- [License](#license)

---

## Overview

**CanteenManager** streamlines day-to-day canteen operations—including menu/item management, order processing, inventory tracking, and sales reporting. The codebase separates UI concerns from business logic using the **MVVM** pattern, while combining **Entity Framework Core** for clean object-relational mapping with **ADO.NET** for high-throughput queries, bulk operations, or custom stored procedures/SQL execution.

---

## Key Features

- **Menu & Product Management:** Create, update, categorize, and toggle availability of food, beverages, and combos.
- **Order & Billing Workflow:** Build customer orders, calculate totals/discounts, and record transaction history.
- **Inventory Tracking:** Monitor stock levels, log restocks, and flag low-stock items.
- **Sales & Operational Reports:** View daily/periodic revenue summaries and top-selling items.
- **Dual Database Provider Support:** Switch seamlessly between **SQLite** (ideal for standalone or offline setups) and **SQL Server** (ideal for multi-terminal or networked environments).

---

## Architecture & Tech Stack

- **UI Framework:** `[WPF / WinForms / MAUI / Avalonia]`
- **Architectural Pattern:** **MVVM** (`INotifyPropertyChanged`, `ICommand` / commands, data binding, separation of View, ViewModel, and Model/Service layers)
- **Data Access:**
  - **Entity Framework Core:** Entity models, `DbContext`, LINQ queries, and schema migrations.
  - **ADO.NET:** `SqlConnection` / `SqliteConnection`, parameterized commands, `SqlDataReader` / `SqliteDataReader`, and batch/performance-critical queries.
- **Databases:** **SQLite** & **Microsoft SQL Server**

---

## Project Structure

```text
CanteenManager/
├── CanteenManager.sln
├── src/
│   ├── CanteenManager.App/          # Views, XAML/UI resources, App startup & DI
│   ├── CanteenManager.ViewModels/   # ViewModels, Commands, UI state & validation
│   ├── CanteenManager.Models/       # Domain entities & DTOs
│   └── CanteenManager.Data/         # EF Core DbContext, migrations & ADO.NET repositories/services
├── tests/                           # Unit & integration tests (optional)
└── README.md	