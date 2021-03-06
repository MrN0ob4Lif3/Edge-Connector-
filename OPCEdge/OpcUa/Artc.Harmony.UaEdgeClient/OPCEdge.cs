﻿using System;
using System.Windows.Forms;
using System.ServiceProcess;
using System.Reflection;
using System.Drawing;
using System.Collections.Generic;
using Microsoft.Win32;
using Opc.Ua;
using Opc.Ua.Configuration;
using Opc.Ua.Client;
using Opc.Ua.Client.Controls;
using Opc.Ua.Sample.Controls;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text.RegularExpressions;

namespace OpcEdgeClient
{
    public partial class OpcEdgeMain : Form
    {
        #region Form Variables
        public string brokerIP;
        private static Session m_session;
        private ApplicationConfiguration m_configuration;
        private ApplicationConfiguration app_configuration;
        private ServiceMessageContext context;
        private ConfiguredEndpointCollection m_endpoints;
        private SessionReconnectHandler m_reconnectHandler;
        private int m_reconnectPeriod = 10;
        public String[] m_topicList;
        string mainFolder;
        string itemsFolder;
        string subscriptionsFolder;
        string sessionsFolder;
        string sessionEndpoint;
        private delegate void FormDelegate();
        static bool autoAccept = true;
        public EndpointConfiguration m_endpoint_configuration;
        private EndpointDescription selectedEndpoint;
        private ConfiguredEndpoint m_endpoint;
        ApplicationInstance application;
        ServiceController OpcWindowsService;
        OpcWCFInterface.OpcWCFInterfaceClient client;
        IDictionary<String, String> publishPayload = new Dictionary<String, String>();
        System.Threading.Timer checkService;
        #endregion

        #region Startup Settings
        //Startup registry key and value
        private static readonly string StartupKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        private static readonly string StartupValue = "OPCEdge";

        //Sets WinFrom application to run at startup.
        private static void SetStartup()
        {
            //Set the application to run at startup
            RegistryKey key = Registry.CurrentUser.OpenSubKey(StartupKey, true);
            key.SetValue(StartupValue, Application.ExecutablePath.ToString());
        }

        //Initializes OPC Application instance.
        public async void InitializeClients()
        {
            //Creates directory for retained information in case it was deleted somehow.
            createDirectories();

            //Initialize OPC Application Instance
            application = new ApplicationInstance
            {
                ApplicationName = "MQTT-OPC Broker",
                ApplicationType = ApplicationType.ClientAndServer,
                ConfigSectionName = "Opc.Ua.SampleClient"
            };
            // load the application configuration.
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
            sessionEndpoint = client.SessionEndpoint();
            //If service is currently connected to a OPC endpoint, recreate the connection on the client.
            if (sessionEndpoint != null)
            {
                sessionEndpoint = Regex.Replace(sessionEndpoint, "\\\\|\"", "");
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
                try
                {
                    //Creates session with OPC Server
                    selectedEndpoint = CoreClientUtils.SelectEndpoint(sessionEndpoint, haveAppCertificate, 15000);
                    m_endpoint_configuration = EndpointConfiguration.Create(config);
                    m_endpoint = new ConfiguredEndpoint(null, selectedEndpoint, m_endpoint_configuration);
                    m_session = await Session.Create(config, m_endpoint, false, "OpcEdgeClient", 60000, new UserIdentity(new AnonymousIdentityToken()), null);
                    // Open dialog to declare Session name.
                    //new SessionOpenDlg().ShowDialog(m_session, opcSession.PreferredLocales);
                    // Deletes the existing session.
                    opcSession.Close();
                    // Adds session to tree.
                    opcSession.AddNode(m_session);
                    //Checks if service has a session connected and disconnects if connected.
                    if (client.CheckConnected())
                    {
                        client.OPCDisconnect();
                    }
                    // Passes endpointURL to service for connection.
                    client.OPCConnect(selectedEndpoint.EndpointUrl);

                    if (m_session != null)
                    {
                        // stop any reconnect operation.
                        if (m_reconnectHandler != null)
                        {
                            m_reconnectHandler.Dispose();
                            m_reconnectHandler = null;
                        }
                        //Register keep alive handler & sets view
                        m_session.KeepAlive += new KeepAliveEventHandler(StandardClient_KeepAlive);
                        opcBrowse.SetView(m_session, BrowseViewType.Objects, null);
                        StandardClient_KeepAlive(m_session, null);

                        //Saves session endpoint URL.
                        m_session.SessionName = "My Session";
                        await SaveSessionAsync(m_session);
                        //Recreates prior session's subscriptions and monitored items.
                        string retainedEndpoint = await LoadSessionAsync(selectedEndpoint.EndpointUrl);
                        if (sessionEndpoint == retainedEndpoint)
                        {
                            RecreateSession(m_session);
                        }
                    }
                    OpcConnectionLabel.Text = "Currently Connected to: " + selectedEndpoint.EndpointUrl;
                }
                catch (Exception ex)
                {
                    // Log the exception.
                    MessageBox.Show(ex.Message);
                }
            }
        }
        //Initializes specific elements for OPC and MQTT interfaces to work.
        public OpcEdgeMain()
        {
            //Loading controller for WCF service.
            client = new OpcWCFInterface.OpcWCFInterfaceClient("NetTcpBinding_IOpcWCFInterface");
            OpcWindowsService = new ServiceController("OpcWindowsService");
            CheckService();
            InitializeComponent();
            SetStartup();
            //Retrieves path to retained information folders from service.
            mainFolder = client.MainFolder();
            sessionsFolder = client.SessionsFolder();
            subscriptionsFolder = client.SubscriptionsFolder();
            itemsFolder = client.ItemsFolder();
            //Loading OPC Application Instance.
            InitializeClients();
            //Use timer callback to check if service is still running
            checkService = new System.Threading.Timer(x => CheckService(), null, 5000, 1000);
        }

        private void OpcEdgeMain_Load(object sender, EventArgs e)
        {
            opcEndpoints.Initialize(m_endpoints, m_configuration);
            opcSession.Configuration = m_configuration = app_configuration;
            opcSession.MessageContext = context;
            if (!app_configuration.SecurityConfiguration.AutoAcceptUntrustedCertificates)
            {
                app_configuration.CertificateValidator.CertificateValidation += new CertificateValidationEventHandler(CertificateValidator_CertificateValidation);
            }
        }
        #endregion

        #region Form Controls (Close, Resize, etc.)
        //Form Resizing 
        private void OpcEdgeMain_Resize(object sender, EventArgs e)
        {
            //if the form is minimized  
            //hide it from the task bar  
            //and show the system tray icon (represented by the NotifyIcon control)  
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                this.ShowInTaskbar = true;
                brokerNotify.Visible = true;
            }
        }

        private void OpcEdgeNotify_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            brokerNotify.Visible = false;
        }

        private void OpcEdgeMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if the form is closed  
            //hide it from the task bar  
            //and show the system tray icon (represented by the NotifyIcon control)  
            e.Cancel = true;
            this.Hide();
            this.ShowInTaskbar = true;
            brokerNotify.Visible = true;
        }

        private void OpcEdgeMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.checkService.Change(Timeout.Infinite, Timeout.Infinite);
            this.checkService.Dispose();
        }

        #endregion

        #region OPC Controls
        #region OPC Connect
        void OpcEndpoints_ConnectEndpoint(object sender, ConnectEndpointEventArgs e)
        {
            try
            {
                OPCConnect(this.application, opcEndpoints.SelectedEndpoint);
            }
            catch (Exception exception)
            {
                GuiUtils.HandleException(this.Text, MethodBase.GetCurrentMethod(), exception);
                e.UpdateControl = false;
            }
        }

        /// <summary>
        /// Creates a session with the endpoint.
        /// </summary>
        /// 
        public async void OPCConnect(ApplicationInstance application, ConfiguredEndpoint endpoint)
        {
            if (endpoint == null)
            {
                return;
            }
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
            string endpointURL = endpoint.EndpointUrl.ToString();
            string retainedEndpoint = null;
            bool retainSession = false;
            retainedEndpoint = await LoadSessionAsync(endpointURL);
            try
            {
                if (retainedEndpoint == null || !retainSession)
                {
                    selectedEndpoint = CoreClientUtils.SelectEndpoint(endpointURL, haveAppCertificate, 15000);
                }
                else
                {
                    selectedEndpoint = CoreClientUtils.SelectEndpoint(retainedEndpoint, haveAppCertificate, 15000);
                }
                //Select endpoint after discovery.
                //Creates session with OPC Server
                sessionEndpoint = selectedEndpoint.EndpointUrl;
                m_endpoint_configuration = EndpointConfiguration.Create(config);
                m_endpoint = new ConfiguredEndpoint(null, selectedEndpoint, m_endpoint_configuration);
                m_session = await Session.Create(config, m_endpoint, false, "OpcEdgeClient", 60000, new UserIdentity(new AnonymousIdentityToken()), null);
                // Open dialog to declare Session name.
                if (new SessionOpenDlg().ShowDialog(m_session, opcSession.PreferredLocales) == false)
                {
                    return;
                }
                // Deletes the existing session.
                opcSession.Close();
                // Adds session to tree.
                opcSession.AddNode(m_session);
                //Checks if service has a session connected and disconnects if connected.
                if (client.CheckConnected())
                {
                    client.OPCDisconnect();
                }
                // Passes endpointURL to service for connection.
                client.OPCConnect(selectedEndpoint.EndpointUrl);
                
                if (m_session != null)
                {
                    // stop any reconnect operation.
                    if (m_reconnectHandler != null)
                    {
                        m_reconnectHandler.Dispose();
                        m_reconnectHandler = null;
                    }
                    //Register keep alive handler & sets view
                    m_session.KeepAlive += new KeepAliveEventHandler(StandardClient_KeepAlive);
                    opcBrowse.SetView(m_session, BrowseViewType.Objects, null);
                    StandardClient_KeepAlive(m_session, null);

                    //Saves session endpoint URL.
                    m_session.SessionName = "My Session";
                    await SaveSessionAsync(m_session);
                    //Recreates prior session's subscriptions and monitored items.
                    if (retainedEndpoint == endpointURL)
                    {
                        RecreateSession(m_session);
                    }
                }
                OpcConnectionLabel.Text = "Currently Connected to: " + selectedEndpoint.EndpointUrl;
            }
            catch (Exception ex)
            {
                // Log the exception.
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region OPC Subscription / Monitored Items
        //Passes subscription to service session for subscription.
        public void OPCSubscribe(object sender, SubscribeEventArgs e)
        {
            try
            {
                client.OPCSubscribe(e.subscription);
            }
            catch (Exception exception)
            {
                GuiUtils.HandleException(this.Text, MethodBase.GetCurrentMethod(), exception);
            }
        }
        //Passes subscription to service session for unsubscription.
        public void OPCUnsubscribe(object sender, DeleteSubscriptionArgs e)
        {
            try
            {
                client.OPCUnsubscribe(e.subscription);
            }
            catch (Exception exception)
            {
                GuiUtils.HandleException(this.Text, MethodBase.GetCurrentMethod(), exception);
            }
        }

        //Passes monitored item to service session to add to subscription.
        public void OPCMonitor(object sender, MonitorEventArgs e)
        {
            try
            {
                client.OPCMonitor(e.subscription, e.monitoredItem);
            }
            catch (Exception exception)
            {
                GuiUtils.HandleException(this.Text, MethodBase.GetCurrentMethod(), exception);
            }
        }
        //Passes monitored item to service session to remove from subscription.
        public void OPCUnmonitor(object sender, DeleteItemArgs e)
        {
            try
            {
                client.OPCUnmonitor(e.subscription, e.monitoredItem);
            }
            catch (Exception exception)
            {
                GuiUtils.HandleException(this.Text, MethodBase.GetCurrentMethod(), exception);
            }
        }
        //Passes modified subscription to service session to reflect changes.
        public void OPCDisconnect(object sender, DeleteSessionArgs e)
        {
            try
            {
                client.OPCDisconnect();
            }
            catch (Exception exception)
            {
                GuiUtils.HandleException(this.Text, MethodBase.GetCurrentMethod(), exception);
            }
        }
        #endregion

        #region OPC Client Alive / Reconnect / Certificate Validator
        /// <summary>
        /// Updates the status control and publishes OPC subscriptions to MQTT broker when a keep alive event occurs 
        /// </summary>
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
        private void StandardClient_Server_ReconnectComplete(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new EventHandler(StandardClient_Server_ReconnectComplete), sender, e);
                return;
            }

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

                opcBrowse.SetView(m_session, BrowseViewType.Objects, null);

                opcSession.Reload(m_session);

                StandardClient_KeepAlive(m_session, null);
            }
            catch (Exception exception)
            {
                GuiUtils.HandleException(this.Text, MethodBase.GetCurrentMethod(), exception);
            }
        }

        /// <summary>
        /// Prompts for user input if certificate validation has error.
        /// </summary>
        void CertificateValidator_CertificateValidation(CertificateValidator validator, CertificateValidationEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new CertificateValidationEventHandler(CertificateValidator_CertificateValidation), validator, e);
                return;
            }

            try
            {
                //GuiUtils.HandleCertificateValidationError(this, validator, e);
                e.Accept = true;
            }
            catch (Exception exception)
            {
                GuiUtils.HandleException(this.Text, MethodBase.GetCurrentMethod(), exception);
            }
        }
        #endregion
        #endregion

        #region Session State
        //Saves endpoint
        public Task SaveSessionAsync(Session session)
        {
            string sessionEndpoint = session.Endpoint.EndpointUrl;
            string sessionFile = Path.Combine(sessionsFolder, string.Format(@"{0}.json", m_session.SessionName));

            if (File.Exists(sessionFile))
            {
                try
                {
                    string[] sessionEndpoints = File.ReadAllLines(sessionFile);
                    foreach (string endpoint in sessionEndpoints)
                    {
                        if (endpoint.Contains(sessionEndpoint))
                        {
                            File.Delete(sessionFile);
                            string newFile = Path.Combine(sessionsFolder, string.Format(@"{0}.json", m_session.SessionName));
                            File.AppendAllText(sessionFile, JsonConvert.SerializeObject(sessionEndpoint));
                            return Task.FromResult(0);
                        }
                        else
                        {
                            return Task.FromResult(0);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception.
                    MessageBox.Show(ex.Message);
                }
                return Task.FromResult(0);
            }
            else
            {
                File.AppendAllText(sessionFile, JsonConvert.SerializeObject(sessionEndpoint));
                return Task.FromResult(0);
            }
        }

        //Loads retained endpoint
        public Task<String> LoadSessionAsync(String sessionEndpoint)
        {
            String retainedSession = null;
            String[] sessionFiles = Directory.GetFiles(sessionsFolder, "*.json");
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
                    catch (Exception ex)
                    {
                        // Log the exception.
                        MessageBox.Show(ex.Message);
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
                        client.MQTTSubscribeTopic(tempSubscription.DisplayName);
                        
                        //Checks for monitored items belonging to subscription and recreates them.
                        foreach (string item in Directory.GetFiles(itemsFolder, "*.json"))
                        {
                            if (item != "")
                            {
                                if (item.Contains(string.Format("{0}.json", tempSubscription.DisplayName)))
                                {
                                    try
                                    {
                                        String[] storedItems = File.ReadAllLines(item);
                                        foreach (string storedItem in storedItems)
                                        {
                                            //Checking and recreating monitored items for each subscription.
                                            ReferenceDescription retainedItem;
                                            retainedItem = JsonConvert.DeserializeObject<ReferenceDescription>(storedItem);
                                            opcBrowse.Subscribe(tempSubscription, retainedItem);
                                        }
                                        break;
                                    }
                                    catch
                                    {
                                        MessageBox.Show("Error Recreating.");
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Error Recreating.");
                    }
                }
            }
        }
        #endregion

        #region Service State
        public void CheckService()
        {
            try
            {
                client.CheckService();
            }
            catch (Exception ex)
            {
                if(this.InvokeRequired)
                {
                    var form = new FormDelegate(CheckService);
                    this.checkService.Change(Timeout.Infinite, Timeout.Infinite);
                    this.checkService.Dispose();
                    this.Invoke(form);
                }
                else
                {
                    MessageBox.Show("The OPC Service has not been started yet or has encountered an error. Please restart it.");
                    this.Close();
                    System.Environment.Exit(1);
                }
            }
        }

        //Create directories to store retained information (Session endpoint, subscriptions, monitored items)
        public void createDirectories()
        {
            //Creates directories to store retained information.
            if (!Directory.Exists(mainFolder))
            {
                Directory.CreateDirectory(mainFolder);
                DirectorySecurity sec = Directory.GetAccessControl(mainFolder);
                SecurityIdentifier everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
                //Changing permissions for mainFolder so EdgeClient can access
                sec.AddAccessRule(new FileSystemAccessRule(everyone, FileSystemRights.Modify | FileSystemRights.Synchronize, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, System.Security.AccessControl.AccessControlType.Allow));
                Directory.SetAccessControl(mainFolder, sec);
            }
            if (!Directory.Exists(itemsFolder))
            {
                Directory.CreateDirectory(itemsFolder);
            }
            if (!Directory.Exists(subscriptionsFolder))
            {
                Directory.CreateDirectory(subscriptionsFolder);
            }
            if (!Directory.Exists(sessionsFolder))
            {
                Directory.CreateDirectory(sessionsFolder);
            }
        }
        #endregion
    }
}

