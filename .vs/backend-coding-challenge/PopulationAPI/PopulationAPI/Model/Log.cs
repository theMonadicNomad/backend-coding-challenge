using System.ComponentModel.DataAnnotations;

namespace PopulationAPI.Model
{
    public class Log
    {
        public int Id { get; set; }

        [Required]
        public DateTime DateTime { get; set; }
        [Required]
        public string QueryType { get; set; }
    }
}
