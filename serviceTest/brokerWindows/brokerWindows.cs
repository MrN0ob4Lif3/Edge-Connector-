using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.ServiceModel;
using System.Threading;
using System.ServiceModel.Description;
using MQTTnet.Extensions.ManagedClient;
using ServiceLogic;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace brokerWindows
{
    public partial class brokerWindows : ServiceBase, IServiceCallback
    {
        ServiceHost host;
        brokerService.BrokerService instance;
        public IManagedMqttClient managedMqtt;
        public List<String> m_topicList = new List<string>();
        static SemaphoreSlim mqttClientSemaphore = new SemaphoreSlim(1, 1);

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
                managedMqtt= ManagedClient.CreateManagedClient();
                string brokerIP = "dev-harmony-01.southeastasia.cloudapp.azure.com:8080/mqtt";
                MQTTConnectClient(managedMqtt,brokerIP);
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



        #region MQTT Methods
        public async void MQTTConnectClient(IManagedMqttClient managedMqtt,string brokerIP)
        {
            await ManagedClient.ManagedMqttConnectWebSocket(managedMqtt, brokerIP);
        }

        //Callback for MQTT connection.
        async void IServiceCallback.MQTTConnect(string brokerIP)
        {
            try
            {
                await ManagedClient.ManagedMqttConnectWebSocket(managedMqtt, brokerIP);
            }
            finally
            {
                //When the task is ready, release the semaphore. It is vital to ALWAYS release the semaphore when we are ready, or else we will end up with a Semaphore that is forever locked.
                //This is why it is important to do the Release within a try...finally clause; program execution may crash or take a different path, this way you are guaranteed execution
                mqttClientSemaphore.Release();
            }
        }

        //Callback for MQTT subscription.
        async void IServiceCallback.MQTTSubscribe(String topic)
        {
            await mqttClientSemaphore.WaitAsync();
            try
            {
                ManagedClient.ManagedMqttSubscribe(managedMqtt, topic);
                m_topicList.Add(topic);
            }
            finally
            {
                //When the task is ready, release the semaphore. It is vital to ALWAYS release the semaphore when we are ready, or else we will end up with a Semaphore that is forever locked.
                //This is why it is important to do the Release within a try...finally clause; program execution may crash or take a different path, this way you are guaranteed execution
                mqttClientSemaphore.Release();
            }
        }

        //Callback for MQTT unsubscription.
        async void IServiceCallback.MQTTUnsubscribe(String topic)
        {
            await mqttClientSemaphore.WaitAsync();
            try
            {
                ManagedClient.ManagedMqttUnsubscribe(managedMqtt, topic);
                m_topicList.Remove(topic);
            }
            finally
            {
                //When the task is ready, release the semaphore. It is vital to ALWAYS release the semaphore when we are ready, or else we will end up with a Semaphore that is forever locked.
                //This is why it is important to do the Release within a try...finally clause; program execution may crash or take a different path, this way you are guaranteed execution
                mqttClientSemaphore.Release();
            }
        }

        //Callback for MQTT publishing.
        async void IServiceCallback.MQTTPublish(string topic, string message)
        {
            await mqttClientSemaphore.WaitAsync();
            try
            {
                await ManagedClient.ManagedMqttPublish(managedMqtt, topic, message);

            }
            finally
            {
                //When the task is ready, release the semaphore. It is vital to ALWAYS release the semaphore when we are ready, or else we will end up with a Semaphore that is forever locked.
                //This is why it is important to do the Release within a try...finally clause; program execution may crash or take a different path, this way you are guaranteed execution
                mqttClientSemaphore.Release();
            }
        }

        //Callback to return MQTT subscribed topics.
        List<String> IServiceCallback.MQTTSubscribedTopics()
        {
            return m_topicList;
        }

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
