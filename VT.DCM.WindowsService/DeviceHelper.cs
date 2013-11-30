using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VT.Model;

namespace VT.DCM.WindowsService
{
    public class DeviceHelper
    {
        private IDictionary<string,JBODevice> _deviceList = new Dictionary<string,JBODevice>();

        public IDictionary<string, JBODevice> DeviceList
        {
            get { return _deviceList; }
            set { _deviceList = value; }
        }

        static DeviceHelper _instance = null;

        public static DeviceHelper Instance()
        {
            if (_instance == null)
            {
                _instance = new DeviceHelper();
            }
            return _instance;
        }

        private DeviceHelper()
        {
 
        }
    }
}
