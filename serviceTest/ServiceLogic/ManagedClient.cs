using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Server;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Protocol;
using Opc.Ua.Configuration;
using Opc.Ua;

namespace ServiceLogic
{
    //Interface to allow data to be passed between Windows Service and hosted WCF Service instance.
    public interface IServiceCallback
    {
        void MQTTConnect(String brokerIP);
        void MQTTSubscribe(String topic);
        void MQTTUnsubscribe(String topic);
        void MQTTPublish(String topic, String message);
        HashSet<String> MQTTSubscribedTopics();
        ApplicationInstance OPCApplicationInstance();
        ConfiguredEndpointCollection OPCEndpoints();

    }

    //Static Property to allow for use of the interface above.
    public static class Host
    {
        public static IServiceCallback Current;
        public static String MQTTBrokerIP;
    }

    //MQTT functions
    public class ManagedClient 
    {

        //Creating a new MQTT client.
        public static IManagedMqttClient CreateManagedClient()
        {
            var managedMqttClient = new MqttFactory().CreateManagedMqttClient();
            return managedMqttClient;
        }

        //MQTTClient connector (Throw in constructed client, brokerIP) [TCP]
        public static async Task ManagedMqttConnectTCPAsync(IManagedMqttClient managedMqttClient, string brokerIP)
        {
            // Use TCP connection.
            string clientID = Guid.NewGuid().ToString();
            var ms = new ClientRetainedMessageHandler();
            var options = new ManagedMqttClientOptions
            {
                ClientOptions = new MqttClientOptions
                {
                    ClientId = "MQTTnetManagedClient "+clientID+" (TCP)",
                    ChannelOptions = new MqttClientTcpOptions
                    {
                        Server = brokerIP
                    }
                },
                AutoReconnectDelay = TimeSpan.FromSeconds(1),
                Storage = ms
            };
            await managedMqttClient.StartAsync(options);
        }

        //MQTTClient connector (Throw in constructed client, brokerIP) [WebSocket]
        public static async Task ManagedMqttConnectWebSocket(IManagedMqttClient managedMqttClient, string brokerIP)
        {
            // Use WebSocket connection.
            string clientID = Guid.NewGuid().ToString();
            var ms = new ClientRetainedMessageHandler();
            var options = new ManagedMqttClientOptions
            {
                ClientOptions = new MqttClientOptions
                {
                    ClientId = "MQTTnetManagedClient " + clientID + " (WebSocket)",
                    ChannelOptions = new MqttClientWebSocketOptions
                    {
                        Uri = brokerIP
                    }
                },
                AutoReconnectDelay = TimeSpan.FromSeconds(1),
                Storage = ms
            };
            await managedMqttClient.StartAsync(options);
        }

        //MQTTClient topic subscription
        public static void ManagedMqttSubscribe(IManagedMqttClient managedMqttClient, string mqttTopic)
        {
            managedMqttClient.UseConnectedHandler(async e =>
            {
                // Subscribe to a topic
                await managedMqttClient.SubscribeAsync(new TopicFilter { Topic = mqttTopic, QualityOfServiceLevel = MqttQualityOfServiceLevel.AtMostOnce });
            });
            /*
            managedMqttClient.UseApplicationMessageReceivedHandler(e =>
            {
                Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
                Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
                Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
                Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
                Console.WriteLine();

                Task.Run(() => managedMqttClient.PublishAsync(mqttTopic));
            });
            */
        }

        //MQTTClient topic cancelling subscription
        public static void ManagedMqttUnsubscribe(IManagedMqttClient managedMqttClient, string mqttTopic)
        {
            managedMqttClient.UseConnectedHandler(async e =>
            {
                await managedMqttClient.UnsubscribeAsync(mqttTopic);
            });
        }

        //MQTTClient message publishing options
        public static async Task ManagedMqttPublish(IManagedMqttClient managedMqttClient, string mqttTopic, string mqttMessage)
        {
            // Message options
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(mqttTopic)
                .WithPayload(mqttMessage)
                .WithExactlyOnceQoS()
                .WithRetainFlag()
                .Build();
            // Publishing
            //await managedMqttClient.PublishAsync(builder => builder.WithTopic(mqttTopic).WithPayload(mqttMessage));
            await managedMqttClient.PublishAsync(message);
        }

        //Handler to retain messages to be sent to broker in the event connection is lost midway
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
