using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text;
using System.Text.Json;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PopulationAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PopulationController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        string apiUrl = "https://datausa.io/api/data?drilldowns=State&measures=Population&year=latest";

        private readonly ILogger<PopulationController> _logger;

        public PopulationController(ILogger<PopulationController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        //public IEnumerable<PopulationDetails> Get()
        public  void Get()
        {

            HttpClient client = new HttpClient();
            var response =  client.GetAsync(apiUrl).Result; //GetStringAsync(apiUrl).Result;
            var json = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine(json);
            var temp = JObject.Parse(json);
            JArray array = temp["links"] as JArray;
            
            
            Console.WriteLine(array);
            var options = new JsonSerializerOptions
            {
                IncludeFields = true,
            };
            List<PopulationDetails> populationDetails = array.ToObject<List<PopulationDetails>>();
                //JsonConvert.DeserializeObject<List<PopulationDetails>>(array);

            if (populationDetails.Count > 0)
            {
                foreach (PopulationDetails p in populationDetails)
                {
                    Console.WriteLine(p.Population);
                }
            }
            else
            {
                Console.WriteLine("No records found.");
            }
            Console.WriteLine();

          

            /* return Enumerable.Range(1, 5).Select(index => new WeatherForecast
             {
                 Date = DateTime.Now.AddDays(index),
                 TemperatureC = Random.Shared.Next(-20, 55),
                 Summary = Summaries[Random.Shared.Next(Summaries.Length)]
             })
             .ToArray();*/
        }
    }
}