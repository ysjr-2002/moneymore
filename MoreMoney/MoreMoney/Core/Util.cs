using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreMoney.Core
{
    class Util
    {
        /// <summary>
        /// 读取返回字节的重复命令
        /// </summary>
        /// <param name="data"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static List<byte[]> getRepeatbuffer(byte[] data, int len)
        {
            var pos = 0;
            var list = new List<byte[]>();
            var repeatArray = new byte[data.Length - 4];
            Array.Copy(data, 1, repeatArray, 0, repeatArray.Length);
            while (true)
            {
                byte[] temp = new byte[len];
                Array.Copy(data, pos, temp, 0, len);
                list.Add(temp);
                pos += len;
                if (pos >= repeatArray.Length)
                    break;
            }
            return list;
        }
    }
}
