using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USCISTracker.Data
{
    public class Case : ICase
    {
        #region Fields

        #endregion

        #region Properties
        public ICaseReceiptNumber ReceiptNumber
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        #endregion


        #region Constructors

        #endregion

        #region Methods
        public string GetCurrentStatus()
        {
            throw new NotImplementedException();
        }

        public string GetFormType()
        {
            throw new NotImplementedException();
        }

        public string GetFullMessage()
        {
            throw new NotImplementedException();
        }

        public string GetLastUpdatedDate()
        {
            throw new NotImplementedException();
        }
        #endregion


    }
}
