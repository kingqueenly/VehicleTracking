using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Net;
using System.Web.Script.Serialization;
using VT.Model.Device;
using VT.Common;

namespace VT.DAL
{
    public class LocationDAL
    {
        public static void UpdateCurrentPosition(SqlParameter[] commandParameters)
        {
            string sql = @"
            IF EXISTS(SELECT 1 FROM [Vehicle_Current_Location] WHERE IDCode = @IDCode )
            BEGIN
            UPDATE [Vehicle_Current_Location]
               SET [GpsTime] = @GpsTime
                  ,[SatelliteNum] = @SatelliteNum
                  ,[GpsLongitude] = @GpsLongitude
                  ,[GpsLatitude] = @GpsLatitude
                  ,[BaiduLongitude] = @BaiduLongitude
                  ,[BaiduLatitude] = @BaiduLatitude
                  ,[Direction] = @Direction
                  ,[GpsSpeed] = @GpsSpeed
                  ,[ObdSpeed] = @ObdSpeed
                  ,[EngineTemperature] = @EngineTemperature
                  ,[CoordinateTypeID] = @CoordinateTypeID
             WHERE [IDCode] = @IDCode
            END
            ELSE
            BEGIN
            INSERT INTO [Vehicle_Current_Location]
                       ([IDCode]
                       ,[GpsTime]
                       ,[SatelliteNum]
                       ,[GpsLongitude]
                       ,[GpsLatitude]
                       ,[BaiduLongitude]
                       ,[BaiduLatitude]
                       ,[Direction]
                       ,[GpsSpeed]
                       ,[ObdSpeed]
                       ,[EngineTemperature]
                       ,[CoordinateTypeID])
                 VALUES
                       (@IDCode ,@GpsTime ,@SatelliteNum ,@GpsLongitude ,@GpsLatitude ,@BaiduLongitude ,@BaiduLatitude ,
                        @Direction ,@GpsSpeed ,@ObdSpeed ,@EngineTemperature ,@CoordinateTypeID)
            END
            ";
            SQLHelper.ExecuteNonQuery(SQLHelper.ConnectionString, CommandType.Text, sql, commandParameters);
        }

        public static void AddPosition(Location location)
        {
            string sql = @"
            INSERT INTO [{0}]
                       ([IDCode]
                       ,[GpsTime]
                       ,[SatelliteNum]
                       ,[GpsLongitude]
                       ,[GpsLatitude]
                       ,[BaiduLongitude]
                       ,[BaiduLatitude]
                       ,[Direction]
                       ,[GpsSpeed]
                       ,[ObdSpeed]
                       ,[EngineTemperature]
                       ,[CoordinateTypeID])
                 VALUES
                       (@IDCode ,@GpsTime ,@SatelliteNum ,@GpsLongitude ,@GpsLatitude ,@BaiduLongitude ,@BaiduLatitude ,
                        @Direction ,@GpsSpeed ,@ObdSpeed ,@EngineTemperature ,@CoordinateTypeID)";

            sql = string.Format(sql, GetTableName());

            List<SqlParameter> commandParameters = new List<SqlParameter>();
            commandParameters.Add(new SqlParameter("@IDCode",location.IDCode));
            commandParameters.Add(new SqlParameter("@GpsTime", location.GpsTime));
            commandParameters.Add(new SqlParameter("@SatelliteNum", location.SatelliteNum));
            commandParameters.Add(new SqlParameter("@GpsLongitude", location.Longitude));
            commandParameters.Add(new SqlParameter("@GpsLatitude", location.Latitude));
            BaiduCoordinate baiduCoord = ConvertCoordinate(new GpsCoordinate { Latitude = location.Latitude, Longitude = location.Longitude });
            commandParameters.Add(new SqlParameter("@BaiduLongitude", baiduCoord.Longitude));
            commandParameters.Add(new SqlParameter("@BaiduLatitude", baiduCoord.Latitude));
            commandParameters.Add(new SqlParameter("@Direction", location.Direction));
            commandParameters.Add(new SqlParameter("@GpsSpeed", location.GpsSpeed));
            commandParameters.Add(new SqlParameter("@ObdSpeed", location.ObdSpeed));
            commandParameters.Add(new SqlParameter("@EngineTemperature", location.EngineTemperature));
            commandParameters.Add(new SqlParameter("@CoordinateTypeID", location.CoordinateType.GetHashCode()));

            SQLHelper.ExecuteNonQuery(GetEntConnectionString(location.IDCode), CommandType.Text, sql, commandParameters.ToArray());
            UpdateCurrentPosition(commandParameters.ToArray());
        }

        private static string GetEntConnectionString(string idcode)
        {
            string sql = @"
            SELECT TOP 1 ConnectionString FROM Master_Enterprise   WHERE EnterpriseID = (
            SELECT TOP 1 EnterpriseID     FROM Enterprise_Vehicles WHERE VehicleID = (
            SELECT TOP 1 VehicleID        FROM Master_VehicleInfo  WHERE IDCode = @IDCode))";

            object obj = SQLHelper.ExecuteScalar(SQLHelper.ConnectionString, CommandType.Text, sql, new SqlParameter("@IDCode", idcode));

            return obj.ToString();
        }

        private static string GetTableName()
        {
            return string.Format("History_Location_{0}_{1}", DateTime.Now.Year,Helper.GetEnglishMonth(DateTime.Now.Month));
        }

        private static BaiduCoordinate ConvertCoordinate(GpsCoordinate gpsCoord)
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
