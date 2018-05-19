using CCNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MoreMoney.Core.CashCore
{
    public class MoneyReceiver
    {
        private string port = "";
        private Iccnet objCCNET = null;
        private bool stop = false;
        private Thread thread = null;

        public event OnAcceptMoneyEventHandler OnAcceptMoney;
        public MoneyReceiver(string port)
        {
            this.port = port;
        }

        public bool Open(out string msg)
        {
            try
            {
                objCCNET = new Iccnet(port, Device.Bill_Validator);
                msg = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return false;
            }
        }

        public void Pool()
        {
            stop = false;
            thread = new Thread(Run);
            thread.Start();
        }

        private void Run()
        {
            var back = objCCNET.RunCommand(CCNETCommand.RESET);
            Print.Log("reset->" + back.Message);
            back = objCCNET.RunCommand(CCNETCommand.SET_SECURITY, new byte[3]);
            Print.Log("security->" + back.Message);
            //0d 1,5,10
            //
            back = objCCNET.RunCommand(CCNETCommand.ENABLE_BILL_TYPES, new byte[3] { 0, 0, 0xff });
            Print.Log("enable bill types->" + back.Message);
            while (!stop)
            {
                back = objCCNET.RunCommand(CCNETCommand.Poll);
                var item = back.ReceivedData;
                BVStatus bvs = (BVStatus)item[3];
                Print.Log("data len->" + item.Length);
                switch (bvs)
                {
                    case BVStatus.Idling:
                        {
                            Print.Log("Idling");
                        }
                        break;
                    case BVStatus.EscrowPosition:
                        {
                            Print.Log("EscrowPosition");
                        }
                        break;
                    case BVStatus.BillStacked:
                        {//接收纸币完成
                            BillType bt = (BillType)item[4];
                            Print.Log("接收完成纸币:" + bt);
                            //if (MoneyReceived != null)
                            //{
                            int money = 0;
                            switch (bt)
                            {
                                case BillType.RMB1: money = 1; break;
                                case BillType.RMB5: money = 5; break;
                                case BillType.RMB10: money = 10; break;
                                case BillType.RMB20: money = 20; break;
                                case BillType.RMB50: money = 50; break;
                                case BillType.RMB100: money = 100; break;
                            }
                            Print.Log("money->" + money);
                            //TimeSpan ts = DateTime.Now - LastRecDT;
                            //if (ts.TotalSeconds > 1)
                            //    MoneyReceived(money, bt);
                            //else
                            //{
                            //    Print.Log("一秒内又发生收币事件，丢弃");
                            //}
                            //LastRecDT = DateTime.Now;
                            //}
                            if (OnAcceptMoney != null)
                            {
                                OnAcceptMoney(this, money);
                            }
                        }
                        break;
                    case BVStatus.BillReturned:
                        {
                            BillType bt = (BillType)item[4];
                            Print.Log("退出纸币:" + bt);
                        }
                        break;
                    case BVStatus.rejecting:
                        {
                            RejectingCode rcode = (RejectingCode)item[4];
                            Print.Log("拒收纸币:" + rcode);
                        }
                        break;
                    case BVStatus.UnitDisabled:
                        {
                            Print.Log("验币器禁止");
                        }
                        break;
                }
                Thread.Sleep(100);
            }
        }

        private void Reset()
        {
            var back = objCCNET.RunCommand(CCNETCommand.RESET);
            Print.Log("reset->" + back.Message);
        }

        public void Stop()
        {
            stop = true;
            Reset();
        }

        public void Close()
        {
            stop = true;
        }
    }
}
