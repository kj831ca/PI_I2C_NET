using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using AMG88xx_DotNET;

namespace SampleApp
{
	class Program
	{
		static void Main(string[] args)
		{

            //var bus = PI_I2C_NET.I2CBus.Open ("/dev/i2c-1");
            //double[] d = new double[3];
            //int temperature;

            //bus.set_slave_address(0x69);
            //if (bus.write_SMBUS_Byte_Data (0x00, 0x00)<0) 
            //{ //switch to normal mode
            //	Console.Write("Error initialize");
            //}

            //if (bus.write_SMBUS_Byte_Data (0x01, 0x3F)<0) 
            //{ //switch to normal mode
            //	Console.Write("Error reset");
            //}

            //temperature = bus.readSMBUSWord (0x0E);
            //Console.WriteLine ("Temperature = {0}", temperature);

            //int pix0 = bus.readSMBUSWord (0x80);
            //Console.WriteLine ("Pixel 0 = {0}", pix0);


            //byte[] pixels = new byte[32];
            //int res;
            //pixels [0] = 20;
            //res = bus.read_SMBUS_i2c_Block_data (0x80, ref pixels);
            //if (res > 0) 
            //{
            //	printfTemp (pixels);
            //}
            AMG8833 gridSensor = new AMG8833();
			Console.WriteLine ("Thermister Reading : {0}", gridSensor.ReadThermister ());
			Console.WriteLine ("Read Pixel 0x80: {0}", gridSensor.ReadWord (0x80));
            short[,] pixelData = gridSensor.ReadPixels();

			do {
				while(!Console.KeyAvailable) {
					pixelData = gridSensor.ReadPixels();
					if (pixelData != null) 
					{
						Console.Write (gridSensor.PrintPixel (pixelData));
					}
					Console.WriteLine("-------------------------");
					Thread.Sleep(100);
				}
			} while(Console.ReadKey (true).Key != ConsoleKey.Escape);
		}
		static double scaleToTemp(int t_read)
		{
			return ((double)t_read * 0.02)-273.15;
		}
		private static void printfTemp(byte [] data)
		{
			StringBuilder str = new StringBuilder ();
			int length = data.Length;

			for (int i = 0; i < length - 1; i += 2) 
			{
				int temp = data [i] + (data [i + 1] << 8);
				str.Append (temp.ToString ());
				str.Append (" ");
			}
			Console.WriteLine (str.ToString ());
		}
	}
}
