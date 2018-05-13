using Common;
using MoreMoney.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MoreMoney
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            test();
            Log.Set(log);
        }

        private void test()
        {
            var bytes = Encoding.ASCII.GetBytes("202005");
            byte l1, l2;
            Package.lrc(bytes, out l1, out l2);
            List<byte> temp = new List<byte>();
            temp.AddRange(bytes);
            temp.Add(l1);
            temp.Add(l2);
            var str = Encoding.ASCII.GetString(temp.ToArray());
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitPorts();
        }

        private void InitPorts()
        {
            var ports = Utility.SerialPorts();
            cmbPorts.ItemsSource = ports;
            if (ports.Count <= 1)
            {
                cmbPorts.SelectedIndex = 0;
            }
            else
            {
                cmbPorts.SelectedIndex = 1;
            }
        }

        private void btnOpenPort_Click(object sender, RoutedEventArgs e)
        {

        }

        Constrant constrant = new Constrant(new SerialCom());
        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            constrant.Reset();
        }

        private void btnReadId_Click(object sender, RoutedEventArgs e)
        {
            constrant.ReadId();
        }

        private void btnOpenCassette_Click(object sender, RoutedEventArgs e)
        {
            constrant.Open();
        }

        private void btnCloseCassette_Click(object sender, RoutedEventArgs e)
        {
            constrant.Close();
        }

        private void btnReadProgram_Click(object sender, RoutedEventArgs e)
        {
            constrant.ReadPROGRAM();
        }

        private void btnReadCounter_Click(object sender, RoutedEventArgs e)
        {
            constrant.Counter();
        }

        private void btnSelfTest_Click(object sender, RoutedEventArgs e)
        {
            constrant.SelfTest();
        }
    }
}
