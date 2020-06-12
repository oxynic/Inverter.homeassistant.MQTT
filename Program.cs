using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inverter.homeassistant.MQTT
{
    class Program
    {
        static void Main(string[] args)
        {
            // this starts MQTTnet client
            Task.Run(() => ManagedClientTest.RunAsync("config"));
            //Initialise inverter
            Inverter x = new Inverter();
            //open serial port
            x.OpenPort();

            x.PollTest(); //Test data
            //x.Poll(); //Live port
            while (true)
                Console.ReadLine();
        }
    }
}
