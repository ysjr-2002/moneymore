﻿using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MoreMoney.Core
{
    public class SerialComIC
    {
        private SerialPort port;
        private Thread thread;
        private bool bRun = false;
        private const byte etx_end2 = 0x0D;
        private const byte etx_end1 = 0x0A;
        public event OnReadCardEventHandle OnReadCardNo;

        public SerialComIC(string com)
        {
            port = new SerialPort(com, 9600, Parity.None, 8, StopBits.One);
        }

        public bool Open(out string msg)
        {
            try
            {
                port.Open();
                bRun = true;
                msg = string.Empty;
                thread = new Thread(Run);
                thread.Start();
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                bRun = false;
            }
            return bRun;
        }

        private void Run()
        {
            while (bRun)
            {
                var cardNo = ReadData();
                if (string.IsNullOrEmpty(cardNo))
                {
                    bRun = false;
                    break;
                }
                if (OnReadCardNo != null)
                {
                    OnReadCardNo(this, cardNo);
                }
            }
        }

        private string ReadData()
        {
            try
            {
                byte b = 0;
                List<byte> bytes = new List<byte>();
                while ((b = (byte)port.ReadByte()) > 0)
                {
                    if (b != etx_end2 && b != etx_end1)
                        bytes.Add(b);

                    if (b == etx_end1)
                        break;
                }
                var temp = bytes.ToArray().ToAscii();
                return temp;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public void Close()
        {
            bRun = false;
            if (port != null && port.IsOpen)
                port.Close();
        }
    }
}
