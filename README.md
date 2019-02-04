# GoIP SMS Gateway Interface API (Client & Server)

This library is using the GoIP SMS Interface C#

I only have a GoIP1 device, no system testing with GoIP4 and larger devices.

## Status

This project is in very early preview stage and it's not suggested to use it in a real project.

## Documentation

  [GoIPSmsServer](GoIPSmsServer.md)
  [GoIPSmsClinet](GoIPSmsClient.md)

## Server Features

- Registration Event
- Delivery report Event
- Receive SMS Event
- Status Event
- Record Event
- Remain Evnet
- Hangup Event (Not documented)
- Expiry Event (Not documented)
- Cell list Event

## Client Features

- Send Bulk SMS
- Get GSM number
- Set GSM number
- Get expiry time of out call of a channel 
- Set expiry time of out call of a channel
- Get Remain time of out call 
- Reset remain time of out call to expiry time
- Get status of channel
- Drop call
- Reboot channel
- Reboot GoIP
- Set GSM call forward
- Send USSD
- Get IMEI
- Set IMEI
- Get out call interval
- Set out call interval
- enable/disable this module
- enable/disable all modules
- Set cell
- Get cells list
- Get current cell

# Contribution

This product is an open source platform

# LICENCE

MIT 

