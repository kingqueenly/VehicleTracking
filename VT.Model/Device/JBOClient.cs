using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace VT.Model.Device
{
    public class JBOClient
    {
        private string _idCode;

        public string IDCode
        {
            get { return _idCode; }
            set { _idCode = value; }
        }
 
        private TcpClient _tcpClient;

        private NetworkStream _stream;

        public JBOClient(TcpClient tcpClient)
        {
            this._tcpClient = tcpClient;
            this._stream = tcpClient.GetStream();
        }

        public void SendMessage(byte[] messageBuffer)
        {
            _stream.Write(messageBuffer, 0, messageBuffer.Length);
        }

        public void Close()
        {
            _tcpClient.Close();
        }

        public byte[] ReceiveBytes()
        {
            byte[] buffer = new byte[100*1024];
            int receivedBufferSize = _stream.Read(buffer, 0, buffer.Length);

            byte[] bytes = new byte[receivedBufferSize];
            Array.Copy(buffer, bytes, receivedBufferSize);
            return bytes;
        }

        public void SendMessage(string p)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(p);
            _stream.Write(buffer, 0, buffer.Length);
        }
    }
}
