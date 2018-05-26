using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dk.CctalkLib.Devices
{
    public class BillTypeInfo
    {
        public BillTypeInfo(String name, Decimal value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// 名称
        /// </summary>
		public String Name { get; private set; }
        /// <summary>
        /// 大小
        /// </summary>
		public Decimal Value { get; private set; }
    }
}
