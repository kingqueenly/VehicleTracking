using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace DCM_Client
{
    public partial class DCMClient : Form
    {
        private Socket clntSocket = null;

        public DCMClient()
        {
            InitializeComponent();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            byte[] bytes = { 170, 18, 52, 86, 120, 144, 0, 0, 0, 0, 1, 99, 0, 34, 0, 4, 19, 17, 32, 9, 49, 67, 8, 0, 15, 8, 16, 64, 64, 105, 48, 48, 131, 49, 179, 47, 47, 71, 0, 6, 5, 227, 0, 28, 0, 0, 10, 28, 95, 170, 170, 18, 52, 86, 120, 144, 0, 0, 0, 0, 1, 100, 0, 34, 0, 4, 19, 17, 32, 9, 49, 85, 2, 8, 0, 15, 8, 16, 64, 64, 104, 48, 48, 145, 85, 2, 179, 46, 46, 72, 0, 6, 5, 198, 0, 28, 0, 0, 11, 28, 18, 170, 170, 18, 52, 86, 120, 144, 0, 0, 0, 0, 1, 101, 0, 34, 0, 4, 19, 17, 32, 9, 50, 6, 8, 0, 15, 8, 16, 64, 64, 100, 48, 48, 153, 54, 179, 52, 52, 72, 0, 6, 6, 141, 0, 29, 0, 0, 11, 29, 108, 170, 170, 18, 52, 86, 120, 144, 0, 0, 0, 0, 1, 102, 0, 34, 0, 4, 19, 17, 32, 9, 50, 23, 8, 0, 15, 8, 16, 64, 64, 100, 48, 49, 8, 66, 179, 60, 60, 74, 0, 6, 6, 236, 0, 30, 0, 0, 12, 30, 254, 170, 170, 18, 52, 86, 120, 144, 0, 0, 0, 0, 1, 103, 0, 34, 0, 4, 19, 17, 32, 9, 50, 48, 8, 0, 15, 8, 16, 64, 64, 97, 48, 49, 25, 57, 179, 51, 51, 75, 0, 6, 6, 72, 0, 31, 0, 0, 13, 31, 19, 170, 170, 18, 52, 86, 120, 144, 0, 0, 0, 0, 1, 104, 0, 34, 0, 4, 19, 17, 32, 9, 50, 66, 8, 0, 15, 8, 16, 64, 64, 88, 48, 49, 39, 53, 178, 36, 48, 76, 0, 6, 3, 181, 0, 31, 0, 0, 13, 31, 143, 170 };
            clntSocket.Send(bytes);
            //string msg = txtMessage.Text;
            //if(!string.IsNullOrWhiteSpace(msg))
            //{
            //    clntSocket.Send(Encoding.UTF8.GetBytes(msg));
            //}
        }

        private void DCMClient_Load(object sender, EventArgs e)
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            //IPAddress ip = IPAddress.Parse("42.96.140.176");
            IPEndPoint remoteIPE = new IPEndPoint(ip, 5858);
            clntSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                clntSocket.Connect(remoteIPE);
                lblStatus.Text = "连接成功";

                Thread trdListening = new Thread(() => {
                    while (true)
                    {
                        byte[] buffer = new byte[100*1024];
                        int len = clntSocket.Receive(buffer);
                        if (len > 0)
                        {
                            //MessageBox.Show(Encoding.UTF8.GetString(buffer));
                        }
                    }
                });
                trdListening.Start();
            }
            catch (SocketException sex)
            {
                System.Console.WriteLine("连接异常 : " + sex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            
        }
    }
}
