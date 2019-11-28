namespace MQTTClientForm
{
    partial class MqttMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MqttMain));
            this.connectButtonMQTT = new System.Windows.Forms.Button();
            this.connectionStringMQTT = new System.Windows.Forms.TextBox();
            this.connectionType = new System.Windows.Forms.Label();
            this.connectionChoice = new System.Windows.Forms.ComboBox();
            this.MqttTabs = new System.Windows.Forms.TabControl();
            this.subscribeTab = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.topicListSub = new System.Windows.Forms.ListBox();
            this.UnsubscribeButton = new System.Windows.Forms.Button();
            this.topicSubscribe = new System.Windows.Forms.TextBox();
            this.SubscribeButton = new System.Windows.Forms.Button();
            this.publishTab = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.topicListPub = new System.Windows.Forms.ListBox();
            this.pubTopic = new System.Windows.Forms.TextBox();
            this.publishText = new System.Windows.Forms.TextBox();
            this.PublishButton = new System.Windows.Forms.Button();
            this.publishTopic = new System.Windows.Forms.TextBox();
            this.mqttStart = new System.Windows.Forms.Button();
            this.mqttStop = new System.Windows.Forms.Button();
            this.mqttNotify = new System.Windows.Forms.NotifyIcon(this.components);
            this.opcStart = new System.Windows.Forms.Button();
            this.opcStop = new System.Windows.Forms.Button();
            this.opcEndpoints = new Opc.Ua.Client.Controls.EndpointSelectorCtrl();
            this.opcSession = new Opc.Ua.Sample.Controls.SessionTreeCtrl();
            this.opcBrowse = new Opc.Ua.Sample.Controls.BrowseTreeCtrl();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.ServerUrlLB = new System.Windows.Forms.ToolStripStatusLabel();
            this.ServerStatusLB = new System.Windows.Forms.ToolStripStatusLabel();
            this.MqttTabs.SuspendLayout();
            this.subscribeTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.publishTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // connectButtonMQTT
            // 
            this.connectButtonMQTT.Location = new System.Drawing.Point(656, 51);
            this.connectButtonMQTT.Name = "connectButtonMQTT";
            this.connectButtonMQTT.Size = new System.Drawing.Size(75, 23);
            this.connectButtonMQTT.TabIndex = 0;
            this.connectButtonMQTT.Text = "Connect";
            this.connectButtonMQTT.UseVisualStyleBackColor = true;
            this.connectButtonMQTT.Click += new System.EventHandler(this.ConnectButton_Click);
            // 
            // connectionStringMQTT
            // 
            this.connectionStringMQTT.Location = new System.Drawing.Point(12, 53);
            this.connectionStringMQTT.Name = "connectionStringMQTT";
            this.connectionStringMQTT.Size = new System.Drawing.Size(638, 20);
            this.connectionStringMQTT.TabIndex = 1;
            this.connectionStringMQTT.Text = "Enter MQTT Broker IP here";
            // 
            // connectionType
            // 
            this.connectionType.AutoSize = true;
            this.connectionType.Location = new System.Drawing.Point(435, 84);
            this.connectionType.Name = "connectionType";
            this.connectionType.Size = new System.Drawing.Size(88, 13);
            this.connectionType.TabIndex = 2;
            this.connectionType.Text = "Connection Type";
            // 
            // connectionChoice
            // 
            this.connectionChoice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.connectionChoice.FormattingEnabled = true;
            this.connectionChoice.Items.AddRange(new object[] {
            "TCP",
            "WebSocket"});
            this.connectionChoice.Location = new System.Drawing.Point(529, 81);
            this.connectionChoice.Name = "connectionChoice";
            this.connectionChoice.Size = new System.Drawing.Size(121, 21);
            this.connectionChoice.TabIndex = 3;
            // 
            // MqttTabs
            // 
            this.MqttTabs.Controls.Add(this.subscribeTab);
            this.MqttTabs.Controls.Add(this.publishTab);
            this.MqttTabs.Location = new System.Drawing.Point(12, 108);
            this.MqttTabs.Name = "MqttTabs";
            this.MqttTabs.SelectedIndex = 0;
            this.MqttTabs.Size = new System.Drawing.Size(776, 330);
            this.MqttTabs.TabIndex = 4;
            this.MqttTabs.SelectedIndexChanged += new System.EventHandler(this.MqttTabs_SelectedIndexChanged);
            // 
            // subscribeTab
            // 
            this.subscribeTab.Controls.Add(this.splitContainer1);
            this.subscribeTab.Location = new System.Drawing.Point(4, 22);
            this.subscribeTab.Name = "subscribeTab";
            this.subscribeTab.Padding = new System.Windows.Forms.Padding(3);
            this.subscribeTab.Size = new System.Drawing.Size(768, 304);
            this.subscribeTab.TabIndex = 0;
            this.subscribeTab.Text = "Topics";
            this.subscribeTab.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.topicListSub);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.UnsubscribeButton);
            this.splitContainer1.Panel2.Controls.Add(this.topicSubscribe);
            this.splitContainer1.Panel2.Controls.Add(this.SubscribeButton);
            this.splitContainer1.Size = new System.Drawing.Size(762, 298);
            this.splitContainer1.SplitterDistance = 254;
            this.splitContainer1.TabIndex = 0;
            // 
            // topicListSub
            // 
            this.topicListSub.FormattingEnabled = true;
            this.topicListSub.Location = new System.Drawing.Point(4, 4);
            this.topicListSub.Name = "topicListSub";
            this.topicListSub.Size = new System.Drawing.Size(247, 290);
            this.topicListSub.TabIndex = 0;
            // 
            // UnsubscribeButton
            // 
            this.UnsubscribeButton.Location = new System.Drawing.Point(426, 30);
            this.UnsubscribeButton.Name = "UnsubscribeButton";
            this.UnsubscribeButton.Size = new System.Drawing.Size(75, 23);
            this.UnsubscribeButton.TabIndex = 2;
            this.UnsubscribeButton.Text = "Unsubscribe";
            this.UnsubscribeButton.UseVisualStyleBackColor = true;
            this.UnsubscribeButton.Click += new System.EventHandler(this.UnsubscribeButton_Click);
            // 
            // topicSubscribe
            // 
            this.topicSubscribe.Location = new System.Drawing.Point(4, 4);
            this.topicSubscribe.Name = "topicSubscribe";
            this.topicSubscribe.Size = new System.Drawing.Size(497, 20);
            this.topicSubscribe.TabIndex = 1;
            this.topicSubscribe.Text = "Enter the name of the topic to subscribe / unsubscribe";
            // 
            // SubscribeButton
            // 
            this.SubscribeButton.Location = new System.Drawing.Point(345, 30);
            this.SubscribeButton.Name = "SubscribeButton";
            this.SubscribeButton.Size = new System.Drawing.Size(75, 23);
            this.SubscribeButton.TabIndex = 0;
            this.SubscribeButton.Text = "Subscribe";
            this.SubscribeButton.UseVisualStyleBackColor = true;
            this.SubscribeButton.Click += new System.EventHandler(this.SubscribeButton_Click);
            // 
            // publishTab
            // 
            this.publishTab.Controls.Add(this.splitContainer2);
            this.publishTab.Location = new System.Drawing.Point(4, 22);
            this.publishTab.Name = "publishTab";
            this.publishTab.Padding = new System.Windows.Forms.Padding(3);
            this.publishTab.Size = new System.Drawing.Size(768, 304);
            this.publishTab.TabIndex = 1;
            this.publishTab.Text = "Publishing";
            this.publishTab.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(3, 3);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.topicListPub);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.pubTopic);
            this.splitContainer2.Panel2.Controls.Add(this.publishText);
            this.splitContainer2.Panel2.Controls.Add(this.PublishButton);
            this.splitContainer2.Panel2.Controls.Add(this.publishTopic);
            this.splitContainer2.Size = new System.Drawing.Size(762, 298);
            this.splitContainer2.SplitterDistance = 254;
            this.splitContainer2.TabIndex = 0;
            // 
            // topicListPub
            // 
            this.topicListPub.FormattingEnabled = true;
            this.topicListPub.Location = new System.Drawing.Point(4, 4);
            this.topicListPub.Name = "topicListPub";
            this.topicListPub.Size = new System.Drawing.Size(247, 290);
            this.topicListPub.TabIndex = 0;
            // 
            // pubTopic
            // 
            this.pubTopic.Location = new System.Drawing.Point(4, 4);
            this.pubTopic.Name = "pubTopic";
            this.pubTopic.Size = new System.Drawing.Size(497, 20);
            this.pubTopic.TabIndex = 3;
            this.pubTopic.Text = "If you want to publish new topic, enter topic name here.";
            // 
            // publishText
            // 
            this.publishText.Location = new System.Drawing.Point(4, 33);
            this.publishText.Multiline = true;
            this.publishText.Name = "publishText";
            this.publishText.Size = new System.Drawing.Size(497, 232);
            this.publishText.TabIndex = 2;
            // 
            // PublishButton
            // 
            this.PublishButton.Location = new System.Drawing.Point(426, 271);
            this.PublishButton.Name = "PublishButton";
            this.PublishButton.Size = new System.Drawing.Size(75, 23);
            this.PublishButton.TabIndex = 0;
            this.PublishButton.Text = "Publish";
            this.PublishButton.UseVisualStyleBackColor = true;
            this.PublishButton.Click += new System.EventHandler(this.PublishButton_Click);
            // 
            // publishTopic
            // 
            this.publishTopic.Location = new System.Drawing.Point(3, 3);
            this.publishTopic.Multiline = true;
            this.publishTopic.Name = "publishTopic";
            this.publishTopic.Size = new System.Drawing.Size(498, 262);
            this.publishTopic.TabIndex = 1;
            // 
            // mqttStart
            // 
            this.mqttStart.Location = new System.Drawing.Point(12, 12);
            this.mqttStart.Name = "mqttStart";
            this.mqttStart.Size = new System.Drawing.Size(75, 23);
            this.mqttStart.TabIndex = 5;
            this.mqttStart.Text = "MQTT Start";
            this.mqttStart.UseVisualStyleBackColor = true;
            this.mqttStart.Click += new System.EventHandler(this.MqttStart_Click);
            // 
            // mqttStop
            // 
            this.mqttStop.Location = new System.Drawing.Point(93, 12);
            this.mqttStop.Name = "mqttStop";
            this.mqttStop.Size = new System.Drawing.Size(75, 23);
            this.mqttStop.TabIndex = 6;
            this.mqttStop.Text = "MQTT Stop";
            this.mqttStop.UseVisualStyleBackColor = true;
            this.mqttStop.Click += new System.EventHandler(this.MqttStop_Click);
            // 
            // mqttNotify
            // 
            this.mqttNotify.Icon = ((System.Drawing.Icon)(resources.GetObject("mqttNotify.Icon")));
            this.mqttNotify.Text = "MQTTBroker";
            this.mqttNotify.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.MqttNotify_MouseDoubleClick);
            // 
            // opcStart
            // 
            this.opcStart.Location = new System.Drawing.Point(816, 12);
            this.opcStart.Name = "opcStart";
            this.opcStart.Size = new System.Drawing.Size(75, 23);
            this.opcStart.TabIndex = 8;
            this.opcStart.Text = "OPC Start";
            this.opcStart.UseVisualStyleBackColor = true;
            this.opcStart.Click += new System.EventHandler(this.OpcStart_Click);
            // 
            // opcStop
            // 
            this.opcStop.Location = new System.Drawing.Point(897, 12);
            this.opcStop.Name = "opcStop";
            this.opcStop.Size = new System.Drawing.Size(75, 23);
            this.opcStop.TabIndex = 9;
            this.opcStop.Text = "OPC Stop";
            this.opcStop.UseVisualStyleBackColor = true;
            this.opcStop.Click += new System.EventHandler(this.OpcStop_Click);
            // 
            // opcEndpoints
            // 
            this.opcEndpoints.Location = new System.Drawing.Point(816, 53);
            this.opcEndpoints.MaximumSize = new System.Drawing.Size(2048, 28);
            this.opcEndpoints.MinimumSize = new System.Drawing.Size(100, 28);
            this.opcEndpoints.Name = "opcEndpoints";
            this.opcEndpoints.Padding = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.opcEndpoints.SelectedEndpoint = null;
            this.opcEndpoints.Size = new System.Drawing.Size(649, 28);
            this.opcEndpoints.TabIndex = 14;
            this.opcEndpoints.ConnectEndpoint += new Opc.Ua.Client.Controls.ConnectEndpointEventHandler(this.OpcEndpoints_ConnectEndpoint);
            // 
            // opcSession
            // 
            this.opcSession.AddressSpaceCtrl = null;
            this.opcSession.Configuration = null;
            this.opcSession.EnableDragging = false;
            this.opcSession.Location = new System.Drawing.Point(816, 130);
            this.opcSession.MessageContext = null;
            this.opcSession.Name = "opcSession";
            this.opcSession.NotificationMessagesCtrl = null;
            this.opcSession.PreferredLocales = null;
            this.opcSession.ServerStatusCtrl = null;
            this.opcSession.Size = new System.Drawing.Size(208, 301);
            this.opcSession.TabIndex = 15;
            // 
            // opcBrowse
            // 
            this.opcBrowse.AttributesCtrl = null;
            this.opcBrowse.EnableDragging = false;
            this.opcBrowse.Location = new System.Drawing.Point(1030, 130);
            this.opcBrowse.Name = "opcBrowse";
            this.opcBrowse.SessionTreeCtrl = this.opcSession;
            this.opcBrowse.Size = new System.Drawing.Size(435, 301);
            this.opcBrowse.TabIndex = 16;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ServerUrlLB,
            this.ServerStatusLB});
            this.statusStrip1.Location = new System.Drawing.Point(0, 428);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1505, 22);
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
            // MqttMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1505, 450);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.opcBrowse);
            this.Controls.Add(this.opcSession);
            this.Controls.Add(this.opcEndpoints);
            this.Controls.Add(this.opcStop);
            this.Controls.Add(this.opcStart);
            this.Controls.Add(this.mqttStop);
            this.Controls.Add(this.mqttStart);
            this.Controls.Add(this.MqttTabs);
            this.Controls.Add(this.connectionChoice);
            this.Controls.Add(this.connectionType);
            this.Controls.Add(this.connectionStringMQTT);
            this.Controls.Add(this.connectButtonMQTT);
            this.Name = "MqttMain";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MqttMain_FormClosed);
            this.Load += new System.EventHandler(this.MqttMain_Load);
            this.Resize += new System.EventHandler(this.MqttMain_Resize);
            this.MqttTabs.ResumeLayout(false);
            this.subscribeTab.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.publishTab.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button connectButtonMQTT;
        private System.Windows.Forms.TextBox connectionStringMQTT;
        private System.Windows.Forms.Label connectionType;
        private System.Windows.Forms.ComboBox connectionChoice;
        private System.Windows.Forms.TabControl MqttTabs;
        private System.Windows.Forms.TabPage subscribeTab;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button SubscribeButton;
        private System.Windows.Forms.TabPage publishTab;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Button PublishButton;
        private System.Windows.Forms.TextBox publishTopic;
        private System.Windows.Forms.ListBox topicListSub;
        private System.Windows.Forms.ListBox topicListPub;
        private System.Windows.Forms.TextBox publishText;
        private System.Windows.Forms.TextBox topicSubscribe;
        private System.Windows.Forms.Button UnsubscribeButton;
        private System.Windows.Forms.TextBox pubTopic;
        private System.Windows.Forms.Button mqttStart;
        private System.Windows.Forms.Button mqttStop;
        private System.Windows.Forms.NotifyIcon mqttNotify;
        private System.Windows.Forms.Button opcStart;
        private System.Windows.Forms.Button opcStop;
        private Opc.Ua.Client.Controls.EndpointSelectorCtrl opcEndpoints;
        private Opc.Ua.Sample.Controls.SessionTreeCtrl opcSession;
        private Opc.Ua.Sample.Controls.BrowseTreeCtrl opcBrowse;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel ServerUrlLB;
        private System.Windows.Forms.ToolStripStatusLabel ServerStatusLB;
    }
}

