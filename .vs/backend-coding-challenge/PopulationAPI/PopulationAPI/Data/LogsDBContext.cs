using Microsoft.EntityFrameworkCore;
using PopulationAPI.Model;

namespace PopulationAPI.Data
{
    public class LogsDBContext: DbContext
    {
        public LogsDBContext(DbContextOptions<LogsDBContext> options) :base(options)
        {

        }

    public DbSet<Log> Logs { get; set; }
    }
}
