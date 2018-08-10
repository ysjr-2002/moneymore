using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoneyCore.CashInEx
{
    /// <summary>
    /// 消息包
    /// </summary>
    class Message
    {
        private static byte head = 0x7F;
        private static byte seq1 = 0x80;
        private static byte seq2 = 0x00;

        private static int index = 0;
        /// <summary>
        /// SEQ必须交替发送
        /// </summary>
        public static byte seq
        {
            get
            {
                if (index == 0)
                {
                    index = 1;
                    return seq1;
                }
                else
                {
                    index = 0;
                    return seq2;
                }
            }

        }

        public static byte[] getSendBytes(byte cmd)
        {
            List<byte> list = new List<byte>();
            list.Add(head);
            list.Add(seq);
            list.Add(1);
            list.Add(cmd);

            CRC.Get_CRC(list.ToArray());

            list.Add(CRC.CRCL);
            list.Add(CRC.CRCH);

            var temp = list.ToArray();
            Log.Out("data=" + temp.ToHex());
            return temp;
        }

        /// <summary>
        /// 设置允许识别哪几种纸币
        /// </summary>
        /// <returns></returns>
        public static byte[] getOpenChannelBytes()
        {
            List<byte> list = new List<byte>();
            list.Add(head);
            list.Add(seq);
            list.Add(03);
            list.Add(02);
            list.Add(0xFF); //全部通道打开， 实际使用过程中，需要调整
            list.Add(00);

            CRC.Get_CRC(list.ToArray());

            list.Add(CRC.CRCL);
            list.Add(CRC.CRCH);

            var temp = list.ToArray();
            Log.Out("data=" + temp.ToHex());
            return temp;
        }
    }
}
