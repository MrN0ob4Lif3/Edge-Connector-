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
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

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
        public EndpointConfiguration m_endpoint_configuration;
        public SessionReconnectHandler m_reconnectHandler;
        public const int reconnectPeriod = 10;
        private ServiceMessageContext m_messageContext;
        public Session m_session;
        public Browser m_browser;
        private CertificateValidationEventHandler m_CertificateValidation;
        static bool autoAccept = true;
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
                //MQTT Broker connection
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
                m_CertificateValidation = new CertificateValidationEventHandler(CertificateValidator_CertificateValidation);

                //OPC Server connection.
                string endpointURL = "opc.tcp://opcua.rocks:4840";
                //OPCConnect(application, endpointURL);
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

        #region Callback methods
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
        #endregion

        #region OPC Methods
        /// <summary>
        /// Creates a session with the endpoint.
        /// </summary>
        /// 
        public async void OPCConnect(ApplicationInstance application, string endpointURL, string filePath)
        {
            // load the application configuration.
            ApplicationConfiguration config = await application.LoadApplicationConfiguration(false);
            // check the application certificate.
            bool haveAppCertificate = await application.CheckApplicationInstanceCertificate(false, 0);
            if (!haveAppCertificate)
            {
                throw new Exception("Application instance certificate invalid!");
            }

            if (haveAppCertificate)
            {
                config.ApplicationUri = Utils.GetApplicationUriFromCertificate(config.SecurityConfiguration.ApplicationCertificate.Certificate);
                if (config.SecurityConfiguration.AutoAcceptUntrustedCertificates)
                {
                    autoAccept = true;
                }
                config.CertificateValidator.CertificateValidation += new CertificateValidationEventHandler(CertificateValidator_CertificateValidation);
            }
            else
            {
                Console.WriteLine("    WARN: missing application certificate, using unsecure connection.");
            }

            //Select endpoint after discovery.
            EndpointDescription selectedEndpoint = CoreClientUtils.SelectEndpoint(endpointURL, haveAppCertificate, 15000);
            //Creates session with OPC Server
            m_endpoint_configuration = EndpointConfiguration.Create(config);
            m_endpoint = new ConfiguredEndpoint(null, selectedEndpoint, m_endpoint_configuration);
            m_session = await Session.Create(config, m_endpoint, false, "OPCEdge", 60000, new UserIdentity(new AnonymousIdentityToken()), null);
            //Register keep alive handler
            m_session.KeepAlive += Client_KeepAlive;
            try
            {
                string Sessions = Path.Combine(@"C:\Users\Andrew\Documents\SITUofGFYP-AY1920", string.Format(@"Retained Sessions\{0}.json", m_session.SessionName));
                string SessionsFolder = Path.Combine(@"C:\Users\Andrew\Documents\SITUofGFYP-AY1920", (@"Retained Sessions"));
                Directory.CreateDirectory(SessionsFolder);
                //Stores edited subscription values in 'Retained Subscriptions' folder.
                foreach (string session in Directory.GetFiles(SessionsFolder, "*.json"))
                {
                    String sessionDetails = File.ReadAllText(session);
                    if (sessionDetails.Contains(m_session.SessionName))
                    {
                        File.Delete(session);
                        string modifiedSession = Path.Combine(@"C:\Users\Andrew\Documents\SITUofGFYP-AY1920", string.Format(@"Retained Sessions\{0}.json", m_session.SessionName));
                        File.AppendAllText(modifiedSession, JsonConvert.SerializeObject(m_session) + "\n");
                        break;
                    }
                }
                File.AppendAllText(Sessions, JsonConvert.SerializeObject(m_session) + "\n");
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
        }

        /// <summary>
        /// Handles a certificate validation error.
        /// </summary>
        private void CertificateValidator_CertificateValidation(CertificateValidator sender, CertificateValidationEventArgs e)
        {
            try
            {
                if (e.Error.StatusCode == StatusCodes.BadCertificateUntrusted)
                {
                    e.Accept = autoAccept;
                    if (autoAccept)
                    {
                        Console.WriteLine("Accepted Certificate: {0}", e.Certificate.Subject);
                    }
                    else
                    {
                        Console.WriteLine("Rejected Certificate: {0}", e.Certificate.Subject);
                    }
                }
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException("Certificate Validation", exception);
            }
        }

        #region Callback methods
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

        //Callback to connect to OPC endpoint
        void IServiceCallback.OPCConnect(String opcEndpoint, String filePath)
        {
            OPCConnect(this.application, opcEndpoint, filePath);
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

        #region Client Keep Alive / Reconnect handlers
        private void Client_KeepAlive(Session sender, KeepAliveEventArgs e)
        {
            if (e.Status != null && ServiceResult.IsNotGood(e.Status))
            {
                Console.WriteLine("{0} {1}/{2}", e.Status, sender.OutstandingRequestCount, sender.DefunctRequestCount);

                if (m_reconnectHandler == null)
                {
                    Console.WriteLine("--- RECONNECTING ---");
                    m_reconnectHandler = new SessionReconnectHandler();
                    m_reconnectHandler.BeginReconnect(sender, reconnectPeriod * 1000, Client_ReconnectComplete);
                }
            }
        }

        private void Client_ReconnectComplete(object sender, EventArgs e)
        {
            // ignore callbacks from discarded objects.
            if (!Object.ReferenceEquals(sender, m_reconnectHandler))
            {
                return;
            }

            m_session = m_reconnectHandler.Session;
            m_reconnectHandler.Dispose();
            m_reconnectHandler = null;

            Console.WriteLine("--- RECONNECTED ---");
        }
        #endregion
        #endregion
    }
}


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

/*
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
        }
    }
    catch (Exception exception)
    {
        ClientUtils.HandleException("Certificate Validation", exception);
    }
}
*/