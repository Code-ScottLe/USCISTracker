using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using USCISTracker.Data;

namespace USCISTracker.Controllers
{
    public class IReceiptNumberToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            ICaseReceiptNumber receiptNumber = value as ICaseReceiptNumber;

            if (receiptNumber == null)
            {
                return null;
            }

            else
            {
                return receiptNumber.ReceiptNumber;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return new CaseReceiptNumber(value as string);
        }
    }
}
