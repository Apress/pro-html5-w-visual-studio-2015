using System;

namespace WsServer
{
    public delegate void WsDisconnectedClientEventHandler
        (WsClientConnection sender, EventArgs e);

    public class WsClientConnection : IDisposable
    {
        public WsConnection _connection;
        public string _name;
        public WsAgentConnection _agent;
        public int _clientID;

        public event WsDisconnectedClientEventHandler ClientDisconnected;

        public WsClientConnection(WsConnection conn,
                                  WsAgentConnection agent,
                                  int id,
                                  string name)
        {
            _connection = conn;
            _agent = agent;
            _clientID = id;
            _name = name;
        }

        public void MessageReceived(WsConnection sender,
                                    MessageReceivedEventArgs e)
        {
            if (_agent != null && e.Message.Length > 0)
            {
                if (e.Message[0] == '\u0003')
                {
                    if (ClientDisconnected != null)
                        ClientDisconnected(this, EventArgs.Empty);
                }
                else
                    _agent.SendMessage(_clientID.ToString() + ": " + e.Message);
            }
        }

        public void SendMessage(string msg)
        {
            if (_connection != null)
                _connection.SendMessage(msg);
        }

        public void Disconnected(WsConnection sender, EventArgs e)
        {
            if (ClientDisconnected != null)
                ClientDisconnected(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            if (_connection != null)
                _connection.Dispose();
        }
    }
}
