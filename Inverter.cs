using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inverter.homeassistant.MQTT
{

    public static class Settings
    {
        // App settings
        public static bool isDebug = Convert.ToBoolean(ConfigurationManager.AppSettings["DebugFlag"].ToString());
        public static int scanTime = Convert.ToInt32(ConfigurationManager.AppSettings["ScanDelay"].ToString());

        public static class Inverter
        {
            // Inverter Settings
            public static Parity parity = (Parity)Enum.Parse(typeof(Parity), ConfigurationManager.AppSettings["InverterParity"].ToString());
            public static StopBits stopBits = (StopBits)Enum.Parse(typeof(StopBits), ConfigurationManager.AppSettings["InverterStopBits"].ToString());
            public static int dataBits = Convert.ToInt32(ConfigurationManager.AppSettings["InverterDataBits"].ToString());
            public static int baudRate = Convert.ToInt32(ConfigurationManager.AppSettings["InverterBaudRate"].ToString());
            public static string[] systemPorts;
            public static string portName = ConfigurationManager.AppSettings["InverterPortName"].ToString();
            public static bool isDTR;
            public static bool isRTS;
        }

        public static class MQTT
        {
            //MQTT Settings
            public static string Server = ConfigurationManager.AppSettings["MQTTServer"].ToString();
            public static int Port = Convert.ToInt32(ConfigurationManager.AppSettings["MQTTPort"].ToString());
            public static string Username = ConfigurationManager.AppSettings["MQTTUsername"].ToString();
            public static string Password = ConfigurationManager.AppSettings["MQTTPassword"].ToString();
            public static string MQTTClientId = ConfigurationManager.AppSettings["MQTTClientId"].ToString();
        }
    }

    public static class InverterData
    {

        public static bool isConnected = false;

        public static string Warning;
        public static string Mode;

        public static class Read {
            public static byte[] data;
            public static string command = "";
            public static DateTime time;
            public static int sequence = 0;
        }

        public static class QPIGS
        {
            public static string raw;
            public static float voltage_grid;
            public static float freq_grid;
            public static float voltage_out;
            public static float freq_out;
            public static int load_va;
            public static int load_watt;
            public static int load_percent;
            public static int voltage_bus;
            public static float voltage_batt;
            public static int batt_charge_current;
            public static int batt_capacity;
            public static int temp_heatsink;
            public static float pv_input_current;
            public static float pv_input_voltage;
            public static float pv_input_watts;
            public static float pv_input_watthour;
            public static float load_watthour = 0;
            public static float scc_voltage;
            public static int batt_discharge_current;
            public static string device_status; // length=9
            public static string v20;
        }

        public static class QPIRI
        {
            public static string raw;
            public static float grid_voltage_rating;
            public static float grid_current_rating;
            public static float out_voltage_rating;
            public static float out_freq_rating;
            public static float out_current_rating;
            public static int out_va_rating;
            public static int out_watt_rating;
            public static float batt_rating;
            public static float batt_recharge_voltage;
            public static float batt_under_voltage;
            public static float batt_bulk_voltage;
            public static float batt_float_voltage;
            public static int batt_type;
            public static int max_grid_charge_current;
            public static int max_charge_current;
            public static int in_voltage_range;
            public static int out_source_priority;
            public static int charger_source_priority;
            public static int machine_type;
            public static int topology;
            public static int out_mode;
            public static float batt_redischarge_voltage;
            public static float v22;
            public static float v23;
            public static float v24;
        }
    }

    public class Inverter
    {

        public Inverter()
        {
        }



        SerialPort comport1 = new SerialPort();


        public void OpenPort()
        {
            try
            {

                comport1.DataReceived += new SerialDataReceivedEventHandler(ReadData);
                bool error = false;

                // If the port is open, close it.
                if (comport1.IsOpen) comport1.Close();
                else
                {
                    // Set the port's settings
                    comport1.BaudRate = Settings.Inverter.baudRate;
                    comport1.DataBits = Settings.Inverter.dataBits;
                    comport1.StopBits = Settings.Inverter.stopBits;
                    comport1.Parity = Settings.Inverter.parity;
                    comport1.PortName = Settings.Inverter.portName;

                    try
                    {
                        // Open the port
                        comport1.Open();
                    }
                    catch (UnauthorizedAccessException) { error = true; }
                    catch (IOException) { error = true; }
                    catch (ArgumentException) { error = true; }

                    if (error) Logging.DebugWrite("Error","COM Port Unavalible - Could not open the COM port.  Most likely it is already in use, has been removed, or is unavailable.");
                    else
                    {
                        Settings.Inverter.isDTR = comport1.DtrEnable;
                        Settings.Inverter.isRTS = comport1.RtsEnable;
                    }
                }

                // If the port is open, send focus to the send data box
                if (comport1.IsOpen)
                {
                    // send data here
                    InverterData.isConnected = true;
                }
            }
            catch (Exception ex)
            {
                Logging.DebugWrite("Error", ex.StackTrace);
            }

        }

        private void SendCommand(string command)
        {
            try
            {
                if (InverterData.isConnected)
                {
                    // Convert the user's string of hex digits (ex: B4 CA E2) to a byte array
                    byte[] data = HexStringToByteArray(HexCommand(command));

                    // Send the binary data out the port
                    comport1.Write(data, 0, data.Length);

                    Logging.DebugWrite("Debug","Write - " + ByteArrayToHexString(data));

                }
            }
            catch (FormatException)
            {
                // Inform the user if the hex string was not properly formatted
                Logging.DebugWrite("Error","Error - Not properly formatted hex string: " + command);
            }

        }

       private void ReadData(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                // If the com port has been closed, do nothing
                if (!comport1.IsOpen) return;

                // Obtain the number of bytes waiting in the port's buffer
                int bytes = comport1.BytesToRead;

                // Create a byte array buffer to hold the incoming data
                byte[] buffer = new byte[bytes];

                // Read the data from the port and store it in our buffer
                comport1.Read(buffer, 0, bytes);

                // Show the user the incoming data in hex format
                Logging.DebugWrite("Debug","HRead - " + ByteArrayToHexString(buffer));

                if (InverterData.Read.data.Length > 0)
                    InverterData.Read.data = Combine(InverterData.Read.data, buffer);
                else
                    InverterData.Read.data = buffer;

                byte b = (byte)Convert.ToByte("0d", 16);
                if (Array.IndexOf(buffer, b) >= 0)
                {
                    ProcessRead();
                }

            }
            catch(Exception ex)
            {
                Logging.DebugWrite("Error",ex.StackTrace);
            }

        }

        public void ProcessRead()
        {
            try
            {

                byte[] data = InverterData.Read.data;

                StringBuilder sb = new StringBuilder(data.Length * 2);
                foreach (byte b in data)
                    sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
                string hexString = sb.ToString();

                string ascii = string.Empty;
                for (int i = 0; i < hexString.Length; i += 2)
                {
                    String hs = string.Empty;
                    hs = hexString.Substring(i, 2);
                    uint decval = System.Convert.ToUInt32(hs, 16);
                    char character = System.Convert.ToChar(decval);
                    ascii += character;
                }

                PopulateData(ascii);

                if (InverterData.Read.command == "QPIWS")
                {
                    Task.Run(() => ManagedClientTest.RunAsync1("values"));
                }

                InverterData.Read.command = "";
                InverterData.Read.data = new byte[0];

                Logging.DebugWrite("Debug", "CRead - " + ascii);

            }
            
            
            catch (Exception ex)
            {
                Logging.DebugWrite("Error", ex.StackTrace);
            }
        }

        public void PopulateData(string data)
        {
            data = data.Substring(1, data.Length - 3); // removing the first and last character.
            if (InverterData.Read.command == "QMOD")
            {
                InverterData.Mode = data.Substring(0,1);
            }
            if (InverterData.Read.command == "QPIWS")
            {
                InverterData.Warning = data;
            }
            else if (InverterData.Read.command == "QPIRI")
            {
                //data.length = 94
                //value.length = 25
                string[] values = data.Split(' ');
                InverterData.QPIRI.raw = data;
                InverterData.QPIRI.grid_voltage_rating = Convert.ToSingle(values[0]);
                InverterData.QPIRI.grid_current_rating = Convert.ToSingle(values[1]);
                InverterData.QPIRI.out_voltage_rating = Convert.ToSingle(values[2]);
                InverterData.QPIRI.out_freq_rating = Convert.ToSingle(values[3]);
                InverterData.QPIRI.out_current_rating = Convert.ToSingle(values[4]);
                InverterData.QPIRI.out_va_rating = Convert.ToInt32(values[5]);
                InverterData.QPIRI.out_watt_rating = Convert.ToInt32(values[6]);
                InverterData.QPIRI.batt_rating = Convert.ToSingle(values[7]);
                InverterData.QPIRI.batt_recharge_voltage = Convert.ToSingle(values[8]);
                InverterData.QPIRI.batt_under_voltage = Convert.ToSingle(values[9]);
                InverterData.QPIRI.batt_bulk_voltage = Convert.ToSingle(values[10]);
                InverterData.QPIRI.batt_float_voltage = Convert.ToSingle(values[11]);
                InverterData.QPIRI.batt_type = Convert.ToInt32(values[12]);
                InverterData.QPIRI.max_grid_charge_current = Convert.ToInt32(values[13]);
                InverterData.QPIRI.max_charge_current = Convert.ToInt32(values[14]);
                InverterData.QPIRI.in_voltage_range = Convert.ToInt32(values[15]);
                InverterData.QPIRI.out_source_priority = Convert.ToInt32(values[16]);
                InverterData.QPIRI.charger_source_priority = Convert.ToInt32(values[17]);
                InverterData.QPIRI.machine_type = Convert.ToInt32(values[19]);
                InverterData.QPIRI.topology = Convert.ToInt32(values[20]);
                InverterData.QPIRI.out_mode = Convert.ToInt32(values[21]);
                InverterData.QPIRI.batt_redischarge_voltage = Convert.ToSingle(values[22]);
                InverterData.QPIRI.v22 = Convert.ToSingle(values[23]);
                //InverterData.QPIRI.v23 = Convert.ToSingle(values[24]);
                //InverterData.QPIRI.v24 = Convert.ToSingle(values[24]);
            }
            else if (InverterData.Read.command == "QPIGS")
            {
                // data.Length = 107;
                // value.Length = 21;
                string[] values = data.Split(' ');
                InverterData.QPIGS.raw = data;
                InverterData.QPIGS.voltage_grid = Convert.ToSingle(values[0]);
                InverterData.QPIGS.freq_grid = Convert.ToSingle(values[1]);
                InverterData.QPIGS.voltage_out = Convert.ToSingle(values[2]);
                InverterData.QPIGS.freq_out = Convert.ToSingle(values[3]);
                InverterData.QPIGS.load_va = Convert.ToInt32(values[4]);
                InverterData.QPIGS.load_watt = Convert.ToInt32(values[5]);
                InverterData.QPIGS.load_percent = Convert.ToInt32(values[6]);
                InverterData.QPIGS.voltage_bus = Convert.ToInt32(values[7]);
                InverterData.QPIGS.voltage_batt = Convert.ToSingle(values[8]);
                InverterData.QPIGS.batt_charge_current = Convert.ToInt32(values[9]);
                InverterData.QPIGS.batt_capacity = Convert.ToInt32(values[10]);
                InverterData.QPIGS.temp_heatsink = Convert.ToInt32(values[11]);
                InverterData.QPIGS.pv_input_current = Convert.ToSingle(values[12]);
                InverterData.QPIGS.pv_input_voltage = Convert.ToSingle(values[13]);
                InverterData.QPIGS.scc_voltage = Convert.ToSingle(values[14]);
                InverterData.QPIGS.batt_discharge_current = Convert.ToInt32(values[15]);
                InverterData.QPIGS.device_status = values[16]; //load_watthour 
                //InverterData.QPIGS.scc_voltage = Convert.ToSingle(values[17]);
                //InverterData.QPIGS.pv_input_watthour = Convert.ToSingle(values[18]);
                //InverterData.QPIGS.device_status = values[19];
                //InverterData.QPIGS.v20 = values[20];

                // Calculate watt-hours generated per run interval period (given as program argument)
                try
                {
                    InverterData.QPIGS.pv_input_watts = InverterData.QPIGS.pv_input_current * InverterData.QPIGS.pv_input_voltage;
                }
                catch
                    {
                    InverterData.QPIGS.pv_input_watts = 0;
                }
                //InverterData.QPIGS.pv_input_watthour = InverterData.QPIGS.pv_input_watts / (3600 / runinterval);
                //InverterData.QPIGS.load_watthour = (float)InverterData.QPIGS.load_watt / (3600 / runinterval);
                PrintData();
            }
        }

        private void PrintData()
        {

            StringBuilder sb = new StringBuilder("");
            sb.Append("<html><body><b>" + DateTime.Now.ToString() + "</b><br/><table>");
            sb.Append("<tr><td><b>QMOD</b>=" + InverterData.Mode + "</td><td><b>QPIWS</b>=" + InverterData.Warning + "</td></tr>");
            sb.Append("<tr><td><b>QPIRI</b></br>");
            sb.Append("" + InverterData.QPIRI.raw + "</br>");
            sb.Append("grid_voltage_rating=" + InverterData.QPIRI.grid_voltage_rating + "</br>");
            sb.Append("grid_current_rating=" + InverterData.QPIRI.grid_current_rating + "</br>");
            sb.Append("out_voltage_rating=" + InverterData.QPIRI.out_voltage_rating + "</br>");
            sb.Append("out_freq_rating=" + InverterData.QPIRI.out_freq_rating + "</br>");
            sb.Append("out_current_rating=" + InverterData.QPIRI.out_current_rating + "</br>");
            sb.Append("out_va_rating=" + InverterData.QPIRI.out_va_rating + "</br>");
            sb.Append("out_watt_rating=" + InverterData.QPIRI.out_watt_rating + "</br>");
            sb.Append("batt_rating=" + InverterData.QPIRI.batt_rating + "</br>");
            sb.Append("batt_recharge_voltage=" + InverterData.QPIRI.batt_recharge_voltage + "</br>");
            sb.Append("batt_under_voltage=" + InverterData.QPIRI.batt_under_voltage + "</br>");
            sb.Append("batt_bulk_voltage=" + InverterData.QPIRI.batt_bulk_voltage + "</br>");
            sb.Append("batt_float_voltage=" + InverterData.QPIRI.batt_float_voltage + "</br>");
            sb.Append("batt_type=" + InverterData.QPIRI.batt_type + "</br>");
            sb.Append("max_grid_charge_current=" + InverterData.QPIRI.max_grid_charge_current + "</br>");
            sb.Append("max_charge_current=" + InverterData.QPIRI.max_charge_current + "</br>");
            sb.Append("in_voltage_range=" + InverterData.QPIRI.in_voltage_range + "</br>");
            sb.Append("out_source_priority=" + InverterData.QPIRI.out_source_priority + "</br>");
            sb.Append("charger_source_priority=" + InverterData.QPIRI.charger_source_priority + "</br>");
            sb.Append("machine_type=" + InverterData.QPIRI.machine_type + "</br>");
            sb.Append("topology=" + InverterData.QPIRI.topology + "</br>");
            sb.Append("out_mode=" + InverterData.QPIRI.out_mode + "</br>");
            sb.Append("batt_redischarge_voltage=" + InverterData.QPIRI.batt_redischarge_voltage + "</br>");
            sb.Append("v22=" + InverterData.QPIRI.v22 + "</br>");
            sb.Append("v23=" + InverterData.QPIRI.v23 + "</br></td>");
            sb.Append("</td><td valign=\"top\"><b>QPIGS</b></br>");
            sb.Append("" + InverterData.QPIGS.raw + "</br>");
            sb.Append("voltage_grid=" + InverterData.QPIGS.voltage_grid + "</br>");
            sb.Append("freq_grid=" + InverterData.QPIGS.freq_grid + "</br>");
            sb.Append("voltage_out=" + InverterData.QPIGS.voltage_out + "</br>");
            sb.Append("freq_out=" + InverterData.QPIGS.freq_out + "</br>");
            sb.Append("load_va=" + InverterData.QPIGS.load_va + "</br>");
            sb.Append("load_watt=" + InverterData.QPIGS.load_watt + "</br>");
            sb.Append("load_percent=" + InverterData.QPIGS.load_percent + "</br>");
            sb.Append("voltage_bus=" + InverterData.QPIGS.voltage_bus + "</br>");
            sb.Append("voltage_batt=" + InverterData.QPIGS.voltage_batt + "</br>");
            sb.Append("batt_charge_current=" + InverterData.QPIGS.batt_charge_current + "</br>");
            sb.Append("batt_capacity=" + InverterData.QPIGS.batt_capacity + "</br>");
            sb.Append("temp_heatsink=" + InverterData.QPIGS.temp_heatsink + "</br>");
            sb.Append("pv_input_current=" + InverterData.QPIGS.pv_input_current + "</br>");
            sb.Append("pv_input_voltage=" + InverterData.QPIGS.pv_input_voltage + "</br>");
            sb.Append("pv_input_watts=" + InverterData.QPIGS.pv_input_watts + "</br>");
            sb.Append("pv_input_watthour=" + InverterData.QPIGS.pv_input_watthour + "</br>");
            sb.Append("load_watthour=" + InverterData.QPIGS.load_watthour + "</br>");
            sb.Append("scc_voltage=" + InverterData.QPIGS.scc_voltage + "</br>");
            sb.Append("batt_discharge_current=" + InverterData.QPIGS.batt_discharge_current + "</br>");
            sb.Append("device_status=" + InverterData.QPIGS.device_status + "</br></td></tr>");
            sb.Append("</table></body></html>");

            Logging.WriteStatus(sb.ToString());

            /*
            ft.wb.Invoke(new EventHandler(delegate
            {
                ft.wb.Navigate("about:blank");
                ft.wb.Document.OpenNew(false);
                ft.wb.Document.Write(sb.ToString());
                ft.wb.Refresh();
            }));*/
        }

        public async Task PollTest()
        {
            while (true)
            {
                InverterData.Read.command = "QMOD";
                PopulateData("(BçÉ");
                InverterData.Read.command = "QPIRI";
                PopulateData("(230.0 21.7 230.0 50.0 21.7 5000 5000 48.0 51.0 48.0 55.0 54.4 2 50 050 0 2 3 1 01 0 0 52.0 0 1 ");
                //                x.PopulateData("(230.0 21.7 230.0 50.0 21.7 5000 5000 48.0 51.0 48.0 55.0 54.4 2 50 050 0 2 3 1 01 0 0 52.0 0 1?");
                InverterData.Read.command = "QPIGS";
                PopulateData("(244.8 50.0 229.8 49.9 0824 0824 016 355 51.70 000 035 0020 00.0 000.0 00.00 00019 00010000 00 00 00000 010<");
                //x.PopulateData("(240.8 49.9 230.2 50.0 0368 0289 007 394 52.40 000 042 0038 00.7 364.3 00.00 00000 00010110 00 00 00290 010ª4");
                InverterData.Read.command = "QPIWS";
                PopulateData("(000000000000000000000000000000000000<?");
                Task.Run(() => ManagedClientTest.RunAsync1("values"));
                await Task.Delay(10000);
            }
        }
    

    public async Task Poll()
        {
            await Task.Run(async () => //Task.Run automatically unwraps nested Task types!
            {
                try
                {
                    if (InverterData.isConnected)
                    {

                        while (comport1.IsOpen)
                        {
                            if (InverterData.Read.command == "")
                            {
                                if (InverterData.Read.sequence == 0)
                                {
                                    InverterData.Read.command = "QMOD";
                                    InverterData.Read.sequence = 1;
                                }
                                else if (InverterData.Read.sequence == 1)
                                {
                                    InverterData.Read.command = "QPIGS";
                                    InverterData.Read.sequence = 2;
                                }
                                else if (InverterData.Read.sequence == 2)
                                {
                                    InverterData.Read.command = "QPIRI";
                                    InverterData.Read.sequence = 3;
                                }
                                else if (InverterData.Read.sequence == 3)
                                {
                                    InverterData.Read.command = "QPIWS";
                                    InverterData.Read.sequence = 0;
                                }
                                InverterData.Read.time = DateTime.Now;
                                InverterData.Read.data = new byte[0];
                                SendCommand(InverterData.Read.command);
                                await Task.Delay(Settings.scanTime);
                            }
                            else 
                            {
                                await Task.Delay(1000);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logging.DebugWrite("Error",ex.StackTrace);
                }
            });
        }
        public string HexCommand(string command)
        {
            return ByteArrayToHexString(System.Text.Encoding.ASCII.GetBytes(command)) + CalculateCRC(command);
        }

        private string CalculateCRC(string str)
        {

            ushort[] crc_table =
            {
                0x0000, 0x1021, 0x2042, 0x3063, 0x4084, 0x50a5, 0x60c6, 0x70e7,
                0x8108, 0x9129, 0xa14a, 0xb16b, 0xc18c, 0xd1ad, 0xe1ce, 0xf1ef
            };

            int i;
            ushort crc = 0;
            byte[] data = System.Text.Encoding.ASCII.GetBytes(str);
            int len = data.Length;

            for (int j = 0; j < len; j++)
            {
                i = (crc >> 12) ^ (data[j] >> 4);
                crc = (ushort)(crc_table[i & 0x0F] ^ (crc << 4));
                i = (crc >> 12) ^ (data[j] >> 0);
                crc = (ushort)(crc_table[i & 0x0F] ^ (crc << 4));
            }
            data = BitConverter.GetBytes(crc);
            byte bTemp = data[0];
            data[0] = data[1];
            data[1] = bTemp;
            return ByteArrayToHexString(data) + "0D";
        }

        private string ByteArrayToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0').PadRight(3, ' '));
            return sb.ToString().ToUpper();
        }

        private byte[] HexStringToByteArray(string s)
        {
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            return buffer;
        }

        public byte[] Combine(byte[] first, byte[] second)
        {
            byte[] ret = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
            return ret;
        }


    }
}
