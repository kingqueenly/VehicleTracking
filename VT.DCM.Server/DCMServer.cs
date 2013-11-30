using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using log4net;
using System.IO;

namespace VT.DCM.Server
{
    public partial class DCMServer : Form
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(DCMServer));

        private TcpListener tcpListener = null;

        delegate void ChangeListenerStatusDelegate();
        delegate void AddListItemDelegate(Device device);

        public DCMServer()
        {
            InitializeComponent();
        }

        private void ChangeListenerStatus()
        {
            lblStatus.Text = "监听中...";
        }

        private void AddListItem(Device device)
        {
            device.DeviceName = "Device " + new Random().Next();
            lstDevices.Items.Add(device);
            lstDevices.DisplayMember = "DeviceName";
        }

        private void ListenerHanlder()
        {
            IPAddress ip = IPAddress.Any;
            IPEndPoint localIPE = new IPEndPoint(ip, 5858);
            tcpListener = new TcpListener(localIPE);

            tcpListener.Start();

            lblStatus.Invoke(new ChangeListenerStatusDelegate(ChangeListenerStatus));

            while (true)
            {
                TcpClient tcpClient = null;
                try
                {
                    tcpClient = tcpListener.AcceptTcpClient();
                    lstDevices.Invoke(new AddListItemDelegate(AddListItem), new Device(tcpClient));
                }
                catch
                {
                    log.Debug("Listener stoped!");
                    break;
                }

                Thread threadMessageHandler = new Thread(new ParameterizedThreadStart(ProcessOneMessage));
                threadMessageHandler.Start(tcpClient);

            }
        }

        private void ProcessOneMessage(object tcp)
        {
            TcpClient tcpClient = tcp as TcpClient;

            NetworkStream ns = tcpClient.GetStream();
           
            byte[] receiveBuffer = new byte[10 * 1024];
            byte[] decodedBuffer = null;
            while (true)
            {
                int receivedBufferSize = 0;
                try
                {
                    receivedBufferSize = ns.Read(receiveBuffer, 0, receiveBuffer.Length);

                    if (receivedBufferSize <= 0) continue;

                }
                catch (IOException iEx)
                {
                    log.Error("IOException", iEx);
                    break;
                }
                catch (ObjectDisposedException odEx)
                {
                    log.Error("ObjectDisposedException", odEx);
                    break;
                }
                catch (Exception ex)
                {
                    log.Error("Exception", ex);
                    break;
                }

                log.Debug("[Start] - Process one message from remote");

                decodedBuffer = ByteHelper.DecodeBytes(receiveBuffer, receivedBufferSize);
                log.Debug(string.Format("-> The size of message : [{0}]", decodedBuffer.Length));

                MessageHandler messageHandler = new MessageHandler(ns);
                List<Message> messageList = messageHandler.SplitMessage(decodedBuffer);
                foreach (Message message in messageList)
                {
                    if (message.MessageType == DeviceMessageType.MessageError) continue;

                    messageHandler.SendServerACK(message.MessageBuffer);
                    switch (message.MessageType)
                    {
                        //case DeviceCommandType.Beat:
                        //    SendServerACK(receiveBuffer, ns);
                        //    break;
                        case DeviceMessageType.DeviceAgpsTime:
                            //SendServerACK(receiveBuffer, ns);
                            messageHandler.SendAgpsCheckResult(message.MessageBuffer, message.MessageBuffer.Length);
                            break;
                        case DeviceMessageType.UpdateAgpsRequest:
                            //SendServerACK(receiveBuffer, ns);
                            messageHandler.SendAgpsData(message.MessageBuffer);
                            break;
                        case DeviceMessageType.UpdateAgpsResponse:
                            //SendServerACK(receiveBuffer, ns);
                            //fsEphemeris.Close();
                            break;
                        case DeviceMessageType.ComfirmTime:
                            messageHandler.SendServerTime(message.MessageBuffer);
                            break;
                        case DeviceMessageType.UploadObdStatus:
                            //170,18,52,86,120,144,0,0,0,0,1,16,0,56,0,8,19,17,16,18,39,84,0,151,1,0,0,39,0,60,0,0,32,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,103,170,170,18,52,86,120,144,0,0,0,0,1,17,0,17,0,3,1,25,12,0,0,1,99,0,0,19,136,0,0,39,16,64,170,
                            //AA 12 34 56 78 90 00 00 00 00 01 10 00 38 00 08 13 11 10 12 27 54 00 97 01 00 00 27 00 3C 00 00 20 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 67 AA AA 12 34 56 78 90 00 00 00 00 01 11 00 11 00 03 01 19 0C 00 00 01 63 00 00 13 88 00 00 27 10 40 AA 
                            //SendServerACK(receiveBuffer, ns);
                            break;
                        case DeviceMessageType.UploadJourneyRecord:
                            //170,18,52,86,120,144,0,0,0,0,1,14,0,38,0,7,19,17,16,18,39,80,19,17,16,18,36,54,0,0,50,0,0,0,0,0,0,0,0,0,0,25,66,7,173,151,0,9,4,70,0,6,202,170,170,18,52,86,120,144,0,0,0,0,1,15,0,26,0,4,19,17,16,18,39,84,8,0,78,7,16,64,52,2,48,41,134,101,62,0,0,0,0,0,24,170,
                            //AA 12 34 56 78 90 00 00 00 00 01 0E 00 26 00 07 13 11 10 12 27 50 13 11 10 12 24 36 00 00 32 00 00 00 00 00 00 00 00 00 00 19 42 07 AD 97 00 09 04 46 00 06 CA AA AA 12 34 56 78 90 00 00 00 00 01 0F 00 1A 00 04 13 11 10 12 27 54 08 00 4E 07 10 40 34 02 30 29 86 65 3E 00 00 00 00 00 18 AA 
                            //SendServerACK(receiveBuffer, ns);
                            break;
                        case DeviceMessageType.UploadFaultCode:
                            //170,18,52,86,120,144,0,0,0,0,1,6,0,8,0,5,1,80,49,51,51,54,196,170,
                            //AA 12 34 56 78 90 00 00 00 00 01 06 00 08 00 05 01 50 31 33 33 36 C4 AA 
                            //SendServerACK(receiveBuffer, ns);
                            break;
                        case DeviceMessageType.LocationInfo:
                            //AA 12 34 56 78 90 00 00 00 00 01 0D 00 22 00 04 13 11 22 22 22 45 08 00 0F 07 10 40 33 01 30 30 39 50 3D 13 19 47 00 06 05 3A 00 01 00 00 00 01 95 AA
                            LocationMessage locationMessage = new LocationMessage(message.MessageBuffer);
                            if (locationMessage.IsGpsLocation && locationMessage.CoordinateType == LocationCoordinateType.GPS)
                            {
                                LocationMessageHandler locationMessageHandler = new LocationMessageHandler();
                                locationMessageHandler.Add(locationMessage);
                            }
                            break;
                        default:
                            break;
                    }
                }

                log.Debug("[End] - Process one message from remote");

            }
        }

        private void btnStartListener_Click(object sender, EventArgs e)
        {
            Thread threadListener = new Thread(new ThreadStart(ListenerHanlder));
            threadListener.Start();

            btnStartListener.Enabled = false;
            btnStopListener.Enabled = true;

        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            Device device = lstDevices.Items[0] as Device;
            if (device != null)
            {
                device.SendMessage(txtMessage.Text);
            }
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            //AA 12 34 56 78 90 00 00 00 00 01 0D 00 22 00 04 13 11 22 22 22 45 08 00 0F 07 10 40 33 01 30 30 39 50 3D 13 19 47 00 06 05 3A 00 01 00 00 00 01 95 AA
            //byte[] buffer = { 170, 18, 52, 86, 120, 144, 0, 0, 0, 0, 1, 13, 0, 34, 0, 4, 19, 17, 34, 34, 34, 69, 8, 0, 15, 7, 16, 64, 51, 1, 48, 48, 57, 80, 61, 19, 25, 71, 0, 6, 5, 58, 0, 1, 0, 0, 0, 1, 149, 170 };
            byte[] buffer = { 170, 18, 52, 86, 120, 144, 0, 0, 0, 0, 1, 233, 0, 26, 0, 4, 19, 17, 37, 9, 66, 86, 8, 0, 78, 10, 16, 64, 66, 3, 48, 50, 89, 53, 18, 0, 0, 0, 0, 0, 117, 170 };
            LocationMessage locationMessage = new LocationMessage(buffer);

            if (locationMessage.IsGpsLocation && locationMessage.CoordinateType == LocationCoordinateType.GPS)
            {
                LocationMessageHandler locationMessageHandler = new LocationMessageHandler();
                locationMessageHandler.Add(locationMessage);
            }

        }

        private bool Check(byte[] bytes)
        {
            bool result = false;
            if (bytes.Length < 16)
            {
                return result;
            }

            return result;
        }

        private void btnStopListener_Click(object sender, EventArgs e)
        {
            log.Debug("[Start] - Stop listener]");
            foreach (object obj in lstDevices.Items)
            {
                Device device = obj as Device;
                if (device != null)
                {
                    device.Close();
                }
            }

            tcpListener.Stop();
            lblStatus.Text = "已停止监听";
            log.Debug("[End] - Stop listener]");

            btnStopListener.Enabled = false;
            btnStartListener.Enabled = true;
        }

        private void DCMServer_Load(object sender, EventArgs e)
        {

        }
    }
}
