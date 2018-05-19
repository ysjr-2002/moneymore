using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreMoney.Core
{
    /// <summary>
    /// 硬币找零(1,5)
    /// </summary>
    class CoinChanger
    {
        bool stop = false;
        SerialPort sp = null;
        ChargeMoneyType chargeType;
        public event OnChargingEventHandler OnCharging;
        public event OnHopperEmptyEventHandler OnHopperEmpty;

        public CoinChanger(string port, ChargeMoneyType chargeType)
        {
            this.sp = new SerialPort(port, 9600, Parity.None, 8, StopBits.One);
            this.chargeType = chargeType;
        }

        public bool Open(out string msg)
        {
            try
            {
                sp.Open();
                stop = false;
                Run();
                msg = "";
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
            Task.Factory.StartNew(() =>
            {
                Read();
            });
        }

        private void Read()
        {
            while (!stop)
            {
                byte b = (byte)sp.ReadByte();
                if (b == 0x4F)
                {
                    Console.WriteLine("Hopper Empty");
                    if (OnHopperEmpty != null)
                    {
                        OnHopperEmpty(this, EventArgs.Empty);
                    }
                }
                if (b == 0x52)
                {
                    //每出一个，设备返回一个
                    Console.WriteLine("OK");
                    if (OnCharging != null)
                    {
                        OnCharging(this, new ChargeItem { ChargeMoneyType = chargeType });
                    }
                }
            }
        }

        /// <summary>
        /// 发送找零指令
        /// </summary>
        /// <param name="money"></param>
        public void Charge(string money)
        {
            var numbers = money.ToCharArray();
            List<byte> send = new List<byte>();
            send.Add(0x02);
            send.Add(0x53);
            foreach (var item in numbers)
            {
                send.Add((byte)item);
            }
            send.Add(0x03);
            var total = send.ToArray();

            if (sp.IsOpen)
                sp.Write(total, 0, total.Length);
        }

        public void Close()
        {
            stop = true;
            if (sp != null && sp.IsOpen)
            {
                sp.Close();
            }
        }
    }
}
