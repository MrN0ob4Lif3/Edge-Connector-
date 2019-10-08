using System;
using System.Net.Mqtt;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Opc.Ua.Client;
using Opc.Ua.Server;
using Opc.Ua.Configuration;
using System.ComponentModel.DataAnnotations;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;


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

            // Create a new MQTT client.
            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();

            // Use WebSocket connection.
            var options = new MqttClientOptionsBuilder()
                .WithWebSocketServer("broker.hivemq.com:8000/mqtt")
                .Build();

            mqttClient.ConnectAsync(options, CancellationToken.None); // Since 3.0.5 with CancellationToken

            //Topics for receipt and transmission
            string rcvTopic = "eebus/daenet/command";
            string sendTopic = "eebus/daenet/telemetry";

            //Subscribes to the specified topic.
            mqttClient.SubscribeAsync(rcvTopic, MqttQualityOfService.ExactlyOnce);

            //Console to allow string messages to be typed and sent.
            Task.Run(() =>
            {
                while (true)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;

                    Console.WriteLine("Enter the text to send.");

                    Console.ForegroundColor = ConsoleColor.Cyan;

                    var line = System.Console.ReadLine();

                    var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(line));

                    mqttClient.PublishAsync(new MqttApplicationMessage(sendTopic, data), MqttQualityOfService.ExactlyOnce).Wait();

                    Console.WriteLine("Message sent");
                }
            });

            //Subscribes to MQTT Server specified
            mqttClient.MessageStream.Subscribe(msg =>
            {
                Console.ForegroundColor = ConsoleColor.Green;

                Console.WriteLine(Encoding.UTF8.GetString(msg.Payload));

                Console.ResetColor();
            });
        }

        public static async Task<IMqttClient> CreateMQTTClient(String serverAddress)
        {
            var serverIP = serverAddress;

            var configuration = new MqttConfiguration
            {
                BufferSize = 128 * 1024,
                Port = 55555,
                KeepAliveSecs = 10,
                WaitTimeoutSecs = 2,
                MaximumQualityOfService = MqttQualityOfService.AtMostOnce,
                AllowWildcardsInTopicFilters = true
            };
            var client = await MqttClient.CreateAsync(serverIP, configuration);
            Console.WriteLine("Intializing MQTT Client to " + serverIP);
            var sessionState = await client.ConnectAsync(new MqttClientCredentials("foo"), cleanSession: true);
            Console.WriteLine("Connection Established");
            return client;
        }

        public void SubscribeMQTT()
        { }
        public void PublishMQTT()
        { }
    }
}
