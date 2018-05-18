using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MoreMoney.Core
{
    public class SerialComIC
    {
        private SerialPort port;
        private Thread thread;
        private bool stop = false;
        private const byte etx_end2 = 0x0D;
        private const byte etx_end1 = 0x0A;
        public event OnReadCardEventHandle OnReadCardNo;

        public SerialComIC(string com)
        {
            port = new SerialPort(com, 9600, Parity.Even, 8, StopBits.One);
        }

        public bool Open(out string msg)
        {
            try
            {
                port.Open();
                stop = false;
                thread = new Thread(Run);
                thread.Start();
                msg = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return false;
            }
        }

        private void Run()
        {
            while (!stop)
            {
                var cardNo = ReadData();
                if (string.IsNullOrEmpty(cardNo))
                {
                    stop = true;
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
                while ((b = (byte)port.ReadByte()) != etx_end2)
                {
                    bytes.Add(b);
                }
                var temp = bytes.ToArray().ToAscii();
                var cardNo = temp.PadLeft(16, '0');
                return cardNo;
            }
            catch
            {
                return string.Empty;
            }
        }

        public void Close()
        {
            stop = true;
            if (port != null && port.IsOpen)
                port.Close();

            port = null;
        }
    }
}
