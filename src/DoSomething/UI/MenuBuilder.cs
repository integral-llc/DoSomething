using System;
using System.Globalization;
using System.Windows.Forms;

namespace DoSomething
{
    /// <summary>
    /// Responsible for building context menu items
    /// </summary>
    public class MenuBuilder
    {
        private readonly Action<int> _onInIntervalSelected;
        private readonly Action<DateTime> _onAtTimeSelected;

        public MenuBuilder(Action<int> onInIntervalSelected, Action<DateTime> onAtTimeSelected)
        {
            _onInIntervalSelected = onInIntervalSelected;
            _onAtTimeSelected = onAtTimeSelected;
        }

        public void BuildInMenu(ToolStripMenuItem parentMenuItem)
        {
            parentMenuItem.DropDownItems.Clear();

            // Create 30-minute intervals up to 12 hours (24 intervals)
            for (int i = 1; i <= 24; i++)
            {
                int minutes = i * 30;
                string text = FormatTimeInterval(minutes);

                var menuItem = new ToolStripMenuItem
                {
                    Text = text,
                    Tag = minutes
                };
                menuItem.Click += (s, e) => _onInIntervalSelected((int)((ToolStripMenuItem)s).Tag);
                parentMenuItem.DropDownItems.Add(menuItem);
            }
        }

        public void BuildInMenu(ContextMenuStrip contextMenu)
        {
            contextMenu.Items.Clear();

            // Create 30-minute intervals up to 12 hours (24 intervals)
            for (int i = 1; i <= 24; i++)
            {
                int minutes = i * 30;
                string text = FormatTimeInterval(minutes);

                var menuItem = new ToolStripMenuItem
                {
                    Text = text,
                    Tag = minutes
                };
                menuItem.Click += (s, e) => _onInIntervalSelected((int)((ToolStripMenuItem)s).Tag);
                contextMenu.Items.Add(menuItem);
            }
        }

        public void BuildAtMenu(ToolStripMenuItem parentMenuItem)
        {
            parentMenuItem.DropDownItems.Clear();

            var now = DateTime.Now;
            var twelveHoursFromNow = now.AddHours(12);
            var interval = TimeSpan.FromMinutes(30);
            var start = RoundUpToInterval(now, interval);

            while (start <= twelveHoursFromNow)
            {
                DateTime targetTime = start;
                string text = targetTime.Date == now.Date
                    ? $"At {targetTime:t}"
                    : $"At {targetTime:t} (tomorrow)";

                var menuItem = new ToolStripMenuItem
                {
                    Text = text,
                    Tag = targetTime
                };
                menuItem.Click += (s, e) => _onAtTimeSelected((DateTime)((ToolStripMenuItem)s).Tag);
                parentMenuItem.DropDownItems.Add(menuItem);

                start = start.Add(interval);
            }
        }

        public void BuildAtMenu(ContextMenuStrip contextMenu)
        {
            contextMenu.Items.Clear();

            var now = DateTime.Now;
            var twelveHoursFromNow = now.AddHours(12);
            var interval = TimeSpan.FromMinutes(30);
            var start = RoundUpToInterval(now, interval);

            while (start <= twelveHoursFromNow)
            {
                DateTime targetTime = start;
                string text = targetTime.Date == now.Date
                    ? $"At {targetTime:t}"
                    : $"At {targetTime:t} (tomorrow)";

                var menuItem = new ToolStripMenuItem
                {
                    Text = text,
                    Tag = targetTime
                };
                menuItem.Click += (s, e) => _onAtTimeSelected((DateTime)((ToolStripMenuItem)s).Tag);
                contextMenu.Items.Add(menuItem);

                start = start.Add(interval);
            }
        }

        private string FormatTimeInterval(int minutes)
        {
            if (minutes < 60)
            {
                return $"In {minutes} min";
            }
            else if (minutes % 60 == 0)
            {
                int hours = minutes / 60;
                return $"In {hours} {(hours == 1 ? "hour" : "hours")}";
            }
            else
            {
                double hours = minutes / 60.0;
                return $"In {hours:0.#} hours";
            }
        }

        private DateTime RoundUpToInterval(DateTime dt, TimeSpan interval)
        {
            var modTicks = dt.Ticks % interval.Ticks;
            var delta = modTicks != 0 ? interval.Ticks - modTicks : 0;
            return new DateTime(dt.Ticks + delta, dt.Kind);
        }
    }
}
