using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreMoney.Core
{
    public class StatusCode
    {
        static Dictionary<int, Remark> table = new Dictionary<int, Remark>();

        public static void Init()
        {
            table.Add(0x30, new Remark { Type = "W", MNEMONICNAME = "Successful Command" });
            table.Add(0x31, new Remark { Type = "W", MNEMONICNAME = "Low Level" });
            table.Add(0x32, new Remark { Type = "O", MNEMONICNAME = "Empty Cassette" });

            table.Add(0x33, new Remark { Type = "O", MNEMONICNAME = "Machine not Opened" });
            table.Add(0x34, new Remark { Type = "W", MNEMONICNAME = "Rejected Notes" });
            table.Add(0x35, new Remark { Type = "F", MNEMONICNAME = "Diverter Failure" });

            table.Add(0x36, new Remark { Type = "R", MNEMONICNAME = "Failure to Feed" });
            table.Add(0x37, new Remark { Type = "S", MNEMONICNAME = "Transmission Error" });
            table.Add(0x38, new Remark { Type = "S", MNEMONICNAME = "Illegal Command or Command Sequence" });

            table.Add(0x39, new Remark { Type = "F", MNEMONICNAME = "Jam in Note Qualifier" });
            table.Add(0x3A, new Remark { Type = "O", MNEMONICNAME = "Feed Cassette Not Present or Properly Installed" });
            table.Add(0x3F, new Remark { Type = "O", MNEMONICNAME = "Reject Vault Not Present or Properly Installed" });
            table.Add(0x42, new Remark { Type = "O", MNEMONICNAME = "Too Many Notes Requested" });
            table.Add(0x43, new Remark { Type = "F", MNEMONICNAME = "Jam in Note Feeder Transport" });
            table.Add(0x44, new Remark { Type = "W", MNEMONICNAME = "Reject Vault Almost Full" });

            table.Add(0x46, new Remark { Type = "F", MNEMONICNAME = "Main Motor Failure" });
            table.Add(0x49, new Remark { Type = "F", MNEMONICNAME = "Note Qualifier Faulty" });
            table.Add(0x4A, new Remark { Type = "R", MNEMONICNAME = "Note Feeder exit sensor failure" });
            table.Add(0x4D, new Remark { Type = "O", MNEMONICNAME = "Notes in Delivery Throat" });

            table.Add(0x4E, new Remark { Type = "S", MNEMONICNAME = "Communications Time-out" });
            table.Add(0x50, new Remark { Type = "S", MNEMONICNAME = "Cassette not Identified" });
            table.Add(0x51, new Remark { Type = "O", MNEMONICNAME = "Reject Vault Full" });

            table.Add(0x57, new Remark { Type = "F", MNEMONICNAME = "Error in Throat" });
            table.Add(0x5B, new Remark { Type = "R", MNEMONICNAME = "Sensor Error or Sensor Covered" });
            table.Add(0x60, new Remark { Type = "F", MNEMONICNAME = "Internal Failure" });

            table.Add(0x63, new Remark { Type = "W", MNEMONICNAME = "Module Need Service" });
            table.Add(0x65, new Remark { Type = "W", MNEMONICNAME = "No Message To Resend" });
            table.Add(0x68, new Remark { Type = "F", MNEMONICNAME = "Error in Note Transport" });
        }

        public static string GetTypeRemark(int key)
        {
            if (table.ContainsKey(key))
            {
                var remark = table[key];
                if( remark.Type != "W")
                {

                }
            }
            else
            {

            }
        }
    }

    public class Remark
    {
        /// <summary>
        /// W Warning Status
        /// S 软恢复
        /// O 操作恢复
        /// R 致命的
        /// F 错误的
        /// </summary>
        public string Type { get; set; }

        public string MNEMONICNAME { get; set; }
    }
}
