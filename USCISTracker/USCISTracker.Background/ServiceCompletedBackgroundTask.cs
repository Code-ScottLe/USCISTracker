using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace USCISTracker.Background
{
    public sealed class ServiceCompletedBackgroundTask : IBackgroundTask
    {
        BackgroundTaskDeferral deferral;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            deferral = taskInstance.GetDeferral();

            BackgroundExecutionManager.RemoveAccess();
            await BackgroundExecutionManager.RequestAccessAsync();

            //Write to setting to show as debug value
            var setting = Windows.Storage.ApplicationData.Current.LocalSettings;
            setting.Values["ServiceCompleted"] = $"Task ServicingCompleted finished running at {DateTime.Now.ToString()}";
            System.Diagnostics.Debug.WriteLine($"Task ServicingCompleted finished running at {DateTime.Now.ToString()}");

            deferral.Complete();
        }
    }
}
