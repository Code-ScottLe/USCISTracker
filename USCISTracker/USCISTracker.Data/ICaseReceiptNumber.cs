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
        /// The getter for the receive number
        /// </summary>
        string ReceiptNumber { get; }

        /// <summary>
        /// Get the name of the Service Center
        /// </summary>
        /// <returns></returns>
        string GetServiceCenter();
    }
}
