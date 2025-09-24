using OfficeOpenXml;
using QuanLyBaiGiuXe.Models;
using QuanLyBaiGiuXe.Services;
using System.IO;

namespace QuanLyBaiGiuXe
{
    public partial class NhatKyVeLuotForm: Form
    {
        Manager manager = new Manager();
        public NhatKyVeLuotForm()
        {
            InitializeComponent();
        }

        private void LoadData()
        {
            List<string> hanhdonglist = new List<string> { "Tất cả hành động", "Xử lý mất thẻ", "Thêm vé lượt", "Sửa vé lượt", "Cho xe ra (Miễn phí)", "Cho xe ra (Tính phí)" };
            cbHanhDong.DataSource = hanhdonglist;

            dtpTu.Format = DateTimePickerFormat.Custom;
            dtpTu.CustomFormat = "dd/MM/yyyy HH:mm";
            dtpTu.Value = DateTime.Now.AddDays(-7);
            dtpDen.Format = DateTimePickerFormat.Custom;
            dtpDen.CustomFormat = "dd/MM/yyyy HH:mm";

            btnTimKiem.PerformClick();
        }

        private void NhatKyVeLuotForm_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnXuatExcel_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel files (*.xlsx)|*.xlsx";
                DateTime now = DateTime.Now;
                sfd.FileName = $"ThongKeXuLyVeLuot_{now:ddMMyyyy}.xlsx";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var excelService = new ExcelExportService();
                        excelService.ExportDataGridViewToExcel(dtgXuLyVeLuot, sfd.FileName);

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
            string hanhdong = cbHanhDong.Text;
            this.dtgXuLyVeLuot.DataSource = manager.GetXuLyVeLuot(hanhdong, tgTu, tgDen);
        }

        private void cbHanhDong_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
