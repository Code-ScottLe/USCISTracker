using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using USCISTracker.Data;
using USCISTracker.API;
using Windows.Storage;
using Newtonsoft.Json;
using Windows.UI.Core;
using Windows.UI.Notifications;
using USCISTracker.Services.ToastServices;

namespace USCISTracker.Background
{
    public sealed class CaseUpdateBackgroundTask : IBackgroundTask
    {
        #region Fields
        //use to determine when the task is starting and completed
        BackgroundTaskDeferral _deferral;
        private static string defaultSaveLocation = "USCISCasesJSON.json";

        //Indicate error and reasons
        volatile bool cancelRequested = false;      //access from any thread
        BackgroundTaskCancellationReason cancelReason = BackgroundTaskCancellationReason.Abort;
        #endregion


        #region Methods

        /// <summary>
        /// Main for the Background Task. This is limited to 30s of execution
        /// </summary>
        /// <param name="taskInstance"></param>
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            //Defferal for the async methods
            _deferral = taskInstance.GetDeferral();

            //Register the OnCanceled event to handle the even that the background task was cancelled (usually on failed condition)
            taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(OnCanceled);

            //Get cases
            List<Case> cases = await GetSavedCasesAsync();

            if(cases != null)
            {
                //Try to see which one hasn't been updated in the longest.
                var earliestTime = cases.Min(n => { return n.LastRefresh.ToFileTime(); });

                var updateMeCase = cases.Where(n => n.LastRefresh.ToFileTime() == earliestTime).Select(n => n).FirstOrDefault();

                //Save the last updated file
                var lastCaseUpdate = updateMeCase.LastCaseUpdate;

                if (updateMeCase != null)
                {
                    var result = await SyncCaseStatusAsync(updateMeCase);
                    
                    if(result != null)
                    {
                        if(result.LastCaseUpdate.CompareTo(lastCaseUpdate) != 0)
                        {
                            //case was updated, send toast.
                            SendToast(result);
                        }
                        await BackupCasesAsync(cases);
                    }
                }
            }


            //Write to setting for debug
            ApplicationData.Current.LocalSettings.Values["CaseUpdate"] = $"Task Update Case finished running at {DateTime.Now.ToString()}";
            System.Diagnostics.Debug.WriteLine($"Task Update Case finished running at {DateTime.Now.ToString()}");

            //Done with async methods
            _deferral.Complete();
        }

        /// <summary>
        /// Event handler for the failed task.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="reason"></param>
        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            //Task has been cancelled for any reason. Indicate the request.
            cancelRequested = true;
            cancelReason = reason;
        }


        /// <summary>
        /// Load up the saved files.
        /// </summary>
        /// <returns></returns>
        private async Task<List<Case>> GetSavedCasesAsync()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;

            StorageFile savedCasesStorageFile = await localFolder.TryGetItemAsync(defaultSaveLocation) as StorageFile;

            if(savedCasesStorageFile == null)
            {
                return null;
            }

            else
            {
                string json = await FileIO.ReadTextAsync(savedCasesStorageFile);
                List<Case> cases = await Task.Run<List<Case>>(() => JsonConvert.DeserializeObject<List<Case>>(json));
                return cases;
            }
        }


        /// <summary>
        /// Check and update individual case
        /// </summary>
        /// <param name="checkingCase"> current case to check for status</param>
        /// <param name="currentSession"> connected sesion if we have one</param>
        /// <returns></returns>
        private async Task<Case> SyncCaseStatusAsync(Case checkingCase)
        {

            Session localSession = new Session();

            string html = await localSession.CheckCaseStatusAsync(checkingCase.ReceiptNumber.ReceiptNumber);
            await checkingCase.UpdateFromHTMLAsync(html);

            return checkingCase;

        }


        /// <summary>
        /// Back up all the tracking cases info into the JSON file
        /// </summary>
        /// <returns></returns>
        private async Task BackupCasesAsync(List<Case> Cases)
        {

            //serialize the collection
            string json = await Task.Run<string>(() => JsonConvert.SerializeObject(Cases, Formatting.Indented));

            //Get the local folder.
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;

            //Get the file, if not, create it.
            StorageFile localCaseFile = await localFolder.TryGetItemAsync(defaultSaveLocation) as StorageFile;

            if (localCaseFile == null)
            {
                localCaseFile = await localFolder.CreateFileAsync(defaultSaveLocation);
            }

            //Write to the file
            await FileIO.WriteTextAsync(localCaseFile, json);

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="updatedCase"></param>
        private void SendToast(Case updatedCase)
        {
            var ToastContent = ToastService.CreateGenericToast("Case was updated!", $"{updatedCase.Name} with Receipt #:{updatedCase.ReceiptNumber.ReceiptNumber} was updated on {updatedCase.LastCaseUpdate.ToString(@"MM\/dd\/yyyy")}");

            //send toast
            ToastNotificationManager.CreateToastNotifier().Show(new ToastNotification(ToastContent.GetXml()));
        }
        #endregion
    }
}
