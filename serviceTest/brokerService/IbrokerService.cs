﻿using System;
using System.Runtime.Serialization;
using System.ServiceModel;


namespace brokerService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IbrokerService
    {

        [OperationContract]
        string GetData(String value);

        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);

        [OperationContract]
        void CreateClientAsync(String brokerIP, int option);

        [OperationContract]
        void ConnectClientAsync(String brokerIP, int option);

        [OperationContract]
        void SubscribeTopicAsync(String topic);

        [OperationContract]
        void UnsubscribeTopicAsync(String topic);

        [OperationContract]
        void PublishTopicAsync(String topic, String message);
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
