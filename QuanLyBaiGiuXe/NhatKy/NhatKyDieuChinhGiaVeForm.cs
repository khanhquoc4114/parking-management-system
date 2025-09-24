using QuanLyBaiGiuXe.Models;
using QuanLyBaiGiuXe.Services;

namespace QuanLyBaiGiuXe
{
    public partial class NhatKyDieuChinhGiaVeForm: Form
    {
        Manager manager = new Manager();
        public NhatKyDieuChinhGiaVeForm()
        {
            InitializeComponent();
        }
        private void btnXuatExcel_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel files (*.xlsx)|*.xlsx";
                DateTime now = DateTime.Now;
                sfd.FileName = $"DuLieuDieuChinhGiaVe_{now:ddMMyyyy}.xlsx";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var excelService = new ExcelExportService();
                        excelService.ExportDataGridViewToExcel(dtgNhatKy, sfd.FileName);

                        MessageBox.Show("Xuất Excel thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi xuất Excel: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            DateTime tgTu = dtpTu.Value;
            DateTime tgDen = dtpDen.Value;
            if (tgTu > tgDen)
            {
                MessageBox.Show("Thời gian bắt đầu không được lớn hơn thời gian kết thúc.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            dtgNhatKy.DataSource = manager.GetNhatKyDieuChinhGiaVe(tgTu, tgDen);
        }

        private void NhatKyDieuChinhGiaVeForm_Load(object sender, EventArgs e)
        {
            LoadData();
            LoadUI();
        }
        private void LoadData()
        {
            dtgNhatKy.DataSource = manager.GetNhatKyDieuChinhGiaVe();
        }

        private void LoadUI()
        {
            dtpTu.Format = DateTimePickerFormat.Custom;
            dtpTu.CustomFormat = "dd/MM/yyyy HH:mm";
            dtpTu.Value = DateTime.Now.AddDays(-7);
            dtpDen.Format = DateTimePickerFormat.Custom;
            dtpDen.CustomFormat = "dd/MM/yyyy HH:mm";
        }
    }
}
