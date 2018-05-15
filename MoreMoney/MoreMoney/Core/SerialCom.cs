using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreMoney.Core
{
    public class SerialCom
    {
        private SerialPort port;

        public bool Open(string com)
        {
            try
            {
                port = new SerialPort(com, 9600, Parity.Even, 8, StopBits.One);
                port.Open();
                return true;
            }
            catch (Exception ex)
            {
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

            List<byte> data = new List<byte>();
            byte b = 0;
            while ((b = (byte)port.ReadByte()) != Package.EOM)
            {
                data.Add(b);
            }
            data.Add(Package.EOM);

            if (Package.Check_Receive_Lrc(data.ToArray()))
                return data.ToArray();
            else
            {
                Log.Out("校验码错误!");
                return null;
            }
        }

        public void Write(byte[] data)
        {
            Log.Out(data.ToAscii());
            if (port != null && port.IsOpen)
                port.Write(data, 0, data.Length);
        }
    }
}
