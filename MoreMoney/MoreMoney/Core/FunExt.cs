using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreMoney
{
    public static class Ext
    {
        public static byte[] ToAscii(this string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }

        public static string ToAscii(this byte[] data)
        {
            var str = Encoding.ASCII.GetString(data);
            return str;
        }
    }
}
