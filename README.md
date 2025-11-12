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
├── ApplicationDriver.cs          # Central orchestrator (business logic)
├── ApplicationState[Manager].cs  # State management
├── MouseController.cs            # Mouse operations coordinator
├── HumanLikeMouseMovement.cs     # Bezier curve movement strategy
├── IMouseMovementStrategy.cs     # Strategy interface
├── GlobalKeyboardHook.cs         # System-wide keyboard detection
├── GlobalMouseHook.cs            # System-wide mouse detection
├── TaskbarIconManager.cs         # Taskbar overlay management
├── MenuBuilder.cs                # Context menu generation
├── NativeMethods.cs              # Win32 API P/Invoke declarations
├── MainForm.cs                   # Thin UI layer (passive view)
└── Program.cs                    # Application entry point
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
3. Output: `bin\Debug\DoSomething.exe`

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

// 2. Inject in ApplicationDriver constructor (ApplicationDriver.cs:36)
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
- [ ] Click pattern randomization
- [ ] Multi-monitor support
- [ ] Configurable pause duration
- [ ] System tray mode (no window)
- [ ] Activity logging/statistics
- [ ] Screen capture on specific events
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
