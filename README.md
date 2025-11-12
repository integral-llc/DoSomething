# DoSomething

A Windows desktop application that prevents idle/away status by simulating natural, human-like mouse movements and clicks. The application intelligently pauses when real user activity is detected and resumes after a configurable idle period.

[![Build Status](https://github.com/integral-llc/DoSomething/actions/workflows/build-and-release.yml/badge.svg)](https://github.com/integral-llc/DoSomething/actions)
[![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.8-blue.svg)](https://dotnet.microsoft.com/)
[![Windows](https://img.shields.io/badge/platform-Windows%207%2B-blue.svg)](https://www.microsoft.com/windows)
[![License](https://img.shields.io/badge/license-GPL--3.0-green.svg)](LICENSE)

## Table of Contents

- [Features](#features)
- [Why DoSomething?](#why-dosomething)
- [Installation](#installation)
- [Usage](#usage)
- [Architecture](#architecture)
- [Building from Source](#building-from-source)
- [Creating Releases](#creating-releases)
- [Configuration](#configuration)
- [Technical Details](#technical-details)
- [Contributing](#contributing)
- [License](#license)

## Features

### Core Functionality
- **Human-like Mouse Movement**: Uses cubic Bezier curves with easing functions to simulate natural mouse movement patterns
- **Intelligent Activity Detection**: Monitors both keyboard and mouse activity system-wide using low-level hooks
- **Smart Auto-pause**: Automatically pauses for 30 seconds when real user activity is detected
- **Visual Feedback**: Taskbar icon overlay shows current application state (Working/Paused/Stopped)
- **Flexible Scheduling**: Set timeout with quick presets ("In X hours" or "At HH:MM")
- **Persistent Settings**: Remembers your last timeout configuration

### Advanced Features
- **Micro-jitter**: Adds subtle pixel variations to simulate natural hand tremor
- **Variable Speed**: Implements ease-in-out acceleration for realistic movement
- **Random Trajectories**: Each mouse path is unique with randomized control points
- **Auto-minimize**: Minimizes to taskbar when running to stay out of your way
- **Clean Architecture**: Fully testable business logic separated from UI

## Why DoSomething?

Modern work environments often use presence detection systems that mark users as "away" or "idle" after a period of inactivity. DoSomething helps maintain an "active" status during legitimate breaks, reading sessions, or when monitoring long-running processes, while ensuring it doesn't interfere with actual work by intelligently pausing when you return.

**Use Cases:**
- Monitoring long-running tasks or dashboards
- Reading documentation or code reviews on another screen
- Attending to non-computer tasks while remaining available
- Preventing auto-lock during presentations or demos

## Installation

### Requirements
- Windows 7 or later (Windows 10/11 recommended)
- .NET Framework 4.8
- Administrator privileges (for global keyboard/mouse hooks)

### Download
1. Download the latest release from the [Releases](../../releases) page
2. Extract the ZIP file to a folder of your choice
3. Ensure `appIcon.ico` is in the same directory as `DoSomething.exe`
4. Run `DoSomething.exe`

## Usage

### Basic Operation

1. **Set Timeout Duration**
   - Enter minutes manually in the "Stop After (min)" field
   - OR right-click the field for quick presets:
     - **In**: 30-minute intervals up to 12 hours
     - **At**: Specific times in 30-minute increments (covers next 12 hours)

2. **Start the Application**
   - Click the "Start" button
   - Application minimizes automatically
   - Mouse will move in natural patterns
   - Status shown in taskbar icon overlay

3. **Application Behavior**
   - **Green indicator**: Currently working (moving mouse)
   - **Orange indicator**: Paused due to user activity (resumes in 30 seconds)
   - **No indicator**: Stopped

4. **Stop the Application**
   - Restore the window from taskbar
   - Click "Stop" button
   - OR wait for auto-stop timeout

### Status Indicators

The application provides real-time status in the main window:
- **"Stopped"**: Not running
- **"Working"**: Actively moving mouse
- **"Idle for N sec... (resume in Xs)"**: Paused, showing countdown to resume

## Architecture

DoSomething follows **Gang of Four design patterns** with clean separation of concerns:

### Design Patterns
- **Strategy Pattern**: Mouse movement algorithms (easily extensible)
- **State Pattern**: Application state management (Stopped/Working/Paused)
- **Observer Pattern**: Event-driven architecture for UI updates
- **MVP Pattern**: ApplicationDriver (Presenter) commands passive MainForm (View)

### Project Structure

```
DoSomething/
├── .github/
│   └── workflows/
│       └── build-and-release.yml     # CI/CD automation
├── docs/
│   └── CLAUDE.md                     # Development documentation
├── src/
│   └── DoSomething/
│       ├── Core/                     # Business logic
│       │   ├── ApplicationDriver.cs  # Central orchestrator
│       │   ├── ApplicationState.cs   # State enum
│       │   └── ApplicationStateManager.cs  # State management
│       ├── Forms/                    # UI layer
│       │   ├── MainForm.cs           # Passive view (zero business logic)
│       │   ├── MainForm.Designer.cs
│       │   └── MainForm.resx
│       ├── Hooks/                    # Input detection
│       │   ├── GlobalKeyboardHook.cs # System-wide keyboard detection
│       │   └── GlobalMouseHook.cs    # System-wide mouse detection
│       ├── MouseMovement/            # Movement strategies
│       │   ├── IMouseMovementStrategy.cs      # Strategy interface
│       │   ├── HumanLikeMouseMovement.cs      # Bezier curve implementation
│       │   └── MouseController.cs             # Mouse operations coordinator
│       ├── UI/                       # UI helpers
│       │   ├── MenuBuilder.cs        # Context menu generation
│       │   └── TaskbarIconManager.cs # Taskbar overlay management
│       ├── Win32/                    # Native API
│       │   └── NativeMethods.cs      # Win32 API P/Invoke (DRY)
│       ├── Properties/               # Assembly info, resources, settings
│       ├── Program.cs                # Application entry point
│       ├── App.config
│       ├── appIcon.ico
│       └── DoSomething.csproj
├── DoSomething.sln
├── LICENSE
└── README.md
```

### Key Components

**ApplicationDriver** - Owns all business logic, fully testable without UI
- Coordinates all components
- Manages internal timer (System.Threading.Timer)
- Commands UI via events (not direct calls)

**MainForm** - Pure view layer, contains zero business logic
- Wires up event handlers
- Updates UI elements when commanded
- Passes user input to driver

## Building from Source

### Prerequisites
- Visual Studio 2019 or later (Community Edition is fine)
- .NET Framework 4.8 SDK

### Build Instructions

**Using Visual Studio:**
1. Open `DoSomething.sln`
2. Build → Build Solution (Ctrl+Shift+B)
3. Output: `src\DoSomething\bin\Debug\DoSomething.exe`

**Using MSBuild (Command Line):**
```bash
# Locate MSBuild (typically installed with Visual Studio)
"C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\amd64\MSBuild.exe" ^
  DoSomething.sln ^
  //p:Configuration=Release ^
  //v:minimal
```

**Clean Build:**
```bash
msbuild DoSomething.sln //t:Clean
```

## Creating Releases

The project uses GitHub Actions for automated builds and releases. Every push to the main/master branch triggers a CI build, and tagged commits create official releases.

### Automated Release Process

When you push a tag starting with `v` (e.g., `v1.0.0`), GitHub Actions will automatically:
1. Build the Release configuration
2. Package the application with all required files
3. Create a ZIP archive (e.g., `DoSomething-v1.0.0.zip`)
4. Publish a GitHub Release with download links
5. Generate release notes

### Creating a New Release

**Step 1: Ensure code is ready**
```bash
# Make sure all changes are committed
git status

# Build and test locally
msbuild DoSomething.sln //p:Configuration=Release
```

**Step 2: Create and push a version tag**
```bash
# Create an annotated tag with semantic versioning
git tag v1.0.0 -m "Release version 1.0.0"

# Push the tag to GitHub (this triggers the release workflow)
git push origin v1.0.0
```

**Step 3: Monitor the build**
- Visit the [Actions tab](../../actions) on GitHub
- Wait for the "Build and Release" workflow to complete
- The release will appear in the [Releases page](../../releases)

### Release Package Contents

Each release ZIP file includes:
- `DoSomething.exe` - Main executable
- `DoSomething.exe.config` - Application configuration
- `appIcon.ico` - Application icon
- `README.md` - Documentation
- `LICENSE` - GPL-3.0 license

### Versioning Guidelines

Follow [Semantic Versioning](https://semver.org/) (MAJOR.MINOR.PATCH):
- **MAJOR** (v2.0.0): Breaking changes or major rewrites
- **MINOR** (v1.1.0): New features, backwards compatible
- **PATCH** (v1.0.1): Bug fixes, backwards compatible

### Pre-release Versions

For beta or release candidate versions:
```bash
git tag v1.0.0-beta.1 -m "Beta release"
git push origin v1.0.0-beta.1
```

## Configuration

### Settings Storage
Settings are stored in: `%LocalAppData%\DoSomething\`
- Last used timeout value
- Persisted automatically on each run

### Customization

To create a custom mouse movement strategy:

```csharp
// 1. Implement IMouseMovementStrategy
public class LinearMouseMovement : IMouseMovementStrategy
{
    public IEnumerable<Point> GeneratePath(Point start, Point end)
    {
        // Your custom movement logic
    }
}

// 2. Inject in ApplicationDriver constructor (src/DoSomething/Core/ApplicationDriver.cs:36)
var movementStrategy = new LinearMouseMovement();
_mouseController = new MouseController(movementStrategy, _stateManager);
```

## Technical Details

### System Requirements
- **Hooks**: Uses `SetWindowsHookEx` with `WH_KEYBOARD_LL` and `WH_MOUSE_LL`
- **Taskbar Integration**: Uses `ITaskbarList3` COM interface (Windows 7+)
- **Threading**: Business logic runs on background thread (System.Threading.Timer)

### Win32 API Usage
- `SetCursorPos`: Move mouse cursor
- `mouse_event`: Simulate clicks
- `SetWindowsHookEx/UnhookWindowsHookEx`: Global input hooks
- `GetModuleHandle`: Module handle for hook installation

### Movement Algorithm
**Cubic Bezier Curves** with random control points:
- 2 control points positioned at 33% and 66% along path
- Random perpendicular offset (up to 20% of distance)
- Ease-in-out cubic easing for acceleration/deceleration
- ±1 pixel micro-jitter for realism
- Adaptive step count based on distance

### Performance
- **CPU Usage**: <1% (idles when paused)
- **Memory**: ~15-20 MB
- **Timer Precision**: 1 second interval
- **Hook Overhead**: Minimal (native Win32 callbacks)

## Contributing

Contributions are welcome! Areas for improvement:

### Potential Enhancements
- [ ] Alternative movement strategies (linear, spiral, etc.)
- [ ] System tray mode (no window, not sure if is needed)
- [ ] Activity logging/statistics
- [ ] Screen capture on specific events (debatable)
- [ ] Configuration UI for advanced settings

### Development Guidelines
1. Follow existing architecture (SOLID principles)
2. Keep UI layer passive (zero business logic in MainForm)
3. Add new strategies by implementing `IMouseMovementStrategy`
4. Maintain DRY principle (centralized Win32 API in NativeMethods)
5. All business logic must be testable without UI

### Submitting Changes
1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the GNU General Public License v3.0 - see the [LICENSE](LICENSE) file for details.

### GPL-3.0 Summary
- ✅ Commercial use
- ✅ Modification
- ✅ Distribution
- ✅ Patent use
- ✅ Private use
- ⚠️ Same license required for derivatives
- ⚠️ Source code must be made available
- ⚠️ Changes must be documented

## Disclaimer

**Use Responsibly**: This tool is designed for legitimate purposes such as maintaining availability during monitoring tasks or preventing auto-lock during presentations. Users are responsible for ensuring compliance with their organization's policies regarding activity monitoring and presence status.

**No Warranty**: This software is provided "as is" without warranty of any kind. Use at your own risk.

---

**Project Status**: Active Development
**Maintainer**: See [Contributors](../../graphs/contributors)
**Issues**: Report bugs via [GitHub Issues](../../issues)
**Discussions**: Join the conversation in [Discussions](../../discussions)
