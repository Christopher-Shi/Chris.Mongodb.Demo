# AGENTS.md - MongoDB Demo API

This document contains guidelines for agentic coding agents working in this .NET 10.0 MongoDB Web API repository.

## Project Overview

**Tech Stack:**
- .NET 10.0 Web API with implicit usings enabled
- MongoDB.Driver v3.6.0 for database operations
- Swashbuckle.AspNetCore for OpenAPI/Swagger documentation
- Nullable reference types enabled

**Architecture:**
- Clean separation: Controllers → Services → Database Context
- Dependency injection pattern throughout
- Repository pattern via `MongoDbContext`
- Async/await for all database operations

## Build & Development Commands

```bash
# Build the project
dotnet build

# Run in development mode
dotnet run

# Run with specific profile
dotnet run --launch-profile http
dotnet run --launch-profile https

# Clean build artifacts
dotnet clean

# Restore packages
dotnet restore

# (Note: No tests currently configured in this demo project)
```

**Development URLs:**
- HTTP: http://localhost:5023
- HTTPS: https://localhost:7140
- Swagger UI: https://localhost:7140/swagger

## Code Style Guidelines

### File & Namespace Organization
- **Namespace pattern**: `Chris.Mongodb.Demo.{FolderName}`
- **Folder structure**: 
  - `Controllers/` - API endpoints
  - `Services/` - Business logic layer
  - `Entities/` - Data models
  - `Properties/` - .NET configuration files

### Naming Conventions
- **Classes**: PascalCase (e.g., `UserService`, `UserController`)
- **Methods**: PascalCase (e.g., `GetUserByIdAsync`, `AddUserAsync`)
- **Properties**: PascalCase (e.g., `Id`, `Name`, `CreateTime`)
- **Fields**: _camelCase with underscore prefix (e.g., `_userCollection`)
- **Collections**: Plural names in MongoDB (`users`, `products`)

### Async Patterns
- All database operations MUST be async with `Async` suffix
- Use `async Task<ActionResult<T>>` for controller methods
- Use `async Task<T>` for service methods returning data
- Use `async Task<bool>` for service methods indicating success/failure

### MongoDB Entity Patterns
```csharp
[BsonId]
[BsonRepresentation(BsonType.String)]
public required string Id { get; set; }

[BsonElement("fieldName")] 
public required string PropertyName { get; set; }

[BsonDateTimeOptions(Kind = DateTimeKind.Local)]
public DateTime CreateTime { get; set; } = DateTime.Now;
```

### Service Layer Patterns
```csharp
public class EntityService
{
    private readonly IMongoCollection<Entity> _collection;
    
    public EntityService(MongoDbContext context)
    {
        _collection = context.GetCollection<Entity>("entities");
    }
    
    public async Task<bool> UpdateEntityAsync(string id, Entity updateEntity)
    {
        var filter = Builders<Entity>.Filter.Eq(e => e.Id, id);
        var update = Builders<Entity>.Update
            .Set(e => e.Property1, updateEntity.Property1)
            .Set(e => e.Property2, updateEntity.Property2);
            
        var result = await _collection.UpdateOneAsync(filter, update);
        return result.MatchedCount > 0;
    }
}
```

### Controller Patterns
```csharp
[ApiController]
[Route("api/[controller]")]
public class EntityController : Controller
{
    private readonly EntityService _service;
    
    public EntityController(EntityService service)
    {
        _service = service;
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<Entity>> GetEntity(string id)
    {
        var entity = await _service.GetEntityByIdAsync(id);
        if (entity == null)
            return NotFound("The entity does not exist");
        return Ok(entity);
    }
}
```

## Error Handling

### HTTP Response Patterns
- **Not Found**: `NotFound("descriptive message")`
- **Conflict**: `Conflict("descriptive message")`
- **Bad Request**: `BadRequest("descriptive message")`
- **Created**: `CreatedAtAction(nameof(MethodName), new { id }, entity)`
- **No Content**: `NoContent()`
- **Success**: `Ok(data)`

### Validation Patterns
- Check for null/empty in GET operations
- Validate ID consistency in PUT operations
- Check for existing records in POST operations
- Return boolean success indicators from service layer

## Configuration

### MongoDB Connection
Configuration in `appsettings.json`:
```json
{
  "MongoDB": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "foo"
  }
}
```

### Service Registration (in Program.cs)
```csharp
builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddScoped<EntityService>();
```

## Database Query Patterns

### Filter Building
```csharp
var filter = Builders<Entity>.Filter.Eq(e => e.Id, id);
var filter = Builders<Entity>.Filter.Gt(e => e.Age, minAge);
var filter = Builders<Entity>.Filter.Lt(e => e.Age, maxAge);
```

### Sorting
```csharp
await _collection.Find(filter)
    .SortByDescending(e => e.Age)
    .ToListAsync();
```

## Important Notes

- **Required Properties**: Use `required` keyword for non-nullable properties
- **String IDs**: MongoDB ObjectId represented as string via `[BsonRepresentation(BsonType.String)]`
- **Chinese Comments**: Existing code includes Chinese comments - maintain this pattern if updating existing files
- **No Tests**: This demo project lacks test configuration - add xUnit/nUnit if implementing tests
- **Singleton Context**: `MongoDbContext` is registered as singleton - don't create multiple instances
- **Scoped Services**: Business services are scoped per request

## Common Gotchas

- Always use `async Task<ActionResult<T>>` for API endpoints
- MongoDB queries return empty collections, not null
- String ID validation should be case-sensitive
- DateTime fields default to local time in this project
- `ImplicitUsings` enabled - no need for common using statements like `System`