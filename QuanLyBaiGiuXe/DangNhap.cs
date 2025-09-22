﻿using System;
using System.Windows.Forms;
using QuanLyBaiGiuXe.DataAccess;
using QuanLyBaiGiuXe.Helper;

namespace QuanLyBaiGiuXe
{
    public partial class DangNhap : Form
    {
        string MaNhanVien = null;
        LoginManager loginManager = new LoginManager();
        bool isLoggedIn = false;
        private bool isPasswordVisible = false;
        public DangNhap()
        {
            InitializeComponent();
        }

        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            if (tbUsername.Text.Trim() == string.Empty || tbPassword.Text == string.Empty)
            {
                MessageBox.Show("Hãy nhập đầy đủ thông tin đăng nhập!", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                string matKhau = tbPassword.Text.Trim();
                string username = tbUsername.Text.Trim();
                if (!string.IsNullOrEmpty(matKhau))
                {
                    matKhau = HashHelper.ComputeSHA256Hash(matKhau);
                }
                MaNhanVien = loginManager.GetMaNhanVien(username, matKhau, out string ErrorMessage);
                if (!string.IsNullOrEmpty(MaNhanVien))
                {
                    Session.MaNhanVien = MaNhanVien;
                    string name = loginManager.GetTen(tbUsername.Text, matKhau);
                    string role = Session.VaiTro = loginManager.GetVaiTro(tbUsername.Text, matKhau);

                    MessageBox.Show($"Đăng nhập thành công!\nXin chào {name}!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    isLoggedIn = true;
                    if (isLoggedIn)
                    {
                        loginManager.CheckIn(MaNhanVien.ToString(), DateTime.Now);
                        if (cbRememberMe.Checked)
                        {
                            Properties.Settings.Default.SavedUsername = tbUsername.Text.Trim();
                            Properties.Settings.Default.SavedPassword = tbPassword.Text.Trim();
                            Properties.Settings.Default.RememberMe = true;
                        }
                        else
                        {
                            Properties.Settings.Default.SavedUsername = "";
                            Properties.Settings.Default.SavedPassword = "";
                            Properties.Settings.Default.RememberMe = false;
                        }
                        Properties.Settings.Default.Save();
                    }
                    this.Hide();
                    MenuForm menu = new MenuForm(name);
                    menu.ShowDialog();
                    this.Show();
                }
                else
                {
                    ToastService.Show(ErrorMessage, this);
                }
            }
            catch (Exception ex)
            {
                ToastService.Show("Lỗi đăng nhập: " + ex.Message, this);
            }
        }

        private void btnDong_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DangNhap_Load(object sender, EventArgs e)
        {
            Session.MayTinhXuLy = Environment.MachineName;
            if (Properties.Settings.Default.RememberMe)
            {
                tbUsername.Text = Properties.Settings.Default.SavedUsername;
                tbPassword.Text = Properties.Settings.Default.SavedPassword;
                cbRememberMe.Checked = true;
            }
            else
            {
                cbRememberMe.Checked = false;
                tbUsername.Text = "";
                tbPassword.Text = "";
            }
            if (tbPassword.UseSystemPasswordChar == false)
            {
                btnShowPassword.Text = "Hide";
            }
            else
            {
                btnShowPassword.Text = "Show";
            }
        }

        private void DangNhap_FormClosing(object sender, FormClosingEventArgs e)
        {
            isLoggedIn = false;
        }

        private void tbPassword_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void tbPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnDangNhap.PerformClick();
                e.Handled = true;
            }
        }

        private void btnShowPassword_Click(object sender, EventArgs e)
        {

            isPasswordVisible = !isPasswordVisible;
            UpdatePasswordVisibility();
        }
        private void UpdatePasswordVisibility()
        {
            if (btnShowPassword.Text == "Show")
            {
                tbPassword.UseSystemPasswordChar = false;
                btnShowPassword.Text = "Hide";
            }
            else if (btnShowPassword.Text == "Hide")
            {
                tbPassword.UseSystemPasswordChar = true;
                btnShowPassword.Text = "Show";
            }
        }
    }
}
