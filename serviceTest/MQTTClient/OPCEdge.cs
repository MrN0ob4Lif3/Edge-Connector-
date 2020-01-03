﻿using System;
using System.Windows.Forms;
using System.ServiceModel;
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

namespace BrokerClient
{
    public partial class BrokerMain : Form
    {
        #region Form Variables
        public string brokerIP;
        brokerService.BrokerServiceClient client;
        ServiceController brokerWindows;
        private static Session m_session;
        private Browser m_browser;
        private bool m_connectedOnce;
        private ApplicationConfiguration m_configuration;
        private ApplicationConfiguration app_configuration;
        private ServiceMessageContext context;
        private ConfiguredEndpointCollection m_endpoints;
        private SessionReconnectHandler m_reconnectHandler;
        private int m_reconnectPeriod = 10;
        public String[] m_topicList;
        IDictionary<String, String> publishPayload = new Dictionary<String, String>();
        string itemsFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Retained Monitored Items");
        string subscriptionsFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Retained Subscriptions");
        string sessionFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Retained Session");
        System.Threading.Timer publishTimer;
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

        //Continuously checks if service is running, if not, restart application instance to refresh connection to restarted service.
        public void CheckService()
        {
            while (true)
            {
                try
                {
                    client.Open();
                }
                catch
                {
                    Application.Restart();
                }
            }
        }

        public void InitializeClients()
        {
            Directory.CreateDirectory(itemsFolder);
            Directory.CreateDirectory(subscriptionsFolder);
            Directory.CreateDirectory(sessionFolder);
            //Loading OPC elements
            ApplicationInstance application = client.GetApplicationInstance();
            // load the application configuration.
            application.LoadApplicationConfiguration(false).Wait();
            // check the application certificate.
            application.CheckApplicationInstanceCertificate(false, 0).Wait();
            m_configuration = app_configuration = application.ApplicationConfiguration;
            // get list of cached endpoints.
            m_endpoints = client.GetEndpoints();
            m_endpoints.DiscoveryUrls = app_configuration.ClientConfiguration.WellKnownDiscoveryUrls;
            if (!app_configuration.SecurityConfiguration.AutoAcceptUntrustedCertificates)
            {
                app_configuration.CertificateValidator.CertificateValidation += new CertificateValidationEventHandler(CertificateValidator_CertificateValidation);
            }
        }

        //Initializes specific elements for OPC and MQTT interfaces to work.
        public BrokerMain()
        {
            InitializeComponent();
            SetStartup();
            //Loading controller for WCF service.
            client = new brokerService.BrokerServiceClient("NetTcpBinding_IBrokerService");
            brokerWindows = new ServiceController("brokerWindows");
            //New thread to monitor for service disruption.
            //Thread checkService = new Thread(CheckService);
            //CheckService();
            //Loading OPC Application Instance.
            InitializeClients();

            //Use timer callback to monitor and publish subscriptions.
            publishTimer = new System.Threading.Timer(x => OPCPublish(m_session), null, 5000, 1000);
        }

        private void MqttMain_Load(object sender, EventArgs e)
        {
            //Loading MQTT elements
            publishPayload = new Dictionary<String, String>();
            connectionStringMQTT.Text = "dev-harmony-01.southeastasia.cloudapp.azure.com:8080/mqtt";
            m_topicList = client.MQTTSubscribedTopics();
            // Initialize Form controls.
            if (m_topicList != null)
            {
                foreach (String topic in m_topicList)
                {
                    topicListPub.Items.Add(topic);
                    topicListSub.Items.Add(topic);
                }
            }

            if (publishKey1.Text != null && publishValue1.Text != null)
            {
                publishPayload.Add(publishKey1.Text, publishValue1.Text);
            }
            if (publishKey2.Text != null && publishValue2.Text != null)
            {
                publishPayload.Add(publishKey2.Text, publishValue2.Text);
            }
            string message = JsonConvert.SerializeObject(publishPayload);
            client.MQTTPublishTopicAsync("Artc/Harmony/Connector/EdgeToCloud", message);
            opcEndpoints.Initialize(m_endpoints, m_configuration);
            opcSession.Configuration = m_configuration = app_configuration;
            opcSession.MessageContext = context;

            if (!app_configuration.SecurityConfiguration.AutoAcceptUntrustedCertificates)
            {
                app_configuration.CertificateValidator.CertificateValidation += new CertificateValidationEventHandler(CertificateValidator_CertificateValidation);
            }
        }
        #endregion

        #region MQTT Controls
        //Connect to specified address.
        private void ConnectButton_Click(object sender, EventArgs e)
        {
            if (connectionStringMQTT.Text != string.Empty)
            {
                brokerIP = connectionStringMQTT.Text;
                if (client.InnerChannel.State != CommunicationState.Faulted)
                {
                    try
                    {
                        client.MQTTConnectClientAsync(connectionStringMQTT.Text);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Error connecting.");
                    }
                }
                else
                {
                    client = new brokerService.BrokerServiceClient("NetTcpBinding_IBrokerService");
                }

            }
        }
        //Subscribe to topic.
        private void SubscribeButton_Click(object sender, EventArgs e)
        {
            try
            {
                string topic = topicSubscribe.Text;
                client.MQTTSubscribeTopic(topic);
                if (topicListPub.Items.Contains(topic))
                    return;
                else
                    topicListPub.Items.Add(topic);
                topicListSub.Items.Add(topic);
            }
            catch (Exception)
            {
                MessageBox.Show("Error subscribing.");
            }
        }
        //Unsubscribes from topic.
        private void UnsubscribeButton_Click(object sender, EventArgs e)
        {
            try
            {
                client.MQTTUnsubscribeTopic(topicSubscribe.Text);
                ListBox.SelectedObjectCollection selectedItems = new ListBox.SelectedObjectCollection(topicListSub);
                selectedItems = topicListSub.SelectedItems;

                if (topicListSub.SelectedIndex != -1)
                {
                    for (int i = selectedItems.Count - 1; i >= 0; i--)
                    {
                        topicListPub.Items.Remove(selectedItems[i]);
                        topicListSub.Items.Remove(selectedItems[i]);
                    }
                }
                else
                    MessageBox.Show("Select a topic to unsubscribe from.");
            }
            catch (Exception)
            {
                MessageBox.Show("Error unsubscribing");
            }
        }
        //Publishes message to selected topic.
        private void PublishButton_Click(object sender, EventArgs e)
        {
            try
            {
                publishPayload = new Dictionary<String, String>();
                string topicChosen = null;

                //Checks if topic selected or entered.
                if (topicListPub.SelectedItem != null && pubTopic.Text != null)
                    topicChosen = topicListPub.SelectedItem.ToString();
                else if (pubTopic.Text != null)
                    topicChosen = pubTopic.Text;
                else
                    MessageBox.Show("Please select a topic or input a new topic to publish to.");

                //Checks if key / value has been filled.
                if (publishKey1.Text != null && publishValue1.Text != null)
                {
                    publishPayload.Add(publishKey1.Text, publishValue1.Text);
                }
                if (publishKey2.Text != null && publishValue2.Text != null)
                {
                    publishPayload.Add(publishKey2.Text, publishValue2.Text);
                }
                if (publishPayload != null)
                {
                    string message = JsonConvert.SerializeObject(publishPayload);
                    client.MQTTPublishTopicAsync(topicChosen, message);
                }

                //Checks if topicChosen is a new input topic. If so, add to topic list.
                if (topicChosen != null && topicListSub.Items.Contains(topicChosen))
                    return;
                else if (topicChosen != null)
                {
                    topicListPub.Items.Add(topicChosen);
                    topicListSub.Items.Add(topicChosen);
                    client.MQTTSubscribeTopic(topicChosen);
                }
                else
                    MessageBox.Show("Please select a topic or input a new topic to publish to.");
            }
            catch (Exception)
            {
                MessageBox.Show("Error publishing.");
            }
        }
        //Refreshes elements after tab switch.
        private void MqttTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            topicListSub.SelectedItem = null;
            topicListPub.SelectedItem = null;
        }
        #endregion

        #region Form Controls (Close, Resize, etc.)
        //Form Resizing 
        private void MqttMain_Resize(object sender, EventArgs e)
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

        private void MqttNotify_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            brokerNotify.Visible = false;
        }



        private void MqttMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if the form is closed  
            //hide it from the task bar  
            //and show the system tray icon (represented by the NotifyIcon control)  
            e.Cancel = true;
            this.Hide();
            this.ShowInTaskbar = true;
            brokerNotify.Visible = true;
        }
        private void MqttMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.publishTimer.Change(Timeout.Infinite, Timeout.Infinite);
            this.publishTimer.Dispose();
        }


        #endregion

        #region OPC Controls

        #region OPC Connect
        void OpcEndpoints_ConnectEndpoint(object sender, ConnectEndpointEventArgs e)
        {
            try
            {
                Connect(e.Endpoint);
            }
            catch (Exception exception)
            {
                GuiUtils.HandleException(this.Text, MethodBase.GetCurrentMethod(), exception);
                e.UpdateControl = false;
            }
        }

        /// <summary>
        /// Connects to a server.
        /// </summary>
        public async void Connect(ConfiguredEndpoint endpoint)
        {
            if (endpoint == null)
            {
                return;
            }
            Session session = await opcSession.Connect(endpoint);

            if (session != null)
            {
                // stop any reconnect operation.
                if (m_reconnectHandler != null)
                {
                    m_reconnectHandler.Dispose();
                    m_reconnectHandler = null;
                }
                m_session = session;
                m_session.KeepAlive += new KeepAliveEventHandler(StandardClient_KeepAlive);
                opcBrowse.SetView(m_session, BrowseViewType.Objects, null);
                StandardClient_KeepAlive(m_session, null);

                //Checks if endpoint URL selected is same as retained endpoint URL
                String retainedEndpoint = null;
                String sessionEndpoint = m_session.Endpoint.EndpointUrl;
                retainedEndpoint = await LoadSessionAsync(sessionEndpoint);
                //If no retained endpoints, saves current endpoint.
                if (retainedEndpoint == null)
                {
                    //Saves session endpoint URL.
                    await SaveSessionAsync(m_session);
                    return;
                }
                //Recreates prior session's subscriptions and monitored items.
                if (sessionEndpoint == retainedEndpoint)
                {
                    //Recreating retained subscriptions.
                    foreach (string sub in Directory.GetFiles(subscriptionsFolder, "*.json"))
                    {
                        String testSubscriptions = File.ReadAllText(sub);
                        if (testSubscriptions != "")
                        {
                            try
                            {
                                //Recreates subscription based on retained subscription details.
                                Subscription retainedSubscription;
                                Subscription tempSubscription = new Subscription(m_session.DefaultSubscription);
                                retainedSubscription = JsonConvert.DeserializeObject<Subscription>(testSubscriptions);
                                m_session.AddSubscription(tempSubscription);
                                tempSubscription.DisplayName = retainedSubscription.DisplayName;
                                tempSubscription.PublishingInterval = retainedSubscription.PublishingInterval;
                                tempSubscription.KeepAliveCount = retainedSubscription.KeepAliveCount;
                                tempSubscription.LifetimeCount = retainedSubscription.LifetimeCount;
                                tempSubscription.MaxNotificationsPerPublish = retainedSubscription.MaxNotificationsPerPublish;
                                tempSubscription.Priority = retainedSubscription.Priority;
                                tempSubscription.PublishingEnabled = retainedSubscription.PublishingEnabled;
                                tempSubscription.Create();
                                client.MQTTSubscribeTopic(tempSubscription.DisplayName);

                                //Checks for monitored items belonging to subscripton and recreates them.
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
                                                    opcBrowse.Subscribe(tempSubscription, retainedItem);
                                                    if (topicListPub.Items.Contains(tempSubscription.DisplayName))
                                                    {
                                                        continue;
                                                    }
                                                    else
                                                    {
                                                        topicListPub.Items.Add((tempSubscription.DisplayName));
                                                        topicListSub.Items.Add((tempSubscription.DisplayName));
                                                    }
                                                }
                                                break;
                                            }
                                            catch { }
                                        }
                                    }
                                }
                            }
                            catch { }
                        }
                    }
                }
            }
        }
        #endregion

        #region OPC Client Alive / Reconnect / Publish
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
        #endregion

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
                GuiUtils.HandleCertificateValidationError(this, validator, e);
            }
            catch (Exception exception)
            {
                GuiUtils.HandleException(this.Text, MethodBase.GetCurrentMethod(), exception);
            }
        }
        #endregion

        //MQTT publication of OPC subscriptions.
        public void OPCPublish(Session session)
        {
            if(session != null)
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
                                            subscriptionPayload.Add(monitoredDisplayName, monitoredValue);
                                        }
                                        string message = JsonConvert.SerializeObject(subscriptionPayload);
                                        client.MQTTPublishTopicAsync(subscription.DisplayName, message);
                                        client.MQTTSubscribeTopic(subscription.DisplayName);
                                        subscription.PreviousPublishedTime = DateTime.Now;
                                        subscription.SubscriptionPublished = true;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }
        #region Session State
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
        #endregion

    }
}
