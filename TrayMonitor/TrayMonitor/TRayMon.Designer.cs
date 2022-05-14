namespace TrayMonitor
{
    partial class TRayMon
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TRayMon));
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.CMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.KillMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.lblErrors = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btnRefresh = new System.Windows.Forms.Button();
            this.lblWorkers = new System.Windows.Forms.Label();
            this.lblJobs = new System.Windows.Forms.Label();
            this.lErr = new System.Windows.Forms.Label();
            this.lWorkers = new System.Windows.Forms.Label();
            this.lJobs = new System.Windows.Forms.Label();
            this.BtnKill = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lblstatus = new System.Windows.Forms.Label();
            this.KillController = new System.Windows.Forms.ToolStripMenuItem();
            this.CMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.CMenu;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "Bitcoin Miner Monitor";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.DoubleClick += new System.EventHandler(this.notifyIcon1_DoubleClick);
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // CMenu
            // 
            this.CMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.KillMenu,
            this.KillController});
            this.CMenu.Name = "contextMenuStrip1";
            this.CMenu.Size = new System.Drawing.Size(181, 70);
            this.CMenu.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // KillMenu
            // 
            this.KillMenu.Name = "KillMenu";
            this.KillMenu.Size = new System.Drawing.Size(180, 22);
            this.KillMenu.Text = "Kill Monitor";
            this.KillMenu.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // lblErrors
            // 
            this.lblErrors.AutoSize = true;
            this.lblErrors.Location = new System.Drawing.Point(213, 39);
            this.lblErrors.Name = "lblErrors";
            this.lblErrors.Size = new System.Drawing.Size(49, 15);
            this.lblErrors.TabIndex = 1;
            this.lblErrors.Text = "ERRORS";
            this.lblErrors.Click += new System.EventHandler(this.label1_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(26, 36);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(104, 23);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.Text = "refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // lblWorkers
            // 
            this.lblWorkers.AutoSize = true;
            this.lblWorkers.Location = new System.Drawing.Point(213, 67);
            this.lblWorkers.Name = "lblWorkers";
            this.lblWorkers.Size = new System.Drawing.Size(85, 15);
            this.lblWorkers.TabIndex = 3;
            this.lblWorkers.Text = "Workers Status";
            // 
            // lblJobs
            // 
            this.lblJobs.AutoSize = true;
            this.lblJobs.Location = new System.Drawing.Point(213, 93);
            this.lblJobs.Name = "lblJobs";
            this.lblJobs.Size = new System.Drawing.Size(30, 15);
            this.lblJobs.TabIndex = 4;
            this.lblJobs.Text = "Jobs";
            // 
            // lErr
            // 
            this.lErr.AutoSize = true;
            this.lErr.Location = new System.Drawing.Point(160, 39);
            this.lErr.Name = "lErr";
            this.lErr.Size = new System.Drawing.Size(37, 15);
            this.lErr.TabIndex = 5;
            this.lErr.Text = "Errors";
            // 
            // lWorkers
            // 
            this.lWorkers.AutoSize = true;
            this.lWorkers.Location = new System.Drawing.Point(160, 67);
            this.lWorkers.Name = "lWorkers";
            this.lWorkers.Size = new System.Drawing.Size(50, 15);
            this.lWorkers.TabIndex = 6;
            this.lWorkers.Text = "Workers";
            // 
            // lJobs
            // 
            this.lJobs.AutoSize = true;
            this.lJobs.Location = new System.Drawing.Point(160, 93);
            this.lJobs.Name = "lJobs";
            this.lJobs.Size = new System.Drawing.Size(30, 15);
            this.lJobs.TabIndex = 7;
            this.lJobs.Text = "Jobs";
            // 
            // BtnKill
            // 
            this.BtnKill.BackColor = System.Drawing.Color.Red;
            this.BtnKill.Location = new System.Drawing.Point(21, 122);
            this.BtnKill.Name = "BtnKill";
            this.BtnKill.Size = new System.Drawing.Size(91, 23);
            this.BtnKill.TabIndex = 8;
            this.BtnKill.Text = "Shut off Miner";
            this.BtnKill.UseVisualStyleBackColor = false;
            this.BtnKill.Click += new System.EventHandler(this.BtnKill_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 189);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 15);
            this.label1.TabIndex = 9;
            this.label1.Text = "Status";
            this.label1.Click += new System.EventHandler(this.label1_Click_1);
            // 
            // lblstatus
            // 
            this.lblstatus.AutoSize = true;
            this.lblstatus.Location = new System.Drawing.Point(45, 190);
            this.lblstatus.Name = "lblstatus";
            this.lblstatus.Size = new System.Drawing.Size(55, 15);
            this.lblstatus.TabIndex = 10;
            this.lblstatus.Text = "STOPPED";
            // 
            // KillController
            // 
            this.KillController.Name = "KillController";
            this.KillController.Size = new System.Drawing.Size(180, 22);
            this.KillController.Text = "Kill Controller";
            this.KillController.Click += new System.EventHandler(this.SilentKillController);
            // 
            // TRayMon
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(304, 207);
            this.Controls.Add(this.lblstatus);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BtnKill);
            this.Controls.Add(this.lJobs);
            this.Controls.Add(this.lWorkers);
            this.Controls.Add(this.lErr);
            this.Controls.Add(this.lblJobs);
            this.Controls.Add(this.lblWorkers);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.lblErrors);
            this.Name = "TRayMon";
            this.Text = "Bitcoin Miner Monitor";
            this.Load += new System.EventHandler(this.TRayMon_Load);
            this.Resize += new System.EventHandler(this.TRayMon_Resize);
            this.CMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private NotifyIcon notifyIcon1;
        private ContextMenuStrip CMenu;
        private Label lblErrors;
        private System.Windows.Forms.Timer timer1;
        private Button btnRefresh;
        private Label lblWorkers;
        private Label lblJobs;
        private Label lErr;
        private Label lWorkers;
        private Label lJobs;
        private Button BtnKill;
        private Label label1;
        private Label lblstatus;
        private ToolStripMenuItem KillMenu;
        private ToolStripMenuItem KillController;
    }
}