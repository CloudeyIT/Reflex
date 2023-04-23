# Reflex
## Core

---

An opinionated framework setup for common functionality.

### Installation

Install with [NuGet](https://www.nuget.org/packages/Cloudey.Reflex.Core/)

For a quick start, use the `AddReflexCore` extension method on the host builder. This will register the logging, configuration, caching, validation, Autofac, and common ASP.NET Core services.

```c#
// Program.cs

var builder = WebApplication.CreateBuilder(args);

builder.AddReflexCore();
```

#### Git

Add the following to your `.gitignore` file to ignore local configuration files:

```gitignore
**/appsettings.Local.*
**/appsettings.*.Local.*
```

### Including assemblies

Reflex makes extensive use of assembly scanning to discover types. You should mark your assemblies with the `IncludeAssembly` attribute to ensure they are included in the scanning process.

```c#
// Anywhere in the assembly
[assembly: IncludeAssembly]
```

### Configuration

#### Usage

```c#
// Program.cs

var builder = WebApplication.CreateBuilder(args);
// ...
builder.AddReflexConfiguration(); // <-- Register configuration
```

#### Configuration files

**Important:** YAML files on the same level always take precedence over JSON files. It is highly recommended to use YAML files exclusively.  

I.e. if you have both `appsettings.json` and `appsettings.yaml` in the same directory, the YAML file will be used.  
If you have `appsettings.yaml` and `appsettings.Development.json`, the JSON file will be used in development. 

Precedence - from highest to lowest:
- `APP__` environment variables
- `appsettings.{Environment}.Local.yaml`
- `appsettings.{Environment}.Local.json`
- `appsettings.{Environment}.yaml`
- `appsettings.{Environment}.json`
- `appsettings.Local.yaml`
- `appsettings.Local.json`
- `appsettings.yaml`
- `appsettings.json`

### Sentry

To enable Sentry, configure the DSN in your application settings (preferably in a .Local.yaml file):

```yaml
Sentry:
  Dsn: YourSentryDsnFromSentryDashboard
```

Then enable logging:

```c#
// Program.cs

var builder = WebApplication.CreateBuilder(args);

// ...

builder.AddReflexLogging();

```

### Logging

This package includes integration with Serilog and Sentry for logging.

Enable logging with:

```c#
// Program.cs

var builder = WebApplication.CreateBuilder(args);

// ...

builder.AddReflexLogging();
```

You can configure the logging level with the configuration key `Logging.Level`.

### Made by
**[Cloudey](https://cloudey.com)**

### License

Licensed under Apache 2.0.  
**Copyright © 2023 Cloudey IT Ltd**  
Cloudey® is a registered trademark of Cloudey IT Ltd. Use of the trademark is NOT GRANTED under the license of this repository or software package.