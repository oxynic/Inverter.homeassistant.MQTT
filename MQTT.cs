using System;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Protocol;
using MQTTnet;
using System.Configuration;

//using System.Linq;


namespace Inverter.homeassistant.MQTT
{

    //below 3 class are example from
    //https://github.com/chkr1011/MQTTnet/blob/master/Tests/MQTTnet.TestApp.NetCore/ManagedClientTest.cs
    public static class ManagedClientTest
    {
        static ManagedMqttClient managedClient = (ManagedMqttClient)new MqttFactory().CreateManagedMqttClient();

        public static async Task RunAsync(string type)
        {
            var ms = new ClientRetainedMessageHandler();

            var options = new ManagedMqttClientOptions
            {
                ClientOptions = new MqttClientOptions
                {
                    ClientId = Settings.MQTT.MQTTClientId,
                    Credentials = new RandomPassword(),
                    ChannelOptions = new MqttClientTcpOptions
                    {
                        Server = Settings.MQTT.Server,
                        Port = Settings.MQTT.Port
                    }
                },

                AutoReconnectDelay = TimeSpan.FromSeconds(1),
                Storage = ms
            };

            try
            {
                managedClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(e =>
                {
                    Logging.DebugWrite("Debug", ">> RECEIVED: " + e.ApplicationMessage.Topic);
                });

                await managedClient.StartAsync(options);

                if (type == "config")
                {
                    //these are for inverter values -32
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_inverter_mode/config").WithPayload("{\"name\": \"" + Settings.MQTT.MQTTClientId + "_inverter_mode\",\"unit_of_measurement\": \"\",\"state_topic\": \"homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_inverter_mode\",\"icon\": \"mdi:solar_power\"}").WithRetainFlag(true));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_ac_grid_voltage/config").WithPayload("{\"name\": \"" + Settings.MQTT.MQTTClientId + "_ac_grid_voltage\",\"unit_of_measurement\": \"V\",\"state_topic\": \"homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_ac_grid_voltage\",\"icon\": \"mdi:power-plug\"}").WithRetainFlag(true));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_ac_grid_frequency/config").WithPayload("{\"name\": \"" + Settings.MQTT.MQTTClientId + "_ac_grid_frequency\",\"unit_of_measurement\": \"Hz\",\"state_topic\": \"homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_ac_grid_frequency\",\"icon\": \"mdi:current-ac\"}").WithRetainFlag(true));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_ac_out_voltage/config").WithPayload("{\"name\": \"" + Settings.MQTT.MQTTClientId + "_ac_out_voltage\",\"unit_of_measurement\": \"V\",\"state_topic\": \"homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_ac_out_voltage\",\"icon\": \"mdi:power-plug\"}").WithRetainFlag(true));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_ac_out_frequency/config").WithPayload("{\"name\": \"" + Settings.MQTT.MQTTClientId + "_ac_out_frequency\",\"unit_of_measurement\": \"Hz\",\"state_topic\": \"homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_ac_out_frequency\",\"icon\": \"mdi:current-ac\"}").WithRetainFlag(true));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_pv_in_voltage/config").WithPayload("{\"name\": \"" + Settings.MQTT.MQTTClientId + "_pv_in_voltage\",\"unit_of_measurement\": \"V\",\"state_topic\": \"homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_pv_in_voltage\",\"icon\": \"mdi:solar-panel-large\"}").WithRetainFlag(true));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_pv_in_current/config").WithPayload("{\"name\": \"" + Settings.MQTT.MQTTClientId + "_pv_in_current\",\"unit_of_measurement\": \"A\",\"state_topic\": \"homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_pv_in_current\",\"icon\": \"mdi:solar-panel-large\"}").WithRetainFlag(true));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_pv_in_watts/config").WithPayload("{\"name\": \"" + Settings.MQTT.MQTTClientId + "_pv_in_watts\",\"unit_of_measurement\": \"W\",\"state_topic\": \"homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_pv_in_watts\",\"icon\": \"mdi:solar-panel-large\"}").WithRetainFlag(true));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_pv_in_watthour/config").WithPayload("{\"name\": \"" + Settings.MQTT.MQTTClientId + "_pv_in_watthour\",\"unit_of_measurement\": \"Wh\",\"state_topic\": \"homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_pv_in_watthour\",\"icon\": \"mdi:solar-panel-large\"}").WithRetainFlag(true));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_scc_voltage/config").WithPayload("{\"name\": \"" + Settings.MQTT.MQTTClientId + "_scc_voltage\",\"unit_of_measurement\": \"V\",\"state_topic\": \"homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_scc_voltage\",\"icon\": \"mdi:current-dc\"}").WithRetainFlag(true));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_load_pct/config").WithPayload("{\"name\": \"" + Settings.MQTT.MQTTClientId + "_load_pct\",\"unit_of_measurement\": \"%\",\"state_topic\": \"homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_load_pct\",\"icon\": \"mdi:brightness-percent\"}").WithRetainFlag(true));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_load_watt/config").WithPayload("{\"name\": \"" + Settings.MQTT.MQTTClientId + "_load_watt\",\"unit_of_measurement\": \"W\",\"state_topic\": \"homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_load_watt\",\"icon\": \"mdi:chart-bell-curve\"}").WithRetainFlag(true));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_load_watthour/config").WithPayload("{\"name\": \"" + Settings.MQTT.MQTTClientId + "_load_watthour\",\"unit_of_measurement\": \"Wh\",\"state_topic\": \"homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_load_watthour\",\"icon\": \"mdi:chart-bell-curve\"}").WithRetainFlag(true));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_load_va/config").WithPayload("{\"name\": \"" + Settings.MQTT.MQTTClientId + "_load_va\",\"unit_of_measurement\": \"VA\",\"state_topic\": \"homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_load_va\",\"icon\": \"mdi:chart-bell-curve\"}").WithRetainFlag(true));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_bus_voltage/config").WithPayload("{\"name\": \"" + Settings.MQTT.MQTTClientId + "_bus_voltage\",\"unit_of_measurement\": \"V\",\"state_topic\": \"homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_bus_voltage\",\"icon\": \"mdi:details\"}").WithRetainFlag(true));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_heatsink_temperature/config").WithPayload("{\"name\": \"" + Settings.MQTT.MQTTClientId + "_heatsink_temperature\",\"unit_of_measurement\": \"\",\"state_topic\": \"homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_heatsink_temperature\",\"icon\": \"mdi:details\"}").WithRetainFlag(true));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_battery_capacity/config").WithPayload("{\"name\": \"" + Settings.MQTT.MQTTClientId + "_battery_capacity\",\"unit_of_measurement\": \"%\",\"state_topic\": \"homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_battery_capacity\",\"icon\": \"mdi:battery-outline\"}").WithRetainFlag(true));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_battery_voltage/config").WithPayload("{\"name\": \"" + Settings.MQTT.MQTTClientId + "_battery_voltage\",\"unit_of_measurement\": \"V\",\"state_topic\": \"homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_battery_voltage\",\"icon\": \"mdi:battery-outline\"}").WithRetainFlag(true));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_battery_charge_current/config").WithPayload("{\"name\": \"" + Settings.MQTT.MQTTClientId + "_battery_charge_current\",\"unit_of_measurement\": \"A\",\"state_topic\": \"homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_battery_charge_current\",\"icon\": \"mdi:current-dc\"}").WithRetainFlag(true));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_battery_discharge_current/config").WithPayload("{\"name\": \"" + Settings.MQTT.MQTTClientId + "_battery_discharge_current\",\"unit_of_measurement\": \"A\",\"state_topic\": \"homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_battery_discharge_current\",\"icon\": \"mdi:current-dc\"}").WithRetainFlag(true));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_load_status_on/config").WithPayload("{\"name\": \"" + Settings.MQTT.MQTTClientId + "_load_status_on\",\"unit_of_measurement\": \"\",\"state_topic\": \"homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_load_status_on\",\"icon\": \"mdi:power\"}").WithRetainFlag(true));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_scc_charge_on/config").WithPayload("{\"name\": \"" + Settings.MQTT.MQTTClientId + "_scc_charge_on\",\"unit_of_measurement\": \"\",\"state_topic\": \"homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_scc_charge_on\",\"icon\": \"mdi:power\"}").WithRetainFlag(true));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_ac_charge_on/config").WithPayload("{\"name\": \"" + Settings.MQTT.MQTTClientId + "_ac_charge_on\",\"unit_of_measurement\": \"\",\"state_topic\": \"homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_ac_charge_on\",\"icon\": \"mdi:power\"}").WithRetainFlag(true));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_battery_recharge_voltage/config").WithPayload("{\"name\": \"" + Settings.MQTT.MQTTClientId + "_battery_recharge_voltage\",\"unit_of_measurement\": \"V\",\"state_topic\": \"homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_battery_recharge_voltage\",\"icon\": \"mdi:current-dc\"}").WithRetainFlag(true));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_battery_under_voltage/config").WithPayload("{\"name\": \"" + Settings.MQTT.MQTTClientId + "_battery_under_voltage\",\"unit_of_measurement\": \"V\",\"state_topic\": \"homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_battery_under_voltage\",\"icon\": \"mdi:current-dc\"}").WithRetainFlag(true));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_battery_bulk_voltage/config").WithPayload("{\"name\": \"" + Settings.MQTT.MQTTClientId + "_battery_bulk_voltage\",\"unit_of_measurement\": \"V\",\"state_topic\": \"homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_battery_bulk_voltage\",\"icon\": \"mdi:current-dc\"}").WithRetainFlag(true));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_battery_float_voltage/config").WithPayload("{\"name\": \"" + Settings.MQTT.MQTTClientId + "_battery_float_voltage\",\"unit_of_measurement\": \"V\",\"state_topic\": \"homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_battery_float_voltage\",\"icon\": \"mdi:current-dc\"}").WithRetainFlag(true));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_max_grid_charge_current/config").WithPayload("{\"name\": \"" + Settings.MQTT.MQTTClientId + "_max_grid_charge_current\",\"unit_of_measurement\": \"A\",\"state_topic\": \"homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_max_grid_charge_current\",\"icon\": \"mdi:current-ac\"}").WithRetainFlag(true));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_max_charge_current/config").WithPayload("{\"name\": \"" + Settings.MQTT.MQTTClientId + "_max_charge_current\",\"unit_of_measurement\": \"A\",\"state_topic\": \"homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_max_charge_current\",\"icon\": \"mdi:current-ac\"}").WithRetainFlag(true));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_out_source_priority/config").WithPayload("{\"name\": \"" + Settings.MQTT.MQTTClientId + "_out_source_priority\",\"unit_of_measurement\": \"\",\"state_topic\": \"homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_out_source_priority\",\"icon\": \"mdi:grid\"}").WithRetainFlag(true));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_charger_source_priority/config").WithPayload("{\"name\": \"" + Settings.MQTT.MQTTClientId + "_charger_source_priority\",\"unit_of_measurement\": \"\",\"state_topic\": \"homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_charger_source_priority\",\"icon\": \"mdi:solar-power\"}").WithRetainFlag(true));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_battery_redischarge_voltage/config").WithPayload("{\"name\": \"" + Settings.MQTT.MQTTClientId + "_battery_redischarge_voltage\",\"unit_of_measurement\": \"V\",\"state_topic\": \"homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_battery_redischarge_voltage\",\"icon\": \"mdi:battery-negative\"}").WithRetainFlag(true));
                    //this is for raw command
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/voltronic/config").WithPayload("{\"name\": \"voltronic\",\"state_topic\": \"homeassistant/sensor/voltronic\"}").WithRetainFlag(true));
                    //await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_$1/config").WithPayload("{\"name\": \"" + Settings.MQTT.MQTTClientId + "_$1\",\"unit_of_measurement\": \"$2\",\"state_topic\": \"homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_$1\",\"icon\": \"mdi:s$3\"}"));
                    //await managedClient.PublishAsync(builder => builder.WithTopic("PV_in_voltage").WithPayload("300").WithAtLeastOnceQoS());

                    //subscribe to raw command
                    await managedClient.SubscribeAsync(new MqttTopicFilter { Topic = "homeassistant/sensor/voltronic", QualityOfServiceLevel = MqttQualityOfServiceLevel.AtMostOnce });
                    //await managedClient.SubscribeAsync(new MqttTopicFilter { Topic = "abc", QualityOfServiceLevel = MqttQualityOfServiceLevel.AtMostOnce });

                    //await managedClient.PublishAsync(builder => builder.WithTopic("PV_in_current").WithPayload("3"));
                }


                Logging.DebugWrite("Debug", "Managed client started.");
                //Console.ReadLine();
            }
            catch (Exception e)
            {
                Logging.DebugWrite("Error", e.ToString());
            }
        }

        public static async Task RunAsync1(string type)
        {
            try
            {
                if (managedClient.IsConnected)
                {
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_inverter_mode").WithPayload(InverterData.Mode));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_ac_grid_voltage").WithPayload(InverterData.QPIGS.voltage_grid.ToString()));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_ac_grid_frequency").WithPayload(InverterData.QPIGS.freq_grid.ToString()));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_ac_out_voltage").WithPayload(InverterData.QPIGS.voltage_out.ToString()));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_ac_out_frequency").WithPayload(InverterData.QPIGS.freq_out.ToString()));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_pv_in_voltage").WithPayload(InverterData.QPIGS.pv_input_voltage.ToString()));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_pv_in_current").WithPayload(InverterData.QPIGS.pv_input_current.ToString()));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_pv_in_watts").WithPayload(InverterData.QPIGS.pv_input_watts.ToString()));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_pv_in_watthour").WithPayload(InverterData.QPIGS.pv_input_watthour.ToString()));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_scc_voltage").WithPayload(InverterData.QPIGS.scc_voltage.ToString()));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_load_pct").WithPayload(InverterData.QPIGS.load_percent.ToString()));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_load_watt").WithPayload(InverterData.QPIGS.load_watt.ToString()));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_load_watthour").WithPayload(InverterData.QPIGS.load_watthour.ToString()));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_load_va").WithPayload(InverterData.QPIGS.load_va.ToString()));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_bus_voltage").WithPayload(InverterData.QPIGS.voltage_bus.ToString()));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_heatsink_temperature").WithPayload(InverterData.QPIGS.temp_heatsink.ToString()));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_battery_capacity").WithPayload(InverterData.QPIGS.batt_capacity.ToString()));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_battery_voltage").WithPayload(InverterData.QPIGS.voltage_batt.ToString()));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_battery_charge_current").WithPayload(InverterData.QPIGS.batt_charge_current.ToString()));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_battery_discharge_current").WithPayload(InverterData.QPIGS.batt_discharge_current.ToString()));
                    try
                    {
                        await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_load_status_on").WithPayload(InverterData.QPIGS.device_status.Substring(3, 1))); // need to fix
                        await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_scc_charge_on").WithPayload(InverterData.QPIGS.device_status.Substring(6, 1))); // need to fix
                        await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_ac_charge_on").WithPayload(InverterData.QPIGS.device_status.Substring(7, 1))); // need to fix
                    }
                    catch
                    {
                        await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_load_status_on").WithPayload("0")); // need to fix
                        await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_scc_charge_on").WithPayload("0")); // need to fix
                        await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_ac_charge_on").WithPayload("0")); // need to fix
                    }
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_battery_recharge_voltage").WithPayload(InverterData.QPIRI.batt_recharge_voltage.ToString()));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_battery_under_voltage").WithPayload(InverterData.QPIRI.batt_under_voltage.ToString()));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_battery_bulk_voltage").WithPayload(InverterData.QPIRI.batt_bulk_voltage.ToString()));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_battery_float_voltage").WithPayload(InverterData.QPIRI.batt_float_voltage.ToString()));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_max_grid_charge_current").WithPayload(InverterData.QPIRI.max_grid_charge_current.ToString()));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_max_charge_current").WithPayload(InverterData.QPIRI.max_charge_current.ToString()));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_out_source_priority").WithPayload(InverterData.QPIRI.out_source_priority.ToString()));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_charger_source_priority").WithPayload(InverterData.QPIRI.charger_source_priority.ToString()));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_battery_redischarge_voltage").WithPayload(InverterData.QPIRI.batt_redischarge_voltage.ToString()));
                    await managedClient.PublishAsync(builder => builder.WithTopic("homeassistant/sensor/" + Settings.MQTT.MQTTClientId + "_warnings").WithPayload("NA"));//need to fix
                }
                else
                {
                    Task.Run(() => ManagedClientTest.RunAsync("config"));
                }
            }
            catch (Exception e)
            {
                Logging.DebugWrite("Error", e.ToString());
            }
        }

        public class RandomPassword : IMqttClientCredentials
        {
            public byte[] Password => System.Text.Encoding.ASCII.GetBytes(Settings.MQTT.Password);
            public string Username => Settings.MQTT.Username;
        }

        public class ClientRetainedMessageHandler : IManagedMqttClientStorage
        {
            private const string Filename = @"RetainedMessages.json";

            public Task SaveQueuedMessagesAsync(IList<ManagedMqttApplicationMessage> messages)
            {
                File.WriteAllText(Filename, JsonConvert.SerializeObject(messages));
                return Task.FromResult(0);
            }

            public Task<IList<ManagedMqttApplicationMessage>> LoadQueuedMessagesAsync()
            {
                IList<ManagedMqttApplicationMessage> retainedMessages;
                if (File.Exists(Filename))
                {
                    var json = File.ReadAllText(Filename);
                    retainedMessages = JsonConvert.DeserializeObject<List<ManagedMqttApplicationMessage>>(json);
                }
                else
                {
                    retainedMessages = new List<ManagedMqttApplicationMessage>();
                }

                return Task.FromResult(retainedMessages);
            }
        }
    }


}
