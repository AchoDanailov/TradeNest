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
- **User Interface** – Razor Views (MVC), Bootstrap, Typescript and Lit-html.

---

## Features

- [x] Authentication using ASP.NET Identity
- [x] Role based authorization
- [x] Responsive UI with Bootstrap 5
- [x] Dynamic loading of content
- [x] Dynamic and XSS/CSRF protected workflows using SPA techniques, Lit-Html and Bootstrap components.
- [x] CRUD operations for Products
- [x] Products quality approval system. Workflow: User creates or modifies listing of a product he owns and sells, admins review and give decision.
- [x] CRUD operations for Carts and CartProducts
- [x] Tracking of SoldProducts, Orders and related data with analytics purposes for the application sellers.
- [x] MyProducts Dashboard - A place where product sellers can manage their products. Complimented by information about admins reviews, and product sales.
- [x] Admin Dashboard - A place where admins can manage Users, Roles, Categories, Products.

---

## Project Structure

```
TradeNest/
│
├── docs/
│   └── SRS/                           # Lightweight specifications (mainly used as reference of the functionality and the workflows)
│
├── .config/
│   └── dotnet-tools.json              # Manifest file for dotnet tools (e.g. dotnet-ef)
│
├── src/
│   ├── TradeNest.Data/                # DbContext, configurations, migrations, repositories
│   ├── TradeNest.Data.Models/         # Entity models(POCOs) used to model the relational models in the database
│   ├── TradeNest.Data.Common/         # Everything common only used in the Data Layer.
│   ├── TradeNest.GCommon/             # Cross-cutting concerns.
│   ├── TradeNest.Services.Core/       # Business logic (Services, Mapperly mappers).
│   ├── TradeNest.Services.Models/     # Holds DTOs used to transfer data between the service layer and the presentation layer.
│   ├── TradeNest.Web/                 # Presentation layer (Controllers, WebApiControllers, Views, Areas, PresentationModel Mapperly mappers)
│   ├── TradeNest.Web.Models/          # Models for transferring data outside application boundaries and to MVC Views.
│   └── TradeNest.Web.Infrastructure/  # Everything the Web Layer relies on: Filters, Middlewares, Extensions, etc...
│
├── tests/
│   ├── unit/                          
│   │   ├── TradeNest.Services.Tests/  # Business logic Unit Tests.
│   │   └── TradeNest.Data.Tests/      # Unit Tests for data layer classes (Repositories, QueryOptions, Helpers, etc...)
│   │
│   ├── integration/                   # Integration Tests 
│   │   └── TradeNest.Data.IntegrationTests/  
│   │
│   └── e2e/                           # End to end tests
```

---

## Quick Start

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0): You'll need the .NET 8 SDK to build and run the project. Newer SDK versions can typically build .NET 8 projects, but .NET 8 is the official target.
- [SQL Server 2022 or higher](https://www.microsoft.com/en-us/sql-server/sql-server-downloads): The application uses SQL server 2022.
- [npm](https://www.npmjs.com/): Required for frontend dependencies.

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
cd src/TradeNest.Web && npm install && cd ../..
dotnet tool restore && dotnet restore
```

**Apply migrations**
```bash
dotnet ef database update --project src/TradeNest.Data --startup-project src/TradeNest.Web
```

**Build and Run the project**
```bash
dotnet run --project src/TradeNest.Web
```

**For development - open your browser and navigate to**
```bash
http://localhost:5188
```
You can configure the port if you wish in the `launchSettings.json` file located in `src/TradeNest.Web/Properties`.

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
Here you can configure the connection strings the application uses when
connecting to its data stores, authentication options and others application configuration values.

### Default users credentials you can use for developing and testing:

1. Email: User1@gmail.com  
Username: User1  
Password: Password1  


2. Email: User2@gmail.com  
Username: User2  
Password: Password2


3. Email: User3@gmail.com  
Username: User3  
Password: Password3  


4. Email: Admin1@gmail.com  
Username: Admin1  
Password: Admin1Password  

You can log in using one of either your username or your email, along with your password.
I have written an extension method to seed some test data when the application is in Development. I also spread it across the different accounts for ease of review, test and development of different scenarios.

### The .config directory
The application uses a manifest file `dotnet-tools.json` for managing dotnet
tools locally in the `.config/` directory. This allows the developer who is
cloning the repository to not worry about a potential mismatch between the
versions of the dotnet sdk and the dotnet-ef tools required for the database
migrations and other tooling. By running `dotnet tool restore` you are set on that part.

Keep in mind that if you change the version of the SDK you need to change the version of the dotnet
ef-tools to prevent unexpected behavior. If you have the tools
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
