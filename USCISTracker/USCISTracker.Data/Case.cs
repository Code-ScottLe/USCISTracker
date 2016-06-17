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
        private string details;
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

        public string Status
        {
            get
            {
                return status;
            }

            protected set

            {
                status = value;
            }
        }

        public string FormType
        {
            get
            {
                return formType;
            }

            protected set
            {
                formType = value;
            }
        }

        public string Details
        {
            get
            {
                return details;
            }

            protected set
            {
                details = value;
            }
        }

        
        public DateTime LastUpdate
        {
            get
            {
                return lastUpdate;
            }

            protected set
            {
                lastUpdate = value;
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

        public string GetDetails()
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
        public static async Task<Case> GenerateFromHTMLAsync(string respondHTML, ICaseReceiptNumber receiptNumber)
        {
            //Parse the given HTML
            HtmlParser parser = new HtmlParser();
            var document = await parser.ParseAsync(respondHTML);

            //Search for the case status
            var caseStatusDom =  document.All.Where(n => n.ClassName == "current-status-sec").Select(n => n).FirstOrDefault();

            //Search for the mainbody message:
            var caseMessageDom = document.All.Where(n => n.ClassName == "rows text-center").Select(n => n).FirstOrDefault();

            //Trim out the un-necessarily part for case status
            string caseStatus = caseStatusDom.TextContent.Trim();
            int temp = caseStatus.IndexOf(":");
            caseStatus = caseStatus.Substring( temp + 1, caseStatus.Length - temp - 2 ).Trim();

            //Get the case detail:
            var caseDetails = caseMessageDom.Children[1].TextContent.Trim();


            //Get the case last updated date
            var pieces = caseDetails.Split(',');

            var lastUpdateInString = pieces[0].Substring(3) + pieces[1];

            DateTime lastUpdate = new DateTime();

            DateTime.TryParse(lastUpdateInString, out lastUpdate);

            //Get Form Type
            string formType = "N/A";

            if(pieces[2].Contains("we received your Form") == true)
            {
                formType = pieces[2].Substring("we received your Form".Length + 1);
            }

            //create a new instance
            Case currentCase = new Case();
            currentCase.ReceiptNumber = receiptNumber;
            currentCase.LastUpdate = lastUpdate;
            currentCase.Status = caseStatus;
            currentCase.Details = caseDetails;
            currentCase.FormType = formType;


            return currentCase;
            
        }
        #endregion


    }
}
