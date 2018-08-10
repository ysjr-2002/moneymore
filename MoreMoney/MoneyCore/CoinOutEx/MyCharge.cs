using dk.CctalkLib.Checksumms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoneyCore.CoinOutEx
{
    public class MyCharge
    {
        public void test()
        {
            var bytes = new byte[] { 0x03, 0x00, 0x01, 0xFE };
            var sum = Checksum.ChecksumHelper(bytes);
            Console.WriteLine(sum.ToHex());
        }
    }
}
