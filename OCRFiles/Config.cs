using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace OCRFiles
{
    public partial class Config : Form
    {
        private string filePath { get; }
        private Main main { get; }

        public Config(Main main, string filePath)
        {
            InitializeComponent();
            //
            this.main = main;
            this.filePath = filePath;
            //
            MessageBox.Show("鼠标单击图片框选区域");
            var imagebyte = File.ReadAllBytes(filePath);
            pictureBox1.Image = new Bitmap(new MemoryStream(imagebyte));
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (main.clickPositions.Count() >= 2)
            {
                main.clickPositions.RemoveAt(0);
            }
            var clickPosition = GetImageCoordinates(e.Location);
            main.clickPositions.Add(clickPosition);
            pictureBox1.Invalidate();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            using (var pen = new Pen(Color.Red, 1))
            {
                foreach (var pos in main.clickPositions)
                {
                    var drawPosition = GetControlCoordinates(pos);
                    // 画横线
                    e.Graphics.DrawLine(pen, new Point(0, drawPosition.Y), new Point(pictureBox1.Width, drawPosition.Y));
                    // 画竖线
                    e.Graphics.DrawLine(pen, new Point(drawPosition.X, 0), new Point(drawPosition.X, pictureBox1.Height));
                }
            }
        }

        private Point GetImageCoordinates(Point controlCoordinates)
        {
            // 计算图片的实际大小和位置
            Size imageSize = pictureBox1.Image.Size;
            Size clientSize = pictureBox1.ClientSize;
            float imageAspect = (float)imageSize.Width / imageSize.Height;
            float clientAspect = (float)clientSize.Width / clientSize.Height;

            int imageWidth, imageHeight;
            if (imageAspect > clientAspect)
            {
                imageWidth = clientSize.Width;
                imageHeight = (int)(clientSize.Width / imageAspect);
            }
            else
            {
                imageHeight = clientSize.Height;
                imageWidth = (int)(clientSize.Height * imageAspect);
            }

            int offsetX = (clientSize.Width - imageWidth) / 2;
            int offsetY = (clientSize.Height - imageHeight) / 2;

            // 转换为图片坐标
            int x = (int)((controlCoordinates.X - offsetX) * ((float)imageSize.Width / imageWidth));
            int y = (int)((controlCoordinates.Y - offsetY) * ((float)imageSize.Height / imageHeight));
            return new Point(x, y);
        }

        private Point GetControlCoordinates(Point imageCoordinates)
        {
            // 计算图片的实际大小和位置
            Size imageSize = pictureBox1.Image.Size;
            Size clientSize = pictureBox1.ClientSize;
            float imageAspect = (float)imageSize.Width / imageSize.Height;
            float clientAspect = (float)clientSize.Width / clientSize.Height;

            int imageWidth, imageHeight;
            if (imageAspect > clientAspect)
            {
                imageWidth = clientSize.Width;
                imageHeight = (int)(clientSize.Width / imageAspect);
            }
            else
            {
                imageHeight = clientSize.Height;
                imageWidth = (int)(clientSize.Height * imageAspect);
            }

            int offsetX = (clientSize.Width - imageWidth) / 2;
            int offsetY = (clientSize.Height - imageHeight) / 2;

            // 转换为控件坐标
            int x = (int)(imageCoordinates.X * ((float)imageWidth / imageSize.Width) + offsetX);
            int y = (int)(imageCoordinates.Y * ((float)imageHeight / imageSize.Height) + offsetY);
            return new Point(x, y);
        }
    }
}
