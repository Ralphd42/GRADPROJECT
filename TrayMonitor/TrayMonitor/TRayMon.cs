namespace TrayMonitor
{
    public partial class TRayMon : Form
    {
        public TRayMon()
        {
            InitializeComponent();
        }
        private bool _cRunning = false;
        private void TRayMon_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == WindowState)
                Hide();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
           // Show();
           // WindowState = FormWindowState.Normal;
           // contextMenuStrip1.Show(Cursor.Position.X, Cursor.Position.Y);
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void TRayMon_Load(object sender, EventArgs e)
        {
            timer1.Start();
            ShowInTaskbar = false;
            notifyIcon1.Visible = true;
            this.Hide();
            


        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await refresh();
            
            /*
            Connector cn = new Connector();
            var tsk  = cn.SendCommand("<E>#");
            var tskW = cn.SendCommand("<W>#");
            var tskj = cn.SendCommand("<J>#");
            btnRefresh.Enabled = false;
            cleanStat();
            await tsk;
            await tskW;
            await tskj;
            this.lblErrors.Text = tsk.Result;// await cn.SendCommand("<E>#");
            this.lblWorkers.Text = tskW.Result;
            this.lblJobs.Text = tskj.Result;
            btnRefresh.Enabled = true;*/
        }
        private void cleanStat()
        {
            lblErrors.Text  = "No Errors";
            lblJobs.Text    = "No Job Data";
            lblWorkers.Text = "No worker Info";

        }

        async Task refresh()
        {
            if (_cRunning)
            {
                Connector cn = new Connector();
                var tsk = cn.SendCommand("<E>#");
                var tskW = cn.SendCommand("<W>#");
                var tskj = cn.SendCommand("<J>#");
                btnRefresh.Enabled = false;
                cleanStat();
                await tsk;
                await tskW;
                await tskj;
                this.lblErrors.Text = removeTypeChars(tsk.Result);// await cn.SendCommand("<E>#");
                this.lblWorkers.Text = removeTypeChars(tskW.Result);
                this.lblJobs.Text = removeTypeChars(tskj.Result);
                lblstatus.Text = "STOPPED";
                if (_cRunning) {
                    lblstatus.Text  = "RUNNING";
                    BtnKill.Enabled = true;
                    btnRefresh.Enabled = true;
                }
                else {
                    btnRefresh.Enabled = false;
                    BtnKill.Enabled    = false;
                }
            }
            else {
                await Ping();
            }
        }
        async Task<bool> Ping()
        {   
            const string png = "<P>#";
            Connector cn = new Connector();
            string rv = await  cn.SendCommand(png);
            if (rv.Contains(png)) { _cRunning = true; btnRefresh.Enabled = true; }
            else { _cRunning = false; }
            return _cRunning;
        }




        private async void BtnKill_Click(object sender, EventArgs e)
        {
            var cn = new Connector();
            var tksKill = cn.Kill();
            BtnKill.Enabled = false;
            await tksKill;
            await refresh();
            BtnKill.Enabled = true;
        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            await Ping();
            if (_cRunning)
            {
                lblstatus.Text = "RUNNING";
            }
            else 
            {
                lblstatus.Text = "STOPPED";
            }
        }
        #region parsers

        public static string removeTypeChars(string resp)
        {
            resp = resp.Trim();
            if (resp.Length > 0) {
                resp = resp.Substring(3);
                resp = resp.Replace("#", "");
            }
            return resp;
        }





        #endregion

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Environment.Exit(1);
            MessageBox.Show("Shutting Down Monitor");
        }

        private void SilentKillController(object sender, EventArgs e)
        {
            BtnKill_Click(sender, e);
        }
    }
}