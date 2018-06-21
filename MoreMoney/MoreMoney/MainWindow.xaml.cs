using MoreMoney.Core;
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
using MoreMoney;
using dk.CctalkLib.Devices;
using MoneyCore;
using MoneyCore.Cash;
using dk.CctalkLib.Connections;

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
            this.Closing += MainWindow_Closing;
            Core.Log.Set(log);
            MoneyCore.Log.SetLogIn(Core.Log.In);
            MoneyCore.Log.SetLogOut(Core.Log.Out);
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DeviceBus.UnInit();
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
            var ports = SerialPort.GetPortNames();
            cmbCashNotePorts.ItemsSource = ports;
            cmbCoinPorts.ItemsSource = ports;
            cmbCashChargePorts.ItemsSource = ports;
            cmbICPorts.ItemsSource = ports;
            cmbM1Ports.ItemsSource = ports;
            cmbM5Ports.ItemsSource = ports;

            if (ports.Length >= 1)
                cmbICPorts.SelectedIndex = 0;
            if (ports.Length >= 2)
                cmbCashNotePorts.SelectedIndex = 1;
            if (ports.Length >= 3)
                cmbCoinPorts.SelectedIndex = 2;
            if (ports.Length >= 4)
                cmbCashChargePorts.SelectedIndex = 3;
            if (ports.Length >= 4)
                cmbM1Ports.SelectedIndex = 4;
            if (ports.Length >= 5)
                cmbM5Ports.SelectedIndex = 5;
        }

        CashReceiver receiver = null;
        private void btnOpenPort_Click(object sender, RoutedEventArgs e)
        {
            var msg = "";
            receiver = new CashReceiver(cmbCashNotePorts.Text);
            var open = receiver.Open(out msg);
            if (!open)
            {
                MessageBox.Show(msg);
                return;
            }
            wpCashIn.IsEnabled = true;
        }

        private void btnICOpenPort_Click(object sender, RoutedEventArgs e)
        {
            var msg = DeviceBus.Init("COM1", "COM2", "COM3", "COM4", "COM5", "COM6");
            if (string.IsNullOrEmpty(msg) == false)
            {
                Core.Log.In(msg);
            }
            else
            {
                Core.Log.In("初始化成功");
                //btnBus.IsEnabled = true;
                //btnStopReceive.IsEnabled = true;
            }
            btnBus.IsEnabled = true;
            btnStopReceive.IsEnabled = true;
            DeviceBus.OnAcceptMoneyWithAll += (s, currentMoney, total) =>
            {
                //m 应收
                //t 实收
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    txtCurrentMoney.Text = currentMoney.ToString();
                    txtTotalMoney.Text = total.ToString();
                }));
            };

            DeviceBus.OnChargeOver += (x, agg, unChargeMoney) =>
            {
                if (unChargeMoney == 0)
                {
                    var m100 = agg[ChargeMoneyType.M100];
                    Core.Log.In("100找零->" + m100);

                    var m50 = agg[ChargeMoneyType.M50];
                    Core.Log.In("50找零->" + m50);

                    var m5 = agg[ChargeMoneyType.M5];
                    Core.Log.In("5找零->" + m5);

                    var m1 = agg[ChargeMoneyType.M1];
                    Core.Log.In("1找零->" + m1);
                }
                else
                {
                    Core.Log.In("未找零金额->" + unChargeMoney);
                }
            };

            DeviceBus.OnReadCardNo += (s, no) =>
            {
                Core.Log.In(no);
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
            receiver.Pool();
        }

        private void btnCoinReset_click(object sender, RoutedEventArgs e)
        {
            _coinCounter = 0;
            _coinAcceptor._rawDev.CmdReset();
        }

        private void btnReadBuffer_click(object sender, RoutedEventArgs e)
        {
            _coinAcceptor._rawDev.CmdReadEventBuffer();
        }

        private void btnStopPool_click(object sender, RoutedEventArgs e)
        {
            receiver.Stop();
        }

        CoinAcceptor _coinAcceptor = null;
        decimal _coinCounter = 0;
        private void btnCoinOpenPort_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(cmbCoinPorts.Text))
            {
                Core.Log.Out("请选择串口");
                return;
            }

            var con = new ConnectionRs232
            {
                PortName = cmbCoinPorts.Text,
                RemoveEcho = true
            };

            Dictionary<byte, CoinTypeInfo> coins;
            //if (!CoinAcceptor.TryParseConfigWord(configWord.Text, out coins))
            //{
            //    MessageBox.Show("Wrong config word, using defaults");
            //    coins = CoinAcceptor.DefaultConfig;
            //    configWord.Text = CoinAcceptor.ConfigWord(CoinAcceptor.DefaultConfig);
            //}

            coins = CoinAcceptor.DefaultConfig;
            _coinAcceptor = new CoinAcceptor(02, con, coins, null);

            _coinAcceptor.CoinAccepted += CoinAcceptorCoinAccepted;
            _coinAcceptor.ErrorMessageAccepted += CoinAcceptorErrorMessageAccepted;

            _coinAcceptor.Init();

            if (_coinAcceptor.IsInitialized)
            {
                wpCoinIn.IsEnabled = true;
            }
        }

        void CoinAcceptorCoinAccepted(object sender, CoinAcceptorCoinEventArgs e)
        {
            if (Application.Current.Dispatcher.CheckAccess() == false)
            {
                Application.Current.Dispatcher.Invoke((EventHandler<CoinAcceptorCoinEventArgs>)CoinAcceptorCoinAccepted, sender, e);
                return;
            }
            _coinCounter += e.CoinValue;
            //Core.Log.In(string.Format("Coin accepted: {0} ({1:X2}), path {3}. Now accepted: {2:C}", e.CoinName, e.CoinCode, _coinCounter, e.RoutePath));
        }

        void CoinAcceptorErrorMessageAccepted(object sender, CoinAcceptorErrorEventArgs e)
        {
            if (Application.Current.Dispatcher.CheckAccess() == false)
            {
                Application.Current.Dispatcher.Invoke((EventHandler<CoinAcceptorErrorEventArgs>)CoinAcceptorErrorMessageAccepted, sender, e);
                return;
            }

            Core.Log.In(string.Format("Coin acceptor error: {0} ({1}, {2:X2})", e.ErrorMessage, e.Error, (Byte)e.Error));
        }

        private void btnChargeTest_click(object sender, RoutedEventArgs e)
        {
            var all = txtNeed.Text.Toint();
            int m1, m5, m50, m100;
            Charge.GetCount(all, out m1, out m5, out m50, out m100);
            Core.Log.In("1元->" + m1);
            Core.Log.In("5元->" + m5);
            Core.Log.In("50元->" + m50);
            Core.Log.In("100元->" + m100);
            Core.Log.In("all->" + (m1 + (m5 * 5) + (m50 * 50) + (m100 * 100)));
        }

        CoinCharge c1;
        private void btnM1Open_click(object sender, RoutedEventArgs e)
        {
            c1 = new CoinCharge(cmbM1Ports.Text, ChargeMoneyType.M1, false);
            var msg = "";
            var open = c1.Open(out msg);
            if (!open)
            {
                Core.Log.In(msg);
                return;
            }
            btnM1Charge.IsEnabled = true;
        }

        private void btnM1ChargeStart_click(object sender, RoutedEventArgs e)
        {
            c1.Charge(txtM1Count.Text);
        }

        CoinCharge c5;
        private void btnM5Open_click(object sender, RoutedEventArgs e)
        {
            c5 = new CoinCharge(cmbM5Ports.Text, ChargeMoneyType.M5, false);
            var msg = "";
            var open = c5.Open(out msg);
            if (!open)
            {
                Core.Log.In(msg);
                return;
            }
            btnM5Charge.IsEnabled = true;
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
                Core.Log.In(msg);
                return;
            }
            StatusCode.Init();
            wrapPanel.IsEnabled = true;
            wp1.IsEnabled = true;
            constrant = new Constrant(com);
        }

        private void ckbInhibit_click(object sender, RoutedEventArgs e)
        {
            var con = _coinAcceptor.Connection;
            var c = new GenericCctalkDevice
            {
                Connection = (ConnectionRs232)con,
                Address = 0
            };
            c.CmdSetMasterInhibitStatus(ckbInhibit.IsChecked.GetValueOrDefault());
        }

        private void btnAllTest_click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtCurrentMoney.Text = "0";
                txtTotalMoney.Text = "0";
                DeviceBus.StartReceiveMoney(txtNeed.Text.Todecimal());
            }
            catch (Exception ex)
            {
                Core.Log.Out("异常1->" + ex.Message);
            }
        }

        private void btnReceiveMoney_Click(object sender, RoutedEventArgs e)
        {
            //DeviceBus.SetReceive(txtyishou.Text.Todecimal());
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            DeviceBus.StopReceiveMoney();
        }

        private void btnStartCharge_Click(object sender, RoutedEventArgs e)
        {
            //var charge = txtyishou.Text.Todecimal();
            var charge = cmbcharge.Text.Todecimal();
            DeviceBus.StartCharge(charge);
        }

        private void btnStartReadCard_Click(object sender, RoutedEventArgs e)
        {
            DeviceBus.StartReadCard();
        }

        private void btnCoinPool_click(object sender, RoutedEventArgs e)
        {
            _coinAcceptor?.StartPoll();
            Core.Log.Out("start pool");
        }

        private void btnCoinStopPool_click(object sender, RoutedEventArgs e)
        {
            _coinAcceptor?.EndPoll();
            Core.Log.Out("end pool");
        }

        private void btnCashChargClosePort_Click(object sender, RoutedEventArgs e)
        {
            com?.Close();
        }

        private void btnStopReadCard_Click(object sender, RoutedEventArgs e)
        {
            DeviceBus.StopReadCard();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            log.Document.Blocks.Clear();
        }
    }
}
