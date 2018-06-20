using System;

namespace dk.CctalkLib.Devices
{
    public class CoinTypeInfo
    {
        public CoinTypeInfo(String name, Decimal value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// 名称
        /// </summary>
		public String Name { get; private set; }
        /// <summary>
        /// 金额大小
        /// </summary>
		public Decimal Value { get; private set; }

    }
}