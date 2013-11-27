using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Net.Sockets;
using System.IO;
using VT.Common;
using VT.Model.Device;

namespace VT.WebService
{
    public class MessageHandler
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MessageHandler));

        NetworkStream _networkStream = null;

        //[begin]更新星历数据需要的字段
        private static readonly string ephemerisPath = Environment.CurrentDirectory + "\\ephemeris.ee";
        private static byte[] ees = null;
        private static int eesLen = 0;
        //[end]

        public MessageHandler(NetworkStream ns)
        {
            this._networkStream = ns;
        }

        public void SendServerTime(byte[] messageBuffer)
        {
            log.Debug("[Start] - Send server time for device");
            byte[] bytes = new byte[1 + 10 + 1 + 2 + 8 + 1 + 1];

            int index = 0;
            bytes[index++] = 0xaa;
            Array.Copy(messageBuffer, 1, bytes, 1, 10);
            index += 10;
            bytes[index++] = Helper.GetThreadNO();
            bytes[index++] = 0x0;
            bytes[index++] = 0x8;
            bytes[index++] = 0x80;
            bytes[index++] = 0x1b;
            DateTime dtCurrent = DateTime.Now;
            bytes[index++] = Helper.Hcd(dtCurrent.Year - 2000);
            bytes[index++] = Helper.Hcd(dtCurrent.Month);
            bytes[index++] = Helper.Hcd(dtCurrent.Day);
            bytes[index++] = Helper.Hcd(dtCurrent.Hour);
            bytes[index++] = Helper.Hcd(dtCurrent.Minute);
            bytes[index++] = Helper.Hcd(dtCurrent.Second);
            bytes[index++] = ByteHelper.Xor(bytes, 1, index);
            bytes[index] = 0xaa;

            bytes = ByteHelper.EncodeBytes(bytes);

            _networkStream.Write(bytes, 0, bytes.Length);

            log.Debug(string.Format("-> Sent [{0}byte(s)]", bytes.Length));
            log.Debug(string.Format("-> {0}", ByteHelper.BytesToHexString(bytes, bytes.Length)));
            log.Debug("[End] - Send server time for device");
        }

        public void SendAgpsData(byte[] messageBuffer)
        {
            log.Debug("[Start] - Send Agps data to device");
            short packageNum = ByteHelper.GetMsgLength(new byte[] { messageBuffer[16], messageBuffer[17] });
            log.Debug(string.Format("-> Current package number [{0}]", packageNum));
            //ep = Ephemeris Package
            byte[] bytes = GenerateAgpsData(messageBuffer, packageNum);

            _networkStream.Write(bytes, 0, bytes.Length);

            log.Debug(string.Format("-> Sent [{0}byte(s)]", bytes.Length));
            log.Debug(string.Format("-> {0}", ByteHelper.BytesToHexString(bytes, bytes.Length)));
            log.Debug("[End] - Send Agps data to device");
        }

        private byte[] GenerateAgpsData(byte[] messageBuffer, short packageNum)
        {
            log.Debug("[Start] - Generate Agps data]");
            int packageSize = 4 * 1024;
            byte[] ep = new byte[packageSize];
            int len = (ees.Length - packageNum * packageSize) > packageSize ? packageSize : (ees.Length - packageNum * packageSize);
            System.Array.Copy(ees, packageNum * packageSize, ep, 0, len);

            //Message Length:2 + 2 + 2 + len + 4 + 1 + 6  
            short msgLen = (short)(2 + 2 + 2 + len + 4 + 1 + 6);
            byte[] bytes = new byte[1 + 10 + 1 + 2 + msgLen + 1 + 1];
            int index = 0;
            bytes[index++] = 0xaa;
            Array.Copy(messageBuffer, 1, bytes, 1, 10);
            index += 10;
            bytes[index++] = Helper.GetThreadNO();
            Array.Copy(ByteHelper.ConvertMsgLength(msgLen), 0, bytes, index, 2);
            index += 2;
            bytes[index++] = 0x80;
            bytes[index++] = 0x14;
            //Array.Copy(ByteHelper.GetPackageNO(packageNum), 0, bytes, index, 2);
            //index += 2;
            bytes[index++] = messageBuffer[16];
            bytes[index++] = messageBuffer[17];
            Array.Copy(ByteHelper.ConvertMsgLength((short)len), 0, bytes, index, 2);
            index += 2;
            Array.Copy(ep, 0, bytes, index, len);
            index += len;
            Array.Copy(ByteHelper.ConvertMsgLength((int)eesLen), 0, bytes, index, 4);
            index += 4;
            bytes[index++] = ByteHelper.Xor(ees, 0, ees.Length);

            DateTime dtCurrent = DateTime.Now;
            bytes[index++] = Helper.Hcd(dtCurrent.Year - 2000);
            bytes[index++] = Helper.Hcd(dtCurrent.Month);
            bytes[index++] = Helper.Hcd(dtCurrent.Day);
            bytes[index++] = Helper.Hcd(dtCurrent.Hour);
            bytes[index++] = Helper.Hcd(dtCurrent.Minute);
            bytes[index++] = Helper.Hcd(dtCurrent.Second);

            bytes[index++] = ByteHelper.Xor(bytes, 1, index);
            bytes[index] = 0xaa;
            log.Debug("[End] - Generate Agps data]");
            return ByteHelper.EncodeBytes(bytes);
        }

        public void SendAgpsCheckResult(byte[] messageBuffer, int messageBufferSize)
        {
            log.Debug("[Start] - Send Agps check result to device");
            //aa 12 34 56 78 90 00 00 00 00 01 ff 00 08 00 0e 12 06 22 10 55 54 ac aa
            string hexCommand = ByteHelper.BytesToHexString(messageBuffer, messageBufferSize);
            string y = hexCommand.Substring(48, 2);
            string m = hexCommand.Substring(51, 2);
            string d = hexCommand.Substring(54, 2);
            string h = hexCommand.Substring(57, 2);
            string mm = hexCommand.Substring(60, 2);
            string s = hexCommand.Substring(63, 2);

            DateTime dtFileUpdateTime = File.GetLastWriteTime(ephemerisPath);
            DateTime dtLastUpdateTime = DateTime.Parse(string.Format("{0}-{1}-{2} {3}:{4}:{5}", y, m, d, h, mm, s));
            byte needUpdate = (byte)(dtFileUpdateTime > dtLastUpdateTime ? 0x00 : 0x01);

            //AA 12 34 56 78 90 00 00 00 00 01 30 00 03 80 15 00 3F AA 
            byte[] bytes = new byte[1 + 10 + 1 + 2 + 2 + 1 + 1 + 1];
            int index = 0;
            bytes[index++] = 0xaa;
            Array.Copy(messageBuffer, 1, bytes, 1, 10);
            index += 10;
            bytes[index++] = Helper.GetThreadNO();
            bytes[index++] = 0x00;
            bytes[index++] = 0x03;
            bytes[index++] = 0x80;
            bytes[index++] = 0x15;
            bytes[index++] = needUpdate;
            bytes[index++] = ByteHelper.Xor(bytes, 1, index);
            bytes[index] = 0xaa;

            bytes = ByteHelper.EncodeBytes(bytes);

            _networkStream.Write(bytes, 0, bytes.Length);

            if (needUpdate == 0x00 && ees == null)
            {
                LoadEphemeris();
            }

            log.Debug(string.Format("-> Sent [{0}byte(s)]", bytes.Length));
            log.Debug(string.Format("-> {0}", ByteHelper.BytesToHexString(bytes, bytes.Length)));
            log.Debug("[End] - Send Agps check result to device");
        }

        private static void LoadEphemeris()
        {
            FileStream fsEphemeris = null;
            try
            {
                log.Debug(string.Format("-> Ephemeris path : [{0}]", ephemerisPath));
                fsEphemeris = new FileStream(ephemerisPath, FileMode.Open);
                ees = new byte[fsEphemeris.Length];
                eesLen = fsEphemeris.Read(ees, 0, ees.Length);
                fsEphemeris.Close();
                log.Debug(string.Format("-> Ephemeris total length : [{0}]", ees.Length));
            }
            catch (FileNotFoundException fnfEx)
            {
                log.Error("FileNotFoundException", fnfEx);
            }
            catch (IOException ioEx)
            {
                log.Error("IOException", ioEx);
            }
            catch (Exception ex)
            {
                log.Error("Exception", ex);
            }
            finally
            {
                fsEphemeris.Close();
            }
        }

        public void SendServerACK(byte[] messageBuffer)
        {
            log.Debug("[Start] - Send ACK to device");
            byte[] bytes = new byte[1 + 10 + 1 + 2 + 2 + 2 + 1 + 1 + 1 + 1];
            int index = 0;
            bytes[index++] = 0xaa;
            Array.Copy(messageBuffer, 1, bytes, 1, 10);
            index += 10;
            bytes[index++] = Helper.GetThreadNO();
            Array.Copy(ByteHelper.ConvertMsgLength((short)6), 0, bytes, index, 2);
            index += 2;
            bytes[index++] = 0x80;
            bytes[index++] = 0x01;
            bytes[index++] = messageBuffer[14];
            bytes[index++] = messageBuffer[15];
            bytes[index++] = messageBuffer[11];
            bytes[index++] = 0x00;
            bytes[index++] = ByteHelper.Xor(bytes, 1, index);
            bytes[index] = 0xaa;

            bytes = ByteHelper.EncodeBytes(bytes);

            _networkStream.Write(bytes, 0, bytes.Length);

            log.Debug(string.Format("-> Sent [{0}byte(s)]", bytes.Length));
            log.Debug(string.Format("-> {0}", ByteHelper.BytesToHexString(bytes, bytes.Length)));
            log.Debug("[End] - Send ACK to device");
        }

        public List<Message> SplitMessages(byte[] messageBuffer)
        {
            List<Message> messageList = new List<Message>();

            int startIndex = 0;
            for (int i = 0; i < messageBuffer.Length; i++)
            {
                byte bc = messageBuffer[i];
                byte bn = 0;
                if (i + 1 == messageBuffer.Length)
                {
                    bn = 170;
                }
                else
                {
                    bn = messageBuffer[i + 1];
                }

                if (bc == 170)
                {
                    if (bn != 170)
                    {
                        startIndex = i;
                    }

                    if (bn == 170)
                    {
                        byte[] bs = new byte[i - startIndex + 1];
                        Array.Copy(messageBuffer, startIndex, bs, 0, bs.Length);
                        Message message = new Message(bs);
                        messageList.Add(message);
                    }
                }
            }

            return messageList;
        }
    }
}
