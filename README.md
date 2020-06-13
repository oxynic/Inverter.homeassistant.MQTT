# Inverter.homeassistant.MQTT
MPP Inverter Homeassistant integration via MQTT

To run this app go to Inverter.homeassistant.MQTT/bin/Debug/ and download all the files.

open file Inverter.homeassistant.MQTT.exe.config and update the settings is this xml file (details below)

InverterPortName - update this with the COM port the inverter is connected to. e.g. COM1.<br/>
MQTTClientId - this is the identifier for the MQTT messages, default is 'voltronic'.<br/>
MQTTServer - your MQTT broker ip address.<br/>
MQTTPort - your MQTT broker port, default is 1883.<br/>
MQTTUsername - username for your MQTT broker<br/>
MQTTPassword - password for your MQTT broker<br/>
DebugFlag - Set True initally to test, if the app is runing file pleaes make sure to change it to False to improve performance.<br/>
ScanDelay - the delay between each ping to your inveter. 2000 (2 sec) works ok for me.<br/>


