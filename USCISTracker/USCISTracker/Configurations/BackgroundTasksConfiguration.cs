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


        //Background Task: Tile Update
        public const string ServiceCompletedBackgroundTaskName = "ServiceCompleted";
        public const string ServiceCompletedBackgroundTaskEntryPoint = "USCISTracker.Background.ServiceCompletedBackgroundTask";

        //Background Task: ServiceComplete


    }
}
