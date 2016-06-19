using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USCISTracker.Data
{
    public interface ICaseReceiptNumber : INotifyPropertyChanged
    {
        string ReceiptNumber { get; set; }

    }
}
