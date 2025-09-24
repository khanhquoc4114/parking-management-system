using OfficeOpenXml;
using QuanLyBaiGiuXe.Helper;
using QuanLyBaiGiuXe.Models;
using QuanLyBaiGiuXe.Services;
using System.IO;

namespace QuanLyBaiGiuXe
{
    public partial class ThongKeTheoNhanVienForm: Form
    {
        Manager manager= new Manager();
        public ThongKeTheoNhanVienForm()
        {
            InitializeComponent();
        }

        private void ThongKeTheoNhanVienForm_Load(object sender, EventArgs e)
        {
            LoadData();
        }
        private void LoadData()
        {
            List<ComboBoxItem> dsLoaiXe = manager.GetDanhSachXe();
            LoadComboBox(cbLoaiXe, dsLoaiXe, "loại xe");

            List<string> dsLoaiVe = new List<string> { "Tất cả loại vé", "Vé tháng", "Vé lượt" };
            cbLoaiVe.DataSource = dsLoaiVe;
            cbLoaiVe.SelectedIndex = 0;

            dtpTu.Format = DateTimePickerFormat.Custom;
            dtpTu.CustomFormat = "dd/MM/yyyy HH:mm";
            dtpTu.Value = DateTime.Now.AddDays(-7);
            dtpDen.Format = DateTimePickerFormat.Custom;
            dtpDen.CustomFormat = "dd/MM/yyyy HH:mm";
            btnThongKe.PerformClick();
        }

        private void LoadComboBox(ComboBox comboBox, List<ComboBoxItem> data, string suffix = null, bool includeTatCa = true)
        {
            if (includeTatCa)
            {
                data.Insert(0, new ComboBoxItem { Value = -1, Text = "Tất cả" + " " + suffix });
            }

            comboBox.DataSource = null;
            comboBox.DataSource = data;
            comboBox.DisplayMember = "Text";
            comboBox.ValueMember = "Value";
            comboBox.SelectedIndex = 0;
        }

        private void btnThongKe_Click(object sender, EventArgs e)
        {
            string Loaive = cbLoaiVe.SelectedItem.ToString();
            string MaLoaiXe = cbLoaiXe.SelectedValue.ToString();
            dtgThongKe.DataSource = manager.GetThongKeTheoNhanVien(
                Loaive,
                MaLoaiXe,
                dtpTu.Value,
                dtpDen.Value
            );
        }

        private void btnXuatExcel_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel files (*.xlsx)|*.xlsx";
                DateTime now = DateTime.Now;
                sfd.FileName = $"ThongKeTheoNhanVien_{now:ddMMyyyy}.xlsx";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var excelService = new ExcelExportService();
                        excelService.ExportDataGridViewToExcel(dtgThongKe, sfd.FileName);

                        MessageBox.Show("Xuất Excel thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi xuất Excel: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
