using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Opc.Ua.Client;
using Opc.Ua.Server;
using Opc.Ua.Configuration;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet.Client.Subscribing;
using MQTTnet.Server;


namespace OPCUAInterface
{
    public class Program
    {
        public static string test_hostname;

        public static void Main()
        {
            Program.test_hostname = System.Net.Dns.GetHostName();
            Console.WriteLine("Hello World!");
            Console.WriteLine(Program.test_hostname);

            Task mainTask = MainAsync();
            mainTask.Wait();

        }

        static async Task MainAsync()
        {
            await SendMQTT();
            Console.ReadLine();
        }

        private static async Task SendMQTT()
        {
            // Create a new MQTT client.
            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();
            Console.WriteLine("Starting MQTT Client");

            // Use WebSocket connection.
            Console.Write("MQTT Broker IP: ");
            string brokerIP = Console.ReadLine();
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(brokerIP)
                .Build();
            Console.WriteLine("Connecting to " + brokerIP + " via TCP");

            await mqttClient.ConnectAsync(options);
        
            // Message options
            Console.Write("Topic: ");
            string mqttTopic = Console.ReadLine();
            Console.Write("Message: ");
            string mqttMessage = Console.ReadLine();
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(mqttTopic)
                .WithPayload(mqttMessage)
                .WithExactlyOnceQoS()
                .WithRetainFlag()
                .Build();

            // Publishing
            await mqttClient.PublishAsync(message);
            Console.WriteLine("Message: " + mqttMessage + " published to Topic " + mqttTopic);
        }
    }
}
