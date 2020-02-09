using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.ServiceModel;
using System.Threading;
using System.ServiceModel.Description;
using MQTTnet.Extensions.ManagedClient;
using Opc.Ua.ServiceLogic;
using System.Collections.Generic;
using Opc.Ua.Configuration;
using Opc.Ua;
using Opc.Ua.Client.Controls;
using System.Threading.Tasks;
using Opc.Ua.Client;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using Newtonsoft.Json;

namespace OpcWindowsService
{
    public partial class OpcWindowsService : ServiceBase, IServiceCallback
    {
        #region Service Properties
        ServiceHost host;
        static string tempFolder = @"C:\Users\Andrew\Documents\SITUofGFYP-AY1920";
        string itemsFolder = Path.Combine(tempFolder, @"Retained Monitored Items");
        string subscriptionsFolder = Path.Combine(tempFolder, @"Retained Subscriptions");
        string sessionFolder = Path.Combine(tempFolder, @"Retained Sessions");
        bool serviceCheck = false;
        #endregion  

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
        System.Threading.Timer publishTimer;
        public static DateTime subscriptionLastModified;
        public static DateTime itemLastModified;
        #endregion

        public OpcWindowsService()
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
                managedMqtt = Mqtt.CreateManagedClient();
                //MQTT Broker connection
                //string brokerIP = "localhost";
                string brokerIP = "dev-harmony-01.southeastasia.cloudapp.azure.com:8080/mqtt";
                MQTTConnectClient(managedMqtt, brokerIP);

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
                //m_CertificateValidation = new CertificateValidationEventHandler(CertificateValidator_CertificateValidation);

                //If there was a pre-exising session before, attempt reconnection.
                foreach(string sessionFile in Directory.GetFiles(sessionFolder, "*.json"))
                {
                    String readSession = File.ReadAllText(sessionFile);
                    if(readSession != "")
                    {
                        try
                        {
                            OPCConnect(application, readSession);
                        }
                        catch (Exception ex)
                        {
                            // Log the exception.
                            EventLog.WriteEntry(ex.Message, EventLogEntryType.Error);
                        }
                        break;
                    }
                }
                serviceCheck = true;
                //Use timer callback to monitor and publish subscriptions.
                publishTimer = new System.Threading.Timer(x => OPCPublish(m_session), null, 5000, 1000);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(ex.Message, EventLogEntryType.Error);
            }
        }

        protected override void OnStop()
        {
            host.Close();
        }

        private void StartBroker()
        {
            host = new ServiceHost(typeof(OpcWCFInterface.OpcWCFInterface));
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
        public async void MQTTConnectClient(IManagedMqttClient managedMqtt, string brokerIP)
        {
            //await ManagedClient.ManagedMqttConnectTCPAsync(managedMqtt, brokerIP);
            await Mqtt.ManagedMqttConnectWebSocket(managedMqtt, brokerIP);
        }

        #region Callback methods
        //Callback for MQTT connection.
        async void IServiceCallback.MQTTConnect(string brokerIP)
        {
            try
            {
                managedMqtt = Mqtt.CreateManagedClient();
                await Mqtt.ManagedMqttConnectWebSocket(managedMqtt, brokerIP);
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
                if (m_topicSet.Contains(topic))
                {
                    return;
                }
                else
                {
                    Mqtt.ManagedMqttSubscribe(managedMqtt, topic);
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
                Mqtt.ManagedMqttUnsubscribe(managedMqtt, topic);
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
                await Mqtt.ManagedMqttPublish(managedMqtt, topic, message);

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
        public async void OPCConnect(ApplicationInstance application, string endpointURL)
        {
            // load the application configuration.
            ApplicationConfiguration config = await application.LoadApplicationConfiguration(false);
            // check the application certificate.
            bool haveAppCertificate = await application.CheckApplicationInstanceCertificate(false, 0);
            if (!haveAppCertificate)
            {
                throw new Exception("Application instance certificate invalid!");
            }
            else if (haveAppCertificate)
            {
                config.ApplicationUri = Utils.GetApplicationUriFromCertificate(config.SecurityConfiguration.ApplicationCertificate.Certificate);
                if (config.SecurityConfiguration.AutoAcceptUntrustedCertificates)
                {
                    autoAccept = true;
                }
                config.CertificateValidator.CertificateValidation += new CertificateValidationEventHandler(CertificateValidator_CertificateValidation);
            }

            //Checks if endpoint URL selected is same as retained endpoint URL
            String retainedEndpoint = null;
            String sessionEndpoint = endpointURL;
            retainedEndpoint = await LoadSessionAsync(sessionEndpoint);

            try
            {
                EndpointDescription selectedEndpoint;
                if (retainedEndpoint == null)
                {
                    selectedEndpoint = CoreClientUtils.SelectEndpoint(sessionEndpoint, haveAppCertificate, 15000);
                }
                else
                {
                    selectedEndpoint = CoreClientUtils.SelectEndpoint(retainedEndpoint, haveAppCertificate, 15000);
                }
                //Select endpoint after discovery.
                //Creates session with OPC Server
                m_endpoint_configuration = EndpointConfiguration.Create(config);
                m_endpoint = new ConfiguredEndpoint(null, selectedEndpoint, m_endpoint_configuration);
                m_session = await Session.Create(config, m_endpoint, false, "OPCEdge", 60000, new UserIdentity(new AnonymousIdentityToken()), null);
            }
            catch (Exception ex)
            {
                // Log the exception.
                EventLog.WriteEntry(ex.Message, EventLogEntryType.Error);
            }

            //Recreates prior session's subscriptions and monitored items.
            if (sessionEndpoint == retainedEndpoint)
            {
                RecreateSession(m_session);
            }
            //Register keep alive handler
            m_session.KeepAlive += Client_KeepAlive;
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

        //Callback to connect to OPC endpoint
        void IServiceCallback.OPCConnect(String opcEndpoint)
        {
            OPCConnect(this.application, opcEndpoint);
        }

        //Callback to add subscription to session
        void IServiceCallback.OPCSubscribe(Subscription subscription)
        {
            //Recreates subscription based on provided subscription details.
            Subscription tempSubscription = new Subscription(m_session.DefaultSubscription);
            m_session.AddSubscription(tempSubscription);
            tempSubscription.DisplayName = subscription.DisplayName;
            tempSubscription.PublishingInterval = subscription.PublishingInterval;
            tempSubscription.KeepAliveCount = subscription.KeepAliveCount;
            tempSubscription.LifetimeCount = subscription.LifetimeCount;
            tempSubscription.MaxNotificationsPerPublish = subscription.MaxNotificationsPerPublish;
            tempSubscription.Priority = subscription.Priority;
            tempSubscription.PublishingEnabled = subscription.PublishingEnabled;
            tempSubscription.Create();
            Host.Current.MQTTSubscribe(tempSubscription.DisplayName);
        }

        //Callback to remove subscription from session
        void IServiceCallback.OPCUnsubscribe(Subscription subscription)
        {
            IEnumerable<Subscription> subscriptions = m_session.Subscriptions;
            foreach (Subscription sub in subscriptions)
            {
                if (sub.DisplayName == subscription.DisplayName)
                {
                    m_session.RemoveSubscription(sub);
                    break;
                }
            }
        }

        //Callback to add monitored item to subscription
        void IServiceCallback.OPCMonitor(Subscription subscription, MonitoredItem monitoredItem)
        {
            IEnumerable<Subscription> subscriptions = m_session.Subscriptions;
            foreach (Subscription sub in subscriptions)
            {
                if (sub.DisplayName == subscription.DisplayName)
                {
                    sub.AddItem(monitoredItem);
                    sub.ApplyChanges();
                    break;
                }
            }
        }

        //Callback to remove monitored item from subscription.
        void IServiceCallback.OPCUnmonitor(Subscription subscription, MonitoredItem monitoredItem)
        {
            IEnumerable<Subscription> subscriptions = m_session.Subscriptions;
            foreach (Subscription sub in subscriptions)
            {
                if (sub.DisplayName == subscription.DisplayName)
                {
                    sub.RemoveItem(monitoredItem);
                    sub.ApplyChanges();
                    break;
                }
            }
        }

        //Callback to have service disconnect and close session.
        void IServiceCallback.OPCDisconnect()
        {
            if (m_session != null)
            {
                if (m_session.Connected)
                {
                    m_session.Close();
                }
            }
        }

        //Callback to check if session is running in service and disconnects if connected.
        bool IServiceCallback.CheckConnected()
        {
            if(m_session != null)
            {
                if (m_session.Connected)
                {
                    return true;
                }
            }
            return false;
        }
        
        //Callback to check if service is running.
        bool IServiceCallback.CheckService()
        {
            return serviceCheck;
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

        #region Session State
        //Saves endpoint
        public Task SaveSessionAsync(Session session)
        {
            string sessionFile = Path.Combine(sessionFolder, string.Format(@"{0}.json", m_session.SessionName));

            if (File.Exists(sessionFile))
            {
                return Task.FromResult(0);
            }
            else
            {
                File.AppendAllText(sessionFile, JsonConvert.SerializeObject(session.Endpoint.EndpointUrl));
                return Task.FromResult(0);
            }
        }

        //Loads retained endpoint
        public Task<String> LoadSessionAsync(String sessionEndpoint)
        {
            String retainedSession = null;
            String[] sessionFiles = Directory.GetFiles(sessionFolder, "*.json");
            if (sessionFiles != null)
            {
                foreach (string session in sessionFiles)
                {
                    try
                    {
                        String[] sessionFile = File.ReadAllLines(session);
                        foreach (string retainedEndpoint in sessionFile)
                        {
                            retainedSession = JsonConvert.DeserializeObject<String>(retainedEndpoint);
                            if (retainedSession.Contains(sessionEndpoint))
                            {
                                return Task.FromResult(retainedSession);
                            }
                        }
                    }
                    catch
                    {

                    }
                }
            }
            else
            {
                return null;
            }
            return Task.FromResult(retainedSession);
        }

        //Session recreation method.
        public void RecreateSession(Session session)
        {
            //Recreating retained subscriptions.
            foreach (string sub in Directory.GetFiles(subscriptionsFolder, "*.json"))
            {
                String readSubscription = File.ReadAllText(sub);
                if (readSubscription != "")
                {
                    try
                    {
                        //Recreates subscription based on retained subscription details.
                        Subscription retainedSubscription;
                        Subscription tempSubscription = new Subscription(session.DefaultSubscription);
                        retainedSubscription = JsonConvert.DeserializeObject<Subscription>(readSubscription);
                        session.AddSubscription(tempSubscription);
                        tempSubscription.DisplayName = retainedSubscription.DisplayName;
                        tempSubscription.PublishingInterval = retainedSubscription.PublishingInterval;
                        tempSubscription.KeepAliveCount = retainedSubscription.KeepAliveCount;
                        tempSubscription.LifetimeCount = retainedSubscription.LifetimeCount;
                        tempSubscription.MaxNotificationsPerPublish = retainedSubscription.MaxNotificationsPerPublish;
                        tempSubscription.Priority = retainedSubscription.Priority;
                        tempSubscription.PublishingEnabled = retainedSubscription.PublishingEnabled;
                        tempSubscription.Create();
                        Host.Current.MQTTSubscribe(tempSubscription.DisplayName);

                        //Checks for monitored items belonging to subscription and recreates them.
                        foreach (string item in Directory.GetFiles(itemsFolder, "*.json"))
                        {
                            if (item != "")
                            {
                                if (item.Contains(string.Format("{0}.json", tempSubscription.DisplayName)))
                                {
                                    try
                                    {
                                        String[] testItems = File.ReadAllLines(item);
                                        foreach (string testItem in testItems)
                                        {
                                            //Checking and recreating monitored items for each subscription.
                                            ReferenceDescription retainedItem;
                                            retainedItem = JsonConvert.DeserializeObject<ReferenceDescription>(testItem);
                                            Subscribe(tempSubscription, retainedItem);
                                        }
                                        break;
                                    }
                                    catch (Exception ex)
                                    {
                                        EventLog.WriteEntry(ex.Message, EventLogEntryType.Error);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        EventLog.WriteEntry(ex.Message, EventLogEntryType.Error);
                    }
                }
            }
        }

        /// <summary>
        /// Adds a item to a subscription.
        /// </summary>
        public void Subscribe(Subscription subscription, ReferenceDescription reference)
        {
            MonitoredItem monitoredItem = new MonitoredItem(subscription.DefaultItem);

            monitoredItem.DisplayName = subscription.Session.NodeCache.GetDisplayText(reference);
            monitoredItem.StartNodeId = (NodeId)reference.NodeId;
            monitoredItem.NodeClass = (NodeClass)reference.NodeClass;
            monitoredItem.AttributeId = Attributes.Value;
            monitoredItem.SamplingInterval = 0;
            monitoredItem.QueueSize = 1;

            // add condition fields to any event filter.
            EventFilter filter = monitoredItem.Filter as EventFilter;

            if (filter != null)
            {
                monitoredItem.AttributeId = Attributes.EventNotifier;
                monitoredItem.QueueSize = 0;
            }

            subscription.AddItem(monitoredItem);
            subscription.ApplyChanges();
        }
        #endregion


        //MQTT publication of OPC subscriptions.
        public void OPCPublish(Session session)
        {
            if (session != null)
            {
                try
                {
                    //Checks if any subscriptions within session.
                    if (session.SubscriptionCount > 0)
                    {
                        //Iterates through session subscriptions.
                        IEnumerable<Subscription> subscriptions = session.Subscriptions;
                        foreach (Subscription subscription in subscriptions)
                        {
                            // If subscription is invalid somehow, move on to next.
                            if (subscription == null)
                            {
                                continue;
                            }
                            IDictionary<String, String> subscriptionPayload = new Dictionary<String, String>();
                            double subscriptionInterval = subscription.CurrentPublishingInterval;
                            subscription.CurrentPublishedTime = DateTime.Now;
                            TimeSpan intervalCheck = subscription.CurrentPublishedTime.Subtract(subscription.PreviousPublishedTime);
                            //Checks if subscription has already been published to MQTT broker.
                            double intervalDifference = (double)intervalCheck.TotalMilliseconds;
                            //Checks if publishing interval has been reached and weather subscription has been published at least once.
                            if (intervalDifference < subscriptionInterval && subscription.SubscriptionPublished == true)
                            {
                                continue;
                            }
                            else
                            {
                                //Iterates through subscription items.               
                                if (subscription.MonitoredItemCount > 0)
                                {
                                    IEnumerable<MonitoredItem> monitoredItems = subscription.MonitoredItems;
                                    //Adds monitored items to a 'payload' dictionary to be seralized as a JSON string.
                                    foreach (MonitoredItem monitoredItem in monitoredItems)
                                    {
                                        if (monitoredItem.LastValue != null)
                                        {
                                            string monitoredDisplayName = monitoredItem.DisplayName;
                                            string monitoredValue = monitoredItem.LastValue.ToString();
                                            NodeId itemID = monitoredItem.ResolvedNodeId;
                                            DataValue nodeValue = session.ReadValue(itemID);
                                            string actualValue = nodeValue.Value.ToString();
                                            subscriptionPayload.Add(monitoredDisplayName, actualValue);
                                        }
                                        string message = JsonConvert.SerializeObject(subscriptionPayload);
                                        Host.Current.MQTTPublish(subscription.DisplayName, message);
                                        Host.Current.MQTTSubscribe(subscription.DisplayName);
                                        subscription.PreviousPublishedTime = DateTime.Now;
                                        subscription.SubscriptionPublished = true;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception.
                    EventLog.WriteEntry(ex.Message, EventLogEntryType.Error);
                }
            }
        }
    
    }
}