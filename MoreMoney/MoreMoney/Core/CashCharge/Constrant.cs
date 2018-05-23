using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MoreMoney.Core
{
    /// <summary>
    /// 纸币找零的约定
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
                    //0 00000 1000001 2000002 00
                    //S HFNNN HFGGGGG ….. LL E
                    var str = receive.ToAscii(1, 5);
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

        public void ClearTransport()
        {
            var send = Package.ClearTransport();
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
                    var str = receive.ToAscii(1, 5);
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
                var s = (char)receive[0];
                if (s == '7' || s == '8' || s == 'N')
                {
                    Log.In("is error");
                    return;
                }
                //0
                //0060000
                //1000001
                //2000002
                //06
                var repeatBuffer = Util.getRepeatbuffer(receive, 1, 7);
                Log.In(string.Format("共{0}个", repeatBuffer.Count));
                foreach (var item in repeatBuffer)
                {
                    //HFGGGGG
                    var h = (char)item[0];
                    if (h == '0')
                    {
                        Log.In("拒绝保险库");
                    }
                    var f = (char)item[1];
                    var str = item.ToAscii(2, 5);
                    Log.In(string.Format("H->{0} F->{1} GGGGG->{2}", h, f, str));
                }
            }
        }

        public void CheckDelivered()
        {
            var send = Package.Check_delivered();
            com.Write(send);
            var receive = com.Receive();
            if (receive != null)
            {
                var s = (char)receive[0];

                var repeatBuffer = Util.getRepeatbuffer(receive, 1, 5);
                Log.In(string.Format("共{0}个", repeatBuffer.Count));
                foreach (var item in repeatBuffer)
                {
                    //HFNNN
                    var h = (char)item[0];
                    if (h == '0')
                    {
                        Log.In("拒绝保险库");
                    }
                    var f = (char)item[1];
                    var str = item.ToAscii(2, 3);
                    Log.In(string.Format("H->{0} F->{1} NNN->{2}", h, f, str));
                }
            }
        }

        public void OpenCassette()
        {
            var send = Package.Open_Cassette();
            com.Write(send);
            return;
            var receive = com.Receive();
            if (receive != null)
            {
                Log.In(receive.ToAscii());
            }
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
            return;
            var receive = com.Receive();
            if (receive != null)
            {
                Log.In(receive.ToAscii());
            }
        }

        public void ReadPROGRAM()
        {

            Task.Factory.StartNew(() =>
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
            });
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

        public void ReadData(string item)
        {
            var send = Package.ReadData(item);
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
            return;
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
                    CheckDelivered();
                }
            }
        }
    }
}
