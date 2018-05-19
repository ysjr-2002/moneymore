using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreMoney.Core
{
    public delegate void OnReadCardEventHandle(object sender, string cardNo);

    public delegate void OnAcceptMoneyEventHandler(object sender, int money);
    public delegate void OnAcceptMoneyWithAllEventHandler(object sender, int money, int total);

    public delegate void OnChargeEventHandler(object sender, List<ChargeItem> items);

    public delegate void OnChargingEventHandler(object sender, ChargeItem item);

    public delegate void OnHopperEmptyEventHandler(object sender, EventArgs e);

    public class ChargeItem
    {
        public ChargeMoneyType ChargeMoneyType { get; set; }
    }

    /// <summary>
    /// 找零枚举
    /// </summary>
    public enum ChargeMoneyType
    {
        /// <summary>
        /// 1元
        /// </summary>
        M1,
        /// <summary>
        /// 5元
        /// </summary>
        M5,
        /// <summary>
        /// 50元
        /// </summary>
        M50,
        /// <summary>
        /// 100元
        /// </summary>
        M100
    }
}
