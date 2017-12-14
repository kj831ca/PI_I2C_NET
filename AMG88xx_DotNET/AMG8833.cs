using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Posix;
using Mono.Unix;
using Mono.Unix.Native;
using PI_I2C_NET;
using System.IO;

namespace AMG88xx_DotNET
{
    public class AMG8833
    {
        I2CBus _i2cBus;
        public const byte CtrlReg = 0x00;
        public const byte ResetReg = 0x01;
        public const byte ThermisterReg = 0x0E;
        private string i2cPath;
        private short[,] pixelData;
        public enum AMG88xxReset : byte
        {
            FLAG_RESET = 0x30,
            INITIAL_RESET = 0x3F
        }
        public enum AMG88xxCtrlMode: byte
        {
            NORMAL = 0x00,
            SLEEP  = 0x10,
            STANDBY_60SEC = 0x20,
            STANDBY_10SEC = 0x21
        }
        public AMG8833()
        { 
            i2cPath = "/dev/i2c-1";
            _i2cBus = I2CBus.Open(i2cPath);
            _i2cBus.set_slave_address(0x69);
            init();
        }
        /// <summary>
        /// Use this method to create AMG88xx device if you are using PI 1 
        /// </summary>
        /// <param name="devPath"></param>
        /// <param name="address">address of AMG88xx</param>
        public AMG8833(string devPath, byte address)
        {
            i2cPath = devPath;
            _i2cBus = I2CBus.Open(i2cPath);
            _i2cBus.set_slave_address(address);
            init();
        }
        public int SetMode(AMG88xxCtrlMode mode)
        {
            return _i2cBus.write_SMBUS_Byte_Data(CtrlReg, (byte)mode);
        }
        public int Reset(AMG88xxReset resetType)
        {
            return _i2cBus.write_SMBUS_Byte_Data(ResetReg, (byte)resetType);
        }
        private void init()
        {
            pixelData = new short[8, 8];
            if ( SetMode(AMG88xxCtrlMode.NORMAL) < 0)
                throw new IOException(String.Format("Error opening bus '{0}': {1}", i2cPath, UnixMarshal.GetErrorDescription(Stdlib.GetLastError())));
            if(Reset(AMG88xxReset.INITIAL_RESET)<0)
                throw new IOException(String.Format("Error opening bus '{0}': {1}", i2cPath, UnixMarshal.GetErrorDescription(Stdlib.GetLastError())));

        }
        private short scaleSign12Bit(int data)
        {
            short b16bit = (short)data;
            if ((data & 0x800) != 0)
                return (short)(-1 * ~(data & 0x7FF));
            else
                return (short)data;
        }
        public int ReadThermister()
        {
            return _i2cBus.readSMBUSWord(ThermisterReg);
        }

        public double ReadThermister(int data)
        {
            if (data < 0) return double.NaN;
            short scaleData = scaleSign12Bit(data);
            return 0.0625 * scaleData;
        }
        public double ConvertPixelTemperature(short data)
        {
            return -0.25 * scaleSign12Bit(data);
        }
        public short[,] ReadPixels()
        {
            byte[] blockData = new byte[32];
            byte addr = 0x80;
            int row = 0;

            for(int i=0;i<4;i++)
            {
                if(_i2cBus.read_SMBUS_i2c_Block_data(addr,ref blockData)>0)
                {
                    for(int j=0;j<16;j+=2)
                    {
                        pixelData[row, j / 2] = (short)(blockData[j] | (blockData[j++] << 8));
                    }
                    row++;
                    for(int j=16;j<32;j+=2)
                    {
                        pixelData[row, (j-16)/2] = (short)(blockData[j] | (blockData[j++] << 8));
                    }
                    row++;
                } else
                {
                    return null; //We got some error here .
                }
                addr += 32;
            }
            return pixelData;
        }
        public StringBuilder PrintPixel(short[,]pixel)
        {
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    str.Append(pixel[i, j].ToString());
                    str.Append(" ");
                }
                str.Append("/r/n");
            }
            return str;
        }
    }
}
