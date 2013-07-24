using System;
using System.Collections.Generic;
using System.Linq;
using eCentral.Core;
using eCentral.Core.Data;
using eCentral.Core.Domain.Tasks;

namespace eCentral.Services.Tasks
{
    /// <summary>
    /// Task service
    /// </summary>
    public partial class ScheduleTaskService : IScheduleTaskService
    {
        #region Fields

        private readonly IRepository<ScheduleTask> taskRepository;

        #endregion

        #region Ctor

        public ScheduleTaskService(IRepository<ScheduleTask> taskRepository)
        {
            this.taskRepository = taskRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a task
        /// </summary>
        /// <param name="task">Task</param>
        public virtual void Delete(ScheduleTask task)
        {
            Guard.IsNotNull(task, "task");

            taskRepository.Delete(task);
        }

        /// <summary>
        /// Gets a task
        /// </summary>
        /// <param name="taskId">Task identifier</param>
        /// <returns>Task</returns>
        public virtual ScheduleTask GetById(Guid taskId)
        {
            if (taskId.IsEmpty())
                return null;

            return taskRepository.GetById(taskId);
        }

        /// <summary>
        /// Gets a task by its type
        /// </summary>
        /// <param name="type">Task type</param>
        /// <returns>Task</returns>
        public virtual ScheduleTask GetByType(string type)
        {
            if (String.IsNullOrWhiteSpace(type))
                return null;

            var query = taskRepository.Table;
            query     = query.Where(st => st.Type == type);
            query     = query.OrderByDescending(t => t.RowId);

            var task = query.FirstOrDefault();
            return task;
        }

        /// <summary>
        /// Gets all tasks
        /// </summary>
        /// <returns>Tasks</returns>
        public virtual IList<ScheduleTask> GetAll()
        {
            var query = taskRepository.Table;
            query     = query.OrderByDescending(t => t.Seconds);

            var tasks = query.ToList();
            return tasks;
        }

        /// <summary>
        /// Inserts a task
        /// </summary>
        /// <param name="task">Task</param>
        public virtual void Insert(ScheduleTask task)
        {
            Guard.IsNotNull(task, "task");

            taskRepository.Insert(task);
        }

        /// <summary>
        /// Updates the task
        /// </summary>
        /// <param name="task">Task</param>
        public virtual void Update(ScheduleTask task)
        {
            Guard.IsNotNull(task, "task");

            taskRepository.Update(task);
        }

        #endregion
    }
}
