using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace WsServer
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create the WsServer, specifying the server's address
            WsServer server = new WsServer(8300);

            // Start the server
            server.StartSocketServer();

            // Keep running until the Enter key is pressed
            string input = Console.ReadLine();
        }
    }
}
