using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Parser.Html;
using AngleSharp.Html;


namespace USCISTracker.Data
{
    public class Case : ICase
    {
        #region Fields
        private ICaseReceiptNumber receiptNumber;
        private string status;
        private string formType;
        private string fullMessage;
        private DateTime lastUpdate;
        #endregion

        #region Properties
        public ICaseReceiptNumber ReceiptNumber
        {
            get
            {
                return receiptNumber;
            }

            protected set
            {
                receiptNumber = value;
            }
        }
        #endregion


        #region Constructors
        /// <summary>
        /// Default Constructor, Hidden
        /// </summary>
        Case()
        {
            ReceiptNumber = null;
        }

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


        /// <summary>
        /// Parse the respond HTML and return a new instance of the Case
        /// </summary>
        /// <param name="respondHTML">the HTML in string form of the respond page</param>
        /// <param name="receiptNumber">Receipt number of the responded HTML</param>
        /// <returns></returns>
        public static async Task<Case> GenerateFromHTML(string respondHTML, ICaseReceiptNumber receiptNumber)
        {
            //Parse the given HTML
            HtmlParser parser = new HtmlParser();
            var document = await parser.ParseAsync(respondHTML);

            //Search for the case status
            var caseStatusDom =  document.All.Where(n => n.ClassName == "current-status-sec").Select(n => n).FirstOrDefault();

            //Search for the mainbody message:
            var caseMessageDom = document.All.Where(n => n.ClassName == "rows text-center").Select(n => n).FirstOrDefault();


            return null;
            
        }
        #endregion


    }
}
