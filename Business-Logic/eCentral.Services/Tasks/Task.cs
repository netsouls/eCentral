using System;
using System.Diagnostics;
using eCentral.Core.Domain.Tasks;
using eCentral.Core.Infrastructure;
using eCentral.Services.Logging;

namespace eCentral.Services.Tasks
{
    /// <summary>
    /// Task
    /// </summary>
    public partial class Task
    {
        private bool _enabled;
        private readonly string _type;
        private readonly string _name;
        private readonly bool _stopOnError;
        private DateTime? _lastStartDate;
        private DateTime? _lastSuccessDate;
        private DateTime? _lastEndDate;
        private bool _isRunning;

        /// <summary>
        /// Ctor for Task
        /// </summary>
        private Task()
        {
            this._enabled = true;
        }

        /// <summary>
        /// Ctor for Task
        /// </summary>
        /// <param name="task">Task </param>
        public Task(ScheduleTask task)
        {
            this._type        = task.Type;
            this._enabled     = task.Enabled;
            this._stopOnError = task.StopOnError;
            this._name        = task.Name;
        }

        private ITask CreateTask()
        {
            ITask task = null;
            if (this.Enabled)
            {
                var type2 = System.Type.GetType(this._type);
                if (type2 != null)
                {
                    task = Activator.CreateInstance(type2) as ITask;
                }
                //this._enabled = task != null;
            }
            return task;
        }
        
        /// <summary>
        /// Executes the task
        /// </summary>
        public void Execute()
        {
            this._isRunning = true;
            try
            {
                var task = this.CreateTask();
                if (task != null)
                {
                    this._lastStartDate = DateTime.UtcNow;
                    task.Execute();
                    this._lastEndDate = this._lastSuccessDate = DateTime.UtcNow;
                }
            }
            catch (Exception exc)
            {
                this._enabled     = !this.StopOnError;
                this._lastEndDate = DateTime.UtcNow;
                
                //log error
                var logger = EngineContext.Current.Resolve<ILogger>();
                logger.Error(string.Format("Error while running the '{0}' schedule task. {1}", this._name, exc.Message), exc);
            }
            
            try
            {
                //find current schedule task
                var scheduleTaskService = EngineContext.Current.Resolve<IScheduleTaskService>();
                var scheduleTask = scheduleTaskService.GetByType(this._type);
                if (scheduleTask != null)
                {
                    scheduleTask.LastStarDate    = this.LastStartDate;
                    scheduleTask.LastEndDate     = this.LastEndDate;
                    scheduleTask.LastSuccessDate = this.LastSuccessDate;
                    scheduleTaskService.Update(scheduleTask);
                }
            }
            catch (Exception exc)
            {
                Debug.WriteLine(string.Format("Error saving schedule task datetimes. Exception: {0}", exc));
            }
            this._isRunning = false;
        }

        /// <summary>
        /// A value indicating whether a task is running
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return this._isRunning;
            }
        }

        /// <summary>
        /// Datetime of the last start
        /// </summary>
        public DateTime? LastStartDate
        {
            get
            {
                return this._lastStartDate;
            }
        }

        /// <summary>
        /// Datetime of the last end
        /// </summary>
        public DateTime? LastEndDate
        {
            get
            {
                return this._lastEndDate;
            }
        }

        /// <summary>
        /// Datetime of the last success
        /// </summary>
        public DateTime? LastSuccessDate
        {
            get
            {
                return this._lastSuccessDate;
            }
        }

        /// <summary>
        /// A value indicating type of the task
        /// </summary>
        public string Type
        {
            get
            {
                return this._type;
            }
        }

        /// <summary>
        /// A value indicating whether to stop task on error
        /// </summary>
        public bool StopOnError
        {
            get
            {
                return this._stopOnError;
            }
        }

        /// <summary>
        /// Get the task name
        /// </summary>
        public string Name
        {
            get
            {
                return this._name;
            }
        }

        /// <summary>
        /// A value indicating whether the task is enabled
        /// </summary>
        public bool Enabled
        {
            get
            {
                return this._enabled;
            }
        }
    }
}
