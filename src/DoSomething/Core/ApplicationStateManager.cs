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
        private int _idleThresholdSeconds;

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

        public ApplicationStateManager(int pauseDurationSeconds = 30)
        {
            _currentState = ApplicationState.Stopped;
            _idleSeconds = 0;
            _idleThresholdSeconds = pauseDurationSeconds;
        }

        /// <summary>
        /// Updates the pause duration (idle threshold in seconds)
        /// </summary>
        public void SetPauseDuration(int seconds)
        {
            _idleThresholdSeconds = seconds;
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

        /// <summary>
        /// Resets the idle timer without changing state
        /// Used to reset timer when user activity is detected while already paused
        /// </summary>
        public void ResetIdleTime()
        {
            _idleSeconds = 0;
        }

        public void IncrementIdleTime()
        {
            _idleSeconds++;

            // Auto-resume after idle threshold
            if (CurrentState == ApplicationState.Paused && _idleSeconds >= _idleThresholdSeconds)
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
                    long remainingSeconds = _idleThresholdSeconds - _idleSeconds;
                    return $"Idle for {_idleSeconds} sec... (resume in {remainingSeconds}s)";

                case ApplicationState.Working:
                    return "Working";

                default:
                    return "Unknown";
            }
        }
    }
}
