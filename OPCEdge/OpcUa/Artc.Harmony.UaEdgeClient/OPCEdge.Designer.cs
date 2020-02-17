namespace OpcEdgeClient
{
    partial class OpcEdgeMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OpcEdgeMain));
            this.brokerNotify = new System.Windows.Forms.NotifyIcon(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.ServerUrlLB = new System.Windows.Forms.ToolStripStatusLabel();
            this.ServerStatusLB = new System.Windows.Forms.ToolStripStatusLabel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.opcBrowse = new Opc.Ua.Sample.Controls.BrowseTreeCtrl();
            this.opcSession = new Opc.Ua.Sample.Controls.SessionTreeCtrl();
            this.opcEndpoints = new Opc.Ua.Client.Controls.EndpointSelectorCtrl();
            this.OpcConnectionLabel = new System.Windows.Forms.Label();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // brokerNotify
            // 
            this.brokerNotify.Icon = ((System.Drawing.Icon)(resources.GetObject("brokerNotify.Icon")));
            this.brokerNotify.Text = "OPCEdge";
            this.brokerNotify.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.OpcEdgeNotify_MouseDoubleClick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ServerUrlLB,
            this.ServerStatusLB});
            this.statusStrip1.Location = new System.Drawing.Point(0, 428);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(677, 22);
            this.statusStrip1.TabIndex = 17;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // ServerUrlLB
            // 
            this.ServerUrlLB.Name = "ServerUrlLB";
            this.ServerUrlLB.Size = new System.Drawing.Size(79, 17);
            this.ServerUrlLB.Text = "Disconnected";
            // 
            // ServerStatusLB
            // 
            this.ServerStatusLB.Name = "ServerStatusLB";
            this.ServerStatusLB.Size = new System.Drawing.Size(49, 17);
            this.ServerStatusLB.Text = "00:00:00";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // opcBrowse
            // 
            this.opcBrowse.AttributesCtrl = null;
            this.opcBrowse.Browser = null;
            this.opcBrowse.EnableDragging = false;
            this.opcBrowse.Location = new System.Drawing.Point(226, 107);
            this.opcBrowse.Name = "opcBrowse";
            this.opcBrowse.SessionTreeCtrl = this.opcSession;
            this.opcBrowse.Size = new System.Drawing.Size(435, 318);
            this.opcBrowse.TabIndex = 16;
            this.opcBrowse.MonitorEvent += new Opc.Ua.Sample.Controls.MonitorEventEventHandler(this.OPCMonitor);
            // 
            // opcSession
            // 
            this.opcSession.AddressSpaceCtrl = null;
            this.opcSession.Configuration = null;
            this.opcSession.EnableDragging = false;
            this.opcSession.Location = new System.Drawing.Point(12, 107);
            this.opcSession.MessageContext = null;
            this.opcSession.Name = "opcSession";
            this.opcSession.NotificationMessagesCtrl = null;
            this.opcSession.PreferredLocales = null;
            this.opcSession.ServerStatusCtrl = null;
            this.opcSession.Size = new System.Drawing.Size(208, 318);
            this.opcSession.TabIndex = 15;
            this.opcSession.SubscribeEvent += new Opc.Ua.Sample.Controls.SubscribeEventEventHandler(this.OPCSubscribe);
            this.opcSession.DeleteSession += new Opc.Ua.Sample.Controls.DeleteSessionEventHandler(this.OPCDisconnect);
            this.opcSession.DeleteSubscription += new Opc.Ua.Sample.Controls.DeleteSubscriptionEventHandler(this.OPCUnsubscribe);
            this.opcSession.DeleteItem += new Opc.Ua.Sample.Controls.DeleteItemEventHandler(this.OPCUnmonitor);
            // 
            // opcEndpoints
            // 
            this.opcEndpoints.Location = new System.Drawing.Point(12, 27);
            this.opcEndpoints.MaximumSize = new System.Drawing.Size(2048, 28);
            this.opcEndpoints.MinimumSize = new System.Drawing.Size(100, 28);
            this.opcEndpoints.Name = "opcEndpoints";
            this.opcEndpoints.Padding = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.opcEndpoints.SelectedEndpoint = null;
            this.opcEndpoints.Size = new System.Drawing.Size(649, 28);
            this.opcEndpoints.TabIndex = 14;
            this.opcEndpoints.ConnectEndpoint += new Opc.Ua.Client.Controls.ConnectEndpointEventHandler(this.OpcEndpoints_ConnectEndpoint);
            // 
            // OpcConnectionLabel
            // 
            this.OpcConnectionLabel.AutoSize = true;
            this.OpcConnectionLabel.Location = new System.Drawing.Point(13, 11);
            this.OpcConnectionLabel.Name = "OpcConnectionLabel";
            this.OpcConnectionLabel.Size = new System.Drawing.Size(239, 13);
            this.OpcConnectionLabel.TabIndex = 18;
            this.OpcConnectionLabel.Text = "Currently Connected to:  No endpoint connected.";
            // 
            // OpcEdgeMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(677, 450);
            this.Controls.Add(this.OpcConnectionLabel);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.opcBrowse);
            this.Controls.Add(this.opcSession);
            this.Controls.Add(this.opcEndpoints);
            this.Name = "OpcEdgeMain";
            this.Text = "OPC-UA - MQTT Edge Connector";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OpcEdgeMain_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OpcEdgeMain_FormClosed);
            this.Load += new System.EventHandler(this.OpcEdgeMain_Load);
            this.Resize += new System.EventHandler(this.OpcEdgeMain_Resize);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.NotifyIcon brokerNotify;
        private Opc.Ua.Client.Controls.EndpointSelectorCtrl opcEndpoints;
        private Opc.Ua.Sample.Controls.SessionTreeCtrl opcSession;
        private Opc.Ua.Sample.Controls.BrowseTreeCtrl opcBrowse;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel ServerUrlLB;
        private System.Windows.Forms.ToolStripStatusLabel ServerStatusLB;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Label OpcConnectionLabel;
    }
}

