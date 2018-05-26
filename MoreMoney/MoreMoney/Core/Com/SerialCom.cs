using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MoreMoney.Core
{
    public class SerialCom
    {
        private string com = "";
        private SerialPort port;

        public SerialCom(string com)
        {
            this.com = com;
        }

        public bool Open(out string msg)
        {
            try
            {
                port = new SerialPort(com, 9600, Parity.Even, 7, StopBits.One);
                port.Handshake = Handshake.None;
                port.Open();
                msg = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return false;
            }
        }

        public void Close()
        {
            if (port != null && port.IsOpen)
                port.Close();

            port = null;
        }

        public byte[] Receive()
        {
            if (port == null)
                return null;

            try
            {
                List<byte> data = new List<byte>();
                byte b = 0;
                port.DiscardInBuffer();
                while ((b = (byte)port.ReadByte()) != Package.EOM)
                {
                    data.Add(b);
                }
                data.Add(Package.EOM);
                if (Package.Check_Receive_Lrc(data.ToArray()))
                    return data.ToArray();
                else
                {
                    Log.In("校验码错误!");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Log.In("error->" + ex.Message);
                return null;
            }
        }

        public void Write(byte[] data)
        {
            Log.Out(data.ToAscii());
            if (port != null && port.IsOpen)
            {
                port.Write(data, 0, data.Length);
            }
            Thread.Sleep(0);
        }
    }
}
