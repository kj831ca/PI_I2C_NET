#ifndef _rpi_i2c_c
#define _rpi_i2c_c
#include <linux/types.h>

#ifdef __cplusplus
extern "C" {
#endif

extern int openBus (char*);
extern int closeBus (int);
extern int writeBytes (int, int, __u8*, int);
extern int readBytes (int, int, __u8*, int);

extern int set_slave_address(int, int, int);
extern int i2c_smbus_write_byte(int, __u8);
extern int i2c_smbus_read_byte(int file);
extern int i2c_smbus_read_word_data(int, __u8);
extern int i2c_smbus_read_byte_data(int file, __u8 regAddr);
extern int i2c_smbus_write_byte_data(int file, __u8 regAddr, __u8 value);
extern int i2c_smbus_read_block_data(int file, __u8 regAddr, __u8 *value);
extern int i2c_smbus_read_i2c_block_data(int file, __u8 regAddr, __u8 length,__u8 *value);

#ifdef __cplusplus
}
#endif

#endif
