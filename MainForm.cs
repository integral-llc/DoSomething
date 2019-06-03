using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

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

        public MainForm()
        {
            InitializeComponent();
        }

        public IEnumerable<Point> YieldLinePoints(int x, int y, int x2, int y2)
        {
            int w = x2 - x;
            int h = y2 - y;
            int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
            if (w < 0) dx1 = -1;
            else if (w > 0) dx1 = 1;
            if (h < 0) dy1 = -1;
            else if (h > 0) dy1 = 1;
            if (w < 0) dx2 = -1;
            else if (w > 0) dx2 = 1;
            int longest = Math.Abs(w);
            int shortest = Math.Abs(h);
            if (!(longest > shortest))
            {
                longest = Math.Abs(h);
                shortest = Math.Abs(w);
                if (h < 0) dy2 = -1;
                else if (h > 0) dy2 = 1;
                dx2 = 0;
            }

            int numerator = longest >> 1;
            for (int i = 0; i <= longest; i++)
            {
                yield return new Point(x, y);
                numerator += shortest;
                if (!(numerator < longest))
                {
                    numerator -= longest;
                    x += dx1;
                    y += dy1;
                }
                else
                {
                    x += dx2;
                    y += dy2;
                }
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
            Point p = _points[_curIndex];
            SetCursorPos(p.X, p.Y);
            _curIndex++;
            if (_curIndex >= _points.Count)
            {
                _points.Clear();
                _start = _end;
                _end = GetPoint(_start, 300);
                _points.AddRange(YieldLinePoints(_start.X, _start.Y, _end.X, _end.Y));
                _curIndex = 0;
                mouse_event(MOUSEEVENTF_LEFTUP | MOUSEEVENTF_LEFTDOWN, (uint) _start.X, (uint) _start.Y, 0, 0);
            }
        }

        private void TmrStop_Tick(object sender, EventArgs e)
        {
            btnStartStop.PerformClick();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            int tag = Convert.ToInt32(btnStartStop.Tag);
            if (tag == 0)
            {
                _start = GetPoint(new Point(0, 0), 100);
                _end = GetPoint(_start, 300);
                _points.Clear();
                _points.AddRange(YieldLinePoints(_start.X, _start.Y, _end.X, _end.Y));
                _curIndex = 0;
                tmrWorker.Start();
                WindowState = FormWindowState.Minimized;
                var ts = TimeSpan.FromMinutes((double) numStopAfter.Value);
                tmrStop.Interval = (int) ts.TotalMilliseconds;
                tmrStop.Start();
            }
            else
            {
                tmrWorker.Stop();
                tmrStop.Stop();
                WindowState = FormWindowState.Normal;
            }

            tag = 1 - tag;
            btnStartStop.Tag = tag;
            btnStartStop.Text = tag == 0 ? "Start" : "Stop";
        }
    }
}
