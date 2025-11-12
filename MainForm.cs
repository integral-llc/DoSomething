using DoSomething.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DoSomething;

namespace DoSomethingEx
{
    public partial class MainForm : Form
    {
        private Point _start, _end;
        private int _curIndex;
        private readonly List<Point> _points = new List<Point>();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);
        //Mouse actions
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        private long IdleTickSeconds;
        private bool _isMouseMovementPaused;
        private bool _isRunning;
        private bool _ignoreNextMouseMove;
        private const int IDLE_THRESHOLD_TO_RESUME = 30; // Resume after 30 seconds of idle time

        readonly GlobalKeyboardHook _globalKeyboardHook;
        readonly GlobalMouseHook _globalMouseHook;

        public MainForm()
        {
            InitializeComponent();

            _globalKeyboardHook = new GlobalKeyboardHook();
            _globalKeyboardHook.KeyboardPressed += HandleGlobalKeyboardPressEvent;
            _globalKeyboardHook.Hook();

            _globalMouseHook = new GlobalMouseHook();
            _globalMouseHook.MouseMoved += HandleGlobalMouseMoveEvent;
            _globalMouseHook.Hook();
        }

        /// <summary>
        /// Generates human-like mouse movement using cubic Bezier curves with random control points
        /// </summary>
        public IEnumerable<Point> GenerateHumanLikeMousePath(Point start, Point end)
        {
            // Calculate distance to determine number of steps
            double distance = Distance(start, end);
            int steps = Math.Max(20, (int)(distance / 10)); // More steps for longer distances

            // Generate random control points for Bezier curve
            // Control points create the "curve" in the path
            Point cp1 = GenerateControlPoint(start, end, 0.33);
            Point cp2 = GenerateControlPoint(start, end, 0.66);

            // Generate points along the Bezier curve with variable speed
            for (int i = 0; i <= steps; i++)
            {
                double t = (double)i / steps;

                // Apply easing function for more human-like acceleration/deceleration
                // Humans tend to start slow, speed up in the middle, and slow down at the end
                double easedT = EaseInOutCubic(t);

                // Calculate point on cubic Bezier curve
                Point p = CalculateBezierPoint(easedT, start, cp1, cp2, end);

                // Add small random jitter to make it more natural (human hands aren't perfectly steady)
                if (i > 0 && i < steps)
                {
                    p.X += R.Next(-1, 2);
                    p.Y += R.Next(-1, 2);
                }

                yield return p;
            }
        }

        /// <summary>
        /// Generates a control point for Bezier curve with some randomness
        /// </summary>
        private Point GenerateControlPoint(Point start, Point end, double position)
        {
            // Linear interpolation between start and end
            int x = (int)(start.X + (end.X - start.X) * position);
            int y = (int)(start.Y + (end.Y - start.Y) * position);

            // Add perpendicular offset for curve
            double angle = Math.Atan2(end.Y - start.Y, end.X - start.X);
            double perpAngle = angle + Math.PI / 2;

            // Random offset distance (up to 20% of total distance)
            double offsetDistance = Distance(start, end) * 0.2 * (R.NextDouble() - 0.5);

            x += (int)(Math.Cos(perpAngle) * offsetDistance);
            y += (int)(Math.Sin(perpAngle) * offsetDistance);

            return new Point(x, y);
        }

        /// <summary>
        /// Calculates a point on a cubic Bezier curve
        /// </summary>
        private Point CalculateBezierPoint(double t, Point p0, Point p1, Point p2, Point p3)
        {
            double u = 1 - t;
            double tt = t * t;
            double uu = u * u;
            double uuu = uu * u;
            double ttt = tt * t;

            int x = (int)(uuu * p0.X +
                         3 * uu * t * p1.X +
                         3 * u * tt * p2.X +
                         ttt * p3.X);

            int y = (int)(uuu * p0.Y +
                         3 * uu * t * p1.Y +
                         3 * u * tt * p2.Y +
                         ttt * p3.Y);

            return new Point(x, y);
        }

        /// <summary>
        /// Easing function for smooth acceleration/deceleration
        /// </summary>
        private double EaseInOutCubic(double t)
        {
            return t < 0.5
                ? 4 * t * t * t
                : 1 - Math.Pow(-2 * t + 2, 3) / 2;
        }

        /// <summary>
        /// Updates the status label based on current application state
        /// </summary>
        private void UpdateStatusLabel()
        {
            if (!_isRunning)
            {
                lblStatus.Text = "Stopped";
            }
            else if (_isMouseMovementPaused)
            {
                long remainingSeconds = IDLE_THRESHOLD_TO_RESUME - IdleTickSeconds;
                lblStatus.Text = $"Idle for {IdleTickSeconds} sec... (resume in {remainingSeconds}s)";
            }
            else
            {
                lblStatus.Text = "Working";
            }
        }

        private static double Distance(Point p1, Point p2)
        {
            var dy = p2.Y - p1.Y;
            var dx = p2.X - p1.X;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        private readonly Random R = new Random();

        private Point GetPoint(Point p, double minDistance)
        {
            Rectangle rc = Screen.PrimaryScreen.WorkingArea;
            while (true)
            {
                var x = R.Next(rc.Left, rc.Right);
                var y = R.Next(rc.Top, rc.Bottom);
                Point pNew = new Point(x, y);
                bool ok = Distance(pNew, p) >= minDistance;
                if (ok)
                    return pNew;
            }
        }

        private void TmrWorker_Tick(object sender, EventArgs e)
        {
            // Check if we should resume mouse movement after idle period
            if (_isMouseMovementPaused && IdleTickSeconds >= IDLE_THRESHOLD_TO_RESUME)
            {
                _isMouseMovementPaused = false;
                Debug.WriteLine("Resuming mouse movement");
            }

            // Only perform mouse actions if not paused
            if (!_isMouseMovementPaused)
            {
                _curIndex++;
                if (_curIndex < _points.Count)
                {
                    // Move mouse along the path
                    Point targetPoint = _points[_curIndex];
                    _ignoreNextMouseMove = true;  // Tell hook to ignore this move
                    SetCursorPos(targetPoint.X, targetPoint.Y);
                    UpdateStatusLabel();
                    return;
                }

                // Generate new path with human-like movement
                _points.Clear();
                _start = _end;
                _end = GetPoint(_start, 300);
                _points.AddRange(GenerateHumanLikeMousePath(_start, _end));
                _curIndex = 0;

                // Perform click after extended idle time
                if (IdleTickSeconds > 5)
                {
                    mouse_event(MOUSEEVENTF_LEFTUP | MOUSEEVENTF_LEFTDOWN, (uint)_start.X, (uint)_start.Y, 0, 0);
                    Debug.WriteLine("Perform click");
                }
            }

            IdleTickSeconds++;
            UpdateStatusLabel();
        }

        private void TmrStop_Tick(object sender, EventArgs e)
        {
            btnStartStop.PerformClick();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            numStopAfter.Value = Settings.Default.LastTimeout;
            var appIcon = new Icon(File.OpenRead("appIcon.ico"));
            Icon = appIcon;
            _isRunning = false;
            UpdateStatusLabel();
        }

        private void CalculateInInterval(int minutes)
        {
            numStopAfter.Value = minutes;
        }

        private void CreateInMenuItems()
        {
            var menuItemParent = inToolStripMenuItem;
            Debug.Assert(menuItemParent != null, nameof(menuItemParent) + " != null");

            // Clear existing items to prevent duplicates
            menuItemParent.DropDownItems.Clear();

            // Create 30-minute interval items up to 12 hours (24 intervals)
            for (int i = 1; i <= 24; i++)
            {
                int minutes = i * 30; // Capture value for closure
                string text;

                if (minutes < 60)
                {
                    text = $"In {minutes} min";
                }
                else if (minutes % 60 == 0)
                {
                    int hours = minutes / 60;
                    text = $"In {hours} {(hours == 1 ? "hour" : "hours")}";
                }
                else
                {
                    double hours = minutes / 60.0;
                    text = $"In {hours:0.#} hours";
                }

                var newMenuItem = new ToolStripMenuItem
                {
                    Text = text,
                    Tag = minutes,
                };
                newMenuItem.Click += (o, args) => CalculateInInterval((int)((ToolStripMenuItem)o).Tag);
                menuItemParent.DropDownItems.Add(newMenuItem);
            }
        }

        public static DateTime RoundUp(DateTime dt, TimeSpan d)
        {
            var modTicks = dt.Ticks % d.Ticks;
            var delta = modTicks != 0 ? d.Ticks - modTicks : 0;
            return new DateTime(dt.Ticks + delta, dt.Kind);
        }

        private void CalculateDifference(DateTime to)
        {
            var minutes = (int)(to - DateTime.Now).TotalMinutes;
            if (minutes < 1) minutes = 1; // Ensure at least 1 minute
            numStopAfter.Value = minutes;
        }

        private void CreateAtMenuItems()
        {
            var now = DateTime.Now;
            var twelveHoursFromNow = now.AddHours(12);
            var interval = TimeSpan.FromMinutes(30);
            var start = RoundUp(now, interval);
            var menuItemParent = atToolStripMenuItem;

            // Clear existing items to prevent duplicates
            menuItemParent.DropDownItems.Clear();

            // Cover next 12 hours, even if it goes to next day
            while (start <= twelveHoursFromNow)
            {
                DateTime targetTime = start; // Capture value for closure
                var newMenuItem = new ToolStripMenuItem
                {
                    // Show date if it's a different day
                    Text = targetTime.Date == now.Date
                        ? $"At {targetTime:t}"
                        : $"At {targetTime:t} (tomorrow)",
                    Tag = targetTime,
                };
                newMenuItem.Click += (o, args) => CalculateDifference((DateTime)((ToolStripMenuItem)o).Tag);
                menuItemParent.DropDownItems.Add(newMenuItem);
                start = start.Add(interval);
            }
        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CreateInMenuItems();
            CreateAtMenuItems();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _globalKeyboardHook.Unhook();
            _globalMouseHook.Unhook();
        }

        private void HandleGlobalKeyboardPressEvent(object sender, EventArgs arg)
        {
            IdleTickSeconds = 0;
            _isMouseMovementPaused = true;
            Debug.WriteLine("Keyboard touch - mouse movement paused");
            UpdateStatusLabel();
        }

        private void HandleGlobalMouseMoveEvent(object sender, EventArgs arg)
        {
            // Ignore mouse moves when we're not running or when we're the ones moving it
            if (!_isRunning || _ignoreNextMouseMove)
            {
                _ignoreNextMouseMove = false;
                return;
            }

            // Only pause if we're currently in working state
            if (!_isMouseMovementPaused)
            {
                IdleTickSeconds = 0;
                _isMouseMovementPaused = true;
                Debug.WriteLine("Real mouse movement detected - pausing");
                UpdateStatusLabel();
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            int tag = Convert.ToInt32(btnStartStop.Tag);
            if (tag == 0)
            {
                _start = GetPoint(new Point(0, 0), 100);
                _end = GetPoint(_start, 300);
                _points.Clear();
                _points.AddRange(GenerateHumanLikeMousePath(_start, _end));
                _curIndex = 0;
                _isMouseMovementPaused = false;
                IdleTickSeconds = 0;
                _isRunning = true;
                _ignoreNextMouseMove = false;

                tmrWorker.Start();
                WindowState = FormWindowState.Minimized;
                var ts = TimeSpan.FromMinutes((double) numStopAfter.Value);
                tmrStop.Interval = (int) ts.TotalMilliseconds;
                tmrStop.Start();
                Settings.Default.LastTimeout = (int)numStopAfter.Value;
                Settings.Default.Save();
            }
            else
            {
                tmrWorker.Stop();
                tmrStop.Stop();
                _isRunning = false;
                WindowState = FormWindowState.Normal;
            }

            tag = 1 - tag;
            btnStartStop.Tag = tag;
            btnStartStop.Text = tag == 0 ? "Start" : "Stop";
            UpdateStatusLabel();
        }
    }
}
