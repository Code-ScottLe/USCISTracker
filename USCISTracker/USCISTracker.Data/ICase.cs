using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USCISTracker.Data
{
    public interface ICase
    {

        /// <summary>
        /// Case Receipt Number
        /// </summary>
        ICaseReceiptNumber ReceiptNumber { get; }

        /// <summary>
        /// Get the form type of this case/ receipt number. I.E : I-765
        /// </summary>
        /// <returns></returns>
        string GetFormType();

        /// <summary>
        /// Get the last updated time stamp in string. I.E : May 18, 2016
        /// </summary>
        /// <returns></returns>
        string GetLastUpdatedDate();

        /// <summary>
        /// Get the current status of the case
        /// </summary>
        /// <returns></returns>
        string GetCurrentStatus();

        /// <summary>
        /// Get the full body message from USCIS
        /// </summary>
        /// <returns></returns>
        string GetDetails();
    }
}
