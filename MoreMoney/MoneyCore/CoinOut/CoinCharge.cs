﻿using MoneyCore.CoinCore;
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
        SerialPort serial = null;
        ChargeMoneyType chargeType;
        int READ_TIME_OUT = 500;
        bool enabletimeout = false;

        public CoinCharge(string port, ChargeMoneyType chargeType, bool enabletimeout)
        {
            try
            {
                this.serial = new SerialPort(port, 9600, Parity.None, 8, StopBits.One);
                this.chargeType = chargeType;
                this.enabletimeout = enabletimeout;
            }
            catch (Exception ex)
            {
                Log.Out("打开找零设备异常->" + ex.Message);
            }
        }

        public bool Open(out string msg)
        {
            try
            {
                serial.Open();
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
        /// 一次指令出多个币
        /// </summary>
        /// <param name="money">5</param>
        public void ChargeMore(string money)
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
            serial.Write(total, 0, total.Length);
        }

        /// <summary>
        /// 发送找零指令
        /// </summary>
        /// <param name="money"></param>
        public void Charge(string money)
        {
            var count = money.Tobyte();
            for (int i = 1; i <= count; i++)
            {
                CoinChargeAnswer answer = Charge();
                if (answer == CoinChargeAnswer.OK)
                {
                    Log.In("出->1");
                }
                if (answer == CoinChargeAnswer.HopperEmpty)
                {
                    Log.In("出->空");
                }
                if (answer == CoinChargeAnswer.TimeOut)
                {
                    Log.In("出->超时");
                }
            }
        }

        public CoinChargeAnswer Charge(char money = '1')
        {
            List<byte> data = new List<byte>();
            data.Add(0x02);
            data.Add(0x53);
            data.Add((byte)money);
            data.Add(0x03);
            var total = data.ToArray();
            CoinChargeAnswer answer = CoinChargeAnswer.TimeOut;
            try
            {
                serial.Write(total, 0, total.Length);
                serial.DiscardInBuffer();
                if (enabletimeout)
                {
                    serial.ReadTimeout = READ_TIME_OUT;
                }
                var b = (byte)serial.ReadByte();
                answer = (CoinChargeAnswer)b;
                Log.Out("charge->" + answer);
            }
            catch (Exception ex)
            {
                Log.Out(ex.Message);
            }
            return answer;
        }

        public void Close()
        {
            if (serial != null && serial.IsOpen)
            {
                serial.Close();
            }
        }
    }
}
