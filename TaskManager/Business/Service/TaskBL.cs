using System.Security.Claims;
using TaskManager.Business.Interface;
using TaskManager.Models.DTOs.TaskDTO;
using TaskManager.Models.Entity;
using TaskManager.Repository.Interface;

namespace TaskManager.Business.Service
{
    public class TaskBL : ITaskBL
    {
        private readonly ITaskRepository _taskRepo;
        private readonly IHttpContextAccessor _httpContext;

        public TaskBL(ITaskRepository taskRepo, IHttpContextAccessor httpContext)
        {
            _taskRepo = taskRepo;
            _httpContext = httpContext;

        }

        private int GetCurrentUserId()
        {
            var userIdClaim = _httpContext.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("User ID not found in token");
            return int.Parse(userIdClaim);
        }

        public TaskM CreateTask(CreateTaskDTO createDto)
        {
            var userId = GetCurrentUserId();
            var task = new TaskM
            {
                Title = createDto.Title,
                Description = createDto.Description,
                UserId = userId,
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            };
            return _taskRepo.CreateTask(task);
        }


        public TaskM GetTaskById(int id)
        {
            
            var task =  _taskRepo.GetTaskById(id);
            if (task == null)
                throw new KeyNotFoundException("Task not found");

            var userId = GetCurrentUserId();
            var isAdmin = _httpContext.HttpContext?.User?.IsInRole("Admin") ?? false;
            if (task.UserId != userId && !isAdmin)
                throw new UnauthorizedAccessException("You don't have permission to view this task");

            return task;
        }


        public List<TaskM> GetTasks()
        {
            var userId = GetCurrentUserId();
            return _taskRepo.GetTasks(userId);
        }

        public TaskM UpdateTask(int id,UpdateTaskDTO updateDto)
        {
            Console.WriteLine(updateDto);
            var existingTask = _taskRepo.GetTaskById(id);
            if (existingTask == null)
                throw new Exception("Task not found");

            var userId = GetCurrentUserId();
            if (existingTask.UserId != userId)
                throw new UnauthorizedAccessException("You don't have permission to update this task");
            if (!string.IsNullOrWhiteSpace(updateDto.Title))
                existingTask.Title = updateDto.Title;
            if (updateDto.Description != null) 
                existingTask.Description = updateDto.Description;
            if (updateDto.IsComplete.HasValue)
                existingTask.IsCompleted = updateDto.IsComplete.Value;

            return _taskRepo.UpdateTask(existingTask);
        }

        public bool DeleteTask(int id)
        {
            var task = _taskRepo.GetTaskById(id);
            if (task == null)
            {
                throw new Exception("Task not found");
            }
            var userId = GetCurrentUserId();
            if (task.UserId != userId)
            {
                throw new UnauthorizedAccessException("You don't have permission to delete this task");
            }
            return _taskRepo.DeleteTask(id);
        }
    }
}
