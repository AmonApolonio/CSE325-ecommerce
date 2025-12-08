# Architecture Guide

This document describes the architecture of the CSE325 E-Commerce application.

## System Overview

The application is a full-stack e-commerce platform with separate backend API and frontend client, following a microservices-inspired architecture within a monolithic deployment.

## Architecture Diagram

```
┌─────────────────┐    HTTP/HTTPS    ┌─────────────────┐
│                 │◄────────────────►│                 │
│  Blazor WASM    │                  │  ASP.NET Core   │
│   Frontend      │                  │     API         │
│                 │                  │                 │
└─────────────────┘                  └─────────────────┘
         │                                   │
         │                                   │
         ▼                                   ▼
┌─────────────────┐                  ┌─────────────────┐
│   Browser       │                  │   PostgreSQL    │
│   LocalStorage  │                  │   Database      │
└─────────────────┘                  └─────────────────┘
```

## Backend Architecture

### ASP.NET Core API

The backend is built with ASP.NET Core 9.0 and follows RESTful API principles.

#### Layers

1. **Controllers**: Handle HTTP requests and responses
   - `AuthController`: Authentication endpoints
   - `CartController`: Shopping cart operations
   - `CategoriesController`: Category management
   - `ClientsController`: Client user operations
   - `ProductsController`: Product CRUD operations
   - `SellersController`: Seller operations

2. **Services**: Business logic layer
   - `TokenService`: JWT token generation and validation

3. **Data Layer**: Database access using Entity Framework Core
   - `AppDbContext`: Main database context
   - Entities: Domain models
   - DTOs: Data transfer objects

#### Authentication Flow

```
Client Request ──► JWT Middleware ──► Controller ──► Service ──► Database
     ▲                │                     │             │
     │                ▼                     ▼             ▼
     └───────── Token Response ◄───── Token Generation ◄─── User Validation
```

### Database Schema

The application uses PostgreSQL with the following main entities:

- **Users**: Clients and Sellers (separate tables)
- **Products**: Items for sale with categories and sellers
- **Cart/CartItem**: Shopping cart functionality
- **Orders/OrdersProduct**: Order management
- **Categories**: Product categorization
- **Recommendations**: Product recommendation system

## Frontend Architecture

### Blazor WebAssembly

The frontend is a Single Page Application (SPA) built with Blazor WebAssembly.

#### Component Structure

```
App.razor (Root)
├── MainLayout.razor
│   ├── Header.razor
│   ├── Footer.razor
│   └── Body
│       ├── Home.razor
│       ├── Login.razor
│       ├── Signup.razor
│       ├── Cart.razor
│       ├── Search.razor
│       ├── ItemDetail.razor
│       ├── Account.razor
│       └── ...
└── Components/
    ├── ProductCard.razor
    ├── ProductGrid.razor
    ├── AddToCartButton.razor
    └── ...
```

#### State Management

- **Authentication State**: `CustomAuthStateProvider` manages user authentication state
- **Local Storage**: Anonymous cart persistence
- **Services**: API communication and business logic

#### Data Flow

```
User Interaction ──► Component ──► Service ──► HttpClient ──► API
     ▲                    │             │             │
     │                    ▼             ▼             ▼
     └───────── UI Update ◄───── State Update ◄───── Response
```

## Security

### Authentication

- **JWT Tokens**: Stateless authentication
- **Role-based Access**: Client and Seller roles
- **HTTPS**: Encrypted communication

### Authorization

- **Middleware**: Validates tokens on protected endpoints
- **CORS**: Configured for frontend origin
- **Input Validation**: Server-side validation on all inputs

## Deployment

### Development

- **Backend**: Runs on Kestrel web server
- **Frontend**: Served by Blazor's development server
- **Database**: Local PostgreSQL instance

### Production Considerations

- **Reverse Proxy**: Nginx or IIS for static file serving
- **Containerization**: Docker support planned
- **CDN**: For static assets
- **Load Balancing**: For API scaling

## Key Design Decisions

1. **Separate Frontend/Backend**: Allows independent scaling and technology choices
2. **JWT Authentication**: Stateless, scalable authentication
3. **PostgreSQL**: Robust relational database for complex queries
4. **Blazor WASM**: .NET in the browser for full-stack .NET development
5. **RESTful API**: Standard, well-documented API design

## Performance Considerations

- **Database Indexing**: Optimized queries with proper indexing
- **Caching**: Not implemented yet, but planned for product listings
- **Lazy Loading**: Components load data as needed
- **Bundle Optimization**: Blazor's built-in bundling and minification

## Scalability

- **Horizontal Scaling**: Stateless API allows multiple instances
- **Database Sharding**: Possible for large product catalogs
- **CDN Integration**: For global content delivery
- **Microservices**: Potential future split into services

## Monitoring and Logging

- **Structured Logging**: Using ASP.NET Core logging
- **Health Checks**: Basic health endpoints
- **Error Handling**: Global exception handling middleware

## Future Enhancements

- **API Versioning**: For backward compatibility
- **Rate Limiting**: Prevent abuse
- **Caching Layer**: Redis for performance
- **Message Queue**: For order processing
- **Real-time Updates**: SignalR for live cart updates