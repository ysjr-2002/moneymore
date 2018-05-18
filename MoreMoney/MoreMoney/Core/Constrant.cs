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
            if (receive != null)
            {
                if (receive.Length == 4)
                {
                    //S LL E
                }
                else
                {
                    //S HFNNN HFGGGGG ….. LL E
                    var str = receive.ToAscii(1, 4);
                    Log.In("HFNNN->" + str);

                    var repeatBuffer = Util.getRepeatbuffer(receive, 6, 7);
                    foreach (var item in repeatBuffer)
                    {
                        str = item.ToArray().ToAscii();
                        Log.In("HFGGGGG->" + str);
                    }
                }
            }
        }

        public void ReadId()
        {
            var send = Package.Read_Cassetteid();
            com.Write(send);
            var receive = com.Receive();
            if (receive != null)
            {
                var repeatBuffer = Util.getRepeatbuffer(receive, 1, 7);
                foreach (var item in repeatBuffer)
                {
                    //HFGGGGG
                    //var h = 
                    var str = item.ToAscii(2, 5);
                    Log.In(str);
                }
            }
        }

        public void OpenCassette()
        {
            var send = Package.Open_Cassette();
            com.Write(send);
            var receive = com.Receive();
        }

        public void Work()
        {
            byte[] send;
            send = Package.Open_Cassette();
            com.Write(send);

            send = Package.Read_Cassetteid();
            com.Write(send);

            send = Package.Close_Cassette();
            com.Write(send);
        }

        public void CloseCassette()
        {
            var send = Package.Close_Cassette();
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

        public void MoveForward(byte hn, string money)
        {
            var send = Package.MoveForward(hn, money);
            com.Write(send);
            var receive = com.Receive();
            if (receive != null)
            {
                var s = (char)receive[0];
                Log.In("status code->" + s);
                if (s == '0')
                {
                    var repeatbuffer = Util.getRepeatbuffer(receive, 1, 5);
                    foreach (var item in repeatbuffer)
                    {
                        //HFNNN
                        var h = (char)item[0];
                        var f = (char)item[1];
                        var number = item.ToArray().ToAscii(2, 3);
                        Log.In(string.Format("HFNNN-> {0} {1} {2}", h, f, number));
                    }
                }
                else
                {
                    //the command X’36’ Check Delivered Notes
                }
            }
        }
    }
}
