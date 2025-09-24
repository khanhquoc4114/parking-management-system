using QuanLyBaiGiuXe.Models;
using OfficeOpenXml;
using System.IO;
using QuanLyBaiGiuXe.Helper;
using QuanLyBaiGiuXe.Services;

namespace QuanLyBaiGiuXe
{
    public partial class ThongKeTheoMayTinhForm: Form
    {
        Manager manager = new Manager();
        public ThongKeTheoMayTinhForm()
        {
            InitializeComponent();
        }

        private void ThongKeTheoMayTinhForm_Load(object sender, EventArgs e)
        {
            LoadData();
            LoadUI();
        }
        private void LoadUI()
        {
            dtpTu.Format = DateTimePickerFormat.Custom;
            dtpTu.CustomFormat = "dd/MM/yyyy HH:mm";
            dtpTu.Value = DateTime.Now.AddDays(-7);
            dtpDen.Format = DateTimePickerFormat.Custom;
            dtpDen.CustomFormat = "dd/MM/yyyy HH:mm";
            List<ComboBoxItem> listXe = manager.GetDanhSachXe();
            LoadComboBox(cbLoaiXe, listXe, "loại xe");

            List<string> groupsVe = new List<string> { "Tất cả loại vé", "Vé tháng", "Vé lượt" };
            cbLoaiVe.DataSource = groupsVe;
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
        private void LoadData()
        {
            dtgThongKe.DataSource = manager.GetThongKeTheoMayTinh();
        }

        private void btnThongKe_Click(object sender, EventArgs e)
        {
            if (dtpTu.Value > dtpDen.Value)
            {
                MessageBox.Show("Ngày bắt đầu không được lớn hơn ngày kết thúc!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (cbLoaiXe.SelectedItem == null || cbLoaiVe.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn loại xe và loại vé!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string loaixe = cbLoaiXe.SelectedValue.ToString();
            string loaive = cbLoaiVe.SelectedItem.ToString();
            if (cbLoaiVe.SelectedIndex == 0) {
                loaive = "";
            }
            dtgThongKe.DataSource = manager.GetThongKeTheoMayTinhByTimKiem(loaive, loaixe, dtpTu.Value, dtpDen.Value);
        }

        private void btnXuatExcel_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel files (*.xlsx)|*.xlsx";
                DateTime now = DateTime.Now;
                sfd.FileName = $"ThongKeTheoMayTinh_{now:ddMMyyyy}.xlsx";

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
