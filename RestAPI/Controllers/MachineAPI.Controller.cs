using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestAPI.Models;
using RestAPI.Data;

namespace RestAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly ApiContext _context;

        public JobsController(ApiContext context)
        {
            _context = context;
        }

        //Create
        [HttpPost]
        public JsonResult Create([FromBody] Job job)
        {
            _context.Jobs.Add(job);
            _context.SaveChanges();
            return new JsonResult(new { message = "Job created successfully", job });

        }

        //Edit
        [HttpPut("{id}")]
        public JsonResult Edit(int id, [FromBody] Job updatedJob)
        {
            var job = _context.Jobs.Find(id);
            if (job == null)
            {
                return new JsonResult(new { message = "Job not found" }) { StatusCode = StatusCodes.Status404NotFound };
            }

            job.JobName = updatedJob.JobName;
            job.Product = updatedJob.Product;
            job.Quantity = updatedJob.Quantity;
            job.CurrentState = updatedJob.CurrentState;
            job.MachineID = updatedJob.MachineID;

            _context.SaveChanges();
            return new JsonResult(new { message = "Job updated successfully", job });
        }

        //GetJob
        [HttpGet("{id}")]
        public ActionResult<Job> GetJob(int id)
        {
            var job = _context.Jobs.Find(id);
            if (job == null)
            {
                return NotFound();
            }
            return Ok(job);
        }

        //Delete
        [HttpDelete ("{id}")]
        public JsonResult Delete(int id)
        {
            var job = _context.Jobs.Find(id);
            if (job == null)
            {
                return new JsonResult(new { message = "Job not found" }) { StatusCode = StatusCodes.Status404NotFound };
            }
            _context.Jobs.Remove(job);
            _context.SaveChanges();
            return new JsonResult(new { message = "Job deleted successfully " });
        }

        // Get All Jobs
        [HttpGet]
        public ActionResult<IEnumerable<Job>> GetAll()
        {
            var jobs = _context.Jobs.ToList();
            return Ok(jobs);
        }
    }
}
