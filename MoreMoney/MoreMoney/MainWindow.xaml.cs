using Common;
using MoreMoney.Core;
using MoreMoney.Core.CashCore;
using System;
using System.Collections.Generic;
using System.IO.Ports;
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
        Constrant constrant = null;
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            Log.Set(log);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitPorts();

            DataItems.Init();
            cmbReadDataItem.ItemsSource = DataItems.Items;
            cmbReadDataItem.DisplayMemberPath = "Text";
            cmbReadDataItem.SelectedValuePath = "Code";
            cmbReadDataItem.SelectedIndex = 0;
        }

        private void InitPorts()
        {
            var ports = Utility.SerialPorts();
            cmbCashNotePorts.ItemsSource = ports;
            cmbCoinPorts.ItemsSource = ports;
            cmbCashChargePorts.ItemsSource = ports;
            cmbICPorts.ItemsSource = ports;
            cmbM1Ports.ItemsSource = ports;
            cmbM5Ports.ItemsSource = ports;

            cmbCashNotePorts.SelectedIndex = 1;
            cmbICPorts.SelectedIndex = 1;
            cmbCashChargePorts.SelectedIndex = 1;
            cmbCoinPorts.SelectedIndex = 1;
            cmbM1Ports.SelectedIndex = 1;
            cmbM5Ports.SelectedIndex = 1;
        }

        private void btnOpenPort_Click(object sender, RoutedEventArgs e)
        {
            DeviceBus.Init();
            DeviceBus.OnAcceptMoneyWithAll += (s, m, total) =>
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    txtHave.Text = total.ToString();
                    if (total > m)
                    {
                        txtCharge.Text = (total - m).ToString();
                    }
                }));
            };

            DeviceBus.OnReadCardNo += (s, no) =>
            {
                Log.In(no);
            };
        }

        private void btnICOpenPort_Click(object sender, RoutedEventArgs e)
        {
            DeviceBus.Init();
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

        private void btnCheckDelivered_Click(object sender, RoutedEventArgs e)
        {
            constrant.CheckDelivered();
        }

        private void btnCleartransport_Click(object sender, RoutedEventArgs e)
        {
            constrant.ClearTransport();
        }

        private void btnReadData_Click(object sender, RoutedEventArgs e)
        {
            var dataitem = cmbReadDataItem.SelectedItem as DataItem;
            var code = dataitem.code;
            constrant.ReadData(code);
        }

        private void btnPool_click(object sender, RoutedEventArgs e)
        {
            var money = txtNeed.Text.ToInt32();
            txtNeed.Text = money.ToString();
            txtHave.Text = "0";
            DeviceBus.ReadPool(money);
        }

        private void btnStopPool_click(object sender, RoutedEventArgs e)
        {
            DeviceBus.StopPool();
        }

        private void btnCoinOpenPort_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnChargeTest_click(object sender, RoutedEventArgs e)
        {
            var all = txtNeed.Text.ToInt32();
            int m1, m5, m50, m100;
            Charge.GetCount(all, out m1, out m5, out m50, out m100);
            Log.In("1元->" + m1);
            Log.In("5元->" + m5);
            Log.In("50元->" + m50);
            Log.In("100元->" + m100);
            Log.In("all->" + (m1 + (m5 * 5) + (m50 * 50) + (m100 * 100)));
        }

        CoinCharge c1;
        private void btnM1Open_click(object sender, RoutedEventArgs e)
        {
            c1 = new CoinCharge(cmbM1Ports.Text, ChargeMoneyType.M1);
            var msg = "";
            var open = c1.Open(out msg);
            if (!open)
            {
                Log.In(msg);
                return;
            }
        }

        private void btnM1ChargeStart_click(object sender, RoutedEventArgs e)
        {
            c1.Charge(txtM1Count.Text);
        }

        CoinCharge c5;
        private void btnM5Open_click(object sender, RoutedEventArgs e)
        {
            c5 = new CoinCharge(cmbM5Ports.Text, ChargeMoneyType.M5);
            var msg = "";
            var open = c5.Open(out msg);
            if (!open)
            {
                Log.In(msg);
                return;
            }
        }

        private void btnM5ChargeStart_click(object sender, RoutedEventArgs e)
        {
            c5.Charge(txtM5Count.Text);
        }

        SerialCom com = null;
        private void btnCashChargOpenPort_Click(object sender, RoutedEventArgs e)
        {
            var msg = "";
            com = new SerialCom(cmbCashChargePorts.Text);
            if (com.Open(out msg) == false)
            {
                Log.In(msg);
                return;
            }
            wrapPanel.IsEnabled = true;
            constrant = new Constrant(com);
        }
    }
}
