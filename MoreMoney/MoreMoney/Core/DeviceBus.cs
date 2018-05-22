using MoreMoney.Core.CashCore;
using MoreMoney.Core.CoinCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MoreMoney.Core
{
    /// <summary>
    /// 一个读卡器
    /// 一个纸币接收机 
    /// 一个硬币接收机
    /// 一个一元硬币找零机
    /// 一个五元硬币找零机
    /// 一个纸币找零机
    /// </summary>
    public static class DeviceBus
    {
        /// <summary>
        /// 读取到卡号
        /// </summary>
        public static event OnReadCardEventHandle OnReadCardNo;
        public static event OnAcceptMoneyWithAllEventHandler OnAcceptMoneyWithAll;
        public static event OnChargeEventHandler OnChargeOver;

        public static event OnChargingEventHandler OnCharging;
        public static event OnHopperEmptyEventHandler OnHopperEmpty;

        static SerialComIC com1;
        static CashReceiver com2;
        static CoinCharge coin1_com3;
        static CoinCharge coin5_com4;

        //应收
        static int expectMoney = 0;
        //实收
        static int acceptMoney = 0;

        static int s_m1, s_m5, s_m50, s_m100;
        static AutoResetEvent are = new AutoResetEvent(false);

        static bool bInit = false;

        public static void Init()
        {
            if (bInit)
                return;

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

            //com2 = new CashReceiver("COM1");
            //open = com2.Open(out msg);
            //if (!open)
            //{
            //    return;
            //}
            //com2.OnAcceptMoney += Com2_OnAcceptMoney;

            //coin1_com3 = new CoinChanger("COM3", ChargeMoneyType.M1);
            //coin5_com4 = new CoinChanger("COM4", ChargeMoneyType.M5);

            //coin1_com3.OnCharging += M1_Change;
            //coin5_com4.OnCharging += M5_Change;

            //coin1_com3.OnHopperEmpty += M1_HopperEmtpy;
            //coin5_com4.OnHopperEmpty += M5_HopperEmtpy;

            //coin1_com3.Open(out msg);
            //coin5_com4.Open(out msg);
            bInit = true;
        }

        public static bool ReadyCardRead()
        {
            var msg = "";
            var open = com1.Open(out msg);
            if (!open)
            {
                Log.In(msg);
                return false;
            }
            return true;
        }

        public static void CloseCardRead()
        {
            com1.Close();
        }

        public static void ReadPool(int expectMoney)
        {
            if (com2.Pool())
            {
                acceptMoney = 0;
                DeviceBus.expectMoney = expectMoney;
            }
        }

        public static void StopPool()
        {
            com2.Stop();
        }

        private static void Com2_OnAcceptMoney(object sender, int money)
        {
            acceptMoney += money;
            if (OnAcceptMoneyWithAll != null)
            {
                OnAcceptMoneyWithAll(sender, money, acceptMoney);
            }
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
                    OnChargeOver(null, new List<ChargeMoneyType>());
                }
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
                //5元找完，再找1元
                Log.In("5元找零->" + s_m5);
                for (var i = 1; i <= m5; i++)
                {
                    CoinChargeAnswer answer = coin5_com4.Charge();
                    if (answer == CoinChargeAnswer.OK)
                    {
                        s_m5--;
                        if (OnCharging != null)
                        {
                            OnCharging(null, ChargeMoneyType.M5);
                        }
                    }
                    else
                    {
                        if (OnHopperEmpty != null)
                        {
                            OnHopperEmpty(null, ChargeMoneyType.M5);
                        }
                        break;
                    }
                }
            }

            if (s_m5 > 0)
            {
                //5元钱不够，使用1元进行补
                s_m1 = s_m5 * 5 + m1;
            }
            if (s_m1 > 0)
            {
                Log.In("1元找零->" + s_m1);
                for (int i = 1; i < s_m1; i++)
                {
                    CoinChargeAnswer answer = coin1_com3.Charge();
                    if (answer == CoinChargeAnswer.OK)
                    {
                        s_m1--;
                        if (OnCharging != null)
                        {
                            OnCharging(null, ChargeMoneyType.M1);
                        }
                    }
                    else
                    {
                        if (OnHopperEmpty != null)
                        {
                            OnHopperEmpty(null, ChargeMoneyType.M1);
                        }
                        break;
                    }
                }
            }
        }

        //private static void M5_Change(object sender, ChargeItem item)
        //{
        //    s_m5--;
        //    if (s_m5 == 0)
        //    {
        //        //5元找零结束
        //        are.Set();
        //    }
        //}

        //private static void M5_HopperEmtpy(object sender, EventArgs e)
        //{
        //    //5元找零箱空
        //    are.Set();
        //}

        //private static void M1_Change(object sender, ChargeItem item)
        //{
        //    s_m1--;
        //    if (s_m1 == 0)
        //    {
        //        //1元找零结束
        //    }
        //}

        //private static void M1_HopperEmtpy(object sender, EventArgs e)
        //{
        //    //1元找零箱空
        //}
    }
}
