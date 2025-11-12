namespace DoSomething
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnStartStop = new System.Windows.Forms.Button();
            this.numStopAfter = new System.Windows.Forms.NumericUpDown();
            this.contextMenuStripIn = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.contextMenuStripAt = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.inToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.atToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnIn = new System.Windows.Forms.Button();
            this.btnAt = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.numPauseDuration = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numStopAfter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPauseDuration)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            //
            // btnStartStop
            //
            this.btnStartStop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnStartStop.FlatAppearance.BorderSize = 0;
            this.btnStartStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStartStop.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnStartStop.ForeColor = System.Drawing.Color.White;
            this.btnStartStop.Location = new System.Drawing.Point(310, 50);
            this.btnStartStop.Name = "btnStartStop";
            this.btnStartStop.Size = new System.Drawing.Size(110, 42);
            this.btnStartStop.TabIndex = 0;
            this.btnStartStop.Tag = "0";
            this.btnStartStop.Text = "Start";
            this.btnStartStop.UseVisualStyleBackColor = false;
            this.btnStartStop.Click += new System.EventHandler(this.btnStartStop_Click);
            this.btnStartStop.MouseEnter += new System.EventHandler(this.btnStartStop_MouseEnter);
            this.btnStartStop.MouseLeave += new System.EventHandler(this.btnStartStop_MouseLeave);
            //
            // numStopAfter
            //
            this.numStopAfter.BackColor = System.Drawing.Color.White;
            this.numStopAfter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numStopAfter.ContextMenuStrip = this.contextMenuStrip1;
            this.numStopAfter.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.numStopAfter.Location = new System.Drawing.Point(180, 58);
            this.numStopAfter.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numStopAfter.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numStopAfter.Name = "numStopAfter";
            this.numStopAfter.Size = new System.Drawing.Size(100, 27);
            this.numStopAfter.TabIndex = 1;
            this.numStopAfter.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numStopAfter.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            //
            // contextMenuStripIn
            //
            this.contextMenuStripIn.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStripIn.Name = "contextMenuStripIn";
            this.contextMenuStripIn.Size = new System.Drawing.Size(181, 26);
            //
            // contextMenuStripAt
            //
            this.contextMenuStripAt.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStripAt.Name = "contextMenuStripAt";
            this.contextMenuStripAt.Size = new System.Drawing.Size(181, 26);
            //
            // contextMenuStrip1
            //
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.inToolStripMenuItem,
            this.atToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(87, 48);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            //
            // inToolStripMenuItem
            //
            this.inToolStripMenuItem.Name = "inToolStripMenuItem";
            this.inToolStripMenuItem.Size = new System.Drawing.Size(86, 22);
            this.inToolStripMenuItem.Text = "In";
            //
            // atToolStripMenuItem
            //
            this.atToolStripMenuItem.Name = "atToolStripMenuItem";
            this.atToolStripMenuItem.Size = new System.Drawing.Size(86, 22);
            this.atToolStripMenuItem.Text = "At";
            //
            // label1
            //
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label1.Location = new System.Drawing.Point(30, 61);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(135, 19);
            this.label1.TabIndex = 2;
            this.label1.Text = "Stop After (minutes):";
            //
            // lblStatus
            //
            this.lblStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.lblStatus.Location = new System.Drawing.Point(0, 140);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Padding = new System.Windows.Forms.Padding(10);
            this.lblStatus.Size = new System.Drawing.Size(450, 45);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "Stopped";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // btnIn
            //
            this.btnIn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.btnIn.FlatAppearance.BorderSize = 0;
            this.btnIn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnIn.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnIn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnIn.Location = new System.Drawing.Point(180, 97);
            this.btnIn.Name = "btnIn";
            this.btnIn.Size = new System.Drawing.Size(45, 28);
            this.btnIn.TabIndex = 4;
            this.btnIn.Text = "In ▼";
            this.btnIn.UseVisualStyleBackColor = false;
            this.btnIn.Click += new System.EventHandler(this.btnIn_Click);
            //
            // btnAt
            //
            this.btnAt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.btnAt.FlatAppearance.BorderSize = 0;
            this.btnAt.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAt.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnAt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnAt.Location = new System.Drawing.Point(235, 97);
            this.btnAt.Name = "btnAt";
            this.btnAt.Size = new System.Drawing.Size(45, 28);
            this.btnAt.TabIndex = 5;
            this.btnAt.Text = "At ▼";
            this.btnAt.UseVisualStyleBackColor = false;
            this.btnAt.Click += new System.EventHandler(this.btnAt_Click);
            //
            // label2
            //
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.label2.Location = new System.Drawing.Point(30, 103);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 15);
            this.label2.TabIndex = 6;
            this.label2.Text = "Quick Presets:";
            //
            // numPauseDuration
            //
            this.numPauseDuration.BackColor = System.Drawing.Color.White;
            this.numPauseDuration.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numPauseDuration.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.numPauseDuration.Location = new System.Drawing.Point(220, 25);
            this.numPauseDuration.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.numPauseDuration.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numPauseDuration.Name = "numPauseDuration";
            this.numPauseDuration.Size = new System.Drawing.Size(60, 25);
            this.numPauseDuration.TabIndex = 7;
            this.numPauseDuration.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numPauseDuration.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            //
            // label3
            //
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label3.Location = new System.Drawing.Point(30, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(176, 15);
            this.label3.TabIndex = 8;
            this.label3.Text = "Pause on Activity (seconds):";
            //
            // MainForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(450, 185);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.numPauseDuration);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnAt);
            this.Controls.Add(this.btnIn);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numStopAfter);
            this.Controls.Add(this.btnStartStop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Do Something";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numStopAfter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPauseDuration)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStartStop;
        private System.Windows.Forms.NumericUpDown numStopAfter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripIn;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripAt;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem inToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem atToolStripMenuItem;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnIn;
        private System.Windows.Forms.Button btnAt;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numPauseDuration;
        private System.Windows.Forms.Label label3;
    }
}

