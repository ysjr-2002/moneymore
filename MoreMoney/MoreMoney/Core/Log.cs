using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;

namespace MoreMoney.Core
{
    class Log
    {
        static RichTextBox log;
        public static void Set(RichTextBox log)
        {
            Log.log = log;
        }

        public static void Out(string str)
        {
            var document = log.Document;
            Paragraph p1 = new Paragraph(new Run("Out:" + str.Replace('\r', ' ')));
            Run run = new Run(str);
            document.Blocks.Add(p1);
        }
    }
}
