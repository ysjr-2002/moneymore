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
        Constrant constrant = new Constrant(new SerialCom());
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            Log.Set(log);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitPorts();
        }

        private void InitPorts()
        {
            var ports = Utility.SerialPorts();
            cmbPorts.ItemsSource = ports;
            cmbICPorts.ItemsSource = ports;
            if (ports.Count <= 1)
            {
                cmbPorts.SelectedIndex = 0;
                cmbICPorts.SelectedIndex = 0;
            }
            else
            {
                cmbPorts.SelectedIndex = 1;
                cmbICPorts.SelectedIndex = 0;
            }
        }

        private void btnOpenPort_Click(object sender, RoutedEventArgs e)
        {

        }

        SerialComIC comIC = null;
        private void btnICOpenPort_Click(object sender, RoutedEventArgs e)
        {
            comIC = new SerialComIC(cmbICPorts.Text);
            var msg = "";
            var open = comIC.Open(out msg);
            if (!open)
            {
                Log.In(msg);
                return;
            }
            comIC.OnReadCardNo += (s, no) =>
            {
                Log.In(no);
            };
        }

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
            constrant.OpenCassette();
        }

        private void btnCloseCassette_Click(object sender, RoutedEventArgs e)
        {
            constrant.CloseCassette();
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

        private void btnMoveForeward_Click(object sender, RoutedEventArgs e)
        {
            var content = Convert.ToChar((((ComboBoxItem)cmbHn.SelectedItem).Content));
            var hn = (byte)content;
            var hn_hex = hn.ToString("x2");
            var hopenotes = txtMoney.Text.PadLeft(3, '0');
            constrant.MoveForward(hn, hopenotes);
        }
    }
}
