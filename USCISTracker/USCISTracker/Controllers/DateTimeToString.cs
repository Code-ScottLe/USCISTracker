using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace USCISTracker.Controllers
{
    public class DateTimeToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is DateTime))
            {
                throw new InvalidCastException("value is not DateTime");
            }

            else
            {
                return ((DateTime)value).ToString(@"MM\/dd\/yyyy HH:mm");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
