using IRepositoryService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using RepositoryFactory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WebApplication1.Const;
using WebApplication1.Dtos.Trips;
using WebApplication1.Middlewares.ExceptionFeatures;

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

        [HttpGet]
        public IActionResult GetTrips([FromQuery] int pagenumber)
        {
            if (pagenumber <= 0) { return BadRequest(new { message = "Invalid page number." }); }

            List<TripGetDto> trips = new List<TripGetDto>();
            var dbTrips = unitOFWork.Trips.GetAllWith(
                new[] { "IncludedItems", "ExcludedItems", "Sites", "TripImages" },
                take: ConstantProject.NumberOfData,
                skip: ((pagenumber - 1) * ConstantProject.NumberOfData)
            );

            foreach (var trip in dbTrips)
            {
                var tripDto = new TripGetDto()
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
                    IncludedItems = trip.IncludedItems?.Select(i => i.Item).ToList() ?? new List<string>(),
                    ExcludedItems = trip.ExcludedItems?.Select(e => e.Item).ToList() ?? new List<string>(),
                    Sites = trip.Sites?.Select(s => s.SiteId).ToList() ?? new List<string>()
                };

                foreach (var img in trip.TripImages)
                {
                    tripDto.TripImages.Add(new TripImageDto()
                    {
                        Id = img.Id,
                        TripId = trip.TripId,
                        ImageUrl = $"{Request.Scheme}://{Request.Host}/images/{img.ImageName}"
                    });
                }
                trips.Add(tripDto);
            }

            return Ok(trips);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public IActionResult CreateTrip([FromForm] TripCreateDto tripDto)
        {
            if (tripDto == null)
            {
                return BadRequest(new { message = "Invalid trip data." });
            }

            if (tripDto.TripImages == null || !tripDto.TripImages.Any())
            {
                return BadRequest(new { message = "At least one image is required." });
            }

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
                var sites = unitOFWork.Sites.FindAll(s => tripDto.Sites.Contains(s.SiteId)).ToList();
                newTrip.Sites = sites;
            }

            try
            {
                unitOFWork.Trips.Addone(newTrip);
                unitOFWork.Compelet();

                // Handle images
                string imagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(imagesDirectory))
                {
                    Directory.CreateDirectory(imagesDirectory);
                }

                var tripImages = new List<TripImage>();
                foreach (var image in tripDto.TripImages)
                {
                    if (image.Length > 0)
                    {
                        string fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                        string filePath = Path.Combine(imagesDirectory, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            image.CopyTo(stream);
                        }

                        tripImages.Add(new TripImage()
                        {
                            ImageName = fileName,
                            TripId = newTrip.TripId
                        });
                    }
                }

                unitOFWork.TripImages.AddRange(tripImages);
                unitOFWork.Compelet();

                return CreatedAtAction(nameof(GetTripById), new { id = newTrip.TripId },
                    new { message = "Trip created successfully", tripId = newTrip.TripId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the trip", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetTripById([FromRoute] string id)
        {
            var trip = unitOFWork.Trips.Findme(e => e.TripId == id,
                new[] { "IncludedItems", "ExcludedItems", "Sites", "TripImages" });

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
                IncludedItems = trip.IncludedItems?.Select(i => i.Item).ToList() ?? new List<string>(),
                ExcludedItems = trip.ExcludedItems?.Select(e => e.Item).ToList() ?? new List<string>(),
                Sites = trip.Sites?.Select(s => s.SiteId).ToList() ?? new List<string>(),
            };

            foreach (var img in trip.TripImages)
            {
                result.TripImages
                    .Add(new TripImageDto()
                {
                    Id = img.Id,
                    TripId = trip.TripId,
                    ImageUrl = $"{Request.Scheme}://{Request.Host}/images/{img.ImageName}"
                });
            }

            return Ok(result);
        }

        [HttpPut]
        [Consumes("multipart/form-data")]
        public IActionResult UpdateTrip([FromForm] TripUpdateDto tripDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(e => e.Value.Errors.Count > 0)
                        .Select(e => new {
                            Field = e.Key,
                            Errors = e.Value.Errors.Select(error => error.ErrorMessage)
                        }).ToList();

                    return BadRequest(new
                    {
                        message = "Validation failed",
                        errors = errors
                    });
                }

                bool hasNewImages = tripDto.Images != null && tripDto.Images.Any();

                var dbTrip = unitOFWork.Trips.Findme(e => e.TripId == tripDto.TripId,
                    new[] { "IncludedItems", "ExcludedItems", "Sites", "TripImages" });
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
                    unitOFWork.TripIncludeds.DeleteRange(dbTrip.IncludedItems);
                    dbTrip.IncludedItems = tripDto.IncludedItems.Select(i => new TripIncluded
                    {
                        Item = i,
                        TripId = dbTrip.TripId
                    }).ToList();
                }

                // Update excluded items
                if (tripDto.ExcludedItems != null)
                {
                    unitOFWork.TripExcludeds.DeleteRange(dbTrip.ExcludedItems);
                    dbTrip.ExcludedItems = tripDto.ExcludedItems.Select(i => new TripExcluded
                    {
                        Item = i,
                        TripId = dbTrip.TripId
                    }).ToList();
                }

                // Update sites
                if (tripDto.Sites != null)
                {
                    var updatedSites = unitOFWork.Sites.FindAll(s => tripDto.Sites.Contains(s.SiteId)).ToList();
                    dbTrip.Sites = updatedSites;
                }

                // Handle images
                string imagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(imagesDirectory))
                {
                    Directory.CreateDirectory(imagesDirectory);
                }

                // Add new images
                if (hasNewImages)
                {
                    foreach (var image in tripDto.Images)
                    {
                        if (image.Length > 0)
                        {
                            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                            string filePath = Path.Combine(imagesDirectory, fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                image.CopyTo(stream);
                            }

                            unitOFWork.TripImages.Addone(new TripImage
                            {
                                TripId = tripDto.TripId,
                                ImageName = fileName
                            });
                        }
                    }
                }

                unitOFWork.Compelet();
                return Ok(new { message = "Trip updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the trip", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTrip([FromRoute] string id)
        {
            var trip = unitOFWork.Trips.Findme(e => e.TripId == id, new[] { "TripImages" });
            if (trip == null) return BadRequest(new { message = "Trip not found." });

            // Delete images from filesystem
            string imagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            foreach (var img in trip.TripImages)
            {
                string fullPath = Path.Combine(imagesDirectory, img.ImageName);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }

            unitOFWork.TripImages.DeleteRange(trip.TripImages);
            unitOFWork.Trips.Delete(trip);
            unitOFWork.Compelet();

            return Ok(new { message = "Trip deleted successfully" });
        }

        [HttpDelete("by-name/{name}")]
        public IActionResult DeleteTripByName([FromRoute] string name)
        {
            // Find the trip by name (you may want to make it case-insensitive or exact match based on your need)
            var trip = unitOFWork.Trips.Findme(
                e => e.Name.ToLower() == name.ToLower(),
                new[] { "TripImages" });

            if (trip == null)
                return BadRequest(new { message = "Trip not found." });

            // Delete images from filesystem
            string imagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            foreach (var img in trip.TripImages)
            {
                string fullPath = Path.Combine(imagesDirectory, img.ImageName);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }

            // Remove related images and the trip itself
            unitOFWork.TripImages.DeleteRange(trip.TripImages);
            unitOFWork.Trips.Delete(trip);
            unitOFWork.Compelet();

            return Ok(new { message = "Trip deleted successfully by name." });
        }

    }

}