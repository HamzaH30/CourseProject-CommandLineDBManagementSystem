# Advanced Database Course Project: Command Line Database Management System

## Overview

This project is a command-line database management system designed to showcase proficiency in C#, .NET, SQL, and Entity Framework. It allows users to perform Create, Read, Update, and Delete (CRUD) operations on a soccer league database, managing data related to teams, players, matches, and more. The system focuses on clear organization, data integrity, and real-world applicability, with an emphasis on structured data, relationships, data types, and efficient data management.

## Features

- **CRUD Operations:** Users can create, read, update, and delete records in the database through a menu-driven interface.
- **Structured Data:** Organized into separate tables for teams, players, matches, etc., ensuring data is manageable.
- **Data Integrity and Relationships:** Utilizes primary and foreign keys to maintain consistency across entities.
- **Efficient Data Management:** Implements actions like cascading deletes to smartly handle data dependencies.
- **Extensible Design:** Covers a wide range of functionalities and allows for future expansion.
- **Complex Relationships:** Effectively models scenarios such as home and away matches.
- **Tabular Data Presentation:** Improves readability and comprehension of data output.

## Getting Started

### Prerequisites

Ensure you have the following installed:
- .NET SDK
- SQL Server
- Entity Framework

### Installation

1. Clone the repository to your local machine.
2. Open the solution in your preferred IDE (e.g., Visual Studio).
3. Ensure the database connection string in `appsettings.json` is correctly set up for your SQL Server instance.
4. Run the Entity Framework migrations to set up the database schema:
    ```bash
    dotnet ef database update
    ```
5. Optionally, contact me to request SQL queries for seeding the database with sample data.

### Usage

1. Navigate to the project directory in your terminal or command prompt.
2. Run the application:
    ```bash
    dotnet run
    ```
3. Follow the on-screen menu to perform CRUD operations on the soccer league database.

## Design and Architecture

The application's entry point is `Program.cs`, presenting users with four main navigation options for CRUD operations. It employs a carefully structured architecture with a separation of concerns and adherence to coding standards. Operations are intentionally decentralized to avoid tightly coupled code, enhancing modularity and maintainability.

## Additional Resources

- [ER Diagram](https://lucid.app/lucidchart/b9784be2-f370-48be-acdc-a42cb988b98d/edit?viewport_loc=-574%2C384%2C2548%2C1248%2C0_0&invitationId=inv_48f6ce5f-c8ec-4ec5-8916-d2a1911190d2) - For a better understanding of the database schema.