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
            byte[] buffer = { 170, 18, 52, 86, 120, 144, 0, 0, 0, 0, 1, 109, 0, 26, 0, 4, 19, 17, 39, 32, 3, 83, 8, 0, 78, 8, 16, 64, 51, 147, 48, 41, 137, 148, 11, 0, 0, 0, 0, 0, 14, 170 };
            Location location = new Location(buffer);
            if (location.IsGpsLocation && location.CoordinateType == LocationCoordinateType.GPS)
            {
                LocationDAL.AddPosition(location);
            }
        }
    }
}
