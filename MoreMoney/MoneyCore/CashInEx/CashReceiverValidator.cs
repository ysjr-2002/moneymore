using dk.CctalkLib.Checksumms;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;

namespace MoneyCore.CashInEx
{
    /// <summary>
    /// 新纸币接收器
    /// </summary>
    public class CashReceiverValidator
    {
        SerialPort port = null;
        Thread thread = null;
        bool bRunning = false;
        //倍率
        static decimal ratio = 0m;
        private static Dictionary<int, decimal> channels = new Dictionary<int, decimal>();

        public event OnAcceptMoneyEventHandler OnAcceptMoney;
        public bool Open(string com)
        {
            try
            {
                port = new SerialPort(com, 9600, Parity.None, 8, StopBits.Two);
                port.Open();
                Log.Out("新纸币OK");
                return true;
            }
            catch (Exception ex)
            {
                Log.Out("新纸币接收->" + ex.Message);
                return false;
            }
        }

        public bool Init()
        {
            byte[] data;
            byte[] responsedata;

            //连接OK
            data = Message.getSendBytes(CCommands.SSP_CMD_SYNC);
            responsedata = send(data);
            if (responsedata == null || responsedata[0] != CCommands.SSP_RESPONSE_CMD_OK)
                return false;

            //读取纸币器通道配置
            data = Message.getSendBytes(CCommands.SSP_CMD_SETUP_REQUEST);
            responsedata = send(data);
            if (responsedata == null || responsedata[0] != CCommands.SSP_RESPONSE_CMD_OK)
                return false;

            byte unittype = responsedata[1]; // 00 纸币器类型
            Log.In((unittype == 0) ? "纸币器类型" : "其他类型" + unittype);
            var version = Encoding.ASCII.GetString(responsedata, 2, 4);
            Log.In("软件版本=" + version);
            var countrycode = Encoding.ASCII.GetString(responsedata, 6, 3);
            Log.In("国家代码=" + countrycode);

            var beilv = responsedata[9] + " " + responsedata[10] + " " + responsedata[11];
            Log.In("倍率=" + countrycode);
            ratio = responsedata[9] + responsedata[10] + responsedata[11];

            var maxchannel = responsedata[12];
            Log.In("最大通道数=" + maxchannel);

            for (int i = 1; i <= maxchannel; i++)
            {
                channels.Add(i, responsedata[12 + i]);
                Log.In(string.Format("通道{0},金额{1}", i, responsedata[12 + i]));
            }

            //设置允许识别哪几种纸币
            data = Message.getOpenChannelBytes();
            responsedata = send(data);
            if (responsedata == null || responsedata[0] != CCommands.SSP_RESPONSE_CMD_OK)
                return false;

            //允许纸币器识别纸币
            data = Message.getSendBytes(CCommands.SSP_CMD_ENABLE);
            responsedata = send(data);
            if (responsedata == null || responsedata[0] != CCommands.SSP_RESPONSE_CMD_OK)
                return false;

            //打开纸币器面板显示灯
            data = Message.getSendBytes(CCommands.SSP_CMD_DISPLAY_OFF);
            responsedata = send(data);
            if (responsedata == null || responsedata[0] != CCommands.SSP_RESPONSE_CMD_OK)
                return false;

            return StartPool();
        }

        private bool StartPool()
        {
            if (bRunning == true)
            {
                return true;
            }
            thread = new Thread(Running);
            thread.Start();
            bRunning = true;
            return bRunning;
        }

        private void Running()
        {
            byte[] data = null;
            while (bRunning)
            {
                data = Message.getSendBytes(CCommands.SSP_CMD_POLL);
                var responseData = send(data);
                if (responseData == null)
                    continue;

                //responseData[0] == 0xF0;
                switch (responseData[1])
                {
                    case CCommands.SSP_POLL_CREDIT: //纸币被接收，（特殊情况下EE不在OK后，详细见附件5说明）
                        if (responseData.Length == 3)
                        {
                            Log.In("接收到纸币，未压入钞箱");
                        }
                        else if (responseData.Length == 5)
                        {
                            if (responseData[4] == CCommands.SSP_POLL_STACKED)
                            {
                                var channel = responseData[2];
                                var money = getMoney(channel);
                                Log.In(string.Format("压入钞箱  通道:{0} 金额:{1}", channel, money));
                                if (OnAcceptMoney != null)
                                {
                                    OnAcceptMoney(this, (int)money);
                                }
                            }
                        }
                        break;
                    case CCommands.SSP_POLL_RESET: //纸币器正在复位
                        Log.In("纸币器正在复位");
                        break;
                    case CCommands.SSP_RESPONSE_CMD_OK: //命令执行OK
                        Log.In("命令执行OK");
                        break;
                    case CCommands.SSP_POLL_CASHBOX_REMOVED: //钱箱被取走
                        Log.In("钱箱被取走");
                        break;
                    case CCommands.SSP_POLL_STACKING: //压币中...
                        Log.In("压币中...");
                        break;
                    case CCommands.SSP_POLL_STACKED: //纸币压钞结束
                        Log.In("纸币压钞结束");
                        break;
                }
                Thread.Sleep(150);
            }
        }

        public bool StopPool()
        {
            bRunning = false;

            //关灯
            var data = Message.getSendBytes(CCommands.SSP_CMD_DISPLAY_OFF);
            var responsedata = send(data);
            if (responsedata == null || responsedata[0] != CCommands.SSP_RESPONSE_CMD_OK)
                return false;

            //禁用
            data = Message.getSendBytes(CCommands.SSP_CMD_DISABLE);
            responsedata = send(data);
            if (responsedata == null || responsedata[0] != CCommands.SSP_RESPONSE_CMD_OK)
                return false;

            return true;
        }

        private byte[] send(byte[] data)
        {
            if (port.IsOpen)
            {
                port.Write(data, 0, data.Length);
                return read();
            }
            else
                return null;
        }

        private byte[] read()
        {
            port.ReadTimeout = 500;
            try
            {
                var b1 = (byte)port.ReadByte();
                var b2 = (byte)port.ReadByte();
                var b3 = (byte)port.ReadByte();
                var len = b3;

                List<byte> list = new List<byte>();
                list.Add(b1);
                list.Add(b2);
                list.Add(b3);
                while (len > 0)
                {
                    list.Add((byte)port.ReadByte());
                    len--;
                }

                //计算校验位
                CRC.Get_CRC(list.ToArray());
                var mycrcl = CRC.CRCL;
                var mycrch = CRC.CRCH;

                var crcl = port.ReadByte();
                var crch = port.ReadByte();

                if (mycrcl == crcl && mycrch == crch)
                {
                    //校验位相同
                    var responseData = new byte[b3];
                    list.CopyTo(3, responseData, 0, responseData.Length);
                    return responseData;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        static decimal getMoney(int channel)
        {
            return channels[channel] * ratio;
        }
    }
}
