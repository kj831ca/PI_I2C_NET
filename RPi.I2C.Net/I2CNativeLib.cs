using System.Runtime.InteropServices;

namespace PI_I2C_NET
{
	internal static class I2CNativeLib
	{
		[DllImport("libnativei2c.so", EntryPoint = "openBus", SetLastError = true)]
		public static extern int OpenBus(string busFileName);

		[DllImport("libnativei2c.so", EntryPoint = "closeBus", SetLastError = true)]
		public static extern int CloseBus(int busHandle);

		[DllImport("libnativei2c.so", EntryPoint = "readBytes", SetLastError = true)]
		public static extern int ReadBytes(int busHandle, int addr, byte[] buf, int len);

		[DllImport("libnativei2c.so", EntryPoint = "writeBytes", SetLastError = true)]
		public static extern int WriteBytes(int busHandle, int addr, byte[] buf, int len);

		[DllImport("libnativei2c.so", EntryPoint = "set_slave_address", SetLastError = true)]
		public static extern int set_slave_address(int busHandle, int addr, int force=0);

		[DllImport("libnativei2c.so", EntryPoint = "i2c_smbus_read_word_data", SetLastError = true)]
		public static extern int i2c_smbus_read_word_data(int busHandle, byte subAddr);

		[DllImport("libnativei2c.so", EntryPoint = "i2c_smbus_read_byte", SetLastError = true)]
		public static extern int i2c_smbus_read_byte(int busHandle);

		[DllImport("libnativei2c.so", EntryPoint = "i2c_smbus_write_byte", SetLastError = true)]
		public static extern int i2c_smbus_write_byte(int busHandle, byte value);

		[DllImport("libnativei2c.so", EntryPoint = "i2c_smbus_read_byte_data", SetLastError = true)]
		public static extern int i2c_smbus_read_byte_data(int busHandle, byte regAddress);

		[DllImport("libnativei2c.so", EntryPoint = "i2c_smbus_write_byte_data", SetLastError = true)]
		public static extern int i2c_smbus_write_byte_data(int busHandle, byte regAddress,byte value);

		[DllImport("libnativei2c.so", EntryPoint = "i2c_smbus_read_block_data", SetLastError = true)]
		public static extern int i2c_smbus_read_block_data(int busHandle, byte regAddress,ref byte[] value);

		[DllImport("libnativei2c.so", EntryPoint = "i2c_smbus_read_i2c_block_data", SetLastError = true)]
		public static extern int i2c_smbus_read_i2c_block_data(int busHandle, byte regAddress,byte length,byte[] value);
	}
}