using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Parser.Html;
using AngleSharp.Html;
using System.ComponentModel;

namespace USCISTracker.Data
{
    public class Case : INotifyPropertyChanged
    {
        #region Fields
        private CaseReceiptNumber receiptNumber;
        private string status;
        private string formType;
        private string details;
        private DateTime lastUpdate;
        private string name;
        private DateTime lastRefresh;
        #endregion

        #region Properties
        public CaseReceiptNumber ReceiptNumber
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

        public string Status
        {
            get
            {
                return status;
            }

            set

            {
                status = value;
                OnPropertyChanged("Status");
            }
        }

        public string FormType
        {
            get
            {
                return formType;
            }

            set
            {
                formType = value;
                OnPropertyChanged("FormType");
            }
        }

        public string Details
        {
            get
            {
                return details;
            }

            set
            {
                details = value;
                OnPropertyChanged("Details");
            }
        }
    
        public DateTime LastCaseUpdate
        {
            get
            {
                return lastUpdate;
            }

            set
            {
                lastUpdate = value;
                OnPropertyChanged("LastCaseUpdate");
            }
        }       

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        public DateTime LastRefresh
        {
            get
            {
                return lastRefresh;
            }

            set
            {
                lastRefresh = value;
                OnPropertyChanged("LastRefresh");
            }
        }
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Constructors
        /// <summary>
        /// Default Constructor
        /// </summary>
        public Case()
        {
            ReceiptNumber = null;
        }


        public Case(string CaseReceiptNumber)
        {
            ReceiptNumber = new CaseReceiptNumber(CaseReceiptNumber);
        }

        public Case(string CaseReceiptNumber, string CaseName)
        {
            ReceiptNumber = new CaseReceiptNumber(CaseReceiptNumber);
            Name = CaseName;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Update the current case with the returned HTML from the current case status page
        /// </summary>
        /// <param name="html">current html from the page</param>
        /// <returns></returns>
        public async Task UpdateFromHTMLAsync(string respondHTML)
        {
            //Parse the given HTML
            HtmlParser parser = new HtmlParser();
            var document = await parser.ParseAsync(respondHTML);

            //Search for problems first.
            if (document.All.Where(n => n.Id == "formErrorMessages").Select(n => n).FirstOrDefault().ChildElementCount > 0)
            {
                //Error form, due to some kind of internal error.
                var formErrorMessageDom = document.All.Where(n => n.Id == "formErrorMessages").Select(n => n).FirstOrDefault();
                string errorStatus = formErrorMessageDom.Children[0].TextContent.Trim();
                string errorDetail = formErrorMessageDom.Children[1].TextContent.Trim();

                Status = "Error!";
                if(FormType == "")
                {
                    FormType = "N/A";
                }

                Details = errorStatus + errorDetail;
                LastCaseUpdate = new DateTime();
                LastRefresh = DateTime.Now;

                return;

            }

            //Search for the case status
            var caseStatusDom = document.All.Where(n => n.ClassName == "current-status-sec").Select(n => n).FirstOrDefault();

            //Search for the mainbody message:
            var caseMessageDom = document.All.Where(n => n.ClassName == "rows text-center").Select(n => n).FirstOrDefault();

            //Trim out the un-necessarily part for case status
            string caseStatus = caseStatusDom.TextContent.Trim();
            int temp = caseStatus.IndexOf(":");
            caseStatus = caseStatus.Substring(temp + 1, caseStatus.Length - temp - 2).Trim();

            //Update Case Status
            Status = caseStatus;

            //Get the case detail:
            var caseDetails = caseMessageDom.Children[1].TextContent.Trim();

            //Update Case Detail
            Details = caseDetails;

            //Get the case last updated date
            var pieces = caseDetails.Split(',');

            var lastUpdateInString = pieces[0].Substring(3) + pieces[1];

            DateTime lastUpdate = new DateTime();

            DateTime.TryParse(lastUpdateInString, out lastUpdate);

            //Update last update
            LastCaseUpdate = lastUpdate;

            //Get Form Type
            string formType = "N/A";

            if (pieces[2].Contains("we received your Form") == true)
            {
                formType = pieces[2].Substring("we received your Form".Length + 1);
            }

            //Update form type if it is new or previously error-ed 
            if(string.IsNullOrEmpty(FormType) || FormType == "N/A")
            {
                FormType = formType;
            }

            //Update last case refresh time
            LastRefresh = DateTime.Now;
           
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

        /// <summary>
        /// Parse the respond HTML and return a new instance of the Case
        /// </summary>
        /// <param name="respondHTML">the HTML in string form of the respond page</param>
        /// <param name="receiptNumber">Receipt number of the responded HTML</param>
        /// <returns></returns>
        //[Obsolete]
        //public static async Task<Case> GenerateFromHTMLAsync(string respondHTML, CaseReceiptNumber receiptNumber)
        //{
        //    //Parse the given HTML
        //    HtmlParser parser = new HtmlParser();
        //    var document = await parser.ParseAsync(respondHTML);

        //    //Search for problems first.
        //    if(document.All.Where(n=>n.Id == "formErrorMessages").Select(n=>n).FirstOrDefault().ChildElementCount > 0)
        //    {
        //        //Error form, due to some kind of internal error.
        //        var formErrorMessageDom = document.All.Where(n => n.Id == "formErrorMessages").Select(n => n).FirstOrDefault();
        //        string errorStatus = formErrorMessageDom.Children[0].TextContent.Trim();
        //        string errorDetail = formErrorMessageDom.Children[1].TextContent.Trim();

        //        Case invalidCase = new Case()
        //        {
        //            ReceiptNumber = receiptNumber,
        //            Status = "Error!",
        //            FormType = "N/A",
        //            Details = errorStatus + errorDetail,
        //            LastCaseUpdate = new DateTime(),
        //            Name = receiptNumber.ReceiptNumber

        //        };

        //        return invalidCase;
                
        //    }

        //    //Search for the case status
        //    var caseStatusDom =  document.All.Where(n => n.ClassName == "current-status-sec").Select(n => n).FirstOrDefault();

        //    //Search for the mainbody message:
        //    var caseMessageDom = document.All.Where(n => n.ClassName == "rows text-center").Select(n => n).FirstOrDefault();

        //    //Trim out the un-necessarily part for case status
        //    string caseStatus = caseStatusDom.TextContent.Trim();
        //    int temp = caseStatus.IndexOf(":");
        //    caseStatus = caseStatus.Substring( temp + 1, caseStatus.Length - temp - 2 ).Trim();

        //    //Get the case detail:
        //    var caseDetails = caseMessageDom.Children[1].TextContent.Trim();

        //    //Get the case last updated date
        //    var pieces = caseDetails.Split(',');

        //    var lastUpdateInString = pieces[0].Substring(3) + pieces[1];

        //    DateTime lastUpdate = new DateTime();

        //    DateTime.TryParse(lastUpdateInString, out lastUpdate);

        //    //Get Form Type
        //    string formType = "N/A";

        //    if(pieces[2].Contains("we received your Form") == true)
        //    {
        //        formType = pieces[2].Substring("we received your Form".Length + 1);
        //    }

        //    //create a new instance
        //    Case currentCase = new Case();
        //    currentCase.ReceiptNumber = receiptNumber;
        //    currentCase.LastCaseUpdate = lastUpdate;
        //    currentCase.Status = caseStatus;
        //    currentCase.Details = caseDetails;
        //    currentCase.FormType = formType;
        //    currentCase.Name = receiptNumber.ReceiptNumber;

        //    return currentCase;
            
        //}
        #endregion


    }
}
