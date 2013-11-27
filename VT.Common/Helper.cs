using System;
using System.Collections.Generic;

namespace VT.Common
{
    public class Helper
    {
        //流水号
        private static byte threadNO = 0x0;

        private static object obj = new object();

        /// <summary>
        /// 取消息流水号
        /// </summary>
        /// <returns>流水号</returns>
        public static byte GetThreadNO()
        {
            lock (obj)
            {
                threadNO++;
            }
            return threadNO;
        }

        /// <summary>
        /// 10进制数据16进制编码
        /// </summary>
        /// <param name="dec">10进制数</param>
        /// <returns>编码之后的16进制数</returns>
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
    }
}