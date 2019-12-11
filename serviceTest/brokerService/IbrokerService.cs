using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using Opc.Ua;
using Opc.Ua.Sample.Controls;

namespace brokerService
{
    [ServiceContract]
    public interface IBrokerService
    {
        [OperationContract]
        void MQTTCreateClientAsync(String mqttIP, int option);

        [OperationContract]
        void MQTTConnectClientAsync(String mqttIP, int option);

        [OperationContract]
        void MQTTSubscribeTopic(String topic);

        [OperationContract]
        void MQTTUnsubscribeTopic(String topic);

        [OperationContract]
        void MQTTPublishTopicAsync(String topic, String message);
        
        [OperationContract]
        void OPCCreateClient(String opcIP, bool securityCheck);

        [OperationContract]
        void OPCConnectClient(ConfiguredEndpoint endpoint, SessionTreeCtrl opcSession, Opc.Ua.Sample.Controls.BrowseTreeCtrl opcBrowse);

        [OperationContract]
        void OPCSubscribeTopic();

        [OperationContract]
        void OPCUnsubscribeTopic();
        
    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}
