using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USCISTracker.API
{
    /// <summary>
    /// Represent a session to USCIS (i.e a browser tab)
    /// </summary>
    public class Session
    {
        #region Fields

        private Windows.UI.Xaml.Controls.WebView currentWebView;

        #endregion


        #region Properties


        /// <summary>
        /// Get the underlaying webview (or tab) that is running this session
        /// </summary>
        public Windows.UI.Xaml.Controls.WebView CurrentWebView
        {
            get
            {
                return currentWebView;
            }

            protected set
            {
                currentWebView = value;
            }
        }

        /// <summary>
        /// Indicate if the WebView has completed 
        /// </summary>
        public bool NavigateCompleted
        {
            get;set;
        }

        public bool NavigateFailed
        {
            get;set;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor, open an instance of WebView to the default page of the checker tool
        /// Default website for the tool is : https://egov.uscis.gov/casestatus/landing.do
        /// </summary>
        public Session()
        {
            //Initialize new session
            CurrentWebView = new Windows.UI.Xaml.Controls.WebView();
            CurrentWebView.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            //Subscribe to the navigation success event. WebView goes from NavStart => ContentLoading => DOMContentLoaded => NavCompleted
            //More info: https://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.controls.webview.aspx
            CurrentWebView.NavigationCompleted += this.CurrentWebView_NavigationCompleted;
            

            //Navigate to the website.
            CurrentWebView.Navigate(new Uri("https://egov.uscis.gov/casestatus/landing.do"));
            NavigateCompleted = false;
            NavigateFailed = false;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Event handler for the Navigation Completed from the WebView instance.
        /// </summary>
        /// <param name="sender">WebView instasnce that fired the event</param>
        /// <param name="args">Custom Arguments about this event</param>
        private void CurrentWebView_NavigationCompleted(Windows.UI.Xaml.Controls.WebView sender, Windows.UI.Xaml.Controls.WebViewNavigationCompletedEventArgs args)
        {
            
            if (args.IsSuccess == false)
            {
                NavigateFailed = true;                               
                NavigateCompleted = true;
            }

            else
            {
                //Navigation success
                
                //Check if the page is still valid.
                if(CurrentWebView.DocumentTitle != "myUSCIS - Case Status Search")
                {
                    NavigateFailed = true;
                }

                NavigateCompleted = true;
            }
        }


        /// <summary>
        /// Set the receipt number for the session
        /// </summary>
        /// <param name="receiptNumber"></param>
        /// <returns></returns>
        public async Task SetReceiptNumberAsync(string receiptNumber)
        {
            while(NavigateCompleted == false)
            {
                //Clunky code. 
                await Task.Delay(500);
            }

            if (NavigateFailed == true)
            {
                throw new InvalidOperationException("WebView failed to navigate to tool. Please refresh.");
            }

            //We use eval to use javascript to manually fill in the receipt number box
            string js = $"document.getElementById(\"receipt_number\").value = \"{receiptNumber}\"";

            //Type in the browser via Javascript
            string[] jsArgs = {js};
            string res = await CurrentWebView.InvokeScriptAsync("eval", jsArgs);


#if DEBUG
            string jsTest = $"document.getElementById(\"receipt_number\").value";
            string[] jsArgsTest = { jsTest };
            string val = await CurrentWebView.InvokeScriptAsync("eval", jsArgsTest);

            var compare = string.Equals(val, receiptNumber);    //for some reason, == won't work here.

            if(compare == false)
            {
                throw new InvalidOperationException("Javascript injection failed! 2 receipt numbers don't match!");
            }
#endif

        } 


        /// <summary>
        /// Invoke the Javascript to send the case number to the website and refresh.
        /// </summary>
        /// <returns></returns>
        public async Task CheckCaseStatusAsync()
        {
            //Try to check if the receipt number is there.
            string jsTestCaseStatusNumber = $"document.getElementById(\"receipt_number\").value";
            string[] jsTestArgs = { jsTestCaseStatusNumber };
            string testVal = await CurrentWebView.InvokeScriptAsync("eval", jsTestArgs);

            if(string.IsNullOrEmpty(testVal) && string.IsNullOrWhiteSpace(testVal))
            {
                //Empty case status number
                throw new MissingFieldException("Receipt Number Missing!");
            }

            if(testVal.Length != 13)
            {
                //Wrong length
                throw new InvalidProgramException("Invalid Receipt Number!");
            }

            //Invoke the button.
            string js = $"document.getElementsByName(\"initCaseSearch\")[0].click()";
            string[] jsArgs = { js };
            string res = await CurrentWebView.InvokeScriptAsync("eval",jsArgs);

            //Because the webView will be reload to a new one, reset status
            NavigateCompleted = false;
            NavigateFailed = false;

        }
        #endregion

    }
}
