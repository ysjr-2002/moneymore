using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyCore
{
    public class DllLog
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
        }

        public static void In(string str)
        {
            login?.Invoke(str);
        }
    }
}
