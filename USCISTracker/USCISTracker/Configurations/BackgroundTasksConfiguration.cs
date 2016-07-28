using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USCISTracker.Configurations
{
    public class BackgroundTasksConfiguration
    {
        //Background Task : Case Update
        public const string CaseUpdateBackgroundTaskName = "CaseUpdate";
        public const string CaseUpdateBackgroundTaskEntryPoint = "USCISTracker.Background.CaseUpdateBackgroundTask";


        //Background Task: ServiceComplete
        public const string ServiceCompletedBackgroundTaskName = "ServiceCompleted";
        public const string ServiceCompletedBackgroundTaskEntryPoint = "USCISTracker.Background.ServiceCompletedBackgroundTask";

        //Background Task: TileService
        public const string TileUpdateBackgroundTaskName = "TileUpdate";
        public const string TileUpdateBackgroundTaskEntryPoint = "USCISTracker.Background.TileUpdateBackgroundTask";

        //Time

#if DEBUG
        public const int TimeUpdateInterval = 15;

#else
        public const int TimeUpdateInterval = 60;
#endif

    }
}
