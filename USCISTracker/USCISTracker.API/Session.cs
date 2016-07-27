using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace USCISTracker.API
{
    /// <summary>
    /// Represent a session to USCIS (i.e a browser tab)
    /// </summary>
    public class Session
    {
        #region Fields

        private static string USCISPrefix = "https://egov.uscis.gov/casestatus/mycasestatus.do?appReceiptNum=";
        private HttpClient client;
        #endregion


        #region Properties

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor, Setup the WebView
        /// </summary>
        public Session()
        {
            client = new HttpClient();   
         
        }

        #endregion


        #region Methods
        public async Task<string> CheckCaseStatusAsync(string receiptNumber)
        {
            string html = await client.GetStringAsync($"{USCISPrefix}{receiptNumber}");
            return html;
        }
 
        #endregion

    }
}
