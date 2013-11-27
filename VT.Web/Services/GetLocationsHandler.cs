using System;
using System.Web;
using System.Collections.Generic;
using System.Text;

namespace VT.Web.Services
{
    public class GetLocationsHandler : IHttpHandler
    {
        /// <summary>
        /// 您将需要在您网站的 web.config 文件中配置此处理程序，
        /// 并向 IIS 注册此处理程序，然后才能进行使用。有关详细信息，
        /// 请参见下面的链接: http://go.microsoft.com/?linkid=8101007
        /// </summary>
        #region IHttpHandler Members

        public bool IsReusable
        {
            // 如果无法为其他请求重用托管处理程序，则返回 false。
            // 如果按请求保留某些状态信息，则通常这将为 false。
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            //BLLLocation bllLocation = new BLLLocation();
            //List<BLL.Location> locations = bllLocation.GetLocations();
            //locations.Reverse();
            //StringBuilder sb = new StringBuilder();
            //foreach (BLL.Location location in locations)
            //{
            //    if (location.BaiduLatitude.HasValue && location.BaiduLongitude.HasValue)
            //    {
            //        sb.Append(string.Format("{0},{1}", location.BaiduLongitude, location.BaiduLatitude));
            //        sb.Append(";");
            //    }
            //}

            ////string formatedCoordinate = "121.5356439,31.2810635;121.537615,31.278752;121.535615,31.276752;121.536615,31.274752;121.533615,31.277752";//
            //string formatedCoordinate = sb.ToString(0, sb.Length - 1);
            //context.Response.Write(formatedCoordinate);
        }

        #endregion
    }
}