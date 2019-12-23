using System;
using MQTTnet.Extensions.ManagedClient;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Client.Controls;
using System.Diagnostics;
using Opc.Ua.Configuration;
using Opc.Ua.Sample.Controls;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;

namespace brokerService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class BrokerService : IBrokerService
    {
        #region MQTT Variables
        //Create a new ManagedMQTT Client.
        public IManagedMqttClient managedMQTT;
        public List<String> m_topicList;
        #endregion

        #region OPC Variables
        //OPC Client variables
        private ApplicationInstance application;
        private ApplicationConfiguration m_configuration;
        private ApplicationConfiguration app_configuration;
        private ConfiguredEndpointCollection m_endpoints;
        private ConfiguredEndpoint m_endpoint;
        private ServiceMessageContext m_messageContext;
        private Session m_session;
        private Browser m_browser;
        private int m_reconnectPeriod = 10;
        private int m_discoverTimeout = 5000;
        private SessionReconnectHandler m_reconnectHandler;
        private CertificateValidationEventHandler m_CertificateValidation;
        private EventHandler m_ReconnectComplete;
        private EventHandler m_ReconnectStarting;
        private EventHandler m_KeepAliveComplete;
        private EventHandler m_ConnectComplete;
        private Subscription m_subscription;
        private MonitoredItem m_monitoredItem;
        private bool m_connectedOnce;
        #endregion

        BrokerService()
        {
            //MQTT Client creation & connection
            managedMQTT = ServiceLogic.ManagedClient.CreateManagedClient();
            //MQTTConnectClientAsync("dev-harmony-01.southeastasia.cloudapp.azure.com:8080/mqtt", 1);
            MQTTConnectClientAsync("localhost", 0);
            MQTTSubscribeTopic("TestTopic");
            MQTTPublishTopicAsync("TestTopic", "TestMessage");

            //OPC-UA Client creation & connection
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

        #region MQTT Methods / Properties
        //MQTT client connection function. Requires IP address of MQTT server and connection option type
        public async void MQTTConnectClientAsync(String mqttIP, int option)
        {
            if (option == 0)
            {
                try
                {
                    // Use TCP connection.
                    managedMQTT = ServiceLogic.ManagedClient.CreateManagedClient();
                    await ServiceLogic.ManagedClient.ManagedMqttConnectTCPAsync(managedMQTT, mqttIP);
                }
                catch (Exception e)
                {
                    // Create an EventLog instance and assign its source.
                    EventLog myLog = new EventLog
                    {
                        Source = "brokerServiceMQTTTCP"
                    };
                    // Write an informational entry to the event log.
                    myLog.WriteEntry(e.Message);
                }
            }
            else if (option == 1)
            {
                try
                {
                    // Use WebSocket connection.
                    managedMQTT = ServiceLogic.ManagedClient.CreateManagedClient();
                    await ServiceLogic.ManagedClient.ManagedMqttConnectWebSocket(managedMQTT, mqttIP);
                }

                catch (Exception e)
                {
                    // Create an EventLog instance and assign its source.
                    EventLog myLog = new EventLog
                    {
                        Source = "brokerServiceMQTTWebSocket"
                    };
                    // Write an informational entry to the event log.
                    myLog.WriteEntry(e.Message);
                }
            }
        }
        //MQTT topic subscription function. Requires topic to subscribe to.
        public void MQTTSubscribeTopic(String topic)
        {
            try
            {
                ServiceLogic.ManagedClient.ManagedMqttSubscribe(managedMQTT, topic);
                m_topicList.Add(topic);
            }
            catch (Exception e)
            {
                // Create an EventLog instance and assign its source.
                EventLog myLog = new EventLog
                {
                    Source = "brokerServiceMQTTSubscribe"
                };
                // Write an informational entry to the event log.
                myLog.WriteEntry(e.Message);
            }
        }
        //MQTT topic unsubscription function. Requires topic to subscribe to.
        public void MQTTUnsubscribeTopic(String topic)
        {
            try
            {
                ServiceLogic.ManagedClient.ManagedMqttUnsubscribe(managedMQTT, topic);
                m_topicList.Remove(topic);
            }
            catch (Exception e)
            {
                // Create an EventLog instance and assign its source.
                EventLog myLog = new EventLog
                {
                    Source = "brokerServiceMQTTSubscribe"
                };
                // Write an informational entry to the event log.
                myLog.WriteEntry(e.Message);
            }
        }
        //MQTT message publishing function. Requires topic to publish message to.
        public async void MQTTPublishTopicAsync(String topic, String message)
        {
            try
            {
                await ServiceLogic.ManagedClient.ManagedMqttPublish(managedMQTT, topic, message);
            }
            catch (Exception e)
            {
                // Create an EventLog instance and assign its source.
                EventLog myLog = new EventLog
                {
                    Source = "brokerServiceMQTTPublish"
                };
                // Write an informational entry to the event log.
                myLog.WriteEntry(e.Message);
            }
        }
        //Returns list of subscribed topics for MQTT client.
        public List<String> MQTTSubscribedTopics()
        {
            return m_topicList;
        }
        #endregion

        #region OPC Methods
        //OPC Client / Session creation.
        public void OPCCreateClient(String opcIP, bool securityCheck)
        {
            application.ApplicationName = "MQTT-OPC Broker";
            application.ApplicationType = ApplicationType.ClientAndServer;
            application.ConfigSectionName = "Opc.Ua.SampleClient";
            // load the application configuration.
            application.LoadApplicationConfiguration(false).Wait();
            // check the application certificate.
            application.CheckApplicationInstanceCertificate(false, 0).Wait();
            m_configuration = application.ApplicationConfiguration;

            if (m_configuration == null)
            {
                throw new ArgumentNullException("m_configuration");
            }

            m_CertificateValidation = new CertificateValidationEventHandler(CertificateValidator_CertificateValidation);

            /*try
            {
                //Find best endpoint
                EndpointDescription endpointDescription = CoreClientUtils.SelectEndpoint(opcIP, securityCheck, m_discoverTimeout);
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
        #endregion

        public void OPCConnectClient()
        {

        }

        //OPC client connection.
        public void OPCConnectClient(ConfiguredEndpoint endpoint, SessionTreeCtrl opcSession, Opc.Ua.Sample.Controls.BrowseTreeCtrl opcBrowse)
        {
            try
            {
                //Connect(endpoint, opcSession, opcBrowse);
            }
            catch (Exception e)
            {

            }
        }
        //OPC topic subscription.
        public void OPCSubscribeTopic()
        {
            //subscribe to specified data value
            if (m_session == null)
            {
                return;
            }
        }
        //OPC topic unsubscription.
        public void OPCUnsubscribeTopic()
        {
            //unsubscribe from specified data value
        }
        #endregion

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


        public ApplicationInstance GetApplicationInstance()
        {
            return application;
        }

        public ConfiguredEndpointCollection GetEndpoints()
        {
            return m_endpoints;
        }

        public Session GetSession()
        {
            try
            {
                //SessionSurrogate sessionSurrogate = new SessionSurrogate();
                //sessionSurrogate.OPCSession = m_session;
                //return sessionSurrogate;
                return m_session;
            }
            catch (Exception e)
            {
                // Create an EventLog instance and assign its source.
                EventLog myLog = new EventLog
                {
                    Source = "brokerServiceSession"
                };
                // Write an informational entry to the event log.
                myLog.WriteEntry(e.Message);
                return null;
            }

        }

        public Browser GetBrowser()
        {
            return m_browser;
        }

        #region Session alive / re-connect
        /// <summary>
        /// Handles a keep alive event from a session.
        /// </summary> 
        private void Session_KeepAlive(Session session, KeepAliveEventArgs e)
        {
            try
            {
                // check for events from discarded sessions.
                if (!Object.ReferenceEquals(session, m_session))
                {
                    return;
                }
                // start reconnect sequence on communication error.
                if (ServiceResult.IsBad(e.Status))
                {
                    if (m_reconnectPeriod <= 0)
                    {
                        //UpdateStatus(true, e.CurrentTime, "Communication Error ({0})", e.Status);
                        return;
                    }
                    //UpdateStatus(true, e.CurrentTime, "Reconnecting in {0}s", m_reconnectPeriod);
                    if (m_reconnectHandler == null)
                    {
                        m_ReconnectStarting?.Invoke(this, e);
                        m_reconnectHandler = new SessionReconnectHandler();
                        m_reconnectHandler.BeginReconnect(m_session, m_reconnectPeriod * 1000, Server_ReconnectComplete);
                    }

                    return;
                }
                // update status.
                //UpdateStatus(false, e.CurrentTime, "Connected [{0}]", session.Endpoint.EndpointUrl);

                // raise any additional notifications.
                m_KeepAliveComplete?.Invoke(this, e);
            }
            catch (Exception exception)
            {
                //ClientUtils.HandleException(this.Text, exception);
                ClientUtils.HandleException("reconnect", exception);
            }
        }

        /// <summary>
        /// Updates the status control when a keep alive event occurs.
        /// </summary> 
        /*
        void StandardClient_KeepAlive(Session sender, KeepAliveEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new KeepAliveEventHandler(StandardClient_KeepAlive), sender, e);
                return;
            }
            else if (!IsHandleCreated)
            {
                return;
            }

            if (sender != null && sender.Endpoint != null)
            {
                ServerUrlLB.Text = Utils.Format(
                    "{0} ({1}) {2}",
                    sender.Endpoint.EndpointUrl,
                    sender.Endpoint.SecurityMode,
                    (sender.EndpointConfiguration.UseBinaryEncoding) ? "UABinary" : "XML");
            }
            else
            {
                ServerUrlLB.Text = "None";
            }

            if (e != null && m_session != null)
            {
                if (ServiceResult.IsGood(e.Status))
                {
                    ServerStatusLB.Text = Utils.Format(
                        "Server Status: {0} {1:yyyy-MM-dd HH:mm:ss} {2}/{3}",
                        e.CurrentState,
                        e.CurrentTime.ToLocalTime(),
                        m_session.OutstandingRequestCount,
                        m_session.DefunctRequestCount);

                    ServerStatusLB.ForeColor = Color.Empty;
                    ServerStatusLB.Font = new Font(ServerStatusLB.Font, FontStyle.Regular);
                }
                else
                {
                    ServerStatusLB.Text = String.Format(
                        "{0} {1}/{2}", e.Status,
                        m_session.OutstandingRequestCount,
                        m_session.DefunctRequestCount);

                    ServerStatusLB.ForeColor = Color.Red;
                    ServerStatusLB.Font = new Font(ServerStatusLB.Font, FontStyle.Bold);

                    if (m_reconnectPeriod <= 0)
                    {
                        return;
                    }

                    if (m_reconnectHandler == null && m_reconnectPeriod > 0)
                    {
                        m_reconnectHandler = new SessionReconnectHandler();
                        m_reconnectHandler.BeginReconnect(m_session, m_reconnectPeriod * 1000, StandardClient_Server_ReconnectComplete);
                    }
                }
            }
        }
        */
        /// <summary>
        /// Handles a reconnect event complete from the reconnect handler.
        /// </summary>
        private void Server_ReconnectComplete(object sender, EventArgs e)
        {
            try
            {
                // ignore callbacks from discarded objects.
                if (!Object.ReferenceEquals(sender, m_reconnectHandler))
                {
                    return;
                }

                m_session = m_reconnectHandler.Session;
                m_reconnectHandler.Dispose();
                m_reconnectHandler = null;

                // raise any additional notifications.
                m_ReconnectComplete?.Invoke(this, e);
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException("reconnect", exception);
            }
        }
        #endregion

    }
}
