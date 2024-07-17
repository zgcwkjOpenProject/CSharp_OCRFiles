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
            dialog.Filter = "图片文件(*.bmp;*.jpg;*.jpeg;*.tif;*.tiff;*.png)|*.bmp;*.jpg;*.jpeg;*.tif;*.tiff;*.png";
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
                    MessageBox.Show("文件路径异常");
                    return;
                }
                var config = new Config(this, filePath);
                config.ShowDialog();
            }
            else
            {
                MessageBox.Show("请选择文件");
            }
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            await Task.Delay(0);
            //识别框选内容
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
                    //打开图片
                    var imagebyte = File.ReadAllBytes(filePath);
                    var bitmap = new Bitmap(new MemoryStream(imagebyte));
                    //获取框选区域的图片坐标
                    var start = clickPositions[0];
                    var end = clickPositions[1];
                    var cropRect = new Rectangle(
                        Math.Min(start.X, end.X),
                        Math.Min(start.Y, end.Y),
                        Math.Abs(start.X - end.X),
                        Math.Abs(start.Y - end.Y)
                    );
                    //截取图片区域
                    if (bitmap.Width >= cropRect.Width && bitmap.Height >= cropRect.Height)
                    {
                        //var target = new Bitmap(cropRect.Width, cropRect.Height);
                        var target = new Bitmap(cropRect.Width + 100, cropRect.Height + 100);//增加宽高，不然识别不出来
                        using (var g = Graphics.FromImage(target))
                        {
                            //绘制白色背景
                            g.Clear(Color.White);
                            //绘制原图
                            g.DrawImage(bitmap,
                                //new Rectangle(0, 0, target.Width, target.Height),
                                new Rectangle(50, 50, cropRect.Width, cropRect.Height),//增加宽高，不然识别不出来
                                cropRect,
                                GraphicsUnit.Pixel);
                        }
                        //保存截取区域
                        target.Save($"{newPath}/{fileName}", ImageFormat.Jpeg);
                    }
                    //OCR识别
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
            //识别内容
            foreach (DataGridViewRow item in dataGridView1.Rows)
            {
                var fileName = item.Cells[0].Value.ToString();
                var fileNewName = item.Cells[1].Value.ToString();
                var filePath = item.Cells[3].Value.ToString();
                //OCR识别
                item.Cells[2].Value = await RecognizeAsync(filePath);
            }
            //完成
            MessageBox.Show("识别完成");
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
                MessageBox.Show("批量提取内容完成");
            }
            else
            {
                MessageBox.Show("没有数据");
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
                MessageBox.Show("批量重命名完成");
            }
            else
            {
                MessageBox.Show("没有数据");
            }
        }

        /// <summary>
        /// 去除文件名中的非法字符
        /// </summary>
        /// <param name="fileName">文件名</param>
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
        /// 识别图片 OCR
        /// <para>详看：https://learn.microsoft.com/en-us/samples/microsoft/windows-universal-samples/ocr/</para>
        /// </summary>
        /// <param name="imagePath">图片地址</param>
        /// <param name="language">语言</param>
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
