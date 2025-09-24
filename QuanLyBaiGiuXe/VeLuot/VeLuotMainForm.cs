using OfficeOpenXml;
using QuanLyBaiGiuXe.Helper;
using QuanLyBaiGiuXe.Models;
using QuanLyBaiGiuXe.Services;

namespace QuanLyBaiGiuXe
{
    public partial class VeLuotMainForm: Form
    {
        Manager manager = new Manager();
        public VeLuotMainForm()
        {
            InitializeComponent();
        }

        public void LoadData()
        {
            try
            {
                this.dtgVeLuot.DataSource = manager.GetAllVeLuot();
                this.dtgTong.DataSource = manager.GetTongVeLuot();
            }
            catch
            {
                MessageBox.Show("Không lấy được nội dung trong table");
            }

            dtpTu.Format = DateTimePickerFormat.Custom;
            dtpTu.CustomFormat = "dd/MM/yyyy HH:mm";
            dtpTu.Value = DateTime.Now.AddDays(-7);
            dtpDen.Format = DateTimePickerFormat.Custom;
            dtpDen.CustomFormat = "dd/MM/yyyy HH:mm";
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            DateTime tgTu = dtpTu.Value;
            DateTime tgDen = dtpDen.Value;
            if (tgTu > tgDen)
            {
                ToastService.Show("Thời gian bắt đầu không được lớn hơn thời gian kết thúc.", this);
                return;
            }
            try
            {
                this.dtgVeLuot.DataSource = manager.GetAllVeLuot(tgTu, tgDen);
                dtgTong.DataSource = manager.GetTongVeLuot(tgTu, tgDen);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tìm kiếm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXuatExcel_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel files (*.xlsx)|*.xlsx";
                DateTime now = DateTime.Now;
                sfd.FileName = $"DuLieuVeLuot_{now:ddMMyyyy}.xlsx";

                ExcelPackage.License.SetNonCommercialPersonal("Wuoc");

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var excelService = new ExcelExportService();
                        excelService.ExportDataGridViewToExcel(dtgVeLuot, sfd.FileName);

                        MessageBox.Show("Xuất Excel thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi xuất Excel: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void VeLuotMainForm_Load(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}
