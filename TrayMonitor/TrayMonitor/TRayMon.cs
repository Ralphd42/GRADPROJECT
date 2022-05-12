namespace TrayMonitor
{
    public partial class TRayMon : Form
    {
        public TRayMon()
        {
            InitializeComponent();
        }

        private void TRayMon_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == WindowState)
                Hide();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            contextMenuStrip1.Show(Cursor.Position.X, Cursor.Position.Y);
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

        }
    }
}