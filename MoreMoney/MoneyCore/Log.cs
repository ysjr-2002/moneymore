using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyCore
{
    public class DllLog
    {
        //private static RichTextBox log;
        //public static void Set(RichTextBox log)
        //{
        //    Log.log = log;
        //}
        static Action<string> login;
        static Action<string> logout;
        public static void SetLogIn(Action<string> act)
        {
            login = act;
        }

        public static void SetLogOut(Action<string> act)
        {
            logout = act;
        }

        public static void Out(string str)
        {
            //if(System.Windows.Application.Current.Dispatcher.CheckAccess())
            //{
            //    var document = log.Document;
            //    Paragraph p1 = new Paragraph(new Run("Out:" + str.Replace('\r', ' ')));
            //    document.Blocks.Add(p1);
            //}
            //else
            //{
            //    System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
            //    {
            //        var document = log.Document;
            //        Paragraph p1 = new Paragraph(new Run("Out:" + str.Replace('\r', ' ')));
            //        document.Blocks.Add(p1);
            //    }));
            //}
            login(str);
        }

        public static void In(string str)
        {
            //System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
            //{
            //    var document = log.Document;
            //    if (document.Blocks.Count > 100)
            //    {
            //        document.Blocks.Clear();
            //    }
            //    Paragraph p1 = new Paragraph(new Run("In:" + str.Replace('\r', ' ')));
            //    document.Blocks.Add(p1);
            //    log.ScrollToEnd();
            //}));
            logout(str);
        }
    }
}
