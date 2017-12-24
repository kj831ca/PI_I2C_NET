using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace AMG88xx_DotNET
{
    public class PixelColorMap
    {
        Color[,] _pixelsColors;
        AMG8833 _sensor;
        Color[] _temperatureTable;
        public event EventHandler<Color[,]> OnPixelColorChanged;
        public PixelColorMap(AMG8833 pixelSensor)
        {
            _sensor = pixelSensor;
            _sensor.OnPixelsDataChanged += _sensor_OnPixelsDataChanged;
            init();
        }
        private void init()
        {
            _pixelsColors = new Color[8, 8];
            initTempTable();
        }

        public Color[,] PixelsColor
        {
            get { return _pixelsColors; }
        }
        private void _sensor_OnPixelsDataChanged(object sender, short[,] e)
        {
            for(int x=0;x<8;x++)
                for(int y=0;y<8;y++)
                {
                    _pixelsColors[x, y] = _temperatureTable[e[x, y]];
                }
            OnPixelColorChanged?.Invoke(this, _pixelsColors);
        }
        private void initTempTable()
        {
            _temperatureTable = new Color[1024];
            byte red, green, blue;
            double temp,d_temp;
			double threshold = 30.0;
            for(int i=0;i<1024;i++)
            {
                temp = i * 0.25;
                //Calculate red level
				if(temp <= threshold ) // 60 C
                {
                    red = 255;
                }else
                {
					d_temp = temp - threshold;
                    d_temp = 329.698727 * Math.Pow(d_temp, -0.133204);
                    red = (byte)d_temp;
                    if (d_temp < 0)
                        red = 0;
                    if (d_temp > 255)
                        red = 255;
                }

                //Calculate blue level 
				if(temp >=threshold)
                {
                    blue = 255;
                }else
                {
                    if (temp <= 10)
                        blue = 0;
                    else
                    {
                        d_temp = temp - 10;
                        d_temp = 138.517731 * Math.Log(d_temp) - 305.04479;
                        blue = (byte)d_temp;
                        if (d_temp < 0)
                            blue = 0;
                        if (d_temp > 255)
                            blue = 255;
                    }
                }

                //Calucate green level
                if(temp <=60)
                {
                    d_temp = temp;
                    d_temp = 99.470802 * Math.Log(d_temp) - 161.1195681;
                    green = (byte)d_temp;
                    if (d_temp < 0)
                        green = 0;
                    if (d_temp > 255)
                        green = 255;
                }else
                {
                    d_temp = temp - 60;
                    d_temp = 288.122169 * Math.Pow(temp, -0.0755148492);
                    green = (byte)d_temp;
                    if (d_temp < 0)
                        green = 0;
                    if (d_temp > 255)
                        green = 255;
                }
                _temperatureTable[i] = Color.FromArgb(blue, green, red);
            }


        }
    }
}
