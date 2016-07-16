using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace USCISTracker.Controllers
{
    public class DateTimeLastUpdateToString : IValueConverter
    {
        /// <summary>
        /// Convert from DateTime to String for Last Update
        /// </summary>
        /// <param name="value">DateTime of LastUpdate</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns>a string represent the Datetime of Last Update</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if((value is DateTime) == false)
            {
                throw new InvalidCastException("Convert Value is not DateTime");
            }

            else
            {
                return ((DateTime)value).ToString(@"MM\/dd\/yyyy");
            }
        }


        /// <summary>
        /// Convert from String to DateTime for Last Update
        /// </summary>
        /// <param name="value">string of Last Update</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns>DateTime</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
