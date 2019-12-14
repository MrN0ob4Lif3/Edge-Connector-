using System;
using System.Windows.Forms;
using System.ServiceModel;
using System.ServiceProcess;
using Microsoft.Win32;
using MQTTnet.Extensions.ManagedClient;
using Opc.Ua;
using Opc.Ua.Configuration;
using Opc.Ua.Client;
using Opc.Ua.Client.Controls;
using System.Reflection;
using Opc.Ua.Sample.Controls;
using System.Drawing;

namespace MQTTClientForm
{
    public partial class MqttMain : Form
    {
        #region Form Variables
        public string brokerIP;
        brokerService.BrokerServiceClient client = new brokerService.BrokerServiceClient("NetTcpBinding_IBrokerService");
        ServiceController brokerWindows = new ServiceController("brokerWindows");
        private Session m_session;
        private Browser m_browser;
        private bool m_connectedOnce;
        private ApplicationConfiguration m_configuration;
        private ApplicationConfiguration app_configuration;
        private ServiceMessageContext context;
        private ConfiguredEndpointCollection m_endpoints;
        private SessionReconnectHandler m_reconnectHandler;
        private int m_reconnectPeriod = 10;
        #endregion

        #region Startup Settings
        //Startup registry key and value
        private static readonly string StartupKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        private static readonly string StartupValue = "MQTTBroker";
        
        //Sts WinFrom application to run at startup.
        private static void SetStartup()
        {
            //Set the application to run at startup
            RegistryKey key = Registry.CurrentUser.OpenSubKey(StartupKey, true);
            key.SetValue(StartupValue, Application.ExecutablePath.ToString());
        }

        public MqttMain()
        {
            InitializeComponent();
        }

        //Initializes specific form elements.
        private void MqttMain_Load(object sender, EventArgs e)
        {
            connectionChoice.SelectedIndex = 1;
            connectionStringMQTT.Text = "dev-harmony-01.southeastasia.cloudapp.azure.com:8080/mqtt";


            ApplicationInstance application = client.GetApplicationInstance();
            // load the application configuration.
            application.LoadApplicationConfiguration(false).Wait();
            // check the application certificate.
            application.CheckApplicationInstanceCertificate(false, 0).Wait();   
            m_configuration = app_configuration = application.ApplicationConfiguration;
            // get list of cached endpoints.
            m_endpoints = client.GetEndpoints();
            // Initialize Form controls.
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
                if(client.InnerChannel.State != CommunicationState.Faulted)
                {
                    if (connectionChoice.SelectedIndex == 0)
                        try
                        {
                            client.MQTTConnectClientAsync(connectionStringMQTT.Text, 0);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    else if (connectionChoice.SelectedIndex == 1)
                        try
                        {
                            client.MQTTConnectClientAsync(connectionStringMQTT.Text, 1);
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
                client.MQTTSubscribeTopicAsync(topic);
                if (topicListPub.Items.Contains(topic))
                    return;
                else
                    topicListPub.Items.Add(topic);
                    topicListSub.Items.Add(topic);
            }
            catch
            {
                MessageBox.Show("Error subscribing.");
            }
        }      
        //Unsubscribes from topic.
        private void UnsubscribeButton_Click(object sender, EventArgs e)
        {
            try
            {       
                client.MQTTUnsubscribeTopicAsync(topicSubscribe.Text);
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
            catch
            {
                MessageBox.Show("Error unsubscribing");
            }
        }
        //Publishes message to selected topic.
        private void PublishButton_Click(object sender, EventArgs e)
        {
            try
            {
                string topicChosen;
                if (topicListPub.SelectedItem != null && pubTopic.Text != null)
                    topicChosen = topicListPub.SelectedItem.ToString();
                else
                    topicChosen = pubTopic.Text;
                client.MQTTPublishTopicAsync(topicChosen, publishText.Text);
                if (topicListSub.Items.Contains(topicChosen))
                    return;
                else
                    topicListPub.Items.Add(topicChosen);
                topicListSub.Items.Add(topicChosen);
            }
            catch
            {
                MessageBox.Show("Error publishing.");
            }
        }
        //Refreshes elements after tab switch.
        private void MqttTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            topicListSub.SelectedItem = null;
            topicListPub.SelectedItem = null;
            pubTopic.Text = "If you want to publish new topic, enter topic name here.";
        }
        //Starts MQTT service.
        private void MqttStart_Click(object sender, EventArgs e)
        {
            try
            {
                brokerWindows.Start();
            }
            catch (Exception)
            {
                MessageBox.Show("Service already running.");
            }

        }       
        //Stop MQTT service.
        private void MqttStop_Click(object sender, EventArgs e)
        {
            try
            {
                brokerWindows.Stop();
            }
            catch (Exception)
            {
                MessageBox.Show("Service already stopped.");
            }
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
                mqttNotify.Visible = true;
            }
        }

        private void MqttNotify_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            mqttNotify.Visible = false;
        }

        private void MqttMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }
        #endregion

        #region OPC Controls
        private void OpcStart_Click(object sender, EventArgs e)
        {
            brokerIP = connectionStringMQTT.Text; 
            try
            {
                client.OPCCreateClient(brokerIP, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void OpcStop_Click(object sender, EventArgs e)
        {
            try
            {
                
            }
            catch (Exception)
            {
                MessageBox.Show("Service already running.");
            }
        }

        #region OPC Connect
        void OpcEndpoints_ConnectEndpoint(object sender, ConnectEndpointEventArgs e)
        {
            try
            {
                client.Connect(e.Endpoint);
                m_browser = client.GetBrowser();
                m_session = client.GetSession();
                //MQTTClientForm.brokerService.SessionSurrogate test = client.GetSession();
                //m_session = test.OPCSession;

                m_session.KeepAlive += new KeepAliveEventHandler(StandardClient_KeepAlive);
                opcBrowse.SetView(m_session, BrowseViewType.Objects, null);
                StandardClient_KeepAlive(m_session, null);


                //Connect(e.Endpoint);
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
            }
        }
        #endregion

        #region OPC Client Alive / Reconnect
        /// <summary>
        /// Updates the status control when a keep alive event occurs.
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
    }
}
