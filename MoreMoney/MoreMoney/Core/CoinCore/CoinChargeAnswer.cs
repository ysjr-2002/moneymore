using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreMoney.Core.CoinCore
{
    /// <summary>
    /// 硬币找零应答
    /// </summary>
    enum CoinChargeAnswer : byte
    {
        TimeOut = 0x00,
        /// <summary>
        /// 找零OK
        /// </summary>
        OK = 0x52,
        /// <summary>
        /// 料斗空
        /// </summary>
        HopperEmpty = 0x4F,
    }
}
