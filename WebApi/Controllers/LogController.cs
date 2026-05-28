using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly RwaProjektContext _context;

        public LogController(RwaProjektContext context)
        {
            _context = context;
        }

        [HttpGet("get/{n}")]
        public ActionResult<IEnumerable<Log>> GetLogs(int n)
        {
            try
            {
                var logs = _context.Logs
                    .OrderByDescending(x => x.Timestamp)
                    .Take(n)
                    .ToList();
                return Ok(logs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("count")]
        public ActionResult<int> GetCount()
        {
            try
            {
                return Ok(_context.Logs.Count());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}