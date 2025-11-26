# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Rhino.Inside.AutoCAD is an AutoCAD plugin that enables Rhino 7 and Grasshopper to run inside AutoCAD. The project is developed by Bimorph Digital Engineering and creates an AutoCAD .bundle plugin that loads Rhino In-Process within AutoCAD's runtime.

**Prerequisites:**
- Rhino 7 must be installed (the extension checks registry key: `HKEY_LOCAL_MACHINE\SOFTWARE\McNeel\Rhinoceros\7.0\Install`)
- AutoCAD 2024 (default debug configuration)
- .NET Framework 4.8

## Build Commands

```bash
# Build entire solution
dotnet build Rhino.Inside.AutoCAD.sln

# Build specific configuration
dotnet build Rhino.Inside.AutoCAD.sln -c Debug
dotnet build Rhino.Inside.AutoCAD.sln -c Release

# Clean build
dotnet clean Rhino.Inside.AutoCAD.sln
dotnet build Rhino.Inside.AutoCAD.sln --no-incremental
```

## Running Tests

```bash
# Run all tests
dotnet test

# Run tests in specific project
dotnet test src/Rhino.Inside.AutoCAD.UserTests/Rhino.Inside.AutoCAD.UserTests.csproj
```

## Deployment

The build process automatically deploys to two locations via PostBuild targets:

1. **Local Installation** (for testing): `%APPDATA%\Autodesk\ApplicationPlugins\Rhino.Inside.AutoCAD.bundle\`
2. **Deployment Package**: `Deployment\OutputFiles\Rhino.Inside.AutoCAD.{version}\`

The PostBuild events copy:
- Compiled DLLs and PDBs to versioned Win64 folder
- Icons to Icons/, Icons/Small32/, and Icons/Large512/
- Toolbar CUIX file
- PackageContents.xml manifest
- JSON resources

## Solution Architecture

The solution is organized into layered solution folders:

### Core Layer
- **Rhino.Inside.AutoCAD.Core** - Core interfaces and enums
  - Defines interfaces for the entire application (IRhinoInsideAutoCadApplication, IRhinoInsideAutoCadSettingsManager, etc.)
  - Contains enums: ButtonApplicationId, RhinoInsideMode, unit system types, etc.
  - No external API dependencies (pure contracts)

### Infrastructure Layer
- **Rhino.Inside.AutoCAD.Services** - Settings management and file resources
  - Implements SettingManager for ApplicationSettings, UserSettings, and Core settings
  - JSON-based configuration system using System.Text.Json
  - FileResourceManager and ApplicationDirectories management
  - Resources folder contains JSON configuration files

- **Rhino.Inside.AutoCAD.Interop** - Rhino ↔ AutoCAD interop bridge
  - RhinoInsideExtension: Singleton that launches Rhino In-Process via RhinoCore
  - InteropService: Manages data exchange between Rhino and AutoCAD
  - Unit conversion, entity comparers, object management
  - Database managers (DataTagDatabaseManager, DocumentFile)

- **Rhino.Inside.AutoCAD.UserTests** - User testing infrastructure
  - PowerShell scripts register supported applications and tests
  - UserTests.json defines test cases

### Addin Layer
- **Rhino.Inside.AutoCAD.Applications** - Main AutoCAD plugin entry point
  - RhinoInsideAutoCadExtension: IExtensionApplication (AutoCAD loads this on startup)
  - RhinoInsideAutoCadCommands: Exposes RHINO and GRASSHOPPER commands
  - RhinoInsideAutoCadApplication: Main application orchestrator
  - PackageContents.xml: AutoCAD bundle manifest

### Applications Layer
- **Rhino.Inside.AutoCAD.RhinoLauncher** - Rhino UI launcher
  - RhinoLauncherMain: Entry point for Rhino windowed mode

- **Rhino.Inside.AutoCAD.GrasshopperLauncher** - Grasshopper UI launcher
  - GrasshopperLauncherMain: Entry point for Grasshopper

Both launchers inherit from ApplicationMainBase and use Autofac for dependency injection.

## Key Architectural Patterns

### Extension Loading Flow
1. AutoCAD loads `RhinoInsideAutoCadExtension.Initialize()` on startup via `[ExtensionApplication]` attribute
2. Creates singleton `RhinoInsideAutoCadApplication` which:
   - Initializes Bootstrapper (from Bimorph.Core.Services)
   - Creates SettingManager and FileResourceManager
   - Sets up SoftwareUpdater
3. Commands (RHINO, GRASSHOPPER) are registered via `[CommandMethod]` attributes

### Command Execution Flow
When user runs RHINO or GRASSHOPPER command:
1. `RhinoInsideAutoCadCommands.RunApplicationWithRhino()` is called
2. Checks if Rhino validation passed (verifies Rhino 7 installation)
3. Calls `RhinoInsideExtension.Instance.Initialize()` to start Rhino In-Process
4. Creates InteropService for Rhino ↔ AutoCAD communication
5. Launches appropriate mainline (RhinoLauncherMain or GrasshopperLauncherMain)
6. Mainline sets up Autofac container and runs the application

### Rhino Assembly Resolution
RhinoInsideExtension uses a static constructor with AssemblyResolve to load RhinoCommon.dll from Rhino's installation directory. This allows the plugin to use Rhino APIs without bundling Rhino assemblies.

### Settings Architecture
Three-tier settings system:
- **Core Settings**: Application-wide configuration (SettingsCore.json)
- **Application Settings**: Per-application configuration (ApplicationsSettingsCore.json)
- **User Settings**: Per-user preferences (UserSettings.json) - persisted via SaveUserSettings()

## Package Management

Uses Central Package Management (CPM) via Directory.Packages.props:
- All package versions defined centrally
- Key dependencies: AutoCAD.NET 24.3.0, RhinoCommon 7.36.x, Grasshopper 7.36.x
- Bimorph.Core.Services provides base application framework
- Material Design Themes for UI
- Autofac for IoC

Note: AutoCAD and Rhino packages use `<ExcludeAssets>runtime</ExcludeAssets>` since they're provided by the host applications.

## Development Notes

### Debugging
The Applications project is configured to launch AutoCAD 2024 on debug:
```xml
<StartProgram>C:\Program Files\Autodesk\AutoCAD 2024\acad.exe</StartProgram>
```

### Adding New Commands
1. Add method to `RhinoInsideAutoCadCommands` with `[CommandMethod("COMMANDNAME")]` attribute
2. Update PackageContents.xml `<Commands>` section
3. Follow existing pattern: call `RunApplicationWithRhino()` with appropriate mainline factory

### Unit System Handling
The application uses InteropConstants.InternalUnitSystem for Rhino-AutoCAD conversions. Unit conversion logic is in Rhino.Inside.AutoCAD.Interop/Units/ and Extensions/UnitsValueExtensions.cs.

### Logging
Uses LoggerService.Instance from Bimorph.Core.Services for error logging throughout the application.

## Common Issues

- **Rhino Not Found Error**: Application checks registry for Rhino 7 installation. Validation errors are shown via CADApplication.ShowAlertDialog().
- **Assembly Load Failures**: Ensure Rhino 7 is installed before debugging. The AssemblyResolve handler in RhinoInsideExtension handles RhinoCommon loading.
- **Build Path Issues**: PostBuild events use Directory.Build.props variables. Ensure all path variables (AppRootDir, DeploymentPackage, etc.) are correctly resolved.
