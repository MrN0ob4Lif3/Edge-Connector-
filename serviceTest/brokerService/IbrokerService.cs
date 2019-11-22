using System;
using System.Runtime.Serialization;
using System.ServiceModel;


namespace brokerService
{
    [ServiceContract]
    public interface IBrokerService
    {
        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);

        [OperationContract]
        void MQTTCreateClientAsync(String mqttIP, int option);

        [OperationContract]
        void MQTTConnectClientAsync(String mqttIP, int option);

        [OperationContract]
        void MQTTSubscribeTopicAsync(String topic);

        [OperationContract]
        void MQTTUnsubscribeTopicAsync(String topic);

        [OperationContract]
        void MQTTPublishTopicAsync(String topic, String message);

        [OperationContract]
        void OPCCreateClient(String opcIP, bool securityCheck);

        [OperationContract]
        void OPCConnectClient();

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
