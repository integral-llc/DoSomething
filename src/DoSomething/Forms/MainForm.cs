using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using DoSomething.Properties;

namespace DoSomething
{
    public partial class MainForm : Form
    {
        private readonly ApplicationDriver _driver;
        private readonly MenuBuilder _menuBuilder;

        public MainForm()
        {
            InitializeComponent();

            // Initialize business logic driver with saved pause duration
            int pauseDuration = Settings.Default.LastPauseDuration;
            if (pauseDuration < 5) pauseDuration = 30; // Default fallback
            _driver = new ApplicationDriver(pauseDuration);

            // Initialize menu builder with callbacks
            _menuBuilder = new MenuBuilder(OnInIntervalSelected, OnAtTimeSelected);

            // Subscribe to ALL driver events (thread-safe)
            _driver.StatusTextChanged += (s, text) => UpdateStatusLabel();
            _driver.MinimizeWindow += (s, e) =>
            {
                if (InvokeRequired)
                    Invoke(new Action(() => WindowState = FormWindowState.Minimized));
                else
                    WindowState = FormWindowState.Minimized;
            };
            _driver.RestoreWindow += (s, e) =>
            {
                if (InvokeRequired)
                    Invoke(new Action(() => WindowState = FormWindowState.Normal));
                else
                    WindowState = FormWindowState.Normal;
            };
            _driver.UpdateButtonText += (s, text) =>
            {
                if (InvokeRequired)
                    Invoke(new Action(() => btnStartStop.Text = text));
                else
                    btnStartStop.Text = text;
            };
            _driver.SaveTimeout += (s, timeout) =>
            {
                if (InvokeRequired)
                    Invoke(new Action(() => { Settings.Default.LastTimeout = timeout; Settings.Default.Save(); }));
                else
                {
                    Settings.Default.LastTimeout = timeout;
                    Settings.Default.Save();
                }
            };
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Load settings
            numStopAfter.Value = Settings.Default.LastTimeout;
            numPauseDuration.Value = Settings.Default.LastPauseDuration;

            // Set icon
            Icon = new Icon(File.OpenRead("appIcon.ico"));

            // Initialize taskbar icon
            _driver.InitializeTaskbarIcon(Handle);

            // Initial UI update
            UpdateStatusLabel();

            // Subscribe to pause duration changes
            numPauseDuration.ValueChanged += (s, ev) =>
            {
                int pauseDuration = (int)numPauseDuration.Value;
                _driver.SetPauseDuration(pauseDuration);
                Settings.Default.LastPauseDuration = pauseDuration;
                Settings.Default.Save();
            };
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _driver?.Dispose();
        }

        private void btnStartStop_Click(object sender, EventArgs e)
        {
            _driver.ToggleRunning((int)numStopAfter.Value);
        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _menuBuilder.BuildInMenu(inToolStripMenuItem);
            _menuBuilder.BuildAtMenu(atToolStripMenuItem);
        }

        private void OnInIntervalSelected(int minutes)
        {
            numStopAfter.Value = minutes;
        }

        private void OnAtTimeSelected(DateTime targetTime)
        {
            int minutes = (int)(targetTime - DateTime.Now).TotalMinutes;
            if (minutes < 1) minutes = 1;
            numStopAfter.Value = minutes;
        }

        private void UpdateStatusLabel()
        {
            // Check if we need to invoke on the UI thread
            if (lblStatus.InvokeRequired)
            {
                lblStatus.Invoke(new Action(UpdateStatusLabel));
                return;
            }

            lblStatus.Text = _driver.GetStatusText();

            // Update status bar colors based on state
            var state = _driver.GetCurrentState();
            switch (state)
            {
                case ApplicationState.Stopped:
                    lblStatus.BackColor = Color.FromArgb(245, 245, 245);
                    lblStatus.ForeColor = Color.FromArgb(100, 100, 100);
                    break;
                case ApplicationState.Working:
                    lblStatus.BackColor = Color.FromArgb(46, 204, 113);
                    lblStatus.ForeColor = Color.White;
                    break;
                case ApplicationState.Paused:
                    lblStatus.BackColor = Color.FromArgb(255, 193, 7);
                    lblStatus.ForeColor = Color.FromArgb(64, 64, 64);
                    break;
            }
        }

        private void btnStartStop_MouseEnter(object sender, EventArgs e)
        {
            btnStartStop.BackColor = Color.FromArgb(0, 102, 184);
        }

        private void btnStartStop_MouseLeave(object sender, EventArgs e)
        {
            btnStartStop.BackColor = Color.FromArgb(0, 122, 204);
        }

        private void btnIn_Click(object sender, EventArgs e)
        {
            // Build the In menu
            _menuBuilder.BuildInMenu(contextMenuStripIn);

            // Show the context menu at the button's location
            contextMenuStripIn.Show(btnIn, new Point(0, btnIn.Height));
        }

        private void btnAt_Click(object sender, EventArgs e)
        {
            // Build the At menu
            _menuBuilder.BuildAtMenu(contextMenuStripAt);

            // Show the context menu at the button's location
            contextMenuStripAt.Show(btnAt, new Point(0, btnAt.Height));
        }
    }
}
