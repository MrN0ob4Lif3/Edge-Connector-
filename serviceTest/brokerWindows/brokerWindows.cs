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
using Opc.Ua.Client;
using System.Security.Cryptography.X509Certificates;

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
        private ConfiguredEndpoint m_endpoint;
        public ConfiguredEndpointCollection m_endpoints;
        private ServiceMessageContext m_messageContext;
        public Session m_session;
        public Browser m_browser;
        private CertificateValidationEventHandler m_CertificateValidation;

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
                Connect(m_endpoint);
                /*
                try
                {
                    //Find best endpoint
                    EndpointDescription endpointDescription = CoreClientUtils.SelectEndpoint(brokerIP, false);
                    EndpointConfiguration endpointConfiguration = EndpointConfiguration.Create(m_configuration);
                    ConfiguredEndpoint endpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration);
                    //Create Session
                    m_session = await Session.Create(
                        m_configuration,
                        endpoint,
                        false,
                        !DisableDomainCheck,
                        (String.IsNullOrEmpty(SessionName)) ? m_configuration.ApplicationName : SessionName,
                        60000,
                        UserIdentity,
                        PreferredLocales);
                    //Keep session alive 
                    m_session.KeepAlive += new KeepAliveEventHandler(Session_KeepAlive);
                }
                catch (Exception e)
                {
                    // Create an EventLog instance and assign its source.
                    EventLog myLog = new EventLog
                    {
                        Source = "brokerServiceOPCClient"
                    };
                    // Write an informational entry to the event log.
                    myLog.WriteEntry(e.Message);
                }
                */

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
        /// Creates a session with the endpoint.
        /// </summary>
        public async void Connect(ConfiguredEndpoint endpoint)
        {
            if (endpoint == null) throw new ArgumentNullException("endpoint");

            m_endpoint = endpoint;

            // copy the message context.
            m_messageContext = m_configuration.CreateMessageContext();


            X509Certificate2 clientCertificate = null;
            X509Certificate2Collection clientCertificateChain = null;

            if (endpoint.Description.SecurityPolicyUri != SecurityPolicies.None)
            {
                if (m_configuration.SecurityConfiguration.ApplicationCertificate == null)
                {
                    throw ServiceResultException.Create(StatusCodes.BadConfigurationError, "ApplicationCertificate must be specified.");
                }

                clientCertificate = await m_configuration.SecurityConfiguration.ApplicationCertificate.Find(true);

                if (clientCertificate == null)
                {
                    throw ServiceResultException.Create(StatusCodes.BadConfigurationError, "ApplicationCertificate cannot be found.");
                }

                // load certificate chain
                clientCertificateChain = new X509Certificate2Collection(clientCertificate);
                List<CertificateIdentifier> issuers = new List<CertificateIdentifier>();
                await m_configuration.CertificateValidator.GetIssuers(clientCertificate, issuers);
                for (int i = 0; i < issuers.Count; i++)
                {
                    clientCertificateChain.Add(issuers[i].Certificate);
                }
            }

            // create the channel.
            ITransportChannel channel = SessionChannel.Create(
                m_configuration,
                endpoint.Description,
                endpoint.Configuration,
                clientCertificate,
                m_configuration.SecurityConfiguration.SendCertificateChain ? clientCertificateChain : null,
                m_messageContext);

            // create the session.
            if (channel == null) throw new ArgumentNullException("channel");
            try
            {
                // create the session.
                Session session = new Session(channel, m_configuration, endpoint, null);
                session.ReturnDiagnostics = DiagnosticsMasks.All;

                // session now owns the channel.
                channel = null;

                // delete the existing session.
                // Close();

                // add session to tree.
                //AddNode(session);

                // Saves session instance in service.
                m_session = session;

                // Saves browser instance in service.
                m_browser = new Browser(session)
                {
                    Session = session,
                    BrowseDirection = BrowseDirection.Forward,
                    ReferenceTypeId = null,
                    IncludeSubtypes = true,
                    NodeClassMask = 0,
                    ContinueUntilDone = false
                };

            }
            finally
            {
                // ensure the channel is closed on error.
                if (channel != null)
                {
                    channel.Close();
                }
            }
        }



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

        //Callback to return OPC Session
        Session IServiceCallback.OPCSession()
        {
            return m_session;
        }
        #endregion



        #region OPC Session Creation Methods
        /// <summary>
        /// Gets or sets a flag indicating that the domain checks should be ignored when connecting.
        /// </summary>
        public bool DisableDomainCheck { get; set; }

        /// <summary>
        /// The name of the session to create.
        /// </summary>
        public string SessionName { get; set; }

        /// <summary>
        /// The user identity to use when creating the session.
        /// </summary>
        public IUserIdentity UserIdentity { get; set; }

        /// <summary>
        /// The client application configuration.
        /// </summary>
        public ApplicationConfiguration Configuration
        {
            get { return m_configuration; }

            set
            {
                if (!Object.ReferenceEquals(m_configuration, value))
                {
                    if (m_configuration != null)
                    {
                        m_configuration.CertificateValidator.CertificateValidation -= m_CertificateValidation;
                    }

                    m_configuration = value;

                    if (m_configuration != null)
                    {
                        m_configuration.CertificateValidator.CertificateValidation += m_CertificateValidation;
                    }
                }
            }
        }

        /// <summary>
        /// The locales to use when creating the session.
        /// </summary>
        public string[] PreferredLocales { get; set; }
        #endregion
    }
}
