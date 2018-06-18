using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyCore
{
    public static class Log
    {
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
            logout?.Invoke(str);
            Debug.WriteLine("hz:" + str);
        }

        static string pre = "";
        public static void In(string str)
        {
            if (pre == str)
                return;

            pre = str;
            login?.Invoke(str);
            Debug.WriteLine("hz:" + str);
        }
    }
}
