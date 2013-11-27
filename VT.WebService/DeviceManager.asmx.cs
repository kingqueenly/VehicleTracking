using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using log4net;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using VT.Common;
using VT.DAL;
using VT.Model.Device;

namespace VT.WebService
{
    [WebService(Namespace = "vt.webservice.devicemanager")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class DeviceManager : System.Web.Services.WebService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DeviceManager));
        private TcpListener tcpListener;

        [WebMethod]
        public void Start()
        {
            Thread threadListener = new Thread(new ThreadStart(ListenerHanlder));
            threadListener.Start();
        }

        [WebMethod]
        public void Stop()
        {
 
        }

        private void ListenerHanlder()
        {
            IPAddress ip = IPAddress.Any;
            IPEndPoint localIPE = new IPEndPoint(ip, 5858);
            tcpListener = new TcpListener(localIPE);

            tcpListener.Start();

            while (true)
            {
                TcpClient tcpClient = null;
                try
                {
                    tcpClient = tcpListener.AcceptTcpClient();
                    //lstDevices.Invoke(new AddListItemDelegate(AddListItem), new Device(tcpClient));
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
                List<Message> messageList = messageHandler.SplitMessages(decodedBuffer);
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
                            Location location = new Location(message.MessageBuffer);
                            if (location.IsGpsLocation && location.CoordinateType == LocationCoordinateType.GPS)
                            {
                                LocationDAL.AddPosition(location);
                            }
                            break;
                        default:
                            break;
                    }
                }

                log.Debug("[End] - Process one message from remote");

            }
        }
    }
}
