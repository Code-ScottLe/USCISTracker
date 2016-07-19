﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using USCISTracker.Data;
using USCISTracker.API;
using Windows.Storage;
using Newtonsoft.Json;

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

            //Get cases
            List<Case> cases = await GetSavedCasesAsync();

            if(cases != null)
            {
                //Try to see which one hasn't been updated in the longest.
                var earliestTime = cases.Min(n => { return n.LastRefresh.ToFileTime(); });

                var updateMeCase = cases.Where(n => n.LastRefresh.ToFileTime() == earliestTime).Select(n => n).FirstOrDefault();

                if (updateMeCase != null)
                {
                    var result = await SyncCaseStatusAsync(updateMeCase);
                    
                    if(result != null)
                    {
                        await BackupCasesAsync(cases);
                    }
                }
            }

                      
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
            await localSession.ConnectAsync();

            if (localSession.NavigateFailed == true)
            {
                return null;
            }
            

            //Check 
            await localSession.SetReceiptNumberAsync(checkingCase.ReceiptNumber.ReceiptNumber);
            await localSession.CheckCaseStatusAsync();
            string html = await localSession.GetCurrentPageHTML();
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


        #endregion
    }
}