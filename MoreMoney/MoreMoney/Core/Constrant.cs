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
            var send = Package.read();
            com.write(send);
            var receive = com.receive();
        }

        public void Open()
        {
            var send = Package.open();
            com.write(send);
            var receive = com.receive();
        }

        public void Work()
        {
            byte[] send;
            send = Package.open();
            com.write(send);

            send = Package.read();
            com.write(send);


            send = Package.close();
            com.write(send);

        }

        internal void Close()
        {
            var send = Package.close();
            com.write(send);
            var receive = com.receive();
        }

        internal void ReadPROGRAM()
        {
            var send = Package.readPROGRAMID();
            com.write(send);
            var receive = com.receive();
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
