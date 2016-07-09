using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USCISTracker.Data
{
    public static class CaseFactory
    {
        /// <summary>
        /// Generate a case with a given receipt number
        /// </summary>
        /// <param name="caseReceiptNumber">Receipt number of the case (Ex: YSC16900xxxxx)</param>
        /// <returns></returns>
        public static ICase GetCase(string caseReceiptNumber, string caseName = "")
        {
            //create a case.
            Case currentCase = new Case();
            currentCase.ReceiptNumber = new CaseReceiptNumber(caseReceiptNumber);

            if(!string.IsNullOrEmpty(caseName))
            {
                currentCase.Name = caseName;
            }

            return currentCase;
        }
    }
}
