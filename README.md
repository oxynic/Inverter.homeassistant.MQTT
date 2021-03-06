# Inverter.homeassistant.MQTT
MPP Inverter Homeassistant integration via MQTT

This app has been derived from <a href="https://github.com/ned-kelly/docker-voltronic-homeassistant">ned-kelly/docker-voltronic-homeassistant</a> and <a href="https://github.com/manio/skymax-demo">/manio/skymax-demo</a>

Its a small windows console app that will read the inverter status via serial cable connected to COM port. It will pass the data to an MQTT broker and can then be used to dispaly data on homeassistant.

To run this app download all files from <a href="https://github.com/oxynic/Inverter.homeassistant.MQTT/tree/master/bin/Debug">Inverter.homeassistant.MQTT<b>/bin/Debug/</b></a>.

Open file Inverter.homeassistant.MQTT.exe.config and update the settings is this xml file (details below)

<b>InverterPortName</b> - update this with the COM port the inverter is connected to. e.g. COM1.<br/>
<b>MQTTClientId</b> - this is the identifier for the MQTT messages, default is 'voltronic'.<br/>
<b>MQTTServer</b> - your MQTT broker ip address.<br/>
<b>MQTTPort</b> - your MQTT broker port, default is 1883.<br/>
<b>MQTTUsername</b> - username for your MQTT broker<br/>
<b>MQTTPassword</b> - password for your MQTT broker<br/>
<b>DebugFlag</b> - Set True initally to test, if the app is runing file pleaes make sure to change it to False to improve performance.<br/>
<b>ScanDelay</b> - the delay between each ping to your inveter. 2000 (2 sec) works ok for me.<br/>

Run the <b>Inverter.homeassistant.MQTT.exe</b> file to run this app.

check log file and status.html (created when you run this app and saved on the same folder). If you have any issues with this app please share content from this file.

