using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text;
using System.Text.Json;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.VisualBasic;
using PopulationAPI.Data;
//using System.Web.Http;
//using System.Web.Http;

namespace PopulationAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PopulationController : ControllerBase
    {
       

        string apiUrl = "https://datausa.io/api/data?drilldowns=State&measures=Population&year=latest";

        private readonly ILogger<PopulationController> _logger;
        private readonly LogsDBContext _context;

        public PopulationController(LogsDBContext context, ILogger<PopulationController> logger)
        {
            _logger = logger;
            _context = context;
        }


        public IEnumerable<PopulationDetails> GetPopulationDetails()
        {

            HttpClient client = new HttpClient();
            var response =  client.GetAsync(apiUrl).Result; 
            var json = response.Content.ReadAsStringAsync().Result;
            //Console.WriteLine(json);
            var temp = JObject.Parse(json);
            JArray array = temp["data"] as JArray;
            
            
            //Console.WriteLine(array);
            var options = new JsonSerializerOptions
            {
                IncludeFields = true,
            };
            List<PopulationDetails> populationDetails = array.ToObject<List<PopulationDetails>>();

            

            //JsonConvert.DeserializeObject<List<PopulationDetails>>(array);

            return populationDetails;
          


        }

        [HttpGet("{stateA}/{stateB}")]
        //public IEnumerable<PopulationDetails> Get()
        public async Task<IActionResult> GetPopulationDifference(String stateA, String stateB)
        {

            var populationDetails = GetPopulationDetails();

            var item1 = populationDetails.SingleOrDefault(p => p.State == stateA);
            var item2 = populationDetails.SingleOrDefault(p => p.State == stateB);
            Console.WriteLine(item1.Population);
            Console.WriteLine (item2?.Population);
            Console.WriteLine(item1.Population-item2.Population);


            await _context.Logs.AddAsync(new Model.Log {  DateTime = DateTime.Now, QueryType = "GetPopulationDetails/" + stateA+ "/" + stateB });
            await _context.SaveChangesAsync();

            return Ok(item1.Population - item2.Population);


        }

        [HttpGet()]
        //public IEnumerable<PopulationDetails> Get()
        public async Task<IActionResult> GetBigSmall()
        {

            var populationDetails = GetPopulationDetails();
            Console.WriteLine("From big small");
            var largestItem = populationDetails.OrderByDescending(i => i.Population).First();
            Console.WriteLine(largestItem.State);
            var smallestItem = populationDetails.OrderByDescending(i => i.Population).Last();
            Console.WriteLine(smallestItem.State);

            await _context.Logs.AddAsync(new Model.Log {  DateTime = DateTime.Now, QueryType = "GetBigSmall" });
            await _context.SaveChangesAsync();

            //return Ok(largestItem.State);
            return Ok(new { largestPopulation = largestItem.State, smallestPopulation = smallestItem.State });    
           // return Ok(new { big = largestItem.State, small = smallestItem.State });

        }


    }
}