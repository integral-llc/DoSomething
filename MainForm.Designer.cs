﻿namespace DoSomethingEx
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.btnStartStop = new System.Windows.Forms.Button();
            this.tmrWorker = new System.Windows.Forms.Timer(this.components);
            this.numStopAfter = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.tmrStop = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.numStopAfter)).BeginInit();
            this.SuspendLayout();
            // 
            // btnStartStop
            // 
            this.btnStartStop.Location = new System.Drawing.Point(24, 23);
            this.btnStartStop.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btnStartStop.Name = "btnStartStop";
            this.btnStartStop.Size = new System.Drawing.Size(150, 44);
            this.btnStartStop.TabIndex = 0;
            this.btnStartStop.Tag = "0";
            this.btnStartStop.Text = "Start";
            this.btnStartStop.UseVisualStyleBackColor = true;
            this.btnStartStop.Click += new System.EventHandler(this.Button1_Click);
            // 
            // tmrWorker
            // 
            this.tmrWorker.Interval = 10;
            this.tmrWorker.Tick += new System.EventHandler(this.TmrWorker_Tick);
            // 
            // numStopAfter
            // 
            this.numStopAfter.Location = new System.Drawing.Point(196, 77);
            this.numStopAfter.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
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
            this.numStopAfter.Size = new System.Drawing.Size(240, 31);
            this.numStopAfter.TabIndex = 1;
            this.numStopAfter.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 81);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(161, 25);
            this.label1.TabIndex = 2;
            this.label1.Text = "Stop After (min)";
            // 
            // tmrStop
            // 
            this.tmrStop.Tick += new System.EventHandler(this.TmrStop_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(666, 294);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numStopAfter);
            this.Controls.Add(this.btnStartStop);
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "Do Something";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numStopAfter)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStartStop;
        private System.Windows.Forms.Timer tmrWorker;
        private System.Windows.Forms.NumericUpDown numStopAfter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer tmrStop;
    }
}

