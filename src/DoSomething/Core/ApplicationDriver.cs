using System;
using System.Threading;

namespace DoSomething
{
    /// <summary>
    /// Central orchestrator that manages all application business logic
    /// This class is fully testable without any UI dependencies
    /// </summary>
    public class ApplicationDriver : IDisposable
    {
        private readonly ApplicationStateManager _stateManager;
        private readonly MouseController _mouseController;
        private readonly GlobalKeyboardHook _keyboardHook;
        private readonly GlobalMouseHook _mouseHook;
        private TaskbarIconManager _taskbarIconManager;
        private Timer _workerTimer;
        private int _autoStopTimeoutMinutes;
        private int _tickCount;
        private bool _disposed;

        // Events for UI updates
        public event EventHandler<string> StatusTextChanged;
        public event EventHandler<ApplicationState> StateChanged;

        // Events for UI commands (Presenter → View communication)
        public event EventHandler MinimizeWindow;
        public event EventHandler RestoreWindow;
        public event EventHandler<string> UpdateButtonText;
        public event EventHandler<int> SaveTimeout;

        public ApplicationDriver()
        {
            // Initialize core components
            _stateManager = new ApplicationStateManager();
            var movementStrategy = new HumanLikeMouseMovement();
            _mouseController = new MouseController(movementStrategy, _stateManager);

            // Initialize input hooks
            _keyboardHook = new GlobalKeyboardHook();
            _keyboardHook.KeyboardPressed += OnKeyboardActivity;
            _keyboardHook.Hook();

            _mouseHook = new GlobalMouseHook();
            _mouseHook.MouseMoved += OnMouseActivity;
            _mouseHook.Hook();

            // Subscribe to state changes
            _stateManager.StateChanged += OnStateChanged;
        }

        /// <summary>
        /// Initializes taskbar icon manager (requires window handle from UI)
        /// </summary>
        public void InitializeTaskbarIcon(IntPtr windowHandle)
        {
            _taskbarIconManager = new TaskbarIconManager(windowHandle);
            _taskbarIconManager.UpdateIcon(_stateManager.CurrentState);
        }

        /// <summary>
        /// Toggles between running and stopped states
        /// </summary>
        public void ToggleRunning(int timeoutMinutes)
        {
            if (_stateManager.IsRunning)
            {
                Stop();
            }
            else
            {
                Start(timeoutMinutes);
            }
        }

        /// <summary>
        /// Starts the application with specified timeout in minutes
        /// </summary>
        private void Start(int timeoutMinutes)
        {
            _autoStopTimeoutMinutes = timeoutMinutes;
            _tickCount = 0;

            _mouseController.Initialize();
            _stateManager.Start();

            // Start internal timer (1 second interval)
            _workerTimer = new Timer(OnTimerTick, null, 0, 1000);

            // Tell UI to update button and save timeout
            UpdateButtonText?.Invoke(this, "Stop");
            SaveTimeout?.Invoke(this, timeoutMinutes);

            // Tell UI to minimize window
            MinimizeWindow?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Stops the application
        /// </summary>
        private void Stop()
        {
            // Stop internal timer
            _workerTimer?.Dispose();
            _workerTimer = null;

            _stateManager.Stop();

            // Tell UI to update button
            UpdateButtonText?.Invoke(this, "Start");

            // Tell UI to restore window
            RestoreWindow?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Internal timer callback - processes tick on background thread
        /// </summary>
        private void OnTimerTick(object state)
        {
            ProcessTick();
        }

        /// <summary>
        /// Processes a timer tick - should be called every second
        /// </summary>
        public void ProcessTick()
        {
            _tickCount++;

            // Check if auto-stop timeout reached (convert minutes to seconds)
            if (_tickCount >= _autoStopTimeoutMinutes * 60)
            {
                Stop();
                return;
            }

            _stateManager.IncrementIdleTime();
            _mouseController.ProcessTick();
            RaiseStatusTextChanged();
        }

        /// <summary>
        /// Gets current status text for UI display
        /// </summary>
        public string GetStatusText()
        {
            return _stateManager.GetStatusText();
        }

        private void OnKeyboardActivity(object sender, EventArgs e)
        {
            if (_stateManager.IsRunning)
            {
                _stateManager.Pause();
            }
        }

        private void OnMouseActivity(object sender, EventArgs e)
        {
            // Ignore mouse moves generated by our application
            if (_mouseController.ShouldIgnoreNextMouseMove)
            {
                return;
            }

            if (_stateManager.IsRunning && !_stateManager.IsPaused)
            {
                _stateManager.Pause();
            }
        }

        private void OnStateChanged(object sender, ApplicationState newState)
        {
            _taskbarIconManager?.UpdateIcon(newState);
            StateChanged?.Invoke(this, newState);
            RaiseStatusTextChanged();
        }

        private void RaiseStatusTextChanged()
        {
            StatusTextChanged?.Invoke(this, GetStatusText());
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _workerTimer?.Dispose();
            _keyboardHook?.Unhook();
            _mouseHook?.Unhook();
            _taskbarIconManager?.Dispose();

            _disposed = true;
        }
    }
}
