using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DoSomething
{
    /// <summary>
    /// Controls mouse operations and coordinates movement strategies
    /// </summary>
    public class MouseController
    {
        private const int MIN_CLICK_INTERVAL_SECONDS = 3;
        private const int MAX_CLICK_INTERVAL_SECONDS = 8;

        private readonly IMouseMovementStrategy _movementStrategy;
        private readonly ApplicationStateManager _stateManager;
        private readonly Random _random;

        private List<Point> _currentPath;
        private int _currentPathIndex;
        private Point _currentTarget;
        private bool _shouldIgnoreNextMouseMove;
        private int _nextClickAfterSeconds;

        public bool ShouldIgnoreNextMouseMove
        {
            get
            {
                bool value = _shouldIgnoreNextMouseMove;
                _shouldIgnoreNextMouseMove = false;
                return value;
            }
        }

        public MouseController(IMouseMovementStrategy movementStrategy, ApplicationStateManager stateManager)
        {
            _movementStrategy = movementStrategy;
            _stateManager = stateManager;
            _random = new Random();
            _currentPath = new List<Point>();
            _currentPathIndex = 0;
            _nextClickAfterSeconds = GenerateRandomClickInterval();
        }

        /// <summary>
        /// Generates a random click interval between MIN and MAX seconds for human-like behavior
        /// </summary>
        private int GenerateRandomClickInterval()
        {
            return _random.Next(MIN_CLICK_INTERVAL_SECONDS, MAX_CLICK_INTERVAL_SECONDS + 1);
        }

        public void Initialize()
        {
            var startPoint = GenerateRandomPoint(new Point(0, 0), 100);
            _currentTarget = GenerateRandomPoint(startPoint, 300);
            _currentPath = _movementStrategy.GeneratePath(startPoint, _currentTarget).ToList();
            _currentPathIndex = 0;
        }

        public void ProcessTick()
        {
            if (_stateManager.CurrentState != ApplicationState.Working)
            {
                return;
            }

            _currentPathIndex++;

            if (_currentPathIndex < _currentPath.Count)
            {
                // Move along current path
                Point targetPoint = _currentPath[_currentPathIndex];
                _shouldIgnoreNextMouseMove = true;
                NativeMethods.SetCursorPos(targetPoint.X, targetPoint.Y);
            }
            else
            {
                // Generate new path
                Point newTarget = GenerateRandomPoint(_currentTarget, 300);
                _currentPath = _movementStrategy.GeneratePath(_currentTarget, newTarget).ToList();
                _currentTarget = newTarget;
                _currentPathIndex = 0;

                // Perform click if idle long enough (randomized interval)
                if (_stateManager.IdleSeconds > _nextClickAfterSeconds)
                {
                    PerformClick(_currentTarget);
                    // Generate new random interval for next click
                    _nextClickAfterSeconds = GenerateRandomClickInterval();
                }
            }
        }

        private void PerformClick(Point location)
        {
            NativeMethods.mouse_event(
                NativeMethods.MOUSEEVENTF_LEFTDOWN | NativeMethods.MOUSEEVENTF_LEFTUP,
                (uint)location.X,
                (uint)location.Y,
                0,
                0);
        }

        private Point GenerateRandomPoint(Point from, int minDistance)
        {
            Rectangle screen = Screen.PrimaryScreen.WorkingArea;

            while (true)
            {
                int x = _random.Next(screen.Left, screen.Right);
                int y = _random.Next(screen.Top, screen.Bottom);
                Point newPoint = new Point(x, y);

                if (CalculateDistance(newPoint, from) >= minDistance)
                {
                    return newPoint;
                }
            }
        }

        private double CalculateDistance(Point p1, Point p2)
        {
            var dy = p2.Y - p1.Y;
            var dx = p2.X - p1.X;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}
