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
            var test = "202010".ToCharArray();
            var ok = test.Select(s => (byte)s).ToArray();
            byte b1, b2;
            Package.Lrc(ok, out b1, out b2);
            var c1 = (char)b1;
            var c2 = (char)b2;
            MainWindow x = new MainWindow();
            x.Show();
            base.OnStartup(e);
        }
    }
}
