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
        static CoinAcceptor coinAcceptor3;
        static SerialCom cashOutCom;
        static Constrant constrant;
        static CoinCharge coin1Com;
        static CoinCharge coin5Com;

        /// <summary>
        /// 100纸币找零箱编号
        /// </summary>
        static readonly char hopper_100 = '1';
        /// <summary>
        /// 50纸币找零箱编号
        /// </summary>
        static readonly char hopper_50 = '2';

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
            IsOpenAndRead = false;
            try
            {
                //读卡器初始化
                cardCom = new SerialComIC(cardcom);
                cardCom.OnReadCardNo += (s, c) =>
                {
                    DllLog.In("回调2");
                    if (OnReadCardNo != null)
                    {
                        DllLog.In("回调3");
                        OnReadCardNo(s, c);
                    }
                };

                //纸币入
                cashInCom = new CashReceiver(cashIncom);
                open = cashInCom.Open(out msg);
                if (!open)
                {
                    return msg;
                }
                cashInCom.OnAcceptMoney += cashInCom_OnAcceptMoney;

                //硬币入
                var con = new ConnectionRs232
                {
                    PortName = coinIncom,
                    RemoveEcho = true
                };
                Dictionary<byte, CoinTypeInfo> coins;
                coins = CoinAcceptor.DefaultConfig;
                coinAcceptor3 = new CoinAcceptor(02, con, coins, null);
                coinAcceptor3.CoinAccepted += CoinAcceptorCoinAccepted;
                coinAcceptor3.ErrorMessageAccepted += CoinAcceptorErrorMessageAccepted;
                coinAcceptor3.Init();

                //纸币出
                cashOutCom = new SerialCom(cashOutcom);
                if (cashOutCom.Open(out msg) == false)
                {
                    return msg;
                }
                StatusCode.Init();
                constrant = new Constrant(cashOutCom);

                //硬币找零
                coin1Com = new CoinCharge(coin1Outcom, ChargeMoneyType.M1);
                coin5Com = new CoinCharge(coin5Outcom, ChargeMoneyType.M5);

                if (coin1Com.Open(out msg))
                {
                    return "1元找零串口打开失败";
                }
                if (coin5Com.Open(out msg))
                {
                    return "5元找零串口打开失败";
                }
                bInit = true;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return msg;
        }

        /// <summary>
        /// 释放
        /// </summary>
        public static void UnInit()
        {
            cardCom?.Close();
            cashInCom?.Close();
            coinAcceptor3?.Dispose();
            constrant.CloseCassette();
            cashOutCom?.Close();
            coin1Com?.Close();
            coin5Com?.Close();
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
            var open = cardCom?.Open(out msg);
            if (!open.GetValueOrDefault())
            {
                DllLog.In(msg);
                return false;
            }
            DllLog.In("开始读卡");
            return true;
        }

        /// <summary>
        /// 停止读卡
        /// </summary>
        public static void StopReadCard()
        {
            DllLog.In("停止读卡");
            cardCom?.Close();
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
                //纸币接收
            }
            if (coinAcceptor3 != null)
            {
                //硬币接收
                coinAcceptor3.StartPoll();
            }
        }

        /// <summary>
        /// 停止收钱
        /// </summary>
        public static void StopReceiveMoney()
        {
            cashInCom?.Stop();
            coinAcceptor3?.EndPoll();
        }

        /// <summary>
        /// 测试
        /// </summary>
        /// <param name="d"></param>
        public static void SetReceive(decimal d)
        {
            acceptMoney = d;
            FireReceiveMoneyEvent();
        }

        static void CoinAcceptorCoinAccepted(object sender, CoinAcceptorCoinEventArgs e)
        {
            DllLog.In(string.Format("name->{0} code->{1}", e.CoinName, e.CoinCode));
            if (e.CoinCode == 2)
            {
                //5角
                acceptMoney += 0.5M;
            }
            if (e.CoinCode == 3)
            {
                //1元
                acceptMoney += 1M;
            }
            FireReceiveMoneyEvent();
        }

        private static void cashInCom_OnAcceptMoney(object sender, int money)
        {
            acceptMoney += money;
            FireReceiveMoneyEvent();
        }

        private static void FireReceiveMoneyEvent()
        {
            if (OnAcceptMoneyWithAll != null)
            {
                OnAcceptMoneyWithAll(null, expectMoney, acceptMoney);
            }
        }

        static List<ChargeMoneyType> chargeItems = new List<ChargeMoneyType>();
        static bool IsOpenAndRead = false;
        /// <summary>
        /// 开始找零
        /// </summary>
        /// <param name="charge">找零金额</param>
        public static void StartCharge(decimal charge)
        {
            chargeItems.Clear();

            if (IsOpenAndRead == false)
            {
                constrant.OpenCassette();
                constrant.ReadId();
                IsOpenAndRead = true;
            }

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

            var unChargeMoney = Charge1(m1);
            if (OnChargeOver != null)
            {
                //触发找零结束事件
                Dictionary<ChargeMoneyType, int> dicts = new Dictionary<MoneyCore.ChargeMoneyType, int>();
                var count100 = chargeItems.Count(s => s == ChargeMoneyType.M100);
                dicts.Add(ChargeMoneyType.M100, count100);

                var count50 = chargeItems.Count(s => s == ChargeMoneyType.M50);
                dicts.Add(ChargeMoneyType.M50, count50);

                var count5 = chargeItems.Count(s => s == ChargeMoneyType.M5);
                dicts.Add(ChargeMoneyType.M5, count5);

                var count1 = chargeItems.Count(s => s == ChargeMoneyType.M1);
                dicts.Add(ChargeMoneyType.M1, count1);

                OnChargeOver(null, dicts, unChargeMoney);
            }
        }

        /// <summary>
        /// 100找零
        /// </summary>
        /// <param name="count"></param>
        /// <returns>50的张数</returns>
        public static int Charge100(int count)
        {
            if (count > 0)
            {
                var ok = false;
                for (int i = 1; i <= count; i++)
                {
                    ok = constrant.MoveForward((byte)hopper_100, "1".PadLeft(3, '0'));
                    if (ok)
                    {
                        s_m100--;
                        chargeItems.Add(ChargeMoneyType.M100);
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

        /// <summary>
        /// 50找零
        /// </summary>
        /// <param name="count"></param>
        /// <returns>5元的张数</returns>
        public static int Charge50(int count)
        {
            if (count > 0)
            {
                var ok = false;
                for (int i = 1; i <= count; i++)
                {
                    ok = constrant.MoveForward((byte)hopper_50, "1".PadLeft(3, '0'));
                    if (ok)
                    {
                        s_m50--;
                        chargeItems.Add(ChargeMoneyType.M50);
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

        /// <summary>
        /// 5元找零
        /// </summary>
        /// <param name="count"></param>
        /// <returns>1元的张数</returns>
        public static int Charge5(int count)
        {
            if (count > 0)
            {
                DllLog.In("5元找零->" + count);
                for (var i = 1; i <= count; i++)
                {
                    CoinChargeAnswer answer = coin5Com.Charge();
                    if (answer == CoinChargeAnswer.OK)
                    {
                        s_m5--;
                        chargeItems.Add(ChargeMoneyType.M5);
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

        private static int Charge1(int count)
        {
            if (count > 0)
            {
                DllLog.In("1元找零->" + count);
                for (int i = 1; i < count; i++)
                {
                    CoinChargeAnswer answer = coin1Com.Charge();
                    if (answer == CoinChargeAnswer.OK)
                    {
                        s_m1--;
                        chargeItems.Add(ChargeMoneyType.M1);
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
            return s_m1;
        }
    }
}
