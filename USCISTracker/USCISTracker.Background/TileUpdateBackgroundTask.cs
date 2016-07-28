using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USCISTracker.Data;
using USCISTracker.Services.TileServices;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI.Notifications;

namespace USCISTracker.Background
{
    public sealed class  TileUpdateBackgroundTask : IBackgroundTask
    {
        BackgroundTaskDeferral _deferral;
        private static string defaultSaveLocation = "USCISCasesJSON.json";

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();

            //Get the CaseList
            var cases = await GetSavedCasesAsync();

            //Update the tile
            UpdateTile(cases);

            //Write to setting for debug
            ApplicationData.Current.LocalSettings.Values["TileUpdate"] = $"Task Tile Update finished running at {DateTime.Now.ToString()}";
            System.Diagnostics.Debug.WriteLine($"Task Update Tile finished running at {DateTime.Now.ToString()}");

            _deferral.Complete();
        }



        /// <summary>
        /// Load up the saved files.
        /// </summary>
        /// <returns></returns>
        private async Task<List<Case>> GetSavedCasesAsync()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;

            StorageFile savedCasesStorageFile = await localFolder.TryGetItemAsync(defaultSaveLocation) as StorageFile;

            if (savedCasesStorageFile == null)
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
        /// Update the main tile with case status with most recent updated cases (5)
        /// </summary>
        private void UpdateTile(List<Case> Cases)
        {
            List<Case> tempCaseList = Cases.ToList();

            //Remove all the irrelavent case
            while (tempCaseList.Count > 5)
            {
                var older = tempCaseList.Min((s) => { return s.LastRefresh.ToFileTime(); });

                var olderCase = tempCaseList.Where(n => n.LastRefresh.ToFileTime() == older).Select(n => n).FirstOrDefault();
                tempCaseList.Remove(olderCase);
            }

            //Create the tile update
            List<NotificationsExtensions.Tiles.TileContent> tileUpdates = new List<NotificationsExtensions.Tiles.TileContent>();

            foreach (Case individualCase in tempCaseList)
            {
                tileUpdates.Add(TileService.CreateAdaptiveMainTileContent(individualCase));
            }

            //Call the updater
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();

            //Enable the queue
            updater.EnableNotificationQueue(true);

            //remove old 
            updater.Clear();

            //push it.
            foreach (var tileUpdate in tileUpdates)
            {
                updater.Update(new TileNotification(tileUpdate.GetXml()));
            }


        }
    }
}
