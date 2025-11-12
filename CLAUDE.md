# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

DoSomething is a Windows Forms application (.NET Framework 4.8) that simulates human-like mouse movement and clicks to prevent idle/away status. It monitors both keyboard and mouse activity using global hooks and pauses for 30 seconds when user activity is detected.

## Build and Run Commands

**Build the project:**
```bash
"/c/Program Files/Microsoft Visual Studio/2022/Community/MSBuild/Current/Bin/amd64/MSBuild.exe" DoSomething.sln //p:Configuration=Debug //v:minimal
```

**Run the application:**
```bash
# After building, run from output directory:
./bin/Debug/DoSomething.exe
```

**Clean build artifacts:**
```bash
"/c/Program Files/Microsoft Visual Studio/2022/Community/MSBuild/Current/Bin/amd64/MSBuild.exe" DoSomething.sln //t:Clean
```

## Architecture

This application follows Gang of Four design patterns with proper separation of concerns:

### Design Patterns Used

1. **Strategy Pattern** - Mouse movement algorithms (IMouseMovementStrategy)
2. **State Pattern** - Application state management (ApplicationStateManager)
3. **Observer Pattern** - Event-driven architecture for state changes and input detection
4. **Dependency Injection** - Components are loosely coupled through constructor injection

### Core Components

#### 1. MainForm.cs (UI Layer)
- **Responsibility**: UI event handling and coordination only
- Initializes all components using dependency injection
- Delegates business logic to specialized classes
- Organized into regions: Timer Events, Input Detection, UI Event Handlers, Application Control, Menu Callbacks, State Management
- ~180 lines (down from 380+ in monolithic version)

#### 2. ApplicationStateManager.cs (State Management)
- **Pattern**: State Pattern implementation
- Manages state transitions: Stopped → Working → Paused → Working
- Provides state-based status text
- Auto-resumes after 30 seconds of idle time
- Fires `StateChanged` events for UI updates

#### 3. MouseController.cs (Mouse Operations)
- **Responsibility**: Orchestrates mouse movement and clicking
- Uses Win32 API: `SetCursorPos` for movement, `mouse_event` for clicks
- Generates random target points within screen boundaries
- Tracks `_shouldIgnoreNextMouseMove` flag to distinguish programmatic vs. user moves
- Performs clicks after 5 seconds of idle time

#### 4. IMouseMovementStrategy.cs & HumanLikeMouseMovement.cs (Strategy Pattern)
- **Pattern**: Strategy Pattern for movement algorithms
- Interface allows swapping movement strategies without changing MouseController
- `HumanLikeMouseMovement` implementation:
  - Uses cubic Bezier curves with random control points
  - Applies ease-in-out cubic easing for natural acceleration/deceleration
  - Adds micro-jitter (±1 pixel) for realism
  - Adaptive step count based on distance

#### 5. MenuBuilder.cs (Menu Construction)
- **Responsibility**: Creates context menu items dynamically
- `BuildInMenu`: Creates 30-minute intervals up to 12 hours
- `BuildAtMenu`: Creates time slots for next 12 hours (crosses midnight boundary)
- Smart formatting: "30 min", "1 hour", "1.5 hours", "At 8:30 PM (tomorrow)"
- Uses callbacks for decoupled communication with MainForm

#### 6. GlobalKeyboardHook.cs & GlobalMouseHook.cs (Input Hooks)
- Low-level hooks using Win32 API (`SetWindowsHookEx`)
- `GlobalKeyboardHook`: Detects any keyboard press system-wide (WH_KEYBOARD_LL = 13)
- `GlobalMouseHook`: Detects mouse movement system-wide (WH_MOUSE_LL = 14)
- Must call `Hook()` to initialize and `Unhook()` to clean up
- Delegates must be stored as fields to prevent garbage collection

#### 7. ApplicationState.cs (Enum)
- Simple enum: Stopped, Working, Paused

### Data Flow

```
User Input (Keyboard/Mouse)
    ↓
GlobalKeyboardHook/GlobalMouseHook
    ↓
MainForm.OnKeyboardActivity / OnMouseActivity
    ↓
ApplicationStateManager.Pause()
    ↓
StateChanged Event → MainForm.UpdateStatusLabel()
    ↓
Timer Tick (every second)
    ↓
ApplicationStateManager.IncrementIdleTime()
    ↓
(After 30 sec) Auto-resume → Working state
    ↓
MouseController.ProcessTick()
    ↓
HumanLikeMouseMovement.GeneratePath()
    ↓
Win32 SetCursorPos
```

### Key Behaviors

- **Pause Duration**: 30 seconds for both keyboard and mouse activity
- **Context Menus**: Dynamic generation covers next 12 hours (crosses midnight)
- **Auto-minimize**: Application minimizes when started
- **Persistent Settings**: Last timeout value saved in user settings
- **Human-like Movement**: Bezier curves with easing and micro-jitter

### Win32 API Dependencies

- `SetCursorPos` - Move mouse cursor (MouseController.cs)
- `mouse_event` - Simulate mouse clicks (MouseController.cs)
- `SetWindowsHookEx` / `UnhookWindowsHookEx` - Global input hooks
- `GetModuleHandle` - Required for hook setup
- `CallNextHookEx` - Pass hook events to next handler

### Namespace Structure

- **DoSomething**: Core business logic classes
- **DoSomethingEx**: UI layer (MainForm, Program)
- Assembly name: `DoSomething`

## Development Notes

### Adding New Movement Strategies

1. Create a class implementing `IMouseMovementStrategy`
2. Implement `GeneratePath(Point start, Point end)` method
3. Inject into MouseController in MainForm constructor

Example:
```csharp
var strategy = new LinearMouseMovement(); // or new HumanLikeMouseMovement()
_mouseController = new MouseController(strategy, _stateManager);
```

### Important Considerations

- Hook delegates must be stored as instance fields to prevent GC
- Always call `Unhook()` in `FormClosing` event
- The `_shouldIgnoreNextMouseMove` flag prevents self-detection loops
- Icon file must be in output directory (CopyToOutputDirectory = PreserveNewest)
- Settings are persisted automatically via .NET user settings

## SOLID Principles Applied

- **Single Responsibility**: Each class has one clear purpose
- **Open/Closed**: Strategy pattern allows extending without modifying
- **Liskov Substitution**: Any IMouseMovementStrategy implementation works
- **Interface Segregation**: Small, focused interfaces
- **Dependency Inversion**: MainForm depends on abstractions, not concrete implementations
