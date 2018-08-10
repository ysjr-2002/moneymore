using MoneyCore.CoinOutEx;
using MoreMoney.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MoreMoney
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            byte[] data = { 0x7F, 0x00, 0x01, 0x07 };

            for (int i = 1; i < data.Length; i++)
            {
                byte b = data[i];
                CRC.Update_CRC(b);
            }

            var ll = CRC.CRCL;
            var hh = CRC.CRCH;
            var l = CRC.CRCL.ToString("X2");
            var h = CRC.CRCH.ToString("X2");

            MainWindow x = new MainWindow();
            x.Show();
            base.OnStartup(e);
        }
    }
}
