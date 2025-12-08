# Frontend Guide

This guide covers the frontend application built with Blazor WebAssembly.

## Overview

The frontend is a Single Page Application (SPA) that provides a user-friendly interface for the e-commerce platform. It communicates with the backend API via HTTP requests.

## Technologies

- **Blazor WebAssembly**: .NET in the browser
- **Tailwind CSS**: Utility-first CSS framework
- **HttpClient**: For API communication
- **JWT Authentication**: Token-based auth with local storage

## Application Structure

### Pages

- **Home** (`/`): Product listing and categories
- **Login** (`/login`): User authentication
- **Signup** (`/signup`): User registration
- **Cart** (`/cart`): Shopping cart management
- **Search** (`/search`): Product search and filtering
- **ItemDetail** (`/item/{id}`): Individual product details
- **Account** (`/account`): User profile management

### Components

#### Shared Components
- **Header**: Navigation and user menu
- **Footer**: Site footer with links
- **ProductCard**: Individual product display
- **ProductGrid**: Grid layout for products
- **AddToCartButton**: Add to cart functionality

#### UI Components
- **Button**: Reusable button component
- **SearchBar**: Search input with suggestions
- **StarRating**: Product rating display

## User Flows

### Anonymous User Flow

1. **Browse Products**: User visits home page, sees featured products
2. **View Categories**: Click category to filter products
3. **Product Details**: Click product to see full details
4. **Add to Cart**: Add items to anonymous cart (stored in localStorage)
5. **Checkout**: Login required for checkout

### Authenticated User Flow

1. **Login/Signup**: Authenticate via login form
2. **Browse Products**: Same as anonymous, but with personalized features
3. **Manage Cart**: Add/remove items, persist across sessions
4. **Account Management**: View/edit profile
5. **Seller Features**: If seller role, manage products

## State Management

### Authentication State

- **CustomAuthStateProvider**: Manages user authentication state
- **JWT Tokens**: Stored in localStorage
- **HttpClient Interceptor**: Automatically adds tokens to requests

### Cart State

- **Anonymous Cart**: Stored in localStorage with cartId
- **Authenticated Cart**: Server-side persistence
- **CartService**: Handles cart operations

## API Integration

### Services

- **ProductService**: Product CRUD operations
- **CategoryService**: Category management
- **CartService**: Shopping cart functionality
- **AuthService**: Authentication operations
- **ClientService**: User profile management
- **SellerService**: Seller operations

### HttpClient Configuration

```csharp
// Program.cs
builder.Services.AddHttpClient("BackendApi", client => 
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"]))
    .AddHttpMessageHandler<JwtHandler>();
```

## Routing

Blazor routing is configured in `App.razor`:

```razor
<Router AppAssembly="@typeof(App).Assembly">
    <Found Context="routeData">
        <RouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" />
    </Found>
    <NotFound>
        <LayoutView Layout="@typeof(MainLayout)">
            <p>Sorry, there's nothing at this address.</p>
        </LayoutView>
    </NotFound>
</Router>
```

## Styling

### Tailwind CSS

- Utility-first approach
- Responsive design with breakpoints (sm, md, lg, xl)
- Custom color scheme with orange theme
- Component-based styling

### CSS Files

- `wwwroot/css/app.css`: Main application styles
- `wwwroot/css/globals.css`: Global styles and variables
- `MainLayout.razor.css`: Layout-specific styles

## Forms and Validation

### Login Form

```razor
<EditForm Model="@loginModel" OnValidSubmit="HandleLogin">
    <DataAnnotationsValidator />
    <ValidationSummary />

    <input @bind="loginModel.Email" />
    <input type="password" @bind="loginModel.Password" />
    <select @bind="loginModel.UserType">
        <option value="Client">Client</option>
        <option value="Seller">Seller</option>
    </select>

    <button type="submit">Login</button>
</EditForm>
```

### Client-Side Validation

- Data annotations on models
- Built-in Blazor validation
- Custom validation attributes

## Error Handling

### Global Error Handling

- Try-catch blocks in component methods
- Error messages displayed to user
- Logging to console/browser dev tools

### API Error Handling

```csharp
try
{
    var products = await ProductService.GetAllProductsAsync();
}
catch (Exception ex)
{
    _errorMessage = $"Error loading products: {ex.Message}";
}
```

## Performance Optimization

### Lazy Loading

- Components load data on demand
- Virtual scrolling for large lists (planned)

### Caching

- Browser caching for static assets
- API response caching (planned)

### Bundle Optimization

- Blazor's built-in bundling
- Tree shaking removes unused code

## Responsive Design

### Breakpoints

- **Mobile**: < 640px (sm)
- **Tablet**: 640px - 1024px (md)
- **Desktop**: > 1024px (lg)

### Mobile-First Approach

- Base styles for mobile
- Progressive enhancement for larger screens

## Accessibility

### ARIA Attributes

- Proper labeling of form elements
- Screen reader support
- Keyboard navigation

### Semantic HTML

- Proper heading hierarchy
- Semantic elements (nav, main, section)

## Testing

### Unit Testing

- xUnit for .NET code
- bUnit for Blazor components (planned)

### Integration Testing

- End-to-end tests with Selenium (planned)

## Deployment

### Build Process

```bash
cd frontend
dotnet publish -c Release
```

### Static Files

- Served by Blazor WebAssembly
- CDN integration possible

### Environment Configuration

- `appsettings.json` for different environments
- API URL configuration

## Browser Support

- Modern browsers with WebAssembly support
- Chrome, Firefox, Safari, Edge
- IE11 not supported

## Future Enhancements

- Progressive Web App (PWA) features
- Offline support
- Push notifications
- Advanced search with filters
- Wishlist functionality
- Order history
- Real-time cart updates