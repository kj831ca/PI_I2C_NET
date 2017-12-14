#include <stdio.h>
#include <stddef.h>
#include <linux/i2c-dev.h>
#include <linux/i2c.h>
#include <fcntl.h>
#include <string.h>
#include <sys/ioctl.h>
#include <unistd.h>
#include <errno.h>

#include "libNativeI2C.h"



int openBus (char* busFileName)
{
	return open (busFileName, O_RDWR);
}

int closeBus (int busHandle)
{
	return close (busHandle);
}

int writeBytes (int busHandle, int addr, __u8* buf, int len)
{
	if (ioctl (busHandle, I2C_SLAVE, addr) < 0)
		return -1;

	if (write (busHandle, buf, len) != len)
		return -2;

	return len;
}
int readBytes (int busHandle, int addr, __u8* buf, int len)
{
	if (ioctl (busHandle, I2C_SLAVE, addr) < 0)
		return -1;

	int n= read (busHandle, buf, len);

	return n;
}

int set_slave_address(int busHandle,int addr, int force)
{
	/* With force, let the user read from/write to the registers
	   even when a driver is also running */
	if (ioctl(busHandle, force ? I2C_SLAVE_FORCE : I2C_SLAVE, addr) < 0) 
	{
		fprintf(stderr,
			"Error: Could not set address to 0x%02x: %s\n",
			addr, strerror(errno));
		return -errno;
	}

	return 0;
}

int i2c_smbus_access(int file, char read_write, __u8 command,
		       int size, union i2c_smbus_data *data)
{
	struct i2c_smbus_ioctl_data args;
	int err;

	args.read_write = read_write;
	args.command = command;
	args.size = size;
	args.data = data;

	err = ioctl(file, I2C_SMBUS, &args);
	if (err == -1)
		err = -errno;
	return err;
}
/*
 * DO single write operation.. 
 */
int i2c_smbus_write_byte(int file,__u8 value)
{
	return i2c_smbus_access(file,I2C_SMBUS_WRITE, value,
	I2C_SMBUS_BYTE, NULL);
}
/*
 * Do a single read operation from i2c
 */
int i2c_smbus_read_byte(int file)
{
	union i2c_smbus_data data;
	int err;
	
	err = i2c_smbus_access(file,I2C_SMBUS_READ, 0, I2C_SMBUS_BYTE, &data);
	if(err < 0)
	 return err;
	
	return 0x0FF & data.byte;
}

int i2c_smbus_read_byte_data(int file, __u8 regAddr)
{
	union i2c_smbus_data data;
	int err;
	
	err = i2c_smbus_access(file,I2C_SMBUS_READ, regAddr, I2C_SMBUS_BYTE_DATA, &data);
	if(err < 0)
	 return err;
	
	return 0x0FF & data.byte;
	
}

int i2c_smbus_write_byte_data(int file, __u8 regAddr, __u8 value)
{
	union i2c_smbus_data data;
	data.byte = value;
	
	return i2c_smbus_access(file,I2C_SMBUS_WRITE, regAddr, I2C_SMBUS_BYTE_DATA, &data);
}

int i2c_smbus_write_word_data(int file, __u8 command,__u16 value)
{
	union i2c_smbus_data data;
	data.word = value;
	return i2c_smbus_access(file, I2C_SMBUS_WRITE, command,I2C_SMBUS_WORD_DATA, &data);
}



int i2c_smbus_read_word_data(int file, __u8 command)
{
	union i2c_smbus_data data;
	int err;

	err = i2c_smbus_access(file, I2C_SMBUS_READ, command,
			       I2C_SMBUS_WORD_DATA, &data);
	if (err < 0)
		return err;

	return 0x0FFFF & data.word;
}

int i2c_smbus_read_block_data(int file, __u8 regAddr, __u8 *value)
{
	union i2c_smbus_data data;
	int i, err;
	
	err = i2c_smbus_access(file, I2C_SMBUS_READ, regAddr, I2C_SMBUS_BLOCK_DATA, &data);
	
	if(err<0)
		return err;
	
	for(i=1;i<=data.block[0];i++)
		value[i-1]=data.block[i];
	return data.block[0];
}

int i2c_smbus_read_i2c_block_data(int file, __u8 regAddr, __u8 length,__u8 *value)
{
	union i2c_smbus_data data;
	int i, err;
	
	if(length > I2C_SMBUS_BLOCK_MAX)
		length = I2C_SMBUS_BLOCK_MAX;
	data.block[0]=length;
	
	err = i2c_smbus_access(file, I2C_SMBUS_READ, regAddr, 
			length == 32 ? I2C_SMBUS_I2C_BLOCK_BROKEN:
			I2C_SMBUS_I2C_BLOCK_DATA, &data);
	
	if(err<0)
		return err;
	
	for(i=1;i<=data.block[0];i++)
		value[i-1]=data.block[i];
	return data.block[0];
}

int main(int argc, char *argv[])
{
	printf("Hello world");
}
