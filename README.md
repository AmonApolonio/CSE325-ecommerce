# CSE325 E-Commerce Project

A full-stack e-commerce application built with ASP.NET Core (backend) and Blazor WebAssembly (frontend), featuring user authentication, product management, shopping cart, and more.

## Table of Contents

- [Features](#features)
- [Technologies Used](#technologies-used)
- [Project Structure](#project-structure)
- [Quick Start](#quick-start)
- [Documentation](#documentation)
- [License](#license)

## Features

- **User Authentication**: JWT-based authentication for clients and sellers
- **Product Management**: Browse, search, and manage products
- **Shopping Cart**: Add/remove items, anonymous and authenticated carts
- **Categories**: Organize products by categories
- **Seller Dashboard**: Manage seller profiles and products
- **Responsive Design**: Mobile-friendly UI with Tailwind CSS

## Technologies Used

### Backend
- ASP.NET Core 9.0
- Entity Framework Core
- PostgreSQL
- JWT Authentication
- Swagger/OpenAPI

### Frontend
- Blazor WebAssembly
- Tailwind CSS
- HttpClient for API calls

## Project Structure

```
CSE325-ecommerce/
├── backend/                 # ASP.NET Core API
│   ├── Controllers/         # API endpoints
│   ├── Data/Entities/       # Database models
│   ├── DTOs/                # Data transfer objects
│   ├── Services/            # Business logic services
│   └── Migrations/          # Database migrations
├── frontend/                # Blazor WebAssembly app
│   ├── Pages/               # Razor pages
│   ├── Components/          # Reusable components
│   ├── Services/            # API client services
│   └── Models/              # Frontend models
├── cse325_ecommerce.Shared/ # Shared models
└── docs/                    # Documentation
```

## Quick Start

1. Ensure you have .NET 9.0 SDK and PostgreSQL installed
2. Clone the repository
3. Set up the database connection in `backend/appsettings.json`
4. Run the backend: `cd backend && dotnet run`
   - API will be available at `https://localhost:5001`
   - Swagger documentation at `http://localhost:5028/swagger/index.html`
5. Run the frontend: `cd frontend && dotnet run`
6. Open https://localhost:5026 in your browser

For detailed setup instructions, see [Setup Guide](docs/setup.md).

## Documentation

- [Setup Guide](docs/setup.md) - Detailed installation and configuration
- [Architecture](docs/architecture.md) - System design and components
- [Frontend Guide](docs/frontend.md) - Frontend components and flows
- [Database Schema](docs/database.md) - Database design and entities


## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.