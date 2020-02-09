using System;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Client.Controls;
using System.Diagnostics;
using Opc.Ua.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using Opc.Ua.ServiceLogic;

namespace OpcWCFInterface
{

    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class OpcWCFInterface : IOpcWCFInterface
    {
        #region MQTT Methods / Properties
        //MQTT client connection function. Requires IP address of MQTT server and connection option type
        public void MQTTConnectClientAsync(String mqttIP)
        {
            try
            {
                Host.Current.MQTTConnect(mqttIP);
            }
            catch (Exception e)
            {
                // Create an EventLog instance and assign its source.
                EventLog myLog = new EventLog
                {
                    Source = "OpcWCFInterfaceMQTTTCP"
                };
                // Write an informational entry to the event log.
                myLog.WriteEntry(e.Message);
            }
        }
        //MQTT topic subscription function. Requires topic to subscribe to.
        public void MQTTSubscribeTopic(String topic)
        {
            try
            {
                Host.Current.MQTTSubscribe(topic);
            }
            catch (Exception e)
            {
                // Create an EventLog instance and assign its source.
                EventLog myLog = new EventLog
                {
                    Source = "OpcWCFInterfaceMQTTSubscribe"
                };
                // Write an informational entry to the event log.
                myLog.WriteEntry(e.Message);
            }
        }
        //MQTT topic unsubscription function. Requires topic to subscribe to.
        public void MQTTUnsubscribeTopic(String topic)
        {
            try
            {
                Host.Current.MQTTUnsubscribe(topic);
            }
            catch (Exception e)
            {
                // Create an EventLog instance and assign its source.
                EventLog myLog = new EventLog
                {
                    Source = "OpcWCFInterfaceMQTTSubscribe"
                };
                // Write an informational entry to the event log.
                myLog.WriteEntry(e.Message);
            }
        }
        //MQTT message publishing function. Requires topic to publish message to.
        public void MQTTPublishTopicAsync(String topic, String message)
        {
            try
            {
                Host.Current.MQTTPublish(topic, message);
            }
            catch (Exception e)
            {
                // Create an EventLog instance and assign its source.
                EventLog myLog = new EventLog
                {
                    Source = "OpcWCFInterfaceMQTTPublish"
                };
                // Write an informational entry to the event log.
                myLog.WriteEntry(e.Message);
            }
        }
        //Returns list of subscribed topics for MQTT client.
        public HashSet<String> MQTTSubscribedTopics()
        {
            return Host.Current.MQTTSubscribedTopics();
        }
        #endregion

        #region OPC Methods
        //Connects to OPC endpoint
        public void OPCConnect(string opcEndpoint)
        {
            Host.Current.OPCConnect(opcEndpoint);
        }

        //Passes subscription to service session for subscription.
        public void OPCSubscribe(Subscription subscription)
        {
            Host.Current.OPCSubscribe(subscription);
        }
        //Passes subscription to service session for unsubscription.

        public void OPCUnsubscribe(Subscription subscription)
        {
            Host.Current.OPCUnsubscribe(subscription);
        }

        //Passes monitored item to service session to add to subscription.
        public void OPCMonitor(Subscription subscription, MonitoredItem monitoredItem)
        {
            Host.Current.OPCMonitor(subscription, monitoredItem);
        }

        //Passes monitored item to service session to remove from subscription.
        public void OPCUnmonitor(Subscription subscription, MonitoredItem monitoredItem)
        {
            Host.Current.OPCUnmonitor(subscription, monitoredItem);
        }

        //Calls service session to disconnect and close existing session
        public void OPCDisconnect()
        {
            Host.Current.OPCDisconnect();
        }

        public string SessionEndpoint()
        {
            return Host.Current.SessionEndpoint();
        }
        #endregion

        #region Service Methods / Properties
        //Checks if service has a session running and disconnects if connected.
        public bool CheckConnected()
        {
            return Host.Current.CheckConnected();
        }

        //Checks if service has been started.
        public bool CheckService()
        {
            return Host.Current.CheckService();
        }
        
        public string SessionsFolder()
        {
            return Host.Current.SessionsFolder();
        }

        public string SubscriptionsFolder()
        {
            return Host.Current.SubscriptionsFolder();
        }

        public string ItemsFolder()
        {
            return Host.Current.ItemsFolder();
        }

        #endregion
    }
}
