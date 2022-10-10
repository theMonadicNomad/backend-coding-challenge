using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using PopulationAPI.Data;
using PopulationAPI.Model;
using System.Text.Json;

namespace PopulationAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class LogController : ControllerBase
    {

        private readonly LogsDBContext _context;
        public LogController(LogsDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<Log>> GetLogDetails() => await _context.Logs.ToListAsync();

    }
}
