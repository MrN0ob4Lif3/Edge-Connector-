using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;

namespace brokerService
{
    [ServiceContract]
    public interface IBrokerService
    {
        #region MQTT Operations
        [OperationContract]
        void MQTTConnectClientAsync(String mqttIP, int option);

        [OperationContract]
        void MQTTSubscribeTopic(String topic);

        [OperationContract]
        void MQTTUnsubscribeTopic(String topic);

        [OperationContract]
        void MQTTPublishTopicAsync(String topic, String message);

        [OperationContract]
        List<String> MQTTSubscribedTopics();
        #endregion

        #region OPC Operations
        [OperationContract]
        void OPCCreateClient(String opcIP, bool securityCheck);

        [OperationContract]
        void OPCConnectClient();

        [OperationContract]
        void OPCSubscribeTopic();

        [OperationContract]
        void OPCUnsubscribeTopic();

        [OperationContract]
        void Connect(ConfiguredEndpoint endpoint);

        [OperationContract]
        ApplicationInstance GetApplicationInstance();

        [OperationContract]
        ConfiguredEndpointCollection GetEndpoints();

        [OperationContract]
        Session GetSession();

        [OperationContract]
        Browser GetBrowser();
        #endregion
    }

    [DataContract]
    public class SessionSurrogate
    {
        Session session;

        [DataMember]
        public Session OPCSession
        {
            get { return session; }
            set { session = value; }
        }
    }

}
