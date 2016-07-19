using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;

namespace USCISTracker.Models
{
    public class BackgroundTasksConfiguration
    {
        #region Fields
        public const string CaseUpdateBackgroundTaskEntryPoint = "USCISTracker.Background.CaseUpdateBackgroundTask";
        public const string CaseUpdateBackgroundTaskName = "CaseUpdateBackgroundTask";
        public static string CaseUpdateBackgroundTaskProgress = "";
        public static bool CaseUpdateBackgroundTaskRegistered = false;

        #endregion


        #region Methods
        /// <summary>
        /// Register a background task with the specified taskEntryPoint, name, trigger,
        /// and condition (optional).
        /// </summary>
        /// <param name="taskEntryPoint">Task entry point for the background task.</param>
        /// <param name="name">A name for the background task.</param>
        /// <param name="trigger">The trigger for the background task.</param>
        /// <param name="condition">An optional conditional event that must be true for the task to fire.</param>
        public static BackgroundTaskRegistration RegisterBackgroundTask(string taskEntryPoint, string name, IBackgroundTrigger trigger, IBackgroundCondition condition)
        {
            // Register the Task
            // If the user denies access, the task will not run.
            var requestTask = BackgroundExecutionManager.RequestAccessAsync();


            //Create an instance of the Background task with the given info about the background task that we want to register.
            var builder = new BackgroundTaskBuilder();
            builder.Name = name;
            builder.TaskEntryPoint = taskEntryPoint;
            builder.SetTrigger(trigger);

            //Check if we have a condition to follow. ( internet available... etc)
            if (condition != null)
            {
                builder.AddCondition(condition);

                //
                // If the condition changes while the background task is executing then it will
                // be canceled.
                //
                builder.CancelOnConditionLoss = true;
            }


            //Register the task that we built.
            BackgroundTaskRegistration task = builder.Register();

            UpdateBackgroundTaskRegistrationStatus(name, true);

            //
            // Remove previous completion status.
            //
            //var settings = ApplicationData.Current.LocalSettings;
            //settings.Values.Remove(name);

            return task;
        }


        /// <summary>
        /// Store the registration status of a background task with a given name.
        /// </summary>
        /// <param name="name">Name of background task to store registration status for.</param>
        /// <param name="registered">TRUE if registered, FALSE if unregistered.</param>
        public static void UpdateBackgroundTaskRegistrationStatus(string name, bool registered)
        {
            switch (name)
            {
                case CaseUpdateBackgroundTaskName:
                    CaseUpdateBackgroundTaskRegistered = registered;
                    break;
            }
        }

        #endregion
    }
}
