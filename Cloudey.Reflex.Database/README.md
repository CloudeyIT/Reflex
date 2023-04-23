# Reflex
## Database

---
Opinionated Entity Framework Core setup for the Reflex framework, using PostgreSQL as the database.

- Auto-discovery of entity configurations in included assemblies (using the Reflex.Core `IncludeAssembly` attribute)
- Automatic Created and Updated fields for entities
- Integration with ASP.NET Core Identity
- Ulid keys used for entities, incl. support for storing Ulid as Guid in the database

### Installation

Install with [NuGet](https://www.nuget.org/packages/Cloudey.Reflex.GraphQL/)

### Setup

To get started, create a database context inheriting from the `ReflexDatabaseContext` class, passing in your Identity entities:

```c#
public class MainDb : ReflexDatabaseContext<User, Role>
{
    public MainDb (DbContextOptions<MainDb> options) : base(options) { }
}
```

Add database configuration to your settings file (see the configuration section in Cloudey.Reflex.Core):

```yaml
Database:
  MainDb: # Must match the database context class name
    Host: 127.0.0.1
    User: MyApp
    Password: MyApp
    Port: 5432
    Database: MyApp
```

Register the database context with DI:

```c#
// Program.cs

builder.Services.AddDatabase<MainDb>(builder.Configuration)
```

Also register the database module with Autofac to enable auto-discovery of triggers:

```c#
public class MyModule : Module
{
	protected override void Load (ContainerBuilder builder)
	{
	    // ...
		builder.RegisterModule<DatabaseModule>();
	}
}
```

Entities in your application must implement the `IEntity` interface. You can optionally create an abstract `Entity` type to avoid re-defining the properties in every entity:

```c#
public abstract class Entity : IEntity
{
	public Ulid Id { get; set; } // Stored as a Guid in the database and automatically converted
	public Guid Guid // The Guid field is not stored in the database and is provided for convenience
	{
		get => Id.ToGuid();
		set => Id = new Ulid(value);
	}
	public DateTime Created { get; set; } = DateTime.MinValue; // Default value for easier seeding
	public DateTime Updated { get; set; } = DateTime.MinValue;
	public Ulid Revision { get; set; }
}
```

If using GraphQL (see Cloudey.Reflex.GraphQL), register the database context with the GraphQL builder:

```c#
var builder = services.AddGraphQLServer()

// ...

builder.RegisterDbContext<MainDb>(DbContextKind.Pooled);
```

### Entities

You can configure entities in your application using the `EntityConfiguration<TEntity>` class:

```c#
public class ToDoItem : Entity // Using the abstract base class, you can also use IEntity if you like
{
    public string Title { get; set; }
    public bool IsComplete { get; set; }
    
    public class Configuration : EntityConfiguration<Hello> {} // Configure this type as a database entity (required)
}
```

### Configuring entities and seeding

You can configure the entities in the Configuration class for each entity:

```c#
public class ToDoItem : Entity // Using the abstract base class, you can also use IEntity if you like
{
    public string Title { get; set; }
    // ...
    
    public class Configuration : EntityConfiguration<Hello> {
        public override void Configure(EntityTypeBuilder<Hello> builder)
        {
            // IMPORTANT: Always call base.Configure(builder) first to make the ID, revisions and timestamps are automatically generated
            base.Configure(builder); 
        
            // Add your own configuration
            builder.HasIndex(x => x.Title).IsUnique();
            
            // Seed some data
            builder.HasData(
                new ToDoItem
                {
                    // ID and revision must be set manually on seeding (EF Core limitation)
                    Id = new Ulid(new Guid("68bbcab1-4aec-4117-8ed9-1101f4768825")),
                    Revision = new Ulid(new Guid("35f9f62b-1d5e-4a37-87b9-b3372771425e")),
                    Title = "My seeded to-do item",
                },
            )
        }
    } 
}
```

### Made by
**[Cloudey](https://cloudey.com)**

### License

Licensed under Apache 2.0.  
**Copyright © 2023 Cloudey IT Ltd**  
Cloudey® is a registered trademark of Cloudey IT Ltd. Use of the trademark is NOT GRANTED under the license of this repository or software package.