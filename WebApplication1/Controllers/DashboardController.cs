using DatabaseConnection;
using IRepositoryService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;

        public DashboardController(IUnitOfWork unitOfWork, ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var tripsCount = _unitOfWork.Trips.Count();
            var bookingsCount = _unitOfWork.Books.Count();
            var governmentCount = _unitOfWork.Governments.Count();
            var driversCount = _unitOfWork.Drivers.Count();
            var sitesCount = _unitOfWork.Sites.Count();
            var transportationsCount = _unitOfWork.Transportations.Count();
            var postsCount = _context.BlogPosts.Count();

            var totalRevenue = _unitOfWork.Books
                .FindAll(b => true)
                .Sum(b => b.AmountMoney);

            return Ok(new
            {
                TripsCount = tripsCount,
                BookingsCount = bookingsCount,
                GovernmentCount = governmentCount,
                TotalRevenue = totalRevenue,
                DriversCount = driversCount,
                SitesCount = sitesCount,
                TransportationsCount = transportationsCount,
                PostsCount = postsCount
            });
        }

        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentBookings()
        {
            var recent = _unitOfWork.Books
                .GetAllWith(new[] { "Trip" })
                .OrderByDescending(b => b.DateBook)
                .Take(5)
                .Select(b => new
                {
                    b.BookId,
                    b.DateBook,
                    b.NumberPeople,
                    b.AmountMoney,
                    TripName = b.Trip.Name
                });

            return Ok(recent);
        }




    }
}
