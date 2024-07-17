using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Windows.Storage;

namespace OCRFiles
{
    public partial class Main : Form
    {
        public List<Point> clickPositions { get; set; } = new List<Point>();

        public Main()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await Task.Delay(0);
            var dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            dialog.Filter = "ͼƬ�ļ�(*.bmp;*.jpg;*.jpeg;*.tif;*.tiff;*.png)|*.bmp;*.jpg;*.jpeg;*.tif;*.tiff;*.png";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var files = dialog.FileNames;
                foreach (var file in files)
                {
                    //Console.WriteLine(file);
                    dataGridView1.Rows.Add(Path.GetFileName(file), "", "", file);
                }
            }
            //var dialog = new FolderBrowserDialog();
            //if (dialog.ShowDialog() == DialogResult.OK)
            //{
            //    var folderPath = dialog.SelectedPath;
            //    var extensions = new[] { "*.bmp", "*.jpg", "*.jpeg", "*.tif", "*.tiff", "*.png" };
            //    var files = extensions.SelectMany(ext => Directory.GetFiles(folderPath, ext)).ToList();
            //    foreach (var file in files)
            //    {
            //        //Console.WriteLine(file);
            //        dataGridView1.Rows.Add(Path.GetFileName(file), "", "", file);
            //    }
            //}
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            await Task.Delay(0);
            if (dataGridView1.Rows.Count > 0)
            {
                var filePath = dataGridView1.Rows[0].Cells[3].Value.ToString();
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

        private async void button3_Click(object sender, EventArgs e)
        {
            await Task.Delay(0);
            //ʶ���ѡ����
            if (clickPositions.Count == 2)
            {
                foreach (DataGridViewRow item in dataGridView1.Rows)
                {
                    var fileName = item.Cells[0].Value.ToString();
                    var fileNewName = item.Cells[1].Value.ToString();
                    var filePath = item.Cells[3].Value.ToString();
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
                        //var target = new Bitmap(cropRect.Width, cropRect.Height);
                        var target = new Bitmap(cropRect.Width + 100, cropRect.Height + 100);//���ӿ�ߣ���Ȼʶ�𲻳���
                        using (var g = Graphics.FromImage(target))
                        {
                            //���ư�ɫ����
                            g.Clear(Color.White);
                            //����ԭͼ
                            g.DrawImage(bitmap,
                                //new Rectangle(0, 0, target.Width, target.Height),
                                new Rectangle(50, 50, cropRect.Width, cropRect.Height),//���ӿ�ߣ���Ȼʶ�𲻳���
                                cropRect,
                                GraphicsUnit.Pixel);
                        }
                        //�����ȡ����
                        target.Save($"{newPath}/{fileName}", ImageFormat.Jpeg);
                    }
                    //OCRʶ��
                    var ocrResult = await RecognizeAsync($"{newPath}/{fileName}");
                    if (!string.IsNullOrEmpty(ocrResult))
                    {
                        fileNewName = ocrResult;
                        if (string.IsNullOrEmpty(fileNewName)) continue;
                        if (fileNewName.Length > 20) fileNewName = fileNewName.Substring(0, 20);
                        item.Cells[1].Value = CleanFileName($"{fileNewName}{extension}");
                    }
                }
            }
            //ʶ������
            foreach (DataGridViewRow item in dataGridView1.Rows)
            {
                var fileName = item.Cells[0].Value.ToString();
                var fileNewName = item.Cells[1].Value.ToString();
                var filePath = item.Cells[3].Value.ToString();
                //OCRʶ��
                item.Cells[2].Value = await RecognizeAsync(filePath);
            }
            //���
            MessageBox.Show("ʶ�����");
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            await Task.Delay(0);
            if (dataGridView1.Rows.Count > 0)
            {
                foreach (DataGridViewRow item in dataGridView1.Rows)
                {
                    if (item.Cells[2].Value == null) continue;
                    if (item.Cells[3].Value == null) continue;
                    var fileName = item.Cells[0].Value.ToString();
                    var fileContent = item.Cells[2].Value.ToString();
                    var filePath = item.Cells[3].Value.ToString();
                    var path = Path.GetDirectoryName(filePath);
                    var extension = Path.GetExtension(fileName);
                    File.WriteAllText($"{path}/{fileName}.txt", fileContent);
                }
                MessageBox.Show("������ȡ�������");
            }
            else
            {
                MessageBox.Show("û������");
            }
        }

        private async void button5_Click(object sender, EventArgs e)
        {
            await Task.Delay(0);
            if (dataGridView1.Rows.Count > 0)
            {
                foreach (DataGridViewRow item in dataGridView1.Rows)
                {
                    if (item.Cells[1].Value == null) continue;
                    if (item.Cells[3].Value == null) continue;
                    var fileName = item.Cells[0].Value.ToString();
                    var fileNewName = item.Cells[1].Value.ToString();
                    var filePath = item.Cells[3].Value.ToString();
                    var path = Path.GetDirectoryName(filePath);
                    var newPath = $"{path}/Copy";
                    var extension = Path.GetExtension(fileName);
                    if (fileNewName == "") fileNewName = fileName;
                    if (!Directory.Exists(newPath)) Directory.CreateDirectory(newPath);
                    File.Copy($"{path}/{fileName}", $"{newPath}/{fileNewName}");
                }
                MessageBox.Show("�������������");
            }
            else
            {
                MessageBox.Show("û������");
            }
        }

        /// <summary>
        /// ȥ���ļ����еķǷ��ַ�
        /// </summary>
        /// <param name="fileName">�ļ���</param>
        /// <returns></returns>
        private string CleanFileName(string fileName)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            foreach (var invalidChar in invalidChars)
            {
                fileName = fileName.Replace(invalidChar.ToString(), "");
            }
            return fileName;
        }

        /// <summary>
        /// ʶ��ͼƬ OCR
        /// <para>�꿴��https://learn.microsoft.com/en-us/samples/microsoft/windows-universal-samples/ocr/</para>
        /// </summary>
        /// <param name="imagePath">ͼƬ��ַ</param>
        /// <param name="language">����</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private async Task<string> RecognizeAsync(string imagePath, string language = "zh-Hans-CN")
        {
            var result = string.Empty;
            var path = Path.GetFullPath(imagePath);
            var storageFile = await StorageFile.GetFileFromPathAsync(path);
            using (var randomAccessStream = await storageFile.OpenReadAsync())
            {
                var decoder = await BitmapDecoder.CreateAsync(randomAccessStream);
                using (var softwareBitmap = await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied))
                {
                    var space = language.Contains("zh") ? "" : " ";
                    var lang = new Windows.Globalization.Language(language);
                    if (OcrEngine.IsLanguageSupported(lang))
                    {
                        var engine = OcrEngine.TryCreateFromLanguage(lang);
                        if (engine != null)
                        {
                            var ocrResult = await engine.RecognizeAsync(softwareBitmap);
                            foreach (var tempLine in ocrResult.Lines)
                            {
                                var line = "";
                                foreach (var word in tempLine.Words)
                                {
                                    line += word.Text + space;
                                }
                                result += line + Environment.NewLine;
                            }
                        }
                    };
                    return result;
                }
            }
        }
    }
}
