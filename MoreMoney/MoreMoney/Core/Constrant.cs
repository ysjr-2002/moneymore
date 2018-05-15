using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreMoney.Core
{
    /// <summary>
    /// 投币机的约定
    /// </summary>
    public class Constrant
    {
        private SerialCom com;
        public Constrant(SerialCom com)
        {
            this.com = com;
        }

        public void Reset()
        {
            var send = Package.Reset();
            com.Write(send);
            var receive = com.Receive();
        }

        public void ReadId()
        {
            var send = Package.Read_cassetteid();
            com.Write(send);
            var receive = com.Receive();
            if (receive != null)
            {
                var list = Util.getRepeatbuffer(receive, 7);
                foreach (var buffer in list)
                {
                    //HFGGGGG
                    var str = buffer.ToAscii();
                    Log.In(str);
                }
            }
        }

        public void OpenCassette()
        {
            var send = Package.open_cassette();
            com.Write(send);
            var receive = com.Receive();
        }

        public void Work()
        {
            byte[] send;
            send = Package.open_cassette();
            com.Write(send);

            send = Package.Read_cassetteid();
            com.Write(send);

            send = Package.Close_cassette();
            com.Write(send);
        }

        public void CloseCassette()
        {
            var send = Package.Close_cassette();
            com.Write(send);
            var receive = com.Receive();
        }

        public void ReadPROGRAM()
        {
            var send = Package.ReadProgramID();
            com.Write(send);
            var receive = com.Receive();
            if (receive != null)
            {
                var no = Encoding.ASCII.GetString(receive, 2, 8);
                Log.In("no->" + no);
            }
            Log.In("receive");
        }

        public void Counter()
        {
            var send = Package.Counter();
            com.Write(send);
            var receive = com.Receive();
        }

        public void SelfTest()
        {
            var send = Package.SelfTest();
            com.Write(send);
            var receive = com.Receive();
        }

        public void ReadData()
        {
            var send = Package.ReadData("");
            com.Write(send);
            var receive = com.Receive();
        }

        public void WriteData()
        {
            var send = Package.WriteData("");
            com.Write(send);
            var receive = com.Receive();
        }
    }
}
