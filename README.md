# TradeNest — All-In-One Marketplace platform
![C#](https://img.shields.io/badge/C%23-25EE77?style=flat&logoColor=white) 
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-512BD4?style=flat&logo=dotnet&logoColor=white) 
![MVC](https://img.shields.io/badge/MVC-20232A?style=flat&logo=dotnet&logoColor=white) 
![Bootstrap](https://img.shields.io/badge/Bootstrap-7952B3?style=flat&logo=bootstrap&logoColor=white) 

TradeNest is an all-in-one online marketplace where users can easily explore,
buy and sell a wide variety of products.  

---

## Tech Stack
- **Backend** – C# & ASP.NET Core MVC.
- **Database** – SQL Server & Entity Framework Core for data access and data store.
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

## Project Structure

```
TradeNest/
│
├── TradeNest.Data/                # DbContext, configurations and migrations
├── TradeNest.Data.Models/         # Entity models EFCore uses
├── TradeNest.GCommon/             # For everething with Cross-cutting concerns
├── TradeNest.Services.Core/       # Business logic \ service layer
├── TradeNest.Web/                 # MVC web application (presentation layer)
└── TradeNest.Web.ViewModels/      # ViewModels
```

---

## Features

- [x] User registration and login (ASP.NET Identity)
- [x] Responsive UI with Bootstrap 5
- [x] CRUD operations for Products
- [ ] Indirect CRUD for Order, Watchlist
- [ ] My Orders Dashboard
- [ ] Admin Dashboard

---

## Configuration

Key settings in `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=127.0.0.1;Database=TradeNestDb;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False"
  }
}
```

### Default users credentials you can use for developing and testing:
1. Email: Har1b0@gmail.com  
Password: Har1b0o
2. Email: M1rk0@gmail.com  
Password: M1rk0o


### The .config directory
The application uses a manifest file `dotnet-tools.json` for managing dotnet
tools locally in the `.config/` directory. This allows the developer who is
cloning the repository to not worry about a potential missmatch between the
versions of the dotnet sdk and the dotnet-ef tools required for the database
migrations - by just running `dotnet tool restore`. Keep in mind that if you
change the version of the SDK you need to change the version of the dotnet
ef-tools to be sure there wont be unexpected behaviour. If you have the tools
globally on your host or you work with the Visual Studio Package Manager Console and
wish to use either your global tools version or you Visual Studio Package
Manager Console you are free to delete the `.config` directory. For more
information see the
[documentation](https://learn.microsoft.com/en-us/dotnet/core/tools/local-tools-how-to-use).
```json
{
  "version": 1,
  "isRoot": true,
  "tools": {
    "dotnet-ef": {
      "version": "8.0.23",
      "commands": [
        "dotnet-ef"
      ],
      "rollForward": false
    }
  }
}

```

---

## Contributing

Contributions are welcome. To contribute:
1. Fork the repository
2. Create a new branch: `git checkout -b feature/your-feature-name`
3. Commit your changes: `git commit -m "Add some feature"`
4. Push to the branch: `git push origin feature/your-feature-name`
5. Open a Pull Request

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
