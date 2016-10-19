using System;
using System.Collections.Generic;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Security.Cryptography;

namespace WsServer
{
    public class WsServer
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
            (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Members

        // This socket listens for new connections
        Socket _listener;

        // Configurable port # that is passed in the constructor
        int _port;

        // List of connections
        List<WsConnection> _unknown;
        List<WsAgentConnection> _agents;
        List<WsClientConnection> _clients;

        #endregion Members

        public WsServer(int port)
        {
            _port = port;

            // This is a list of active connections
            _unknown = new List<WsConnection>();
            _agents = new List<WsAgentConnection>();
            _clients = new List<WsClientConnection>();
        }

        public void StartSocketServer()
        {
            try
            {
                // Create a socket that will listen for messages
                _listener = new Socket(AddressFamily.InterNetwork,
                                       SocketType.Stream,
                                       ProtocolType.IP);

                // Create and bind the endpoint
                IPEndPoint ip = new IPEndPoint(IPAddress.Loopback, _port);
                _listener.Bind(ip);

                // Listen for new connections - the OnConnect() method
                // will be invoked to handle them
                _listener.Listen(100);
                _listener.BeginAccept(new AsyncCallback(OnConnect), null);
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Listener failed, handle = {0}: {1}",
                    _listener.Handle, ex.Message);
            }
        }


        void MessageReceived(WsConnection sender, MessageReceivedEventArgs e)
        {
            string msg = e.Message;

            log.Debug("Message received: " + msg);

            if (e.DataLength > 14 && (msg.Substring(0, 14) == "[Agent SignOn:"))
            {
                // This is an agent signing on
                string name = msg.Substring(14, e.DataLength - 15);
                WsAgentConnection agent = new WsAgentConnection(sender, name);

                // Re-wire the event handlers
                sender.Disconnected -= Disconnected;
                sender.MessageReceived -= MessageReceived;
                sender.Disconnected += agent.Disconnected;
                sender.MessageReceived += agent.MessageReceived;

                agent.AgentDisconnected +=
                    new WsDisconnectedAgentEventHandler(AgentDisconnected);

                // Move this socket to the agent list
                _unknown.Remove(sender);
                _agents.Add(agent);

                log.InfoFormat("Socket attached to agent {0}, handle = {1}",
                    name, sender._mySocket.Handle.ToString());

                // Send a response
                agent.SendMessage("Welcome, " + name);
            }
            else if (e.DataLength > 15 &&
                    (msg.Substring(0, 15) == "[Client SignOn:"))
            {
                // This is a client requesting assistance
                string name = msg.Substring(15, e.DataLength - 16);

                // Find an agent
                WsAgentConnection agent = null;
                int clientID = 0;
                foreach (WsAgentConnection a in _agents)
                {
                    foreach (KeyValuePair<int, WsClientConnection> d in a._clients)
                    {
                        if (d.Value == null)
                        {
                            agent = a;
                            clientID = d.Key;
                            break;
                        }
                    }
                    if (agent != null)
                        break;
                }

                if (agent != null)
                {
                    WsClientConnection client =
                        new WsClientConnection(sender, agent, clientID, name);

                    log.InfoFormat("Client {0} assigned to agent {1}", name, agent._name);
                    
                    // Re-wire the event handlers
                    sender.Disconnected -= Disconnected;
                    sender.MessageReceived -= MessageReceived;
                    sender.Disconnected += client.Disconnected;
                    sender.MessageReceived += client.MessageReceived;

                    client.ClientDisconnected +=
                        new WsDisconnectedClientEventHandler(ClientDisconnected);

                    // Add this to the agent list
                    _unknown.Remove(sender);
                    _clients.Add(client);

                    agent._clients[clientID] = client;

                    // Send a message to the agent
                    agent.SendMessage("[ClientName:" + clientID.ToString() +
                                      name + "]");

                    // Send a response
                    client.SendMessage("Hello! My name is " + agent._name +
                        ". How may I help you?");
                }
                else
                {
                    // There are no agents available
                    sender.SendMessage("There are no agents currently available;" +
                        "please try again later");

                    sender.Dispose();
                }
            }
        }

        void Disconnected(WsConnection sender, EventArgs e)
        {
            log.InfoFormat("Unattached socket disconnected, handle = {0}",
                sender._mySocket.Handle.ToString());

            _unknown.Remove(sender);
        }

        private void PerformHandshake(Socket s)
        {
            using (NetworkStream stream = new NetworkStream(s))
            using (StreamReader reader = new StreamReader(stream))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                string key = "";

                // Read the input data using the stream reader, one line 
                // at a time until all lines have been processed. The only  
                // item that we need to get is the request key.
                string input = "Empty";
                while (!string.IsNullOrWhiteSpace(input))
                {
                    input = reader.ReadLine();

                    if (input != null &&
                        input.Length > 18 &&
                        input.Substring(0, 18) == "Sec-WebSocket-Key:")
                        // Save the request key
                        key = input.Substring(19);
                }
                // This guid is used to generate the response key
                const String keyGuid = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
                string webSocketAccept;

                // The response key in generated by concatenating the request
                // key and the special guid. The result is then encrypted.
                string ret = key + keyGuid;
                SHA1 sha = new SHA1CryptoServiceProvider();
                byte[] sha1Hash = sha.ComputeHash(Encoding.UTF8.GetBytes(ret));
                webSocketAccept = Convert.ToBase64String(sha1Hash);

                // Send handshake response to the client using the
                // stream writer
                writer.WriteLine("HTTP/1.1 101 Switching Protocols");
                writer.WriteLine("Upgrade: websocket");
                writer.WriteLine("Connection: Upgrade");
                writer.WriteLine("Sec-WebSocket-Accept: " + webSocketAccept);
                writer.WriteLine("");
            }
        }

        private void OnConnect(IAsyncResult asyn)
        {
            // create a new socket for the connection
            Socket socket = _listener.EndAccept(asyn);

            // Perform the necessary handshaking
            PerformHandshake(socket);

            log.InfoFormat("New socket created, handle = {0}",
                socket.Handle.ToString());

            // Create a WsConnection object for this connection
            WsConnection client = new WsConnection(socket);
            _unknown.Add(client);

            // Wire-up the event handlers
            client.MessageReceived += new MessageReceivedEventHandler(MessageReceived);
            client.Disconnected += new WsDisconnectedEventHandler(Disconnected);

            // Listen for more connections
            _listener.BeginAccept(new AsyncCallback(OnConnect), null);
        }

        void ClientDisconnected(WsClientConnection sender, EventArgs e)
        {
            log.InfoFormat("Client {0}, socket disconnected, handle = {1}",
                sender._name, sender._connection._mySocket.Handle.ToString());

            if (sender._agent != null)
            {
                sender._agent._clients[sender._clientID] = null;
                sender._agent.SendMessage("[ClientClose:" +
                    sender._clientID.ToString() + "]");
            }
            _clients.Remove(sender);
            sender.Dispose();
        }

        void AgentDisconnected(WsAgentConnection sender, EventArgs e)
        {
            log.InfoFormat("Agent {0} socket disconnected, handle = {1}",
                sender._name, sender._connection._mySocket.Handle.ToString());

            foreach (KeyValuePair<int, WsClientConnection> d in sender._clients)
            {
                if (d.Value != null)
                {
                    _clients.Remove(d.Value);
                    d.Value.SendMessage
                        ("The agent has been disconnected; please reconnect");
                }
            }
            _agents.Remove(sender);
            sender.Dispose();
        }
    }
}
