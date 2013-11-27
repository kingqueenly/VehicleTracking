using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VT.Common;

namespace VT.Model.Device
{
    public enum LocationCoordinateType
    {
        GPS = 0,
        GSM = 1
    }

    public class CoordinateConvertResponse
    {
        public string error { get; set; }
        public string x { get; set; }
        public string y { get; set; }
    }

    public class Coordinate 
    {
        private decimal _longitude;
        private decimal _latitude;

        public decimal Longitude
        {
            get { return _longitude; }
            set { _longitude = value; }
        }
        
        public decimal Latitude
        {
            get { return _latitude; }
            set { _latitude = value; }
        }
    }

    public class GpsCoordinate : Coordinate
    {
 
    }

    public class BaiduCoordinate : Coordinate
    {
 
    }

    public class Location
    {
        private string _idCode;

        public string IDCode
        {
            get { return _idCode; }
            set { _idCode = value; }
        }
        private DateTime _gpsTime;

        public DateTime GpsTime
        {
            get { return _gpsTime; }
            set { _gpsTime = value; }
        }
        private byte _satelliteNum;

        public byte SatelliteNum
        {
            get { return _satelliteNum; }
            set { _satelliteNum = value; }
        }
        private decimal _longitude;

        public decimal Longitude
        {
            get { return _longitude; }
            set { _longitude = value; }
        }
        private decimal _latitude;

        public decimal Latitude
        {
            get { return _latitude; }
            set { _latitude = value; }
        }
        private byte _direction;

        public byte Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }
        private byte _gpsSpeed;

        public byte GpsSpeed
        {
            get { return _gpsSpeed; }
            set { _gpsSpeed = value; }
        }
        private byte _obdSpeed;

        public byte ObdSpeed
        {
            get { return _obdSpeed; }
            set { _obdSpeed = value; }
        }
        private byte _engineTemperature;

        public byte EngineTemperature
        {
            get { return _engineTemperature; }
            set { _engineTemperature = value; }
        }

        private bool _isGpsLocation;

        public bool IsGpsLocation
        {
            get { return _isGpsLocation; }
            set { _isGpsLocation = value; }
        }
        private LocationCoordinateType _coordinateType;

        public LocationCoordinateType CoordinateType
        {
            get { return _coordinateType; }
            set { _coordinateType = value; }
        }

        public Location(byte[] buffer)
        {
            //AA 12 34 56 78 90 00 00 00 00 01 0D 00 22 00 04 13 11 22 22 22 45 08 00 0F 07 10 40 33 01 30 30 39 50 3D 13 19 47 00 06 05 3A 00 01 00 00 00 01 95 AA
            //0  1  2  3  4  5  6  7  8  9  10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28 29 30 31 32 33 34 35 36 37 38 39 40 41 42 43 44 45 46 47 48 49 
            string location = ByteHelper.BytesToHexString(buffer, buffer.Length);
            string[] details = location.Split(' ');
            _idCode = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}", details[1], details[2], details[3], details[4], details[5], details[6], details[7], details[8], details[9], details[10]);
            _gpsTime = DateTime.Parse(string.Format("20{0}-{1}-{2} {3}:{4}:{5}", details[16], details[17], details[18], details[19], details[20], details[21]));

            short wm = ByteHelper.GetShort(new byte[] { buffer[22], buffer[23] });
            string bins = Convert.ToString(wm, 2).PadLeft(16, '0');
            _coordinateType = bins[2] == '1' ? LocationCoordinateType.GSM : LocationCoordinateType.GPS;
            //_warningMark = string.Format("{0}{1}", details[22], details[23]);
            //_statusBit = details[24];
            byte b = buffer[24];
            bins = Convert.ToString(b, 2).PadLeft(8, '0');
            _isGpsLocation = bins[6] == '1' ? true : false;

            _satelliteNum = buffer[25];
            //_longitude = string.Format("{0}{1}{2}{3}", details[26], details[27], details[28], details[29]);
            //_latitude = string.Format("{0}{1}{2}{3}", details[30], details[31], details[32], details[33]);
            string strLon = string.Format("{0}{1}{2}{3}", details[26], details[27], details[28], details[29]);
            string strLat = string.Format("{0}{1}{2}{3}", details[30], details[31], details[32], details[33]);
            //104 03.301
            decimal d = Convert.ToDecimal(strLon.Substring(0, 3));
            decimal f = Convert.ToDecimal(strLon.Substring(3, 2) + "." + strLon.Substring(5, 3));
            _longitude = d + f / 60;
            //30 30.3950
            d = Convert.ToDecimal(strLat.Substring(0, 2));
            f = Convert.ToDecimal(strLat.Substring(2, 2) + "." + strLat.Substring(4, 4));
            _latitude = d + f / 60;

            _direction = buffer[34];
            _gpsSpeed = buffer[35];
            _obdSpeed = buffer[36];
            _engineTemperature = buffer[37];
        }

    }
}
