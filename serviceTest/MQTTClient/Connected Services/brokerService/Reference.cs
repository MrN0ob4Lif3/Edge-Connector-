﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BrokerClient.brokerService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.CollectionDataContractAttribute(Name="ListOfString", Namespace="http://opcfoundation.org/UA/2008/02/Types.xsd", ItemName="String")]
    [System.SerializableAttribute()]
    public class ListOfString : System.Collections.Generic.List<string> {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.CollectionDataContractAttribute(Name="ListOfUserTokenPolicy", Namespace="http://opcfoundation.org/UA/2008/02/Types.xsd", ItemName="UserTokenPolicy")]
    [System.SerializableAttribute()]
    public class ListOfUserTokenPolicy : System.Collections.Generic.List<Opc.Ua.UserTokenPolicy> {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.CollectionDataContractAttribute(Name="ListOfXmlElement", Namespace="http://opcfoundation.org/UA/2008/02/Types.xsd", ItemName="XmlElement")]
    [System.SerializableAttribute()]
    public class ListOfXmlElement : System.Collections.Generic.List<System.Xml.XmlElement> {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.CollectionDataContractAttribute(Name="ListOfCertificateIdentifier", Namespace="http://opcfoundation.org/UA/SDK/Configuration.xsd", ItemName="CertificateIdentifier")]
    [System.SerializableAttribute()]
    public class ListOfCertificateIdentifier : System.Collections.Generic.List<Opc.Ua.CertificateIdentifier> {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.CollectionDataContractAttribute(Name="ListOfTransportConfiguration", Namespace="http://opcfoundation.org/UA/SDK/Configuration.xsd", ItemName="TransportConfiguration")]
    [System.SerializableAttribute()]
    public class ListOfTransportConfiguration : System.Collections.Generic.List<Opc.Ua.TransportConfiguration> {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.CollectionDataContractAttribute(Name="ListOfServerSecurityPolicy", Namespace="http://opcfoundation.org/UA/SDK/Configuration.xsd", ItemName="ServerSecurityPolicy")]
    [System.SerializableAttribute()]
    public class ListOfServerSecurityPolicy : System.Collections.Generic.List<Opc.Ua.ServerSecurityPolicy> {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.CollectionDataContractAttribute(Name="ListOfLocalizedText", Namespace="http://opcfoundation.org/UA/2008/02/Types.xsd", ItemName="LocalizedText")]
    [System.SerializableAttribute()]
    public class ListOfLocalizedText : System.Collections.Generic.List<Opc.Ua.LocalizedText> {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.CollectionDataContractAttribute(Name="ListOfServerRegistration", Namespace="http://opcfoundation.org/UA/SDK/Configuration.xsd", ItemName="ServerRegistration")]
    [System.SerializableAttribute()]
    public class ListOfServerRegistration : System.Collections.Generic.List<Opc.Ua.ServerRegistration> {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.CollectionDataContractAttribute(Name="ListOfSamplingRateGroup", Namespace="http://opcfoundation.org/UA/SDK/Configuration.xsd", ItemName="SamplingRateGroup")]
    [System.SerializableAttribute()]
    public class ListOfSamplingRateGroup : System.Collections.Generic.List<Opc.Ua.SamplingRateGroup> {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.CollectionDataContractAttribute(Name="ListOfEndpointDescription", Namespace="http://opcfoundation.org/UA/2008/02/Types.xsd", ItemName="EndpointDescription")]
    [System.SerializableAttribute()]
    public class ListOfEndpointDescription : System.Collections.Generic.List<Opc.Ua.EndpointDescription> {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="MemberInfo", Namespace="http://schemas.datacontract.org/2004/07/System.Reflection")]
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(BrokerClient.brokerService.Type))]
    public partial class MemberInfo : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Type", Namespace="http://schemas.datacontract.org/2004/07/System")]
    [System.SerializableAttribute()]
    public partial class Type : BrokerClient.brokerService.MemberInfo {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.CollectionDataContractAttribute(Name="ListOfCertificateIdentifier", Namespace="http://opcfoundation.org/UA/2011/03/SecuredApplication.xsd", ItemName="CertificateIdentifier")]
    [System.SerializableAttribute()]
    public class ListOfCertificateIdentifier1 : System.Collections.Generic.List<Opc.Ua.Security.CertificateIdentifier> {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.CollectionDataContractAttribute(Name="ListOfBaseAddresses", Namespace="http://opcfoundation.org/UA/2011/03/SecuredApplication.xsd", ItemName="BaseAddress")]
    [System.SerializableAttribute()]
    public class ListOfBaseAddresses : System.Collections.Generic.List<string> {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.CollectionDataContractAttribute(Name="ListOfSecurityProfiles", Namespace="http://opcfoundation.org/UA/2011/03/SecuredApplication.xsd", ItemName="SecurityProfile")]
    [System.SerializableAttribute()]
    public class ListOfSecurityProfiles : System.Collections.Generic.List<Opc.Ua.Security.SecurityProfile> {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.CollectionDataContractAttribute(Name="ListOfExtensions", Namespace="http://opcfoundation.org/UA/2011/03/SecuredApplication.xsd", ItemName="Extension")]
    [System.SerializableAttribute()]
    public class ListOfExtensions : System.Collections.Generic.List<System.Xml.XmlElement> {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.CollectionDataContractAttribute(Name="ListOfApplicationAccessRule", Namespace="http://opcfoundation.org/UA/SDK/Configuration.xsd", ItemName="ApplicationAccessRule")]
    [System.SerializableAttribute()]
    public class ListOfApplicationAccessRule : System.Collections.Generic.List<Opc.Ua.ApplicationAccessRule> {
    }
    
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
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="brokerService.IBrokerService")]
    public interface IBrokerService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBrokerService/MQTTConnectClientAsync", ReplyAction="http://tempuri.org/IBrokerService/MQTTConnectClientAsyncResponse")]
        void MQTTConnectClientAsync(string mqttIP);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBrokerService/MQTTConnectClientAsync", ReplyAction="http://tempuri.org/IBrokerService/MQTTConnectClientAsyncResponse")]
        System.Threading.Tasks.Task MQTTConnectClientAsyncAsync(string mqttIP);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBrokerService/MQTTSubscribeTopic", ReplyAction="http://tempuri.org/IBrokerService/MQTTSubscribeTopicResponse")]
        void MQTTSubscribeTopic(string topic);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBrokerService/MQTTSubscribeTopic", ReplyAction="http://tempuri.org/IBrokerService/MQTTSubscribeTopicResponse")]
        System.Threading.Tasks.Task MQTTSubscribeTopicAsync(string topic);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBrokerService/MQTTUnsubscribeTopic", ReplyAction="http://tempuri.org/IBrokerService/MQTTUnsubscribeTopicResponse")]
        void MQTTUnsubscribeTopic(string topic);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBrokerService/MQTTUnsubscribeTopic", ReplyAction="http://tempuri.org/IBrokerService/MQTTUnsubscribeTopicResponse")]
        System.Threading.Tasks.Task MQTTUnsubscribeTopicAsync(string topic);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBrokerService/MQTTPublishTopicAsync", ReplyAction="http://tempuri.org/IBrokerService/MQTTPublishTopicAsyncResponse")]
        void MQTTPublishTopicAsync(string topic, string message);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBrokerService/MQTTPublishTopicAsync", ReplyAction="http://tempuri.org/IBrokerService/MQTTPublishTopicAsyncResponse")]
        System.Threading.Tasks.Task MQTTPublishTopicAsyncAsync(string topic, string message);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBrokerService/MQTTSubscribedTopics", ReplyAction="http://tempuri.org/IBrokerService/MQTTSubscribedTopicsResponse")]
        string[] MQTTSubscribedTopics();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBrokerService/MQTTSubscribedTopics", ReplyAction="http://tempuri.org/IBrokerService/MQTTSubscribedTopicsResponse")]
        System.Threading.Tasks.Task<string[]> MQTTSubscribedTopicsAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBrokerService/OPCCreateClient", ReplyAction="http://tempuri.org/IBrokerService/OPCCreateClientResponse")]
        void OPCCreateClient(string opcIP, bool securityCheck);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBrokerService/OPCCreateClient", ReplyAction="http://tempuri.org/IBrokerService/OPCCreateClientResponse")]
        System.Threading.Tasks.Task OPCCreateClientAsync(string opcIP, bool securityCheck);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBrokerService/Connect", ReplyAction="http://tempuri.org/IBrokerService/ConnectResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Opc.Ua.UserNameIdentityToken))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Opc.Ua.X509IdentityToken))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Opc.Ua.IssuedIdentityToken))]
        void Connect(Opc.Ua.ConfiguredEndpoint endpoint);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBrokerService/Connect", ReplyAction="http://tempuri.org/IBrokerService/ConnectResponse")]
        System.Threading.Tasks.Task ConnectAsync(Opc.Ua.ConfiguredEndpoint endpoint);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBrokerService/GetApplicationInstance", ReplyAction="http://tempuri.org/IBrokerService/GetApplicationInstanceResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Opc.Ua.CertificateTrustList))]
        Opc.Ua.Configuration.ApplicationInstance GetApplicationInstance();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBrokerService/GetApplicationInstance", ReplyAction="http://tempuri.org/IBrokerService/GetApplicationInstanceResponse")]
        System.Threading.Tasks.Task<Opc.Ua.Configuration.ApplicationInstance> GetApplicationInstanceAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBrokerService/GetEndpoints", ReplyAction="http://tempuri.org/IBrokerService/GetEndpointsResponse")]
        Opc.Ua.ConfiguredEndpointCollection GetEndpoints();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBrokerService/GetEndpoints", ReplyAction="http://tempuri.org/IBrokerService/GetEndpointsResponse")]
        System.Threading.Tasks.Task<Opc.Ua.ConfiguredEndpointCollection> GetEndpointsAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBrokerService/OPCConnect", ReplyAction="http://tempuri.org/IBrokerService/OPCConnectResponse")]
        void OPCConnect(string opcEndpoint);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBrokerService/OPCConnect", ReplyAction="http://tempuri.org/IBrokerService/OPCConnectResponse")]
        System.Threading.Tasks.Task OPCConnectAsync(string opcEndpoint);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBrokerService/OPCSubscribe", ReplyAction="http://tempuri.org/IBrokerService/OPCSubscribeResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Opc.Ua.DataChangeFilter))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Opc.Ua.EventFilter))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Opc.Ua.AggregateFilter))]
        void OPCSubscribe(Opc.Ua.Client.Subscription subscription);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBrokerService/OPCSubscribe", ReplyAction="http://tempuri.org/IBrokerService/OPCSubscribeResponse")]
        System.Threading.Tasks.Task OPCSubscribeAsync(Opc.Ua.Client.Subscription subscription);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBrokerService/OPCUnsubscribe", ReplyAction="http://tempuri.org/IBrokerService/OPCUnsubscribeResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Opc.Ua.DataChangeFilter))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Opc.Ua.EventFilter))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Opc.Ua.AggregateFilter))]
        void OPCUnsubscribe(Opc.Ua.Client.Subscription subscription);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBrokerService/OPCUnsubscribe", ReplyAction="http://tempuri.org/IBrokerService/OPCUnsubscribeResponse")]
        System.Threading.Tasks.Task OPCUnsubscribeAsync(Opc.Ua.Client.Subscription subscription);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBrokerService/OPCMonitor", ReplyAction="http://tempuri.org/IBrokerService/OPCMonitorResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Opc.Ua.DataChangeFilter))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Opc.Ua.EventFilter))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Opc.Ua.AggregateFilter))]
        void OPCMonitor(Opc.Ua.Client.Subscription subscription, Opc.Ua.Client.MonitoredItem monitoredItem);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBrokerService/OPCMonitor", ReplyAction="http://tempuri.org/IBrokerService/OPCMonitorResponse")]
        System.Threading.Tasks.Task OPCMonitorAsync(Opc.Ua.Client.Subscription subscription, Opc.Ua.Client.MonitoredItem monitoredItem);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBrokerService/OPCUnmonitor", ReplyAction="http://tempuri.org/IBrokerService/OPCUnmonitorResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Opc.Ua.DataChangeFilter))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Opc.Ua.EventFilter))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Opc.Ua.AggregateFilter))]
        void OPCUnmonitor(Opc.Ua.Client.Subscription subscription, Opc.Ua.Client.MonitoredItem monitoredItem);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBrokerService/OPCUnmonitor", ReplyAction="http://tempuri.org/IBrokerService/OPCUnmonitorResponse")]
        System.Threading.Tasks.Task OPCUnmonitorAsync(Opc.Ua.Client.Subscription subscription, Opc.Ua.Client.MonitoredItem monitoredItem);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBrokerService/CheckConnected", ReplyAction="http://tempuri.org/IBrokerService/CheckConnectedResponse")]
        bool CheckConnected();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBrokerService/CheckConnected", ReplyAction="http://tempuri.org/IBrokerService/CheckConnectedResponse")]
        System.Threading.Tasks.Task<bool> CheckConnectedAsync();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IBrokerServiceChannel : BrokerClient.brokerService.IBrokerService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class BrokerServiceClient : System.ServiceModel.ClientBase<BrokerClient.brokerService.IBrokerService>, BrokerClient.brokerService.IBrokerService {
        
        public BrokerServiceClient() {
        }
        
        public BrokerServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public BrokerServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public BrokerServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public BrokerServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
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
        
        public void OPCCreateClient(string opcIP, bool securityCheck) {
            base.Channel.OPCCreateClient(opcIP, securityCheck);
        }
        
        public System.Threading.Tasks.Task OPCCreateClientAsync(string opcIP, bool securityCheck) {
            return base.Channel.OPCCreateClientAsync(opcIP, securityCheck);
        }
        
        public void Connect(Opc.Ua.ConfiguredEndpoint endpoint) {
            base.Channel.Connect(endpoint);
        }
        
        public System.Threading.Tasks.Task ConnectAsync(Opc.Ua.ConfiguredEndpoint endpoint) {
            return base.Channel.ConnectAsync(endpoint);
        }
        
        public Opc.Ua.Configuration.ApplicationInstance GetApplicationInstance() {
            return base.Channel.GetApplicationInstance();
        }
        
        public System.Threading.Tasks.Task<Opc.Ua.Configuration.ApplicationInstance> GetApplicationInstanceAsync() {
            return base.Channel.GetApplicationInstanceAsync();
        }
        
        public Opc.Ua.ConfiguredEndpointCollection GetEndpoints() {
            return base.Channel.GetEndpoints();
        }
        
        public System.Threading.Tasks.Task<Opc.Ua.ConfiguredEndpointCollection> GetEndpointsAsync() {
            return base.Channel.GetEndpointsAsync();
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
        
        public bool CheckConnected() {
            return base.Channel.CheckConnected();
        }
        
        public System.Threading.Tasks.Task<bool> CheckConnectedAsync() {
            return base.Channel.CheckConnectedAsync();
        }
    }
}
