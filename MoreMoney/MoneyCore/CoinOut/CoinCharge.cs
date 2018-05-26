using MoneyCore.CoinCore;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyCore
{
    /// <summary>
    /// 硬币找零(1,5)
    /// </summary>
    public class CoinCharge
    {
        SerialPort sp = null;
        ChargeMoneyType chargeType;
        private const int READ_TIME_OUT = 100;

        public CoinCharge(string port, ChargeMoneyType chargeType)
        {
            this.sp = new SerialPort(port, 9600, Parity.None, 8, StopBits.One);
            this.chargeType = chargeType;
        }

        public bool Open(out string msg)
        {
            try
            {
                sp.Open();
                msg = "";
                return true;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 发送找零指令
        /// </summary>
        /// <param name="money"></param>
        public void Charge(string money)
        {
            var count = money.ToByte();
            for (int i = 1; i <= count; i++)
            {
                CoinChargeAnswer answer = Charge();
                if (answer == CoinChargeAnswer.OK)
                {
                    DllLog.In("出->1");
                }
                if (answer == CoinChargeAnswer.HopperEmpty)
                {
                    DllLog.In("出->空");
                }
                if (answer == CoinChargeAnswer.TimeOut)
                {
                    DllLog.In("出->超时");
                }
            }
        }

        public CoinChargeAnswer Charge(char money = '1')
        {
            List<byte> send = new List<byte>();
            send.Add(0x02);
            send.Add(0x53);
            send.Add((byte)money);
            send.Add(0x03);
            var total = send.ToArray();
            sp.Write(total, 0, total.Length);

            CoinChargeAnswer answer = CoinChargeAnswer.TimeOut;
            sp.ReadTimeout = READ_TIME_OUT;
            try
            {
                sp.DiscardInBuffer();
                var b = (byte)sp.ReadByte();
                answer = (CoinChargeAnswer)b;
            }
            catch
            {
            }
            return answer;
        }

        public void Close()
        {
            if (sp != null && sp.IsOpen)
            {
                sp.Close();
            }
        }
    }
}
