using QuanLyBaiGiuXe.DataAccess;
using QuanLyBaiGiuXe.Helper;
using QuanLyBaiGiuXe.Models;
using System.Data;
using System.IO;
using System.Drawing.Imaging;
using QuanLyBaiGiuXe.Properties;
using Microsoft.Data.SqlClient;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace QuanLyBaiGiuXe
{
    public partial class XuLyMatThe : Form
    {
        Manager manager = new Manager();
        string maveluot = string.Empty;
        Connector db = new Connector();
        TinhTienManager TinhTienManager = new TinhTienManager();
        public bool isThanhCong = false;
        private VideoCapture capture;
        private CancellationTokenSource cts;
        private Task cameraTask;
        private readonly object lockObject = new object();

        public XuLyMatThe(string maveluot = "0")
        {
            InitializeComponent();
            this.maveluot = maveluot;
        }

        #region Button Logic
        private void btnChoRaKoTinhPhi_Click(object sender, EventArgs e)
        {
            bool raAnh = XuLyPathAnh("ra_", out string path);
            pbRa.Image = System.Drawing.Image.FromFile(path);
            if (cbKhoaThe.Checked == true)
            {
                bool isKhoaThe = manager.SetTrangThaiSuDungThe(tbMaThe.Text, -1);
                if (!isKhoaThe)
                {
                    ToastService.Show("Không thể khóa thẻ!", this);
                    return;
                }
                else
                {
                    ToastService.Show("Khóa thẻ thành công!", this);
                }
            }
            bool result = manager.CapNhatMatTheVeLuot(maveluot, path, 0, cbKhoaThe.Checked);
            if (result)
            {
                ToastService.Show("Cập nhật vé thành công!, 0 đ", this);
                isThanhCong = true;
                LoadData();
                StopCamera();
            }
            else
            {
                ToastService.Show("Cập nhật vé thất bại!", this);
            }
        }

        private void btnChoRaTinhPhi_Click(object sender, EventArgs e)
        {
            bool raAnh = XuLyPathAnh("ra_", out string path);
            pbRa.Image = System.Drawing.Image.FromFile(path);
            if (cbKhoaThe.Checked == true)
            {
                bool isKhoaThe = manager.SetTrangThaiSuDungThe(tbMaThe.Text, -1);
                if (!isKhoaThe)
                {
                    ToastService.Show("Không thể khóa thẻ!", this);
                    return;
                }
                else
                {
                    ToastService.Show("Khóa thẻ thành công!", this);
                }
            }

            int tien = Convert.ToInt32(tbGiaVe.Text) + AppConfig.TienPhatMatThe;
            bool result = manager.CapNhatMatTheVeLuot(maveluot, path, tien, cbKhoaThe.Checked);
            if (result)
            {
                ToastService.Show($"Cập nhật vé thành công!, {tien.ToString("N0")} đ", this);
                isThanhCong = true;
                StopCamera();
                LoadData();
            }
            else
            {
                ToastService.Show("Cập nhật vé thất bại!", this);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        private void XuLyMatThe_Load(object sender, EventArgs e)
        {
            LoadCamera();
            LoadData();
        }
        private void LoadData()
        {
            lbTienPhat.Text = "(+ " + AppConfig.TienPhatMatThe.ToString("N0") + " VNĐ)";

            try
            {
                db.OpenConnection();
                using (var cmd = new SqlCommand(@"sp_xulymatthe", db.GetConnection()))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@maveluot", maveluot);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            tbMaThe.Text = reader["MaThe"].ToString();
                            tbBienSo.Text = reader["BienSo"].ToString();
                            tbLoaiXe.Text = reader["TenLoaiXe"].ToString();
                            tbThoiGianVao.Text = reader["ThoiGianVao"].ToString();
                            DateTime tgVao = Convert.ToDateTime(reader["ThoiGianVao"]);
                            DateTime tgRa = DateTime.Now;
                            tbNhanVien.Text = reader["HoTen"].ToString();
                            tbMayTinh.Text = reader["MayTinhXuLy"].ToString();
                            string CachTinhTien = reader["CachTinhTien"].ToString();
                            int maLoaiXe = Convert.ToInt32(reader["MaLoaiXe"]);
                            string TongTienReal = reader["TongTien"].ToString();
                            string AnhVaoPath = reader["AnhVaoPath"].ToString();
                            string AnhRaPath = reader["AnhRaPath"].ToString();
                            string TrangThai = reader["TrangThai"].ToString();
                            if (TrangThai == "Chưa ra")
                            {
                                int tongtiengiasu = TinhTienManager.TinhTien(CachTinhTien, tgVao, tgRa, maLoaiXe);
                                tbGiaVe.Text = tongtiengiasu.ToString();
                            }
                            else if (TrangThai == "Đã ra")
                            {
                                tbGiaVe.Text = TongTienReal;
                                lbTienPhat.Text = "";
                                btnChoRaKoTinhPhi.Enabled = false;
                                btnChoRaTinhPhi.Enabled = false;
                                cbKhoaThe.Enabled = false;
                                StopCamera();
                            }

                            LoadImageToPictureBox(pbVao, AnhVaoPath);
                            if (!string.IsNullOrEmpty(AnhRaPath))
                            {
                                LoadImageToPictureBox(pbRa, AnhRaPath);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy dữ liệu với mã vé lượt đã cho.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lấy danh sách loại xe: " + ex.Message);
            }
        }
        private void tbMaThe_KeyDown(object sender, KeyEventArgs e)
        {

        }
        private void LoadImageToPictureBox(PictureBox picBox, string imagePath)
        {
            try
            {
                if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                {
                    picBox.SizeMode = PictureBoxSizeMode.Zoom;
                    picBox.Image?.Dispose();

                    using (var ms = new MemoryStream(File.ReadAllBytes(imagePath)))
                    {
                        picBox.Image = System.Drawing.Image.FromStream(ms);
                    }
                }
                else
                {
                    picBox.Image = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi load ảnh: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                picBox.Image = null;
            }
        }
        private bool XuLyPathAnh(string prefix, out string imagePath)
        {
            imagePath = string.Empty;
            string path = Settings.Default.ImagePath;
            try
            {
                string imageDir = Path.Combine($@"{path}\XeImages", DateTime.Now.ToString("yyyyMMdd"));

                if (!Directory.Exists(imageDir))
                    Directory.CreateDirectory(imageDir);

                string fileName = prefix + "_" + Guid.NewGuid().ToString("N").Substring(0, 9) + ".jpg";
                imagePath = Path.Combine(imageDir, fileName);

                using (Bitmap img = new Bitmap(pbRa.Image))
                {
                    img.Save(imagePath, ImageFormat.Jpeg);
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lưu ảnh: " + ex.Message);
                return false;
            }
        }

        private void XuLyMatThe_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                StopCamera();

                // Đợi camera task hoàn thành (tối đa 2 giây)
                if (cameraTask != null && !cameraTask.IsCompleted)
                {
                    if (!cameraTask.Wait(2000))
                    {
                        // Force cancel nếu không dừng được
                        cts?.Cancel();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Form closing error: {ex.Message}");
            }
        }

        private void LoadCamera()
        {
            try
            {
                // Stop existing camera first
                StopCamera();

                capture = new VideoCapture(0);
                if (!capture.IsOpened())
                {
                    MessageBox.Show("Không tìm thấy camera!");
                    return;
                }

                cts = new CancellationTokenSource();

                cameraTask = Task.Run(async () =>
                {
                    var mat = new Mat();
                    try
                    {
                        while (!cts.Token.IsCancellationRequested)
                        {
                            lock (lockObject)
                            {
                                if (capture == null || !capture.IsOpened())
                                    break;

                                if (!capture.Read(mat) || mat.Empty())
                                    continue;
                            }

                            // Kiểm tra form còn tồn tại không
                            if (IsDisposed || Disposing)
                                break;

                            using var bmp = BitmapConverter.ToBitmap(mat);
                            var toShow = (Bitmap)bmp.Clone();

                            // Kiểm tra PictureBox còn tồn tại
                            if (pbRa.IsDisposed || pbRa.Disposing)
                            {
                                toShow?.Dispose();
                                break;
                            }

                            if (pbRa.InvokeRequired)
                            {
                                try
                                {
                                    pbRa.Invoke(new Action(() =>
                                    {
                                        if (!pbRa.IsDisposed && !IsDisposed)
                                        {
                                            pbRa.Image?.Dispose();
                                            pbRa.Image = toShow;
                                            pbRa.SizeMode = PictureBoxSizeMode.Zoom;
                                        }
                                        else
                                        {
                                            toShow?.Dispose();
                                        }
                                    }));
                                }
                                catch (ObjectDisposedException)
                                {
                                    toShow?.Dispose();
                                    break;
                                }
                                catch (InvalidOperationException)
                                {
                                    // Control handle không còn tồn tại
                                    toShow?.Dispose();
                                    break;
                                }
                            }
                            else
                            {
                                if (!pbRa.IsDisposed && !IsDisposed)
                                {
                                    pbRa.Image?.Dispose();
                                    pbRa.Image = toShow;
                                    pbRa.SizeMode = PictureBoxSizeMode.Zoom;
                                }
                                else
                                {
                                    toShow?.Dispose();
                                    break;
                                }
                            }

                            // Sử dụng delay có thể cancel được
                            await Task.Delay(33, cts.Token);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        // Expected khi cancel
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Camera task error: {ex.Message}");
                    }
                    finally
                    {
                        mat?.Dispose();
                    }
                }, cts.Token);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khởi tạo camera: {ex.Message}");
            }
        }

        private void StopCamera()
        {
            try
            {
                // Cancel task trước
                cts?.Cancel();

                // Đợi task dừng (tối đa 1 giây)
                if (cameraTask != null && !cameraTask.IsCompleted)
                {
                    cameraTask.Wait(1000);
                }

                lock (lockObject)
                {
                    // Clean up capture
                    capture?.Release();
                    capture?.Dispose();
                    capture = null;
                }

                // Clean up CancellationTokenSource
                cts?.Dispose();
                cts = null;
                cameraTask = null;

                // Clean up PictureBox image
                if (pbRa != null && !pbRa.IsDisposed)
                {
                    pbRa.Image?.Dispose();
                    pbRa.Image = null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Stop camera error: {ex.Message}");
            }
        }

        // Thêm method này để cleanup khi form load
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // Đảm bảo cleanup khi form đóng
            this.FormClosed += (s, args) => StopCamera();
        }
    }
}