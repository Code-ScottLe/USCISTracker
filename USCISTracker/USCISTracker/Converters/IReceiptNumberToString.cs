using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace USCISTracker.Converters
{
    public class IReceiptNumberToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Data.ICaseReceiptNumber receiptNumber = value as Data.ICaseReceiptNumber;

            return receiptNumber.ReceiptNumber;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            string receiptNumber = value as string;
            
            return receiptNumber;
        }
    }
}
