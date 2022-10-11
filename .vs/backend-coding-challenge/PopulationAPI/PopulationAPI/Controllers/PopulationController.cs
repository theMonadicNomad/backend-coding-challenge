using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text;
using System.Text.Json;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.VisualBasic;
using PopulationAPI.Data;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PopulationAPI.Helper;
//sing System.Web.Http;
//using System.Web.Http;
//using System.Web.Http;
//using System.Web.Http;

namespace PopulationAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PopulationController : ControllerBase
    {




        private readonly ILogger<PopulationController> _logger;
        private readonly IConfiguration _configuration;
        private readonly LogsDBContext _context;

        public PopulationController(LogsDBContext context, ILogger<PopulationController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
        }

        [HttpGet("{stateA}/{stateB}")]

        public async Task<IActionResult> GetPopulationDifference(String stateA, String stateB)
        {
            if (string.IsNullOrEmpty(stateA) || string.IsNullOrEmpty(stateB))
                throw new Exception("State should not be null");

            var populationDetails = GetPopulationDetails();

            var state1 = populationDetails.Where(s => s.State.ToLower() == stateA.ToLower()).FirstOrDefault();
            var state2 = populationDetails.Where(s => s.State.ToLower() == stateB.ToLower()).FirstOrDefault();
            if (state1 == null || state2 == null) return NotFound();// throw new Exception("The given state doesnt found in the database");


             LogData("GetPopulationDetails/" + stateA + "/" + stateB );
/*            await _context.Logs.AddAsync(new Model.Log { DateTime = DateTime.Now, QueryType = "GetPopulationDetails/" + stateA + "/" + stateB });
            await _context.SaveChangesAsync();*/

            return Ok(Math.Abs(state1.Population - state2.Population));


        }

        [HttpGet()]

        public IActionResult GetLargestandSmallestPopulation()
        {

            var sortedPopulationDetails = GetPopulationDetails().OrderByDescending(i => i.Population);
            Console.WriteLine("From big small");

            var largestState = sortedPopulationDetails.First().State;
            var smallestState = sortedPopulationDetails.Last().State;

            LogData("GetLargestandSmallest");
            /*await _context.Logs.AddAsync(new Model.Log { DateTime = DateTime.Now, QueryType = "GetBigSmall" });
            await _context.SaveChangesAsync();*/


            return Ok(new { largestPopulation = largestState, smallestPopulation = smallestState });


        }

        private async void LogData(string queryType)
        {
            await _context.Logs.AddAsync(new Model.Log { DateTime = DateTime.Now, QueryType = queryType });
            await _context.SaveChangesAsync();

        }

        private IEnumerable<PopulationDetails> GetPopulationDetails()
        {
            string populationResponse = HttpHelper.Get(_configuration["Url"].ToString());
            var populationData = ParsePopulationData(JObject.Parse(populationResponse));
            return populationData;
        }

        private List<PopulationDetails> ParsePopulationData(JObject populationData)
        {
            List<PopulationDetails> populationDetails = new List<PopulationDetails>();
            if (populationData != null)
            {
                JArray? array = populationData["data"] as JArray;
                populationDetails = array.ToObject<List<PopulationDetails>>();
            }
            return populationDetails;
        }
    }
}