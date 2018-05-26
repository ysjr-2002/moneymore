using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyCore
{
    public class DataItems
    {
        public static List<DataItem> Items = new List<DataItem>();
        public static void Init()
        {
            Items.Add(new DataItem { code = "100", Text = "Program ID" });
            Items.Add(new DataItem { code = "104", Text = "Max. notes per bundle" });
            Items.Add(new DataItem { code = "110", Text = "Machine ID" });
            Items.Add(new DataItem { code = "118", Text = "Currency codes" });
            Items.Add(new DataItem { code = "202", Text = "Note sizes" });
            Items.Add(new DataItem { code = "211", Text = "Status on Non Addressed Feeders" });
            Items.Add(new DataItem { code = "236", Text = "Check Notes in transport" });
            Items.Add(new DataItem { code = "251", Text = "Tray ID source" });
            Items.Add(new DataItem { code = "252", Text = "Handshake mode" });
            Items.Add(new DataItem { code = "300", Text = "Reject counter 300" });
            Items.Add(new DataItem { code = "301", Text = "Status counter 301" });
            Items.Add(new DataItem { code = "303", Text = "Dispense counter(trip)" });
            Items.Add(new DataItem { code = "304", Text = "Reject counter(trip)" });
            Items.Add(new DataItem { code = "305", Text = "Zero reply" });
            Items.Add(new DataItem { code = "308", Text = "Total notes single rejected" });
            Items.Add(new DataItem { code = "309", Text = "Zero reply" });
            Items.Add(new DataItem { code = "318", Text = "Zero reply" });
            Items.Add(new DataItem { code = "319", Text = "Zero reply" });
            Items.Add(new DataItem { code = "320", Text = "Number of transactions started(trip)" });
            Items.Add(new DataItem { code = "330", Text = "Dispense counter(life)" });
            Items.Add(new DataItem { code = "331", Text = "Reject counter(life)" });
            Items.Add(new DataItem { code = "332", Text = "Zero reply" });
            Items.Add(new DataItem { code = "333", Text = "Number of transactions started(life)" });
            Items.Add(new DataItem { code = "334", Text = "Zero reply" });
            Items.Add(new DataItem { code = "390", Text = "Zero reply" });
            Items.Add(new DataItem { code = "395", Text = "Enable/ Disable Reject Vault" });
            Items.Add(new DataItem { code = "397", Text = "Module Status" });
            Items.Add(new DataItem { code = "399", Text = "Maximum number of notes in the reject compartment(full limit)" });
        }
    }

    public class DataItem
    {
        public string code { get; set; }

        public string Text { get; set; }
    }
}
