using PaddleOCRSharp;
using System.Drawing.Imaging;

namespace OCRFiles
{
    public partial class Main : Form
    {
        private PaddleOCREngine engine { get; }
        public List<Point> clickPositions { get; set; } = [];

        public Main()
        {
            InitializeComponent();
            //
            var config = null as OCRModelConfig;
            var oCRParameter = new OCRParameter();
            engine = new PaddleOCREngine(config, oCRParameter);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var folderPath = dialog.SelectedPath;
                var extensions = new[] { "*.bmp", "*.jpg", "*.jpeg", "*.tif", "*.tiff", "*.png" };
                var files = extensions.SelectMany(ext => Directory.GetFiles(folderPath, ext)).ToList();
                foreach (var file in files)
                {
                    //Console.WriteLine(file);
                    dataGridView1.Rows.Add(Path.GetFileName(file), "", file);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)
            {
                var filePath = dataGridView1.Rows[0].Cells[2].Value.ToString();
                if (filePath == null || filePath == "")
                {
                    MessageBox.Show("�ļ�·���쳣");
                    return;
                }
                var config = new Config(this, filePath);
                config.ShowDialog();
            }
            else
            {
                MessageBox.Show("��ѡ���ļ�");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (clickPositions.Count == 2)
            {
                foreach (DataGridViewRow item in dataGridView1.Rows)
                {
                    var fileName = item.Cells[0].Value.ToString();
                    var fileNewName = item.Cells[1].Value.ToString();
                    var filePath = item.Cells[2].Value.ToString();
                    var path = Path.GetDirectoryName(filePath);
                    var newPath = $"{path}/Crop";
                    var extension = Path.GetExtension(fileName);
                    if (fileNewName == "") fileNewName = fileName;
                    if (!Directory.Exists(newPath)) Directory.CreateDirectory(newPath);
                    //��ͼƬ
                    var imagebyte = File.ReadAllBytes(filePath);
                    var bitmap = new Bitmap(new MemoryStream(imagebyte));
                    //��ȡ��ѡ�����ͼƬ����
                    var start = clickPositions[0];
                    var end = clickPositions[1];
                    var cropRect = new Rectangle(
                        Math.Min(start.X, end.X),
                        Math.Min(start.Y, end.Y),
                        Math.Abs(start.X - end.X),
                        Math.Abs(start.Y - end.Y)
                    );
                    //��ȡͼƬ����
                    if (bitmap.Width >= cropRect.Width && bitmap.Height >= cropRect.Height)
                    {
                        var target = new Bitmap(cropRect.Width, cropRect.Height);
                        using var g = Graphics.FromImage(target);
                        {
                            g.DrawImage(bitmap,
                                new Rectangle(0, 0, target.Width, target.Height),
                                cropRect,
                                GraphicsUnit.Pixel);
                        }
                        //�����ȡ����
                        target.Save($"{newPath}/{fileName}", ImageFormat.Jpeg);
                        bitmap = target;
                    }
                    //�ٶȷɽ�OCRʶ��
                    var ocrResult = engine.DetectText(bitmap);
                    if (ocrResult != null)
                    {
                        fileNewName = ocrResult.Text;
                        if (fileNewName.Length > 20) fileNewName = fileNewName[..20];
                        item.Cells[1].Value = CleanFileName($"{fileNewName}{extension}");
                    }
                }
                MessageBox.Show("ʶ�����");
            }
            else
            {
                MessageBox.Show("����ȷ����");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)
            {
                foreach (DataGridViewRow item in dataGridView1.Rows)
                {
                    var fileName = item.Cells[0].Value.ToString();
                    var fileNewName = item.Cells[1].Value.ToString();
                    var filePath = item.Cells[2].Value.ToString();
                    var path = Path.GetDirectoryName(filePath);
                    var newPath = $"{path}/Copy";
                    var extension = Path.GetExtension(fileName);
                    if (fileNewName == "") fileNewName = fileName;
                    if (!Directory.Exists(newPath)) Directory.CreateDirectory(newPath);
                    File.Copy($"{path}/{fileName}", $"{newPath}/{fileNewName}");
                }
                MessageBox.Show("���������");
            }
            else
            {
                MessageBox.Show("û������");
            }
        }

        private string CleanFileName(string fileName)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            foreach (char invalidChar in invalidChars)
            {
                fileName = fileName.Replace(invalidChar.ToString(), "");
            }
            return fileName;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "http://zgcwkj.cn/");
        }
    }
}
