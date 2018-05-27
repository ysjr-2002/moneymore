﻿using MoreMoney.Core;
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
            Log.Set(log);
            DllLog.SetLogIn(Log.In);
            DllLog.SetLogOut(Log.Out);
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

            cmbCashNotePorts.SelectedIndex = 1;
            cmbICPorts.SelectedIndex = 1;
            cmbCashChargePorts.SelectedIndex = 1;
            cmbCoinPorts.SelectedIndex = 1;
            cmbM1Ports.SelectedIndex = 1;
            cmbM5Ports.SelectedIndex = 1;
        }

        private void btnOpenPort_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnICOpenPort_Click(object sender, RoutedEventArgs e)
        {
            DeviceBus.Init("COM6", "COM2", "COM6", "COM12", "COM5", "COM6");
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

            DeviceBus.OnChargeOver += (x, y, unChargeMoney) =>
            {
                if (unChargeMoney == 0)
                {
                    var m100 = y[ChargeMoneyType.M100];
                    Log.In("100找零->" + m100);

                    var m50 = y[ChargeMoneyType.M50];
                    Log.In("50找零->" + m50);

                    var m5 = y[ChargeMoneyType.M5];
                    Log.In("5找零->" + m5);

                    var m1 = y[ChargeMoneyType.M1];
                    Log.In("1找零->" + m1);
                }
                else
                {
                    Log.In("未找零金额->" + unChargeMoney);
                }
            };

            DeviceBus.OnReadCardNo += (s, no) =>
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
            _coinAcceptor.StartPoll();
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
            _coinAcceptor.EndPoll();
        }

        CoinAcceptor _coinAcceptor = null;
        decimal _coinCounter = 0;
        private void btnCoinOpenPort_Click(object sender, RoutedEventArgs e)
        {
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
        }

        void CoinAcceptorCoinAccepted(object sender, CoinAcceptorCoinEventArgs e)
        {
            if (Application.Current.Dispatcher.CheckAccess() == false)
            {
                Application.Current.Dispatcher.Invoke((EventHandler<CoinAcceptorCoinEventArgs>)CoinAcceptorCoinAccepted, sender, e);
                return;
            }
            _coinCounter += e.CoinValue;
            Log.In(String.Format("Coin accepted: {0} ({1:X2}), path {3}. Now accepted: {2:C}", e.CoinName, e.CoinCode, _coinCounter, e.RoutePath));

        }

        void CoinAcceptorErrorMessageAccepted(object sender, CoinAcceptorErrorEventArgs e)
        {
            if (Application.Current.Dispatcher.CheckAccess() == false)
            {
                Application.Current.Dispatcher.Invoke((EventHandler<CoinAcceptorErrorEventArgs>)CoinAcceptorErrorMessageAccepted, sender, e);
                return;
            }

            Log.In(String.Format("Coin acceptor error: {0} ({1}, {2:X2})", e.ErrorMessage, e.Error, (Byte)e.Error));
        }

        private void btnChargeTest_click(object sender, RoutedEventArgs e)
        {
            var all = txtNeed.Text.Toint();
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
            StatusCode.Init();
            wrapPanel.IsEnabled = true;
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
            txtHave.Text = "0";
            txtCharge.Text = "0";
            DeviceBus.StartReceiveMoney(txtNeed.Text.Todecimal());
        }

        private void btnReceiveMoney_Click(object sender, RoutedEventArgs e)
        {
            DeviceBus.SetReceive(txtyishou.Text.Todecimal());
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            DeviceBus.StopReceiveMoney();
        }

        private void btnStartCharge_Click(object sender, RoutedEventArgs e)
        {
            var charge = txtyishou.Text.Todecimal() - txtNeed.Text.Todecimal();
            DeviceBus.StartCharge(charge);
        }

        private void btnStartReadCard_Click(object sender, RoutedEventArgs e)
        {
            DeviceBus.StartReadCard();
        }
    }
}
