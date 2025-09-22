﻿using OfficeOpenXml;
using QuanLyBaiGiuXe.Models;
using System;
using System.IO;
using System.Windows.Forms;

namespace QuanLyBaiGiuXe
{
    public partial class NhatKyXuLyVeThangForm: Form
    {
        Manager manager = new Manager();
        public NhatKyXuLyVeThangForm()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            dtgXuLyVeThang.DataSource = manager.GetAllXuLyVeThang();

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

            this.dtgXuLyVeThang.DataSource = manager.GetAllXuLyVeThang(tgTu, tgDen);
        }

        private void NhatKyXuLyVeThangForm_Load(object sender, EventArgs e)
        {

        }

        private void btnXuatExcel_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel files (*.xlsx)|*.xlsx";
                DateTime now = DateTime.Now;
                sfd.FileName = $"ThongKeXuLyVeThang_{now:ddMMyyyy}.xlsx";

                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    using (ExcelPackage package = new ExcelPackage())
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");

                        // Xuất tiêu đề cột
                        for (int col = 0; col < dtgXuLyVeThang.Columns.Count; col++)
                        {
                            worksheet.Cells[1, col + 1].Value = dtgXuLyVeThang.Columns[col].HeaderText;
                        }

                        // Xuất dữ liệu từ DataGridView
                        for (int row = 0; row < dtgXuLyVeThang.Rows.Count; row++)
                        {
                            for (int col = 0; col < dtgXuLyVeThang.Columns.Count; col++)
                            {
                                worksheet.Cells[row + 2, col + 1].Value = dtgXuLyVeThang.Rows[row].Cells[col].Value?.ToString();
                            }
                        }

                        File.WriteAllBytes(sfd.FileName, package.GetAsByteArray());

                        MessageBox.Show("Xuất Excel thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }
    }
}
