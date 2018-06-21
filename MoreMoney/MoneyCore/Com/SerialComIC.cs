using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MoneyCore
{
    public class SerialComIC
    {
        private SerialPort port;
        private Thread thread;
        private bool bRun = false;
        private const byte etx_end2 = 0x0D;
        private const byte etx_end1 = 0x0A;
        public event OnReadCardEventHandle OnReadCardNo;

        public SerialComIC(string com)
        {
            port = new SerialPort(com, 9600, Parity.None, 8, StopBits.One);
        }

        public bool Open(out string msg)
        {
            try
            {
                msg = string.Empty;
                if (bRun)
                    return true;
                port.Open();
                bRun = true;
                thread = new Thread(Run);
                thread.Start();
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                bRun = false;
            }
            return bRun;
        }

        private void Run()
        {
            while (bRun)
            {
                var cardNo = ReadData();
                if (string.IsNullOrEmpty(cardNo))
                {
                    bRun = false;
                    break;
                }
                if (OnReadCardNo != null)
                {
                    OnReadCardNo(this, cardNo);
                }
            }
        }

        private string ReadData()
        {
            try
            {
                byte b = 0;
                List<byte> bytes = new List<byte>();
                while ((b = (byte)port.ReadByte()) > 0)
                {
                    if (b != etx_end2 && b != etx_end1)
                        bytes.Add(b);

                    if (b == etx_end1)
                        break;
                }
                var cardStr = bytes.ToArray().ToAscii();
                Log.In("原始卡号->" + cardStr);
                var cardInt = Convert.ToUInt32(cardStr);
                var cardHex = cardInt.ToString("X2");
                Log.In("16进制卡号->" + cardHex);
                cardHex = cardHex.PadLeft(8, '0');
                //取后6位
                cardHex = cardHex.Substring(2);
                cardStr = cardHex.PadLeft(16, '0');
                return cardStr;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public void Close()
        {
            bRun = false;
            if (port != null && port.IsOpen)
                port.Close();
        }
    }
}
