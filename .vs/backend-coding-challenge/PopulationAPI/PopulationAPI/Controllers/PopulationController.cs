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

        [HttpGet("YearlyDifference/{stateA}/{stateB}/{year=latest}")]

        public IActionResult GetPopulationDifference(String stateA, String stateB, string year )
        {
            if (string.IsNullOrEmpty(stateA) || string.IsNullOrEmpty(stateB))
                throw new Exception("State should not be null");

            var populationDetails = GetPopulationDetails(year);

            if (populationDetails.Count() == 0)
            {
                return NotFound($"No data found for the year {year}");
            }

            var state1 = populationDetails.Where(s => s.State.ToLower() == stateA.ToLower()).FirstOrDefault();
            if (state1 == null) return NotFound($"State {stateA} not found");

            var state2 = populationDetails.Where(s => s.State.ToLower() == stateB.ToLower()).FirstOrDefault();
            if (state2 == null) return NotFound($"State {stateB} not found");


            LogData(Request.Path);

            return Ok( new { difference = Math.Abs(state1.Population - state2.Population) });


        }

        [HttpGet("LargestSmallest/{year=latest}")]

        public IActionResult GetLargestandSmallestPopulation(string year)
        {

            var sortedPopulationDetails = GetPopulationDetails(year).OrderByDescending(i => i.Population);
            if (sortedPopulationDetails.Count() == 0)
            {
                return NotFound($"No data found for the year {year}");
            }

            var largestState = sortedPopulationDetails.First();
            var smallestState = sortedPopulationDetails.Last();

            LogData(Request.Path);
           // await _context.Logs.AddAsync(new Model.Log { DateTime = DateTime.Now, QueryType = Request.Path });
            //await _context.SaveChangesAsync();

            var response = new { largestPopulation = new { state = largestState.State, population = largestState.Population },
                smallestPopulation = new { state= smallestState.State, population= smallestState.Population} };
            return Ok(response);


        }

        private async Task LogData(string queryType)
        {
            await _context.Logs.AddAsync(new Model.Log { DateTime = DateTime.Now, QueryType = queryType });
            await _context.SaveChangesAsync();

        }

        private IEnumerable<PopulationDetails> GetPopulationDetails(String year="latest")
        {
            string url = _configuration["Url"];
            url = url.Replace("{year}", year);
            string populationResponse = HttpHelper.Get(url);
            var populationData = ParsePopulationData(JObject.Parse(populationResponse));
            return populationData;
        }

        private List<PopulationDetails> ParsePopulationData(JObject populationData)
        {
            List<PopulationDetails> populationDetails = new List<PopulationDetails>();
            if (populationData.TryGetValue("data", out var value))
            {
                var valueAsArray = value.ToObject<PopulationDetails[]>();

                if (valueAsArray != null)
                {
                    populationDetails = valueAsArray.ToList<PopulationDetails>();
                }
            }

            return populationDetails;
        }
    }
}