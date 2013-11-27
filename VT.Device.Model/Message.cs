using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.IO;
using System.Net.Sockets;
using VT.Common;

namespace VT.Device.Model
{
    public enum DeviceMessageType
    {
        Beat,                      //心跳
        ACK,                       //终端ACK
        SearchParameterResponse,   //参数查询应答
        LocationInfo,              //位置信息
        UploadFaultCode,           //上传故障码
        UploadJourneyRecord,       //上传行程记录
        UploadObdStatus,           //上传OBD状态
        UpdateRequest,             //远程升级请求
        UpdateResponse,            //升级结果
        ObdFaultStatus,            //OBD故障状态
        UploadUnknownSms,          //上传未知短息
        DeviceAgpsTime,            //本地AGPS更新时间
        UpdateAgpsRequest,         //更新AGPS请求
        UpdateAgpsResponse,        //更新AGPS结果
        SmsNoticeParameters,       //短信提醒参数
        ComfirmTime,               //时间查询
        MessageError
    }

    public class Message
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Message));
        private DeviceMessageType _messageType;
        private byte[] _messageBuffer;
        private string _messageHexEncoded;

        public string MessageHexEncoded
        {
            get { return _messageHexEncoded; }
        }

        public DeviceMessageType MessageType
        {
            get { return _messageType; }
            set { _messageType = value; }
        }

        public byte[] MessageBuffer
        {
            get { return _messageBuffer; }
        }

        public Message(byte[] messageBuffer)
        {
            this._messageBuffer = messageBuffer;

            string key = "";
            try
            {
                string original = ByteHelper.BytesToString(messageBuffer, messageBuffer.Length);
                string hexEncoded = ByteHelper.BytesToHexString(messageBuffer, messageBuffer.Length);
                
                _messageHexEncoded = hexEncoded;

                log.Debug(string.Format("-> Original : [{0}]", original));
                log.Debug(string.Format("-> Hex encoded : [{0}]", hexEncoded));

                key = hexEncoded.Substring(42, 5).ToLower();
            }
            catch (ArgumentOutOfRangeException aEx)
            {
                log.Debug("ArgumentOutOfRangeException", aEx);
                _messageType = DeviceMessageType.MessageError;
            }
            catch (Exception ex)
            {
                log.Debug("Exception", ex);
                _messageType = DeviceMessageType.MessageError;
            }

            if (key != "")
            {
                InitMessageType(key);
            }

            log.Debug(string.Format("-> CommandType : [{0}]", _messageType.ToString()));
        }

        private void InitMessageType(string key)
        {
            switch (key)
            {
                case "00 01":
                    _messageType = DeviceMessageType.Beat;
                    break;
                case "00 02":
                    _messageType = DeviceMessageType.ACK;
                    break;
                case "00 03":
                    _messageType = DeviceMessageType.SearchParameterResponse;
                    break;
                case "00 04":
                    _messageType = DeviceMessageType.LocationInfo;
                    break;
                case "00 05":
                    _messageType = DeviceMessageType.UploadFaultCode;
                    break;
                case "00 07":
                    _messageType = DeviceMessageType.UploadJourneyRecord;
                    break;
                case "00 08":
                    _messageType = DeviceMessageType.UploadObdStatus;
                    break;
                case "00 09":
                    _messageType = DeviceMessageType.UpdateRequest;
                    break;
                case "00 0a":
                    _messageType = DeviceMessageType.UpdateResponse;
                    break;
                case "00 0b":
                    _messageType = DeviceMessageType.ObdFaultStatus;
                    break;
                case "00 0d":
                    _messageType = DeviceMessageType.UploadUnknownSms;
                    break;
                case "00 0e":
                    _messageType = DeviceMessageType.DeviceAgpsTime;
                    break;
                case "00 0f":
                    _messageType = DeviceMessageType.UpdateAgpsRequest;
                    break;
                case "00 10":
                    _messageType = DeviceMessageType.UpdateAgpsResponse;
                    break;
                case "00 11":
                    _messageType = DeviceMessageType.SmsNoticeParameters;
                    break;
                case "00 14":
                    _messageType = DeviceMessageType.ComfirmTime;
                    break;
                default:
                    break;
            }
        }
    }
}
