using System;
using Opc.Ua.Client;
using Opc.Ua.Server;
using Opc.Ua.Configuration;

namespace OPCUAInterface
{
    class Program
    {
        public static string test_hostname;

        static void Main(string[] args)
        {
            Program.test_hostname = System.Net.Dns.GetHostName();
            Console.WriteLine("Hello World!");
            Console.WriteLine(Program.test_hostname);
            Console.ReadKey();
        }

        public void receiveMQTT()
        { }
        public void sendMQTT()
        { }
    }
}
