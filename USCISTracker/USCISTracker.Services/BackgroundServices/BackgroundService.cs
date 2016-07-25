using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace USCISTracker.Services.BackgroundServices
{
    public class BackgroundService
    {
        /// <summary>
        /// Check if a Task is registered with the OS
        /// </summary>
        /// <param name="taskName"></param>
        /// <returns></returns>
        public static bool IsTaskRegistered(string taskName)
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == taskName)
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskName"></param>
        /// <returns></returns>
        public static IBackgroundTaskRegistration GetBackgroundTask(string taskName)
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == taskName)
                {
                    return task.Value;
                }
            }

            return null;
        }

        /// <summary>
        /// Register a background task to the OS
        /// </summary>
        /// <param name="taskName">name of the background task</param>
        /// <param name="taskEntryPoint">entry point of the background task. Format: {namespace}.{backgroundTaskClass}. I.E: (Tasks.SampleBackgroundTask)</param>
        /// <param name="taskTrigger">trigger to fire up the background task.</param>
        /// <param name="taskCondition"> [optional] condition for the task to run. is null by default</param>
        /// <returns></returns>
        public async static Task<BackgroundTaskRegistration> RegisterBackgroundTask(string taskName, string taskEntryPoint, IBackgroundTrigger taskTrigger, IBackgroundCondition taskCondition = null)
        {
            //
            var taskRequired = await BackgroundExecutionManager.RequestAccessAsync();
            //Get a background task builder
            BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder();

            //Set the name and entry point for the background task
            taskBuilder.Name = taskName;
            taskBuilder.TaskEntryPoint = taskEntryPoint;

            //Set the trigger to fire up the background task
            taskBuilder.SetTrigger(taskTrigger);

            //set the condition if we have it
            if (taskCondition != null)
            {
                taskBuilder.AddCondition(taskCondition);
            }

            //register the task.
            var backgroundTask = taskBuilder.Register();

            return backgroundTask;
        }


        /// <summary>
        /// Unregister a given background task
        /// </summary>
        /// <param name="taskName"></param>
        public static void UnregisterBackgroundTask(string taskName)
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == taskName)
                {
                    task.Value.Unregister(true);
                }
            }
        }
    }
}
