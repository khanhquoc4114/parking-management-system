using System.Drawing;

namespace QuanLyBaiGiuXe
{
    public partial class ToastForm : Form
    {
        private System.Windows.Forms.Timer timer;

        public ToastForm(string message, Form ownerForm, int duration = 1000)
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.BackColor = System.Drawing.Color.LightYellow;
            this.ShowInTaskbar = false;
            this.Size = new System.Drawing.Size(280, 80);

            int x = ownerForm.Left + (ownerForm.Width - this.Width) / 2;
            int y = ownerForm.Top + (ownerForm.Height - this.Height) / 2;
            this.Location = new System.Drawing.Point(x, y);

            Label lbl = new Label()
            {
                Text = message,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,

            };
            this.Controls.Add(lbl);

            timer = new System.Windows.Forms.Timer();
            timer.Interval = duration;
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                this.Close();
            };
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            timer.Start();
        }

        private void ToastForm_Load(object sender, EventArgs e)
        {

        }
    }
}
