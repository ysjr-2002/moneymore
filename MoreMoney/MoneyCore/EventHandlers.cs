using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyCore
{
    public delegate void OnReadCardEventHandle(object sender, string cardNo);

    public delegate void OnAcceptMoneyEventHandler(object sender, int money);
    /// <summary>
    /// 投钱
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="currentMoney">本次金额</param>
    /// <param name="recevieMoney">实收</param>
    public delegate void OnAcceptMoneyWithAllEventHandler(object sender, decimal currentMoney, decimal recevieMoney);

    public delegate void OnChargeEventHandler(object sender, Dictionary<ChargeMoneyType, int> items, decimal unChargeMoney);

    public delegate void OnChargingEventHandler(object sender, ChargeMoneyType item);

    public delegate void OnHopperEmptyEventHandler(object sender, ChargeMoneyType item);

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
