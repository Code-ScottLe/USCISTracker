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
        /// Default constructor, Setup the WebView
        /// </summary>
        public Session()
        {
            //Initialize new session
            CurrentWebView = new Windows.UI.Xaml.Controls.WebView(Windows.UI.Xaml.Controls.WebViewExecutionMode.SeparateThread);
            CurrentWebView.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            //Subscribe to the navigation success event. WebView goes from NavStart => ContentLoading => DOMContentLoaded => NavCompleted
            //More info: https://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.controls.webview.aspx
            CurrentWebView.NavigationCompleted += CurrentWebView_NavigationCompleted;
            
         
        }

        #endregion


        #region Methods

        /// <summary>
        /// Connect to the USCIS tool
        /// Default website for the tool is : https://egov.uscis.gov/casestatus/landing.do
        /// </summary>
        /// <returns></returns>
        public async Task ConnectAsync()
        {
            //Navigate to the website.
            NavigateCompleted = false;
            NavigateFailed = false;
            CurrentWebView.Navigate(new Uri("https://egov.uscis.gov/casestatus/landing.do"));

            while(NavigateCompleted == false)
            {
                await Task.Delay(500);
            }
            
        }

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
            if (NavigateFailed == true)
            {
                throw new InvalidOperationException("WebView failed to navigate to tool. Please refresh.");
            }

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

            //Because the webView will be reload to a new one, reset status
            NavigateCompleted = false;
            NavigateFailed = false;

            //Invoke the button.
            string js = "var x = document.getElementsByTagName(\"input\");var i; for(i = 0; i < x.length; i++){ if(x[i].value == \"CHECK STATUS\") { break; } }; x[i].click()";
            string[] jsArgs = { js };
            string res = await CurrentWebView.InvokeScriptAsync("eval",jsArgs);

            //Wait until page is properly refreshed.
            while (NavigateCompleted == false)
            {
                await Task.Delay(500);
            }

        }


        /// <summary>
        /// Get the current page in the WebView HTML
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetCurrentPageHTML()
        {
            string js = "document.documentElement.innerHTML";
            string[] jsArgs = { js };
            string val = await CurrentWebView.InvokeScriptAsync("eval", jsArgs);
            return val;
        }
        #endregion

    }
}
