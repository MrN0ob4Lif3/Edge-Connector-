using System;
using System.Windows.Forms;
using System.ServiceModel;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using System.Threading;
using System.Diagnostics;

namespace MQTTClientForm
{
    public partial class MqttMain : Form
    {
        public IMqttClient mqttClient;
        public IManagedMqttClient managedMqttClient;
        public string brokerIP;

        public MqttMain()
        {
            InitializeComponent();
        }   

        private async void connectButton_Click(object sender, EventArgs e)
        {
            if(connectionString.Text != string.Empty)
            {
                brokerIP = connectionString.Text;
                managedMqttClient = MQTTCore.ManagedClient.CreateManagedClient(); 
                if(connectionChoice.SelectedIndex == 0)
                    try
                    {
                        //await MQTTCore.MqttClient.MqttConnectTCPAsync(mqttClient, brokerIP);
                        await MQTTCore.ManagedClient.ManagedMqttConnectTCPAsync(managedMqttClient, brokerIP);

                    }
                    catch
                    {
                        System.Windows.Forms.MessageBox.Show("Connection failed.\n\nPlease check that you have entered the correct broker address.");
                    }
                        
                else if(connectionChoice.SelectedIndex == 1)
                    try
                    {
                        //await MQTTCore.MqttClient.MqttConnectWebSocket(mqttClient, brokerIP);
                        await MQTTCore.ManagedClient.ManagedMqttConnectWebSocket(managedMqttClient, brokerIP);
                    }
                    catch
                    {
                        System.Windows.Forms.MessageBox.Show("Connection failed.\n\nPlease check that you have entered the correct broker address.");
                    }
            }
        }

        private void MqttMain_Load(object sender, EventArgs e)
        {
            connectionChoice.SelectedIndex = 0;
            connectionString.Text = "localhost";
            brokerService.IbrokerServiceClient client = new brokerService.IbrokerServiceClient("NetTcpBinding_IbrokerService");   
            try
            {
                client.CreateClientAsync(connectionString.Text, 2);
                // Log an event to indicate successful start.
                labelMessage.Text = client.GetData("Service Function works");
            } catch(Exception)
            {
                // Log the exception.
                labelMessage.Text = "Forms Error";
            }

            btnStart.Enabled = false;
            btnStop.Enabled = true;
        }

        private void SubscribeButton_Click(object sender, EventArgs e)
        {
            try
            {
                string topic = topicSubscribe.Text;
                //MQTTCore.MqttClient.MqttSubscribe(mqttClient, topic);
                MQTTCore.ManagedClient.ManagedMqttSubscribe(managedMqttClient, topic);
                if (topicListPub.Items.Contains(topic))
                    return;
                else
                    topicListPub.Items.Add(topic);
                    topicListSub.Items.Add(topic);
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("Error subscribe");
            }
        }

        private void UnsubscribeButton_Click(object sender, EventArgs e)
        {
            try
            {
                //MQTTCore.MqttClient.MqttUnsubscribe(mqttClient, topicSubscribe.Text);
                MQTTCore.ManagedClient.ManagedMqttSubscribe(managedMqttClient, topicSubscribe.Text);
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
                System.Windows.Forms.MessageBox.Show("Error unsubscribe");
            }
        }

        private async void PublishButton_Click(object sender, EventArgs e)
        {
            try
            {
                string topicChosen;
                if (topicListPub.SelectedItem != null && pubTopic.Text != null)
                    topicChosen = topicListPub.SelectedItem.ToString();
                else
                    topicChosen = pubTopic.Text;
                //await MQTTCore.MqttClient.MqttPublish(mqttClient, topicChosen, publishText.Text);
                await MQTTCore.ManagedClient.ManagedMqttPublish(managedMqttClient, topicChosen, publishText.Text);
                if (topicListSub.Items.Contains(topicChosen))
                    return;
                else
                    topicListPub.Items.Add(topicChosen);
                topicListSub.Items.Add(topicChosen);
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("Error publish");
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            labelMessage.Text = "Service Started";
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            labelMessage.Text = "Service Stopped";
        }

        private void MqttMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            btnStart.Enabled = true;
            btnStop.Enabled = false;
        }
    }
}
