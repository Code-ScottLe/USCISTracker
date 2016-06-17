using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USCISTracker.Data
{
    public interface ICaseReceiptNumber
    {
        /// <summary>
        /// Get the actual USCIS Receipt Number
        /// </summary>
        /// <returns></returns>
        string GetReceiptNumber();

        /// <summary>
        /// Get the name of the Service Center
        /// </summary>
        /// <returns></returns>
        string GetServiceCenter();
    }
}
