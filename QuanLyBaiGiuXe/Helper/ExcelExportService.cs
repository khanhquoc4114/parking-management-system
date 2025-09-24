using System.IO;
using OfficeOpenXml;

namespace QuanLyBaiGiuXe.Services
{
    public class ExcelExportService
    {
        public void ExportDataGridViewToExcel(DataGridView dgv, string filePath)
        {
            if (dgv == null || dgv.Columns.Count == 0)
                throw new ArgumentException("DataGridView không có dữ liệu để export.");

            ExcelPackage.License.SetNonCommercialPersonal("Wuoc");

            using (ExcelPackage package = new ExcelPackage(new FileInfo(filePath)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");

                // Header
                for (int col = 0; col < dgv.Columns.Count; col++)
                {
                    worksheet.Cells[1, col + 1].Value = dgv.Columns[col].HeaderText;
                }

                // Dữ liệu
                for (int row = 0; row < dgv.Rows.Count; row++)
                {
                    for (int col = 0; col < dgv.Columns.Count; col++)
                    {
                        worksheet.Cells[row + 2, col + 1].Value = dgv.Rows[row].Cells[col].Value?.ToString();
                    }
                }

                // Lưu file
                File.WriteAllBytes(filePath, package.GetAsByteArray());
            }
        }
    }
}
