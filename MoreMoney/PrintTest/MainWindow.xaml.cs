using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Printing;
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
        /// <summary>
        /// http://www.chongshang.com.cn/manual/ZPL_font.shtml
        /// 字体设置
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog pd = new PrintDialog();
            //if (pd.ShowDialog().GetValueOrDefault())
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
                String str = "";
                StringBuilder sb = new StringBuilder();
                sb.Append("^XA");
                sb.Append("^FO300,20");
                sb.Append("^BQ,2,10");
                sb.Append("^FDQA,0123456789ABCDEFGHKJKFDJKFDJFKDJFDKF^FS");
                sb.Append("^XZ");

                //StringBuilder sb = new StringBuilder();
                //sb.Append("^XA");
                //sb.Append("^A2N,50,50,B: CYRI_UB.FNT");
                //sb.Append("^FO100,100");
                //sb.Append("^FDZebra Printer Fonts^ FS");
                //sb.Append("^A2N,40,40");
                //sb.Append("^F0100,150");
                //sb.Append("^FDThis uses B:CYRI_UB.FNT ^ FS");
                //sb.Append("^XZ");

                str = sb.ToString();
                SendByTcp(str);
            }
            catch (Exception ex)
            {
                MessageBox.Show("连接失败->" + ex.Message);
            }
        }

        private void SendByTcp(string content)
        {
            TcpClient tcp = new TcpClient();
            tcp.Connect(new IPEndPoint(IPAddress.Parse("192.168.0.2"), 9100));
            NetworkStream nws = tcp.GetStream();
            var data = Encoding.UTF8.GetBytes(content);
            nws.Write(data, 0, data.Length);
            nws.Close();
            tcp.Close();
            tcp = null;
        }

        private void SendByUdp(string content)
        {
            var data = Encoding.UTF8.GetBytes(content);

            UdpClient udp = new UdpClient();
            udp.Send(data, data.Length, new IPEndPoint(IPAddress.Parse("192.168.0.5"), 9100));
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

            UcPrint print = new PrintTest.UcPrint();

            PrintTicket pt = new PrintTicket();
            pt.CopyCount = 2;
            PrintDialog pd = new PrintDialog();
            //pt = pd.PrintTicket;
            //pt.CopyCount = 2;
            pd.PrintTicket = pt;
            pd.PrintVisual(print, "est");
        }

        private void Button_Click3(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("^XA");
            sb.Append("^FO130,10^BY3");
            sb.Append("^BAN,20,Y,N,N");
            sb.Append("^FD0123456789ABCDE^FS");
            sb.Append("^FO300,60");
            sb.Append("^BQ,2,10");
            sb.Append("^FDQA,0123456789ABCDEFGHKJKFDJKFDJFKDJFDKF^FS");
            sb.Append("^XZ");

            SendByTcp(sb.ToString());
        }
    }
}
