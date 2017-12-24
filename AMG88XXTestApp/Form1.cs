using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AMG88xx_DotNET;

namespace AMG88XXTestApp
{
    public partial class Form1 : Form
    {
        AMG8833 _gridSensor;
        PixelColorMap _pixelColor;
        Bitmap bitmapSensor = new Bitmap(8, 8);
        public Form1()
        {
            _gridSensor = new AMG8833();
            _pixelColor = new PixelColorMap(_gridSensor);
            InitializeComponent();
            _pixelColor.OnPixelColorChanged += _pixelColor_OnPixelColorChanged;
        }
        private delegate void UpdatePictureBox(Bitmap img);

        private void updatePictureBox(Bitmap bitMap)
        {
            if(this.pictureBox1.InvokeRequired)
            {
                this.Invoke(new UpdatePictureBox(updatePictureBox), new object[] { bitMap });
                return;
            }
            Bitmap newImg = new Bitmap(pictureBox1.Size.Width, pictureBox1.Size.Height);
            using (Graphics g = Graphics.FromImage(newImg))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bicubic;
                g.DrawImage(bitMap, new Rectangle(Point.Empty, newImg.Size));
            }
            pictureBox1.Image = newImg;
        }
        private void _pixelColor_OnPixelColorChanged(object sender, Color[,] e)
        {
           for(int x=0;x<8;x++)
                for(int y=0;y<8;y++)
                {
                    bitmapSensor.SetPixel(x, y, e[x, y]);
                }
			updatePictureBox (bitmapSensor);
        }

        private void buttonStartPolling_Click(object sender, EventArgs e)
        {
            _gridSensor.StartMonitor();
        }
    }
}
