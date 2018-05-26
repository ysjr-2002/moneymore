using dk.CctalkLib.Connections;
using dk.CctalkLib.Devices;
using MoneyCore.Cash;
using MoneyCore.CoinCore;
using MoreMoney.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MoneyCore
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

        static SerialComIC cardCom;
        static CashReceiver cashInCom;
        static CoinAcceptor _coinAcceptor3;
        static SerialCom cashOutCom;
        static Constrant constrant;
        static CoinCharge coin1_com5;
        static CoinCharge coin5_com6;

        //应收
        static decimal expectMoney = 0;
        //实收
        static decimal acceptMoney = 0;
        static int s_m1, s_m5, s_m50, s_m100;
        static bool bInit = false;

        public static string Init(string cardcom, string cashIncom, string coinIncom, string cashOutcom, string coin1Outcom, string coin5Outcom)
        {
            if (bInit)
                return "";

            var msg = "";
            var open = false;
            //com1 = new SerialComIC(cardcom);
            //com1.OnReadCardNo += (s, c) =>
            //{
            //    if (OnReadCardNo != null)
            //    {
            //        OnReadCardNo(s, c);
            //    }
            //};

            ////纸币入
            //com2 = new CashReceiver(cashIncom);
            //open = com2.Open(out msg);
            //if (!open)
            //{
            //    return "";
            //}
            //com2.OnAcceptMoney += Com2_OnAcceptMoney;

            //硬币入
            var con = new ConnectionRs232
            {
                PortName = coinIncom,
                RemoveEcho = true
            };

            Dictionary<byte, CoinTypeInfo> coins;
            coins = CoinAcceptor.DefaultConfig;
            _coinAcceptor3 = new CoinAcceptor(02, con, coins, null);
            _coinAcceptor3.CoinAccepted += CoinAcceptorCoinAccepted;
            _coinAcceptor3.ErrorMessageAccepted += CoinAcceptorErrorMessageAccepted;
            _coinAcceptor3.Init();

            //纸币出
            cashOutCom = new SerialCom(cashOutcom);
            if (cashOutCom.Open(out msg) == false)
            {
                return msg;
            }
            StatusCode.Init();
            constrant = new Constrant(cashOutCom);

            //coin1_com5 = new CoinCharge(coin1Outcom, ChargeMoneyType.M1);
            //coin5_com6 = new CoinCharge("coin5Outcom", ChargeMoneyType.M5);

            //if (coin1_com5.Open(out msg))
            //{
            //    return "1元找零串口打开失败";
            //}
            //if (coin5_com6.Open(out msg))
            //{
            //    return "5元找零串口打开失败";
            //}
            bInit = true;
            return string.Empty;
        }

        static void CoinAcceptorErrorMessageAccepted(object sender, CoinAcceptorErrorEventArgs e)
        {
            //if (Application.Current.Dispatcher.CheckAccess() == false)
            //{
            //    Application.Current.Dispatcher.Invoke((EventHandler<CoinAcceptorErrorEventArgs>)CoinAcceptorErrorMessageAccepted, sender, e);
            //    return;
            //}
            //Log.In(String.Format("Coin acceptor error: {0} ({1}, {2:X2})", e.ErrorMessage, e.Error, (Byte)e.Error));
        }

        /// <summary>
        /// 开始读卡
        /// </summary>
        /// <returns></returns>
        public static bool StartReadCard()
        {
            var msg = "";
            var open = cardCom.Open(out msg);
            if (!open)
            {
                DllLog.In(msg);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 停止读卡
        /// </summary>
        public static void StopReadCard()
        {
            cardCom.Close();
        }

        /// <summary>
        /// 开始收钱
        /// </summary>
        /// <param name="expectMoney"></param>
        public static void StartReceiveMoney(decimal expectMoney)
        {
            acceptMoney = 0;
            DeviceBus.expectMoney = expectMoney;
            if (cashInCom != null && cashInCom.Pool())
            {
            }
            if (_coinAcceptor3 != null)
            {
                _coinAcceptor3.StartPoll();
            }
        }

        /// <summary>
        /// 停止收钱
        /// </summary>
        public static void StopReceiveMoney()
        {
            cashInCom?.Stop();
            _coinAcceptor3?.EndPoll();
        }

        /// <summary>
        /// 测试
        /// </summary>
        /// <param name="d"></param>
        public static void SetReceive(decimal d)
        {
            acceptMoney += d;
            IsNeedCharge();
        }

        static void CoinAcceptorCoinAccepted(object sender, CoinAcceptorCoinEventArgs e)
        {
            DllLog.In(string.Format("name->{0} code->{1}", e.CoinName, e.CoinCode));
            if (e.CoinCode == 2)
            {
                //5角
                acceptMoney += (decimal)0.5;
            }
            if (e.CoinCode == 3)
            {
                //1元
                acceptMoney += (decimal)1;
            }
            IsNeedCharge();
        }

        private static void Com2_OnAcceptMoney(object sender, int money)
        {
            acceptMoney += money;
            IsNeedCharge();
        }

        private static void IsNeedCharge()
        {
            if (OnAcceptMoneyWithAll != null)
            {
                OnAcceptMoneyWithAll(null, expectMoney, acceptMoney);
            }
            if (acceptMoney > expectMoney)
            {
                //缴费多，需要找零
                cashInCom?.Stop();
                int charge = (int)(acceptMoney - expectMoney);
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

            var remaing50 = Charge100(m100);
            m50 += remaing50;
            s_m50 = m50;

            var remaing5 = Charge50(m50);
            m5 += remaing5;
            s_m5 = m5;

            var remaing1 = Charge5(m5);
            m1 += remaing1;
            s_m1 = m1;

            Charge1(m1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m100"></param>
        /// <returns></returns>
        public static int Charge100(int m100)
        {
            if (m100 > 0)
            {
                var ok = false;
                for (int i = 1; i <= m100; i++)
                {
                    ok = constrant.MoveForward((byte)'1', "1".PadLeft(3, '0'));
                    if (ok)
                    {
                        s_m100--;
                        if (OnCharging != null)
                        {
                            OnCharging(null, ChargeMoneyType.M100);
                        }
                    }
                    else
                    {
                        //找钱失败
                        if (OnHopperEmpty != null)
                        {
                            OnHopperEmpty(null, ChargeMoneyType.M100);
                        }
                        break;
                    }
                }
                DllLog.In("100找零结束");
            }
            if (s_m100 > 0)
            {
                //100是50的2倍
                var temp = s_m100 * 2;
                return temp;
            }
            return 0;
        }

        public static int Charge50(int m50)
        {
            if (m50 > 0)
            {
                var ok = false;
                for (int i = 1; i <= m50; i++)
                {
                    ok = constrant.MoveForward((byte)'2', "1".PadLeft(3, '0'));
                    if (ok)
                    {
                        s_m50--;
                        if (OnCharging != null)
                        {
                            OnCharging(null, ChargeMoneyType.M50);
                        }
                    }
                    else
                    {
                        if (OnHopperEmpty != null)
                        {
                            OnHopperEmpty(null, ChargeMoneyType.M50);
                        }
                        break;
                    }
                }
                DllLog.In("50找零结束");
            }
            if (s_m50 > 0)
            {
                //50是5的10倍
                var temp = s_m50 * 10;
                return temp;
            }
            return 0;
        }

        public static int Charge5(int m5)
        {
            if (m5 > 0)
            {
                DllLog.In("5元找零->" + s_m5);
                for (var i = 1; i <= m5; i++)
                {
                    CoinChargeAnswer answer = coin5_com6.Charge();
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
                var temp = s_m5 * 5;
                return temp;
            }
            return 0;
        }

        private static int Charge1(int m1)
        {
            if (m1 > 0)
            {
                DllLog.In("1元找零->" + s_m1);
                for (int i = 1; i < s_m1; i++)
                {
                    CoinChargeAnswer answer = coin1_com5.Charge();
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
            return 0;
        }
    }
}
