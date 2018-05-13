using Common.NotifyBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MoreMoney.ViewModel
{
    public class MainViewModel : PropertyNotifyObject
    {
        public ICommand LoadedCommand { get; set; }

        public int TabSelecteIndex
        {
            get { return this.GetValue(s => s.TabSelecteIndex); }
            set { this.SetValue(s => s.TabSelecteIndex, value); }
        }
    }
}
