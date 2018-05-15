using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace MoreMoney.Core
{
    public class Log
    {
        private static RichTextBox log;
        public static void Set(RichTextBox log)
        {
            Log.log = log;
        }

        public static void Out(string str)
        {
            var document = log.Document;
            Paragraph p1 = new Paragraph(new Run("Out:" + str.Replace('\r', ' ')));
            document.Blocks.Add(p1);
        }

        public static void In(string str)
        {
            var document = log.Document;
            Paragraph p1 = new Paragraph(new Run("In:" + str.Replace('\r', ' ')));
            document.Blocks.Add(p1);
        }
    }
}
