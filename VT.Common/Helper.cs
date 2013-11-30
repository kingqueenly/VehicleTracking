using System;
using System.Collections.Generic;

namespace VT.Common
{
    public class Helper
    {
        //��ˮ��
        private static byte threadNO = 0x0;

        private static object obj = new object();

        /// <summary>
        /// ȡ��Ϣ��ˮ��
        /// </summary>
        /// <returns>��ˮ��</returns>
        public static byte GetThreadNO()
        {
            lock (obj)
            {
                threadNO++;
            }
            return threadNO;
        }

        /// <summary>
        /// 10��������16���Ʊ���
        /// </summary>
        /// <param name="dec">10������</param>
        /// <returns>����֮���16������</returns>
        public static byte Hcd(int dec)
        {
            byte b = (byte)0;
            string s = dec.ToString();
            double m = 0.0;
            for (int x = s.Length - 1; x > -1; x--)
            {
                int z = Convert.ToInt32(s[x].ToString());
                b += (byte)(z * Math.Pow(16.0, m));
                m += 1.0;
            }
            return b;
        }

        public static string GetEnglishMonth(int month)
        {
            string eMonth = "";
            switch (month)
            {
                case 1:
                    eMonth = "Jan";
                    break;
                case 2:
                    eMonth = "Feb";
                    break;
                case 3:
                    eMonth = "Mar";
                    break;
                case 4:
                    eMonth = "Apr";
                    break;
                case 5:
                    eMonth = "May";
                    break;
                case 6:
                    eMonth = "Jun";
                    break;
                case 7:
                    eMonth = "Jul";
                    break;
                case 8:
                    eMonth = "Aug";
                    break;
                case 9:
                    eMonth = "Sep";
                    break;
                case 10:
                    eMonth = "Oct";
                    break;
                case 11:
                    eMonth = "Nov";
                    break;
                case 12:
                    eMonth = "Dec";
                    break;
            }
            return eMonth;
        }
    }
}