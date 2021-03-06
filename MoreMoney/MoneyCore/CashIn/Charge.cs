﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyCore.Cash
{
    public static class Charge
    {
        /// <summary>
        /// 找零逻辑
        /// </summary>
        /// <param name="charge"></param>
        /// <param name="m1"></param>
        /// <param name="m5"></param>
        /// <param name="m50"></param>
        /// <param name="m100"></param>
        public static void GetCount(decimal charge, out int m1, out int m5, out int m50, out int m100)
        {
            m100 = 0;
            m50 = 0;
            m5 = 0;
            m1 = 0;
            if (charge >= 100)
            {
                m100 = (int)(charge / 100);
                charge = charge - (m100 * 100);
            }
            if (charge >= 50)
            {
                m50 = (int)(charge / 50);
                charge = charge - (m50 * 50);
            }
            if (charge >= 5)
            {
                m5 = (int)(charge / 5);
                charge = charge - (m5 * 5);
            }
            m1 = (int)charge;
        }
    }
}
