using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet.Client.Subscribing;
using MQTTnet.Server;

namespace OPCUAMQTT
{
    public partial class MainPage : ContentPage
    {
        StackLayout stacklayout = new StackLayout();
        static LabelViewModel textInfo = new LabelViewModel();
        SampleClient OpcClient = new SampleClient(textInfo);
        public string _OpcURL;
        public string _mqttURL;

        public string OpcURL    
        {
            get { return _OpcURL; }
            set { _OpcURL = value;}
        }

        public string mqttURL
        {
            get { return _mqttURL; }
            set { _mqttURL = value; }
        }

        public MainPage()
        {
            InitializeComponent();
        }

        async void MqttConnect(object sender, EventArgs args)
        {
            mqttURL = MqttUrl.Text;

            if(mqttURL != null)
            {
                // Create a new MQTT client.
                var factory = new MqttFactory();
                var mqttClient = factory.CreateMqttClient();
                mainText.Text = "Starting MQTT Client";

                // Use TCP connection.
                var options = new MqttClientOptionsBuilder()
                    .WithTcpServer(mqttURL)
                    .Build();
                mainText.Text = "Connecting to localhost via TCP";
                try
                {
                    await mqttClient.ConnectAsync(options);
                }
                catch
                {
                    mainText.Text = "Connection failed, check if broker is still online or try another broker";
                    return;
                }

                // Message options
                string mqttTopic = "Test";
                string mqttMessage = "Test";
                var message = new MqttApplicationMessageBuilder()
                    .WithTopic(mqttTopic)
                    .WithPayload(mqttMessage)
                    .WithExactlyOnceQoS()
                    .WithRetainFlag()
                    .Build();

                // Publishing
                await mqttClient.PublishAsync(message);
                mainText.Text = "Message: test published to Topic: test)";
            }
            else
            {
                await DisplayAlert("Warning", "MQTT Server endpoint URL cannot be null", "Ok");
            }

        }

        async void OPCUAConnect(object sender, EventArgs e)
        {
            OpcURL = OpcUrl.Text;

            if (OpcURL != null)
            {
                if (ConnectButton.Text == "Connect")
                {
                    bool connectToServer = true;
                    ConnectIndicator.IsRunning = true;

                    await Task.Run(() => OpcClient.CreateCertificate());

                    if (OpcClient.haveAppCertificate == false)
                    {
                        connectToServer = await DisplayAlert("Warning", "missing application certificate, \nusing unsecure connection. \nDo you want to continue?", "Yes", "No");
                    }

                    if (connectToServer == true)
                    {
                        var connectionStatus = await Task.Run(() => OpcClient.OpcClient(OpcURL));

                        if (connectionStatus == SampleClient.ConnectionStatus.Connected)
                        {
                            Tree tree;
                            ConnectButton.Text = "Disconnect";

                            tree = OpcClient.GetRootNode(textInfo);
                            if (tree.currentView[0].children == true)
                            {
                                tree = OpcClient.GetChildren(tree.currentView[0].id);
                            }

                            ConnectIndicator.IsRunning = false;
                            Page treeViewRoot = new TreeView(tree, OpcClient);
                            treeViewRoot.Title = "/Root";
                            await Navigation.PushAsync(treeViewRoot);
                        }
                        else
                        {
                            ConnectIndicator.IsRunning = false;
                            await DisplayAlert("Warning", "Cannot connect to an OPC UA server", "Ok");
                        }
                    }
                    else
                    {
                        ConnectIndicator.IsRunning = false;
                    }
                }
                else
                {
                    OpcClient.Disconnect(OpcClient.session);
                    ConnectButton.Text = "Connect";
                }
            }
            else
            {
                await DisplayAlert("Warning", "OPC Server endpoint URL cannot be null", "Ok");
            }
        }
    }
}
