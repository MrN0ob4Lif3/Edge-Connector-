using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.ServiceModel;
using System.Threading;
using ServiceLogic;


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
                // Create the thread object that will do the service's work.
                Thread brokerThread = new Thread(StartBroker);

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

        private void StartBroker()
        {
            host = new ServiceHost(typeof(brokerService.BrokerService));
            host.Open();
        }

        protected override void OnStop()
        {
            host.Close();
        }
    }
}
