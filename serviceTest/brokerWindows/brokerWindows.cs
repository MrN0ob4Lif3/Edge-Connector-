using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.ServiceModel;
using System.Threading;
using System.ServiceModel.Description;
using MQTTnet.Extensions.ManagedClient;
using ServiceLogic;
using System.Collections.Generic;
using Opc.Ua.Configuration;
using Opc.Ua;
using Opc.Ua.Client.Controls;
using System.Threading.Tasks;

namespace brokerWindows
{
    public partial class brokerWindows : ServiceBase, IServiceCallback
    {
        ServiceHost host;

        #region MQTT Properties
        public IManagedMqttClient managedMqtt;
        public HashSet<String> m_topicSet = new HashSet<string>();
        public List<String> m_topicList = new List<string>();
        static SemaphoreSlim mqttClientSemaphore = new SemaphoreSlim(1, 1);
        #endregion

        #region OPC Properties
        public ApplicationInstance application;
        public ApplicationConfiguration m_configuration;
        public ApplicationConfiguration app_configuration;
        public ConfiguredEndpointCollection m_endpoints;
        #endregion

        public brokerWindows()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {   
            try
            {
                //Set the static callback reference and creates interface for WinForms by hosting a WCF Service.
                Host.Current = this;
                StartBroker();

                // Log an event to indicate successful start.
                EventLog.WriteEntry("Successful start.", EventLogEntryType.Information);

                //Initialize MQTT Client.
                managedMqtt= ManagedClient.CreateManagedClient();
                //string brokerIP = "dev-harmony-01.southeastasia.cloudapp.azure.com:8080/mqtt";
                string brokerIP = "localhost";
                MQTTConnectClient(managedMqtt,brokerIP);

                //Initialize OPC Application Instance
                application = new ApplicationInstance
                {
                    ApplicationName = "MQTT-OPC Broker",
                    ApplicationType = ApplicationType.ClientAndServer,
                    ConfigSectionName = "Opc.Ua.SampleClient"
                };
                //load the application configuration.
                application.LoadApplicationConfiguration(false).Wait();
                // check the application certificate.
                application.CheckApplicationInstanceCertificate(false, 0).Wait();
                m_configuration = app_configuration = application.ApplicationConfiguration;
                // get list of cached endpoints.
                m_endpoints = m_configuration.LoadCachedEndpoints(true);
                m_endpoints.DiscoveryUrls = app_configuration.ClientConfiguration.WellKnownDiscoveryUrls;
                if (!app_configuration.SecurityConfiguration.AutoAcceptUntrustedCertificates)
                {
                    app_configuration.CertificateValidator.CertificateValidation += new CertificateValidationEventHandler(CertificateValidator_CertificateValidation);
                }
            }
            catch (Exception ex)
            {
                // Log the exception.
                EventLog.WriteEntry(ex.Message, EventLogEntryType.Error);
            }                 
        }

        protected override void OnStop()
        {
            host.Close();
        }

        private void StartBroker()
        {
            host = new ServiceHost(typeof(brokerService.BrokerService));
            ServiceDebugBehavior debug = host.Description.Behaviors.Find<ServiceDebugBehavior>();
            // if not found - add behavior with setting turned on 
            if (debug == null)
            {
                host.Description.Behaviors.Add(
                     new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });
            }
            else
            {
                // make sure setting is turned ON
                if (!debug.IncludeExceptionDetailInFaults)
                {
                    debug.IncludeExceptionDetailInFaults = true;
                }
            }
            host.Open();
        }

        #region MQTT Methods
        public async void MQTTConnectClient(IManagedMqttClient managedMqtt,string brokerIP)
        {
            await ManagedClient.ManagedMqttConnectTCPAsync(managedMqtt, brokerIP);
            //await ManagedClient.ManagedMqttConnectWebSocket(managedMqtt, brokerIP);
        }

        //Callback for MQTT connection.
        async void IServiceCallback.MQTTConnect(string brokerIP)
        {
            try
            {
                managedMqtt = ManagedClient.CreateManagedClient();
                await ManagedClient.ManagedMqttConnectWebSocket(managedMqtt, brokerIP);
            }
            finally
            {
                mqttClientSemaphore.Release();
            }
        }

        //Callback for MQTT subscription.
        async void IServiceCallback.MQTTSubscribe(String topic)
        {
            await mqttClientSemaphore.WaitAsync();
            try
            {
                if(m_topicSet.Contains(topic))
                {
                    return;
                }
                else
                {
                    ManagedClient.ManagedMqttSubscribe(managedMqtt, topic);
                    m_topicSet.Add(topic);
                }
            }
            finally
            {
                mqttClientSemaphore.Release();
            }
        }

        //Callback for MQTT unsubscription.
        async void IServiceCallback.MQTTUnsubscribe(String topic)
        {
            await mqttClientSemaphore.WaitAsync();
            try
            {
                ManagedClient.ManagedMqttUnsubscribe(managedMqtt, topic);
                m_topicSet.Remove(topic);
            }
            finally
            {
                mqttClientSemaphore.Release();
            }
        }

        //Callback for MQTT publishing.
        async void IServiceCallback.MQTTPublish(string topic, string message)
        {
            await mqttClientSemaphore.WaitAsync();
            try
            {
                await ManagedClient.ManagedMqttPublish(managedMqtt, topic, message);

            }
            finally
            {
                mqttClientSemaphore.Release();
            }
        }

        //Callback to return MQTT subscribed topics.
        HashSet<String> IServiceCallback.MQTTSubscribedTopics()
        {
            return m_topicSet;
        }
        #endregion

        #region OPC Methods
        /// <summary>
        /// Handles a certificate validation error.
        /// </summary>
        private void CertificateValidator_CertificateValidation(CertificateValidator sender, CertificateValidationEventArgs e)
        {
            try
            {
                e.Accept = m_configuration.SecurityConfiguration.AutoAcceptUntrustedCertificates;
                /*
                if (!m_configuration.SecurityConfiguration.AutoAcceptUntrustedCertificates)
                {
                    DialogResult result = MessageBox.Show(
                        e.Certificate.Subject,
                        "Untrusted Certificate",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    e.Accept = (result == DialogResult.Yes);
                }*/
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException("Certificate Validation", exception);
            }
        }
        
        //Callback to return OPC Application Instance
        ApplicationInstance IServiceCallback.OPCApplicationInstance()
        {
            return application;
        }

        //Callback to return OPC Endpoints
        ConfiguredEndpointCollection IServiceCallback.OPCEndpoints()
        {
            return m_endpoints;
        }
        #endregion
    }
}
