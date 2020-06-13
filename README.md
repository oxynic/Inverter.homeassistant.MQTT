# Inverter.homeassistant.MQTT
MPP Inverter Homeassistant integration via MQTT

To run this app go to Inverter.homeassistant.MQTT/bin/Debug/ and download all the files.

open file Inverter.homeassistant.MQTT.exe.config and update the settings is this xml file (details below)

InverterPortName - update this with the COM port the inverter is connected to. e.g. COM1.
MQTTClientId - this is the identifier for the MQTT messages, default is 'voltronic'.
MQTTServer - your MQTT broker ip address.
MQTTPort - your MQTT broker port, default is 1883.
MQTTUsername - username for your MQTT broker
MQTTPassword - password for your MQTT broker
DebugFlag - Set True initally to test, if the app is runing file pleaes make sure to change it to False to improve performance.
ScanDelay - the delay between each ping to your inveter. 2000 (2 sec) works ok for me.


