# TradeNest — All-In-One Marketplace platform
![C#](https://img.shields.io/badge/C%23-25EE77?style=flat&logoColor=white) 
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-512BD4?style=flat&logo=dotnet&logoColor=white) 
![MVC](https://img.shields.io/badge/MVC-20232A?style=flat&logo=dotnet&logoColor=white) 
![Bootstrap](https://img.shields.io/badge/Bootstrap-7952B3?style=flat&logo=bootstrap&logoColor=white) 

TradeNest is an all-in-one online marketplace where users can easily explore,
buy and sell a wide variety of products.  

---

## Features & Tech Stack
- **Backend** – C# & ASP.NET Core MVC.
- **Database** – SQL Server & Entity Framework Core for data access.
- **Authentication & Authorization** – Secure user login and registration powered by ASP.NET Core Identity.
- **User Interface** – Razor Views (MVC) & Bootstrap.

---

## Quick Start

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0): You'll need the .NET 8 SDK to build and run the project. Newer SDK versions can tipically build .NET 8 projects, but .NET 8 is the official target.
- [SQL Server 2022 or higher](https://www.microsoft.com/en-us/sql-server/sql-server-downloads): The application uses SQL server 2022.

### Setup

**Clone the repository**
```bash
git clone https://github.com/AchoDanailov/TradeNest.git
```

**Navigate into the project directory**
```bash
cd TradeNest
```

**Restore the tools & dependencies**
```bash
dotnet tool restore
dotnet restore
```

**Apply migrations**
```bash
dotnet ef database update --project TradeNest.Data --startup-project TradeNest.Web
```

**Build and Run the project**
```bash
dotnet run --project TradeNest.Web
```

**Open your browser and navigate to**
```bash 
http://localhost:{defaultAppPort}
```

---

## License

This project is licensed under the MIT License—see the LICENSE file for details.
