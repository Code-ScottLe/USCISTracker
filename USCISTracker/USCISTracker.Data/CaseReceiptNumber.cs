using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USCISTracker.Data
{
    public class CaseReceiptNumber : ICaseReceiptNumber
    {
        #region Fields
        private string receiptNumber;
        #endregion


        #region Properties
        public string ReceiptNumber
        {
            get
            {
                return receiptNumber;
            }

            set
            {
                receiptNumber = value;
            }
        }
        #endregion


        #region Constructors
        CaseReceiptNumber()
        {
            ReceiptNumber = "";
        }

        CaseReceiptNumber(string Receipt)
        {
            ReceiptNumber = Receipt;
        }
        #endregion

        #region Methods


        /// <summary>
        /// Get the Receipt Number
        /// </summary>
        /// <returns></returns>
        public string GetReceiptNumber()
        {
            return ReceiptNumber;
        }


        /// <summary>
        /// Get the Service Center Location based on the Code
        /// </summary>
        /// <returns></returns>
        public string GetServiceCenter()
        {
            //get the code of the service center
            string serviceCenterCode = ReceiptNumber.Substring(0, 3);

            switch(serviceCenterCode)
            {
                case "YSC":
                    return "Potomac";
                case "EAC":
                    return "Vermont";
                case "WAC":
                    return "California";
                case "LIN":
                    return "Nebraska";
                case "SRC":
                    return "Texas";
            }


            return "N/A";

        }


        public static CaseReceiptNumber operator++(CaseReceiptNumber a)
        {
            //YSC1690058940
            string ServiceCenterAndCompNumber = a.ReceiptNumber.Substring(0, 8);
            string UniqueID = a.ReceiptNumber.Substring(8);

            int temp = -1;

            int.TryParse(UniqueID, out temp);

            temp++;

            return new CaseReceiptNumber(ServiceCenterAndCompNumber + temp.ToString());
        } 

        #endregion
    }
}
