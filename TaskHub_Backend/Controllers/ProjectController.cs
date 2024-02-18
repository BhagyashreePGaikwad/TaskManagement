using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using taskHub.Context;
using taskHub.Model;

namespace taskHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        public IConfiguration _configuration;
        private readonly TestContext _projectContext;


        public ProjectController(IConfiguration configuration, TestContext projectContext)
        {
            _projectContext = projectContext;
            _configuration = configuration;
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetProject(int id)
        {
            if (_projectContext.Project == null)
            {
                return NotFound();
            }
            var u = await _projectContext.Project.FindAsync(id);
            if (u == null)
            {
                return NotFound();
            }
            return u;
        }




        [HttpPost]
        public async Task<ActionResult<Response>> PostRole(Project proj)
        {
            var response = new Response(); 

            try
            {
                var existingProject = await _projectContext.Project.FirstOrDefaultAsync(p => p.ProjectName == proj.ProjectName);

                if (existingProject != null)
                {
                    
                    response.StatusCode = StatusCodes.Status409Conflict; 
                    response.StatusMessage = "Project already exists";
                }
                else
                {
                    
                    _projectContext.Project.Add(proj);
                    await _projectContext.SaveChangesAsync();

                  
                    response.StatusCode = StatusCodes.Status201Created; 
                    response.StatusMessage = "Project added successfully";
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during the database operation

                // Set the response properties for failure
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.StatusMessage = "Internal server error: " + ex.Message;
            }

            return response;
        }


        [HttpGet]
        public ActionResult<IEnumerable<Project>> GetProject()
        {
            try
            {
                var proj = _projectContext.Project.ToList(); 
                return Ok(proj);
            }
            catch (Exception ex)
            {
                
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { StatusCode = StatusCodes.Status500InternalServerError, StatusMessage = "Internal server error: " + ex.Message });
            }

        }


        [HttpGet("GetProjectId/{projectName}")]
        public async Task<ActionResult<int>> GetProjectIdByName(string projectName)
        {
            try
            {
                var project = await _projectContext.Project.FirstOrDefaultAsync(p => p.ProjectName == projectName);

                if (project == null)

                {
                    return NotFound(); 
                }

                return Ok(project.Id); 
            }
            catch (Exception ex)
            {
               
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error: " + ex.Message);
            }
        }





    }
   
}