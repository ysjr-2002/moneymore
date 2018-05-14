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
    class Constrant
    {
        SerialCom com;
        public Constrant(SerialCom com)
        {
            this.com = com;
        }

        public void Reset()
        {
            var send = Package.reset();
            com.write(send);
            var receive = com.receive();
        }

        public void ReadId()
        {
            var send = Package.read_cassetteid();
            com.write(send);
            var receive = com.receive();
            if (receive != null)
            {
                var list = Util.getRepeatbuffer(receive, 7);
                foreach (var buffer in list)
                {
                    //HFGGGGG
                    var str = buffer.byteToAscii();
                    Log.In(str);
                }
            }
        }

        public void Open()
        {
            var send = Package.open_cassette();
            com.write(send);
            var receive = com.receive();
        }

        public void Work()
        {
            byte[] send;
            send = Package.open_cassette();
            com.write(send);

            send = Package.read_cassetteid();
            com.write(send);


            send = Package.close_cassette();
            com.write(send);

        }

        internal void Close()
        {
            var send = Package.close_cassette();
            com.write(send);
            var receive = com.receive();
        }

        internal void ReadPROGRAM()
        {
            var send = Package.readPROGRAMID();
            com.write(send);
            var receive = com.receive();
            if (receive != null)
            {
                var no = Encoding.ASCII.GetString(receive, 2, 8);
                Log.In("no->" + no);
            }
            Log.In("shit");
        }

        internal void Counter()
        {
            var send = Package.counter();
            com.write(send);
            var receive = com.receive();
        }

        internal void SelfTest()
        {
            var send = Package.selftest();
            com.write(send);
            var receive = com.receive();
        }
    }
}
