﻿using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using Mono.Unix;
using Mono.Unix.Native;

namespace PI_I2C_NET
{
	public class I2CBus : II2CBus, IDisposable
	{
		private int busHandle;


		/// <summary>
		/// .ctor
		/// </summary>
		/// <param name="busPath"></param>
		protected I2CBus(string busPath)
		{
			int res= I2CNativeLib.OpenBus(busPath);
			if (res< 0)
				throw new IOException(String.Format("Error opening bus '{0}': {1}", busPath, UnixMarshal.GetErrorDescription(Stdlib.GetLastError())));
			
			busHandle = res;
		}


		/// <summary>
		/// Creates new instance of I2CBus.
		/// </summary>
		/// <param name="busPath">Path to system file associated with I2C bus.<br/>
		/// For RPi rev.1 it's usually "/dev/i2c-0",<br/>
		/// For rev.2 it's "/dev/i2c-1".</param>
		/// <returns></returns>
		public static I2CBus Open(string busPath)
		{
			return new I2CBus(busPath);
		}



		public void Finalyze()
		{
			Dispose(false);
		}



		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}



		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				// disposing managed resouces
			}

			if (busHandle != 0)
			{
				I2CNativeLib.CloseBus(busHandle);
				busHandle = 0;
			}
		}

		public int set_slave_address(byte slaveAdress)
		{
			if (busHandle > 0) 
			{
				int err = I2CNativeLib.set_slave_address (busHandle, slaveAdress, 0);
				return err;
			}
			return -1;
		}
		public int readSMBUSWord(byte regAddress)
		{
			int res = I2CNativeLib.i2c_smbus_read_word_data (busHandle, regAddress);
			return res;
		}


		public int read_SMBUS_Byte_Data(byte regAddress, out byte read)
		{
			int res = I2CNativeLib.i2c_smbus_read_byte_data (busHandle, regAddress);
			read = 0;
			if (res >= 0)
				read = (byte)res;
			else
				return -1;

			return 0;
		}

		public int read_SMBUS_i2c_Block_data(byte regAddress, ref byte[] data)
		{
			byte length = (byte)data.Length;
			//IntPtr p = Marshal.AllocHGlobal (Marshal.SizeOf (typeof(byte)) * length);
			//Marshal.Copy (data, 0, p, length);
			int res = I2CNativeLib.i2c_smbus_read_i2c_block_data (busHandle, regAddress,length,data);
			//Marshal.Copy (p, 0, data, length);
			//Marshal.FreeHGlobal (p);
			return  res;
		}

		public int write_SMBUS_Byte_Data(byte regAddress, byte val)
		{
			return I2CNativeLib.i2c_smbus_write_byte_data (busHandle, regAddress, val);
		}

		/// <summary>
		/// Writes single byte.
		/// </summary>
		/// <param name="address">Address of a destination device</param>
		/// <param name="b"></param>
		public void WriteByte(int address, byte b)
		{
			byte[] bytes= new byte[1];
			bytes[0] = b;
			WriteBytes(address, bytes);
		}


		/// <summary>
		/// Writes array of bytes.
		/// </summary>
		/// <remarks>Do not write more than 3 bytes at once, RPi drivers don't support this currently.</remarks>
		/// <param name="address">Address of a destination device</param>
		/// <param name="bytes"></param>
		public void WriteBytes(int address, byte[] bytes)
		{
			int res= I2CNativeLib.WriteBytes(busHandle, address, bytes, bytes.Length);
			if (res== -1)
				throw new IOException(String.Format("Error accessing address '{0}': {1}", address, UnixMarshal.GetErrorDescription(Stdlib.GetLastError())));
			if (res== -2)
				throw new IOException(String.Format("Error writing to address '{0}': I2C transaction failed", address));
		}


		/// <summary>
		/// Writes command with data.
		/// </summary>
		/// <param name="address"></param>
		/// <param name="command"></param>
		/// <param name="data"></param>
		public void WriteCommand(int address, byte command, byte data)
		{
			byte[] bytes = new byte[2];
			bytes[0] = command;
			bytes[1] = data;
			WriteBytes (address, bytes);
		}


		/// <summary>
		/// Writes command with data.
		/// </summary>
		/// <param name="address"></param>
		/// <param name="command"></param>
		/// <param name="data1"></param>
		/// <param name="data2"></param>
		public void WriteCommand(int address, byte command, byte data1, byte data2)
		{
			byte[] bytes = new byte[3];
			bytes[0] = command;
			bytes[1] = data1;
			bytes[2] = data2;
			WriteBytes(address, bytes);
		}

		
		/// <summary>
		/// Writes command with data.
		/// </summary>
		/// <param name="address"></param>
		/// <param name="command"></param>
		/// <param name="data"></param>
		public void WriteCommand(int address, byte command, ushort data)
		{
			byte[] bytes = new byte[3];
			bytes[0] = command;
			bytes[1] = (byte)(data & 0xff);
			bytes[2] = (byte)(data >> 8);
			WriteBytes(address, bytes);
		}


		/// <summary>
		/// Reads bytes from device with passed address.
		/// </summary>
		/// <param name="address"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public byte[] ReadBytes(int address, int count)
		{
			byte[] buf = new byte[count];
			int res= I2CNativeLib.ReadBytes(busHandle, address, buf, buf.Length);
			if (res== -1)
				throw new IOException(String.Format("Error accessing address '{0}': {1}", address, UnixMarshal.GetErrorDescription(Stdlib.GetLastError())));
			if (res<= 0)
				throw new IOException(String.Format("Error reading from address '{0}': I2C transaction failed", address));

			if (res< count)
				Array.Resize(ref buf, res);

			return buf;
		}
	}
}