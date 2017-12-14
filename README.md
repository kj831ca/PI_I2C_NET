PI_I2C_NET
===========
Based on RPi.I2C.Net
Add support on  Raspberry PI for i2c_smbus_read_byte , i2c_smbus_write_byte and i2c_smbus_i2c_read_block_data
## Description
The library provides basic read/write functionality with I2C-devices for Mono v. 5.x.x
The original RPi.I2C.Net doesn't seem to work with some I2C device such as MLX90614 with need a Restart to read byte for I2C bus.. but it is working with new SMBUS commands.


## License
The project uses [MIT license](https://github.com/mshmelev/RPi.I2C.Net/blob/master/license.txt): do whatever you want wherever you want it.

