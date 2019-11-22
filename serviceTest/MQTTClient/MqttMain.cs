using System;
using System.Windows.Forms;
using System.ServiceModel;
using System.ServiceProcess;
using Microsoft.Win32;
using MQTTnet.Extensions.ManagedClient;


namespace MQTTClientForm
{
    public partial class MqttMain : Form
    {
        public IManagedMqttClient managedMqttClient;
        public string brokerIP;
        brokerService.BrokerServiceClient client = new brokerService.BrokerServiceClient("NetTcpBinding_IBrokerService");
        ServiceController brokerWindows = new ServiceController("brokerWindows");

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
            connectionChoice.SelectedIndex = 0;
            connectionString.Text = "localhost";
        }

        //Connect to specified address.
        private void ConnectButton_Click(object sender, EventArgs e)
        {
            if (connectionString.Text != string.Empty)
            {
                brokerIP = connectionString.Text;
                if(client.InnerChannel.State != CommunicationState.Faulted)
                {
                    if (connectionChoice.SelectedIndex == 0)
                        try
                        {
                            client.MQTTConnectClientAsync(connectionString.Text, 0);
                            labelMessage.Text = "MQTT connected";
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    else if (connectionChoice.SelectedIndex == 1)
                        try
                        {
                            client.MQTTConnectClientAsync(connectionString.Text, 1);
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
                labelMessage.Text = "Service Started";
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
                labelMessage.Text = "Service Stopped";
            }
            catch (Exception)
            {
                MessageBox.Show("Service already stopped.");
            }
        }

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

        private void OpcStart_Click(object sender, EventArgs e)
        {
            try
            {
                client.OPCCreateClient("opc.tcp://localhost:62541/Quickstarts/ReferenceServer", false);
                labelMessage.Text = "OPC connected";
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
                labelMessage.Text = "Service Started";
            }
            catch (Exception)
            {
                MessageBox.Show("Service already running.");
            }
        }
    }
}
