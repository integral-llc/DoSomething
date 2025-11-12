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

            // Initialize business logic driver
            _driver = new ApplicationDriver();

            // Initialize menu builder with callbacks
            _menuBuilder = new MenuBuilder(OnInIntervalSelected, OnAtTimeSelected);

            // Subscribe to ALL driver events
            _driver.StatusTextChanged += (s, text) => UpdateStatusLabel();
            _driver.MinimizeWindow += (s, e) => WindowState = FormWindowState.Minimized;
            _driver.RestoreWindow += (s, e) => WindowState = FormWindowState.Normal;
            _driver.UpdateButtonText += (s, text) => btnStartStop.Text = text;
            _driver.SaveTimeout += (s, timeout) => { Settings.Default.LastTimeout = timeout; Settings.Default.Save(); };
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Load settings
            numStopAfter.Value = Settings.Default.LastTimeout;

            // Set icon
            Icon = new Icon(File.OpenRead("appIcon.ico"));

            // Initialize taskbar icon
            _driver.InitializeTaskbarIcon(Handle);

            // Initial UI update
            UpdateStatusLabel();
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
            lblStatus.Text = _driver.GetStatusText();
        }
    }
}
