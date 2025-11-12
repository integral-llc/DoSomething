# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

DoSomething is a Windows Forms application (.NET Framework 4.8) that simulates mouse movement and clicks to prevent idle/away status. It monitors keyboard activity using a global keyboard hook and performs automated mouse clicks when the system is idle for more than 5 seconds.

## Build and Run Commands

**Build the project:**
```bash
msbuild DoSomething.sln /p:Configuration=Debug
# Or for Release build:
msbuildDoSomething.sln /p:Configuration=Release
```

**Run the application:**
```bash
# After building, run from output directory:
./bin/Debug/DoSomething.exe
# Or for Release:
./bin/Release/DoSomething.exe
```

**Clean build artifacts:**
```bash
msbuild DoSomething.sln /t:Clean
```

## Architecture

### Core Components

1. **MainForm.cs** - Main UI and application logic
   - Entry point form launched by Program.cs
   - Implements mouse movement simulation using Win32 API (`SetCursorPos`, `mouse_event`)
   - Uses Bresenham's line algorithm (`YieldLinePoints`) to generate smooth mouse movement paths
   - Tracks idle time via `IdleTickSeconds` counter
   - Performs left-click after 5+ seconds of idle time (MainForm.cs:119-123)
   - Two timers: `tmrWorker` (periodic tick for idle check) and `tmrStop` (auto-stop after user-configured duration)

2. **GlobalKeyboardHook.cs** - System-wide keyboard monitoring
   - Low-level keyboard hook implementation using Win32 API
   - Hooks into Windows message queue via `SetWindowsHookEx` with `WH_KEYBOARD_LL` (constant 13)
   - Fires `KeyboardPressed` event on any keyboard input system-wide
   - Resets idle counter in MainForm when keyboard activity detected
   - Must call `Hook()` to start and `Unhook()` to clean up properly

3. **Form1.cs** - Unused legacy form
   - Empty form class, not currently used in the application flow

### Key Behaviors

- **Idle Detection**: Resets to 0 on any keyboard press via global hook (MainForm.cs:207-211)
- **Random Point Generation**: `GetPoint()` ensures new mouse positions are at least `minDistance` pixels from previous position
- **Auto-minimize**: Application minimizes when started (MainForm.cs:224)
- **Persistent Settings**: Stores last timeout value in `Settings.Default.LastTimeout` (App.config user settings)
- **Dynamic Menu Generation**: Context menu dynamically generates "In X hours" and "At HH:MM" time options

### Win32 API Dependencies

The application relies on P/Invoke calls to user32.dll and kernel32.dll:
- `SetCursorPos` - Move mouse cursor
- `mouse_event` - Simulate mouse clicks (flags: MOUSEEVENTF_LEFTDOWN, MOUSEEVENTF_LEFTUP)
- `SetWindowsHookEx` / `UnhookWindowsHookEx` - Global keyboard hook
- `GetModuleHandle` - Required for keyboard hook setup

### Namespace Inconsistency

Note: The project has mixed namespaces:
- Program.cs and MainForm.cs use `DoSomethingEx`
- Form1.cs and GlobalKeyboardHook.cs use `DoSomething`
- Assembly name is `DoSomething` (DoSomething.csproj:10)

## Development Notes

- The application requires `appIcon.ico` in the output directory (set to copy on build)
- Settings persistence uses .NET Framework's built-in user settings system
- Global keyboard hook requires proper cleanup (`Unhook()` called in `MainForm_FormClosing`)
- The `_proc` delegate in GlobalKeyboardHook must be stored as a field to prevent garbage collection
