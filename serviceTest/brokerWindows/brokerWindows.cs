using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.ServiceModel;
using System.Threading;
using System.ServiceModel.Description;
using MQTTnet.Extensions.ManagedClient;
using ServiceLogic;

namespace brokerWindows
{
    public partial class brokerWindows : ServiceBase, IServiceCallback
    {
        ServiceHost host;
        brokerService.BrokerService instance;
        public IManagedMqttClient managedMQTT;

        public brokerWindows()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {   
            try
            {
                //Set the static callback reference.
                Host.Current = this;
                StartBroker();
                /*
                // Create the thread object that will do the service's work.
                Thread brokerThread = new Thread(StartBroker);

                // Start the thread.
                brokerThread.Start();
                */
                // Log an event to indicate successful start.
                EventLog.WriteEntry("Successful start.", EventLogEntryType.Information);

                // initialize and work with myObject
                managedMQTT = ServiceLogic.ManagedClient.CreateManagedClient();
            }
            catch (Exception ex)
            {
                // Log the exception.
                EventLog.WriteEntry(ex.Message, EventLogEntryType.Error);
            }                 
        }

        private void StartBroker()
        {
            host = new ServiceHost(typeof(brokerService.BrokerService));
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
            //MQTTCreateClientAsync("localhost", 0);
        }

        //Here you have data from WCF and myObject available.
        void IServiceCallback.MQTTConnect(string brokerIP)
        {
            //Be careful here. 
            //Depending on your WCF service and WCF clients this might 
            //get called simultaneously from different threads.
            lock (managedMQTT)
            {
                // ManagedClient.ManagedMqttConnectTCPAsync(managedMQTT,brokerIP);
                //myObject.DoSomething(iSomeDataFromWCFToHost);
            }
        }

        #region MQTT Methods
        //MQTT client creation function. Requires IP address of MQTT server and connection option type
        public async void MQTTCreateClientAsync(String mqttIP, int option)
        {
            if (option == 0)
            {
                try
                {
                    // Use TCP connection.
                    await ServiceLogic.ManagedClient.ManagedMqttConnectTCPAsync(instance.managedMQTT, mqttIP);
                }
                catch (Exception e)
                {
                    // Create an EventLog instance and assign its source.
                    EventLog myLog = new EventLog
                    {
                        Source = "brokerServiceMQTTTCP"
                    };
                    // Write an informational entry to the event log.
                    myLog.WriteEntry(e.Message);
                }
            }
            else if (option == 1)
            {
                try
                {
                    // Use WebSocket connection.
                    await ServiceLogic.ManagedClient.ManagedMqttConnectWebSocket(instance.managedMQTT, mqttIP);
                }

                catch (Exception e)
                {
                    // Create an EventLog instance and assign its source.
                    EventLog myLog = new EventLog
                    {
                        Source = "brokerServiceMQTTWebSocket"
                    };
                    // Write an informational entry to the event log.
                    myLog.WriteEntry(e.Message);
                }
            }

        }
        #endregion

        protected override void OnStop()
        {
            host.Close();
        }
    }
}
