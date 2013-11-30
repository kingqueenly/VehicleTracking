using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Sockets;
using System.Text;

namespace VT.WebService
{
    public class JBODevice
    {
        private string _idCode;
        private TcpClient _tcpClient;

        public string IDCode
        {
            get { return _idCode; }
            set { _idCode = value; }
        }

        public TcpClient TcpClient
        {
            get { return _tcpClient; }
            set { _tcpClient = value; }
        }

        private NetworkStream _stream;

        public JBODevice(TcpClient tcpClient)
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