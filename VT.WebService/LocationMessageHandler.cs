using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web.Script.Serialization;
using log4net;
using VT.Device.Model;

namespace VT.WebService
{
    public class LocationMessageHandler
    {
        //private static readonly ILog log = LogManager.GetLogger(typeof(DCMServer));

        //VTDataContext dataContext = new VTDataContext();

        public void Add(Location locationMessage)
        {
            try
            {
                //Location location = new Location();
                //location.CoordinateTypeID = locationMessage.CoordinateType.GetHashCode();
                //location.DeviceID = locationMessage.DeviceID;
                //location.Direction = locationMessage.Direction;
                //location.EngineTemperature = locationMessage.EngineTemperature;
                //location.GpsSpeed = locationMessage.GpsSpeed;
                //location.GpsTime = locationMessage.GpsTime;
                //location.GpsLatitude = locationMessage.Latitude;
                //location.GpsLongitude = locationMessage.Longitude;
                //location.ObdSpeed = locationMessage.ObdSpeed;
                //location.SatelliteNum = locationMessage.SatelliteNum;

                //BaiduCoordinate baiduCoord = ConvertCoordinate(new GpsCoordinate { Latitude = locationMessage.Latitude, Longitude = locationMessage.Longitude });
                //location.BaiduLatitude = baiduCoord.Latitude;
                //location.BaiduLongitude = baiduCoord.Longitude;

                //dataContext.Location.InsertOnSubmit(location);
                //dataContext.SubmitChanges();

                
            }
            catch (Exception ex)
            {
                //log.Error("Exception", ex);
            }
        }

        private BaiduCoordinate ConvertCoordinate(GpsCoordinate gpsCoord)
        {
            //其中：
            //from: 来源坐标系   （0表示原始GPS坐标，2表示Google坐标）
            //to:   转换后的坐标   （4就是百度自己啦，好像这个必须是4才行）
            //x:    经度
            //y:    纬度
            //返回的结果是一个json字符串：
            //{"error":0,"x":"MTIxLjUwMDIyODIxNDk2","y":"MzEuMjM1ODUwMjYwMTE3"}
            //其中：
            //error：是结果是否出错标志位，"0"表示OK
            //x:     百度坐标系的经度(Base64加密)
            //y:     百度坐标系的纬度(Base64加密)

            string url = string.Format("http://api.map.baidu.com/ag/coord/convert?from={0}&to={1}&x={2}&y={3}", 0, 4, gpsCoord.Longitude, gpsCoord.Latitude);
            WebClient wc = new WebClient();
            byte[] gps = wc.DownloadData(url);

            JavaScriptSerializer js = new JavaScriptSerializer();
            CoordinateConvertResponse rsp = js.Deserialize<CoordinateConvertResponse>(Encoding.UTF8.GetString(gps));

            BaiduCoordinate baiduCoord = new BaiduCoordinate();

            baiduCoord.Latitude = Convert.ToDecimal(Encoding.UTF8.GetString(Convert.FromBase64String(rsp.y)));
            baiduCoord.Longitude = Convert.ToDecimal(Encoding.UTF8.GetString(Convert.FromBase64String(rsp.x)));
            return baiduCoord;
        }
    }
}
