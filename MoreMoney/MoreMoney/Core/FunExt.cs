﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreMoney
{
    public static class Ext
    {
        public static byte ToByte(this string s)
        {
            byte ret = 0;
            byte.TryParse(s, out ret);
            return ret;
        }

        public static int ToInt32(this string s)
        {
            int ret = 0;
            int.TryParse(s, out ret);
            return ret;
        }

        public static decimal ToDecimal(this string s)
        {
            decimal ret = 0;
            decimal.TryParse(s, out ret);
            return ret;
        }

        public static string ToStr(this byte[] bytes)
        {
            var str = string.Join(" ", bytes.Select(s => string.Format("{0:d2}", s)));
            return str;
        }

        public static byte[] ToAscii(this string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }

        public static string ToAscii(this byte[] data)
        {
            var str = Encoding.ASCII.GetString(data);
            return str;
        }

        public static string ToAscii(this byte[] data, int index, int count)
        {
            var str = Encoding.ASCII.GetString(data, index, count);
            return str;
        }
    }
}
