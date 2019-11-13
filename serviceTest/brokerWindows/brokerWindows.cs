using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;



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
            host = new ServiceHost(typeof(brokerService.brokerService));
            host.Open();
        }

        protected override void OnStop()
        {
            host.Close();
        }
    }
}
