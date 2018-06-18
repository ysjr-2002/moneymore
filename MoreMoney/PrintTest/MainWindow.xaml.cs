using System;
using System.Collections.Generic;
using System.Globalization;
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

        static FormattedText getFt(string content)
        {
            FormattedText ft = new FormattedText(content, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("宋体"), 14, Brushes.Black);
            return ft;
        }

        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            PrintDialog pd = new PrintDialog();

            if (pd.ShowDialog().GetValueOrDefault())
            {

                DrawingVisual dv = new DrawingVisual();
                DrawingContext dc = dv.RenderOpen();

                var ft1 = getFt("welcome to park");
                dc.DrawText(ft1, new Point(10, 0));

                var ft2 = getFt("datetime");
                dc.DrawText(ft2, new Point(10, 30));

                var ft3 = getFt("leave time");
                dc.DrawText(ft3, new Point(10, 60));

                var ft4 = getFt("停车时长:2小时10分");
                dc.DrawText(ft4, new Point(10, 90));

                //
                dc.DrawImage(imgqr.Source, new Rect { X = 10, Y = 120, Width = 100, Height = 100 });

                dc.Close();

                pd.PrintVisual(dv, "Print Test");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            PrintDialog pd = new PrintDialog();
            if (pd.ShowDialog().GetValueOrDefault())
            {
                pd.PrintVisual(printGridArea, "Print test");
            }
        }
    }
}
