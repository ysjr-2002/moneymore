using MoreMoney.Core.CashCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MoreMoney.Core
{
    public static class MoneyBus
    {
        public static event OnReadCardEventHandle OnReadCardNo;
        public static event OnAcceptMoneyEventHandler OnAcceptMoney;
        public static event OnAcceptMoneyWithAllEventHandler OnAcceptMoneyWithAll;
        public static event OnChargeEventHandler OnChargeOver;

        static SerialComIC com1;
        static MoneyReceiver com2;
        static CoinChanger coin1_com3;
        static CoinChanger coin5_com4;

        //应收
        static int expectMoney = 0;
        //实收
        static int acceptMoney = 0;

        static int s_m1, s_m5, s_m50, s_m100;
        static AutoResetEvent are = new AutoResetEvent(false);

        public static void Init()
        {
            var msg = "";
            var open = false;
            com1 = new SerialComIC("COM1");
            com1.OnReadCardNo += (s, c) =>
            {
                if (OnReadCardNo != null)
                {
                    OnReadCardNo(s, c);
                }
            };


            com2 = new MoneyReceiver("COM1");
            open = com2.Open(out msg);
            if (!open)
            {
                return;
            }
            com2.OnAcceptMoney += Com2_OnAcceptMoney;

            coin1_com3 = new CoinChanger("COM3", ChargeMoneyType.M1);
            coin5_com4 = new CoinChanger("COM4", ChargeMoneyType.M5);

            coin1_com3.OnCharging += M1_Change;
            coin5_com4.OnCharging += M5_Change;

            coin1_com3.OnHopperEmpty += M1_HopperEmtpy;
            coin5_com4.OnHopperEmpty += M5_HopperEmtpy;

            coin1_com3.Open(out msg);
            coin5_com4.Open(out msg);
        }

        public static bool ReadReadCard()
        {
            var msg = "";
            var open = com1.Open(out msg);
            if (open)
            {
                return false;
            }
            return true;
        }

        public static void CloseReadCard()
        {
            com1.Close();
        }

        public static void ReadPool(int expectMoney)
        {
            acceptMoney = 0;
            MoneyBus.expectMoney = expectMoney;
            com2.Pool();
        }

        public static void StopPool()
        {
            com2.Stop();
        }

        private static void Com2_OnAcceptMoney(object sender, int money)
        {
            acceptMoney += money;
            if (acceptMoney > expectMoney)
            {
                //缴费多，需要找零
                com2.Stop();
                int charge = acceptMoney - expectMoney;
                DoCharge(charge);
            }

            if (acceptMoney == expectMoney)
            {
                //缴费相同，通知结束
                if (OnChargeOver != null)
                {
                    OnChargeOver(null, new List<ChargeItem>());
                }
            }

            if (OnAcceptMoney != null)
            {
                OnAcceptMoney(sender, money);
            }

            if (OnAcceptMoneyWithAll != null)
            {
                OnAcceptMoneyWithAll(sender, money, acceptMoney);
            }
        }

        private static void DoCharge(int charge)
        {
            int m1, m5, m50, m100;
            Charge.GetCount(charge, out m1, out m5, out m50, out m100);
            s_m1 = m1;
            s_m5 = m5;
            s_m50 = m50;
            s_m100 = m100;

            are.Reset();
            if (s_m5 > 0)
            {
                Log.In("5元找零->" + s_m5);
                coin5_com4.Charge(s_m5.ToString());
                //5元找完，再找1元
                are.WaitOne();
            }

            if (s_m5 > 0)
            {
                //5元钱不够，使用1元进行补
                s_m1 = s_m5 * 5 + m1;
            }
            if (s_m1 > 0)
            {
                Log.In("1元找零->" + s_m1);
                coin1_com3.Charge(s_m1.ToString());
            }
        }

        private static void M5_Change(object sender, ChargeItem item)
        {
            s_m5--;
            if (s_m5 == 0)
            {
                //5元找零结束
                are.Set();
            }
        }

        private static void M5_HopperEmtpy(object sender, EventArgs e)
        {
            //5元找零箱空
            are.Set();
        }

        private static void M1_Change(object sender, ChargeItem item)
        {
            s_m1--;
            if (s_m1 == 0)
            {
                //1元找零结束
            }
        }

        private static void M1_HopperEmtpy(object sender, EventArgs e)
        {
            //1元找零箱空
        }
    }
}
