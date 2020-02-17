using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;

namespace OpcWCFInterface
{
    [ServiceContract]
    public interface IOpcWCFInterface
    {
        #region MQTT Operations
        [OperationContract]
        void MQTTConnectClientAsync(String mqttIP);

        [OperationContract]
        void MQTTSubscribeTopic(String topic);

        [OperationContract]
        void MQTTUnsubscribeTopic(String topic);

        [OperationContract]
        void MQTTPublishTopicAsync(String topic, String message);

        [OperationContract]
        HashSet<String> MQTTSubscribedTopics();
        #endregion

        #region OPC Operations
        [OperationContract]
        void OPCConnect(String opcEndpoint);

        [OperationContract]
        void OPCSubscribe(Subscription subscription);

        [OperationContract]
        void OPCUnsubscribe(Subscription subscription);

        [OperationContract]
        void OPCMonitor(Subscription subscription, MonitoredItem monitoredItem);

        [OperationContract]
        void OPCUnmonitor(Subscription subscription, MonitoredItem monitoredItem);

        [OperationContract]
        void OPCDisconnect();

        [OperationContract]
        bool CheckConnected();

        [OperationContract]
        bool CheckService();
        [OperationContract]
        string SessionEndpoint();
        #endregion

        #region Service Operations
        [OperationContract]
        string SessionsFolder();

        [OperationContract]
        string SubscriptionsFolder();

        [OperationContract]
        string ItemsFolder();
        #endregion
    }
}
