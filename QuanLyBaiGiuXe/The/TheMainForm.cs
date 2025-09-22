﻿using QuanLyBaiGiuXe.Helper;
using QuanLyBaiGiuXe.Models;
using System;
using System.Windows.Forms;

namespace QuanLyBaiGiuXe
{
    public partial class TheMainForm : Form
    {
        Manager manager = new Manager();

        public TheMainForm()
        {
            InitializeComponent();
        }

        #region Chức năng chính của thẻ
        private void btnXoaThe_Click(object sender, EventArgs e)
        {
            if (dtgThe.SelectedRows.Count > 0)
            {
                int r = dtgThe.CurrentCell.RowIndex;
                string MaThe = dtgThe.Rows[r].Cells["Mã thẻ"].Value.ToString();

                var result = MessageBox.Show($"Bạn có chắc chắn muốn xoá thẻ {MaThe} chứ?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    bool isDeleted = manager.XoaThe(MaThe);

                    if (isDeleted)
                    {
                        ToastService.Show("Xoá thẻ thành công!", this);
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show("Xóa thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một hàng để xoá!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void ThemThe()
        {
            string MaThe = tbMaThe.Text.Trim();
            bool isAdded = manager.ThemThe(MaThe, "Chung", DateTime.Now);

            if (isAdded)
            {
                ToastService.Show("Thêm thẻ thành công!", this);
                LoadData();
            }
        }
        #endregion
        private void TheMainForm_Load(object sender, EventArgs e)
        {
            tbMaThe.Enabled = false;
            LoadData();
        }
        private void LoadData()
        {
            try
            {
                this.dtgThe.DataSource = manager.GetAllThe();
                this.dtgCountThe.DataSource = manager.GetCountThe();
            }
            catch
            {
                MessageBox.Show("Không lấy được nội dung trong table");
            }
        }

        // Phương thức thêm thẻ
        private void tbMaThe_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                ThemThe();
                tbMaThe.Clear();
            }
        }
        private void cbSoThe_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSoThe.Checked)
            {
                tbMaThe.Enabled = true;
                tbMaThe.ReadOnly = false;
                tbMaThe.Focus();
            }
            else
            {
                tbMaThe.Enabled = false;
                tbMaThe.ReadOnly = true;
            }
        }
        private void btnKhoiPhucThe_Click(object sender, EventArgs e)
        {
            KhoiPhucThe();
        }
        private void KhoiPhucThe()
        {
            if (dtgThe.SelectedRows.Count > 0)
            {
                int r = dtgThe.CurrentCell.RowIndex;
                string MaThe = dtgThe.Rows[r].Cells["Mã thẻ"].Value.ToString();
                var result = MessageBox.Show($"Bạn có chắc chắn muốn khôi phục thẻ {MaThe} chứ?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    bool isRestored = manager.KhoiPhucThe(MaThe);
                    if (isRestored)
                    {
                        ToastService.Show("Khôi phục thẻ thành công!", this);
                        LoadData();
                    }
                    else
                    {
                        ToastService.Show("Khôi phục thẻ thất bại!",this);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một hàng để khôi phục!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
