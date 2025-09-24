using OfficeOpenXml;
using QuanLyBaiGiuXe.Models;
using QuanLyBaiGiuXe.Services;
using System.IO;

namespace QuanLyBaiGiuXe
{
    public partial class NhatKyDangNhapForm: Form
    {
        Manager manager = new Manager();

        public NhatKyDangNhapForm()
        {
            InitializeComponent();
        }

        private void NhatKyDangNhapForm_Load(object sender, EventArgs e)
        {
            LoadUI();
            LoadData();
        }

        private void LoadUI()
        {
            dtpTu.Format = DateTimePickerFormat.Custom;
            dtpTu.CustomFormat = "dd/MM/yyyy HH:mm";
            dtpTu.Value = DateTime.Now.AddDays(-7);
            dtpDen.Format = DateTimePickerFormat.Custom;
            dtpDen.CustomFormat = "dd/MM/yyyy HH:mm";
        }

        private void LoadData() {
            try
            {
                btnTimKiem.PerformClick();
            }
            catch
            {
                MessageBox.Show("Không lấy được nội dung trong table");
            }
        }

        private void btnXuatExcel_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel files (*.xlsx)|*.xlsx";
                DateTime now = DateTime.Now;
                sfd.FileName = $"DuLieuDangNhap_{now:ddMMyyyy}.xlsx";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var excelService = new ExcelExportService();
                        excelService.ExportDataGridViewToExcel(dtgNhatKyDangNhap, sfd.FileName);

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

            this.dtgNhatKyDangNhap.DataSource = manager.GetNhatKyDangNhap(tgTu, tgDen);
        }
    }
}
