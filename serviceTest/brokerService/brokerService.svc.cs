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
using System.Diagnostics;

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

        public async void CreateClientAsync(String brokerIP, int option)
        {
            //Create a new ManagedMQTT Client.
            var mqttFactory = new MqttFactory();
            var managedMQTT = mqttFactory.CreateManagedMqttClient();

            if(option == 1)
            {
                try
                {
                    // Use WebSocket connection.
                    await MQTTCore.ManagedClient.ManagedMqttConnectWebSocket(managedMQTT, brokerIP);

                    // Message options
                    string mqttTopic = "WebSocketTopic";
                    string mqttMessage = "WebSocketMessage";

                    //Subscription
                    MQTTCore.ManagedClient.ManagedMqttSubscribe(managedMQTT, mqttTopic);

                    // Publishing
                    await MQTTCore.ManagedClient.ManagedMqttPublish(managedMQTT, mqttTopic, mqttMessage);
                }

                catch (Exception e)
                {
                    // Create an EventLog instance and assign its source.
                    EventLog myLog = new EventLog();
                    myLog.Source = "brokerServiceWebSocket";
                    // Write an informational entry to the event log.
                    myLog.WriteEntry(e.Message);
                }
            }
            else if(option == 2)
            {
                try
                {
                    // Use TCP connection.
                    await MQTTCore.ManagedClient.ManagedMqttConnectTCPAsync(managedMQTT, brokerIP);

                    // Message options
                    string mqttTopic = "TCPTopic";
                    string mqttMessage = "TCPMessage";

                    //Subscription
                    MQTTCore.ManagedClient.ManagedMqttSubscribe(managedMQTT, mqttTopic);

                    // Publishing
                    await MQTTCore.ManagedClient.ManagedMqttPublish(managedMQTT, mqttTopic, mqttMessage);
                }catch (Exception e)
                {
                    // Create an EventLog instance and assign its source.
                    EventLog myLog = new EventLog();
                    myLog.Source = "brokerServiceTCP";
                    // Write an informational entry to the event log.
                    myLog.WriteEntry(e.Message);
                }
            }

        }
    }
}
