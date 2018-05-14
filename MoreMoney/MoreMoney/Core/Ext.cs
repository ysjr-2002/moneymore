using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreMoney
{
    static class Ext
    {
        public static byte[] stringToAscii(this string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }

        public static string byteToAscii(this byte[] data)
        {
            var str = Encoding.ASCII.GetString(data);
            return str;
        }
    }
}
