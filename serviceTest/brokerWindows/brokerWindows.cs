﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Threading;



namespace brokerWindows
{
    public partial class brokerWindows : ServiceBase
    {
        ServiceHost host;

        public brokerWindows()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {   
            try
            {
                //System.Diagnostics.Debugger.Break();

                // Create the thread object that will do the service's work.
                Thread brokerThread = new Thread(startBroker);

                // Start the thread.
                brokerThread.Start();

                // Log an event to indicate successful start.
                EventLog.WriteEntry("Successful start.", EventLogEntryType.Information);
            }
            catch (Exception ex)
            {
                // Log the exception.
                EventLog.WriteEntry(ex.Message, EventLogEntryType.Error);
            }                 
        }

        private void startBroker()
        {
            host = new ServiceHost(typeof(brokerService.brokerService));
            host.Open();
        }

        protected override void OnStop()
        {
            host.Close();
        }
    }
}