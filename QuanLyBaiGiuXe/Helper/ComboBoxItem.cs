namespace QuanLyBaiGiuXe.Helper
{
    public class ComboBoxItem
    {
        public int Value { get; set; }
        public required string Text { get; set; }
        public override string ToString() => Text;
    }
}
