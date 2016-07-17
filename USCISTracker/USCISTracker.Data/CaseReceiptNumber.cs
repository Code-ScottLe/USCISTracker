using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USCISTracker.Data
{
    public class CaseReceiptNumber
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
                OnPropertyChanged("ReceiptNumber");
            }
        }
        #endregion


        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion


        #region Constructors
        public CaseReceiptNumber()
        {
            ReceiptNumber = "";
        }

        /// <summary>
        /// Generete the new instance of the class with a given receipt number
        /// </summary>
        /// <param name="Receipt">Receipt number in string, i.e "YSC16..."</param>
        public CaseReceiptNumber(string Receipt)
        {
            if(Receipt.Length != 13)
            {
                throw new ArgumentNullException("Receipt is not 13 char");
            }
            ReceiptNumber = Receipt;
        }
        #endregion

        #region Methods

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

        public CaseReceiptNumber GetNext()
        {
            string ServiceCenterAndCompNumber = ReceiptNumber.Substring(0, 8);
            string UniqueID = ReceiptNumber.Substring(8);

            int temp = -1;

            int.TryParse(UniqueID, out temp);

            temp++;

            return new CaseReceiptNumber(ServiceCenterAndCompNumber + temp.ToString()); ;
        }

        public CaseReceiptNumber GetPrevious()
        {
            string ServiceCenterAndCompNumber = ReceiptNumber.Substring(0, 8);
            string UniqueID = ReceiptNumber.Substring(8);

            int temp = -1;

            int.TryParse(UniqueID, out temp);

            temp--;

            return new CaseReceiptNumber(ServiceCenterAndCompNumber + temp.ToString());
        }

        public static CaseReceiptNumber operator++(CaseReceiptNumber a)
        {
            return a.GetNext() as CaseReceiptNumber;
        } 

        public static CaseReceiptNumber operator--(CaseReceiptNumber a)
        {
            return a.GetPrevious() as CaseReceiptNumber;
        }

        /// <summary>
        /// Helper method to fire the Property Changed
        /// </summary>
        /// <param name="propertyName">the name of the changed property</param>
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
