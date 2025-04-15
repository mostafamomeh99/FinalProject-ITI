using DatabaseConnection;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace WebApplication1.BackGroundJobs
{
    public class DailyJob
    {
        private readonly ApplicationDbContext DbContext;

        public DailyJob(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public async Task Run()
        {
            try { 
            var trips =await DbContext.Trips.Where
                (e => e.EndDate < DateTime.UtcNow && e.OutOfDate==false)
                .ToListAsync();
            foreach(var trip in trips)
            {
               
                    trip.OutOfDate = true;
                Console.WriteLine($"Job executed at: {DateTime.Now}");
            }
            await DbContext.SaveChangesAsync();
            }
            catch
            {
                Console.WriteLine($"Job failed at: {DateTime.Now}");

            }
        }
    }
}
