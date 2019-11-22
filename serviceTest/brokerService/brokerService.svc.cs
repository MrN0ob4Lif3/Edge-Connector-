﻿using System;
using MQTTnet;
using MQTTnet.Extensions.ManagedClient;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Client.Controls;
using System.Diagnostics;

namespace brokerService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class BrokerService : IBrokerService
    {
        //Create a new ManagedMQTT Client.
        IManagedMqttClient managedMQTT = new MqttFactory().CreateManagedMqttClient();

        //OPC Client variables
        private ApplicationConfiguration m_configuration;
        private Session m_session;
        private int m_reconnectPeriod = 10;
        private int m_discoverTimeout = 5000;
        private SessionReconnectHandler m_reconnectHandler;
        private CertificateValidationEventHandler m_CertificateValidation;
        private EventHandler m_ReconnectComplete;
        private EventHandler m_ReconnectStarting;
        private EventHandler m_KeepAliveComplete;
        private EventHandler m_ConnectComplete;

        private Subscription m_subscription;
        private MonitoredItem monitoredItem;
        private bool m_connectedOnce;

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }
        //MQTT client creation function. Requires IP address of MQTT server and connection option type
        public async void MQTTCreateClientAsync(String mqttIP, int option)
        {
            if (option == 0)
            {
                try
                {
                    // Use TCP connection.
                    await MQTTCore.ManagedClient.ManagedMqttConnectTCPAsync(managedMQTT, mqttIP);

                    // Message options
                    string mqttTopic = "TCPTopic";
                    string mqttMessage = "TCPMessage";

                    //Subscription
                    MQTTCore.ManagedClient.ManagedMqttSubscribe(managedMQTT, mqttTopic);

                    // Publishing
                    await MQTTCore.ManagedClient.ManagedMqttPublish(managedMQTT, mqttTopic, mqttMessage);
                }
                catch (Exception e)
                {
                    // Create an EventLog instance and assign its source.
                    EventLog myLog = new EventLog
                    {
                        Source = "brokerServiceTCP"
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
                    await MQTTCore.ManagedClient.ManagedMqttConnectWebSocket(managedMQTT, mqttIP);

                    // Message options
                    string mqttTopic = "WebSocketTopic";
                    string mqttMessage = "WebSocketMessage";

                    //Subscription
                    MQTTCore.ManagedClient.ManagedMqttSubscribe(managedMQTT, mqttTopic);

                    // Publishing
                    await MQTTCore.ManagedClient.ManagedMqttPublish(managedMQTT, mqttTopic, mqttMessage);
                }

                catch (Exception e)
                {
                    // Create an EventLog instance and assign its source.
                    EventLog myLog = new EventLog
                    {
                        Source = "brokerServiceWebSocket"
                    };
                    // Write an informational entry to the event log.
                    myLog.WriteEntry(e.Message);
                }
            }

        }
        //MQTT client connection function. Requires IP address of MQTT server and connection option type
        public async void MQTTConnectClientAsync(String mqttIP, int option)
        {
            if (option == 0)
            {
                try
                {
                    // Use TCP connection.
                    await MQTTCore.ManagedClient.ManagedMqttConnectTCPAsync(managedMQTT, mqttIP);
                }
                catch (Exception e)
                {
                    // Create an EventLog instance and assign its source.
                    EventLog myLog = new EventLog
                    {
                        Source = "brokerServiceTCP"
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
                    await MQTTCore.ManagedClient.ManagedMqttConnectWebSocket(managedMQTT, mqttIP);
                }

                catch (Exception e)
                {
                    // Create an EventLog instance and assign its source.
                    EventLog myLog = new EventLog
                    {
                        Source = "brokerServiceWebSocket"
                    };
                    // Write an informational entry to the event log.
                    myLog.WriteEntry(e.Message);
                }
            }
        }
        //MQTT topic subscription function. Requires topic to subscribe to.
        public void MQTTSubscribeTopicAsync(String topic)
        {
            try
            {
                MQTTCore.ManagedClient.ManagedMqttSubscribe(managedMQTT, topic);
            }
            catch (Exception e)
            {
                // Create an EventLog instance and assign its source.
                EventLog myLog = new EventLog
                {
                    Source = "brokerServiceSubscribe"
                };
                // Write an informational entry to the event log.
                myLog.WriteEntry(e.Message);
            }
        }
        //MQTT topic unsubscription function. Requires topic to subscribe to.
        public void MQTTUnsubscribeTopicAsync(String topic)
        {
            try
            {
                MQTTCore.ManagedClient.ManagedMqttUnsubscribe(managedMQTT, topic);
            }
            catch (Exception e)
            {
                // Create an EventLog instance and assign its source.
                EventLog myLog = new EventLog
                {
                    Source = "brokerServiceSubscribe"
                };
                // Write an informational entry to the event log.
                myLog.WriteEntry(e.Message);
            }
        }
        //MQTT message publishing function. Requires topic to publish message to.
        public async void MQTTPublishTopicAsync(String topic, String message)
        {
            try
            {
                await MQTTCore.ManagedClient.ManagedMqttPublish(managedMQTT, topic, message);
            }
            catch (Exception e)
            {
                // Create an EventLog instance and assign its source.
                EventLog myLog = new EventLog
                {
                    Source = "brokerServicePublish"
                };
                // Write an informational entry to the event log.
                myLog.WriteEntry(e.Message);
            }
        }
        //OPC client creation function.
        public async void OPCCreateClient(String opcIP, bool securityCheck)
        {
            //Find best endpoint
            EndpointDescription endpointDescription = CoreClientUtils.SelectEndpoint(opcIP, securityCheck, m_discoverTimeout);
            EndpointConfiguration endpointConfiguration = EndpointConfiguration.Create(m_configuration);
            ConfiguredEndpoint endpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration);
            //Create Session
            m_session = await Session.Create(
                m_configuration,
                endpoint,
                false,
                !DisableDomainCheck,
                (String.IsNullOrEmpty(SessionName)) ? m_configuration.ApplicationName : SessionName,
                60000,
                UserIdentity,
                PreferredLocales);
            //Keep session alive 
            m_session.KeepAlive += new KeepAliveEventHandler(Session_KeepAlive);
        }

        /// <summary>
        /// Gets or sets a flag indicating that the domain checks should be ignored when connecting.
        /// </summary>
        public bool DisableDomainCheck { get; set; }

        /// <summary>
        /// The name of the session to create.
        /// </summary>
        public string SessionName { get; set; }

        /// <summary>
        /// The user identity to use when creating the session.
        /// </summary>
        public IUserIdentity UserIdentity { get; set; }

        /// <summary>
        /// The locales to use when creating the session.
        /// </summary>
        public string[] PreferredLocales { get; set; }

        /// <summary>
        /// Handles a keep alive event from a session.
        /// </summary>
        private void Session_KeepAlive(Session session, KeepAliveEventArgs e)
        {
            try
            {
                // check for events from discarded sessions.
                if (!Object.ReferenceEquals(session, m_session))
                {
                    return;
                }
                // start reconnect sequence on communication error.
                if (ServiceResult.IsBad(e.Status))
                {
                    if (m_reconnectPeriod <= 0)
                    {
                        //UpdateStatus(true, e.CurrentTime, "Communication Error ({0})", e.Status);
                        return;
                    }
                    //UpdateStatus(true, e.CurrentTime, "Reconnecting in {0}s", m_reconnectPeriod);
                    if (m_reconnectHandler == null)
                    {
                        m_ReconnectStarting?.Invoke(this, e);
                        m_reconnectHandler = new SessionReconnectHandler();
                        m_reconnectHandler.BeginReconnect(m_session, m_reconnectPeriod * 1000, Server_ReconnectComplete);
                    }

                    return;
                }
                // update status.
                //UpdateStatus(false, e.CurrentTime, "Connected [{0}]", session.Endpoint.EndpointUrl);

                // raise any additional notifications.
                m_KeepAliveComplete?.Invoke(this, e);
            }
            catch (Exception exception)
            {
                //ClientUtils.HandleException(this.Text, exception);
                ClientUtils.HandleException("reconnect", exception);
            }
        }

        /// <summary>
        /// Handles a reconnect event complete from the reconnect handler.
        /// </summary>
        private void Server_ReconnectComplete(object sender, EventArgs e)
        {
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

                // raise any additional notifications.
                m_ReconnectComplete?.Invoke(this, e);
            }
            catch (Exception exception)
            {
                ClientUtils.HandleException("reconnect", exception);
            }
        }

        //OPC client connection function.
        public void OPCConnectClient()
        {
            //Endpoint find
            //Connect to endpoint
        }
        //OPC topic subscription function.
        public void OPCSubscribeTopic()
        {
            //subscribe to specified data value
        }
        //OPC topic unsubscription function.
        public void OPCUnsubscribeTopic()
        {
            //unsubscribe from specified data value
        }
    }
}
