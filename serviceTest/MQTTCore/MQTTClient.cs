using System;
using System.Text;
using System.Threading.Tasks;

using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet.Client.Subscribing;
using MQTTnet.Server;

namespace MQTTCore
{
    public class MqttClient
    {
        //Creating a new MQTT client.
        public static IMqttClient CreateClient()
        {
            var mqttClient = new MqttFactory().CreateMqttClient();
            return mqttClient;
        }

        //MQTTClient connector (Throw in constructed client, brokerIP) [TCP]
        public static async Task MqttConnectTCPAsync(IMqttClient mqttClient, string brokerIP)
        {
            // Use TCP connection.
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(brokerIP)
                .Build();
            await mqttClient.ConnectAsync(options);

            mqttClient.UseDisconnectedHandler(async e =>
            {
                Console.WriteLine("### DISCONNECTED FROM SERVER ###");
                await Task.Delay(TimeSpan.FromSeconds(5));

                try
                {
                    await mqttClient.ConnectAsync(options); 
                }
                catch
                {
                    Console.WriteLine("### RECONNECTING FAILED ###");
                }
            });
        }

        //MQTTClient connector (Throw in constructed client, brokerIP) [WebSocket]
        public static async Task MqttConnectWebSocket(IMqttClient mqttClient, string brokerIP)
        {
            // Use WebSocket connection.
            var options = new MqttClientOptionsBuilder()
                .WithWebSocketServer(brokerIP)
                .Build();
            await mqttClient.ConnectAsync(options);

            mqttClient.UseDisconnectedHandler(async e =>
            {
                Console.WriteLine("### DISCONNECTED FROM SERVER ###");
                await Task.Delay(TimeSpan.FromSeconds(5));

                try
                {
                    await mqttClient.ConnectAsync(options);
                }
                catch
                {
                    Console.WriteLine("### RECONNECTING FAILED ###");
                }
            });
        }

        //MQTTClient topic subscription
        public static void MqttSubscribe(IMqttClient mqttClient, string mqttTopic)
        {
            mqttClient.UseConnectedHandler(async e =>
            {
                // Subscribe to a topic
                await mqttClient.SubscribeAsync(new TopicFilterBuilder().WithTopic(mqttTopic).Build());
            });

            mqttClient.UseApplicationMessageReceivedHandler(e =>
            {
                Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
                Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
                Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
                Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
                Console.WriteLine();

                Task.Run(() => mqttClient.PublishAsync(mqttTopic));
            });
        }

        //MQTTClient topic cancelling subscription
        public static void MqttUnsubscribe(IMqttClient mqttClient, string mqttTopic)
        {
            mqttClient.UseConnectedHandler(async e =>
            {
                await mqttClient.UnsubscribeAsync(mqttTopic);
            });
        }

        //MQTTClient message publishing options
        public static async Task MqttPublish(IMqttClient mqttClient, string mqttTopic, string mqttMessage)
        {
            // Message options
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(mqttTopic)
                .WithPayload(mqttMessage)
                .WithExactlyOnceQoS()
                .WithRetainFlag()
                .Build();
            // Publishing
            await mqttClient.PublishAsync(message);
        }

    }
}
