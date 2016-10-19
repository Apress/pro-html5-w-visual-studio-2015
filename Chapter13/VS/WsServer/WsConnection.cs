using System;
using System.Text;

using System.Net.Sockets;

namespace WsServer
{
    // This class defines the data that is passed to the MessageReceived
    // event handler
    public class MessageReceivedEventArgs
    {
        public string Message { get; private set; }
        public int DataLength { get; private set; }
        public MessageReceivedEventArgs(string msg, int len)
        {
            DataLength = len;
            Message = msg;
        }
    }

    // Define the event handler delegates
    public delegate void MessageReceivedEventHandler
        (WsConnection sender, MessageReceivedEventArgs e);

    public delegate void WsDisconnectedEventHandler
        (WsConnection sender, EventArgs e);

    public class WsConnection : IDisposable
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
            (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Members

        public Socket _mySocket;
        protected byte[] _inputBuffer;
        protected StringBuilder _inputString;

        // Define the events that are available
        public event MessageReceivedEventHandler MessageReceived;
        public event WsDisconnectedEventHandler Disconnected;

        #endregion Members

        public WsConnection(Socket s)
        {
            _mySocket = s;
            _inputBuffer = new byte[255];
            _inputString = new StringBuilder();

            // Begin listening - the ReadMessage() method will be
            // invoked when a message is received.
            _mySocket.BeginReceive(_inputBuffer,
                                   0,
                                   _inputBuffer.Length,
                                   0,
                                   ReadMessage,
                                   null);
        }

        protected void OnMessageReceived(string msg)
        {
            // When a message is received, call the event handler if
            // one has been specified
            if (MessageReceived != null)
                MessageReceived(this, new MessageReceivedEventArgs(msg, msg.Length));
        }

        public void Dispose()
        {
            log.DebugFormat("Socket closing, handle {0}", _mySocket.Handle);

            _mySocket.Close();
        }

        protected void ReadMessage(IAsyncResult msg)
        {
            int sizeOfReceivedData = _mySocket.EndReceive(msg);
            if (sizeOfReceivedData > 0)
            {
                // Get the data provided in the first 2 bytes
                bool final = (_inputBuffer[0] & 0x80) > 0 ? true : false;
                bool masked = (_inputBuffer[1] & 0x80) > 0 ? true : false;
                int dataLength = _inputBuffer[1] & 0x7F;

                int actualLength;
                int dataIndex = 0;
                byte[] length = new byte[8];
                byte[] masks = new byte[4];

                // Depending on the initial data length, get the actual length
                // and the maskingkey from the appropriate bytes.
                if (dataLength == 126)
                {
                    dataIndex = 4;
                    Array.Copy(_inputBuffer, 2, length, 0, 2);
                    actualLength = BitConverter.ToInt16(length, 0);

                    if (masked)
                        Array.Copy(_inputBuffer, 4, masks, 0, 4);
                }
                else if (dataLength == 127)
                {
                    dataIndex = 10;
                    Array.Copy(_inputBuffer, 2, length, 0, 8);
                    actualLength = (int)BitConverter.ToInt64(length, 0);
                    if (masked)
                        Array.Copy(_inputBuffer, 10, masks, 0, 4);
                }
                else
                {
                    dataIndex = 2;
                    actualLength = dataLength;
                    if (masked)
                        Array.Copy(_inputBuffer, 2, masks, 0, 4);
                }

                // If a mask is supplied, skip another 4 bytes
                if (masked)
                    dataIndex += 4;

                // Get the actual data in the payload array
                byte[] payload = new byte[actualLength];
                Array.Copy(_inputBuffer, dataIndex, payload, 0, dataLength);

                // Unmask the data, if necessary
                if (masked)
                {
                    for (int i = 0; i < actualLength; i++)
                    {
                        payload[i] = (byte)(payload[i] ^ masks[i % 4]);
                    }
                }

                // Copy the data into the input string and empty the buffer
                _inputString.Append(Encoding.UTF8
                    .GetString(payload, 0, (int)actualLength));
                Array.Clear(_inputBuffer, 0, _inputBuffer.Length);

                // If this is the final frame, raise an event and clear the input
                if (final)
                {
                    // Do something with the data
                    OnMessageReceived(_inputString.ToString());

                    // Clear the input string
                    _inputString.Clear();
                }

                // Listen for more messages
                try
                {
                    _mySocket.BeginReceive(_inputBuffer,
                                           0,
                                           _inputBuffer.Length,
                                           0,
                                           ReadMessage,
                                           null);
                }
                catch (Exception ex)
                {
                    log.ErrorFormat("ReceiveMessage failed, handle {0}, {1}",
                        _mySocket.Handle, ex.Message);
                }
            }
            // If we were not able to read the message, assume that
            // the socket is closed
            else
            {
                log.ErrorFormat("ReceiveMessage failed, handle {0}", _mySocket.Handle);
            }
        }

        public void SendMessage(string msg)
        {
            if (_mySocket.Connected)
            {
                // Create the output buffer
                Int64 dataLength = msg.Length;
                int dataStart = 0;
                byte[] dataOut = new byte[dataLength + 10];

                // Build the frame data - depending on the length, it can
                // be passed one of three ways
                dataOut[0] = 0x81;

                // Store the length in the 2nd byte
                if (dataLength < 256)
                {
                    dataOut[1] = (byte)dataLength;
                    dataStart = 2;
                }
                // Store the length in the 3rd and 4th bytes
                else if (dataLength < UInt16.MaxValue)
                {
                    dataOut[1] = 0xFE;
                    dataOut[2] = (byte)(dataLength & 0x00FF);
                    dataOut[3] = (byte)(dataLength & 0xFF00);
                    dataStart = 4;
                }
                // Store the length in bytes 3 - 9
                else
                {
                    dataOut[1] = 0xFF;
                    for (int i = 0; i < 8; i++)
                        dataOut[i + 2] = (byte)((dataLength >> (i * 8)) & 0x000000FF);
                    dataStart = 10;
                }

                // Encode the data and store it in the output buffer
                byte[] data = Encoding.UTF8.GetBytes(msg);
                Array.Copy(data, 0, dataOut, dataStart, dataLength);

                // Send the message
                try
                {
                    _mySocket.Send(dataOut,
                                   (int)(dataLength + dataStart),
                                   SocketFlags.None);
                }
                catch (Exception ex)
                {
                    log.ErrorFormat("SendMessage failed, handle = {0}: {1}", _mySocket.Handle, ex.Message);
                    
                    // If we get an error, assume the socket has been disconnected
                    if (Disconnected != null)
                        Disconnected(this, EventArgs.Empty);
                }
            }
        }
    }
}
