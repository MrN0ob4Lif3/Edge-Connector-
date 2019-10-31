using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet.Client.Subscribing;
using MQTTnet.Server;

namespace MQTTService
{
    public partial class MQTTService : ServiceBase
    {
        private Thread _workerThread;
        // Used to cache any unhandled exception
        private Exception _asyncException;

        public MQTTService()
        {
            InitializeComponent();

            // Wire up the UnhandledExcepetion event of the current AppDomain.  
            // This will fire any time an undandled exception is thrown
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionHandler);
        }

        protected override void OnStart(string[] args)
        {
            // Start up a thread that will simulate a failure by throwing an unhandled exception asynchronously
            _workerThread = new Thread(new ThreadStart(createClientAsync));
            _workerThread.Start();     
        }

        protected override void OnStop()
        {
            // If there was an unhandled exception, throw it here
            // Make sure to wrap the unhandled exception, as opposed to just rethrowing it, to preserve its StackTrace
            // To wrap it, create a new Exception and pass the unhandled exception as the InnerException
            // The exception info will be in the Windows Event Viewer's Application log
            // You could also use some logging/tracing mechanism to capture information about the exception 
            if (_asyncException != null)
                throw new InvalidOperationException("Unhandled exception in service", _asyncException);
            _workerThread.Abort();
            Stop();
        }

        void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            // Cache the unhandled exception and begin a shutdown of the service
            _asyncException = e.ExceptionObject as Exception;
            Stop();
        }

        private void SimulateAsyncFailure()
        {
            // Simulate an asynchronous unhandled excpetion by sleeping for some time and then throwing
            // There is no caller to catch this exception since it is running in a separate thread
            Thread.Sleep((int)TimeSpan.FromSeconds(30).TotalMilliseconds);
            throw new InvalidOperationException("Simulating a service error");
        }

        private async void createClientAsync()
        {
            // Create a new MQTT client.
            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();
            Console.WriteLine("Starting MQTT Client");

            // Use WebSocket connection.
            Console.Write("MQTT Broker IP: ");
            string brokerIP = Console.ReadLine();
            var options = new MqttClientOptionsBuilder()
                .WithWebSocketServer(brokerIP)
                .Build();
            Console.WriteLine("Connecting to " + brokerIP + " via TCP");

            await mqttClient.ConnectAsync(options);

            // Reconnection
            mqttClient.UseDisconnectedHandler(async e =>
            {
                Console.WriteLine("### DISCONNECTED FROM SERVER ###");
                await Task.Delay(TimeSpan.FromSeconds(5));

                try
                {
                    await mqttClient.ConnectAsync(options); // Since 3.0.5 with CancellationToken
                }
                catch
                {
                    Console.WriteLine("### RECONNECTING FAILED ###");
                }
            });

            // Message options
            Console.Write("Topic: ");
            string mqttTopic = Console.ReadLine();
            Console.Write("Message: ");
            string mqttMessage = Console.ReadLine();
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(mqttTopic)
                .WithPayload(mqttMessage)
                .WithExactlyOnceQoS()
                .WithRetainFlag()
                .Build();

            // Publishing
            await mqttClient.PublishAsync(message);
            Console.WriteLine("Message: " + mqttMessage + " published to Topic " + mqttTopic);
        }
    }
}
