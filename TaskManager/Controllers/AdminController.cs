using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Repository.Interface;


[ServiceFilter(typeof(SimpleFilter))]
[Authorize(Roles = "admin")]
[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly ITaskRepository _taskRepo;

    public AdminController(ITaskRepository taskRepo)
    {
        _taskRepo = taskRepo;
    }

    [HttpGet("tasks")]
    public IActionResult GetAllTasks()
    {
        try
        {
            var tasks = _taskRepo.GetAllTasks(); 
            return Ok(new { success = true, data = tasks });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}