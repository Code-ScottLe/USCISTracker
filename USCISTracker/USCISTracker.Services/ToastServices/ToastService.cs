using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotificationsExtensions;
using NotificationsExtensions.Toasts;

namespace USCISTracker.Services.ToastServices
{
    public class ToastService
    {
        public static ToastContent CreateGenericToast(string title, string message)
        {

            //Toast Content Layout
            ToastBindingGeneric mainContent = new ToastBindingGeneric()
            {
                Children =
                {
                    new AdaptiveText()
                    {
                        Text = title,
                        HintStyle = AdaptiveTextStyle.Base
                    },

                    new AdaptiveText()
                    {
                        Text = message,
                        HintStyle = AdaptiveTextStyle.BodySubtle
                    }

                }

            };


            //Toast Layout
            ToastContent content = new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    BindingGeneric = mainContent,

                },
                Audio = new ToastAudio()
                {
                    Src = new Uri("ms-winsoundevent:Notification.Reminder")
                }
            };


            return content;

        }
    }
}
