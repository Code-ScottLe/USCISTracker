using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USCISTracker.Data
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
            CurrentWebView.Navigate(new Uri("https://egov.uscis.gov/casestatus/landing.do"));

            //Subscribe to the navigation success event. WebView goes from NavStart => ContentLoading => DOMContentLoaded => NavCompleted
            //More info: https://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.controls.webview.aspx
            CurrentWebView.NavigationCompleted += this.CurrentWebView_NavigationCompleted;

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
            if(args.IsSuccess == false)
            {
                //navigation failed for any reason. raise exception.  
                throw new OperationCanceledException($"WebView navigate failed with error: {args.WebErrorStatus.ToString()}");
            }

            else
            {
                //Navigation success
            }
        }
        #endregion

    }
}
