﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OpcEdgeClient.OpcWCFInterface {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.CollectionDataContractAttribute(Name="ListOfSimpleAttributeOperand", Namespace="http://opcfoundation.org/UA/2008/02/Types.xsd", ItemName="SimpleAttributeOperand")]
    [System.SerializableAttribute()]
    public class ListOfSimpleAttributeOperand : System.Collections.Generic.List<Opc.Ua.SimpleAttributeOperand> {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.CollectionDataContractAttribute(Name="ListOfQualifiedName", Namespace="http://opcfoundation.org/UA/2008/02/Types.xsd", ItemName="QualifiedName")]
    [System.SerializableAttribute()]
    public class ListOfQualifiedName : System.Collections.Generic.List<Opc.Ua.QualifiedName> {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.CollectionDataContractAttribute(Name="ListOfContentFilterElement", Namespace="http://opcfoundation.org/UA/2008/02/Types.xsd", ItemName="ContentFilterElement")]
    [System.SerializableAttribute()]
    public class ListOfContentFilterElement : System.Collections.Generic.List<Opc.Ua.ContentFilterElement> {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.CollectionDataContractAttribute(Name="ListOfExtensionObject", Namespace="http://opcfoundation.org/UA/2008/02/Types.xsd", ItemName="ExtensionObject")]
    [System.SerializableAttribute()]
    public class ListOfExtensionObject : System.Collections.Generic.List<Opc.Ua.ExtensionObject> {
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="OpcWCFInterface.IOpcWCFInterface")]
    public interface IOpcWCFInterface {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpcWCFInterface/MQTTConnectClientAsync", ReplyAction="http://tempuri.org/IOpcWCFInterface/MQTTConnectClientAsyncResponse")]
        void MQTTConnectClientAsync(string mqttIP);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpcWCFInterface/MQTTConnectClientAsync", ReplyAction="http://tempuri.org/IOpcWCFInterface/MQTTConnectClientAsyncResponse")]
        System.Threading.Tasks.Task MQTTConnectClientAsyncAsync(string mqttIP);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpcWCFInterface/MQTTSubscribeTopic", ReplyAction="http://tempuri.org/IOpcWCFInterface/MQTTSubscribeTopicResponse")]
        void MQTTSubscribeTopic(string topic);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpcWCFInterface/MQTTSubscribeTopic", ReplyAction="http://tempuri.org/IOpcWCFInterface/MQTTSubscribeTopicResponse")]
        System.Threading.Tasks.Task MQTTSubscribeTopicAsync(string topic);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpcWCFInterface/MQTTUnsubscribeTopic", ReplyAction="http://tempuri.org/IOpcWCFInterface/MQTTUnsubscribeTopicResponse")]
        void MQTTUnsubscribeTopic(string topic);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpcWCFInterface/MQTTUnsubscribeTopic", ReplyAction="http://tempuri.org/IOpcWCFInterface/MQTTUnsubscribeTopicResponse")]
        System.Threading.Tasks.Task MQTTUnsubscribeTopicAsync(string topic);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpcWCFInterface/MQTTPublishTopicAsync", ReplyAction="http://tempuri.org/IOpcWCFInterface/MQTTPublishTopicAsyncResponse")]
        void MQTTPublishTopicAsync(string topic, string message);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpcWCFInterface/MQTTPublishTopicAsync", ReplyAction="http://tempuri.org/IOpcWCFInterface/MQTTPublishTopicAsyncResponse")]
        System.Threading.Tasks.Task MQTTPublishTopicAsyncAsync(string topic, string message);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpcWCFInterface/MQTTSubscribedTopics", ReplyAction="http://tempuri.org/IOpcWCFInterface/MQTTSubscribedTopicsResponse")]
        string[] MQTTSubscribedTopics();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpcWCFInterface/MQTTSubscribedTopics", ReplyAction="http://tempuri.org/IOpcWCFInterface/MQTTSubscribedTopicsResponse")]
        System.Threading.Tasks.Task<string[]> MQTTSubscribedTopicsAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpcWCFInterface/OPCConnect", ReplyAction="http://tempuri.org/IOpcWCFInterface/OPCConnectResponse")]
        void OPCConnect(string opcEndpoint);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpcWCFInterface/OPCConnect", ReplyAction="http://tempuri.org/IOpcWCFInterface/OPCConnectResponse")]
        System.Threading.Tasks.Task OPCConnectAsync(string opcEndpoint);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpcWCFInterface/OPCSubscribe", ReplyAction="http://tempuri.org/IOpcWCFInterface/OPCSubscribeResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Opc.Ua.DataChangeFilter))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Opc.Ua.EventFilter))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Opc.Ua.AggregateFilter))]
        void OPCSubscribe(Opc.Ua.Client.Subscription subscription);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpcWCFInterface/OPCSubscribe", ReplyAction="http://tempuri.org/IOpcWCFInterface/OPCSubscribeResponse")]
        System.Threading.Tasks.Task OPCSubscribeAsync(Opc.Ua.Client.Subscription subscription);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpcWCFInterface/OPCUnsubscribe", ReplyAction="http://tempuri.org/IOpcWCFInterface/OPCUnsubscribeResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Opc.Ua.DataChangeFilter))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Opc.Ua.EventFilter))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Opc.Ua.AggregateFilter))]
        void OPCUnsubscribe(Opc.Ua.Client.Subscription subscription);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpcWCFInterface/OPCUnsubscribe", ReplyAction="http://tempuri.org/IOpcWCFInterface/OPCUnsubscribeResponse")]
        System.Threading.Tasks.Task OPCUnsubscribeAsync(Opc.Ua.Client.Subscription subscription);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpcWCFInterface/OPCMonitor", ReplyAction="http://tempuri.org/IOpcWCFInterface/OPCMonitorResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Opc.Ua.DataChangeFilter))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Opc.Ua.EventFilter))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Opc.Ua.AggregateFilter))]
        void OPCMonitor(Opc.Ua.Client.Subscription subscription, Opc.Ua.Client.MonitoredItem monitoredItem);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpcWCFInterface/OPCMonitor", ReplyAction="http://tempuri.org/IOpcWCFInterface/OPCMonitorResponse")]
        System.Threading.Tasks.Task OPCMonitorAsync(Opc.Ua.Client.Subscription subscription, Opc.Ua.Client.MonitoredItem monitoredItem);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpcWCFInterface/OPCUnmonitor", ReplyAction="http://tempuri.org/IOpcWCFInterface/OPCUnmonitorResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Opc.Ua.DataChangeFilter))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Opc.Ua.EventFilter))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Opc.Ua.AggregateFilter))]
        void OPCUnmonitor(Opc.Ua.Client.Subscription subscription, Opc.Ua.Client.MonitoredItem monitoredItem);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpcWCFInterface/OPCUnmonitor", ReplyAction="http://tempuri.org/IOpcWCFInterface/OPCUnmonitorResponse")]
        System.Threading.Tasks.Task OPCUnmonitorAsync(Opc.Ua.Client.Subscription subscription, Opc.Ua.Client.MonitoredItem monitoredItem);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpcWCFInterface/OPCDisconnect", ReplyAction="http://tempuri.org/IOpcWCFInterface/OPCDisconnectResponse")]
        void OPCDisconnect();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpcWCFInterface/OPCDisconnect", ReplyAction="http://tempuri.org/IOpcWCFInterface/OPCDisconnectResponse")]
        System.Threading.Tasks.Task OPCDisconnectAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpcWCFInterface/CheckConnected", ReplyAction="http://tempuri.org/IOpcWCFInterface/CheckConnectedResponse")]
        bool CheckConnected();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpcWCFInterface/CheckConnected", ReplyAction="http://tempuri.org/IOpcWCFInterface/CheckConnectedResponse")]
        System.Threading.Tasks.Task<bool> CheckConnectedAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpcWCFInterface/CheckService", ReplyAction="http://tempuri.org/IOpcWCFInterface/CheckServiceResponse")]
        bool CheckService();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpcWCFInterface/CheckService", ReplyAction="http://tempuri.org/IOpcWCFInterface/CheckServiceResponse")]
        System.Threading.Tasks.Task<bool> CheckServiceAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpcWCFInterface/SessionEndpoint", ReplyAction="http://tempuri.org/IOpcWCFInterface/SessionEndpointResponse")]
        string SessionEndpoint();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpcWCFInterface/SessionEndpoint", ReplyAction="http://tempuri.org/IOpcWCFInterface/SessionEndpointResponse")]
        System.Threading.Tasks.Task<string> SessionEndpointAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpcWCFInterface/MainFolder", ReplyAction="http://tempuri.org/IOpcWCFInterface/MainFolderResponse")]
        string MainFolder();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpcWCFInterface/MainFolder", ReplyAction="http://tempuri.org/IOpcWCFInterface/MainFolderResponse")]
        System.Threading.Tasks.Task<string> MainFolderAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpcWCFInterface/SessionsFolder", ReplyAction="http://tempuri.org/IOpcWCFInterface/SessionsFolderResponse")]
        string SessionsFolder();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpcWCFInterface/SessionsFolder", ReplyAction="http://tempuri.org/IOpcWCFInterface/SessionsFolderResponse")]
        System.Threading.Tasks.Task<string> SessionsFolderAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpcWCFInterface/SubscriptionsFolder", ReplyAction="http://tempuri.org/IOpcWCFInterface/SubscriptionsFolderResponse")]
        string SubscriptionsFolder();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpcWCFInterface/SubscriptionsFolder", ReplyAction="http://tempuri.org/IOpcWCFInterface/SubscriptionsFolderResponse")]
        System.Threading.Tasks.Task<string> SubscriptionsFolderAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpcWCFInterface/ItemsFolder", ReplyAction="http://tempuri.org/IOpcWCFInterface/ItemsFolderResponse")]
        string ItemsFolder();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOpcWCFInterface/ItemsFolder", ReplyAction="http://tempuri.org/IOpcWCFInterface/ItemsFolderResponse")]
        System.Threading.Tasks.Task<string> ItemsFolderAsync();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IOpcWCFInterfaceChannel : OpcEdgeClient.OpcWCFInterface.IOpcWCFInterface, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class OpcWCFInterfaceClient : System.ServiceModel.ClientBase<OpcEdgeClient.OpcWCFInterface.IOpcWCFInterface>, OpcEdgeClient.OpcWCFInterface.IOpcWCFInterface {
        
        public OpcWCFInterfaceClient() {
        }
        
        public OpcWCFInterfaceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public OpcWCFInterfaceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public OpcWCFInterfaceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public OpcWCFInterfaceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public void MQTTConnectClientAsync(string mqttIP) {
            base.Channel.MQTTConnectClientAsync(mqttIP);
        }
        
        public System.Threading.Tasks.Task MQTTConnectClientAsyncAsync(string mqttIP) {
            return base.Channel.MQTTConnectClientAsyncAsync(mqttIP);
        }
        
        public void MQTTSubscribeTopic(string topic) {
            base.Channel.MQTTSubscribeTopic(topic);
        }
        
        public System.Threading.Tasks.Task MQTTSubscribeTopicAsync(string topic) {
            return base.Channel.MQTTSubscribeTopicAsync(topic);
        }
        
        public void MQTTUnsubscribeTopic(string topic) {
            base.Channel.MQTTUnsubscribeTopic(topic);
        }
        
        public System.Threading.Tasks.Task MQTTUnsubscribeTopicAsync(string topic) {
            return base.Channel.MQTTUnsubscribeTopicAsync(topic);
        }
        
        public void MQTTPublishTopicAsync(string topic, string message) {
            base.Channel.MQTTPublishTopicAsync(topic, message);
        }
        
        public System.Threading.Tasks.Task MQTTPublishTopicAsyncAsync(string topic, string message) {
            return base.Channel.MQTTPublishTopicAsyncAsync(topic, message);
        }
        
        public string[] MQTTSubscribedTopics() {
            return base.Channel.MQTTSubscribedTopics();
        }
        
        public System.Threading.Tasks.Task<string[]> MQTTSubscribedTopicsAsync() {
            return base.Channel.MQTTSubscribedTopicsAsync();
        }
        
        public void OPCConnect(string opcEndpoint) {
            base.Channel.OPCConnect(opcEndpoint);
        }
        
        public System.Threading.Tasks.Task OPCConnectAsync(string opcEndpoint) {
            return base.Channel.OPCConnectAsync(opcEndpoint);
        }
        
        public void OPCSubscribe(Opc.Ua.Client.Subscription subscription) {
            base.Channel.OPCSubscribe(subscription);
        }
        
        public System.Threading.Tasks.Task OPCSubscribeAsync(Opc.Ua.Client.Subscription subscription) {
            return base.Channel.OPCSubscribeAsync(subscription);
        }
        
        public void OPCUnsubscribe(Opc.Ua.Client.Subscription subscription) {
            base.Channel.OPCUnsubscribe(subscription);
        }
        
        public System.Threading.Tasks.Task OPCUnsubscribeAsync(Opc.Ua.Client.Subscription subscription) {
            return base.Channel.OPCUnsubscribeAsync(subscription);
        }
        
        public void OPCMonitor(Opc.Ua.Client.Subscription subscription, Opc.Ua.Client.MonitoredItem monitoredItem) {
            base.Channel.OPCMonitor(subscription, monitoredItem);
        }
        
        public System.Threading.Tasks.Task OPCMonitorAsync(Opc.Ua.Client.Subscription subscription, Opc.Ua.Client.MonitoredItem monitoredItem) {
            return base.Channel.OPCMonitorAsync(subscription, monitoredItem);
        }
        
        public void OPCUnmonitor(Opc.Ua.Client.Subscription subscription, Opc.Ua.Client.MonitoredItem monitoredItem) {
            base.Channel.OPCUnmonitor(subscription, monitoredItem);
        }
        
        public System.Threading.Tasks.Task OPCUnmonitorAsync(Opc.Ua.Client.Subscription subscription, Opc.Ua.Client.MonitoredItem monitoredItem) {
            return base.Channel.OPCUnmonitorAsync(subscription, monitoredItem);
        }
        
        public void OPCDisconnect() {
            base.Channel.OPCDisconnect();
        }
        
        public System.Threading.Tasks.Task OPCDisconnectAsync() {
            return base.Channel.OPCDisconnectAsync();
        }
        
        public bool CheckConnected() {
            return base.Channel.CheckConnected();
        }
        
        public System.Threading.Tasks.Task<bool> CheckConnectedAsync() {
            return base.Channel.CheckConnectedAsync();
        }
        
        public bool CheckService() {
            return base.Channel.CheckService();
        }
        
        public System.Threading.Tasks.Task<bool> CheckServiceAsync() {
            return base.Channel.CheckServiceAsync();
        }
        
        public string SessionEndpoint() {
            return base.Channel.SessionEndpoint();
        }
        
        public System.Threading.Tasks.Task<string> SessionEndpointAsync() {
            return base.Channel.SessionEndpointAsync();
        }
        
        public string MainFolder() {
            return base.Channel.MainFolder();
        }
        
        public System.Threading.Tasks.Task<string> MainFolderAsync() {
            return base.Channel.MainFolderAsync();
        }
        
        public string SessionsFolder() {
            return base.Channel.SessionsFolder();
        }
        
        public System.Threading.Tasks.Task<string> SessionsFolderAsync() {
            return base.Channel.SessionsFolderAsync();
        }
        
        public string SubscriptionsFolder() {
            return base.Channel.SubscriptionsFolder();
        }
        
        public System.Threading.Tasks.Task<string> SubscriptionsFolderAsync() {
            return base.Channel.SubscriptionsFolderAsync();
        }
        
        public string ItemsFolder() {
            return base.Channel.ItemsFolder();
        }
        
        public System.Threading.Tasks.Task<string> ItemsFolderAsync() {
            return base.Channel.ItemsFolderAsync();
        }
    }
}
