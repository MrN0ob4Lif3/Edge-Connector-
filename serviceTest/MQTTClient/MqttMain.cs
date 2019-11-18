using System;
using System.Windows.Forms;
using System.ServiceModel;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using System.Threading;
using System.Diagnostics;
using System.ServiceProcess;

namespace MQTTClientForm
{
    public partial class MqttMain : Form
    {
        public IManagedMqttClient managedMqttClient;
        public string brokerIP;
        brokerService.IbrokerServiceClient client = new brokerService.IbrokerServiceClient("NetTcpBinding_IbrokerService");
        ServiceController brokerWindows = new ServiceController("brokerWindows");

        public MqttMain()
        {
            InitializeComponent();
        }

        //Initializes specific form elements.
        private void MqttMain_Load(object sender, EventArgs e)
        {
            connectionChoice.SelectedIndex = 0;
            connectionString.Text = "localhost";
            /*
            brokerService.IbrokerServiceClient client = new brokerService.IbrokerServiceClient("NetTcpBinding_IbrokerService");   
            try
            {
                client.CreateClientAsync(connectionString.Text, 1);
                // Log an event to indicate successful start.
                labelMessage.Text = client.GetData("Service Function works");
            } catch(Exception)
            {
                // Log the exception.
                labelMessage.Text = "Forms Error";
            }

            btnStart.Enabled = false;
            btnStop.Enabled = true;
            */
        }

        //Connect to specified address.
        private void connectButton_Click(object sender, EventArgs e)
        {
            if (connectionString.Text != string.Empty)
            {
                brokerIP = connectionString.Text;
                if (connectionChoice.SelectedIndex == 0)
                    try
                    {            
                        client.ConnectClientAsync(connectionString.Text, 0);
                        // Log an event to indicate successful start.
                        labelMessage.Text = client.GetData("Service Function works");
                    }
                    catch (Exception)
                    {
                        // Log the exception.
                        labelMessage.Text = "Forms Error";
                    }
                else if (connectionChoice.SelectedIndex == 1)
                    try
                    {
                        client.ConnectClientAsync(connectionString.Text, 1);
                        // Log an event to indicate successful start.
                        labelMessage.Text = client.GetData("Service Function works");
                    }
                    catch (Exception)
                    {
                        // Log the exception.
                        labelMessage.Text = "Forms Error";
                    }
            }
        }

        //Subscribe to topic.
        private void SubscribeButton_Click(object sender, EventArgs e)
        {
            try
            { 
                string topic = topicSubscribe.Text;
                client.SubscribeTopicAsync(topic);
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
        
        //Unsubscribes from topic.
        private void UnsubscribeButton_Click(object sender, EventArgs e)
        {
            try
            {       
                client.UnsubscribeTopicAsync(topicSubscribe.Text);
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
                client.PublishTopicAsync(topicChosen, publishText.Text);
                if (topicListSub.Items.Contains(topicChosen))
                    return;
                else
                    topicListPub.Items.Add(topicChosen);
                topicListSub.Items.Add(topicChosen);
            }
            catch
            {
                MessageBox.Show("Error publish");
            }
        }

        //Refreshes elements after tab switch.
        private void MqttTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            topicListSub.SelectedItem = null;
            topicListPub.SelectedItem = null;
            pubTopic.Text = "If you want to publish new topic, enter topic name here.";
        }

        //Starts service.
        private void btnStart_Click(object sender, EventArgs e)
        {
            brokerWindows.Start();
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            labelMessage.Text = "Service Started";
        }
        
        //Stop service.
        private void btnStop_Click(object sender, EventArgs e)
        {
            brokerWindows.Stop();
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            labelMessage.Text = "Service Stopped";
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

        private void mqttNotify_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            mqttNotify.Visible = false;
        }

        private void MqttMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }

    }
}
