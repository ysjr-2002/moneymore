using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreMoney.Core
{
    class Package
    {
        public const int cmd_reset = 0x30;
        /// <summary>
        /// MOVE FORWARD
        /// </summary>
        public const int cmd_forward = 0x32;
        /// <summary>
        /// READ CASSETTE-ID
        /// </summary>
        public const int cmd_read_cassetteid = 0x35;
        /// <summary>
        /// 
        /// </summary>
        public const int cmd_checkdevilier = 0x36;
        /// <summary>
        /// CLOSE CASSETTE 
        /// </summary>
        public const int cmd_close_cassette = 0x37;
        /// <summary>
        ///  OPEN CASSETTE
        /// </summary>
        public const int cmd_open_cassette = 0x38;

        public const int cmd_PROGRAM_ID = 0x41;

        public const int cmd_self_test = 0x47;

        /// <summary>
        /// 结束符
        /// </summary>
        public const int EOM = 0X0d;

        public static void Musthavedata()
        {
            //X’32’	MOVE FORWARD
            //X’47’	SEND SELF TEST DATA
            //X’52’ X’44’	READ DATA 
            //X’57’ X’44’	WRITE DATA
        }

        public static byte[] reset()
        {
            List<byte> list = new List<byte>();
            list.Add(cmd_reset);
            composite(list);
            return list.ToArray();
        }

        public static byte[] open_cassette()
        {
            List<byte> list = new List<byte>();
            list.Add(cmd_open_cassette);
            composite(list);
            return list.ToArray();
        }

        public static byte[] read_cassetteid()
        {
            List<byte> list = new List<byte>();
            list.Add(cmd_read_cassetteid);
            composite(list);
            return list.ToArray();
        }

        public static byte[] close_cassette()
        {
            List<byte> list = new List<byte>();
            list.Add(cmd_close_cassette);
            composite(list);
            return list.ToArray();
        }

        public static byte[] readPROGRAM()
        {
            List<byte> list = new List<byte>();
            list.Add(cmd_PROGRAM_ID);
            composite(list);
            return list.ToArray();
        }

        public static byte[] selftest()
        {
            List<byte> list = new List<byte>();
            list.Add(cmd_self_test);
            composite(list);
            return list.ToArray();
        }

        public static byte[] counter()
        {
            var cmd = "RD/303";
            List<byte> list = new List<byte>();
            list.AddRange(Encoding.ASCII.GetBytes(cmd));
            composite(list);
            return list.ToArray();
        }

        /// <summary>
        /// 查钱
        /// </summary>
        /// <returns></returns>
        public static byte[] chaqian()
        {
            List<byte> list = new List<byte>();
            return list.ToArray();
        }

        public static byte[] readPROGRAMID()
        {
            List<byte> list = new List<byte>();
            list.Add((byte)'A');
            composite(list);
            return list.ToArray();
        }

        public static void composite(List<byte> list)
        {
            byte l1, l2;
            lrc(list.ToArray(), out l1, out l2);
            list.Add(l1);
            list.Add(l2);
            list.Add(EOM);
        }

        public static void lrc(byte[] bytes, out byte l1, out byte l2)
        {
            byte v = 0x00;
            l1 = 0;
            l2 = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                v = (byte)(v ^ bytes[i]);
            }

            var y = v / 0x10;
            var z = v & 0x0F;

            l1 = (byte)(y | 0x30);
            l2 = (byte)(z | 0x30);

            var lx1 = l1.ToString("X2");
            var lx2 = l2.ToString("X2");
        }

        public static bool check_receive_lrc(byte[] source)
        {
            byte send_l1, send_l2;
            var len = source.Length;
            send_l1 = source[len - 3];
            send_l2 = source[len - 2];

            byte[] data = new byte[source.Length - 3];
            Array.Copy(source, data, data.Length);

            byte l1, l2;
            lrc(data, out l1, out l2);

            if (send_l1 == l1 && send_l2 == l2)
                return true;
            else
                return false;
        }
    }
}
