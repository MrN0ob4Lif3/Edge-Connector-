﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MQTTClientForm.brokerService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="CompositeType", Namespace="http://schemas.datacontract.org/2004/07/brokerService")]
    [System.SerializableAttribute()]
    public partial class CompositeType : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool BoolValueField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string StringValueField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool BoolValue {
            get {
                return this.BoolValueField;
            }
            set {
                if ((this.BoolValueField.Equals(value) != true)) {
                    this.BoolValueField = value;
                    this.RaisePropertyChanged("BoolValue");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string StringValue {
            get {
                return this.StringValueField;
            }
            set {
                if ((object.ReferenceEquals(this.StringValueField, value) != true)) {
                    this.StringValueField = value;
                    this.RaisePropertyChanged("StringValue");
                }
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
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="brokerService.IbrokerService")]
    public interface IbrokerService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IbrokerService/GetData", ReplyAction="http://tempuri.org/IbrokerService/GetDataResponse")]
        string GetData(string value);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IbrokerService/GetData", ReplyAction="http://tempuri.org/IbrokerService/GetDataResponse")]
        System.Threading.Tasks.Task<string> GetDataAsync(string value);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IbrokerService/GetDataUsingDataContract", ReplyAction="http://tempuri.org/IbrokerService/GetDataUsingDataContractResponse")]
        MQTTClientForm.brokerService.CompositeType GetDataUsingDataContract(MQTTClientForm.brokerService.CompositeType composite);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IbrokerService/GetDataUsingDataContract", ReplyAction="http://tempuri.org/IbrokerService/GetDataUsingDataContractResponse")]
        System.Threading.Tasks.Task<MQTTClientForm.brokerService.CompositeType> GetDataUsingDataContractAsync(MQTTClientForm.brokerService.CompositeType composite);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IbrokerService/CreateClientAsync", ReplyAction="http://tempuri.org/IbrokerService/CreateClientAsyncResponse")]
        void CreateClientAsync(string brokerIP, int option);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IbrokerService/CreateClientAsync", ReplyAction="http://tempuri.org/IbrokerService/CreateClientAsyncResponse")]
        System.Threading.Tasks.Task CreateClientAsyncAsync(string brokerIP, int option);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IbrokerServiceChannel : MQTTClientForm.brokerService.IbrokerService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class IbrokerServiceClient : System.ServiceModel.ClientBase<MQTTClientForm.brokerService.IbrokerService>, MQTTClientForm.brokerService.IbrokerService {
        
        public IbrokerServiceClient() {
        }
        
        public IbrokerServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public IbrokerServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public IbrokerServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public IbrokerServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public string GetData(string value) {
            return base.Channel.GetData(value);
        }
        
        public System.Threading.Tasks.Task<string> GetDataAsync(string value) {
            return base.Channel.GetDataAsync(value);
        }
        
        public MQTTClientForm.brokerService.CompositeType GetDataUsingDataContract(MQTTClientForm.brokerService.CompositeType composite) {
            return base.Channel.GetDataUsingDataContract(composite);
        }
        
        public System.Threading.Tasks.Task<MQTTClientForm.brokerService.CompositeType> GetDataUsingDataContractAsync(MQTTClientForm.brokerService.CompositeType composite) {
            return base.Channel.GetDataUsingDataContractAsync(composite);
        }
        
        public void CreateClientAsync(string brokerIP, int option) {
            base.Channel.CreateClientAsync(brokerIP, option);
        }
        
        public System.Threading.Tasks.Task CreateClientAsyncAsync(string brokerIP, int option) {
            return base.Channel.CreateClientAsyncAsync(brokerIP, option);
        }
    }
}
