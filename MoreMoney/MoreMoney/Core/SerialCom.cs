using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreMoney.Core
{
    class SerialCom
    {
        SerialPort sp;
        public bool Open(string com)
        {
            try
            {
                sp = new SerialPort(com, 9600, Parity.Even, 8, StopBits.One);
                sp.Open();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public byte[] receive()
        {
            if (sp == null)
                return null;

            List<byte> data = new List<byte>();
            byte b = 0;
            while ((b = (byte)sp.ReadByte()) != Package.EOM)
            {
                data.Add(b);
            }
            data.Add(Package.EOM);
            return data.ToArray();
        }

        public void write(byte[] data)
        {
            log(data);
            if (sp != null && sp.IsOpen)
                sp.Write(data, 0, data.Length);
        }

        private void log(byte[] data)
        {
            var log = Encoding.ASCII.GetString(data);
            Log.Out(log);
        }
    }
}
