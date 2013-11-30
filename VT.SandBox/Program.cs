using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using VT.DAL;
using VT.Model.Device;

namespace VT.SandBox
{
    class Program
    {
        static void Main(string[] args)
        {
            //byte[] buffer = { 170, 18, 52, 86, 120, 144, 0, 0, 0, 0, 1, 109, 0, 26, 0, 4, 19, 17, 39, 32, 3, 83, 8, 0, 78, 8, 16, 64, 51, 147, 48, 41, 137, 148, 11, 0, 0, 0, 0, 0, 14, 170 };
            //Location location = new Location(buffer);
            //if (location.IsGpsLocation && location.CoordinateType == LocationCoordinateType.GPS)
            //{
            //    LocationDAL.AddPosition(location);
            //}

            string hex = "AA 12 34 56 78 90 00 00 00 00 01 6D 00 1A 00 04 13 11 27 20 03 53 08 00 4E 08 10 40 33 93 30 29 89 94 0B 00 00 00 00 00 0E AA";
            string idcode = hex.Substring(3, 38);
        }
    }
}
