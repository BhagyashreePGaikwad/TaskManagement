using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using taskHub.Context;
using taskHub.Model;

namespace taskHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskTypeController : ControllerBase
    {

        public IConfiguration _configuration;
        private readonly TestContext _taskCContext;


        public TaskTypeController(IConfiguration configuration, TestContext taskCContext)
        {
            _taskCContext = taskCContext;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<ActionResult<Response>> PostTaskType(TaskCategory taskC)
        {
            var response = new Response(); 
            try
            {
               
                var existingProject = await _taskCContext.TaskCategory.FirstOrDefaultAsync(p => p.TaskType == taskC.TaskType && p.ProjectId==taskC.ProjectId);

                if (existingProject != null)
                {
                   
                    response.StatusCode = StatusCodes.Status409Conflict;
                    response.StatusMessage = "Project already exists";
                }
                else
                {
                    
                    _taskCContext.TaskCategory.Add(taskC);
                    await _taskCContext.SaveChangesAsync();

                    response.StatusCode = StatusCodes.Status201Created;
                    response.StatusMessage = "Project added successfully";
                }
            }
            catch (Exception ex)
            {
              
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.StatusMessage = "Internal server error: " + ex.Message;
            }

            return response;
        }

        [HttpGet]
        public ActionResult<IEnumerable<TaskCategory>> GetTaskCategory()
        {
            try
            {
                var taskl = _taskCContext.TaskCategory.ToList(); 

                return Ok(taskl);
            }
            catch (Exception ex)
            {
              
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { StatusCode = StatusCodes.Status500InternalServerError, StatusMessage = "Internal server error: " + ex.Message });
            }

        }


        [HttpGet("{ProjectId}")]
        public ActionResult<IEnumerable<Project>> GetTaskTypeList(int ProjectId)
        {
            try
            {
                var proj = _taskCContext.TaskCategory.Where(e => e.ProjectId == ProjectId).ToList();  
                return Ok(proj);
            }
            catch (Exception ex)
            {
               
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { StatusCode = StatusCodes.Status500InternalServerError, StatusMessage = "Internal server error: " + ex.Message });
            }

        }



        [HttpGet("GetTaskCatId/{TaskType}/{ProjectId}")]
        public async Task<ActionResult<int>> GetTaskTypeIdByName(string TaskType, int ProjectId)
        {
            try
            {
                var task = await _taskCContext.TaskCategory.FirstOrDefaultAsync(p => p.TaskType == TaskType && p.ProjectId == ProjectId);

                if (task == null)

                {
                    return NotFound(); 
                }

                return Ok(task.Id);
            }
            catch (Exception ex)
            {
               
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error: " + ex.Message);
            }
        }

    }
}