using System;
using System.Collections.Generic;

namespace WsServer
{
    public delegate void WsDisconnectedAgentEventHandler
        (WsAgentConnection sender, EventArgs e);

    public class WsAgentConnection : IDisposable
    {
        public WsConnection _connection;
        public string _name;
        public Dictionary<int, WsClientConnection> _clients;

        public event WsDisconnectedAgentEventHandler AgentDisconnected;

        public WsAgentConnection(WsConnection conn, string name)
        {
            _connection = conn;
            _name = name;

            // Initialize our client list
            _clients = new Dictionary<int, WsClientConnection>();
            for (int i = 1; i <= 4; i++)
            {
                _clients.Add(i, null);
            }
        }

        public void MessageReceived(WsConnection sender,
                                    MessageReceivedEventArgs e)
        {
            if (e.Message.Length >= 1)
            {
                if (e.Message[0] == '\u0003')
                {
                    if (AgentDisconnected != null)
                        AgentDisconnected(this, EventArgs.Empty);
                }

                else if (e.Message.Length > 1)
                {
                    string s = e.Message.Substring(0, 1);
                    int i = 0;
                    if (int.TryParse(s, out i))
                    {
                        WsClientConnection client = _clients[i];
                        if (client != null)
                        {
                            client.SendMessage(e.Message.Substring(2));
                        }
                    }
                }
            }
        }

        public void SendMessage(string msg)
        {
            if (_connection != null)
                _connection.SendMessage(msg);
        }

        public void Disconnected(WsConnection sender, EventArgs e)
        {
            if (AgentDisconnected != null)
                AgentDisconnected(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            if (_connection != null)
                _connection.Dispose();
        }
    }
}
