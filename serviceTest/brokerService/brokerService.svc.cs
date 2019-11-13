using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet.Client.Subscribing;
using MQTTnet.Server;
using MQTTnet.Extensions.ManagedClient;

namespace brokerService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class brokerService : IbrokerService
    {
        public string GetData(String value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }

        public async void CreateClientAsync()
        {
            //Create a new ManagedMQTT Client.
            var mqttFactory = new MqttFactory();
            var managedMQTT = mqttFactory.CreateManagedMqttClient();
            Console.WriteLine("Starting Managed MQTT Client");

            // Use WebSocket connection.
            Console.Write("MQTT Broker IP: ");
            string brokerIP = Console.ReadLine();
            var options = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(new MqttClientOptionsBuilder()
                    .WithClientId(System.Environment.MachineName)
                    .WithWebSocketServer(brokerIP)
                    .WithTls().Build())
                .Build();
            Console.WriteLine("Attempting connection to " + brokerIP + " via WebSocket");

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

            await managedMQTT.SubscribeAsync(new TopicFilterBuilder().WithTopic(mqttTopic).Build());
            // Publishing
            await managedMQTT.PublishAsync(message);
            Console.WriteLine("Message: " + mqttMessage + " published to Topic " + mqttTopic);
            await managedMQTT.StartAsync(options);

            /*
            // Create a new MQTT client.
            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();
            Console.WriteLine("Starting MQTT Client");

            // Use WebSocket connection.
            Console.Write("MQTT Broker IP: ");
            string brokerIP2 = Console.ReadLine();
            var options2 = new MqttClientOptionsBuilder()
                .WithWebSocketServer(brokerIP)
                .Build();
            Console.WriteLine("Connecting to " + brokerIP2 + " via TCP");

            await mqttClient.ConnectAsync(options2);        

            // Reconnection
            mqttClient.UseDisconnectedHandler(async e =>
            {
                Console.WriteLine("### DISCONNECTED FROM SERVER ###");
                await Task.Delay(TimeSpan.FromSeconds(5));
                
                try
                {
                    await mqttClient.ConnectAsync(options2); // Since 3.0.5 with CancellationToken
                }
                catch
                {
                    Console.WriteLine("### RECONNECTING FAILED ###");
                }
            });

            // Message options
            Console.Write("Topic: ");
            string mqttTopic2 = Console.ReadLine();
            Console.Write("Message: ");
            string mqttMessage2 = Console.ReadLine();
            var message2 = new MqttApplicationMessageBuilder()
                .WithTopic(mqttTopic)
                .WithPayload(mqttMessage)
                .WithExactlyOnceQoS()
                .WithRetainFlag()
                .Build();

            // Publishing
            await mqttClient.PublishAsync(message2);
            Console.WriteLine("Message: " + mqttMessage2 + " published to Topic " + mqttTopic2);
            */
        }
    }
}
