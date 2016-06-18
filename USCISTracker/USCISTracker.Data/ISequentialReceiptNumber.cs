using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USCISTracker.Data
{
    public interface ISequentialReceiptNumber
    {
        /// <summary>
        /// Get the next Receipt Number in the sequence
        /// </summary>
        /// <returns></returns>
        ICaseReceiptNumber GetNext();

        /// <summary>
        /// Get the previous Receipt Number in the sequence
        /// </summary>
        /// <returns></returns>
        ICaseReceiptNumber GetPrevious();
    }
}
