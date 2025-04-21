using IRepositoryService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.StoredprocMapping;
using RepositoryFactory;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Const;
using WebApplication1.Dtos.Trips;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripController : ControllerBase
    {

        private readonly IUnitOfWork unitOFWork;

        public TripController(IUnitOfWork unitOFWork)
        {
            this.unitOFWork = unitOFWork;
        }

        // for user
        [HttpGet]
        public async Task<IActionResult> GetTrips([FromQuery] int pagenumber)
        {
            if (pagenumber <= 0) { return BadRequest("Invalid page number."); }

            List<TripGetDto> trips = new List<TripGetDto>();
            var dbTrips = unitOFWork.Trips.GetAllWith(              
              skip: ((pagenumber - 1) * ConstantProject.NumberOfData), take: ConstantProject.NumberOfData);

            var ratings = await unitOFWork.Ratings.GetWithGroup(e => e.TripId).Select(
                group =>
              new
              {
                  value = new
                  {
                      average = group.Average(e => e.UserRate),
                      count = group.Count()
                  },
                  key = group.Key
              }
                ).ToDictionaryAsync(item => item.key, item => item.value);

            foreach (var trip in dbTrips)
            {
                trips.Add(new TripGetDto()
                {
                    TripId = trip.TripId,
                    Name = trip.Name,
                    Description = trip.Description,
                    StartDate = trip.StartDate,
                    EndDate = trip.EndDate,
                    Duration = trip.Duration,
                    Money = trip.Money,
                    TripRating =((ratings==null|| !ratings.ContainsKey(trip.TripId) )
                    ? 0: ratings[trip.TripId].average) ,
                    UserNumbersRating = ((ratings == null || !ratings.ContainsKey(trip.TripId))
                    ? 0 : ratings[trip.TripId].count),
                    AvailablePeople = trip.AvailablePeople,
                    MaxPeople = trip.MaxPeople,
                });
            }

            return Ok(trips);
        }

        [HttpGet("{tripId}")]
        public async Task<IActionResult> GetTripSiteDetails([FromRoute] string tripId)
        {
            var trip = unitOFWork.Trips.Findme(e => e.TripId== tripId);
            if (trip == null) {
                return BadRequest(new { message = "trip not found" }); }

            var tripwithdetails = await unitOFWork.TripSiteDetails
                .UseOurSql("GetTripSiteDetails", tripId);

            return Ok(tripwithdetails);
        }


        //[HttpGet("{id}")]
        //public IActionResult GetTripById([FromRoute] string tripid, [FromQuery] int pagenumber)
        //{


        //    return  _context.Set<TripSiteDetailDto>()
        //.FromSqlInterpolated($"EXEC GetTripSiteDetails @tripid = {tripId}")
        //.ToList();

        //    return Ok(trips);
        //}

        //// PUT: api/Trip
        //[HttpPut]
        //public IActionResult UpdateTrip([FromBody] TripUpdateDto tripDto)
        //{
        //    if (tripDto == null) { return BadRequest(new { message = "Invalid trip data." }); }

        //    var dbTrip = unitOFWork.Trips.Findme(e => e.TripId == tripDto.TripId);
        //    if (dbTrip == null) return BadRequest(new { message = "Trip not found." });

        //    dbTrip.Name = tripDto.Name;
        //    dbTrip.Description = tripDto.Description;
        //    dbTrip.StartDate = tripDto.StartDate;
        //    dbTrip.EndDate = tripDto.EndDate;
        //    dbTrip.Duration = tripDto.Duration;
        //    dbTrip.Money = tripDto.Money;
        //    dbTrip.AvailablePeople = tripDto.AvailablePeople;
        //    dbTrip.MaxPeople = tripDto.MaxPeople;
        //    dbTrip.IsDeleted = tripDto.IsDeleted ?? dbTrip.IsDeleted;
        //    dbTrip.OutOfDate = tripDto.OutOfDate ?? dbTrip.OutOfDate;

        //    // Update included items
        //    if (tripDto.IncludedItems != null)
        //    {
        //        dbTrip.IncludedItems = tripDto.IncludedItems.Select(i => new TripIncluded { Name = i }).ToList();
        //    }

        //    // Update excluded items
        //    if (tripDto.ExcludedItems != null)
        //    {
        //        dbTrip.ExcludedItems = tripDto.ExcludedItems.Select(i => new TripExcluded { Name = i }).ToList();
        //    }

        //    // Update sites
        //    if (tripDto.Sites != null)
        //    {
        //        dbTrip.Sites = tripDto.Sites.Select(s => new Site { SiteId = s }).ToList();
        //    }

        //    unitOFWork.Compelet(); // Save changes

        //    return Ok(new { message = "Trip updated successfully" });
        //}

        // DELETE: api/Trip/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteTrip([FromRoute] string id)
        {
            var trip = unitOFWork.Trips.Findme(e => e.TripId == id);
            if (trip == null) return BadRequest(new { message = "Trip not found." });

            unitOFWork.Trips.Delete(trip);
            unitOFWork.Compelet(); // Save changes

            return Ok(new { message = "Trip deleted successfully" });
        }


        /////////Get trip by id
        [HttpGet("single/{id}")]
        public IActionResult GetTripById([FromRoute] string id)
        {
            var trip = unitOFWork.Trips.Findme(e => e.TripId == id);

            if (trip == null)
                return NotFound(new { message = "Trip not found" });

            var result = new TripGetDto
            {
                TripId = trip.TripId,
                Name = trip.Name,
                Description = trip.Description,
                StartDate = trip.StartDate,
                EndDate = trip.EndDate,
                Duration = trip.Duration,
                Money = trip.Money,
                AvailablePeople = trip.AvailablePeople,
                MaxPeople = trip.MaxPeople,
                TripRating = 0,
                UserNumbersRating = 0
            };

            return Ok(result);
        }


        /// /////// Create a new trip
   
        [HttpPost]
        public IActionResult CreateTrip([FromBody] TripCreateDto tripDto)
        {
            if (tripDto == null)
            {
                return BadRequest(new { message = "Invalid trip data." });
            }
            // Validate required fields
            if (string.IsNullOrEmpty(tripDto.Name) || tripDto.StartDate == default || tripDto.EndDate == default)
            {
                return BadRequest(new { message = "Name, StartDate, and EndDate are required." });
            }

            var newTrip = new Trip
            {
                TripId = Guid.NewGuid().ToString(),
                Name = tripDto.Name,
                Description = tripDto.Description,
                StartDate = tripDto.StartDate,
                EndDate = tripDto.EndDate,
                Duration = tripDto.Duration,
                Money = tripDto.Money,
                AvailablePeople = tripDto.AvailablePeople,
                MaxPeople = tripDto.MaxPeople,
                IsDeleted = tripDto.IsDeleted ?? false,
                OutOfDate = tripDto.OutOfDate ?? false,
              
            };

            // Add included items
            if (tripDto.IncludedItems != null && tripDto.IncludedItems.Any())
            {
                newTrip.IncludedItems = tripDto.IncludedItems.Select(i => new TripIncluded
                {
                    Item = i
                }).ToList();
            }

            // Add excluded items
            if (tripDto.ExcludedItems != null && tripDto.ExcludedItems.Any())
            {
                newTrip.ExcludedItems = tripDto.ExcludedItems.Select(i => new TripExcluded
                {
                    Item = i
                }).ToList();
            }

            // Add sites

                if (tripDto.Sites != null && tripDto.Sites.Any())
                {
                    var sites = unitOFWork.Sites.FindAll(s => tripDto.Sites.Select(id => id.ToString()).Contains(s.SiteId)).ToList();
                    newTrip.Sites = sites;
                }
                
            try
            {
                unitOFWork.Trips.Addone(newTrip);
                unitOFWork.Compelet();

                return CreatedAtAction(nameof(GetTripById), new { id = newTrip.TripId },
                    new { message = "Trip created successfully", tripId = newTrip.TripId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the trip", error = ex.Message });
            }
        }

        //update trip
        [HttpPut]
        public IActionResult UpdateTrip([FromBody] TripUpdateDto tripDto)
        {
            if (tripDto == null)
            {
                return BadRequest(new { message = "Invalid trip data." });
            }

            var dbTrip = unitOFWork.Trips.Findme(e => e.TripId == tripDto.TripId);
            if (dbTrip == null)
            {
                return NotFound(new { message = "Trip not found." });
            }

            // Update basic properties
            dbTrip.Name = tripDto.Name ?? dbTrip.Name;
            dbTrip.Description = tripDto.Description ?? dbTrip.Description;
            dbTrip.StartDate = tripDto.StartDate;
            dbTrip.EndDate = tripDto.EndDate;
            dbTrip.Duration = tripDto.Duration;
            dbTrip.Money = tripDto.Money;
            dbTrip.AvailablePeople = tripDto.AvailablePeople;
            dbTrip.MaxPeople = tripDto.MaxPeople;
            dbTrip.IsDeleted = tripDto.IsDeleted ?? dbTrip.IsDeleted;
            dbTrip.OutOfDate = tripDto.OutOfDate ?? dbTrip.OutOfDate;

            // Update included items
            if (tripDto.IncludedItems != null)
            {
                // Clear existing included items
                unitOFWork.TripIncludeds.DeleteRange(dbTrip.IncludedItems);
                // Add new ones
                dbTrip.IncludedItems = tripDto.IncludedItems.Select(i => new TripIncluded
                {
                    Item = i,
                    TripId = dbTrip.TripId
                }).ToList();
            }

            // Update excluded items
            if (tripDto.ExcludedItems != null)
            {
                // Clear existing excluded items
                unitOFWork.TripExcludeds.DeleteRange(dbTrip.ExcludedItems);
                // Add new ones
                dbTrip.ExcludedItems = tripDto.ExcludedItems.Select(i => new TripExcluded
                {
                    Item = i,
                    TripId = dbTrip.TripId
                }).ToList();
            }

            // Update sites
            if (tripDto.Sites != null)
            {
                dbTrip.Sites = unitOFWork.Sites.FindAll(s => tripDto.Sites.Contains(s.SiteId)).ToList();
            }
            try
            {
                unitOFWork.Compelet();
                return Ok(new { message = "Trip updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the trip", error = ex.Message });
            }
        }



    }
}
