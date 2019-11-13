﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.ServiceModel;

namespace MQTTService
{
    public partial class MQTTService : ServiceBase
    {
        ServiceHost host;

        public MQTTService()
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
