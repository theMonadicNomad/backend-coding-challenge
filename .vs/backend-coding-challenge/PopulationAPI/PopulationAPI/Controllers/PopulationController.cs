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
            var parsed  = JObject.Parse(json);
//            JArray value = new JArray();
            //var value ="";
           if (parsed.TryGetValue("data", out var value ))
            {
                //JArray array = value as JArray;
               JArray array = value!=null ? value as JArray : new JArray();

                List<PopulationDetails> populationDetails = array.ToObject<List<PopulationDetails>>();
                return populationDetails;


            }

/*            if (parsed != null)
            {
                JArray? array = parsed["data"] as JArray;

                List<PopulationDetails> populationDetails = array.ToObject<List<PopulationDetails>>();

                /*var options = new JsonSerializerOptions
                {
                    IncludeFields = true,
                }; 

                //JsonConvert.DeserializeObject<List<PopulationDetails>>(array);

                return populationDetails;
            } */
            else return Enumerable.Empty<PopulationDetails>();


        }

        [HttpGet("{stateA}/{stateB}")]
        //public IEnumerable<PopulationDetails> Get()
        public async Task<IActionResult> GetPopulationDifference(String stateA, String stateB)
        {


            if (string.IsNullOrEmpty(stateA) || string.IsNullOrEmpty(stateB))
                throw new Exception("State should not be null");
            //new System.Web.Http.HttpResponseException(HttpStatusCode.NotFound);

            var populationDetails = GetPopulationDetails();



                var item1 = populationDetails.SingleOrDefault(p => p.State.ToLower() == stateA.ToLower());
                var item2 = populationDetails.SingleOrDefault(p => p.State.ToLower() == stateB.ToLower());
            if (item1 == null || item2 == null) return NotFound();// throw new Exception("The given state doesnt found in the database");

            Console.WriteLine(item1.Population);
                Console.WriteLine(item2.Population);
                Console.WriteLine(item1.Population - item2.Population);


                await _context.Logs.AddAsync(new Model.Log { DateTime = DateTime.Now, QueryType = "GetPopulationDetails/" + stateA + "/" + stateB });
                await _context.SaveChangesAsync();

                return Ok(item1.Population - item2.Population);


        }

        [HttpGet()]
        //public IEnumerable<PopulationDetails> Get()
        public async Task<IActionResult> GetBigSmall()
        {

            var sortedPopulationDetails = GetPopulationDetails().OrderByDescending(i =>i.Population);
            Console.WriteLine("From big small");

            var largestItem = sortedPopulationDetails.First();
            Console.WriteLine(largestItem.State);
            var smallestItem = sortedPopulationDetails.Last();
            Console.WriteLine(smallestItem.State);

            await _context.Logs.AddAsync(new Model.Log {  DateTime = DateTime.Now, QueryType = "GetBigSmall" });
            await _context.SaveChangesAsync();

         
            return Ok(new { largestPopulation = largestItem.State, smallestPopulation = smallestItem.State });    
         

        }


    }
}