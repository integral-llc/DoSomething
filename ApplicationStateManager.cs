using System;

namespace DoSomething
{
    /// <summary>
    /// Manages application state transitions and provides state-based information
    /// </summary>
    public class ApplicationStateManager
    {
        private ApplicationState _currentState;
        private long _idleSeconds;
        private const int IDLE_THRESHOLD_SECONDS = 30;

        public event EventHandler<ApplicationState> StateChanged;

        public ApplicationState CurrentState
        {
            get => _currentState;
            private set
            {
                if (_currentState != value)
                {
                    _currentState = value;
                    StateChanged?.Invoke(this, _currentState);
                }
            }
        }

        public long IdleSeconds => _idleSeconds;
        public bool IsRunning => _currentState != ApplicationState.Stopped;
        public bool IsPaused => _currentState == ApplicationState.Paused;

        public ApplicationStateManager()
        {
            _currentState = ApplicationState.Stopped;
            _idleSeconds = 0;
        }

        public void Start()
        {
            CurrentState = ApplicationState.Working;
            _idleSeconds = 0;
        }

        public void Stop()
        {
            CurrentState = ApplicationState.Stopped;
            _idleSeconds = 0;
        }

        public void Pause()
        {
            if (CurrentState == ApplicationState.Working)
            {
                CurrentState = ApplicationState.Paused;
                _idleSeconds = 0;
            }
        }

        public void IncrementIdleTime()
        {
            _idleSeconds++;

            // Auto-resume after idle threshold
            if (CurrentState == ApplicationState.Paused && _idleSeconds >= IDLE_THRESHOLD_SECONDS)
            {
                CurrentState = ApplicationState.Working;
            }
        }

        public string GetStatusText()
        {
            switch (CurrentState)
            {
                case ApplicationState.Stopped:
                    return "Stopped";

                case ApplicationState.Paused:
                    long remainingSeconds = IDLE_THRESHOLD_SECONDS - _idleSeconds;
                    return $"Idle for {_idleSeconds} sec... (resume in {remainingSeconds}s)";

                case ApplicationState.Working:
                    return "Working";

                default:
                    return "Unknown";
            }
        }
    }
}
