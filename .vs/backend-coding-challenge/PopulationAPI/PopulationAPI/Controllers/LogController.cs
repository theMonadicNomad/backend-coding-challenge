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
        public async Task<string> GetLogDetails() { 

            var logs = await _context.Logs.ToListAsync();
            var response = string.Join("\n", logs.Select(s => $"{s.DateTime} - {s.QueryType}"));

            if (string.IsNullOrEmpty(response))
            {
                response = "No logs found!";
            }
            return response;
        }
    }
}
