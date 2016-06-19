using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USCISTracker.Data
{
    public interface ICase : INotifyPropertyChanged
    {
        ICaseReceiptNumber ReceiptNumber { get; }
        string Status { get; }
        string FormType { get; }
        string Details { get;}
        string LastUpdate { get; }
        string Name { get; set; }
        Task UpdateFromHTMLAsync(string html);
    }
}
