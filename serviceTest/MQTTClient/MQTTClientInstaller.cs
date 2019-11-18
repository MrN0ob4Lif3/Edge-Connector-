using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace MQTTClientForm
{
    [RunInstaller(true)]
    public partial class MQTTClientInstaller : System.Configuration.Install.Installer
    {
        public MQTTClientInstaller()
        {
            InitializeComponent();
        }

        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);

            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            Process.Start(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\MQTTClientForm.exe");
        }
    }
}
