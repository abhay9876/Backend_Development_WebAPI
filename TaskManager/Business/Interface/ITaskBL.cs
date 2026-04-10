using TaskManager.Models.DTOs.TaskDTO;
using TaskManager.Models.Entity;

namespace TaskManager.Business.Interface
{
    public interface ITaskBL
    {
        TaskM CreateTask(CreateTaskDTO createDto);
        TaskM GetTaskById(int userId);
        List<TaskM> GetTasks();
        TaskM UpdateTask(int id,UpdateTaskDTO updateDto);
        bool DeleteTask(int id);

    }
}
