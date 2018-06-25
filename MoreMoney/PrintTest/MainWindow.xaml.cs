using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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

namespace PrintTest
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog pd = new PrintDialog();
            if (pd.ShowDialog().GetValueOrDefault())
            {
                pd.PrintVisual(printArea, "print test");
            }
        }

        static FormattedText getFt(string content, int fontsize = 18)
        {
            FormattedText ft = new FormattedText(content, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("宋体"), fontsize, Brushes.Black);
            return ft;
        }

        private void btnPrintDC_click(object sender, RoutedEventArgs e)
        {
            PrintDialog pd = new PrintDialog();
            DrawingVisual dv = new DrawingVisual();
            DrawingContext dc = dv.RenderOpen();

            var ft1 = getFt("欢迎光大安石中心");
            dc.DrawText(ft1, new Point(80, 10));

            dc.DrawImage(imgqr.Source, new Rect { X = 100, Y = 40, Width = 100, Height = 100 });

            var ft3 = getFt("打印时间:2018-06-24 13:28:35", 14);
            dc.DrawText(ft3, new Point(60, 150));

            dc.Close();
            pd.PrintVisual(dv, "Print Test");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            PrintDialog pd = new PrintDialog();
            if (pd.ShowDialog().GetValueOrDefault())
            {
                pd.PrintVisual(printGridArea, "Print test");
            }
        }

        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            try
            {
                TcpClient tcp = new TcpClient();
                tcp.Connect(new IPEndPoint(IPAddress.Parse("192.168.254.254"), 9100));

                NetworkStream nws = tcp.GetStream();
                StringBuilder sb = new StringBuilder();
                sb.Append("^XA");
                sb.Append("^FO300,20");
                sb.Append("^BQ,2,10");
                sb.Append("^FDQA,0123456789ABCDEFGHKJKFDJKFDJFKDJFDKF^FS");
                sb.Append("^XZ");

                var str = sb.ToString();
                var data = Encoding.UTF8.GetBytes(str);
                nws.Write(data, 0, data.Length);

                nws.Close();
                tcp.Close();
                tcp = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("连接失败->" + ex.Message);
            }
        }
    }
}
