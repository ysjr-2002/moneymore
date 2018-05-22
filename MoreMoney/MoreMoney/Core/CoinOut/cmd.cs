using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreMoney.Core.CoinOut
{
    class cmd
    {
        public static int cmd_reset = 0x01;

        public static int cmd_pool = 254;

        public static int cmd_test_hopper = 163;

        public static int cmd_enable_hopper = 164;

        public static int cmd_request_hopper_status = 166;
    }
}
