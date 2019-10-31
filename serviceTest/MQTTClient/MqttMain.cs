using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MQTTCore;

using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet.Client.Subscribing;
using MQTTnet.Server;
using MQTTnet.Extensions.ManagedClient;

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
                //mqttClient = MQTTCore.MqttClient.CreateClient();
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
    }
}
