using TaskManager.Models.Entity;

namespace TaskManager.Repository.Interface
{
    public interface ITaskRepository
    {
        TaskM CreateTask(TaskM task);
        TaskM GetTaskById(int id);
        List<TaskM> GetTasks(int userId);
        TaskM UpdateTask(TaskM task);   //---> 
        bool DeleteTask(int id);

        List<TaskM> GetAllTasks(); // For admin

    }
}
