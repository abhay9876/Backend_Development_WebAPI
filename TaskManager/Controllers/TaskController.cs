using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Business.Interface;
using TaskManager.Models.DTOs.TaskDTO;

namespace TaskManager.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskBL _taskBl;
        public TaskController(ITaskBL taskBl)
        {
            _taskBl = taskBl;
        }
        [HttpPost("create-task")]
        public IActionResult CreateTask([FromBody] CreateTaskDTO createDto)
        {
            try
            {
                var result = _taskBl.CreateTask(createDto);
                return CreatedAtAction(nameof(GetTaskById), new { id = result.Id },
                                    new { success = true, data = result, message = "Task created successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Error while Task Creating", error = ex.Message });
            }
        }

        [HttpGet("getAllTasks")]
        public IActionResult GetAllTasks()
        {
            try
            {
                var tasks = _taskBl.GetTasks();
                return Ok(new { success = true, data = tasks, message = "Tasks fetched successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("task/{id}")]
        public IActionResult GetTaskById(int id)
        {
            try
            {
                var result = _taskBl.GetTaskById(id);
                return Ok(new { success = true, data = result, message = "Task Fetch " });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "" });
            }
        }

        [HttpPut("update/{id}")]
        public IActionResult UpdateTask(int id, [FromBody] UpdateTaskDTO updateDto)
        {
            try
            {
                var task = _taskBl.UpdateTask(id, updateDto);
                return Ok(new { success = true, data = task, message = "Task updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }


        [HttpPut("complete/{id}")]
        public IActionResult MarkComplete(int id)
        {
            try
            {
                var existingTask = _taskBl.GetTaskById(id);

                if (existingTask == null)
                    return NotFound(new { success = false, message = "Task not found" });

                var updateDto = new UpdateTaskDTO { IsComplete = !existingTask.IsCompleted };
                var task = _taskBl.UpdateTask(id, updateDto);
                return Ok(new { success = true, data = task, message = "Task marked as completed" });
            }

            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("delete/{id}")]
        public IActionResult DeleteTask(int id)
        {
            try
            {
                var deleted = _taskBl.DeleteTask(id);
                if (deleted)
                    return Ok(new { success = true, message = "Task deleted successfully" });
                else
                    return NotFound(new { success = false, message = "Task not found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }



    }
}
